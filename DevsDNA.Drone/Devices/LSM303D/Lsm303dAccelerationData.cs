namespace DevsDNA.Drone.Devices.Lsm303d
{
    /// <summary>
    /// Represents 3-axis raw acceleration data from LSM303D.
    /// </summary>
    public struct Lsm303dAccelerationData
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;

        /// <summary>
        /// Gets the raw acceleration data for the X-axis.
        /// </summary>
        public short X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets the raw acceleratino data for the Y-axis.
        /// </summary>
        public short Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Gets the raw acceleration data for the Z-axis.
        /// </summary>
        public short Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Initializes the 3-axis acceleratin data.
        /// </summary>
        /// <param name="x">The raw acceleration data for the X-axis.</param>
        /// <param name="y">The raw acceleration data for the Y-axis.</param>
        /// <param name="z">The raw acceleration data for the Z-axis.</param>
        public Lsm303dAccelerationData(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
