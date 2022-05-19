//using System.ComponentModel;
using System.Net.Sockets;

namespace WSMBT
{
    public class WSMBTControl //: Component
    {
        private CModbus Modbus;
        private TcpClient client = new();
        private CTxRx TxRx;
        private int _ResponseTimeout = 2000;
        private int _ConnectTimeout = 2000;
        private Mode _Mode;
        private string Error = "";
        private Result Res;
        //private IContainer components;

        public WSMBTControl()
        {
            //this.InitializeComponent();
            this.Modbus = new CModbus(this.TxRx = new CTxRx());
            this.TxRx.Mode = Mode.TCP_IP;
        }

        //public WSMBTControl(IContainer container)
        //{
        //    container.Add((IComponent)this);
        //    //this.InitializeComponent();
        //    this.Modbus = new CModbus(this.TxRx = new CTxRx());
        //    this.TxRx.Mode = Mode.TCP_IP;
        //}

        //[Category("Modbus")]
        //[Description("Select which protocol mode to use.")]
        public Mode Mode
        {
            get { return this._Mode; }
            set { this._Mode = value; }
        }

        //[Description("Max time to wait for response 100 - 30000ms.")]
        //[Category("Modbus")]
        public int ResponseTimeout
        {
            get { return this._ResponseTimeout; }
            set
            {
                if (value < 100 || value > 30000)
                    return;
                this._ResponseTimeout = value;
            }
        }

        //[Description("Max time to wait for connection 100 - 30000ms.")]
        //[Category("Modbus")]
        public int ConnectTimeout
        {
            get { return this._ConnectTimeout; }
            set
            {
                if (value < 100 || value > 30000)
                    return;
                this._ConnectTimeout = value;
            }
        }

        public Result ReadCoils(byte unitId, ushort address, ushort quantity, bool[] coils) => this.Res = this.Modbus.ReadFlags(unitId, (byte)1, address, quantity, coils, 0);

        public Result ReadCoils(
          byte unitId,
          ushort address,
          ushort quantity,
          bool[] coils,
          int offset)
        {
            return this.Res = this.Modbus.ReadFlags(unitId, (byte)1, address, quantity, coils, offset);
        }

        public Result ReadDiscreteInputs(
          byte unitId,
          ushort address,
          ushort quantity,
          bool[] discreteInputs)
        {
            return this.Res = this.Modbus.ReadFlags(unitId, (byte)2, address, quantity, discreteInputs, 0);
        }

        public Result ReadDiscreteInputs(
          byte unitId,
          ushort address,
          ushort quantity,
          bool[] discreteInputs,
          int offset)
        {
            return this.Res = this.Modbus.ReadFlags(unitId, (byte)2, address, quantity, discreteInputs, offset);
        }

