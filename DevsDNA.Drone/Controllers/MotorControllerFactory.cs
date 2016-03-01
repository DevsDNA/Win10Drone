namespace DevsDNA.Drone.Controllers
{
    using Devices;
    using System;

    /// <summary>
    /// Provides motor controllers for PCA9685.
    /// </summary>
    public sealed class MotorControllerFactory : IDisposable
    {
        private const int FREQUENCY = 400;
        private readonly PCA9685Client pca9685;
        private bool disposed;
        private static Lazy<MotorControllerFactory> _default = new Lazy<MotorControllerFactory>(CreateDefault, true);

        /// <summary>
        /// Gets the default instance.
        /// </summary>
        public static MotorControllerFactory Default
        {
            get { return _default.Value; }
        }

        /// <summary>
        /// Initialzes the factory.
        /// </summary>
        public MotorControllerFactory()
        {
            this.pca9685 = new PCA9685Client();
        }

        /// <summary>
        /// Initializes the factory.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address of PCA9685.</param>
        public MotorControllerFactory(int slaveAddress)
        {
            this.pca9685 = new PCA9685Client(slaveAddress);
        }

        /// <summary>
        /// Initializes the factory.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address of PCA9685.</param>
        public MotorControllerFactory(string deviceId, int slaveAddress)
        {
            this.pca9685 = new PCA9685Client(deviceId, slaveAddress);
        }

        /// <summary>
        /// Sets the frequency of PCA9685.
        /// </summary>
        public void Initialize()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(MotorControllerFactory));
            }

            this.pca9685.SetFrequency(FREQUENCY);
        }

        /// <summary>
        /// Returns a motor controller for the specified pin number.
        /// </summary>
        /// <param name="number">The pin number.</param>
        /// <returns>The motor controller.</returns>
        public MotorController Create(int number)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(MotorControllerFactory));
            }

            return new MotorController(pca9685, FREQUENCY, number);
        }

        private static MotorControllerFactory CreateDefault()
        {
            var factory = new MotorControllerFactory(Commons.CommonsKeys.I2C_DEVICE_ID, Commons.CommonsKeys.PCA9685_DEFAULTADDRESS);
            factory.Initialize();

            return factory;
        }

        /// <summary>
        /// Disposes the factory.
        /// </summary>
        public void Dispose()
        {
            this.pca9685.Dispose();
            this.disposed = true;
        }
    }
}
