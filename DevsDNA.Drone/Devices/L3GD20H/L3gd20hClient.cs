namespace DevsDNA.Drone.Devices.L3gd20h
{
    using System;
    using System.Linq;
    using Windows.Devices.Enumeration;
    using Windows.Devices.I2c;

    /// <summary>
    /// Provides an I2C interface to L3GD20H.
    /// </summary>
    /// <see href="https://www.pololu.com/file/download/L3GD20.pdf?file_id=0J563" />
    public class L3gd20hClient : IDisposable
    {
        /// <summary>
        /// The default I2C slave address.
        /// </summary>
        public const int HighAddress = 0x6B/*0b1101011*/;

        /// <summary>
        /// The alternate I2C slave address.
        /// </summary>
        public const int LowAddress = 0x6a/*0b1101010*/;
        
        private readonly int address;
        private I2cDevice device;
        private bool disposed;
        
        /// <summary>
        /// Gets the I2C slave address.
        /// </summary>
        public int Address
        {
            get { return this.address; }
        }

        /// <summary>
        /// Initializes the I2C interface using the default slave address to L3GD20H.
        /// </summary>
        public L3gd20hClient()
            : this(HighAddress)
        {
        }
        
        /// <summary>
        /// Initializes the I2C interface to L3GD20H.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address.</param>
        public L3gd20hClient(int slaveAddress)
        {
            this.address = slaveAddress;
            Initialize();
        }

        /// <summary>
        /// Initializes the I2C interface to L3GD20H.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address.</param>
        public L3gd20hClient(string deviceId, int slaveAddress)
        {
            if (deviceId == null)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            this.address = slaveAddress;
            Initialize(deviceId);
        }

        

        /// <summary>
        /// Writes the value for the CTRL_REG1 register.
        /// </summary>
        /// <param name="outputDataRate">The output data rate for raw anglar rate data.</param>
        /// <param name="enableZ">The value indicating whether the Z-axis is enabled.</param>
        /// <param name="enableY">The value indicating whether the Y-axis is enabled.</param>
        /// <param name="enableX">The value indicating whether the X-axis is enabled.</param>
        public void WriteCtrlReg1(L3gd20hOutputDataRate outputDataRate, bool enableZ = true, bool enableY = true, bool enableX = true)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(L3gd20hClient));
            }
            
            var value =
                ((byte)outputDataRate << 6) |
                (0x2 << 4)/*Bandwidth (BW)*/ |
                (0x1 << 3)/*Power-down (PD) = Normal*/ |
                (enableZ ? 0x1 << 2 : 0x0) |
                (enableY ? 0x1 << 1 : 0x0) |
                (enableX ? 0x1 : 0x0);

            this.device.Write(new[] { (byte)0x20/*CTRL_REG1*/, (byte)value });
        }

        /// <summary>
        /// Writes the value for the CTRL_REG4 register.
        /// </summary>
        /// <param name="scale">The full scale for raw angular rate data.</param>
        public void WriteCtrlReg4(L3gd20hFullScale scale)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(L3gd20hClient));
            }

            this.device.Write(new[] { (byte)0x23/*CTRL_REG4*/, (byte)((byte)scale << 4) });
        }

        /// <summary>
        /// Returns 3-axis raw angular rate data.
        /// </summary>
        /// <returns>The 3-axis raw angular rate data.</returns>
        public L3gd20hAngularRateData Read()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(L3gd20hClient));
            }
            
            var buffer = new byte[6];
            this.device.WriteRead(new[] { (byte)(0x28/*OUTthis.x_L*/ | 0x80/*auto-increment*/) }, buffer);

            var data = new L3gd20hAngularRateData(
                x: (short)(buffer[0] | (buffer[1] << 8)),
                y: (short)(buffer[2] | (buffer[3] << 8)),
                z: (short)(buffer[4] | (buffer[5] << 8)));

            return data;
        }

        /// <summary>
        /// Disposes the I2C interface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
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
    }
}
