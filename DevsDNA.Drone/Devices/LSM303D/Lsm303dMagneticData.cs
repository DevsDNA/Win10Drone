namespace DevsDNA.Drone.Devices.Lsm303d
{
    /// <summary>
    /// Represents 3-axis raw magnetic data from LSM303D.
    /// </summary>
    public struct Lsm303dMagneticData
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;

        /// <summary>
        /// Gets raw magnetic data for the X-axis.
        /// </summary>
        public short X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets raw magnetic data for the Y-axis.
        /// </summary>
        public short Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Gets raw magnetic data for the Z-axis.
        /// </summary>
        public short Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Initializes the 3-axis raw magnetic data.
        /// </summary>
        /// <param name="x">The raw magnetic data for the X-axis.</param>
        /// <param name="y">The raw magnetic data for the Y-axis.</param>
        /// <param name="z">The raw magnetic data for the Z-axis.</param>
        public Lsm303dMagneticData(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
