namespace WSMBT
{
    internal class CAscii
    {
        public static byte Num2Ascii(byte nNum)
        {
            if (nNum <= (byte)9)
                return (byte)((uint)nNum + 48U);
            return nNum >= (byte)10 && nNum <= (byte)15 ? (byte)((int)nNum - 10 + 65) : (byte)48;
        }

        public static byte Ascii2Num(byte nChar)
        {
            if (nChar >= (byte)48 && nChar <= (byte)57)
                return (byte)((uint)nChar - 48U);
            return nChar >= (byte)65 && nChar <= (byte)70 ? (byte)((int)nChar - 65 + 10) : (byte)0;
        }

        public static byte HiLo4BitsToByte(byte nHi, byte nLo) => (byte)((15 & (int)nHi) << 4 | 15 & (int)nLo);

        public static void RTU2ASCII(byte[] nRtu, int Size, byte[] nAscii)
        {
            for (int index = 0; index < Size; ++index)
            {
                nAscii[1 + index * 2] = CAscii.Num2Ascii(ByteAccess.HI4BITS(nRtu[index]));
                nAscii[1 + index * 2 + 1] = CAscii.Num2Ascii(ByteAccess.LO4BITS(nRtu[index]));
            }
        }

        public static byte LRC(byte[] nMsg, int DataLen)
        {
            byte num = 0;
            for (int index = 0; index < DataLen; ++index)
                num += nMsg[index];
            return (byte)-num;//mk: return -num;
        }

        public static byte LRCASCII(byte[] MsgASCII, int DataLen)
        {
            byte num1 = 0;
            int num2 = (DataLen - 5) / 2;
            for (int index = 0; index < num2; ++index)
            {
                byte num3 = CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(MsgASCII[1 + index * 2]), CAscii.Ascii2Num(MsgASCII[1 + index * 2 + 1]));
                num1 += num3;
            }
            return (byte)-num1; // mk: return -num1;
        }

        public static bool VerifyRespLRC(byte[] Resp, int Length) => Length >= 5 && (int)CAscii.LRCASCII(Resp, Length) == (int)CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(Resp[Length - 4]), CAscii.Ascii2Num(Resp[Length - 3]));
    }
}