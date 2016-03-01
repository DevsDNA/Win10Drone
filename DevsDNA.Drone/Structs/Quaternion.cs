namespace DevsDNA.Drone.Structs
{
    using System;

    public struct Quaternion
    {
        public static Quaternion Empty = new Quaternion(0d, 0d, 0d, 1d);

        private readonly double w;
        private readonly double x;
        private readonly double y;
        private readonly double z;
        private readonly double magnitude;

        /// <summary>
        /// </summary>
        /// <value>
        /// The w.
        /// </value>
        public double W
        {
            get { return this.w; }
        }

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
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="vector">The vector.</param>
        public Quaternion(double w, Vector3D vector)
            : this(w, vector.X, vector.Y, vector.Z)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quaternion"/> struct.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="z">The z.</param>
        public Quaternion(double w, double x, double y, double z)
        {
            this.w = w;
            this.x = x;
            this.y = y;
            this.z = z;
            this.magnitude = Math.Sqrt(this.w * this.w + this.x * this.x + this.y * this.y + this.z * this.z);
        }

        /// <summary>
        /// Normalizes this instance.
        /// </summary>
        /// <returns></returns>
        public Quaternion Normalize()
        {
            if (this.magnitude == 0d)
            {
                return new Quaternion(this.w, this.x, this.y, this.z);
            }

            return new Quaternion(
                w: this.w / this.magnitude,
                x: this.x / this.magnitude,
                y: this.y / this.magnitude,
                z: this.z / this.magnitude);
        }

        /// <summary>
        /// Froms the axis angle.
        /// </summary>
        /// <param name="w">The w.</param>
        /// <param name="theta">The theta.</param>
        /// <returns></returns>
        public static Quaternion FromAxisAngle(Vector3D w, double theta)
        {
            double cosThetaOverTwo = Math.Cos(theta / 2d);
            double sinThetaOverTwo = Math.Sin(theta / 2d);

            return new Quaternion(
                w: cosThetaOverTwo,
                x: sinThetaOverTwo * w.X,
                y: sinThetaOverTwo * w.Y,
                z: sinThetaOverTwo * w.Z);
        }

        /// <summary>
        /// Froms the euler angles.
        /// </summary>
        /// <param name="angles">The angles.</param>
        /// <returns></returns>
        public static Quaternion FromEulerAngles(EulerAngles angles)
        {
            double cosRoll = Math.Cos(angles.Roll / 2d);
            double sinRoll = Math.Sin(angles.Roll / 2d);
            double cosPitch = Math.Cos(angles.Pitch / 2d);
            double sinPitch = Math.Sin(angles.Pitch / 2d);
            double cosYaw = Math.Cos(angles.Yaw / 2d);
            double sinYaw = Math.Sin(angles.Yaw / 2d);

            return new Quaternion(
                w: cosRoll * cosPitch * cosYaw + sinRoll * sinPitch * sinYaw,
                x: sinRoll * cosPitch * cosYaw - cosRoll * sinPitch * sinYaw,
                y: cosRoll * sinPitch * cosYaw + sinRoll * cosPitch * sinYaw,
                z: cosRoll * cosPitch * sinYaw - sinRoll * sinPitch * cosYaw);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static Quaternion operator +(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                w: left.w + right.w,
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
        public static Quaternion operator -(Quaternion left, Quaternion right)
        {
            return new Quaternion(
                w: left.w - right.w,
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
        public static Quaternion operator *(Quaternion self, double value)
        {
            return new Quaternion(
                w: self.w * value,
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
        public static Quaternion operator /(Quaternion self, double value)
        {
            return new Quaternion(
                w: self.w / value,
                x: self.x / value,
                y: self.y / value,
                z: self.z / value);
        }

        /// <summary>
        /// Conjugates this instance.
        /// </summary>
        /// <returns></returns>
        public Quaternion Conjugate()
        {
            return new Quaternion(
                w: this.w,
                x: -this.x,
                y: -this.y,
                z: -this.z);
        }

        /// <summary>
        /// Multiplies the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public Quaternion Multiply(Quaternion value)
        {
            return new Quaternion(
                w: this.w * value.w - this.x * value.x - this.y * value.y - this.z * value.z,
                x: this.w * value.x + this.x * value.w + this.y * value.z - this.z * value.y,
                y: this.w * value.y - this.x * value.z + this.y * value.w + this.z * value.x,
                z: this.w * value.z + this.x * value.y - this.y * value.x + this.z * value.w);
        }

        /// <summary>
        /// Dots the product.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public double DotProduct(Quaternion value)
        {
            return this.w * value.w + this.x * value.x + this.y * value.y + this.z * value.z;
        }

        /// <summary>
        /// Scales the specified scale.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public Quaternion Scale(double scale)
        {
            return new Quaternion(
                w: this.w / this.magnitude * scale,
                x: this.x / this.magnitude * scale,
                y: this.y / this.magnitude * scale,
                z: this.z / this.magnitude * scale);
        }

        /// <summary>
        /// Slerps the specified end.
        /// </summary>
        /// <param name="end">The end.</param>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public Quaternion Slerp(Quaternion end, double t)
        {
            Quaternion endAcute = end;
            double dot = DotProduct(end);

            if (dot < 0d)
            {
                endAcute = new Quaternion(
                    w: -end.w,
                    x: -end.x,
                    y: -end.y,
                    z: -end.z);
            }

            double theta = Math.Acos(dot);
            double slerpPitch = Math.Sin((1d - t) * theta);
            Quaternion q1 = this * slerpPitch;

            double slerpRoll = Math.Sin(t * theta);
            Quaternion q2 = endAcute * slerpRoll;

            Quaternion qt = q1 + q2;
            return qt * 1d / Math.Sin(theta);
        }

        /// <summary>
        /// Slerps the angle.
        /// </summary>
        /// <param name="end">The end.</param>
        /// <param name="omega">The omega.</param>
        /// <returns></returns>
        public Quaternion SlerpAngle(Quaternion end, double omega)
        {
            Quaternion endAcute = end;
            double dot = DotProduct(end);

            if (dot < 0d)
            {
                endAcute = new Quaternion(
                    w: -end.w,
                    x: -end.x,
                    y: -end.y,
                    z: -end.z);
            }
            else if (dot > 1d)
            {
                dot = 1d;
            }

            double theta = Math.Acos(dot);

            if (theta == 0d)
            {
                return end;
            }

            double t = omega / theta;
            if (t > 1d) t = 1d;

            double slerpPitch = Math.Sin((1d - t) * theta);
            Quaternion q1 = this * slerpPitch;

            double slerpRoll = Math.Sin(t * theta);
            Quaternion q2 = endAcute * slerpRoll;

            Quaternion qt = q1 + q2;
            return qt * 1d / Math.Sin(theta);
        }

        /// <summary>
        /// Gets the axis.
        /// </summary>
        /// <returns></returns>
        public Vector3D GetAxis()
        {
            double scale = 1d / Math.Sqrt(1d - this.w * this.w);

            return new Vector3D(
                x: this.x * scale,
                y: this.y * scale,
                z: this.z * scale);
        }

        /// <summary>
        /// Gets the angle.
        /// </summary>
        /// <returns></returns>
        public double GetAngle()
        {
            return 2d * Math.Acos(this.w);
        }

        /// <summary>
        /// To the euler angles.
        /// </summary>
        /// <returns></returns>
        public EulerAngles ToEulerAngles()
        {
            double sw = this.w * this.w;
            double sx = this.x * this.x;
            double sy = this.y * this.y;
            double sz = this.z * this.z;

            return new EulerAngles(
                pitch: Math.Asin(-2d * (this.x * this.z - this.y * this.w) / (sx + sy + sz + sw)),
                roll: Math.Atan2(2d * (this.y * this.z + this.x * this.w), -sx - sy + sz + sw),
                yaw: Math.Atan2(2d * (this.x * this.y + this.z * this.w), sx - sy - sz + sw));
        }
    }
}
