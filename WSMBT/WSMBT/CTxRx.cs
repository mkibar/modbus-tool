using System.Net.Sockets;

namespace WSMBT
{
    internal class CTxRx
    {
        private Mode _Mode;
        private TcpClient client = new();
        private string Error = "";
        private int Time;
        private bool _License;
        private int timeStamp;
        private bool firstTime = true;
        private int _Timeout = 1000;
        private ushort TransactionID;
        private byte[] _TxBuf = new byte[600];
        private byte[] _RxBuf = new byte[600];
        private int _TxBufSize;
        private int _RxBufSize;

        //public bool License
        //{
        //    get { return this._License; }
        //    set { this._License = value; }
        //}

        public void SetClient(TcpClient _client) => this.client = _client;

        public Mode Mode
        {
            get { return this._Mode; }
            set { this._Mode = value; }
        }

        public int Timeout
        {
            get { return this._Timeout; }
            set { this._Timeout = value; }
        }

        public string GetErrorMessage() => this.Error;

        public int GetTxBuffer(byte[] byteArray)
        {
            if (byteArray.GetLength(0) < this._TxBufSize)
                return 0;
            Array.Copy((Array)this._TxBuf, (Array)byteArray, this._TxBufSize);
            return this._TxBufSize;
        }

        public int GetRxBuffer(byte[] byteArray)
        {
            if (byteArray.GetLength(0) < this._RxBufSize)
                return 0;
            Array.Copy((Array)this._RxBuf, (Array)byteArray, this._RxBufSize);
            return this._RxBufSize;
        }

        public Result TxRx(byte[] TXBuf, int QueryLength, byte[] RXBuf, int ResponseLength)
        {
            int num1 = Environment.TickCount & int.MaxValue;
            this._TxBufSize = 0;
            this._RxBufSize = 0;
            //if (!this._License)
            //{
            //    if (this.firstTime)
            //    {
            //        this.timeStamp = num1;
            //        this.firstTime = false;
            //        //int num2 = (int)MessageBox.Show("WSMBT is running in evaluation mode and will run for 30 minutes.", "WSMBT Modbus Master TCP/IP Component");
            //        Console.WriteLine("WSMBT is running in evaluation mode and will run for 30 minutes. WSMBT Modbus Master TCP/IP Component");
            //    }
            //    else if (num1 - this.timeStamp > 1800000)
            //        return Result.DEMO_TIMEOUT;
            //}
            if (num1 - this.Time < 20)
                Thread.Sleep(4);
            if (this.client == null)
                return Result.ISCLOSED;
            try
            {
                if (!this.client.Connected)
                    return Result.ISCLOSED;
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                return Result.ISCLOSED;
            }
            Result result;
            switch (this._Mode)
            {
                case Mode.TCP_IP:
                    if (ResponseLength == int.MaxValue)
                        return Result.ILLEGAL_FUNCTION;
                    result = this.TxRxTCP(TXBuf, QueryLength, RXBuf, ResponseLength);
                    break;
                case Mode.RTU_OVER_TCP_IP:
                    result = this.TxRxRTUOverTCP(TXBuf, QueryLength, RXBuf, ResponseLength);
                    break;
                case Mode.ASCII_OVER_TCP_IP:
                    result = this.TxRxASCIIOverTCP(TXBuf, QueryLength, RXBuf, ResponseLength);
                    break;
                default:
                    result = Result.SUCCESS;
                    break;
            }
            this.Time = Environment.TickCount;
            return result;
        }

        private void DiscardReadBuffer()
        {
            byte[] buffer = new byte[100];
            if (!this.client.GetStream().CanRead)
                return;
            while (this.client.GetStream().DataAvailable)
                this.client.GetStream().Read(buffer, 0, 100);
        }

        private bool ResponseTimeout()
        {
            int num = Environment.TickCount & int.MaxValue;
            int millisecondsTimeout = this._Timeout / 100;
            if (millisecondsTimeout == 0)
                millisecondsTimeout = 1;
            while (!this.client.GetStream().DataAvailable)
            {
                if (Math.Abs((Environment.TickCount & int.MaxValue) - num) > this._Timeout)
                    return true;
                Thread.Sleep(millisecondsTimeout);
            }
            return false;
        }

