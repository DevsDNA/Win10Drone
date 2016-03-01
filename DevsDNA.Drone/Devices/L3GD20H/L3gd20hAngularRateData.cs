namespace DevsDNA.Drone.Devices.L3gd20h
{
    /// <summary>
    /// Represents 3-axis raw angular rate data from L3GD20H.
    /// </summary>
    public struct L3gd20hAngularRateData
    {
        private readonly short x;
        private readonly short y;
        private readonly short z;

        /// <summary>
        /// Gets the raw angular rate data for the X-axis.
        /// </summary>
        public short X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets the raw angular rate data for the Y-axis.
        /// </summary>
        public short Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// Gets the raw angular rate data for the Z-axis.
        /// </summary>
        public short Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Initializes the 3-axis angular rate data.
        /// </summary>
        /// <param name="x">The raw angular rate data for the X-axis.</param>
        /// <param name="y">The raw angular rate data for the Y-axis.</param>
        /// <param name="z">The raw angular rate data for the Z-axis.</param>
        public L3gd20hAngularRateData(short x, short y, short z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
