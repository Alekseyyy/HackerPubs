using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class compression : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if ((theString.Contains("LZOpenFile")) ||
                (theString.Contains("LZClose")) ||
                (theString.Contains("LZCopy")) ||
                (theString.Contains("LZRead")) ||
                (theString.Contains("LZInit")) ||
                (theString.Contains("LZSeek")))
            {
                results.SetResult("LZ Compression", "yes", 1, 0);
            }

            return true;
        }
    }
}