        public Result ReadHoldingRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)3, address, quantity, registers, 0);
        }

        public Result ReadHoldingRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)3, address, quantity, registers, offset);
        }

        public Result ReadInputRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)4, address, quantity, registers, 0);
        }

        public Result ReadInputRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)4, address, quantity, registers, offset);
        }

        public Result WriteSingleCoil(byte unitId, ushort address, bool coil) => this.Res = this.Modbus.WriteSingleCoil(unitId, address, coil);

        public Result WriteSingleRegister(byte unitId, ushort address, short register) => this.Res = this.Modbus.WriteSingleRegister(unitId, address, register);

        public Result WriteMultipleCoils(
          byte unitId,
          ushort address,
          ushort quantity,
          bool[] coils)
        {
            return this.Res = this.Modbus.WriteFlags(unitId, (byte)15, address, quantity, coils, 0);
        }

        public Result WriteMultipleCoils(
          byte unitId,
          ushort address,
          ushort quantity,
          bool[] coils,
          int offset)
        {
            return this.Res = this.Modbus.WriteFlags(unitId, (byte)15, address, quantity, coils, offset);
        }

        public Result WriteMultipleRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers)
        {
            return this.Res = this.Modbus.WriteRegisters(unitId, (byte)16, address, quantity, registers, 0);
        }

        public Result WriteMultipleRegisters(
          byte unitId,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            return this.Res = this.Modbus.WriteRegisters(unitId, (byte)16, address, quantity, registers, offset);
        }

        public Result ReadWriteMultipleRegisters(
          byte unitId,
          ushort readAddress,
          ushort readQuantity,
          short[] readRegisters,
          ushort writeAddress,
          ushort writeQuantity,
          short[] writeRegisters)
        {
            return this.Res = this.Modbus.ReadWriteMultipleRegisters(unitId, readAddress, readQuantity, readRegisters, writeAddress, writeQuantity, writeRegisters);
        }

        public Result ReadUserDefinedCoils(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] coils)
        {
            return this.Res = this.Modbus.ReadFlags(unitId, function, address, quantity, coils, 0);
        }

        public Result ReadUserDefinedCoils(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] coils,
          int offset)
        {
            return this.Res = this.Modbus.ReadFlags(unitId, function, address, quantity, coils, offset);
        }

        public Result ReadUserDefinedRegisters(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          short[] registers)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)function, address, quantity, registers, 0);
        }

        public Result ReadUserDefinedRegisters(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            return this.Res = this.Modbus.ReadRegisters(unitId, (ushort)function, address, quantity, registers, offset);
        }

        public Result WriteUserDefinedCoils(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] coils)
        {
            return this.Res = this.Modbus.WriteFlags(unitId, function, address, quantity, coils, 0);
        }

        public Result WriteUserDefinedCoils(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          bool[] coils,
          int offset)
        {
            return this.Res = this.Modbus.WriteFlags(unitId, function, address, quantity, coils, offset);
        }

        public Result WriteUserDefinedRegisters(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          short[] registers)
        {
            return this.Res = this.Modbus.WriteRegisters(unitId, function, address, quantity, registers, 0);
        }

        public Result WriteUserDefinedRegisters(
          byte unitId,
          byte function,
          ushort address,
          ushort quantity,
          short[] registers,
          int offset)
        {
            return this.Res = this.Modbus.WriteRegisters(unitId, function, address, quantity, registers, offset);
        }

        public Result ReportSlaveID(byte unitId, out byte byteCount, byte[] deviceSpecific) => this.Res = this.Modbus.ReportSlaveID(unitId, out byteCount, deviceSpecific);

        public Result MaskWriteRegister(
          byte unitId,
          ushort address,
          ushort andMask,
          ushort orMask)
        {
            return this.Res = this.Modbus.MaskWriteRegister(unitId, address, andMask, orMask);
        }

        public Result Connect(string IPAddress, int port)
        {
            this.client = new TcpClient();
            this.TxRx.SetClient(this.client);
            this.TxRx.Timeout = this._ResponseTimeout;
            this.client.SendTimeout = 5000; //mk: this.client.SendTimeout = 2000;
            this.client.ReceiveTimeout = this._ResponseTimeout;
            try
            {
                IAsyncResult asyncResult = this.client.BeginConnect(IPAddress, port, null, null);
                WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
                if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds((double)this._ConnectTimeout), false))
                {
                    this.client.Close();
                    return this.Res = Result.CONNECT_TIMEOUT;
                }
                this.client.EndConnect(asyncResult);
                asyncWaitHandle.Close();
            }
            catch (Exception ex)
            {
                this.Error = ex.Message;
                return this.Res = Result.CONNECT_ERROR;
            }
            this.TxRx.Mode = this._Mode;
            return this.Res = Result.SUCCESS;
        }

        public void Close()
        {
            if (this.client == null)
                return;
            this.client.Close();
        }

        public string GetLastErrorString()
        {
            switch (this.Res)
            {
                case Result.SUCCESS:
                    return "Success";
                case Result.ILLEGAL_FUNCTION:
                    return "Illegal function.";
                case Result.ILLEGAL_DATA_ADDRESS:
                    return "Illegal data address.";
                case Result.ILLEGAL_DATA_VALUE:
                    return "Illegal data value.";
                case Result.SLAVE_DEVICE_FAILURE:
                    return "Slave device failure.";
                case Result.ACKNOWLEDGE:
                    return "Acknowledge.";
                case Result.SLAVE_DEVICE_BUSY:
                    return "Slave device busy.";
                case Result.NEGATIVE_ACKNOWLEDGE:
                    return "Negative acknowledge.";
                case Result.MEMORY_PARITY_ERROR:
                    return "Memory parity error.";
                case Result.CONNECT_ERROR:
                    return this.Error;
                case Result.CONNECT_TIMEOUT:
                    return "Could not connect within the specified time";
                case Result.WRITE:
                    return "Write error. " + this.TxRx.GetErrorMessage();
                case Result.READ:
                    return "Read error. " + this.TxRx.GetErrorMessage();
                case Result.RESPONSE_TIMEOUT:
                    return "Response timeout.";
                case Result.ISCLOSED:
                    return "Connection is closed.";
                case Result.CRC:
                    return "CRC Error.";
                case Result.RESPONSE:
                    return "Not the expected response received.";
                case Result.BYTECOUNT:
                    return "Byte count error.";
                case Result.QUANTITY:
                    return "Quantity is out of range.";
                case Result.FUNCTION:
                    return "Modbus function code out of range. 1 - 127.";
                case Result.DEMO_TIMEOUT:
                    return "Demo mode expired. Restart your application to continue.";
                default:
                    return "Unknown Error - " + this.Res.ToString();
            }
        }

        //public bool LicenseKey(string LicenseKey)
        //{
        //    if (LicenseKey.Length == 29)
        //    {
        //        byte[] data = new byte[25];
        //        for (int index = 0; index < 25; ++index)
        //            data[index] = (byte)LicenseKey[index];
        //        byte[] numArray = CCRC16.CRC16(data, 25);
        //        if (string.Format("{0:X2}{1:X2}", (object)numArray[1], (object)numArray[0]) == LicenseKey.Substring(25, 4))
        //        {
        //            this.TxRx.License = true;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public float RegistersToFloat(short hiReg, short loReg) => BitConverter.ToSingle(((IEnumerable<byte>)BitConverter.GetBytes(loReg)).Concat<byte>((IEnumerable<byte>)BitConverter.GetBytes(hiReg)).ToArray<byte>(), 0);

        public int RegistersToInt32(short hiReg, short loReg) => BitConverter.ToInt32(((IEnumerable<byte>)BitConverter.GetBytes(loReg)).Concat<byte>((IEnumerable<byte>)BitConverter.GetBytes(hiReg)).ToArray<byte>(), 0);

        public short[] FloatToRegisters(float value)
        {
            short[] registers = new short[2];
            byte[] bytes = BitConverter.GetBytes(value);
            registers[1] = BitConverter.ToInt16(bytes, 0);
            registers[0] = BitConverter.ToInt16(bytes, 2);
            return registers;
        }

        public short[] Int32ToRegisters(int value)
        {
            short[] registers = new short[2];
            byte[] bytes = BitConverter.GetBytes(value);
            registers[1] = BitConverter.ToInt16(bytes, 0);
            registers[0] = BitConverter.ToInt16(bytes, 2);
            return registers;
        }

        public int GetTxBuffer(byte[] byteArray) => this.TxRx.GetTxBuffer(byteArray);

        public int GetRxBuffer(byte[] byteArray) => this.TxRx.GetRxBuffer(byteArray);

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && this.components != null)
        //        this.components.Dispose();
        //    base.Dispose(disposing);
        //}

        //private void InitializeComponent() => this.components = (IContainer)new Container();
    }
}
