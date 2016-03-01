namespace DevsDNA.Drone.Aircraft
{
    using Interfaces;
    using Structs;
    using System;

    /// <summary>
    /// The aircraft class
    /// </summary>
    public abstract class Aircraft
    {
        private readonly IDifferential differential;
        
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public abstract IObservable<Vector3D> Orientation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Aircraft"/> class.
        /// </summary>
        /// <param name="differential">The differential.</param>
        protected Aircraft(IDifferential differential)
        {
            this.differential = differential;
        }

        /// <summary>
        /// Sets the control.
        /// </summary>
        /// <param name="thrust">The thrust.</param>
        /// <param name="pitch">The pitch.</param>
        /// <param name="roll">The roll.</param>
        /// <param name="yaw">The yaw.</param>
        public virtual void SetControl(double thrust, double pitch, double roll, double yaw)
        {
            this.differential.SetControl(thrust, pitch, roll, yaw);
        }
    }
}
