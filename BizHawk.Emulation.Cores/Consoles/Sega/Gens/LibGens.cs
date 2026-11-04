using System.Runtime.InteropServices;

namespace BizHawk.Emulation.Cores.Consoles.Sega.Gens
{
	public class LibGens
	{
		const string dll = "Gens.dll";
		const CallingConvention cc = CallingConvention.Cdecl;

		[DllImport(dll, CallingConvention = cc)]
		public static extern int gens_init(byte[] rom, int romLength);

		[DllImport(dll, CallingConvention = cc)]
		public static extern bool gens_advance(int[] vbuff);

		[DllImport(dll, CallingConvention = cc)]
		public static extern byte gens_peek_m68k(uint addr);

		[DllImport(dll, CallingConvention = cc)]
		public static extern byte gens_peek_rom(uint addr);
	}
}