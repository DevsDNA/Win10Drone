namespace DevsDNA.Drone.Sensors
{
    using DevsDNA.Drone.Devices.Lsm303d;
    using DevsDNA.Drone.Structs;
    using System;

    /// <summary>
    /// The Accelerometer
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class Accelerometer : IDisposable
    {
        private const double SCALE = 0.001F;
        private readonly Lsm303dClient lsm303d;
        private bool disposed;
        private static Lazy<Accelerometer> DefaultValue = new Lazy<Accelerometer>(CreateDefault, true);

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static Accelerometer Default
        {
            get { return DefaultValue.Value; }
        }

        /// <summary>
        /// Initializes the accelerometer.
        /// </summary>
        public Accelerometer()
        {
            this.lsm303d = new Lsm303dClient();
        }

        /// <summary>
        /// Initializes the accelerometer.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address of LSM303D.</param>
        public Accelerometer(int slaveAddress)
        {
            this.lsm303d = new Lsm303dClient(slaveAddress);
        }

        /// <summary>
        /// Initializes the accelerometer.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address of LSM303D.</param>
        public Accelerometer(string deviceId, int slaveAddress)
        {
            this.lsm303d = new Lsm303dClient(deviceId, slaveAddress);
        }

        /// <summary>
        /// Configures the default output data rate, anti-alias filter bandwidth and full scale of the accelerometer.
        /// </summary>
        public virtual void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Accelerometer));
            }

            this.lsm303d.WriteCtrlReg1(Lsm303dAccelerationOutputDataRate.Rate1600Hz);
            this.lsm303d.WriteCtrlReg2(Lsm303dAccelerometerAntiAliasFilterBandwidth.Bandwidth773Hz, Lsm303dAccelerometerFullScale.Scale2g);
        }

        /// <summary>
        /// Returns 3-axis acceleration force.
        /// </summary>
        /// <returns>The 3-axis acceleration force in G-Force (g).</returns>
        public virtual GForce Read()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Accelerometer));
            }

            Lsm303dAccelerationData data = this.lsm303d.ReadAccelerationData();

            var accelerattion = new GForce(
                x: data.X * Commons.CommonsKeys.SENSORS_GRAVITY_EARTH * SCALE,
                y: data.Y * Commons.CommonsKeys.SENSORS_GRAVITY_EARTH * SCALE,
                z: data.Z * Commons.CommonsKeys.SENSORS_GRAVITY_EARTH * SCALE);

            return accelerattion;
        }

        private static Accelerometer CreateDefault()
        {
            var accelerometer = new Accelerometer(Commons.CommonsKeys.I2C_DEVICE_ID, Commons.CommonsKeys.ACCEL_DEFAULTADDRESS);
            accelerometer.Initialize();

            return accelerometer;
        }

        /// <summary>
        /// Disposes the accelerometer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the accelerometer.
        /// </summary>
        /// <param name="disposing">The value indicating whether the accelerometer is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.lsm303d.Dispose();
                this.disposed = true;
            }
        }
    }
}
