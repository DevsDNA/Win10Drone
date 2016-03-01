namespace DevsDNA.Drone.Aircraft
{
    using Controllers;
    using Converters;
    using Interfaces;
    using Reactive;
    using Reactive.Extensions;
    using Sensors;
    using Structs;
    using System;
    using System.Reactive.Linq;

    public class Quadcopter : Aircraft, IDisposable
    {
        private readonly AccelerationObservable acceleration;
        private readonly MagnetismObservable magnetism;
        private readonly RotationObservable rotation;

       
        /// <summary>
        /// Gets the orientation.
        /// </summary>
        /// <value>
        /// The orientation.
        /// </value>
        public override IObservable<Vector3D> Orientation
        {
            get
            {
                ///TODO Combine Magentometer data with acceleration data
                IObservable<Vector3D> orientation =
                    acceleration.Select(accelData => 
                    {
                        var vector = new Vector3D(accelData.X, accelData.Y, accelData.Z);
                        return vector;
                    });

                return orientation;
            }
        }

        protected IObservable<GForce> Acceleration
        {
            get
            {
                return this.acceleration.Filter(estimatedMeasurementNoiseCovariance: 0.05d, estimatedProcessNoiseCovariance: 0d, controlWeight: 0.1d);
            }
        }

        protected IObservable<MagneticFluxDensity> Magnetism
        {
            get
            {
                return this.magnetism;
            }
        }

        /// <summary>
        /// Gets the rotation.
        /// </summary>
        /// <value>
        /// The rotation.
        /// </value>
        public IObservable<Quaternion> Rotation
        {
            get
            {
                return this.rotation
                    .Shift(offsetX: -0.48665334370601881d, offsetY: -0.567703342045178d, offsetZ: -0.24932173162604757d)
                    .TimeInterval()
                    .Select(wt =>
                    {
                        Vector3D r = new Vector3D(
                            x: -wt.Value.X,
                            y: -wt.Value.Y,
                            z: -wt.Value.Z);

                        double dT = (double)wt.Interval.Ticks / TimeSpan.TicksPerSecond;
                        double theta = 1.1d * 2d * DegreeConverter.ToRadians(r.Magnitude) * dT;

                        Vector3D w = r.Normalize();
                        return Quaternion.FromAxisAngle(w, theta);
                    });
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadcopter"/> class.
        /// </summary>
        public Quadcopter()
            : this(MotorControllerFactory.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadcopter"/> class.
        /// </summary>
        /// <param name="frontLeftNumber">The front left number.</param>
        /// <param name="frontRightNumber">The front right number.</param>
        /// <param name="rearRightNumber">The rear right number.</param>
        /// <param name="rearLeftNumber">The rear left number.</param>
        public Quadcopter(int frontLeftNumber, int frontRightNumber, int rearRightNumber, int rearLeftNumber)
            : this(MotorControllerFactory.Default, frontLeftNumber, frontRightNumber, rearRightNumber, rearLeftNumber)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadcopter"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Quadcopter(MotorControllerFactory factory)
            : this(factory, 0, 1, 2, 3)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadcopter"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <param name="frontLeftNumber">The front left number.</param>
        /// <param name="frontRightNumber">The front right number.</param>
        /// <param name="rearRightNumber">The rear right number.</param>
        /// <param name="rearLeftNumber">The rear left number.</param>
        public Quadcopter(MotorControllerFactory factory, int frontLeftNumber, int frontRightNumber, int rearRightNumber, int rearLeftNumber)
            : this(factory.Create(frontLeftNumber), factory.Create(frontRightNumber), factory.Create(rearRightNumber), factory.Create(rearLeftNumber))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Quadcopter"/> class.
        /// </summary>
        /// <param name="frontLeft">The front left.</param>
        /// <param name="frontRight">The front right.</param>
        /// <param name="rearRight">The rear right.</param>
        /// <param name="rearLeft">The rear left.</param>
        public Quadcopter(MotorController frontLeft, MotorController frontRight, MotorController rearRight, MotorController rearLeft)
            : base(new Differential(frontLeft, frontRight, rearRight, rearLeft))
        {
            this.acceleration = AccelerationObservable.FromAccelerometer(Accelerometer.Default);
            this.magnetism = MagnetismObservable.FromMagnetometer(Magnetometer.Default);
            this.rotation = RotationObservable.FromGyroscope(Gyroscope.Default);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="DevsDNA.Drone.Interfaces.IDifferential" />
        public class Differential : IDifferential
        {
            private readonly MotorController frontLeft;
            private readonly MotorController frontRight;
            private readonly MotorController rearRight;
            private readonly MotorController rearLeft;

            /// <summary>
            /// Initializes a new instance of the <see cref="Differential"/> class.
            /// </summary>
            /// <param name="frontLeft">The front left.</param>
            /// <param name="frontRight">The front right.</param>
            /// <param name="rearRight">The rear right.</param>
            /// <param name="rearLeft">The rear left.</param>
            public Differential(MotorController frontLeft, MotorController frontRight, MotorController rearRight, MotorController rearLeft)
            {
                this.frontLeft = frontLeft;
                this.frontRight = frontRight;
                this.rearRight = rearRight;
                this.rearLeft = rearLeft;
            }

            /// <summary>
            /// Sets the control.
            /// </summary>
            /// <param name="thrust">The thrust.</param>
            /// <param name="pitch">The pitch.</param>
            /// <param name="roll">The roll.</param>
            /// <param name="yaw">The yaw.</param>
            /// <exception cref="ArgumentOutOfRangeException">
            /// </exception>
            public void SetControl(double thrust, double pitch, double roll, double yaw)
            {
                if (thrust < 0d || thrust > 1d)
                {
                    return;
                }

                if (pitch < -1d || pitch > 1d)
                {
                    return;
                }

                if (roll < -1d || roll > 1d)
                {
                    return;
                }

                if (yaw < -1d || yaw > 1d)
                {
                    return;
                }

                double effectivePitch = pitch;
                double effectiveRoll = roll;

                double effectiveThrust = thrust *
                    (1d / (1d - Math.Abs(pitch) / 2d)) *
                    (1d / (1d - Math.Abs(roll) / 2d)) *
                    (1d / (1d - Math.Abs(yaw) / 2d));

                double effectiveYaw;

                if (effectiveThrust <= 1d)
                {
                    effectiveYaw = yaw;

                    SetControlCore(effectiveThrust, effectivePitch, effectiveRoll, effectiveYaw);
                    return;
                }

                effectiveThrust = 1d;

                double requestedThrustForYaw = thrust * (1d / (1d - Math.Abs(yaw) / 2d)) - thrust;
                double availableThrustForYaw = 1d - (thrust * (1d / (1d - Math.Abs(pitch) / 2d)) * (1d / (1d - Math.Abs(roll) / 2d)));
                effectiveYaw = availableThrustForYaw > 0d ? yaw * availableThrustForYaw / requestedThrustForYaw : 0d;

                SetControlCore(effectiveThrust, effectivePitch, effectiveRoll, effectiveYaw);
            }

            private void SetControlCore(double thrust, double pitch, double roll, double yaw)
            {
                double frontLeftSpeed = thrust *
                    (pitch > 0d ? 1d : 1d - -pitch) *
                    (roll > 0d ? 1d : 1d - -roll) *
                    (yaw > 0d ? 1d : 1d - -yaw);

                double frontRightSpeed = thrust *
                    (pitch > 0d ? 1d : 1d - -pitch) *
                    (roll < 0d ? 1d : 1d - roll) *
                    (yaw < 0d ? 1d : 1d - yaw);

                double rearRightSpeed = thrust *
                    (pitch < 0d ? 1d : 1d - pitch) *
                    (roll < 0d ? 1d : 1d - roll) *
                    (yaw > 0d ? 1d : 1d - -yaw);

                double rearLeftSpeed = thrust *
                    (pitch < 0d ? 1d : 1d - pitch) *
                    (roll > 0d ? 1d : 1d - -roll) *
                    (yaw < 0d ? 1d : 1d - yaw);

                this.frontLeft.SetSpeed(frontLeftSpeed);
                this.frontRight.SetSpeed(frontRightSpeed);
                this.rearRight.SetSpeed(rearRightSpeed);
                this.rearLeft.SetSpeed(rearLeftSpeed);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.acceleration.Dispose();
                this.magnetism.Dispose();
                this.rotation.Dispose();
            }
        }
    }
}
