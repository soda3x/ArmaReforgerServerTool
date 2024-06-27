using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReforgerServerApp.Helpers
{
    public class BrowserHelper
    {
        private BrowserHelper() { }

        public static void Open(string url)
        {
            ProcessStartInfo psInfo = new()
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psInfo);
        }
    }
}
