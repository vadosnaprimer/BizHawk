﻿using System;
using System.Collections.Generic;
using BizHawk.Common;
using BizHawk.Emulation.Common;
using System.Numerics;

namespace BizHawk.Emulation.Cores.Atari.Atari2600
{
	// Emulates the TIA
	public partial class TIA : IVideoProvider, ISoundProvider
	{

		#region palette

		const int BackColor = unchecked((int)0xff000000);

		static TIA()
		{
			// add alpha to palette entries
			for (int i = 0; i < PALPalette.Length; i++)
				PALPalette[i] |= unchecked((int)0xff000000);
			for (int i = 0; i < NTSCPalette.Length; i++)
				NTSCPalette[i] |= unchecked((int)0xff000000);
		}

		private static readonly int[] PALPalette =
		{
			0x000000, 0x000000, 0x2b2b2b, 0x2b2b2b,
            0x525252, 0x525252, 0x767676, 0x767676,
            0x979797, 0x979797, 0xb6b6b6, 0xb6b6b6,
            0xd2d2d2, 0xd2d2d2, 0xececec, 0xececec,

            0x000000, 0x000000, 0x2b2b2b, 0x2b2b2b,
            0x525252, 0x525252, 0x767676, 0x767676,
            0x979797, 0x979797, 0xb6b6b6, 0xb6b6b6,
            0xd2d2d2, 0xd2d2d2, 0xececec, 0xececec,

            0x805800, 0x000000, 0x96711a, 0x2b2b2b,
            0xab8732, 0x525252, 0xbe9c48, 0x767676,
            0xcfaf5c, 0x979797, 0xdfc06f, 0xb6b6b6,
            0xeed180, 0xd2d2d2, 0xfce090, 0xececec,

            0x445c00, 0x000000, 0x5e791a, 0x2b2b2b,
            0x769332, 0x525252, 0x8cac48, 0x767676,
            0xa0c25c, 0x979797, 0xb3d76f, 0xb6b6b6,
            0xc4ea80, 0xd2d2d2, 0xd4fc90, 0xececec,

            0x703400, 0x000000, 0x89511a, 0x2b2b2b,
            0xa06b32, 0x525252, 0xb68448, 0x767676,
            0xc99a5c, 0x979797, 0xdcaf6f, 0xb6b6b6,
            0xecc280, 0xd2d2d2, 0xfcd490, 0xececec,

            0x006414, 0x000000, 0x1a8035, 0x2b2b2b,
            0x329852, 0x525252, 0x48b06e, 0x767676,
            0x5cc587, 0x979797, 0x6fd99e, 0xb6b6b6,
            0x80ebb4, 0xd2d2d2, 0x90fcc8, 0xececec,

            0x700014, 0x000000, 0x891a35, 0x2b2b2b,
            0xa03252, 0x525252, 0xb6486e, 0x767676,
            0xc95c87, 0x979797, 0xdc6f9e, 0xb6b6b6,
            0xec80b4, 0xd2d2d2, 0xfc90c8, 0xececec,

            0x005c5c, 0x000000, 0x1a7676, 0x2b2b2b,
            0x328e8e, 0x525252, 0x48a4a4, 0x767676,
            0x5cb8b8, 0x979797, 0x6fcbcb, 0xb6b6b6,
            0x80dcdc, 0xd2d2d2, 0x90ecec, 0xececec,

            0x70005c, 0x000000, 0x841a74, 0x2b2b2b,
            0x963289, 0x525252, 0xa8489e, 0x767676,
            0xb75cb0, 0x979797, 0xc66fc1, 0xb6b6b6,
            0xd380d1, 0xd2d2d2, 0xe090e0, 0xececec,

            0x003c70, 0x000000, 0x195a89, 0x2b2b2b,
            0x2f75a0, 0x525252, 0x448eb6, 0x767676,
            0x57a5c9, 0x979797, 0x68badc, 0xb6b6b6,
            0x79ceec, 0xd2d2d2, 0x88e0fc, 0xececec,

            0x580070, 0x000000, 0x6e1a89, 0x2b2b2b,
            0x8332a0, 0x525252, 0x9648b6, 0x767676,
            0xa75cc9, 0x979797, 0xb76fdc, 0xb6b6b6,
            0xc680ec, 0xd2d2d2, 0xd490fc, 0xececec,

            0x002070, 0x000000, 0x193f89, 0x2b2b2b,
            0x2f5aa0, 0x525252, 0x4474b6, 0x767676,
            0x578bc9, 0x979797, 0x68a1dc, 0xb6b6b6,
            0x79b5ec, 0xd2d2d2, 0x88c8fc, 0xececec,

            0x340080, 0x000000, 0x4a1a96, 0x2b2b2b,
            0x5f32ab, 0x525252, 0x7248be, 0x767676,
            0x835ccf, 0x979797, 0x936fdf, 0xb6b6b6,
            0xa280ee, 0xd2d2d2, 0xb090fc, 0xececec,

            0x000088, 0x000000, 0x1a1a9d, 0x2b2b2b,
            0x3232b0, 0x525252, 0x4848c2, 0x767676,
            0x5c5cd2, 0x979797, 0x6f6fe1, 0xb6b6b6,
            0x8080ef, 0xd2d2d2, 0x9090fc, 0xececec,

            0x000000, 0x000000, 0x2b2b2b, 0x2b2b2b,
            0x525252, 0x525252, 0x767676, 0x767676,
            0x979797, 0x979797, 0xb6b6b6, 0xb6b6b6,
            0xd2d2d2, 0xd2d2d2, 0xececec, 0xececec,

            0x000000, 0x000000, 0x2b2b2b, 0x2b2b2b,
            0x525252, 0x525252, 0x767676, 0x767676,
            0x979797, 0x979797, 0xb6b6b6, 0xb6b6b6,
            0xd2d2d2, 0xd2d2d2, 0xececec, 0xececec
		};

