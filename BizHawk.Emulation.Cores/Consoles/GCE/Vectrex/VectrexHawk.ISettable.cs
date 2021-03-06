﻿using System;
using System.ComponentModel;

using Newtonsoft.Json;

using BizHawk.Common;
using BizHawk.Emulation.Common;

namespace BizHawk.Emulation.Cores.Consoles.Vectrex
{
	public partial class VectrexHawk : IEmulator, IStatable, ISettable<VectrexHawk.VectrexSettings, VectrexHawk.VectrexSyncSettings>
	{
		public VectrexSettings GetSettings()
		{
			return _settings.Clone();
		}

		public VectrexSyncSettings GetSyncSettings()
		{
			return _syncSettings.Clone();
		}

		public bool PutSettings(VectrexSettings o)
		{
			_settings = o;
			return false;
		}

		public bool PutSyncSettings(VectrexSyncSettings o)
		{
			bool ret = VectrexSyncSettings.NeedsReboot(_syncSettings, o);
			_syncSettings = o;
			return ret;
		}

		private VectrexSettings _settings = new VectrexSettings();
		public VectrexSyncSettings _syncSettings = new VectrexSyncSettings();

		public class VectrexSettings
		{

			public VectrexSettings Clone()
			{
				return (VectrexSettings)MemberwiseClone();
			}
		}

		public class VectrexSyncSettings
		{
			[JsonIgnore]
			public string Port1 = VectrexHawkControllerDeck.DefaultControllerName;

			public enum ControllerType
			{
				Default,
			}

			[JsonIgnore]
			private ControllerType _VectrexController;

			[DisplayName("Controller")]
			[Description("Select Controller Type")]
			[DefaultValue(ControllerType.Default)]
			public ControllerType VectrexController
			{
				get { return _VectrexController; }
				set
				{
					if (value == ControllerType.Default) { Port1 = VectrexHawkControllerDeck.DefaultControllerName; }
					else { Port1 = VectrexHawkControllerDeck.DefaultControllerName; }

					_VectrexController = value;
				}
			}

			[DisplayName("Use Existing SaveRAM")]
			[Description("When true, existing SaveRAM will be loaded at boot up")]
			[DefaultValue(false)]
			public bool Use_SRAM { get; set; }

			public VectrexSyncSettings Clone()
			{
				return (VectrexSyncSettings)MemberwiseClone();
			}

			public static bool NeedsReboot(VectrexSyncSettings x, VectrexSyncSettings y)
			{
				return !DeepEquality.DeepEquals(x, y);
			}
		}
	}
}
