namespace DevsDNA.Drone.Extensions
{
    using DevsDNA.Drone.Structs;
    using System;

    /// <summary>
    /// MagneticFluxDensityExtensions
    /// </summary>
    public static class MagneticFluxDensityExtensions
    {
        /// <summary>
        /// Shifts the specified minimum x.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minX">The minimum x.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="minY">The minimum y.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <param name="minZ">The minimum z.</param>
        /// <param name="maxZ">The maximum z.</param>
        /// <returns></returns>
        public static MagneticFluxDensity Shift(this MagneticFluxDensity value, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            return new MagneticFluxDensity(
                x: 2d * (value.X - minX) / (maxX - minX) - 1d,
                y: 2d * (value.Y - minY) / (maxY - minY) - 1d,
                z: 2d * (value.Z - minZ) / (maxZ - minZ) - 1d);
        }

        /// <summary>
        /// To the orientation.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="pitch">The pitch.</param>
        /// <param name="roll">The roll.</param>
        /// <returns></returns>
        public static EulerAngles ToOrientation(this MagneticFluxDensity value, double pitch, double roll)
        {
            double x = value.X * Math.Cos(pitch) + value.Z * Math.Sin(pitch);
            double y = value.X * Math.Sin(roll) * Math.Sin(pitch) + value.Y * Math.Cos(roll) - value.Z * Math.Sin(roll) * Math.Cos(pitch);
            
            return new EulerAngles(pitch, roll, Math.Atan2(y, x));
        }
    }
}
