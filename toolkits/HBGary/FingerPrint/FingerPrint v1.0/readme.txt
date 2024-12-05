//------------------------------------------------------------------------
//
// Copyright © 2010 HBGary, Inc.  All Rights Reserved. 
//
//------------------------------------------------------------------------

FingerPrint v1.0

-----------------------------------------------------------
Brief Overview
-----------------------------------------------------------

FingerPrint is a simple framework for scanning binaries (preferably binaries extracted from memory so they are already unpacked).  It allows scanning for ascii/wide strings and byte patterns, then annotating results.  Results are saved in an xml format and can be compared to previous results.  The goal is to allow quick development of new search patterns and easy comparison to previous binaries.

FingerPrint is 100% C# and requires the Microsoft .NET Framework v3.5

FingerPrint is extendable via "FingerPrints"... FingerPrints are C# files that implement the IFingerPrint interface (aka plugins).  You can create new FingerPrints and the FP.exe will automatically compile and execute them if they are placed in the \FingerPrint sub directory.

The Source code is provided, with restrictions (see HBGary-FREEWARE-EULA.txt), primarily that it not be resold.


-----------------------------------------------------------
General Usage
-----------------------------------------------------------


 fp <file or directory> to get a dump of fingerprint data

 fp -c <file 1> <file 2> to compare two files

 fp -c <directory> to scan a directory and compare it to the scan history, showing a summary of results

 fp -r <directory> to recursively scan a directory

 fp -db <file 1> to compare a file to the scan history, only showing > 80% matches

 fp -dball <file 1> to compare a file to the scan history, showing all comparisons

Everytime you fingerprint a file, it is added to a database called "scan_history.xml" in the current directory.  Scan_history.xml can get very large when examining large sets of files, so if you need more speed/efficiency, modify the ScanResults.cs class to output a binary format or backend to SQL.


-----------------------------------------------------------
Create a new FingerPrint
-----------------------------------------------------------

1) copy an existing plugin (for example libs.cs) and rename it as appropriate.
2) open the new plugin and edit the class name at the top to something appropriate, for example:

namespace FP
{
    class my_new_fingerprint : BaseFingerPrint
    {

    }
}

3) for string based fingerprinting, edit or add the OnEvaluateString() function to your class:

override public bool OnEvaluateString(string theString,
ScanResultCollection results)
{
    if (theString == "HBGary, Inc.")
    {
       results.SetResult("HBGary found", "yes", 1, 0)  // this will overwrite any existing results named HBGary Official!
       // or results.AddResult("HBGary Official!", "yes", 1, 0) // this will create duplicates
    }
}

Add/SetResult parameters: (string name, string value, int weight, int uniquness)

weight is used in determining match percentage for comparisons uniqueness is not currently used

4) for byte based fingerprinting, edit or add the OnRegisterPatterns() function to your class:

override public bool OnRegisterPatterns(PatternList theList)
{
    AddPattern(theList, "Debug stuff!", "ba df f0 0d", 1, 0, null);
    AddPattern(theList, "byte pattern!", "56 8b 35 ?? 00 00", 1, 0, null);
}

AddPattern parameters: (PatternList, string name, string bytepattern, int weight, int uniqueness, object tag)

bytepattern is hexadecimal, space delimited, with no 0x prefix... ?? is a wildcard tag