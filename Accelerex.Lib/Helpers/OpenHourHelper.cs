using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accelerex.Lib.Helpers
{
    public static class OpenHourHelper
    {
        public static string ProcessAllEntries<T, U>(List<T> tuesdayHours, List<U> wednesdayHours, T lastOpening) where T : class, new()
        {
            var result = new List<string>();

            for (int i = 0; i <= (tuesdayHours.Count / 2);)
            {
                var openTime = tuesdayHours[i];
                var closeTime = tuesdayHours[i + 1];

                result.Add(ProcessTwoEntries(openTime, closeTime));
                i += 2;
            }

            if (lastOpening != null)
            {
                T closeTimeFromWed = wednesdayHours.Select(x => new
                {
                    Type = ((dynamic)x).Type,
                    Value = ((dynamic)x).Value
                }).ToList().First(x => x.Type == "close") as T;

                if (((dynamic)wednesdayHours.First()).Type == "open")
                {
                    closeTimeFromWed = default;
                }

                result.Add(ProcessTwoEntries(lastOpening, closeTimeFromWed));
            }

            return string.Join(", ", result);
        }

        public static string ProcessTwoEntries<T>(T openTime, T closeTime)
        {
            var result = new List<string>();

            if (openTime == null)
                result.Add("Error - Open time not set");
            else
            {
                var open = ConvertEpochTime(((dynamic)openTime).Value.ToString());
                result.Add(open);
            }

            if (closeTime == null)
                result.Add("Error - Close time not set");
            else
            {
                var close = ConvertEpochTime(((dynamic)closeTime).Value.ToString());
                result.Add(close);
            }

            return string.Join(" - ", result);
        }

        public static string ProcessOneEntry<T, U>(List<T> tuesdayHours, List<U> wednesdayHours)
        {
            var result = new List<string>();

            if (((dynamic)tuesdayHours.First()).Type == "close")
            {
                return "Closed";
            }

            var openTime = ConvertEpochTime(((dynamic)tuesdayHours.First()).Value.ToString());
            result.Add(openTime);

            if (wednesdayHours.Count == 0 || ((dynamic)wednesdayHours.First()).Type == "open")
            {
                result.Add("Error - Close time not set");
            }
            else
            {
                var closeTime = ConvertEpochTime(((dynamic)wednesdayHours.First()).Value.ToString());
                result.Add(closeTime);
            }

            return string.Join(" - ", result);
        }

        public static string ConvertEpochTime(string time)
        {
            var isTime = double.TryParse(time, out double result);
            result = (!isTime) ? 0 : result;

            string hourSuffix = " AM";
            var conversion = Math.Round((24 * result) / 86400);

            if (conversion > 12)
            {
                conversion -= 12;
                hourSuffix = " PM";
            }

            return $"{conversion}{hourSuffix}";
        }
    }
}
