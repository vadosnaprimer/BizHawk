﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizHawk.Common;
using BizHawk.Emulation.Common;
using System.Runtime.InteropServices;
using System.IO;
using System.ComponentModel;

namespace BizHawk.Emulation.Cores.Nintendo.GBA
{
	[CoreAttributes("mGBA", "endrift", true, true, "0.4.0", "https://mgba.io/", false)]
	[ServiceNotApplicable(typeof(IDriveLight), typeof(IRegionable))]
	public class MGBAHawk : IEmulator, IVideoProvider, ISyncSoundProvider, IGBAGPUViewable, ISaveRam, IStatable, IInputPollable, ISettable<MGBAHawk.Settings, MGBAHawk.SyncSettings>
	{
		IntPtr core;

		[CoreConstructor("GBA")]
		public MGBAHawk(byte[] file, CoreComm comm, SyncSettings syncSettings, Settings settings, bool deterministic)
		{
			_syncSettings = syncSettings ?? new SyncSettings();
			_settings = settings ?? new Settings();
			DeterministicEmulation = deterministic;

			byte[] bios = comm.CoreFileProvider.GetFirmware("GBA", "Bios", false);
			DeterministicEmulation &= bios != null;

			if (DeterministicEmulation != deterministic)
			{
				throw new InvalidOperationException("A BIOS is required for deterministic recordings!");
			}
			if (!DeterministicEmulation && bios != null && !_syncSettings.RTCUseRealTime && !_syncSettings.SkipBios)
			{
				// in these situations, this core is deterministic even though it wasn't asked to be
				DeterministicEmulation = true;
			}

			if (bios != null && bios.Length != 16384)
			{
				throw new InvalidOperationException("BIOS must be exactly 16384 bytes!");
			}
			core = LibmGBA.BizCreate(bios);
			if (core == IntPtr.Zero)
			{
				throw new InvalidOperationException("BizCreate() returned NULL!  Bad BIOS?");
			}
			try
			{
				if (!LibmGBA.BizLoad(core, file, file.Length))
				{
					throw new InvalidOperationException("BizLoad() returned FALSE!  Bad ROM?");
				}

				if (!DeterministicEmulation && _syncSettings.SkipBios)
				{
					LibmGBA.BizSkipBios(core);
				}


				CreateMemoryDomains(file.Length);
				var ser = new BasicServiceProvider(this);
				ser.Register<IDisassemblable>(new ArmV4Disassembler());
				ser.Register<IMemoryDomains>(MemoryDomains);

				ServiceProvider = ser;
				CoreComm = comm;

				CoreComm.VsyncNum = 262144;
				CoreComm.VsyncDen = 4389;
				CoreComm.NominalWidth = 240;
				CoreComm.NominalHeight = 160;

				InitStates();
			}
			catch
			{
				LibmGBA.BizDestroy(core);
				throw;
			}
		}

		MemoryDomainList MemoryDomains;

		public IEmulatorServiceProvider ServiceProvider { get; private set; }
		public ControllerDefinition ControllerDefinition { get { return GBA.GBAController; } }
		public IController Controller { get; set; }

		public void FrameAdvance(bool render, bool rendersound = true)
		{
			Frame++;
			if (Controller["Power"])
			{
				LibmGBA.BizReset(core);
				//BizReset caused memorydomain pointers to change.
				WireMemoryDomainPointers();
			}

			IsLagFrame = LibmGBA.BizAdvance(core, VBANext.GetButtons(Controller), videobuff, ref nsamp, soundbuff,
				RTCTime(),
				(short)Controller.GetFloat("Tilt X"),
				(short)Controller.GetFloat("Tilt Y"),
				(short)Controller.GetFloat("Tilt Z"),
				(byte)(255 - Controller.GetFloat("Light Sensor")));

			if (IsLagFrame)
				LagCount++;
			// this should be called in hblank on the appropriate line, but until we implement that, just do it here
			if (_scanlinecb != null)
				_scanlinecb();
		}

		public int Frame { get; private set; }

		public string SystemId { get { return "GBA"; } }

		public bool DeterministicEmulation { get; private set; }

