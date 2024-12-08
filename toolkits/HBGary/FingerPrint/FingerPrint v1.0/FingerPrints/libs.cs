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
    class libs : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            Regex r_xvid = new Regex("xvid codec \\((?<version>[0-9\\.]+)\\)", RegexOptions.IgnoreCase);
            Match aMatch = r_xvid.Match(theString);
            if (aMatch.Success)
            {
                string version = aMatch.Groups["version"].ToString();
                results.AddResult("XviD codec", version, 1, 0);
            }

            Regex r_png = new Regex("MNG features are not allowed in a PNG datastream", RegexOptions.IgnoreCase);
            aMatch = r_png.Match(theString);
            if (aMatch.Success)
            {
                results.AddResult("libpng", "yes", 1, 0);
            }

            Regex r_inflate = new Regex("inflate (?<version>[0-9\\.]+) Copyright 1995", RegexOptions.IgnoreCase);
            aMatch = r_inflate.Match(theString);
            if (aMatch.Success)
            {
                string version = aMatch.Groups["version"].ToString();
                results.AddResult("Inflate Library", version, 1, 0);
            }

            Regex r_lexx = new Regex("yy_create_buffer", RegexOptions.IgnoreCase);
            aMatch = r_lexx.Match(theString);
            if (aMatch.Success)
            {
                results.AddResult("Lex/Yacc", "yes", 1, 0);
            }

            Regex r__stl_malloc = new Regex("AVbad_alloc");
            aMatch = r__stl_malloc.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("STL new", "yes", 1, 0);
            }

            return true;
        }
    }
}
