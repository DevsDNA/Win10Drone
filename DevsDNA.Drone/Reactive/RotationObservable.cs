namespace DevsDNA.Drone.Reactive
{
    using Sensors;
    using Structs;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides 3-axis rotation speed.
    /// </summary>
    public sealed class RotationObservable : IObservable<AngularVelocity>, IDisposable
    {
        private readonly Gyroscope gyroscope;
        private readonly HashSet<IObserver<AngularVelocity>> observers = new HashSet<IObserver<AngularVelocity>>();
        private readonly object syncRoot = new object();
        private bool disposed;

        private RotationObservable(Gyroscope gyroscope)
        {
            this.gyroscope = gyroscope;
            new Task(() => Run()).Start();
        }

        /// <summary>
        /// Creates an observable from a sensor.
        /// </summary>
        /// <param name="gyroscope">The gyroscope.</param>
        /// <returns>An observable that provides 3-axis rotation speed.</returns>
        public static RotationObservable FromGyroscope(Gyroscope gyroscope)
        {
            return new RotationObservable(gyroscope);
        }

        IDisposable IObservable<AngularVelocity>.Subscribe(IObserver<AngularVelocity> observer)
        {
            return Subscribe(observer);
        }

        /// <summary>
        /// Subscribes to the observable sequence.
        /// </summary>
        /// <param name="observer">The observer that will receive 3-axis rotation speed.</param>
        /// <returns>The disposable object that can be used to unsubscribe the observer.</returns>
        public Subscription Subscribe(IObserver<AngularVelocity> observer)
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
                AngularVelocity value = this.gyroscope.Read();

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
            private readonly IObserver<AngularVelocity> _observer;
            private readonly RotationObservable _context;

            /// <summary>
            /// The observer that receives 3-axis rotation speed.
            /// </summary>
            public IObserver<AngularVelocity> Observer
            {
                get { return _observer; }
            }

            internal Subscription(IObserver<AngularVelocity> observer, RotationObservable context)
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
