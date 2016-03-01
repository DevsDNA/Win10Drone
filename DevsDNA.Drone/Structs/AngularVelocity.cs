namespace DevsDNA.Drone.Structs
{
    public struct AngularVelocity
    {
        private Vector3D value;

        /// <summary>
        /// Gets the speed of rotation around the X-axis, in degrees-per-second (dps).
        /// </summary>
        public double X
        {
            get { return this.value.X; }
        }

        /// <summary>
        /// Gets the speed of rotation around the Y-axis, in degrees-per-second (dps).
        /// </summary>
        public double Y
        {
            get { return this.value.Y; }
        }

        /// <summary>
        /// Gets the speed of rotation around the Z-axis, in degrees-per-second (dps).
        /// </summary>
        public double Z
        {
            get { return this.value.Z; }
        }

        /// <summary>
        /// Gets the magnitude of rotation.
        /// </summary>
        public double Magnitude
        {
            get { return this.value.Magnitude; }
        }

        /// <summary>
        /// Initializes the 3-axis rotation speed.
        /// </summary>
        /// <param name="x">The speed of rotation around the X-axis, in degrees-per-second (dps).</param>
        /// <param name="y">The speed of rotation around the Y-axis, in degrees-per-second (dps).</param>
        /// <param name="z">The speed of rotation around the Z-axis, in degrees-per-second (dps).</param>
        public AngularVelocity(double x, double y, double z)
            : this(new Vector3D(x, y, z))
        {
        }

        private AngularVelocity(Vector3D value)
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
        public static AngularVelocity operator +(AngularVelocity left, AngularVelocity right)
        {
            return new AngularVelocity(left.value + right.value);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static AngularVelocity operator -(AngularVelocity left, AngularVelocity right)
        {
            return new AngularVelocity(left.value - right.value);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static AngularVelocity operator *(AngularVelocity self, double value)
        {
            return new AngularVelocity(self.value * value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static AngularVelocity operator /(AngularVelocity self, double value)
        {
            return new AngularVelocity(self.value / value);
        }

        /// <summary>
        /// Returns a 3-axis unit vector that represents the direction of rotation.
        /// </summary>
        /// <returns>The 3-axis unit vector.</returns>
        public Vector3D Normalize()
        {
            return this.value.Normalize();
        }
    }
}
