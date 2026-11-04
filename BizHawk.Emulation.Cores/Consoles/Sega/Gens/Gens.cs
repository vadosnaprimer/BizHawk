using System;
using System.Collections;
using System.Collections.Generic;

using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Consoles.Sega.Gens
{
	[CoreAttributes("Gens", "authors")]
	public class Gens : IEmulator, IInputPollable, IVideoProvider
	{
		[CoreConstructor("GEN")]
		public Gens(CoreComm comm, byte[] file, object settings, object syncSettings, GameInfo game)
		{
			int result = LibGens.gens_init(file, file.Length);
			ServiceProvider = new BasicServiceProvider(this);
			CoreComm = comm;

			ControllerDefinition = new ControllerDefinition
			{
				Name = "Gens Controller",
				BoolButtons =
				{
					"Up", "Down", "Left", "Right", "Start", "A", "B", "C"
				}
			};

			SetMemoryDomains();
		}
		
		public IEmulatorServiceProvider ServiceProvider { get; }
		public ControllerDefinition ControllerDefinition { get; private set; }
		public CoreComm CoreComm { get; private set; }
		public int Frame { get; private set; }
		public string SystemId => "GEN";
		public bool DeterministicEmulation => true;
		public int LagCount { get; set; }
		public bool IsLagFrame { get; set; }
		public int VirtualWidth => 224*4/3;
		public int VirtualHeight => 224;
		public int BufferWidth => 320;
		public int BufferHeight => 224;
		public int VsyncNumerator => 60;
		public int VsyncDenominator => 1;
		public int BackgroundColor => 0x0000ff;
		public bool HasSystemBus => false;
		public int[] GetVideoBuffer() => _videobuff;

		IInputCallbackSystem IInputPollable.InputCallbacks => throw new NotImplementedException();

		private readonly int[] _videobuff = new int[320 * 224];
		private IMemoryDomains MemoryDomains;

		public void FrameAdvance(IController controller, bool render, bool renderSound = true)
		{
			Frame++;

			IsLagFrame = LibGens.gens_advance(_videobuff);
			//UpdateVideo();
			//update_audio();

			if (IsLagFrame)
				LagCount++;
		}

		public void ResetCounters()
		{
			Frame = 0;
		}

		public void Dispose()
		{

		}

		public object GetSettings() => throw new NotImplementedException();
		public object GetSyncSettings() => throw new NotImplementedException();
		public bool PutSettings(object o) => throw new NotImplementedException();
		public bool PutSyncSettings(object o) => throw new NotImplementedException();

		private unsafe void SetMemoryDomains()
		{
			var mm = new List<MemoryDomain>();

			var m68k = new MemoryDomainDelegate("M68K", 0x1000000, MemoryDomain.Endian.Big,
				delegate (long addr)
				{
					var a = (uint)addr;
					if (a >= 0x1000000)
						throw new ArgumentOutOfRangeException();
					return LibGens.gens_peek_m68k(a);
				}, null, 2);
			mm.Add(m68k);

			var rom = new MemoryDomainDelegate("ROM", 0x600000, MemoryDomain.Endian.Little,
				delegate (long addr)
				{
					var a = (uint)addr;
					if (a >= 0x600000)
						throw new ArgumentOutOfRangeException();
					return LibGens.gens_peek_rom(a);
				}, null, 2);
			mm.Add(rom);

			MemoryDomains = new MemoryDomainList(mm);

			(ServiceProvider as BasicServiceProvider).Register<IMemoryDomains>(MemoryDomains);
		}
	}
}