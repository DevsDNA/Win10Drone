namespace DevsDNA.Drone.Devices.BMP085
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Devices.Enumeration;
    using Windows.Devices.I2c;
    public class Bmp085Client : IDisposable
    {
        /// <summary>
        /// The default I2C slave this.address.
        /// </summary>
        public const int DEFAULTADDRESS = (0x77);

        private readonly int address;
        private I2cDevice device;
        private bool disposed;

        /// <summary>
        /// Gets the I2C slave this.address.
        /// </summary>
        public int Address
        {
            get { return this.address; }
        }

        /// <summary>
        /// Initializes the I2C interface using the default slave this.address.
        /// </summary>
        public Bmp085Client()
            : this(DEFAULTADDRESS)
        {
        }

        /// <summary>
        /// Initializes the I2C interface.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave this.address.</param>
        public Bmp085Client(int slaveAddress)
        {
            this.address = slaveAddress;
            Initialize();
        }

        /// <summary>
        /// Initializes the I2C interface.
        /// </summary>
        /// <param name="deviceId">The I2C this.device identifier.</param>
        /// <param name="slaveAddress">The I2C slave this.address.</param>
        public Bmp085Client(string deviceId, int slaveAddress)
        {
            if (deviceId == null)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            this.address = slaveAddress;
            Initialize(deviceId);
        }

       

        /// <summary>
        /// Writes the value for the CRTL_REG1 register.
        /// </summary>
        /// <param name="outputDataRate">The output data rate for raw temperature and pressure data.</param>
        /// <param name="powerDown">The value indicating whether to enter power-down mode.</param>
        public void WriteCtrlReg1(Bmp085OutputDataRate outputDataRate, bool powerDown = false)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Bmp085Client));
            }

            byte id = 0;
            read8(0xD0, ref id);
            if (id != 0x55)
            {
                
            } else
            {
                readCoefficients();
            }
        }

        /// <summary>
        /// Disposes the I2C interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Reads the pressure data.
        /// </summary>
        /// <returns></returns>
        public double ReadPressureData()
        {
            int temperature = 0, pressure = 0, barometicPressure = 0;
            int x1, x2, b5, b6, x3, b3, p;
            uint b4, b7;

            /* Get the raw pressure and temperature values */
            temperature = ReadTemperatureData();
            pressure = ReadPressureDataFromSensor();

            /* Temperature compensation */
            b5 = computeB5(temperature);

            /* Pressure compensation */
            b6 = b5 - 4000;
            x1 = (this.bmp085_coeffs.b2 * ((b6 * b6) >> 12)) >> 11;
            x2 = (this.bmp085_coeffs.ac2 * b6) >> 11;
            x3 = x1 + x2;
            b3 = (((((int)this.bmp085_coeffs.ac1) * 4 + x3) << (byte)Bmp085OutputDataRate.Bmp085_Mode_Ultrahighres) + 2) >> 2;
            x1 = (this.bmp085_coeffs.ac3 * b6) >> 13;
            x2 = (this.bmp085_coeffs.b1 * ((b6 * b6) >> 12)) >> 16;
            x3 = ((x1 + x2) + 2) >> 2;
            b4 = (this.bmp085_coeffs.ac4 * (uint)(x3 + 32768)) >> 15;
            b7 = ((uint)(pressure - b3) * ((uint)50000 >> (byte)Bmp085OutputDataRate.Bmp085_Mode_Ultrahighres));

            if (b7 < 0x80000000)
            {
                p = (int)((b7 << 1) / b4);
            }
            else
            {
                p = (int)((b7 / b4) << 1);
            }

            x1 = (p >> 8) * (p >> 8);
            x1 = (x1 * 3038) >> 16;
            x2 = (-7357 * p) >> 16;
            barometicPressure = p + ((x1 + x2 + 3791) >> 4);

            /* Assign compensated pressure value */
            return barometicPressure;
        }


        /// <summary>
        /// Returns raw temperature data.
        /// </summary>
        /// <returns>Raw temperature data.</returns>
        public short ReadTemperatureData()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Bmp085Client));
            }
            this.device.Write(new byte[] { 0xF4, 0x2E });
            Task.Delay(5).Wait();

            var buffer = new byte[2];
            this.device.WriteRead(new[] { (byte)(0xF6) }, buffer);

            return (short)((buffer[0] << 8) | (buffer[1]));
        }

        private void read8(byte reg, ref byte value)
        {
            byte[] addr = new byte[] { reg };
            byte[] data = new byte[1];
            this.device.WriteRead(addr, data);
            value = data[0];
        }

        private void read16(byte reg, ref ushort value)
        {
            byte[] addr = new byte[] { reg };
            byte[] data = new byte[2];
            this.device.WriteRead(addr, data);
            value = (ushort)((data[0] << 8) | (data[1]));
        }

        private void readS16(byte reg, ref short value)
        {
            ushort i = 0;
            read16(reg, ref i);
            value = (short)i;
        }

        private Bmp085CalibrationData bmp085_coeffs;

        private void readCoefficients()
        {
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC1, ref this.bmp085_coeffs.ac1);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC2, ref this.bmp085_coeffs.ac2);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC3, ref this.bmp085_coeffs.ac3);
            read16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC4, ref this.bmp085_coeffs.ac4);
            read16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC5, ref this.bmp085_coeffs.ac5);
            read16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_AC6, ref this.bmp085_coeffs.ac6);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_B1, ref this.bmp085_coeffs.b1);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_B2, ref this.bmp085_coeffs.b2);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_MB, ref this.bmp085_coeffs.mb);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_MC, ref this.bmp085_coeffs.mc);
            readS16(Bmp085CalibrationDataConst.BMP085_REGISTER_CAL_MD, ref this.bmp085_coeffs.md);
        }

        private void Initialize()
        {
            string query = I2cDevice.GetDeviceSelector("I2C1");
            DeviceInformationCollection deviceInfos = DeviceInformation.FindAllAsync(query).AsTask().Result;
            string deviceId = deviceInfos.FirstOrDefault()?.Id;

            if (deviceId == null)
            {
                throw new NotSupportedException();
            }

            Initialize(deviceId);
        }

        private void Initialize(string deviceId)
        {
            var connection = new I2cConnectionSettings(this.address)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Shared
            };

            this.device = I2cDevice.FromIdAsync(deviceId, connection).AsTask().Result;
        }

        private int ReadPressureDataFromSensor()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Bmp085Client));
            }
            this.device.Write(new byte[] { 0xF4, (byte)0x34 + (((byte)Bmp085OutputDataRate.Bmp085_Mode_Ultrahighres << (byte)6)) });
            Task.Delay(26).Wait();

            var buffer = new byte[2];
            this.device.WriteRead(new[] { (byte)(0xF6) }, buffer);
            var presure16Bit = (ushort)((buffer[0] << 8) | (buffer[1]));
            var presure32 = (int)((uint)presure16Bit << 8);


            byte[] data = new byte[1];
            this.device.WriteRead(new[] { (byte)(0xF6) }, data);
            var presure8 = data[0];
            presure32 += presure8;
            presure32 >>= (8 - (byte)Bmp085OutputDataRate.Bmp085_Mode_Ultrahighres);


            return presure32;
        }

       

        /// <summary>
        /// Disposes the I2C interface.
        /// </summary>
        /// <param name="disposing">The value indicating whether the I2C interface is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.device.Dispose();
                this.disposed = true;
            }
        }

        private int computeB5(int ut)
        {
            int X1 = (ut - (int)this.bmp085_coeffs.ac6) * ((int)this.bmp085_coeffs.ac5) >> 15;
            int X2 = ((int)this.bmp085_coeffs.mc << 11) / (X1 + (int)this.bmp085_coeffs.md);
            return X1 + X2;
        }
    }
}