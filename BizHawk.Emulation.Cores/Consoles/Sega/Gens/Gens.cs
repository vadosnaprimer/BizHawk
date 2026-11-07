using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Consoles.Sega.Gens
{
	[CoreAttributes("Gens", "authors")]
	public class Gens : IEmulator, IInputPollable, IVideoProvider, IDebuggable
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
					"P1 Up", "P1 Down", "P1 Left", "P1 Right", "P1 Start", "P1 A", "P1 B", "P1 C", "P1 X", "P1 Y", "P1 Z", "P1 Mode",
					"P2 Up", "P2 Down", "P2 Left", "P2 Right", "P2 Start", "P2 A", "P2 B", "P2 C", "P2 X", "P2 Y", "P2 Z", "P2 Mode"
				}
			};

			SetMemoryDomains();
			InitMemCallbacks();
			KillMemCallbacks();
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
		public int BackgroundColor => 0x000000;
		public bool HasSystemBus => true;
		public int[] GetVideoBuffer() => _videobuff;
		private readonly int[] _videobuff = new int[320 * 224];
		private IMemoryDomains MemoryDomains;

		public void FrameAdvance(IController controller, bool render, bool renderSound = true)
		{
			int input = 0;

			for (int player = 1; player <= 2; player++)
			{
				foreach (var b in Enum.GetValues(typeof(LibGens.Buttons)))
				{
					if (controller.IsPressed($"P{player} {Enum.GetName(typeof(LibGens.Buttons), b)}"))
					{
						input |= (int)b << ((player - 1) * 16);
					}
				}
			}

			Frame++;
			IsLagFrame = LibGens.gens_advance(_videobuff, input);

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

		IInputCallbackSystem IInputPollable.InputCallbacks => throw new NotImplementedException();
		public int TotalExecutedCycles => throw new NotImplementedException();
		public object GetSettings() => throw new NotImplementedException();
		public object GetSyncSettings() => throw new NotImplementedException();
		public bool PutSettings(object o) => throw new NotImplementedException();
		public bool PutSyncSettings(object o) => throw new NotImplementedException();
		public void SetCpuRegister(string register, int value) { throw new NotImplementedException(); }
		public void Step(StepType type) { throw new NotImplementedException(); }
		public bool CanStep(StepType type) { return false; }

		private unsafe void SetMemoryDomains()
		{
			var mm = new List<MemoryDomain>();

			var m68Bus = new MemoryDomainDelegate("M68K BUS", 0x1000000, MemoryDomain.Endian.Big,
				delegate (long addr)
				{
					var a = (uint)addr;
					if (a >= 0x1000000)
						throw new ArgumentOutOfRangeException();
					return LibGens.gens_peek_m68k(a);
				}, null, 2);
			mm.Add(m68Bus);

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
			MemoryDomains.SystemBus = m68Bus;

			(ServiceProvider as BasicServiceProvider).Register<IMemoryDomains>(MemoryDomains);
		}

		public IMemoryCallbackSystem MemoryCallbacks
		{
			get { return _memoryCallbacks; }
		}
		private readonly MemoryCallbackSystem _memoryCallbacks = new MemoryCallbackSystem();

		private LibGens.mem_cb ExecCallback;
		private LibGens.mem_cb ReadCallback;
		private LibGens.mem_cb WriteCallback;

		private void InitMemCallbacks()
		{
			ExecCallback = new LibGens.mem_cb(a => MemoryCallbacks.CallExecutes(a));
			ReadCallback = new LibGens.mem_cb(a => MemoryCallbacks.CallReads(a));
			WriteCallback = new LibGens.mem_cb(a => MemoryCallbacks.CallWrites(a));
			_memoryCallbacks.ActiveChanged += RefreshMemCallbacks;
		}

		private void RefreshMemCallbacks()
		{
			LibGens.gens_set_mem_callback(
				MemoryCallbacks.HasReads ? ReadCallback : null,
				MemoryCallbacks.HasWrites ? WriteCallback : null,
				MemoryCallbacks.HasExecutes ? ExecCallback : null);
		}

		private void KillMemCallbacks()
		{
			LibGens.gens_set_mem_callback(null, null, null);
		}
		public IDictionary<string, RegisterValue> GetCpuFlagsAndRegisters()
		{
			LibGens.RegisterInfo[] regs = new LibGens.RegisterInfo[18];

			int n = LibGens.gens_getregs(regs);
			if (n > regs.Length)
				throw new InvalidOperationException("A buffer overrun has occured!");
			var ret = new Dictionary<string, RegisterValue>();
			for (int i = 0; i < n; i++)
			{
				// el hacko
				string name = Marshal.PtrToStringAnsi(regs[i].Name);
				byte size = 32;
				if (name.Contains("68K SR"))
					size = 16;

				ret[name] = new RegisterValue((ulong)regs[i].Value, size);
			}

			return ret;
		}


	}
}