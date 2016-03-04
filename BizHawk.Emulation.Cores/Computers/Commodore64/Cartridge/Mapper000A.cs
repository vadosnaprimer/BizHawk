﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizHawk.Emulation.Cores.Computers.Commodore64.Cartridge
{
    //792

    // Epyx Fastload. Uppermost page is always visible at DFxx.
    // They use a capacitor that is recharged by accesses to DExx
    // to pull down EXROM.

    public abstract partial class CartridgeDevice
    {
        private class Mapper000A : CartridgeDevice
        {
            // This constant differs depending on whose research you reference. TODO: Verify.
            [SaveState.DoNotSave]
            private const int RESET_CAPACITOR_CYCLES = 512;

            [SaveState.SaveWithName("CapacitorCycles")]
            private int _capacitorCycles;
            [SaveState.DoNotSave]
            private readonly int[] _rom;

            public Mapper000A(IList<int> newAddresses, IList<int> newBanks, IList<int[]> newData)
            {
                _rom = new int[0x2000];
                Array.Copy(newData.First(), _rom, 0x2000);
                pinGame = true;
            }

            public override void ExecutePhase()
            {
                pinExRom = !(_capacitorCycles > 0);
                if (!pinExRom)
                {
                    _capacitorCycles--;
                }
            }

            public override void HardReset()
            {
                _capacitorCycles = RESET_CAPACITOR_CYCLES;
                base.HardReset();
            }

            public override int Peek8000(int addr)
            {
                return _rom[addr & 0x1FFF];
            }

            public override int PeekDE00(int addr)
            {
                return 0x00;
            }

            public override int PeekDF00(int addr)
            {
                return _rom[(addr & 0xFF) | 0x1F00];
            }

            public override int Read8000(int addr)
            {
                _capacitorCycles = RESET_CAPACITOR_CYCLES;
                return _rom[addr & 0x1FFF];
            }

            public override int ReadDE00(int addr)
            {
                _capacitorCycles = RESET_CAPACITOR_CYCLES;
                return 0x00;
            }

            public override int ReadDF00(int addr)
            {
                return _rom[(addr & 0xFF) | 0x1F00];
            }
        }
    }

}