		private static readonly int[] NTSCPalette =
		{
			0x000000, 0, 0x4a4a4a, 0, 0x6f6f6f, 0, 0x8e8e8e, 0,
			0xaaaaaa, 0, 0xc0c0c0, 0, 0xd6d6d6, 0, 0xececec, 0,
			0x484800, 0, 0x69690f, 0, 0x86861d, 0, 0xa2a22a, 0,
			0xbbbb35, 0, 0xd2d240, 0, 0xe8e84a, 0, 0xfcfc54, 0,
			0x7c2c00, 0, 0x904811, 0, 0xa26221, 0, 0xb47a30, 0,
			0xc3903d, 0, 0xd2a44a, 0, 0xdfb755, 0, 0xecc860, 0,
			0x901c00, 0, 0xa33915, 0, 0xb55328, 0, 0xc66c3a, 0,
			0xd5824a, 0, 0xe39759, 0, 0xf0aa67, 0, 0xfcbc74, 0,
			0x940000, 0, 0xa71a1a, 0, 0xb83232, 0, 0xc84848, 0,
			0xd65c5c, 0, 0xe46f6f, 0, 0xf08080, 0, 0xfc9090, 0,
			0x840064, 0, 0x97197a, 0, 0xa8308f, 0, 0xb846a2, 0,
			0xc659b3, 0, 0xd46cc3, 0, 0xe07cd2, 0, 0xec8ce0, 0,
			0x500084, 0, 0x68199a, 0, 0x7d30ad, 0, 0x9246c0, 0,
			0xa459d0, 0, 0xb56ce0, 0, 0xc57cee, 0, 0xd48cfc, 0,
			0x140090, 0, 0x331aa3, 0, 0x4e32b5, 0, 0x6848c6, 0,
			0x7f5cd5, 0, 0x956fe3, 0, 0xa980f0, 0, 0xbc90fc, 0,
			0x000094, 0, 0x181aa7, 0, 0x2d32b8, 0, 0x4248c8, 0,
			0x545cd6, 0, 0x656fe4, 0, 0x7580f0, 0, 0x8490fc, 0,
			0x001c88, 0, 0x183b9d, 0, 0x2d57b0, 0, 0x4272c2, 0,
			0x548ad2, 0, 0x65a0e1, 0, 0x75b5ef, 0, 0x84c8fc, 0,
			0x003064, 0, 0x185080, 0, 0x2d6d98, 0, 0x4288b0, 0,
			0x54a0c5, 0, 0x65b7d9, 0, 0x75cceb, 0, 0x84e0fc, 0,
			0x004030, 0, 0x18624e, 0, 0x2d8169, 0, 0x429e82, 0,
			0x54b899, 0, 0x65d1ae, 0, 0x75e7c2, 0, 0x84fcd4, 0,
			0x004400, 0, 0x1a661a, 0, 0x328432, 0, 0x48a048, 0,
			0x5cba5c, 0, 0x6fd26f, 0, 0x80e880, 0, 0x90fc90, 0,
			0x143c00, 0, 0x355f18, 0, 0x527e2d, 0, 0x6e9c42, 0,
			0x87b754, 0, 0x9ed065, 0, 0xb4e775, 0, 0xc8fc84, 0,
			0x303800, 0, 0x505916, 0, 0x6d762b, 0, 0x88923e, 0,
			0xa0ab4f, 0, 0xb7c25f, 0, 0xccd86e, 0, 0xe0ec7c, 0,
			0x482c00, 0, 0x694d14, 0, 0x866a26, 0, 0xa28638, 0,
			0xbb9f47, 0, 0xd2b656, 0, 0xe8cc63, 0, 0xfce070, 0
		};

		private static readonly int[] SECAMPalette =
		{
			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,

			0x000000,0x000000,0x2121FF,0x2121FF,			
			0xF03C79,0xF03C79,0xFF50FF,0xFF50FF,			
			0x7FFF00,0x7FFF00,0x7FFFFF,0x7FFFFF,			
			0xFFFF3F,0xFFFF3F,0xffffff,0xffffff,
		};

		#endregion

		// in all cases, the TIA has 228 clocks per scanline
		// the NTSC TIA has a clock rate of 3579575hz
		// the PAL/SECAM TIA has a clock rate of 3546894hz

		private bool _pal;

		public int NominalNumScanlines
		{
			get
			{
				return _pal ? 312 : 262;
			}
		}

		public void GetFrameRate(out int num, out int den)
		{
			// TODO when sound timing is made exact:
			// NTSC refclock is actually 315 / 88 mhz

			int clockrate = _pal ? 3546895 : 3579545;
			int clocksperframe = 228 * NominalNumScanlines;
			int gcd = (int)BigInteger.GreatestCommonDivisor(clockrate, clocksperframe);
			num = clockrate / gcd;
			den = clocksperframe / gcd;
		}

		private const int ScreenWidth = 160;
		private const int MaxScreenHeight = 312;

		private const byte CXP0 = 0x01;
		private const byte CXP1 = 0x02;
		private const byte CXM0 = 0x04;
		private const byte CXM1 = 0x08;
		private const byte CXPF = 0x10;
		private const byte CXBL = 0x20;

		private readonly Atari2600 _core;
		private int[] _scanlinebuffer = new int[ScreenWidth * MaxScreenHeight];

		private int[] _palette;

        private int test_count_p0;
        private int test_count_p1;
        private int test_count_m0;
        private int test_count_m1;
        private int test_count_b;

        private byte pf0_update = 0;
        private byte pf1_update = 0;
        private byte pf2_update = 0;
        private bool pf0_updater = false;
        private bool pf1_updater = false;
        private bool pf2_updater = false;
        private byte pf0_delay_clock = 0;
        private byte pf1_delay_clock = 0;
        private byte pf2_delay_clock = 0;
        private byte pf0_max_delay = 0;
        private byte pf1_max_delay = 0;
        private byte pf2_max_delay = 0;

        private int enam0_delay = 0;
        private int enam1_delay = 0;
        private int enamb_delay = 0;
        private bool enam0_val = false;
        private bool enam1_val = false;
        private bool enamb_val = false;


        private int prg0_delay = 0;
        private int prg1_delay = 0;
        private byte prg0_val = 0;
        private byte prg1_val = 0;

