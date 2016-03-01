namespace DevsDNA.Drone.Sensors
{
    using DevsDNA.Drone.Devices.L3gd20h;
    using Structs;
    using System;

    public class Gyroscope : IDisposable
    {
        private const float SCALE = 1000f;
        private readonly L3gd20hClient l3gd20h;
        private bool disposed;
        private static Lazy<Gyroscope> DefaultValue = new Lazy<Gyroscope>(CreateDefault, true);

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static Gyroscope Default
        {
            get { return DefaultValue.Value; }
        }

        /// <summary>
        /// Initializes the gyroscope.
        /// </summary>
        public Gyroscope()
        {
            this.l3gd20h = new L3gd20hClient();
        }

        /// <summary>
        /// Initializes the gyroscope.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address of L3GD20H.</param>
        public Gyroscope(int slaveAddress)
        {
            this.l3gd20h = new L3gd20hClient(slaveAddress);
        }

        /// <summary>
        /// Initializes the gyroscope.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C clave address of L3GD20H.</param>
        public Gyroscope(string deviceId, int slaveAddress)
        {
            this.l3gd20h = new L3gd20hClient(deviceId, slaveAddress);
        }

        /// <summary>
        /// Configures the output data rate and full scale of the gyroscope.
        /// </summary>
        public virtual void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Gyroscope));
            }

            this.l3gd20h.WriteCtrlReg1(L3gd20hOutputDataRate.Rate760Hz);
            this.l3gd20h.WriteCtrlReg4(L3gd20hFullScale.Scale500dps);
        }

        /// <summary>
        /// Returns 3-axis rotation speed.
        /// </summary>
        /// <returns>The 3-axis rotation speed in degrees-per-second (dps).</returns>
        public AngularVelocity Read()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Gyroscope));
            }

            L3gd20hAngularRateData data = this.l3gd20h.Read();

            var angularData = new AngularVelocity(
                x: (float)data.X / short.MaxValue * SCALE,
                y: (float)data.Y / short.MaxValue * SCALE,
                z: (float)data.Z / short.MaxValue * SCALE);

            return angularData;
        }

        private static Gyroscope CreateDefault()
        {
            var gyroscope = new Gyroscope(Commons.CommonsKeys.I2C_DEVICE_ID, Commons.CommonsKeys.GYRO_DEFAULTADDRESS);
            gyroscope.Initialize();

            return gyroscope;
        }

        /// <summary>
        /// Disposes the gyroscope.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the gyroscope.
        /// </summary>
        /// <param name="disposing">The value indicating whether the gyroscope is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.l3gd20h.Dispose();
                this.disposed = true;
            }
        }
    }
}