        private Result TxRxTCP(byte[] TXBuf, int QueryLength, byte[] RXBuf, int ResponseLength)
        {
            int num = 0;
            ++this.TransactionID;
            for (int index = 0; index < 5; ++index)
                this._TxBuf[index] = (byte)0;
            this._TxBuf[0] = (byte)((uint)this.TransactionID >> 8);
            this._TxBuf[1] = (byte)((uint)this.TransactionID & (uint)byte.MaxValue);
            this._TxBuf[4] = (byte)(QueryLength >> 8);
            this._TxBuf[5] = (byte)(QueryLength & (int)byte.MaxValue);
            for (int index = 0; index < QueryLength; ++index)
                this._TxBuf[6 + index] = TXBuf[index];
            if (this.client.GetStream().CanWrite)
            {
                try
                {
                    this.client.GetStream().Write(this._TxBuf, 0, QueryLength + 6);
                }
                catch (Exception ex)
                {
                    this.Error = ex.Message;
                    return Result.WRITE;
                }
                this._TxBufSize = QueryLength + 6;
                if (this.client.GetStream().CanRead)
                {
                    try
                    {
                        do
                        {
                            if (this.ResponseTimeout())
                                return Result.RESPONSE_TIMEOUT;
                            num += this.client.GetStream().Read(this._RxBuf, num, ResponseLength + 6 - num);
                            switch (num)
                            {
                                case 0:
                                    return Result.RESPONSE_TIMEOUT;
                                case 9:
                                    if (this._TxBuf[7] > (byte)128)
                                    {
                                        this._RxBufSize = num;
                                        return (Result)this._TxBuf[8];
                                    }
                                    break;
                            }
                        }
                        while (num < ResponseLength + 6);
                        if ((int)this.TransactionID != (int)ByteAccess.MakeWord(this._RxBuf[0], this._RxBuf[1]))
                        {
                            this.DiscardReadBuffer();
                            this._RxBufSize = num;
                            return Result.TRANSACTIONID;
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        return Result.READ;
                    }
                    finally
                    {
                        this._RxBufSize = num;
                    }
                    if (num - 6 < ResponseLength)
                        return Result.RESPONSE_TIMEOUT;
                    for (int index = 0; index < Math.Min(num, ResponseLength + 6) - 6; ++index)
                        RXBuf[index] = this._RxBuf[index + 6];
                    return Result.SUCCESS;
                }
                this.Error = "You cannot read from this NetworkStream.";
                return Result.READ;
            }
            this.Error = "You cannot write to this NetworkStream.";
            return Result.WRITE;
        }

        private Result TxRxRTUOverTCP(
          byte[] TXBuf,
          int QueryLength,
          byte[] RXBuf,
          int ResponseLength)
        {
            int offset = 0;
            byte[] numArray1 = CCRC16.CRC16(TXBuf, QueryLength);
            TXBuf[QueryLength] = numArray1[0];
            TXBuf[QueryLength + 1] = numArray1[1];
            this.DiscardReadBuffer();
            if (this.client.GetStream().CanWrite)
            {
                try
                {
                    this.client.GetStream().Write(TXBuf, 0, QueryLength + 2);
                }
                catch (Exception ex)
                {
                    this.Error = ex.Message;
                    return Result.WRITE;
                }
                this._TxBufSize = QueryLength + 2;
                Array.Copy((Array)TXBuf, (Array)this._TxBuf, this._TxBufSize);
                if (TXBuf[0] == (byte)0)
                    return Result.SUCCESS;
                if (this.client.GetStream().CanRead)
                {
                    try
                    {
                        do
                        {
                            if (this.ResponseTimeout())
                                return Result.RESPONSE_TIMEOUT;
                            int num = this.client.GetStream().Read(RXBuf, offset, 5 - offset);
                            offset += num;
                            if (offset == 0)
                                return Result.RESPONSE_TIMEOUT;
                        }
                        while (5 - offset > 0);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        return Result.READ;
                    }
                    finally
                    {
                        this._RxBufSize = offset;
                        Array.Copy((Array)RXBuf, (Array)this._RxBuf, this._RxBufSize);
                    }
                    if (RXBuf[1] > (byte)128)
                    {
                        byte[] numArray2 = CCRC16.CRC16(RXBuf, 5);
                        return numArray2[0] != (byte)0 || numArray2[1] != (byte)0 ? Result.CRC : (Result)RXBuf[2];
                    }
                    if (ResponseLength == int.MaxValue)
                    {
                        if (RXBuf[1] == (byte)17)
                        {
                            ResponseLength = (int)RXBuf[2] + 3;
                        }
                        else
                        {
                            this.DiscardReadBuffer();
                            return Result.RESPONSE;
                        }
                    }
                    try
                    {
                        int num1 = ResponseLength + 2;
                        do
                        {
                            if (this.ResponseTimeout())
                                return Result.RESPONSE_TIMEOUT;
                            int num2 = this.client.GetStream().Read(RXBuf, offset, num1 - offset);
                            offset += num2;
                        }
                        while (num1 - offset > 0);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        return Result.READ;
                    }
                    finally
                    {
                        this._RxBufSize = offset;
                        Array.Copy((Array)RXBuf, (Array)this._RxBuf, this._RxBufSize);
                    }
                    byte[] numArray3 = CCRC16.CRC16(RXBuf, ResponseLength + 2);
                    return numArray3[0] != (byte)0 || numArray3[1] != (byte)0 ? Result.CRC : Result.SUCCESS;
                }
                this.Error = "You cannot read from this NetworkStream.";
                return Result.READ;
            }
            this.Error = "You cannot write to this NetworkStream.";
            return Result.WRITE;
        }

        private Result TxRxASCIIOverTCP(
          byte[] TXBuf,
          int QueryLength,
          byte[] RXBuf,
          int ResponseLength)
        {
            int num1 = 0;
            byte[] numArray1 = new byte[531];
            byte[] numArray2 = new byte[523];
            CAscii.RTU2ASCII(TXBuf, QueryLength, numArray1);
            byte n = CAscii.LRC(TXBuf, QueryLength);
            numArray1[0] = (byte)58;
            numArray1[QueryLength * 2 + 1] = CAscii.Num2Ascii(ByteAccess.HI4BITS(n));
            numArray1[QueryLength * 2 + 2] = CAscii.Num2Ascii(ByteAccess.LO4BITS(n));
            numArray1[QueryLength * 2 + 3] = (byte)13;
            numArray1[QueryLength * 2 + 4] = (byte)10;
            this.DiscardReadBuffer();
            if (this.client.GetStream().CanWrite)
            {
                try
                {
                    this.client.GetStream().Write(numArray1, 0, QueryLength * 2 + 5);
                }
                catch (Exception ex)
                {
                    this.Error = ex.Message;
                    return Result.WRITE;
                }
                this._TxBufSize = QueryLength * 2 + 5;
                Array.Copy((Array)numArray1, (Array)this._TxBuf, this._TxBufSize);
                if (TXBuf[0] == (byte)0)
                    return Result.SUCCESS;
                if (this.client.GetStream().CanRead)
                {
                    try
                    {
                        do
                        {
                            if (this.ResponseTimeout())
                                return Result.RESPONSE_TIMEOUT;
                            int num2 = this.client.GetStream().Read(numArray2, num1, 11 - num1);
                            num1 += num2;
                            if (num1 == 0)
                                return Result.RESPONSE_TIMEOUT;
                        }
                        while (11 - num1 > 0);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        return Result.READ;
                    }
                    finally
                    {
                        this._RxBufSize = num1;
                        Array.Copy((Array)numArray2, (Array)this._RxBuf, this._RxBufSize);
                    }
                    if (CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(numArray2[3]), CAscii.Ascii2Num(numArray2[4])) > (byte)128)
                        return !CAscii.VerifyRespLRC(numArray2, 11) ? Result.CRC : (Result)CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(numArray2[5]), CAscii.Ascii2Num(numArray2[6]));
                    if (ResponseLength == int.MaxValue)
                    {
                        if (CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(numArray2[3]), CAscii.Ascii2Num(numArray2[4])) == (byte)17)
                        {
                            ResponseLength = (int)CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(numArray2[5]), CAscii.Ascii2Num(numArray2[6])) + 3;
                        }
                        else
                        {
                            this.DiscardReadBuffer();
                            return Result.RESPONSE;
                        }
                    }
                    try
                    {
                        int num3 = ResponseLength * 2 + 5;
                        do
                        {
                            if (this.ResponseTimeout())
                                return Result.RESPONSE_TIMEOUT;
                            int num4 = this.client.GetStream().Read(numArray2, num1, num3 - num1);
                            num1 += num4;
                        }
                        while (num3 - num1 > 0);
                    }
                    catch (Exception ex)
                    {
                        this.Error = ex.Message;
                        return Result.READ;
                    }
                    finally
                    {
                        this._RxBufSize = num1;
                        Array.Copy((Array)numArray2, (Array)this._RxBuf, this._RxBufSize);
                    }
                    if (!CAscii.VerifyRespLRC(numArray2, num1))
                        return Result.CRC;
                    if (numArray2[num1 - 2] != (byte)13 || numArray2[num1 - 1] != (byte)10)
                        return Result.RESPONSE;
                    int num5 = (num1 - 5) / 2;
                    for (int index = 0; index < num5; ++index)
                        RXBuf[index] = CAscii.HiLo4BitsToByte(CAscii.Ascii2Num(numArray2[1 + index * 2]), CAscii.Ascii2Num(numArray2[2 + index * 2]));
                    return Result.SUCCESS;
                }
                this.Error = "You cannot read from this NetworkStream.";
                return Result.READ;
            }
            this.Error = "You cannot write to this NetworkStream.";
            return Result.WRITE;
        }
    }

}