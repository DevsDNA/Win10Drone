namespace DevsDNA.Drone.Structs
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Pitch={Pitch}, Roll={Roll}, Yaw={Yaw}")]
    public struct EulerAngles
    {
        public static readonly EulerAngles Zero = new EulerAngles(0d, 0d, 0d);

        private readonly double pitch;
        private readonly double roll;
        private readonly double yaw;

        /// <summary>
        /// Gets the pitch.
        /// </summary>
        /// <value>
        /// The pitch.
        /// </value>
        public double Pitch
        {
            get { return this.pitch; }
        }

        /// <summary>
        /// Gets the roll.
        /// </summary>
        /// <value>
        /// The roll.
        /// </value>
        public double Roll
        {
            get { return this.roll; }
        }

        /// <summary>
        /// Gets the yaw.
        /// </summary>
        /// <value>
        /// The yaw.
        /// </value>
        public double Yaw
        {
            get { return this.yaw; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EulerAngles"/> struct.
        /// </summary>
        /// <param name="pitch">The pitch.</param>
        /// <param name="roll">The roll.</param>
        /// <param name="yaw">The yaw.</param>
        public EulerAngles(double pitch, double roll, double yaw)
        {
            this.pitch = pitch;
            this.roll = roll;
            this.yaw = yaw;
        }

        /// <summary>
        /// Froms the vector.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static EulerAngles FromVector(Vector3D value)
        {
            return FromVector(value, 0d);
        }

        // http://www.freescale.com/files/sensors/doc/app_note/AN3461.pdf
        /// <summary>
        /// Froms the vector.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="miu">The miu.</param>
        /// <returns></returns>
        public static EulerAngles FromVector(Vector3D value, double miu)
        {
            double signOfZ = value.Z < 0d ? -1d : 1d;
            //double roll = Math.Atan2(-value.X, signOfZ * Math.Sqrt(value.Z * value.Z + miu * value.X * value.X));
            //double pitch = Math.Atan2(value.Y, Math.Sqrt(value.X * value.X + value.Z * value.Z));
            double roll;
            double pitch;
            double t_pitch;
            double t_roll;

            t_roll = value.X * value.X + value.Z * value.Z;
            roll = Math.Atan2(value.Y, Math.Sqrt(t_roll)) * 180 / Math.PI;

            t_pitch = value.Y * value.Y + value.Z * value.Z;
            pitch = Math.Atan2(value.X, signOfZ * Math.Sqrt(t_pitch)) * 180 / Math.PI;


            return new EulerAngles(pitch, roll, 0.0);

        }

        /// <summary>
        /// To the vector.
        /// </summary>
        /// <returns></returns>
        public Vector3D ToVector()
        {
            double x = Math.Cos(this.pitch) * -Math.Sin(this.roll);
            double y = Math.Sin(this.pitch);
            double z = Math.Sqrt(1d - x * x - y * y);

            return new Vector3D(x, y, z);
        }
    }
}
