namespace DevsDNA.Drone.Reactive
{
    using Sensors;
    using Structs;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides 3-axis magnetic flux density.
    /// </summary>
    public sealed class MagnetismObservable : IObservable<MagneticFluxDensity>, IDisposable
    {
        private readonly Magnetometer magnetometer;
        private readonly HashSet<IObserver<MagneticFluxDensity>> observers = new HashSet<IObserver<MagneticFluxDensity>>();
        private readonly object syncRoot = new object();
        private bool disposed;

        private MagnetismObservable(Magnetometer magnetometer)
        {
            this.magnetometer = magnetometer;
            new Task(() => Run()).Start();
        }

        /// <summary>
        /// Creates an observable from a sensor.
        /// </summary>
        /// <param name="magnetometer">The magnetometer.</param>
        /// <returns>An observable that provides 3-axis magnetic flux density.</returns>
        public static MagnetismObservable FromMagnetometer(Magnetometer magnetometer)
        {
            return new MagnetismObservable(magnetometer);
        }

        IDisposable IObservable<MagneticFluxDensity>.Subscribe(IObserver<MagneticFluxDensity> observer)
        {
            return Subscribe(observer);
        }

        /// <summary>
        /// Subscribes to the observable sequence.
        /// </summary>
        /// <param name="observer">The observer that will receive 3-axis magnetic flux density.</param>
        /// <returns>The disposable object that can be used to unsubscribe the observer.</returns>
        public Subscription Subscribe(IObserver<MagneticFluxDensity> observer)
        {
            lock (this.syncRoot)
            {
                if (!this.observers.Contains(observer))
                {
                    this.observers.Add(observer);
                }
            }

            return new Subscription(observer, this);
        }

        private void Run()
        {
            while (!this.disposed)
            {
                MagneticFluxDensity value = this.magnetometer.Read();

                lock (this.syncRoot)
                {
                    foreach (var observer in this.observers)
                    {
                        observer.OnNext(value);
                    }
                }
            }
        }

        /// <summary>
        /// Disposes the observable.
        /// </summary>
        public void Dispose()
        {
            this.disposed = true;
        }

        internal void Unsubscribe(Subscription subscription)
        {
            lock (this.syncRoot)
            {
                if (this.observers.Contains(subscription.Observer))
                {
                    this.observers.Remove(subscription.Observer);
                }
            }
        }

        /// <summary>
        /// Represents the disposable object that can be used to unsubscribe from the observable.
        /// </summary>
        public class Subscription : IDisposable
        {
            private readonly IObserver<MagneticFluxDensity> _observer;
            private readonly MagnetismObservable _context;

            /// <summary>
            /// The observer that receives 3-axis magnetic flux density.
            /// </summary>
            public IObserver<MagneticFluxDensity> Observer
            {
                get { return _observer; }
            }

            internal Subscription(IObserver<MagneticFluxDensity> observer, MagnetismObservable context)
            {
                _observer = observer;
                _context = context;
            }

            /// <summary>
            /// Unsubscribes from the observable.
            /// </summary>
            public void Dispose()
            {
                _context.Unsubscribe(this);
            }
        }
    }
}
