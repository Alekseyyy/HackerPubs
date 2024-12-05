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
    class compression : BaseFingerPrint
    {
        override public bool OnEvaluateStringSet(Dictionary<string, bool> stringSet, ScanResultCollection results)
        {
            if ((stringSet.ContainsKey("LZOpenFile".ToLower())) ||
                (stringSet.ContainsKey("LZClose".ToLower())) ||
                (stringSet.ContainsKey("LZCopy".ToLower())) ||
                (stringSet.ContainsKey("LZRead".ToLower())) ||
                (stringSet.ContainsKey("LZInit".ToLower())) ||
                (stringSet.ContainsKey("LZSeek".ToLower())))
            {
                results.SetResult("LZ Compression", "yes", 1, 0);
            }
            else if ((stringSet.ContainsKey("UPX0".ToLower())) ||
                (stringSet.ContainsKey("UPX1".ToLower())))
            {
                results.SetResult("UPX Packing", "yes", 1, 0);
            }

            return true;
        }
    }
}
