using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Cores.Consoles.Sega.Gens
{
	public class LibGens
	{
		const string dll = "Gens.dll";
		const CallingConvention cc = CallingConvention.Cdecl;

		public enum Buttons
		{
			Up    = 1 << 0x00,
			Down  = 1 << 0x01,
			Left  = 1 << 0x02,
			Right = 1 << 0x03,
			Start = 1 << 0x04,
			A     = 1 << 0x05,
			B     = 1 << 0x06,
			C     = 1 << 0x07,
			X     = 1 << 0x08,
			Y     = 1 << 0x09,
			Z     = 1 << 0x0a,
			Mode  = 1 << 0x0b
		}

		[DllImport(dll, CallingConvention = cc)]
		public static extern int gens_init(byte[] rom, int romLength);

		[DllImport(dll, CallingConvention = cc)]
		public static extern bool gens_advance(int[] vbuff, int input);

		[DllImport(dll, CallingConvention = cc)]
		public static extern byte gens_peek_m68k(uint addr);

		[DllImport(dll, CallingConvention = cc)]
		public static extern byte gens_peek_rom(uint addr);
	}
}