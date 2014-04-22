using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bauhaus.Models;

namespace Bauhaus.Helpers
{
    /// <summary>
    /// Datetime Extensions to handle business days
    /// </summary>
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Add Business Days to Datetime
        /// </summary>
        /// <param name="date">Base Date</param>
        /// <param name="days">Days to be added</param>
        /// <returns></returns>
        public static DateTime AddBusinessDays(this DateTime date, int days)
        {
            int direction = days < 0 ? -1 : 1;

            while (days != 0)
            {
                date = date.AddDays(direction);
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    days -= direction;
                }
            }

            return date;
        }
    }
}

