using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class strings : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if (theString.Contains("printf"))
            {
                string sType = "ansi";
                string safe = string.Empty;
                string length = string.Empty;
                string dest = "Stdout";

                if (theString.Contains("wprintf"))
                {
                    sType = "wide";
                }

                if (theString.Contains("f_s"))
                {
                    safe = "safe";
                }

                if ((theString.Contains("nprintf")) || (theString.Contains("nwprintf")))
                {
                    length = "length check";
                }

                if ((theString.StartsWith("_s")) || (theString.StartsWith("s")))
                {
                    dest = "String";
                }
                else if ((theString.StartsWith("_v")) || (theString.StartsWith("v")))
                {
                    dest = "Vararg";
                }
                else if ((theString.StartsWith("_f")) || (theString.StartsWith("f")))
                {
                    dest = "File output";

                }

                dest += " Formatting";

                results.AppendResult(dest, sType, 1, 0);

                if (safe != string.Empty)
                    results.AppendResult(dest, safe, 1, 0);

                if (length != string.Empty)
                    results.AppendResult(dest, length, 1, 0);

            }

            return true;
        }
    }
}