		public string BoardName { get { return null; } }

		public void ResetCounters()
		{
			Frame = 0;
			LagCount = 0;
			IsLagFrame = false;
		}

		public CoreComm CoreComm { get; private set; }

		public void Dispose()
		{
			if (core != IntPtr.Zero)
			{
				LibmGBA.BizDestroy(core);
				core = IntPtr.Zero;
			}
		}

		#region IVideoProvider
		public int VirtualWidth { get { return 240; } }
		public int VirtualHeight { get { return 160; } }
		public int BufferWidth { get { return 240; } }
		public int BufferHeight { get { return 160; } }
		public int BackgroundColor
		{
			get { return unchecked((int)0xff000000); }
		}
		public int[] GetVideoBuffer()
		{
			return videobuff;
		}
		private readonly int[] videobuff = new int[240 * 160];
		#endregion

		#region ISoundProvider
		private readonly short[] soundbuff = new short[2048];
		private int nsamp;
		public void GetSamples(out short[] samples, out int nsamp)
		{
			nsamp = this.nsamp;
			samples = soundbuff;
			DiscardSamples();
		}
		public void DiscardSamples()
		{
			nsamp = 0;
		}
		public ISoundProvider SoundProvider { get { throw new InvalidOperationException(); } }
		public ISyncSoundProvider SyncSoundProvider { get { return this; } }
		public bool StartAsyncSound() { return false; }
		public void EndAsyncSound() { }
		#endregion

		#region IMemoryDomains

		unsafe byte PeekWRAM(IntPtr xwram, long addr) { return ((byte*)xwram.ToPointer())[addr];}
		unsafe void PokeWRAM(IntPtr xwram, long addr, byte value) { ((byte*)xwram.ToPointer())[addr] = value; }

		void WireMemoryDomainPointers()
		{
			var s = new LibmGBA.MemoryAreas();
			LibmGBA.BizGetMemoryAreas(core, s);

			var LE = MemoryDomain.Endian.Little;
			MemoryDomains["IWRAM"].SetDelegatesForIntPtr(MemoryDomains["IWRAM"].Size, LE, s.wram, true, 4);
			MemoryDomains["EWRAM"].SetDelegatesForIntPtr(MemoryDomains["EWRAM"].Size, LE, s.wram, true, 4);
			MemoryDomains["BIOS"].SetDelegatesForIntPtr(MemoryDomains["BIOS"].Size, LE, s.bios, false, 4);
			MemoryDomains["PALRAM"].SetDelegatesForIntPtr(MemoryDomains["PALRAM"].Size, LE, s.palram, false, 4);
			MemoryDomains["VRAM"].SetDelegatesForIntPtr(MemoryDomains["VRAM"].Size, LE, s.vram, true, 4);
			MemoryDomains["OAM"].SetDelegatesForIntPtr(MemoryDomains["OAM"].Size, LE, s.oam, false, 4);
			MemoryDomains["ROM"].SetDelegatesForIntPtr(MemoryDomains["ROM"].Size, LE, s.rom, false, 4);

			// special combined ram memory domain
			MemoryDomains["Combined WRAM"].SetPeekPokeDelegates(
				delegate(long addr)
				{
					LibmGBA.BizGetMemoryAreas(core, s);
					if (addr < 0 || addr >= (256 + 32) * 1024)
						throw new IndexOutOfRangeException();
					if (addr >= 256 * 1024)
						return PeekWRAM(s.iwram, addr & 32767);
					else
						return PeekWRAM(s.wram, addr);
				},
				delegate(long addr, byte val)
				{
					if (addr < 0 || addr >= (256 + 32) * 1024)
						throw new IndexOutOfRangeException();
					if (addr >= 256 * 1024)
						PokeWRAM(s.iwram, addr & 32767, val);
					else
						PokeWRAM(s.wram, addr, val);
				}
			);

			_gpumem = new GBAGPUMemoryAreas
			{
				mmio = s.mmio,
				oam = s.oam,
				palram = s.palram,
				vram = s.vram
			};

		}

