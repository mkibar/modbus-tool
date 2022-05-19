namespace WSMBT
{
    internal class CModbus
    {
        private CTxRx TxRx;

        public CModbus(CTxRx Tx) { this.TxRx = Tx; }

        public Result ReadFlags(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] Bools,
          int offset)
        {
            ushort num1 = 0;
            ushort num2 = 0;
            if (function < (byte)1 || function > (byte)127)
                return Result.FUNCTION;
            if (quantity < (ushort)1 || quantity > (ushort)2000 || (int)quantity + offset > Bools.GetLength(0))
                return Result.QUANTITY;
            byte[] TXBuf = new byte[8];
            byte[] RXBuf = new byte[261];
            TXBuf[0] = unitId;
            TXBuf[1] = function;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte(quantity);
            TXBuf[5] = ByteAccess.LoByte(quantity);
            int ResponseLength = ((int)quantity + 7) / 8 + 3;
            Result result = this.TxRx.TxRx(TXBuf, 6, RXBuf, ResponseLength);
            if (result == Result.SUCCESS)
            {
                if ((int)TXBuf[0] != (int)RXBuf[0] || (int)TXBuf[1] != (int)RXBuf[1])
                    result = Result.RESPONSE;
                else if (ResponseLength - 3 != (int)RXBuf[2])
                {
                    result = Result.BYTECOUNT;
                }
                else
                {
                    int num3 = (int)RXBuf[3];
                    for (int index = 0; index < (int)quantity; ++index)
                    {
                        Bools[index + offset] = (num3 & 1) == 1;
                        num3 >>= 1;
                        if (++num1 == (ushort)8)
                        {
                            ++num2;
                            num1 = (ushort)0;
                            num3 = (int)RXBuf[3 + (int)num2];
                        }
                    }
                }
            }
            return result;
        }

        public Result ReadRegisters(
          byte unitId,
          ushort function,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            if (function < (ushort)1 || function > (ushort)sbyte.MaxValue)
                return Result.FUNCTION;
            if (quantity < (ushort)1 || quantity > (ushort)125 || (int)quantity + offset > registers.GetLength(0))
                return Result.QUANTITY;
            byte[] TXBuf = new byte[8];
            byte[] RXBuf = new byte[261];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)((uint)function & (uint)byte.MaxValue);
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte(quantity);
            TXBuf[5] = ByteAccess.LoByte(quantity);
            int ResponseLength = 3 + (int)quantity * 2;
            Result result = this.TxRx.TxRx(TXBuf, 6, RXBuf, ResponseLength);
            if (result == Result.SUCCESS)
            {
                if ((int)TXBuf[0] != (int)RXBuf[0] || (int)TXBuf[1] != (int)RXBuf[1])
                    result = Result.RESPONSE;
                else if ((int)quantity * 2 != (int)RXBuf[2])
                {
                    result = Result.BYTECOUNT;
                }
                else
                {
                    for (int index = 0; index < (int)quantity; ++index)
                        registers[index + offset] = (short)((int)RXBuf[2 * index + 4] & (int)byte.MaxValue | ((int)RXBuf[2 * index + 3] & (int)byte.MaxValue) << 8);
                }
            }
            return result;
        }

        public Result WriteSingleCoil(byte unitId, ushort address, bool coil)
        {
            byte[] TXBuf = new byte[8];
            byte[] RXBuf = new byte[8];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)5;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = coil ? byte.MaxValue : (byte)0;
            TXBuf[5] = (byte)0;
            Result result = this.TxRx.TxRx(TXBuf, 6, RXBuf, 6);
            if (result == Result.SUCCESS && TXBuf[0] != (byte)0)
            {
                for (int index = 0; index < 6; ++index)
                {
                    if ((int)TXBuf[index] != (int)RXBuf[index])
                        result = Result.RESPONSE;
                }
            }
            return result;
        }

        public Result WriteSingleRegister(byte unitId, ushort address, short register)
        {
            byte[] TXBuf = new byte[8];
            byte[] RXBuf = new byte[8];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)6;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte((ushort)register);
            TXBuf[5] = ByteAccess.LoByte((ushort)register);
            Result result = this.TxRx.TxRx(TXBuf, 6, RXBuf, 6);
            if (result == Result.SUCCESS && TXBuf[0] != (byte)0)
            {
                for (int index = 0; index < 6; ++index)
                {
                    if ((int)TXBuf[index] != (int)RXBuf[index])
                        result = Result.RESPONSE;
                }
            }
            return result;
        }

        public Result WriteFlags(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] Bools,
          int offset)
        {
            if (function < (byte)1 || function > (byte)127)
                return Result.FUNCTION;
            if (quantity < (ushort)1 || quantity > (ushort)1968 || (int)quantity + offset > Bools.GetLength(0))
                return Result.QUANTITY;
            byte[] TXBuf = new byte[265];
            byte[] RXBuf = new byte[8];
            ushort num1 = 0;
            byte index1 = 7;
            byte num2 = 0;
            TXBuf[0] = unitId;
            TXBuf[1] = function;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte(quantity);
            TXBuf[5] = ByteAccess.LoByte(quantity);
            TXBuf[6] = (byte)(((int)quantity + 7) / 8);
            for (int index2 = 0; index2 < (int)quantity; ++index2)
            {
                if (Bools[index2 + offset])
                    num2 |= (byte)(1U << (int)num1);
                ++num1;
                if (num1 == (ushort)8)
                {
                    num1 = (ushort)0;
                    TXBuf[(int)index1] = num2;
                    ++index1;
                    num2 = (byte)0;
                }
            }
            TXBuf[(int)index1] = num2;
            Result result = this.TxRx.TxRx(TXBuf, (int)TXBuf[6] + 7, RXBuf, 6);
            if (result == Result.SUCCESS && TXBuf[0] != (byte)0)
            {
                for (int index3 = 0; index3 < 6; ++index3)
                {
                    if ((int)TXBuf[index3] != (int)RXBuf[index3])
                        result = Result.RESPONSE;
                }
            }
            return result;
        }

        public Result WriteRegisters(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            if (function < (byte)1 || function > (byte)127)
                return Result.FUNCTION;
            if (quantity < (ushort)1 || quantity > (ushort)123 || (int)quantity + offset > registers.GetLength(0))
                return Result.QUANTITY;
            byte[] TXBuf = new byte[265];
            byte[] RXBuf = new byte[8];
            TXBuf[0] = unitId;
            TXBuf[1] = function;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte(quantity);
            TXBuf[5] = ByteAccess.LoByte(quantity);
            TXBuf[6] = (byte)((uint)quantity * 2U);
            for (int index = 0; index < (int)quantity; ++index)
            {
                TXBuf[7 + index * 2] = ByteAccess.HiByte((ushort)registers[index + offset]);
                TXBuf[7 + index * 2 + 1] = ByteAccess.LoByte((ushort)registers[index + offset]);
            }
            Result result = this.TxRx.TxRx(TXBuf, (int)TXBuf[6] + 7, RXBuf, 6);
            if (result == Result.SUCCESS && TXBuf[0] != (byte)0)
            {
                for (int index = 0; index < 6; ++index)
                {
                    if ((int)TXBuf[index] != (int)RXBuf[index])
                        result = Result.RESPONSE;
                }
            }
            return result;
        }

        public Result MaskWriteRegister(
          byte unitId,
          ushort address,
          ushort ANDMask,
          ushort ORMask)
        {
            byte[] TXBuf = new byte[10];
            byte[] RXBuf = new byte[10];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)22;
            TXBuf[2] = ByteAccess.HiByte(address);
            TXBuf[3] = ByteAccess.LoByte(address);
            TXBuf[4] = ByteAccess.HiByte(ANDMask);
            TXBuf[5] = ByteAccess.LoByte(ANDMask);
            TXBuf[6] = ByteAccess.HiByte(ORMask);
            TXBuf[7] = ByteAccess.LoByte(ORMask);
            Result result = this.TxRx.TxRx(TXBuf, 8, RXBuf, 8);
            if (result == Result.SUCCESS && TXBuf[0] != (byte)0)
            {
                for (int index = 0; index < 8; ++index)
                {
                    if ((int)TXBuf[index] != (int)RXBuf[index])
                        result = Result.RESPONSE;
                }
            }
            return result;
        }

        public Result ReadWriteMultipleRegisters(
          byte unitId,
          ushort readAddress,
          ushort readSize,
          short[] readRegisters,
          ushort writeAddress,
          ushort writeSize,
          short[] writeRegisters)
        {
            if (readSize < (ushort)1 || readSize > (ushort)123 || writeSize < (ushort)1 || writeSize > (ushort)121 || (int)readSize > readRegisters.GetLength(0) || (int)writeSize > writeRegisters.GetLength(0))
                return Result.QUANTITY;
            byte[] TXBuf = new byte[269];
            byte[] RXBuf = new byte[261];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)23;
            TXBuf[2] = ByteAccess.HiByte(readAddress);
            TXBuf[3] = ByteAccess.LoByte(readAddress);
            TXBuf[4] = ByteAccess.HiByte(readSize);
            TXBuf[5] = ByteAccess.LoByte(readSize);
            TXBuf[6] = ByteAccess.HiByte(writeAddress);
            TXBuf[7] = ByteAccess.LoByte(writeAddress);
            TXBuf[8] = ByteAccess.HiByte(writeSize);
            TXBuf[9] = ByteAccess.LoByte(writeSize);
            TXBuf[10] = (byte)((uint)writeSize * 2U);
            int ResponseLength = 3 + (int)readSize * 2;
            for (int index = 0; index < (int)writeSize; ++index)
            {
                TXBuf[11 + index * 2] = ByteAccess.HiByte((ushort)writeRegisters[index]);
                TXBuf[11 + index * 2 + 1] = ByteAccess.LoByte((ushort)writeRegisters[index]);
            }
            Result result = this.TxRx.TxRx(TXBuf, (int)TXBuf[10] + 11, RXBuf, ResponseLength);
            if (result == Result.SUCCESS)
            {
                if ((int)TXBuf[0] != (int)RXBuf[0] || (int)TXBuf[1] != (int)RXBuf[1])
                {
                    result = Result.RESPONSE;
                }
                else
                {
                    for (int index = 0; index < (int)readSize; ++index)
                        readRegisters[index] = (short)ByteAccess.MakeWord(RXBuf[2 * index + 4], RXBuf[2 * index + 3]);
                }
            }
            return result;
        }

        public Result ReportSlaveID(byte unitId, out byte byteCount, byte[] deviceSpecific)
        {
            byte[] TXBuf = new byte[4];
            byte[] RXBuf = new byte[(int)byte.MaxValue];
            TXBuf[0] = unitId;
            TXBuf[1] = (byte)17;
            byteCount = (byte)0;
            Result result = this.TxRx.TxRx(TXBuf, 2, RXBuf, int.MaxValue);
            if (result == Result.SUCCESS)
            {
                if ((int)TXBuf[0] != (int)RXBuf[0])
                {
                    result = Result.RESPONSE;
                }
                else
                {
                    byteCount = RXBuf[2];
                    int num = Math.Min((int)RXBuf[2], deviceSpecific.GetLength(0));
                    for (int index = 0; index < num; ++index)
                        deviceSpecific[index] = RXBuf[index + 3];
                }
            }
            return result;
        }
    }

}