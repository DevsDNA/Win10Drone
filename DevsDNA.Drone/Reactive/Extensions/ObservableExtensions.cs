namespace DevsDNA.Drone.Reactive.Extensions
{
    using DevsDNA.Drone.Extensions;
    using Filters;
    using Structs;
    using System;
    using System.Reactive;
    using System.Reactive.Linq;

    /// <summary>
    /// ObservableExtensions
    /// </summary>
    public static class ObservableExtensions
    {
        /// <summary>
        /// Samples the fast.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        public static IObservable<TimeInterval<TSource>> SampleFast<TSource>(this IObservable<TSource> source, TimeSpan interval)
        {
            return source
                .TimeInterval()
                .Scan(
                    Tuple.Create(false, TimeSpan.Zero, default(TimeInterval<TSource>)), 
                    (accu, v) =>
                    {
                        TimeSpan t = accu.Item2 + v.Interval;
                
                        if (t >= interval)
                        {
                            return Tuple.Create(true, TimeSpan.Zero, new TimeInterval<TSource>(v.Value, t));
                        }

                        return Tuple.Create(false, t, default(TimeInterval<TSource>));
                    })
                .Where(t => t.Item1)
                .Select(t => t.Item3);
        }

        /// <summary>
        /// Combines the latest.
        /// </summary>
        /// <typeparam name="TSource1">The type of the source1.</typeparam>
        /// <typeparam name="TSource2">The type of the source2.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source1">The source1.</param>
        /// <param name="source2">The source2.</param>
        /// <param name="resultSelector">The result selector.</param>
        /// <returns></returns>
        public static IObservable<TResult> CombineLatest<TSource1, TSource2, TResult>(this IObservable<TSource1> source1, IObservable<TSource2> source2, Func<TSource1, TSource2, bool, bool, TResult> resultSelector)
        {
            IObservable<Tuple<TSource1, bool>> alt1 = Observable.Scan(source1, Tuple.Create(default(TSource1), false), (accu, v) => Tuple.Create(v, !accu.Item2));
            IObservable<Tuple<TSource2, bool>> alt2 = Observable.Scan(source2, Tuple.Create(default(TSource2), false), (accu, v) => Tuple.Create(v, !accu.Item2));

            bool odd1 = false;
            bool odd2 = false;

            return Observable.CombineLatest(alt1, alt2, (v1, v2) =>
            {
                TResult result = resultSelector(v1.Item1, v2.Item1, v1.Item2 != odd1, v2.Item2 != odd2);

                odd1 = v1.Item2;
                odd2 = v2.Item2;
                return result;
            });
        }

        /// <summary>
        /// Shifts the specified minimum x.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="minX">The minimum x.</param>
        /// <param name="maxX">The maximum x.</param>
        /// <param name="minY">The minimum y.</param>
        /// <param name="maxY">The maximum y.</param>
        /// <param name="minZ">The minimum z.</param>
        /// <param name="maxZ">The maximum z.</param>
        /// <returns></returns>
        public static IObservable<MagneticFluxDensity> Shift(this IObservable<MagneticFluxDensity> source, double minX, double maxX, double minY, double maxY, double minZ, double maxZ)
        {
            return source.Select(m => MagneticFluxDensityExtensions.Shift(m, minX, maxX, minY, maxY, minZ, maxZ));
        }

        /// <summary>
        /// Shifts the specified offset x.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="offsetX">The offset x.</param>
        /// <param name="offsetY">The offset y.</param>
        /// <param name="offsetZ">The offset z.</param>
        /// <returns></returns>
        public static IObservable<AngularVelocity> Shift(this IObservable<AngularVelocity> source, double offsetX, double offsetY, double offsetZ)
        {
            return source.Select(r => r.Shift(offsetX, offsetY, offsetZ));
        }

        /// <summary>
        /// Filters the specified estimated measurement noise covariance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="estimatedMeasurementNoiseCovariance">The estimated measurement noise covariance.</param>
        /// <param name="estimatedProcessNoiseCovariance">The estimated process noise covariance.</param>
        /// <param name="controlWeight">The control weight.</param>
        /// <returns></returns>
        public static IObservable<MagneticFluxDensity> Filter(this IObservable<MagneticFluxDensity> source, double estimatedMeasurementNoiseCovariance, double estimatedProcessNoiseCovariance, double controlWeight)
        {
            var fx = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fy = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fz = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);

            return source.Select(m => new MagneticFluxDensity(x: fx.Filter(m.X), y: fy.Filter(m.Y), z: fz.Filter(m.Z)));
        }

        /// <summary>
        /// Filters the specified estimated measurement noise covariance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="estimatedMeasurementNoiseCovariance">The estimated measurement noise covariance.</param>
        /// <param name="estimatedProcessNoiseCovariance">The estimated process noise covariance.</param>
        /// <param name="controlWeight">The control weight.</param>
        /// <returns></returns>
        public static IObservable<GForce> Filter(this IObservable<GForce> source, double estimatedMeasurementNoiseCovariance, double estimatedProcessNoiseCovariance, double controlWeight)
        {
            var fx = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fy = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fz = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);

            return source.Select(a => new GForce(x: fx.Filter(a.X), y: fy.Filter(a.Y), z: fz.Filter(a.Z)));
        }

        /// <summary>
        /// Filters the specified estimated measurement noise covariance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="estimatedMeasurementNoiseCovariance">The estimated measurement noise covariance.</param>
        /// <param name="estimatedProcessNoiseCovariance">The estimated process noise covariance.</param>
        /// <param name="controlWeight">The control weight.</param>
        /// <returns></returns>
        public static IObservable<AngularVelocity> Filter(this IObservable<AngularVelocity> source, double estimatedMeasurementNoiseCovariance, double estimatedProcessNoiseCovariance, double controlWeight)
        {
            var fx = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fy = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            var fz = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);

            return source.Select(a => new AngularVelocity(x: fx.Filter(a.X), y: fy.Filter(a.Y), z: fz.Filter(a.Z)));
        }

        /// <summary>
        /// Filters the specified estimated measurement noise covariance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="estimatedMeasurementNoiseCovariance">The estimated measurement noise covariance.</param>
        /// <param name="estimatedProcessNoiseCovariance">The estimated process noise covariance.</param>
        /// <param name="controlWeight">The control weight.</param>
        /// <returns></returns>
        public static IObservable<double> Filter(this IObservable<double> source, double estimatedMeasurementNoiseCovariance, double estimatedProcessNoiseCovariance, double controlWeight)
        {
            var f = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight);
            return source.Select(v => f.Filter(v));
        }

        /// <summary>
        /// Filters the specified estimated measurement noise covariance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="estimatedMeasurementNoiseCovariance">The estimated measurement noise covariance.</param>
        /// <param name="estimatedProcessNoiseCovariance">The estimated process noise covariance.</param>
        /// <param name="controlWeight">The control weight.</param>
        /// <param name="initialEstimate">The initial estimate.</param>
        /// <param name="initialErrorCovariance">The initial error covariance.</param>
        /// <returns></returns>
        public static IObservable<double> Filter(this IObservable<double> source, double estimatedMeasurementNoiseCovariance, double estimatedProcessNoiseCovariance, double controlWeight, double initialEstimate, double initialErrorCovariance)
        {
            var f = new KalmanFilter(estimatedMeasurementNoiseCovariance, estimatedProcessNoiseCovariance, controlWeight, initialEstimate, initialErrorCovariance);
            return source.Select(v => f.Filter(v));
        }
    }
}