        private bool do_ticks = false;
        private byte _hsyncCnt;
		private int _capChargeStart;
		private bool _capCharging;
		private bool _vblankEnabled;
		private bool _vsyncEnabled;
		private int _CurrentScanLine;
		private int _audioClocks; // not savestated

		private PlayerData _player0;
		private PlayerData _player1;
		private PlayfieldData _playField;
		private HMoveData _hmove;
		private BallData _ball;

		public Audio[] AUD = { new Audio(), new Audio() };

		public TIA(Atari2600 core, bool pal, bool secam)
		{
			_core = core;
			_player0.ScanCnt = 8;
			_player1.ScanCnt = 8;
			_pal = pal;
			SetSECAM(secam);
		}

		public void SetSECAM(bool secam)
		{
			_palette = _pal ? secam ? SECAMPalette : PALPalette : NTSCPalette;
		}

		public int CurrentScanLine
		{
			get { return _CurrentScanLine; }
		}

		public bool IsVBlank
		{
			get { return _vblankEnabled; }
		}

		public bool IsVSync
		{
			get { return _vsyncEnabled; }
		}

		/// <summary>
		/// a count of lines emulated; incremented by the TIA but not used by it
		/// </summary>
		public int LineCount { get; set; }

		/// <summary>
		/// called at the end of a video frame.  used internally
		/// </summary>
		public Action<int> FrameEndCallBack { get; set; }

		public int MaxVolume { get; set; }

		public int VirtualWidth
		{
			// TODO: PAL?
			get
			{
				if (_pal)
				{
					return 320;
				}

				return 275; //  275 comes from NTSC specs and the actual pixel clock of a 2600 TIA
			}
		}

		public int VirtualHeight
		{
			get { return BufferHeight; }
		}

		public int BufferWidth
		{
			get { return ScreenWidth; }
		}

		public int BufferHeight
		{
			get
			{
				if (_pal)
					return _core.Settings.PALBottomLine - _core.Settings.PALTopLine;
				else
					return _core.Settings.NTSCBottomLine - _core.Settings.NTSCTopLine;
			}
		}

		public int BackgroundColor
		{
			get { return _core.Settings.BackgroundColor.ToArgb(); }
		}

		public int[] GetVideoBuffer()
		{
			return FrameBuffer;
		}

		public void Reset()
		{
			_hsyncCnt = 0;
			_capChargeStart = 0;
			_capCharging = false;
			_vblankEnabled = false;
			_vsyncEnabled = false;
			_CurrentScanLine = 0;
			_audioClocks = 0;

			_player0 = new PlayerData();
			_player1 = new PlayerData();
			_playField = new PlayfieldData();
			_hmove = new HMoveData();
			_ball = new BallData();

			_player0.ScanCnt = 8;
			_player1.ScanCnt = 8;
		}

