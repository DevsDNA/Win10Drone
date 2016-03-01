namespace DevsDNA.Drone.Extensions
{
    using DevsDNA.Drone.Converters;
    using DevsDNA.Drone.Structs;

    /// <summary>
    /// AngularVelocityExtensions
    /// </summary>
    public static class AngularVelocityExtensions
    {
        /// <summary>
        /// Shifts the specified offset x.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="offsetX">The offset x.</param>
        /// <param name="offsetY">The offset y.</param>
        /// <param name="offsetZ">The offset z.</param>
        /// <returns></returns>
        public static AngularVelocity Shift(this AngularVelocity rotation, double offsetX, double offsetY, double offsetZ)
        {
            return new AngularVelocity(
                x: rotation.X + offsetX,
                y: rotation.Y + offsetY,
                z: rotation.Z + offsetZ);
        }

        /// <summary>
        /// To the rotation.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <param name="duration">The duration.</param>
        /// <returns></returns>
        public static EulerAngles ToRotation(this AngularVelocity rotation, double duration)
        {
            return new EulerAngles(
                pitch: 2d * duration * DegreeConverter.ToRadians(rotation.X),
                roll: 2d * duration * DegreeConverter.ToRadians(rotation.Y),
                yaw: 2d * duration * DegreeConverter.ToRadians(-rotation.Z));
        }
    }
}
