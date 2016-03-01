namespace DevsDNA.Drone.Converters
{
    using System;

    /// <summary>
    /// DegreeConverter
    /// </summary>
    public class DegreeConverter
    {
        /// <summary>
        /// To degrees to radians.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double ToRadians(double value)
        {
            return value * Math.PI / 180d;
        }

        /// <summary>
        /// Convert radians to degrees.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double ToDegrees(double value)
        {
            return value * 180d / Math.PI;
        }
    }
}
