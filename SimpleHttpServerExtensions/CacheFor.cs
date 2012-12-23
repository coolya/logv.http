using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleHttpServer
{
    public enum CacheFor
    {
        ADay = 86400,
        AnHour = 3600,
        TwoHours = 7200,
        AMinute = 60,
        HalfAnHour = 1800,
        AWeek = 604800,
        TwelveHours = 43200
    }
}
