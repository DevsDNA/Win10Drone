namespace DevsDNA.Drone.Reactive
{
    using Sensors;
    using Structs;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides 3-axis acceleration force.
    /// </summary>
    public sealed class AccelerationObservable : IObservable<GForce>, IDisposable
    {
        private readonly Accelerometer accelerometer;
        private readonly HashSet<IObserver<GForce>> observers = new HashSet<IObserver<GForce>>();
        private readonly object syncRoot = new object();
        private bool disposed;

        private AccelerationObservable(Accelerometer accelerometer)
        {
            this.accelerometer = accelerometer;
            new Task(() => Run()).Start();
        }

        /// <summary>
        /// Creates an observable from a sensor.
        /// </summary>
        /// <param name="accelerometer">The accelerometer.</param>
        /// <returns>An observable that provides 3-axis acceleration force.</returns>
        public static AccelerationObservable FromAccelerometer(Accelerometer accelerometer)
        {
            return new AccelerationObservable(accelerometer);
        }

        IDisposable IObservable<GForce>.Subscribe(IObserver<GForce> observer)
        {
            return Subscribe(observer);
        }

        /// <summary>
        /// Subscribes to the observable sequence.
        /// </summary>
        /// <param name="observer">The observer that will receive 3-axis acceleration force.</param>
        /// <returns>The disposable object that can be used to unsubscribe the observer.</returns>
        public Subscription Subscribe(IObserver<GForce> observer)
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
                GForce value = this.accelerometer.Read();

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
            if (this.observers.Contains(subscription.Observer))
            {
                lock (this.syncRoot)
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
            private readonly IObserver<GForce> _observer;
            private readonly AccelerationObservable _context;

            /// <summary>
            /// The observer that receives 3-axis acceleration force.
            /// </summary>
            public IObserver<GForce> Observer
            {
                get { return _observer; }
            }

            internal Subscription(IObserver<GForce> observer, AccelerationObservable context)
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
