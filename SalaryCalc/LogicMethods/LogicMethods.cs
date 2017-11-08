using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryCalc
{
    class LogicMethods
    {
        public static DateTime DateFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static double DateToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Floor(diff.TotalSeconds);
        }

        public static int ExperienceInYears(long timestamp, DateTime CheckDate)
        {
            DateTime HireDate = LogicMethods.DateFromUnixTimestamp(timestamp);

            TimeSpan diff = CheckDate - HireDate;
            return (int)Math.Floor(diff.TotalDays) / 365;
        }

    }

}
