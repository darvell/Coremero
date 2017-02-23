using System;
using System.Collections.Generic;
using System.Text;

namespace Coremero.Client.Discord
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Converts a DateTimeOffset to the magic that is a 
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static ulong ToSnowflake(this DateTimeOffset dateTime)
        {
            ulong unixStamp = (ulong) dateTime.ToUnixTimeMilliseconds();
            return (unixStamp - 1420070400000L) << 22;
        }
    }
}
