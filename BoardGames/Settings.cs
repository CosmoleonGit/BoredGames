using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BoardGames
{
    public static class Settings
    {
        const int majorVer = 1,
                  minorVer = 1,
                  bugVer = 0;

        public static string GetVersion => $"{majorVer}.{minorVer}.{bugVer}";

        public static string username = "";
    }
}