		// Execute TIA cycles
		public void Execute(int cycles)
		{
			// Still ignoring cycles...

            //delay latch to new playfield register
            if (pf0_updater==true)
            {
                pf0_delay_clock++;
                if (pf0_delay_clock > pf0_max_delay)
                {
                    _playField.Grp = (uint)((_playField.Grp & 0x0FFFF) + ((ReverseBits(pf0_update, 8) & 0x0F) << 16));
                    pf0_updater = false;
                }
            }
            if (pf1_updater == true)
            {
                pf1_delay_clock++;
                if (pf1_delay_clock > pf1_max_delay)
                {
                    _playField.Grp = (uint)((_playField.Grp & 0xF00FF) + (pf1_update << 8));
                    pf1_updater = false;
                }
            }
            if (pf2_updater == true)
            {
                pf2_delay_clock++;
                if (pf2_delay_clock > pf2_max_delay)
                {
                    _playField.Grp = (uint)((_playField.Grp & 0xFFF00) + ReverseBits(pf2_update, 8));
                    pf2_updater = false;
                }
            }

            //delay latch to missile enable
            if (enam0_delay>0)
            {
                enam0_delay++;
                if (enam0_delay==3)
                {
                    enam0_delay = 0;
                    _player0.Missile.Enabled = enam0_val;
                }
                
            }

            if (enam1_delay > 0)
            {
                enam1_delay++;
                if (enam1_delay == 3)
                {
                    enam1_delay = 0;
                    _player1.Missile.Enabled = enam1_val;
                }

            }

            // delay latch to ball enable
            if (enamb_delay > 0)
            {
                enamb_delay++;
                if (enamb_delay == 3)
                {
                    enamb_delay = 0;
                    _ball.Enabled = enamb_val;
                }

            }

            // delay latch to player graphics registers
            if (prg0_delay > 0)
            {
                prg0_delay++;
                if (prg0_delay == 3)
                {
                    prg0_delay = 0;
                    _player0.Grp = prg0_val;
                    _player1.Dgrp = _player1.Grp;
                }

            }

            if (prg1_delay > 0)
            {
                prg1_delay++;
                if (prg1_delay == 3)
                {
                    prg1_delay = 0;
                    _player1.Grp = prg1_val;
                    _player0.Dgrp = _player0.Grp;

                    // TODO: Find a game that uses this functionality and test it
                    _ball.Denabled = _ball.Enabled;
                }

            }

            // Reset the RDY flag when we reach hblank
            if (_hsyncCnt <= 0)
			{
				_core.Cpu.RDY = true;
			}

			// Assume we're on the left side of the screen for now
			var rightSide = false;

            // ---- Things that happen only in the drawing section ----
            // TODO: Remove this magic number (17). It depends on the HMOVE
            if ((_hsyncCnt) >= (_hmove.LateHBlankReset ? 76 : 68))
            {
                do_ticks = false;
                
                // TODO: Remove this magic number
                if ((_hsyncCnt / 4) >= 37)
                {
                    rightSide = true;
                }

                // The bit number of the PF data which we want
                int pfBit = ((_hsyncCnt / 4) - 17) % 20;

                // Create the mask for the bit we want
                // Note that bits are arranged 0 1 2 3 4 .. 19
                int pfMask = 1 << (20 - 1 - pfBit);

                // Reverse the mask if on the right and playfield is reflected
                if (rightSide && _playField.Reflect)
                {
                    pfMask = ReverseBits(pfMask, 20);
                }

                // Calculate collisions
                byte collisions = 0x00;

                if ((_playField.Grp & pfMask) != 0)
                {
                    collisions |= CXPF;
                }


                // ---- Player 0 ----
                collisions |= _player0.Tick() ? CXP0 : (byte)0x00;

                // ---- Missile 0 ----
                collisions |= _player0.Missile.Tick() ? CXM0 : (byte)0x00;

                // ---- Player 1 ----
                collisions |= _player1.Tick() ? CXP1 : (byte)0x00;

                // ---- Missile 0 ----
                collisions |= _player1.Missile.Tick() ? CXM1 : (byte)0x00;

                // ---- Ball ----
                collisions |= _ball.Tick() ? CXBL : (byte)0x00;


                // Pick the pixel color from collisions
                int pixelColor = BackColor;
                if (_core.Settings.ShowBG)
                {
                    pixelColor = _palette[_playField.BkColor];
                }

                if ((collisions & CXPF) != 0 && _core.Settings.ShowPlayfield)
                {
                    if (_playField.Score)
                    {
                        if (!rightSide)
                        {
                            pixelColor = _palette[_player0.Color];
                        }
                        else
                        {
                            pixelColor = _palette[_player1.Color];
                        }
                    }
                    else
                    {
                        pixelColor = _palette[_playField.PfColor];
                    }
                }

                if ((collisions & CXBL) != 0)
                {
                    _ball.Collisions |= collisions;
                    if (_core.Settings.ShowBall)
                    {
                        pixelColor = _palette[_playField.PfColor];
                    }
                }

                if ((collisions & CXM1) != 0)
                {
                    _player1.Missile.Collisions |= collisions;
                    if (_core.Settings.ShowMissle2)
                    {
                        pixelColor = _palette[_player1.Color];
                    }
                }

                if ((collisions & CXP1) != 0)
                {
                    _player1.Collisions |= collisions;
                    if (_core.Settings.ShowPlayer2)
                    {
                        pixelColor = _palette[_player1.Color];
                    }
                }

                if ((collisions & CXM0) != 0)
                {
                    _player0.Missile.Collisions |= collisions;
                    if (_core.Settings.ShowMissle1)
                    {
                        pixelColor = _palette[_player0.Color];
                    }
                }

                if ((collisions & CXP0) != 0)
                {
                    _player0.Collisions |= collisions;
                    if (_core.Settings.ShowPlayer1)
                    {
                        pixelColor = _palette[_player0.Color];
                    }
                }

                if (_playField.Score && !_playField.Priority && (collisions & CXPF) != 0 && _core.Settings.ShowPlayfield)
                {
                    pixelColor = !rightSide ? _palette[_player0.Color] : _palette[_player1.Color];
                }

                    if (_playField.Priority && (collisions & CXPF) != 0 && _core.Settings.ShowPlayfield)
                {

                    pixelColor = _palette[_playField.PfColor];

                }

                // Handle vblank
                if (_vblankEnabled)
                {
                    pixelColor = BackColor;
                }

                // Add the pixel to the scanline
                // TODO: Remove this magic number (68)

                int y = _CurrentScanLine;
                // y >= max screen height means lag frame or game crashed, but is a legal situation.
                // either way, there's nothing to display
                if (y < MaxScreenHeight)
                {
                    int x = _hsyncCnt - 68;
                    if (x < 0 || x > 159) // this can't happen, right?
                        throw new Exception(); // TODO
                    _scanlinebuffer[_CurrentScanLine * ScreenWidth + x] = pixelColor;
                }
            } else
            {
                do_ticks = true;
            }

            // if extended HBLank is active, the screen area still needs a color
            if (_hsyncCnt >= 68 && _hsyncCnt < 76 && _hmove.LateHBlankReset)
            {
                int pixelColor = 0;

                // Add the pixel to the scanline
                // TODO: Remove this magic number (68)

                int y = _CurrentScanLine;
                // y >= max screen height means lag frame or game crashed, but is a legal situation.
                // either way, there's nothing to display
                if (y < MaxScreenHeight)
                {
                    int x = _hsyncCnt - 68;
                    if (x < 0 || x > 159) // this can't happen, right?
                        throw new Exception(); // TODO
                    _scanlinebuffer[_CurrentScanLine * ScreenWidth + x] = pixelColor;
                }
            }



                // ---- Things that happen every time ----

                // Handle HMOVE
                if (_hmove.HMoveEnabled)
                {
                    // On the first time, set the latches and counters
                    if (_hmove.HMoveJustStarted)
                    {


                        _hmove.Player0Latch = true;
                        _hmove.Player0Cnt = 0;
                        test_count_p0 = 0;
                        test_count_p1 = 0;
                        test_count_m0 = 0;
                        test_count_m1 = 0;
                        test_count_b = 0;


                        _hmove.Missile0Latch = true;
                        _hmove.Missile0Cnt = 0;

                        _hmove.Player1Latch = true;
                        _hmove.Player1Cnt = 0;

                        _hmove.Missile1Latch = true;
                        _hmove.Missile1Cnt = 0;

                        _hmove.BallLatch = true;
                        _hmove.BallCnt = 0;

                        _hmove.HMoveCnt = 0;

                        _hmove.HMoveJustStarted = false;
                        _hmove.LateHBlankReset = true;
                        _hmove.DecCntEnabled = false;

                    }

                    if (_hmove.DecCntEnabled)
                    {


                        // Actually do stuff only evey 4 pulses
                        if (_hmove.HMoveCnt == 0)
                        {
                            // If the latch is still set
                            if (_hmove.Player0Latch)
                            {
                                // If the move counter still has a bit in common with the HM register
                                if (((15 - _hmove.Player0Cnt) ^ ((_player0.HM & 0x07) | ((~(_player0.HM & 0x08)) & 0x08))) != 0x0F)
                                {
                                // "Clock-Stuffing"
                                if (do_ticks==true)
                                {
                                    _player0.Tick();
                                }
                                

                                    // Increase by 1, max of 15

                                    test_count_p0++;
                                    if (test_count_p0 < 16)
                                    {
                                        _hmove.Player0Cnt++;
                                    }
                                    else
                                    {
                                        _hmove.Player0Cnt = 0;
                                    }

                                    //_hmove.Player0Cnt %= 16;
                                }
                                else
                                {
                                    _hmove.Player0Latch = false;
                                }
                            }

                            if (_hmove.Missile0Latch)
                            {
                               
                                // If the move counter still has a bit in common with the HM register
                                if (((15 - _hmove.Missile0Cnt) ^ ((_player0.Missile.Hm & 0x07) | ((~(_player0.Missile.Hm & 0x08)) & 0x08))) != 0x0F)
                                {
                                // "Clock-Stuffing"
                                if (do_ticks == true)
                                {
                                    _player0.Missile.Tick();
                                }
                                    // Increase by 1, max of 15


                                    test_count_m0++;
                                    if (test_count_m0 < 16)
                                    {
                                        _hmove.Missile0Cnt++;
                                    }
                                    else
                                    {
                                        _hmove.Missile0Cnt = 0;
                                    }
                                    //_hmove.Missile0Cnt %= 16;
                                }
                                else
                                {
                                    _hmove.Missile0Latch = false;
                                    
                                }
                            }

                            if (_hmove.Player1Latch)
                            {
                                // If the move counter still has a bit in common with the HM register
                                if (((15 - _hmove.Player1Cnt) ^ ((_player1.HM & 0x07) | ((~(_player1.HM & 0x08)) & 0x08))) != 0x0F)
                                {
                                // "Clock-Stuffing"
                                if (do_ticks == true)
                                {
                                    _player1.Tick();
                                }
                                    // Increase by 1, max of 15
                                    test_count_p1++;
                                    if (test_count_p1 < 16)
                                    {
                                        _hmove.Player1Cnt++;
                                    }
                                    else
                                    {
                                        _hmove.Player1Cnt = 0;
                                    }
                                    //_hmove.Player1Cnt %= 16;
                                }
                                else
                                {
                                    _hmove.Player1Latch = false;
                                }
                            }

                            if (_hmove.Missile1Latch)
                            {
                                // If the move counter still has a bit in common with the HM register
                                if (((15 - _hmove.Missile1Cnt) ^ ((_player1.Missile.Hm & 0x07) | ((~(_player1.Missile.Hm & 0x08)) & 0x08))) != 0x0F)
                                {
                                // "Clock-Stuffing"
                                if (do_ticks == true)
                                {
                                    _player1.Missile.Tick();
                                }
                                    // Increase by 1, max of 15
                                    test_count_m1++;
                                    if (test_count_m1 < 16)
                                    {
                                        _hmove.Missile1Cnt++;
                                    }
                                    else
                                    {
                                        _hmove.Missile1Cnt = 0;
                                    }
                                    // _hmove.Missile1Cnt %= 16;
                                }
                                else
                                {
                                    _hmove.Missile1Latch = false;
                                }
                            }

                            if (_hmove.BallLatch)
                            {
                                // If the move counter still has a bit in common with the HM register
                                if (((15 - _hmove.BallCnt) ^ ((_ball.HM & 0x07) | ((~(_ball.HM & 0x08)) & 0x08))) != 0x0F)
                                {
                                // "Clock-Stuffing"
                                if (do_ticks == true)
                                {
                                    _ball.Tick();
                                }
                                    // Increase by 1, max of 15
                                    test_count_b++;
                                    if (test_count_b < 16)
                                    {
                                        _hmove.BallCnt++;
                                    }
                                    else
                                    {
                                        _hmove.BallCnt = 0;
                                    }
                                    //_hmove.BallCnt %= 16;
                                }
                                else
                                {
                                    _hmove.BallLatch = false;
                                }
                            }

                            if (!_hmove.Player0Latch && !_hmove.Player1Latch && !_hmove.BallLatch && !_hmove.Missile0Latch && !_hmove.Missile1Latch)
                            {
                                _hmove.HMoveEnabled = false;
                                _hmove.DecCntEnabled = false;
                                _hmove.HMoveDelayCnt = 0;
                            }
                        }

                        _hmove.HMoveCnt++;
                        _hmove.HMoveCnt %= 4;
                    }
                   
                    if (_hmove.HMoveDelayCnt < 5)
                    {
                        _hmove.HMoveDelayCnt++;
                    }

                    if (_hmove.HMoveDelayCnt == 5)
                    {
                        _hmove.HMoveDelayCnt++;
                        _hmove.HMoveCnt = 0;
                        _hmove.DecCntEnabled = true;
                    }
                }
            

			// Increment the hsync counter
			_hsyncCnt++;
			_hsyncCnt %= 228;

			// End of the line? Add it to the buffer!
			if (_hsyncCnt == 0)
			{
				_hmove.LateHBlankReset = false;
				_CurrentScanLine++;
				LineCount++;
				_audioClocks += 2; // TODO: increment this at the appropriate places twice per line
			}
		}

