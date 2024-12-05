using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class msapi : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if (theString.Contains("CreateProcess"))
            {
                if (theString.Contains("CreateProcessAsUser"))
                {
                    results.SetResult("CreateProcessAsUser", "yes", 1, 0);
                }
                else if (theString.Contains("CreateProcessWithLogon"))
                {
                    results.SetResult("CreateProcessWithLogon", "yes", 1, 0);
                }
                else if (theString.Contains("CreateProcessWithToken"))
                {
                    results.SetResult("CreateProcessWithToken", "yes", 1, 0);
                }
                else
                {
                    results.SetResult("CreateProcess", "yes", 1, 0);
                }
            }

            if (theString.Contains("ShellExecute"))
            {
                if (theString.Contains("ShellExecuteEx"))
                {
                    results.SetResult("ShellExecuteEx", "yes", 1, 0);
                }
                else
                {
                    results.SetResult("ShellExecute", "yes", 1, 0);
                }
            }

            if (theString.Contains("cmd"))
            {
                if (theString.Contains("cmd.exe /c"))
                {
                    results.SetResult("Command shell single run", "yes", 1, 0);
                }
                else if (theString.Contains("cmd.exe /q"))
                {
                    results.SetResult("Command shell echo off", "yes", 1, 0);
                }
                else if (theString.Contains("cmd.exe /q"))
                {
                    results.SetResult("Command shell echo off", "yes", 1, 0);
                }
                else
                {
                    results.SetResult("Command shell", "yes", 1, 0);
                }
            }

            if ((theString.Contains("_system")) || (theString.Contains("_wsystem")))
            {
                bool bWide = theString.Contains("_w");

                results.AddResult("CRT shell", bWide ? "wide" : "ansi", 1, 0);
            }

            Regex r_crt_fileio = new Regex("^(|_)f(open|close|seek|read|write)$");
            Match aMatch = r_crt_fileio.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("CRT File IO", "yes", 1, 0);
            }

            Regex r_crt_malloc = new Regex("^(|_)malloc_crt$");
            aMatch = r_crt_malloc.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("CRT Memory", "yes", 1, 0);
            }

            Regex r_exec = new Regex("^_(?<type>exec|wexec|spawn|wspawn)(?<extra>[a-z])$", RegexOptions.IgnoreCase);
            aMatch = r_exec.Match(theString);
            if (aMatch.Success)
            {
                bool bWide = theString.Contains("_w");

                string type = aMatch.Groups["type"].ToString();
                string extra = aMatch.Groups["extra"].ToString();

                results.AddResult("CRT " + type, bWide ? "wide " + extra : "ansi " + extra, 1, 0);
            }


            if ((theString.Contains("findfirst")) || (theString.Contains("findnext")))
            {
                if (theString.Contains("_find"))
                {
                    results.SetResult("CRT File Searching", "yes", 1, 0);
                }
                if ((theString.Contains("FindFirstFile")) || (theString.Contains("FindNextFile")))
                {
                    results.SetResult("Win32 File Searching", "yes", 1, 0);
                }
            }

            if (theString.Contains("CommandLineToArgv"))
            {
                results.SetResult("Win32 command line parsing", "yes", 1, 0);
            }
            else if (theString.Contains("GetCommandLine"))
            {
                results.SetResult("Win32 command line parsing", "yes", 1, 0);
            }
            else if ((theString.Contains("_setargv")) || (theString.Contains("_wsetargv")))
            {
                results.SetResult("CRT command line parsing", "yes", 1, 0);
            }


            return true;
        }


        override public bool OnRegisterPatterns(PatternList theList)
        {

            return true;
        }
    }
}
