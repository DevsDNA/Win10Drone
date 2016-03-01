namespace DevsDNA.Drone.Sensors
{
    using DevsDNA.Drone.Devices.Lsm303d;
    using Structs;
    using System;

    public class Magnetometer : IDisposable
    {
        private const double SCALE = 12d;
        private readonly Lsm303dClient lsm303d;
        private bool disposed;
        private static Lazy<Magnetometer> DefaultValue = new Lazy<Magnetometer>(CreateDefault, true);
        private const float LSM303MAG_GAUSS_LSB_XY = 1100.0F;
        private const float LSM303MAG_GAUSS_LSB_Z = 980.0F;

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static Magnetometer Default
        {
            get { return DefaultValue.Value; }
        }

        /// <summary>
        /// Initializes the magetometer.
        /// </summary>
        public Magnetometer()
        {
            this.lsm303d = new Lsm303dClient();
        }

        /// <summary>
        /// Initializes the magnetometer.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address of LSM303D.</param>
        public Magnetometer(int slaveAddress)
        {
            this.lsm303d = new Lsm303dClient(slaveAddress);
        }

        /// <summary>
        /// Initializes the magnetometer.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address of LSM303D.</param>
        public Magnetometer(string deviceId, int slaveAddress)
        {
            this.lsm303d = new Lsm303dClient(deviceId, slaveAddress);
        }

        /// <summary>
        /// Configures the default magnetic resolution, output data rate, full scale and sensor mode of the magnetometer.
        /// </summary>
        public void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Magnetometer));
            }

            this.lsm303d.WriteCtrlReg5(Lsm303dMagneticResolution.High, Lsm303dMagneticOutputDataRate.Rate100Hz);
            this.lsm303d.WriteCtrlReg6(Lsm303dMagneticFullScale.Scale12G);
            this.lsm303d.WriteCtrlReg7(Lsm303dMagneticSensorMode.ContinuousConversion);
        }

        /// <summary>
        /// Returns 3-axis magnetic flux density.
        /// </summary>
        /// <returns>The 3-axis magnetic flux density in Guass (G).</returns>
        public MagneticFluxDensity Read()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(Magnetometer));
            }

            Lsm303dMagneticData data = this.lsm303d.ReadMagneticData();

            var magData = new MagneticFluxDensity(
                x: data.X / LSM303MAG_GAUSS_LSB_XY * Commons.CommonsKeys.SENSORS_GAUSS_TO_MICROTESLA,
                y: data.Y / LSM303MAG_GAUSS_LSB_XY * Commons.CommonsKeys.SENSORS_GAUSS_TO_MICROTESLA,
                z: data.Z / LSM303MAG_GAUSS_LSB_Z * Commons.CommonsKeys.SENSORS_GAUSS_TO_MICROTESLA);

            return magData;
        }

        private static Magnetometer CreateDefault()
        {
            var magnometer = new Magnetometer(Commons.CommonsKeys.I2C_DEVICE_ID, Commons.CommonsKeys.MAG_DEFAULTADDRESS);
            magnometer.Initialize();

            return magnometer;
        }

        /// <summary>
        /// Disposes the magnetometer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the magnetometer.
        /// </summary>
        /// <param name="disposing">The value indicating whether the magnetometer is disposing.</param>
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
