namespace WSMBT
{
    internal class ByteAccess
    {
        public static byte HI4BITS(byte n) => (byte)((int)n >> 4 & 15);

        public static byte LO4BITS(byte n) => (byte)((uint)n & 15U);

        public static uint MakeLong(ushort high, ushort low) => (uint)((int)low & (int)ushort.MaxValue | ((int)high & (int)ushort.MaxValue) << 16);

        public static ushort MakeWord(byte high, byte low) => (ushort)((int)low & (int)byte.MaxValue | ((int)high & (int)byte.MaxValue) << 8);

        public static ushort LoWord(uint nValue) => (ushort)(nValue & (uint)ushort.MaxValue);

        public static ushort HiWord(uint nValue) => (ushort)(nValue >> 16);

        public static byte LoByte(ushort nValue) => (byte)((uint)nValue & (uint)byte.MaxValue);

        public static byte HiByte(ushort nValue) => (byte)((uint)nValue >> 8);
    }

}