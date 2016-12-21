using System;

namespace GitIntegration.Extensions
{
    public class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.Now;
        public static Func<DateTime> Today = () => DateTime.Now.Date;
    }
}