//------------------------------------------------------------------------
//
// Copyright © 2010 HBGary, Inc.  All Rights Reserved. 
//
//------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class microsoft : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            Regex r_wingdi = new Regex("^(comctl32.dll|gdi32.dll)$", RegexOptions.IgnoreCase);
            Match aMatch = r_wingdi.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows GDI/Common Controls", "yes", 1, 0);
            }

            Regex r_winmm = new Regex("^(winmm.dll)$", RegexOptions.IgnoreCase);
            aMatch = r_winmm.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows Multimedia", "yes", 1, 0);
            }

            Regex r_wsock = new Regex("^(wsock32.dll|ws2_32.dll)$", RegexOptions.IgnoreCase);
            aMatch = r_wsock.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows socket library", "yes", 1, 0);
            }

            Regex r_winet = new Regex("^(wininet.dll)$", RegexOptions.IgnoreCase);
            aMatch = r_winet.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows Internet API", "yes", 1, 0);
            }

            Regex r_hhctrl = new Regex("^(hhctrl.ocx)$", RegexOptions.IgnoreCase);
            aMatch = r_hhctrl.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows HTML Help Control", "yes", 1, 0);
            }

            Regex r_msvid1 = new Regex("^(msvfw32.dll)$", RegexOptions.IgnoreCase);
            aMatch = r_msvid1.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Windows Video For Windows", "yes", 1, 0);
            }

            if (theString.Contains("Microsoft (c)"))
            {
                results.AppendResult("MS copyright", "faked", 1, 0);
            }

            return true;
        }
    }
}
