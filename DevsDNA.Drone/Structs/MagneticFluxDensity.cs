namespace DevsDNA.Drone.Structs
{
    public struct MagneticFluxDensity
    {
        private readonly Vector3D value;

        /// <summary>
        /// Gets the magnetic flux density along the X-axis, in Gauss (g).
        /// </summary>
        public double X
        {
            get { return this.value.X; }
        }

        /// <summary>
        /// Gets the magnetic flux density along the Y-axis, in Gauss (g).
        /// </summary>
        public double Y
        {
            get { return this.value.Y; }
        }

        /// <summary>
        /// Gets the magnetic flux density along the Z-axis, in Gauss (g).
        /// </summary>
        public double Z
        {
            get { return this.value.Z; }
        }

        public double Magnitude
        {
            get { return this.value.Magnitude; }
        }

        /// <summary>
        /// Initializes the 3-axis magnetic flux density.
        /// </summary>
        /// <param name="x">The magnetic flux density along the X-axis, in Gauss (g).</param>
        /// <param name="y">The magnetic flux density along the Y-axis, in Gauss (g).</param>
        /// <param name="z">The magnetic flux density along the Z-axis, in Gauss (g).</param>
        public MagneticFluxDensity(double x, double y, double z)
            : this(new Vector3D(x, y, z))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MagneticFluxDensity"/> struct.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <param name="magnitude">The magnitude.</param>
        public MagneticFluxDensity(Vector3D direction, double magnitude)
        {
            this.value = direction.Scale(magnitude);
        }

        private MagneticFluxDensity(Vector3D value)
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
        public static MagneticFluxDensity operator +(MagneticFluxDensity left, MagneticFluxDensity right)
        {
            return new MagneticFluxDensity(left.value + right.value);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static MagneticFluxDensity operator -(MagneticFluxDensity left, MagneticFluxDensity right)
        {
            return new MagneticFluxDensity(left.value - right.value);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static MagneticFluxDensity operator *(MagneticFluxDensity self, double value)
        {
            return new MagneticFluxDensity(self.value * value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static MagneticFluxDensity operator /(MagneticFluxDensity self, double value)
        {
            return new MagneticFluxDensity(self.value / value);
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
