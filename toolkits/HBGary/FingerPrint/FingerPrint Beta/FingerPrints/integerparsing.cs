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
            //Regular expression for data conversion types: ato* & wto*
            //Regex aMatch = new Regex("(a|w)+(to)+(i|l|f|db|dbl)+(64|)(_l|)");
            
            //Make theString LowerCase
            string theStringToLower = theString.ToLower();

            // remove any prefix _
            if (theStringToLower.StartsWith("_"))
                theStringToLower = theStringToLower.Substring(1);

            if (theStringToLower.Contains("64"))
            {
                results.AppendResult("DataConversion", "64bit", 1, 0);
            }

            // prefixes
            if ((theStringToLower.StartsWith("wto")) || (theStringToLower.StartsWith("wcsto")))
            {
                results.AppendResult("DataConversion", "wide", 1, 0);

            }
            else if ((theStringToLower.StartsWith("ato")) || (theStringToLower.StartsWith("strto")))
            {
                results.AppendResult("DataConversion", "ansi", 1, 0);
            }

            // middle/postfixes
            if (theStringToLower.Contains("flt"))
            {
                results.AppendResult("DataConversion", "float", 1, 0);
            }
            else if (theStringToLower.Contains("ldbl"))
            {
                results.AppendResult("DataConversion", "long double", 1, 0);
            }
            else if ((theStringToLower.Contains("dbl")) || 
                (theStringToLower.Contains("tof")) ||
                (theStringToLower.Contains("tod"))
                ) // atof is actual to double not float
            {
                results.AppendResult("DataConversion", "double", 1, 0);
            }
            else if (theStringToLower.Contains("tol"))
            {
                results.AppendResult("DataConversion", "long", 1, 0);
            }
            else if (theStringToLower.Contains("toul"))
            {
                results.AppendResult("DataConversion", "ulong", 1, 0);
            }


            if (theStringToLower.EndsWith("_l"))
            {
                results.AppendResult("DataConversion", "locale", 1, 0);
            }

            return true;
        }
    }
}
