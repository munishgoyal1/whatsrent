using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using HttpLibrary;
using System.Linq;

namespace WhatsRent.Common.Utilities
{
    public class Utilities
    {
        public static string GetWebPage(string url, ref int webCallCntr, CookieContainer cookieContainer)
        {
            var res = Interlocked.Increment(ref webCallCntr);
            Console.WriteLine(webCallCntr + " Webcall Counter at: " + DateTime.Now.ToLongTimeString());

            int retryCnt = 0;
            string response = "";
            do
            {
                response = HttpHelper.GetWebPageResponse(url, null, null, cookieContainer);
                retryCnt++;
            } while (string.IsNullOrEmpty(response) && retryCnt < 3);

            return response;
        }

        public static Dictionary<string, string> GetConfig(string configfilepath)
        {
            Dictionary<string, string> config = new Dictionary<string, string>();
            Array.ForEach<string>(File.ReadAllLines(configfilepath), l =>
            {
                var parts = l.Split(new string[] { ":=" }, StringSplitOptions.None);
                if (parts.Length == 2)
                    if (!config.ContainsKey(parts[0]))
                        config.Add(parts[0], parts[1]);
            });
            return config;
        }

        public static int? GetNullableInt(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            int result;
            if (!int.TryParse(input, out result))
                return null;

            return result;
        }

        public static double? GetNullableDouble(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            double result;
            if (!double.TryParse(input, out result))
                return null;

            return result;
        }

        public static DateTime? GetNullableDateTime(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            DateTime result;
            if (!DateTime.TryParse(input, out result))
                return null;

            return result;
        }

        public static bool? GetNullableBool(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            bool result;
            if (!bool.TryParse(input, out result))
                return null;

            return result;
        }
    }
}