		public int[] FrameBuffer = new int[ScreenWidth * MaxScreenHeight];

		void OutputFrame(int validlines)
		{
			int topLine = _pal ? _core.Settings.PALTopLine : _core.Settings.NTSCTopLine;
			int bottomLine = _pal ? _core.Settings.PALBottomLine : _core.Settings.NTSCBottomLine;

			// if vsync occured unexpectedly early, black out the remainer
			for (; validlines < bottomLine; validlines++)
			{
				for (int i = 0; i < 160; i++)
					_scanlinebuffer[validlines * 160 + i] = BackColor;
			}

			int srcbytes = sizeof(int) * ScreenWidth * topLine;
			int count = bottomLine - topLine; // no +1, as the bottom line number is not inclusive
			count *= sizeof(int) * ScreenWidth;

			Buffer.BlockCopy(_scanlinebuffer, srcbytes, FrameBuffer, 0, count);
		}

		public byte ReadMemory(ushort addr, bool peek)
		{
			var maskedAddr = (ushort)(addr & 0x000F);
			if (maskedAddr == 0x00) // CXM0P
			{
				return (byte)((((_player0.Missile.Collisions & CXP1) != 0) ? 0x80 : 0x00) | (((_player0.Missile.Collisions & CXP0) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x01) // CXM1P
			{
				return (byte)((((_player1.Missile.Collisions & CXP0) != 0) ? 0x80 : 0x00) | (((_player1.Missile.Collisions & CXP1) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x02) // CXP0FB
			{
				return (byte)((((_player0.Collisions & CXPF) != 0) ? 0x80 : 0x00) | (((_player0.Collisions & CXBL) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x03) // CXP1FB
			{
				return (byte)((((_player1.Collisions & CXPF) != 0) ? 0x80 : 0x00) | (((_player1.Collisions & CXBL) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x04) // CXM0FB
			{
				return (byte)((((_player0.Missile.Collisions & CXPF) != 0) ? 0x80 : 0x00) | (((_player0.Missile.Collisions & CXBL) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x05) // CXM1FB
			{
				return (byte)((((_player1.Missile.Collisions & CXPF) != 0) ? 0x80 : 0x00) | (((_player1.Missile.Collisions & CXBL) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x06) // CXBLPF
			{
				return (byte)(((_ball.Collisions & CXPF) != 0) ? 0x80 : 0x00);
			}

			if (maskedAddr == 0x07) // CXPPMM
			{
				return (byte)((((_player0.Collisions & CXP1) != 0) ? 0x80 : 0x00) | (((_player0.Missile.Collisions & CXM1) != 0) ? 0x40 : 0x00));
			}

			if (maskedAddr == 0x08) // INPT0
			{
				// Changing the hard coded value will change the paddle position. The range seems to be roughly 0-56000 according to values from stella
				// 6105 roughly centers the paddle in Breakout
				if (_capCharging && _core.Cpu.TotalExecutedCycles - _capChargeStart >= 6105)
				{
					return 0x80;
				}

				return 0x00;
			}

			if (maskedAddr == 0x0C) // INPT4
			{
				return (byte)((_core.ReadControls1(peek) & 0x08) != 0 ? 0x80 : 0x00);
			}

			if (maskedAddr == 0x0D) // INPT5
			{
				return (byte)((_core.ReadControls2(peek) & 0x08) != 0 ? 0x80 : 0x00);
			}

			return 0x00;
		}

		public void WriteMemory(ushort addr, byte value)
		{
			var maskedAddr = (ushort)(addr & 0x3f);

			if (maskedAddr == 0x00) // VSYNC
			{
				if ((value & 0x02) != 0)
				{
					// Frame is complete, output to buffer
					_vsyncEnabled = true;
				}
				else if (_vsyncEnabled)
				{
					// When VSYNC is disabled, this will be the first line of the new frame

					// write to frame buffer
					OutputFrame(_CurrentScanLine);

					if (FrameEndCallBack != null)
						FrameEndCallBack(_CurrentScanLine);

					// Clear all from last frame
					_CurrentScanLine = 0;

					// Frame is done
					_vsyncEnabled = false;

					// Do not reset hsync, since we're on the first line of the new frame
					// hsyncCnt = 0;
				}
			}
			else if (maskedAddr == 0x01) // VBLANK
			{
				_vblankEnabled = (value & 0x02) != 0;
				_capCharging = (value & 0x80) == 0;
				if ((value & 0x80) == 0)
				{
					_capChargeStart = _core.Cpu.TotalExecutedCycles;
				}
			}
			else if (maskedAddr == 0x02) // WSYNC
			{
				// Halt the CPU until we reach hblank
				_core.Cpu.RDY = false;
			}
			else if (maskedAddr == 0x04) // NUSIZ0
			{
				_player0.Nusiz = (byte)(value & 0x37);
				_player0.Missile.Size = (byte)((value & 0x30) >> 4);
				_player0.Missile.Number = (byte)(value & 0x07);
			}
			else if (maskedAddr == 0x05) // NUSIZ1
			{
				_player1.Nusiz = (byte)(value & 0x37);
				_player1.Missile.Size = (byte)((value & 0x30) >> 4);
				_player1.Missile.Number = (byte)(value & 0x07);
			}
			else if (maskedAddr == 0x06) // COLUP0
			{
				_player0.Color = (byte)(value & 0xFE);
			}
			else if (maskedAddr == 0x07) // COLUP1
			{
				_player1.Color = (byte)(value & 0xFE);
			}
			else if (maskedAddr == 0x08) // COLUPF
			{
                _playField.PfColor = (byte)(value & 0xFE);
			}
			else if (maskedAddr == 0x09) // COLUBK
			{
				_playField.BkColor = (byte)(value & 0xFE);
			}
			else if (maskedAddr == 0x0A) // CTRLPF
			{
				_playField.Reflect = (value & 0x01) != 0;
                _playField.Score = (value & 0x02) != 0;
				_playField.Priority = (value & 0x04) != 0;

				_ball.Size = (byte)((value & 0x30) >> 4);
			}
			else if (maskedAddr == 0x0B) // REFP0
			{
				_player0.Reflect = (value & 0x08) != 0;
			}
			else if (maskedAddr == 0x0C) // REFP1
			{
				_player1.Reflect = (value & 0x08) != 0;
			}
			else if (maskedAddr == 0x0D) // PF0
			{
                pf0_update = value;
                pf0_updater = true;
                pf0_delay_clock = 0;
                if (((_hsyncCnt / 3) & 3) == 0)
                {
                    pf0_max_delay = 4;
                }
                if (((_hsyncCnt / 3) & 3) == 1)
                {
                    pf0_max_delay = 5;
                }
                if (((_hsyncCnt / 3) & 3) == 2)
                {
                    pf0_max_delay = 2;
                }
                if (((_hsyncCnt / 3) & 3) == 3)
                {
                    pf0_max_delay = 3;
                }

                //_playField.Grp = (uint)((_playField.Grp & 0x0FFFF) + ((ReverseBits(value, 8) & 0x0F) << 16));
            }
			else if (maskedAddr == 0x0E) // PF1
			{
                pf1_update = value;
                pf1_updater = true;
                pf1_delay_clock = 0;
                if (((_hsyncCnt / 3) & 3) == 0)
                {
                    pf1_max_delay = 4;
                }
                if (((_hsyncCnt / 3) & 3) == 1)
                {
                    pf1_max_delay = 5;
                }
                if (((_hsyncCnt / 3) & 3) == 2)
                {
                    pf1_max_delay = 2;
                }
                if (((_hsyncCnt / 3) & 3) == 3)
                {
                    pf1_max_delay = 3;
                }
                //_playField.Grp = (uint)((_playField.Grp & 0xF00FF) + (value << 8));
            }
			else if (maskedAddr == 0x0F) // PF2
			{
                pf2_update = value;
                pf2_updater = true;
                pf2_delay_clock = 0;
                if (((_hsyncCnt / 3) & 3) == 0)
                {
                    pf2_max_delay = 4;
                }
                if (((_hsyncCnt / 3) & 3) == 1)
                {
                    pf2_max_delay = 5;
                }
                if (((_hsyncCnt / 3) & 3) == 2)
                {
                    pf2_max_delay = 2;
                }
                if (((_hsyncCnt / 3) & 3) == 3)
                {
                    pf2_max_delay = 3;
                }
                //_playField.Grp = (uint)((_playField.Grp & 0xFFF00) + ReverseBits(value, 8));
            }
			else if (maskedAddr == 0x10) // RESP0
			{
                // Resp depends on HMOVE
                if (!_hmove.LateHBlankReset)
                {
                    if (_hsyncCnt < 69)
                    {
                        _player0.HPosCnt = 0;
                        _player0.ResetCnt = 2;
                        _player0.Reset = true;
                    }
                    else
                    {
                        _player0.ResetCnt = 0;
                    }
                }
                else
                {
                    if (_hsyncCnt < 76)
                    {
                        _player0.HPosCnt = 0;
                        _player0.ResetCnt = 1;
                        _player0.Reset = true;
                    }
                    else
                    {
                        _player0.ResetCnt = 0;
                    }
                }
            }
			else if (maskedAddr == 0x11) // RESP1
			{
				// RESP depends on HMOVE
                if (!_hmove.LateHBlankReset)
                {
                    if (_hsyncCnt < 69)
                    {
                        _player1.HPosCnt = 0;
                        _player1.ResetCnt = 2;
                        _player1.Reset = true;
                    } else
                    {
                        _player1.ResetCnt = 0;
                    }
                } else
                {
                    if (_hsyncCnt < 76)
                    {
                        _player1.HPosCnt = 0;
                        _player1.ResetCnt = 1;
                        _player1.Reset = true;
                    }
                    else
                    {
                        _player1.ResetCnt = 0;
                    }
                }
					
			}
			else if (maskedAddr == 0x12) // RESM0
			{
                if (!_hmove.LateHBlankReset)
                {
                    _player0.Missile.HPosCnt = (byte)(_hsyncCnt < 68 ? 160 - 2 : 160 - 4);
                } else
                {
                    _player0.Missile.HPosCnt = (byte)(_hsyncCnt < 76 ? 160 - 2 : 160 - 4);
                }

            }
			else if (maskedAddr == 0x13) // RESM1
			{
                if (!_hmove.LateHBlankReset)
                {
                    _player1.Missile.HPosCnt = (byte)(_hsyncCnt < 68 ? 160 - 2 : 160 - 4);
                }
                else
                {
                    _player1.Missile.HPosCnt = (byte)(_hsyncCnt < 76 ? 160 - 2 : 160 - 4);
                }
            }
			else if (maskedAddr == 0x14) // RESBL
			{
                if (!_hmove.LateHBlankReset)
                {
                    _ball.HPosCnt = (byte)(_hsyncCnt < 68 ? 160 - 3 : 160 - 4);
                }
                else
                {
                    _ball.HPosCnt = (byte)(_hsyncCnt < 76 ? 160 - 3 : 160 - 4);
                }
                
			}
			else if (maskedAddr == 0x15) // AUDC0
			{
				WriteAudio(0, AudioRegister.AUDC, (byte)(value & 15));
			}
			else if (maskedAddr == 0x16) // AUDC1
			{
				WriteAudio(1, AudioRegister.AUDC, (byte)(value & 15));
			}
			else if (maskedAddr == 0x17) // AUDF0
			{
				WriteAudio(0, AudioRegister.AUDF, (byte)((value & 31) + 1));
			}
			else if (maskedAddr == 0x18) // AUDF1
			{
				WriteAudio(1, AudioRegister.AUDF, (byte)((value & 31) + 1));
			}
			else if (maskedAddr == 0x19) // AUDV0
			{
				WriteAudio(0, AudioRegister.AUDV, (byte)(value & 15));
			}
			else if (maskedAddr == 0x1A) // AUDV1
			{
				WriteAudio(1, AudioRegister.AUDV, (byte)(value & 15));
			}
			else if (maskedAddr == 0x1B) // GRP0
			{
                prg0_val = value;
                prg0_delay = 1;
                
			}
			else if (maskedAddr == 0x1C) // GRP1
			{
                prg1_val = value;
                prg1_delay = 1;
                
			}
			else if (maskedAddr == 0x1D) // ENAM0
			{
                enam0_val = (value & 0x02) != 0;
                enam0_delay = 1;
            }
			else if (maskedAddr == 0x1E) // ENAM1
			{
                enam1_val = (value & 0x02) != 0;
                enam1_delay = 1;
            }
			else if (maskedAddr == 0x1F) // ENABL
			{
                enamb_val = (value & 0x02) != 0;
                enamb_delay = 1;
			}
			else if (maskedAddr == 0x20) // HMP0
			{
				_player0.HM = (byte)((value & 0xF0) >> 4);
			}
			else if (maskedAddr == 0x21) // HMP1
			{
				_player1.HM = (byte)((value & 0xF0) >> 4);
			}
			else if (maskedAddr == 0x22) // HMM0
			{
				_player0.Missile.Hm = (byte)((value & 0xF0) >> 4);
			}
			else if (maskedAddr == 0x23) // HMM1
			{
				_player1.Missile.Hm = (byte)((value & 0xF0) >> 4);
			}
			else if (maskedAddr == 0x24) // HMBL
			{
				_ball.HM = (byte)((value & 0xF0) >> 4);
			}
			else if (maskedAddr == 0x25) // VDELP0
			{
				_player0.Delay = (value & 0x01) != 0;
			}
			else if (maskedAddr == 0x26) // VDELP1
			{
				_player1.Delay = (value & 0x01) != 0;
			}
			else if (maskedAddr == 0x27) // VDELBL
			{
				_ball.Delay = (value & 0x01) != 0;
			}
			else if (maskedAddr == 0x28) // RESMP0
			{
				_player0.Missile.ResetToPlayer = (value & 0x02) != 0;
			}
			else if (maskedAddr == 0x29) // RESMP1
			{
				_player1.Missile.ResetToPlayer = (value & 0x02) != 0;
			}
			else if (maskedAddr == 0x2A) // HMOVE
			{
				_hmove.HMoveEnabled = true;
				_hmove.HMoveJustStarted = true;
				_hmove.HMoveDelayCnt = 0;
			}
			else if (maskedAddr == 0x2B) // HMCLR
			{
				_player0.HM = 0;
				_player0.Missile.Hm = 0;
				_player1.HM = 0;
				_player1.Missile.Hm = 0;
				_ball.HM = 0;
			}
			else if (maskedAddr == 0x2C) // CXCLR
			{
				_player0.Collisions = 0;
				_player0.Missile.Collisions = 0;
				_player1.Collisions = 0;
				_player1.Missile.Collisions = 0;
				_ball.Collisions = 0;
			}
		}

		private static int ReverseBits(int value, int bits)
		{
			int result = 0;
			for (int i = 0; i < bits; i++)
			{
				result = (result << 1) | ((value >> i) & 0x01);
			}

			return result;
		}

		#region Audio bits

		private enum AudioRegister : byte { AUDC, AUDF, AUDV }
		private struct QueuedCommand
		{
			public int Time;
			public byte Channel;
			public AudioRegister Register;
			public byte Value;
		}

		private int frameStartCycles, frameEndCycles;
		private Queue<QueuedCommand> commands = new Queue<QueuedCommand>(4096);

		public void BeginAudioFrame()
		{
			frameStartCycles = _core.Cpu.TotalExecutedCycles;
		}

		public void CompleteAudioFrame()
		{
			frameEndCycles = _core.Cpu.TotalExecutedCycles;
		}

		private void WriteAudio(byte channel, AudioRegister register, byte value)
		{
			commands.Enqueue(new QueuedCommand { Channel = channel, Register = register, Value = value, Time = _core.Cpu.TotalExecutedCycles - frameStartCycles });
		}

		private void ApplyAudioCommand(QueuedCommand cmd)
		{
			switch (cmd.Register)
			{
				case AudioRegister.AUDC:
					AUD[cmd.Channel].AUDC = cmd.Value;
					break;
				case AudioRegister.AUDF:
					AUD[cmd.Channel].AUDF = cmd.Value;
					break;
				case AudioRegister.AUDV:
					AUD[cmd.Channel].AUDV = cmd.Value;
					break;
			}
		}

		// TODO: more accurate would be to have audio.Cycle() occur at
		// the explicit exact times in the scanline, instead of just approximately spaced
		public void GetSamples(short[] samples)
		{
			var samples31khz = new short[_audioClocks]; // mono

			int elapsedCycles = frameEndCycles - frameStartCycles;
			if (elapsedCycles == 0)
			{
				elapsedCycles = 1; // better than diving by zero
			}

			int start = 0;
			while (commands.Count > 0)
			{
				var cmd = commands.Dequeue();
				int pos = (cmd.Time * samples31khz.Length) / elapsedCycles;
				pos = Math.Min(pos, samples31khz.Length); // sometimes the cpu timestamp of the write is > frameEndCycles
				GetSamplesImmediate(samples31khz, start, pos - start);
				start = pos;
				ApplyAudioCommand(cmd);
			}

			GetSamplesImmediate(samples31khz, start, samples31khz.Length - start);

			// convert from 31khz to 44khz
			for (var i = 0; i < samples.Length / 2; i++)
			{
				samples[i * 2] = samples31khz[(int)(((double)samples31khz.Length / (double)(samples.Length / 2)) * i)];
				samples[(i * 2) + 1] = samples[i * 2];
			}

			_audioClocks = 0;
		}

		public void GetSamplesImmediate(short[] samples, int start, int len)
		{
			for (var i = start; i < start + len; i++)
			{
				samples[i] += AUD[0].Cycle();
				samples[i] += AUD[1].Cycle();
			}
		}

		public void DiscardSamples()
		{
			commands.Clear();
			_audioClocks = 0;
		}

		#endregion

		public void SyncState(Serializer ser)
		{
			ser.BeginSection("TIA");
			_ball.SyncState(ser);
			_hmove.SyncState(ser);
			ser.Sync("hsyncCnt", ref _hsyncCnt);
			// some of these things weren't in the state because they weren't needed if
			// states were always taken at frame boundaries
			ser.Sync("capChargeStart", ref _capChargeStart);
			ser.Sync("capCharging", ref _capCharging);
			ser.Sync("vblankEnabled", ref _vblankEnabled);
			ser.Sync("vsyncEnabled", ref _vsyncEnabled);
			ser.Sync("CurrentScanLine", ref _CurrentScanLine);
			ser.Sync("scanlinebuffer", ref _scanlinebuffer, false);

			ser.BeginSection("Player0");
			_player0.SyncState(ser);
			ser.EndSection();
			ser.BeginSection("Player1");
			_player1.SyncState(ser);
			ser.EndSection();
			_playField.SyncState(ser);
			ser.EndSection();
		}
	}
}
