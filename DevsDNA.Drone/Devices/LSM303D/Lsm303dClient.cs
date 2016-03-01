namespace DevsDNA.Drone.Devices.Lsm303d
{
    using System;
    using System.Linq;
    using Windows.Devices.Enumeration;
    using Windows.Devices.I2c;

    /// <summary>
    /// Provides an I2C interface to LSM303D.
    /// </summary>
    /// <see href="https://www.pololu.com/file/download/LSM303D.pdf?file_id=0J703" />
    public class Lsm303dClient
    {
        /// <summary>
        /// The default I2C slave address.
        /// </summary>
        public const int DefaultAddress = (0x32 >> 1);

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
        /// Initializes the I2C interface using the default slave address.
        /// </summary>
        public Lsm303dClient()
            : this(DefaultAddress)
        {
        }

        /// <summary>
        /// Initializes the I2C interface.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address.</param>
        public Lsm303dClient(int slaveAddress)
        {
            this.address = slaveAddress;
            Initialize();
        }

        /// <summary>
        /// Initializes the I2C interface.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address.</param>
        public Lsm303dClient(string deviceId, int slaveAddress)
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
        /// <param name="outputDataRate">The output data rate for raw acceleratino data.</param>
        /// <param name="enableZ">The value indicating whether the Z-axis is enabled.</param>
        /// <param name="enableY">The value indicating whether the Y-axis is enabled.</param>
        /// <param name="enableX">The value indicating whether the X-axis is enabled.</param>
        public void WriteCtrlReg1(Lsm303dAccelerationOutputDataRate outputDataRate, bool enableZ = true, bool enableY = true, bool enableX = true)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }

            var value =
                ((byte)outputDataRate << 4) |
                (enableZ ? 0x1 << 2 : 0x0) |
                (enableY ? 0x1 << 1 : 0x0) |
                (enableX ? 0x1 : 0x0);

            this.device.Write(new[] { (byte)0x20, (byte)0x57 });
        }

        /// <summary>
        /// Writes the value for the CTRL_REG2 register.
        /// </summary>
        /// <param name="bandwidth">The acceleration anti-alias filter bandwidth.</param>
        /// <param name="scale">The full scale for raw acceleration data.</param>
        public void WriteCtrlReg2(Lsm303dAccelerometerAntiAliasFilterBandwidth bandwidth, Lsm303dAccelerometerFullScale scale)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }
            
            var value =
                ((byte)bandwidth << 6) |
                ((byte)scale << 3);
            
            this.device.Write(new[] { (byte)0x21, (byte)value });
        }

        /// <summary>
        /// Writes the value for the CTRL_REG5 register.
        /// </summary>
        /// <param name="magneticResolution">The magnetic resolution selection.</param>
        /// <param name="outputDataRate">The output data rate for raw magnetic data.</param>
        /// <param name="enableTemperature">The value indicating whether the thermometer is enabled.</param>
        public void WriteCtrlReg5(Lsm303dMagneticResolution magneticResolution, Lsm303dMagneticOutputDataRate outputDataRate, bool enableTemperature = false)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }

            var value =
                (enableTemperature ? 0x8 : 0x0) |
                ((byte)magneticResolution << 6) |
                ((byte)outputDataRate << 2);

            this.device.Write(new[] { (byte)0x24, (byte)value });
        }

        /// <summary>
        /// Writes the value for the CTRL_REG6 register.
        /// </summary>
        /// <param name="scale">The full scale for raw magnetic data.</param>
        public void WriteCtrlReg6(Lsm303dMagneticFullScale scale)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }
            
            this.device.Write(new[] { (byte)0x25/*CTRL_REG6*/, (byte)((byte)scale << 5) });
        }

        /// <summary>
        /// Writes the value for the CTRL_REG7 register.
        /// </summary>
        /// <param name="mode">The magnetic sensor mode selection.</param>
        public void WriteCtrlReg7(Lsm303dMagneticSensorMode mode)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }
            
            this.device.Write(new[] { (byte)0x26/*CTRL_REG7*/, (byte)mode });
        }

        /// <summary>
        /// Returns 3-axis raw acceleration data.
        /// </summary>
        /// <returns>The 3-axis raw acceleration data.</returns>
        public Lsm303dAccelerationData ReadAccelerationData()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }
            
            var buffer = new byte[6];
            this.device.WriteRead(new[] { (byte)(0x28 | 0x80) }, buffer);

            byte xlo = buffer[0];
            byte xhi = buffer[1];
            byte ylo = buffer[2];
            byte yhi = buffer[3];
            byte zlo = buffer[4];
            byte zhi = buffer[5];

            var data = new Lsm303dAccelerationData(
                x: (short)((short)(xlo | (xhi << 8)) >> 4),
                y: (short)((short)(ylo | (yhi << 8)) >> 4),
                z: (short)((short)(zlo | (zhi << 8)) >> 4)
            );

            return data;
        }

        /// <summary>
        /// Returns 3-axis raw magnetic data.
        /// </summary>
        /// <returns>The 3-axis raw magnetic data.</returns>
        public Lsm303dMagneticData ReadMagneticData()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Lsm303dClient));
            }
            
            var buffer = new byte[6];
            this.device.WriteRead(new[] { (byte)(0x8 | 0x80) }, buffer);

            byte xlo = buffer[0];
            byte xhi = buffer[1];
            byte ylo = buffer[2];
            byte yhi = buffer[3];
            byte zlo = buffer[4];
            byte zhi = buffer[5];


            var magData =  new Lsm303dMagneticData(
                x: (short)(xlo | ((short)xhi << 8)),
                y: (short)(ylo | ((short)yhi << 8)),
                z: (short)(zlo | ((short)zhi << 8))
            );

            return magData;
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
            var connection = new I2cConnectionSettings(address)
            {
                BusSpeed = I2cBusSpeed.FastMode,
                SharingMode = I2cSharingMode.Shared
            };

            this.device = I2cDevice.FromIdAsync(deviceId, connection).AsTask().Result;
        }
    }
}
