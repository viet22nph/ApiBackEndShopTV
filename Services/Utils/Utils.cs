using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.utils
{
    public static class Utils
    {
        public static DateTime GetDateTimeToVietNam()
        {
            DateTime utcTime = DateTime.UtcNow;

            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

            
            DateTime vietnamTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, vietnamTimeZone);
            return vietnamTime;
        }

    }
}
