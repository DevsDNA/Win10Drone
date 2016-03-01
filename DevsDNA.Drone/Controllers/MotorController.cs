namespace DevsDNA.Drone.Controllers
{
    using Devices;
    using System;

    /// <summary>
    /// Provides control over the speed of a motor.
    /// </summary>
    public class MotorController : IDisposable
    {
        private const int SCALE = 4095;
        private const double DUTYCYCLE = 1d;
        private readonly PCA9685Client pca9685;
        private readonly bool pca9685IsExternal;
        private readonly int frequency;
        private readonly int number;
        private bool disposed;

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="frequency">The frequency of PCA9685 used to calculate duty cycles.</param>
        /// <param name="number">The pin number to control.</param>
        public MotorController(int frequency, int number)
        {
            this.pca9685 = new PCA9685Client();
            this.frequency = frequency;
            this.number = number;
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="slaveAddress">The I2C slave address of PCA9685.</param>
        /// <param name="frequency">The frequency of PCA9685 used to calculate duty cycles.</param>
        /// <param name="number">The pin number to control.</param>
        public MotorController(int slaveAddress, int frequency, int number)
        {
            this.pca9685 = new PCA9685Client(slaveAddress);
            this.frequency = frequency;
            this.number = number;
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="deviceId">The I2C device identifier.</param>
        /// <param name="slaveAddress">The I2C slave address of PCA9685.</param>
        /// <param name="frequency">The frequency of PCA9685 used to calculate duty cycles.</param>
        /// <param name="number">The pin number to control.</param>
        public MotorController(string deviceId, int slaveAddress, int frequency, int number)
        {
            this.pca9685 = new PCA9685Client(deviceId, slaveAddress);
            this.frequency = frequency;
            this.number = number;
        }

        /// <summary>
        /// Initializes the controller.
        /// </summary>
        /// <param name="pca9685">The I2C interface to PCA9685.</param>
        /// <param name="frequency">The frequency of PCA9685 used to calculate duty cycles.</param>
        /// <param name="number">The pin number to control.</param>
        protected internal MotorController(PCA9685Client pca9685, int frequency, int number)
        {
            this.pca9685IsExternal = true;
            this.pca9685 = pca9685;
            this.frequency = frequency;
            this.number = number;
        }

        /// <summary>
        /// Sets the speed of the motor.
        /// </summary>
        /// <param name="value">The speed of the motor, specified as a value between 0 (stopped) and 1 (full speed).</param>
        public void SetSpeed(double value)
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(nameof(MotorController));
            }

            if (value < 0d || value > 1d)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            double wavelength = 1000d / frequency;
            int low = (int)((1d + value * DUTYCYCLE) / wavelength * SCALE);

            this.pca9685.SetValue(number, 0, low);
        }

        /// <summary>
        /// Disposes the controller.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Disposes the controller.
        /// </summary>
        /// <param name="disposing">The value indicating whether the controller is disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (!this.pca9685IsExternal)
                {
                    this.pca9685.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
