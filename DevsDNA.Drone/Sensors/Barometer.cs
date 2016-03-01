namespace DevsDNA.Drone.Sensors
{
    using DevsDNA.Drone.Devices.BMP085;
    using System;

    public class Barometer : IDisposable
    {
        private readonly Bmp085Client bmp085Client;
        private bool disposed;
        private static Lazy<Barometer> defaultValue = new Lazy<Barometer>(CreateDefault, true);

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static Barometer Default
        {
            get { return defaultValue.Value; }
        }

        /// <summary>
        /// Initializes the barometer.
        /// </summary>
        public Barometer()
        {
            this.bmp085Client = new Bmp085Client();
        }

        /// <summary>
        /// Initializes the barometer.
        /// </summary>
        /// <param name="slaveAddress"></param>
        public Barometer(int slaveAddress)
        {
            this.bmp085Client = new Bmp085Client(slaveAddress);
        }

        /// <summary>
        /// Initializes the barometer.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address of LPS25H.</param>
        public Barometer(string deviceId, int slaveAddress)
        {
            this.bmp085Client = new Bmp085Client(deviceId, slaveAddress);
        }

        /// <summary>
        /// Configures the default output data rate of the barometer.
        /// </summary>
        public void Initialize()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Barometer));
            }

            this.bmp085Client.WriteCtrlReg1(Bmp085OutputDataRate.Bmp085_Mode_Ultrahighres);
        }

        /// <summary>
        /// Returns pressure.
        /// </summary>
        /// <returns>The pressure in Pascal (Pa)</returns>
        public double Read()
        {
            if (disposed)
            {
                throw new ObjectDisposedException(nameof(Barometer));
            }

            double value = this.bmp085Client.ReadPressureData();

            return value;
        }

        private static Barometer CreateDefault()
        {
            var barometer = new Barometer(Commons.CommonsKeys.I2C_DEVICE_ID, Commons.CommonsKeys.BAR_DEFAULTADDRESS);
            barometer.Initialize();

            return barometer;
        }

        /// <summary>
        /// Disposes the barometer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the barometer.
        /// </summary>
        /// <param name="disposing">The value indicating whether the barometer is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.bmp085Client.Dispose();
                this.disposed = true;
            }
        }
    }
}
