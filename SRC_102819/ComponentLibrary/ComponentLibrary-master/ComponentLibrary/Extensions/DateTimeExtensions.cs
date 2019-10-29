using System;

namespace TE.ComponentLibrary.ComponentLibrary.Extensions
{
    /// <summary>
    /// An extension class for date time
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Compares the date time till minute precision.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="compareValue">The compare value.</param>
        /// <returns></returns>
        public static int CompareTillMinutePrecision(this DateTime source, DateTime compareValue)
        {
            AreDatesComparable(source, compareValue);
            var sourceInUniversalTime = TimeInUtc(source).ToUniversalTime();
            var compareValueInUniversalTime = TimeInUtc(compareValue).ToUniversalTime();

            var minutePrecisionSource = new DateTime(sourceInUniversalTime.Year, sourceInUniversalTime.Month,
                sourceInUniversalTime.Day, sourceInUniversalTime.Hour, sourceInUniversalTime.Minute, 0);
            var minutePrecisionCompareValue = new DateTime(compareValueInUniversalTime.Year,
                compareValueInUniversalTime.Month, compareValueInUniversalTime.Day, compareValueInUniversalTime.Hour,
                compareValueInUniversalTime.Minute, 0);
            return DateTime.Compare(minutePrecisionSource, minutePrecisionCompareValue);
        }

        private static void AreDatesComparable(DateTime source, DateTime compareValue)
        {
            if (source.Kind == DateTimeKind.Unspecified || compareValue.Kind == DateTimeKind.Unspecified)
                throw new InvalidOperationException("Cannot compare without timezone Info.");
        }

        /// <summary>
        /// Compares dates with day precision.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="compareValue"></param>
        /// <returns></returns>
        public static int CompareTillIstDayPrecision(this DateTime source, DateTime compareValue)
        {
            AreDatesComparable(source, compareValue);
            var sourceInIst = source.InIst();
            var compareValueInIst = compareValue.InIst();

            var minutePrecisionSource = new DateTime(sourceInIst.Year, sourceInIst.Month,
                sourceInIst.Day, 0, 0, 0);
            var minutePrecisionCompareValue = new DateTime(compareValueInIst.Year,
                compareValueInIst.Month, compareValueInIst.Day, 0, 0, 0);
            return DateTime.Compare(minutePrecisionSource, minutePrecisionCompareValue);
        }

        /// <summary>
        /// Converts date to IST.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Value should have timezone information.</exception>
        public static DateTime InIst(this DateTime source)
        {
            if (source.Kind == DateTimeKind.Unspecified)
            {
                throw new InvalidOperationException("Value should have timezone information.");
            }

            var dateInUTC = TimeZoneInfo.ConvertTimeToUtc(source);
            var INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateInUTC, INDIAN_ZONE);
        }

        /// <summary>
        /// Shoulds the be a future day in ist.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Applied on date should be in ISO format.
        /// or
        /// Applied on date cannot be set to past.
        /// or
        /// Applied on date cannot be set on same day in IST.
        /// </exception>
        public static bool ShouldBeAFutureDayInIst(this DateTime source)
        {
            if (source.Kind == DateTimeKind.Unspecified)
                throw new ArgumentException("Applied on date should be in ISO format.");

            var appliedOnInUtc = TimeZoneInfo.ConvertTimeToUtc(source);
            var todayIndianTime = DateTime.UtcNow.InIst();
            var appliedOnIndianTime = source.InIst();

            if (appliedOnInUtc.CompareTillMinutePrecision(DateTime.UtcNow) == -1)
                throw new ArgumentException("Applied on date cannot be set to past.");

            if (appliedOnIndianTime.Date == todayIndianTime.Date)
                throw new ArgumentException("Applied on date cannot be set on same day in IST.");
            return true;
        }

        /// <summary>
        /// Additionals the time since midnight ist.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static TimeSpan AdditionalTimeSinceMidnightIst(this DateTime source)
        {
            var appliedOnInIst = source.InIst();
            return appliedOnInIst.Date.Subtract(source.InIst());
        }

        private static DateTime TimeInUtc(DateTime dateTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(dateTime);
        }


    }
}