		private void CreateMemoryDomains(int romsize)
		{
			var LE = MemoryDomain.Endian.Little;

			var mm = new List<MemoryDomain>();
			mm.Add(new MemoryDomain("IWRAM", 32 * 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("EWRAM", 256 * 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("BIOS", 16 * 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("PALRAM", 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("VRAM", 96 * 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("OAM", 1024, LE, null, null, 4));
			mm.Add(new MemoryDomain("ROM", romsize, LE, null, null, 4));
			mm.Add(new MemoryDomain("Combined WRAM", (256 + 32) * 1024, LE, null, null, 4));

			MemoryDomains = new MemoryDomainList(mm);
			WireMemoryDomainPointers();
		}

		#endregion

		private Action _scanlinecb;

		private GBAGPUMemoryAreas _gpumem;

		public GBAGPUMemoryAreas GetMemoryAreas()
		{
			return _gpumem;
		}

		[FeatureNotImplemented]
		public void SetScanlineCallback(Action callback, int scanline)
		{
			_scanlinecb = callback;
		}

		#region ISaveRam

		public byte[] CloneSaveRam()
		{
			byte[] ret = new byte[LibmGBA.BizGetSaveRamSize(core)];
			if (ret.Length > 0)
			{
				LibmGBA.BizGetSaveRam(core, ret);
				return ret;
			}
			else
			{
				return null;
			}
		}

		private static byte[] LegacyFix(byte[] saveram)
		{
			// at one point vbanext-hawk had a special saveram format which we want to load.
			var br = new BinaryReader(new MemoryStream(saveram, false));
			br.ReadBytes(8); // header;
			int flashSize = br.ReadInt32();
			int eepromsize = br.ReadInt32();
			byte[] flash = br.ReadBytes(flashSize);
			byte[] eeprom = br.ReadBytes(eepromsize);
			if (flash.Length == 0)
				return eeprom;
			else if (eeprom.Length == 0)
				return flash;
			else
			{
				// well, isn't this a sticky situation!
				return flash; // woops
			}
		}

		public void StoreSaveRam(byte[] data)
		{
			if (data.Take(8).SequenceEqual(Encoding.ASCII.GetBytes("GBABATT\0")))
			{
				data = LegacyFix(data);
			}

			int len = LibmGBA.BizGetSaveRamSize(core);
			if (len > data.Length)
			{
				byte[] _tmp = new byte[len];
				Array.Copy(data, _tmp, data.Length);
				for (int i = data.Length; i < len; i++)
					_tmp[i] = 0xff;
				data = _tmp;
			}
			else if (len < data.Length)
			{
				// we could continue from this, but we don't expect it
				throw new InvalidOperationException("Saveram will be truncated!");
			}
			LibmGBA.BizPutSaveRam(core, data);
		}

		public bool SaveRamModified
		{
			get { return LibmGBA.BizGetSaveRamSize(core) > 0; }
		}

		#endregion

		private void InitStates()
		{
			savebuff = new byte[LibmGBA.BizGetStateMaxSize(core)];
			savebuff2 = new byte[savebuff.Length + 13];
		}

		private byte[] savebuff;
		private byte[] savebuff2;

		public bool BinarySaveStatesPreferred
		{
			get { return true; }
		}

		public void SaveStateText(TextWriter writer)
		{
			var tmp = SaveStateBinary();
			BizHawk.Common.BufferExtensions.BufferExtensions.SaveAsHexFast(tmp, writer);
		}
		public void LoadStateText(TextReader reader)
		{
			string hex = reader.ReadLine();
			byte[] state = new byte[hex.Length / 2];
			BizHawk.Common.BufferExtensions.BufferExtensions.ReadFromHexFast(state, hex);
			LoadStateBinary(new BinaryReader(new MemoryStream(state)));
		}

		public void SaveStateBinary(BinaryWriter writer)
		{
			int size = LibmGBA.BizGetState(core, savebuff, savebuff.Length);
			if (size < 0)
				throw new InvalidOperationException("Core failed to save!");
			writer.Write(size);
			writer.Write(savebuff, 0, size);

			// other variables
			writer.Write(IsLagFrame);
			writer.Write(LagCount);
			writer.Write(Frame);
		}

		public void LoadStateBinary(BinaryReader reader)
		{
			int length = reader.ReadInt32();
			if (length > savebuff.Length)
			{
				savebuff = new byte[length];
				savebuff2 = new byte[length + 13];
			}
			reader.Read(savebuff, 0, length);
			if (!LibmGBA.BizPutState(core, savebuff, length))
				throw new InvalidOperationException("Core rejected the savestate!");

			// other variables
			IsLagFrame = reader.ReadBoolean();
			LagCount = reader.ReadInt32();
			Frame = reader.ReadInt32();
		}

		public byte[] SaveStateBinary()
		{
			var ms = new MemoryStream(savebuff2, true);
			var bw = new BinaryWriter(ms);
			SaveStateBinary(bw);
			bw.Flush();
			ms.Close();
			return savebuff2;
		}

		public int LagCount { get; set; }
		public bool IsLagFrame { get; set; }

		[FeatureNotImplemented]
		public IInputCallbackSystem InputCallbacks
		{
			get { throw new NotImplementedException(); }
		}

		private long RTCTime()
		{
			if (!DeterministicEmulation && _syncSettings.RTCUseRealTime)
			{
				return (long)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			}
			long basetime = (long)_syncSettings.RTCInitialTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
			long increment = Frame * 4389L >> 18;
			return basetime + increment;
		}

		public Settings GetSettings()
		{
			return _settings.Clone();
		}

		public bool PutSettings(Settings o)
		{
			LibmGBA.Layers mask = 0;
			if (o.DisplayBG0) mask |= LibmGBA.Layers.BG0;
			if (o.DisplayBG1) mask |= LibmGBA.Layers.BG1;
			if (o.DisplayBG2) mask |= LibmGBA.Layers.BG2;
			if (o.DisplayBG3) mask |= LibmGBA.Layers.BG3;
			if (o.DisplayOBJ) mask |= LibmGBA.Layers.OBJ;
			LibmGBA.BizSetLayerMask(core, mask);
			_settings = o;
			return false;
		}

		private Settings _settings;

		public class Settings
		{
			[DefaultValue(true)]
			public bool DisplayBG0 { get; set; }
			[DefaultValue(true)]
			public bool DisplayBG1 { get; set; }
			[DefaultValue(true)]
			public bool DisplayBG2 { get; set; }
			[DefaultValue(true)]
			public bool DisplayBG3 { get; set; }
			[DefaultValue(true)]
			public bool DisplayOBJ { get; set; }

			public Settings Clone()
			{
				return (Settings)MemberwiseClone();
			}

			public Settings()
			{
				SettingsUtil.SetDefaultValues(this);
			}
		}

		public SyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public bool PutSyncSettings(SyncSettings o)
		{
			bool ret = SyncSettings.NeedsReboot(o, _syncSettings);
			_syncSettings = o;
			return ret;
		}

		private SyncSettings _syncSettings;

		public class SyncSettings
		{
			[DisplayName("Skip BIOS")]
			[Description("Skips the BIOS intro.  Not applicable when a BIOS is not provided.")]
			[DefaultValue(true)]
			public bool SkipBios { get; set; }

			[DisplayName("RTC Use Real Time")]
			[Description("Causes the internal clock to reflect your system clock.  Only relevant when a game has an RTC chip.  Forced to false for movie recording.")]
			[DefaultValue(true)]
			public bool RTCUseRealTime { get; set; }

			[DisplayName("RTC Initial Time")]
			[Description("The initial time of emulation.  Only relevant when a game has an RTC chip and \"RTC Use Real Time\" is false.")]
			[DefaultValue(typeof(DateTime), "2010-01-01")]
			public DateTime RTCInitialTime { get; set; }

			public SyncSettings()
			{
				SettingsUtil.SetDefaultValues(this);
			}

			public static bool NeedsReboot(SyncSettings x, SyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y);
			}

			public SyncSettings Clone()
			{
				return (SyncSettings)MemberwiseClone();
			}
		}
	}
}
