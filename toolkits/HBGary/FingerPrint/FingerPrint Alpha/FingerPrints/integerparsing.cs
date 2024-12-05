using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class integerparsing : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {


            Regex r_xvid = new Regex("(_+|)(a|w)(toi)(_l|)", RegexOptions.IgnoreCase);
            Match aMatch = r_xvid.Match(theString);
            if (aMatch.Success)
            {
                if (theString.Contains("64"))
                {
                    if (theString.Contains("wtoi"))
                    {
                        results.SetResult("String(wide) to Integer (64-bit)", "yes", 1, 0);
                    }
                    else if (theString.Contains("atoi"))
                    {
                        results.SetResult("String to Integer (64-bit)", "yes", 1, 0);
                    }
                    else if (theString.Contains("_l"))
                    {
                        results.SetResult("String to Integer locale (64-bit)", "yes", 1, 0);
                    }
                    
                }
                else 
                {
                    if (theString.Contains("wtoi"))
                    {
                        results.SetResult("String(wide) to Integer", "yes", 1, 0);
                    }
                    else if (theString.Contains("atoi"))
                    {
                        results.SetResult("String to Integer", "yes", 1, 0);
                    }
                    else if (theString.Contains("_l"))
                    {
                        results.SetResult("String to Integer locale", "yes", 1, 0);
                    }
                    
                }
            }

            return true;
        }
    }
}
