using BizHawk.Emulation.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BizHawk.Emulation.Cores.Nintendo.Dolphin
{
	public partial class Dolphin : IDebuggable
	{
		[FeatureNotImplemented]
		public IDictionary<string, RegisterValue> GetCpuFlagsAndRegisters()
		{
			throw new NotImplementedException();
		}

		[FeatureNotImplemented]
		public void SetCpuRegister(string register, int value)
		{
			throw new NotImplementedException();
		}

		[FeatureNotImplemented]
		public IMemoryCallbackSystem MemoryCallbacks { get; }

		public bool CanStep(StepType type) { return false; }

		[FeatureNotImplemented]
		public void Step(StepType type) { throw new NotImplementedException(); }

		public long TotalExecutedCycles => (long)_core.Dolphin_GetTicks();
	}
}
