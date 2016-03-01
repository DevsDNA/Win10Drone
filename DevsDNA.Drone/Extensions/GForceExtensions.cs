namespace DevsDNA.Drone.Extensions
{
    using DevsDNA.Drone.Structs;

    /// <summary>
    /// GForceExtensions
    /// </summary>
    public static class GForceExtensions
    {
        /// <summary>
        /// To the orientation.
        /// </summary>
        /// <param name="gravity">The gravity.</param>
        /// <returns></returns>
        public static EulerAngles ToOrientation(this GForce gravity)
        {
            return ToOrientation(gravity, 0.001d);
        }

        /// <summary>
        /// To the orientation.
        /// </summary>
        /// <param name="gravity">The gravity.</param>
        /// <param name="miu">The miu.</param>
        /// <returns></returns>
        public static EulerAngles ToOrientation(this GForce gravity, double miu)
        {
            return EulerAngles.FromVector(gravity.Normalize(), miu);
        }
    }
}
