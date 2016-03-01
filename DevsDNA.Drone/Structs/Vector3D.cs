namespace DevsDNA.Drone.Structs
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("X={X}, Y={Y}, Z={Z}, Magnitude={Magnitude}")]
    public struct Vector3D
    {
        private readonly double x;
        private readonly double y;
        private readonly double z;
        private readonly double magnitude;

        /// <summary>
        /// </summary>
        /// <value>
        /// The x.
        /// </value>
        public double X
        {
            get { return this.x; }
        }

        /// <summary>
        /// Gets the y.
        /// </summary>
        /// <value>
        /// The y.
        /// </value>
        public double Y
        {
            get { return this.y; }
        }

        /// <summary>
        /// </summary>
        /// <value>
        /// The z.
        /// </value>
        public double Z
        {
            get { return this.z; }
        }

        /// <summary>
        /// Gets the magnitude.
        /// </summary>
        /// <value>
        /// The magnitude.
        /// </value>
        public double Magnitude
        {
            get { return this.magnitude; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Vector3D"/> struct.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Vector3D(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.magnitude = Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        /// <summary>
        /// Normalizes this instance.
        /// </summary>
        /// <returns></returns>
        public Vector3D Normalize()
        {
            if (this.magnitude == 0d)
            {
                return new Vector3D(this.x, this.y, this.z);
            }

            return new Vector3D(
                x: this.x / this.magnitude,
                y: this.y / this.magnitude,
                z: this.z / this.magnitude);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vector3D operator +(Vector3D left, Vector3D right)
        {
            return new Vector3D(
                x: left.x + right.x,
                y: left.y + right.y,
                z: left.z + right.z);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vector3D operator -(Vector3D left, Vector3D right)
        {
            return new Vector3D(
                x: left.x - right.x,
                y: left.y - right.y,
                z: left.z - right.z);
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vector3D operator *(Vector3D self, double value)
        {
            return new Vector3D(
                x: self.x * value,
                y: self.y * value,
                z: self.z * value);
        }

        /// <summary>
        /// Implements the operator /.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Vector3D operator /(Vector3D self, double value)
        {
            return new Vector3D(
                x: self.x / value,
                y: self.y / value,
                z: self.z / value);
        }

        /// <summary>
        /// Dots the product.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double DotProduct(Vector3D value)
        {
            return this.x * value.x + this.y * value.y + this.z * value.z;
        }

        /// <summary>
        /// Crosses the product.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Vector3D CrossProduct(Vector3D value)
        {
            return new Vector3D(
                x: this.y * value.z - this.z * value.y,
                y: this.z * value.x - this.x * value.z,
                z: this.x * value.y - this.y * value.x);
        }

        /// <summary>
        /// Scales the specified scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public Vector3D Scale(double scale)
        {
            return new Vector3D(
                x: this.x / this.magnitude * scale,
                y: this.y / this.magnitude * scale,
                z: this.z / this.magnitude * scale);
        }
    }
}
