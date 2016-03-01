namespace DevsDNA.Drone.Structs
{
    /// <summary>
    /// GForce struct
    /// </summary>
    public struct GForce
    {
        private Vector3D value;

        /// <summary>
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X
        {
            get { return this.value.X; }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public double Y
        {
            get { return this.value.Y; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public double Z
        {
            get { return this.value.Z; }
        }

        /// <summary>
        /// Gets the magnitude.
        /// </summary>
        /// <value>
        /// The magnitude.
        /// </value>
        public double Magnitude
        {
            get { return this.value.Magnitude; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GForce"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public GForce(double x, double y, double z)
            : this(new Vector3D(x, y, z))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GForce"/> struct.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="magnitude">The magnitude.</param>
        public GForce(Vector3D direction, double magnitude)
        {
            this.value = direction.Scale(magnitude);
        }

        private GForce(Vector3D value)
        {
            this.value = value;
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static GForce operator +(GForce left, GForce right)
        {
            return new GForce(left.value + right.value);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static GForce operator -(GForce left, GForce right)
        {
            return new GForce(left.value - right.value);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static GForce operator *(GForce self, double value)
        {
            return new GForce(self.value * value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static GForce operator /(GForce self, double value)
        {
            return new GForce(self.value / value);
        }

        /// <summary>
        /// Normalizes this instance.
        /// </summary>
        /// <returns></returns>
        public Vector3D Normalize()
        {
            return this.value.Normalize();
        }
    }
}
