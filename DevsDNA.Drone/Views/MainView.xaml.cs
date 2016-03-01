namespace DevsDNA.Drone.Views
{
    using Aircraft;
    using Controllers;
    using Converters;
    using Structs;
    using System;
    using System.Diagnostics;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainView : Page
    {
        private Quadcopter devsDNADrone;
        private double thrust = 0.0;
        private double pitchForceValue;
        private double rollForceValue;
        private double yawForceValue;
        private double rollCenter;
        private double pitchCenter;
        private double yawCenter;
        private DispatcherTimer ShowDataTimer;
        private Vector3D vector;

        private PIDController pidRollController;
        private PIDController pidPitchController;
        private PIDController pidYawController;
        
        private double pid_P = 1;
        private double pid_I = 1;
        private double pid_D = 1;
        private double maxErrorCumulative = 0.02;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainView"/> class.
        /// </summary>
        public MainView()
        {
            this.InitializeComponent();
            InitializePIDControllers();
            InitializeDevsDNADrone();
        }

        private void InitializeDevsDNADrone()
        {
            bool firstMeasurement = true;
            Stopwatch timer = new Stopwatch();

            this.devsDNADrone = new Quadcopter();
            this.devsDNADrone.SetControl(thrust, pitchForceValue, rollForceValue, yawForceValue);


            this.devsDNADrone.Orientation.Subscribe(orientation =>
            {
                vector = orientation;
                if (firstMeasurement)
                {
                    this.rollCenter = vector.X;
                    this.pitchCenter = vector.Y;
                    this.yawCenter = vector.Z;
                    firstMeasurement = false;
                    timer.Start();
                }
                else
                {
                    timer.Stop();

                    this.rollForceValue = this.pidRollController.Manipulate(vector.X, this.rollCenter, timer.ElapsedMilliseconds);
                    this.pitchForceValue = this.pidPitchController.Manipulate(vector.Y, this.pitchCenter, timer.ElapsedMilliseconds);
                    this.yawForceValue = this.pidPitchController.Manipulate(vector.Z, this.yawCenter, timer.ElapsedMilliseconds);

                    this.devsDNADrone.SetControl(this.thrust, this.pitchForceValue, this.rollForceValue, this.yawForceValue);
                    timer.Start();
                }
            });
        }

        private void RefreshUI()
        {
            var task = App.RootFrame.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => 
            {
                txtRollValue.Text = Math.Round(this.rollForceValue, 3).ToString();
                txtPitchValue.Text = Math.Round(this.pitchForceValue, 3).ToString();

                CenterTransform.X = DegreeConverter.ToDegrees(this.rollCenter);
                CenterTransform.Y = DegreeConverter.ToDegrees(this.pitchCenter);

                ForceTransform.X = DegreeConverter.ToDegrees(this.rollForceValue);
                ForceTransform.Y = DegreeConverter.ToDegrees(this.pitchForceValue);

                GyroTransform.X = DegreeConverter.ToDegrees(vector.X);
                GyroTransform.Y = DegreeConverter.ToDegrees(vector.Y);


                planeProjection.RotationY = DegreeConverter.ToDegrees((-1 * this.vector.X) - this.rollCenter);
                planeProjection.RotationX = DegreeConverter.ToDegrees(this.vector.Y - this.pitchCenter);

                txtGyroX.Text = Math.Round((vector.X), 3).ToString();
                txtGyroY.Text = Math.Round((vector.Y), 3).ToString();
            });
        }

        private void InitializePIDControllers()
        {
            this.pidRollController = new PIDController(this.pid_P, this.pid_I, this.pid_D, this.maxErrorCumulative);
            this.pidPitchController = new PIDController(this.pid_P, this.pid_I, this.pid_D, this.maxErrorCumulative);
            this.pidYawController = new PIDController(this.pid_P, this.pid_I, this.pid_D, this.maxErrorCumulative);
        }

        private void sldThrust_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            this.thrust = e.NewValue;
        }

        private void Button_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.rollCenter = this.vector.X/10;
            this.pitchCenter = this.vector.Y/10;
        }

        private void Button_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.thrust = 0.0;
        }
    }
}
