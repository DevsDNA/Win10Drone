namespace DevsDNA.Drone.Devices
{
    using System;
    using System.Linq;
    using Windows.Devices.Enumeration;
    using Windows.Devices.I2c;

    public class PCA9685Client : IDisposable
    {
        /// <summary>
        /// The default I2C slave this.address.
        /// </summary>
        public const int DEFAULTADDRESS = 0x40;

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
        public PCA9685Client()
            : this(DEFAULTADDRESS)
        {
        }

        /// <summary>
        /// Initializes the I2C interdace.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave this.address.</param>
        public PCA9685Client(int slaveAddress)
        {
            this.address = slaveAddress;
            Initialize();
        }

        /// <summary>
        /// Initializes the I2C interface.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave this.address.</param>
        public PCA9685Client(string deviceId, int slaveAddress)
        {
            if (deviceId == null)
            {
                throw new ArgumentNullException(nameof(deviceId));
            }

            this.address = slaveAddress;
            Initialize(deviceId);
        }

        /// <summary>
        /// Sets the frequency for all outputs.
        /// </summary>
        /// <param name="frequency">The number of pulses per second (Hz).</param>
        public void SetFrequency(int frequency)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(PCA9685Client));
            }

            this.device.Write(new[] { (byte)0x0/*MODE1*/, (byte)0x10/*SLEEP = 1 (Low Power Mode)*/ });

            int prescale = (int)Math.Round(25000000f / (4096f * frequency), MidpointRounding.AwayFromZero) - 1;
            this.device.Write(new[] { (byte)0xfe/*PRE_SCALE*/, (byte)prescale });

            this.device.Write(new[] { (byte)0x0/*MODE1*/, (byte)0x0/*SLEEP = 0 (Normal Mode)*/ });

            const int ticksPerMicrosecond = (int)(TimeSpan.TicksPerMillisecond / 1000);

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            while (sw.ElapsedTicks < 5 * ticksPerMicrosecond) { }

            this.device.Write(new[] { (byte)0x0/*MODE1*/, (byte)0xa0/*RESTART = 1 (Enabled), AI (auto-increment) = 1 (Enabled)*/ });

            var buffer = new byte[1];
            this.device.WriteRead(new[] { (byte)0xfe/*PRE_SCALE*/ }, buffer);
        }

        /// <summary>
        /// Sets the duty cycle for pulses generated at the given pin number.
        /// </summary>
        /// <remarks>
        /// For exampe, given frequency set at 50Hz (20ms cycles), a value of 0 for high and a value of 102 for low would yield duty cycles from approximately 0ms to 0.5ms.
        /// </remarks>
        /// <param name="number">The pin number to set.</param>
        /// <param name="high">The cyclic position at which pusles must transition to high, specified as a value from 0 (at beginning of cycle) to 4095 (at end of cycle).</param>
        /// <param name="low">The cyclic position at which pusles must transition to low, specified as a value from 0 (at beginning of cycle) to 4095 (at end of cycle).</param>
        public void SetValue(int number, int high, int low)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(PCA9685Client));
            }

            if (number < 0 || number > 15)
            {
                throw new ArgumentOutOfRangeException(nameof(number));
            }

            if (high < 0 || high > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(high));
            }

            if (low < 0 || low > 4095)
            {
                throw new ArgumentOutOfRangeException(nameof(low));
            }

            if (low < high)
            {
                throw new ArgumentException(nameof(low));
            }

            int register = 0x6/*LED0_ON_L*/ + 4 * number;
            this.device.Write(new[] { (byte)(register), (byte)(0xff & high), (byte)(0xf & (high >> 8)), (byte)(0xff & low), (byte)(0xf & (low >> 8)) });
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
            try
            {
                this.device = I2cDevice.FromIdAsync(deviceId, connection).AsTask().Result;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
