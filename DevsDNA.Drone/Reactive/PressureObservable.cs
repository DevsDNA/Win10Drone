namespace DevsDNA.Drone.Reactive
{
    using Sensors;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public sealed class PressureObservable : IObservable<double>, IDisposable
    {
        private readonly Barometer _barometer;
        private readonly HashSet<IObserver<double>> _observers = new HashSet<IObserver<double>>();
        private readonly object _syncRoot = new object();
        private bool _disposed;

        private PressureObservable(Barometer barometer)
        {
            _barometer = barometer;
            new Task(() => Run()).Start();
        }

        /// <summary>
        /// Creates an observable from a sensor.
        /// </summary>
        /// <param name="barometer">The barometer.</param>
        /// <returns>An observable that provides pressure in Pascal (Pa).</returns>
        public static PressureObservable FromBarometer(Barometer barometer)
        {
            return new PressureObservable(barometer);
        }

        IDisposable IObservable<double>.Subscribe(IObserver<double> observer)
        {
            return Subscribe(observer);
        }

        /// <summary>
        /// Subscribes to the observable sequence.
        /// </summary>
        /// <param name="observer">The observer that will receive pressure in Pascal (Pa).</param>
        /// <returns>The disposable object that can be used to unsubscribe the observer.</returns>
        public Subscription Subscribe(IObserver<double> observer)
        {
            lock (_syncRoot)
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                }
            }

            return new Subscription(observer, this);
        }

        private void Run()
        {
            while (!_disposed)
            {
                double value = _barometer.Read();

                lock (_syncRoot)
                {
                    foreach (var observer in _observers)
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
            _disposed = true;
        }

        internal void Unsubscribe(Subscription subscription)
        {
            lock (_syncRoot)
            {
                if (_observers.Contains(subscription.Observer))
                {
                    _observers.Remove(subscription.Observer);
                }
            }
        }

        /// <summary>
        /// Represents the disposable object that can be used to unsubscribe from the observable.
        /// </summary>
        public class Subscription : IDisposable
        {
            private readonly IObserver<double> _observer;
            private readonly PressureObservable _context;

            /// <summary>
            /// The observer that receives pressure in Pascal (Pa).
            /// </summary>
            public IObserver<double> Observer
            {
                get { return _observer; }
            }

            internal Subscription(IObserver<double> observer, PressureObservable context)
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
