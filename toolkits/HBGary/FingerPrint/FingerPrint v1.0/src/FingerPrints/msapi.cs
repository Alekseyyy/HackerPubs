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
    class msapi : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            theString = theString.ToLower();

            if (theString.Contains("createprocess"))
            {                
                if (theString.Contains("createprocessasuser"))
                {
                    results.AppendResult("CreateProcess", "AsUser", 1, 0);
                }
                else if (theString.Contains("createprocesswithlogon"))
                {
                    results.AppendResult("CreateProcess", "WithLogon", 1, 0);
                }
                else if (theString.Contains("createprocesswithtoken"))
                {
                    results.AppendResult("CreateProcess", "WithToken", 1, 0);
                }
                else
                {
                    results.AppendResult("CreateProcess", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("shellexecute"))
            {
                if (theString.Contains("shellexecuteex"))
                {
                    results.AppendResult("ShellExecute", "Ex", 1, 0);
                }
                else
                {
                    results.AppendResult("ShellExecute", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("cmd"))
            {
                if (theString.Contains("cmd.exe /c"))
                {
                    results.AppendResult("Command shell", "single run", 1, 0);
                }
                else if (theString.Contains("cmd.exe /q"))
                {
                    results.AppendResult("Command shell", "echo off", 1, 0);
                }
                else
                {
                    results.AppendResult("Command shell", "Generic", 1, 0);
                }
            }
            else if ((theString.Contains("_system")) || (theString.Contains("_wsystem")))
            {
                bool bWide = theString.Contains("_w");

                results.AppendResult("crt shell", bWide ? "wide" : "ansi", 1, 0);
            }
            else if ((theString.Contains("findfirst")) || (theString.Contains("findnext")))
            {
                if (theString.Contains("_find"))
                {
                    results.AppendResult("CRT File Searching", "yes", 1, 0);
                }
                if ((theString.Contains("findfirstfile")) || (theString.Contains("findnextfile")))
                {
                    if (theString.Contains("ex"))
                    {
                        results.AppendResult("Win32 File Searching", "Ex", 1, 0);
                    }
                    else
                    {
                        results.AppendResult("Win32 File Searching", "Generic", 1, 0);
                    }
                }
            }
            else if (theString.Contains("commandlinetoargv"))
            {
                results.AppendResult("Command line parsing", "Win32", 1, 0);
            }
            else if (theString.Contains("getcommandline"))
            {
                results.AppendResult("Command line parsing", "Win32", 1, 0);
            }
            else if ((theString.Contains("_setargv")) || (theString.Contains("_wsetargv")))
            {
                results.AppendResult("Command line parsing", "CRT", 1, 0);
            }
            else if (theString.Contains("loadlibrary"))
            {
                if (theString.Contains("ex"))
                {
                    results.AppendResult("LoadLibrary", "Ex", 1, 0);
                }
                else
                {
                    results.AppendResult("LoadLibrary", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("getprocaddress"))
            {
                results.SetResult("GetProcAddress", "yes", 1, 0);
            }
            else if (theString.Contains("createthread"))
            {
                if (theString.Contains("ex"))
                {
                    results.AppendResult("Thread Creation", "Ex", 1, 0);
                }
                else
                {
                    results.AppendResult("Thread Creation", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("_beginthread"))
            {
                results.AppendResult("Thread Creation", "CRT", 1, 0);
            }
            else if (theString.Contains("createremotethread"))
            {
                results.AppendResult("Remote Thread", "Generic", 1, 0);
            }
            else if ((theString.Contains("createtoolhelp32snapshot")) ||
                     (theString.Contains("enumprocesses")) ||
                     (theString.Contains("process32first")) ||
                     (theString.Contains("process32next")))
            {
                results.AppendResult("Process Enumeration", "toolhelp library", 1, 0);
            }
            else if (theString.Contains("enumprocessmodules"))
            {
                results.AppendResult("Process Enumeration", "modules", 1, 0);
            }
            else if (theString.Contains("readprocessmemory"))
            {
                if (theString.Contains("toolhelp32readprocessmemory"))
                {
                    results.AppendResult("Read Process memory", "toolhelp library", 1, 0);
                }
                else
                {
                    results.AppendResult("Read Process memory", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("writeprocessmemory"))
            {
                results.AppendResult("WriteProcessMemory", "Generic", 1, 0);
            }
            else if ((theString.Contains("virtualquery")) ||
                     (theString.Contains("virtualalloc")) ||
                     (theString.Contains("virtualfree")))
            {
                results.AppendResult("Virtual Memory", "Generic", 1, 0);
            }
            else if (theString.Contains("virtualprotect"))
            {
                if (theString.Contains("ex"))
                {
                    results.AppendResult("Virtual Memory", "ProtectEx", 1, 0);
                }
                else
                {
                    results.AppendResult("Virtual Memory", "Protect", 1, 0);
                }
            }
            else if ((theString.Contains("mapuserphysicalpages")) ||
                     (theString.Contains("allocateuserphysicalpages")) ||
                     (theString.Contains("freeuserphysicalpages")))
            {
                results.SetResult("AWE", "Generic", 1, 0);
            }
            else if ((theString.Contains("createfilemapping")) ||
                     (theString.Contains("mapviewoffile")) ||
                     (theString.Contains("openfilemapping")))
            {
                results.AppendResult("File Mapping", "Generic", 1, 0);
            }
            else if ((theString.Contains("getprocessdeppolicy")) ||
                     (theString.Contains("getsystemdeppolicy")) ||
                     (theString.Contains("setsystemdeppolicy")))
            {
                results.SetResult("DEP aware", "yes", 1, 0);
            }
            else if ((theString.Contains("deviceiocontrol")) ||
                     (theString.Contains("installnewdevice")) ||
                     (theString.Contains("registerdevicenotification")))
            {
                results.SetResult("Device Management", "yes", 1, 0);
            }
            else if ((theString.Contains("findfirstvolumne")) ||
                     (theString.Contains("findnextvolume")) ||
                     (theString.Contains("setvolumemountpoint")) ||
                     (theString.Contains("getvolumeinformation")) ||
                     (theString.Contains("getvolumepathname")))
            {
                results.SetResult("Volume Management", "yes", 1, 0);
            }
            else if ((theString.Contains("getdrivetype")) ||
                     (theString.Contains("getlogicaldrives")))
            {
                results.SetResult("Drive Query", "yes", 1, 0);
            }
            else if ((theString.Contains("gettempfilename")) ||
                     (theString.Contains("gettemppath")))
            {
                results.SetResult("Temp file locations", "yes", 1, 0);
            }
            else if ((theString.Contains("changeclipboardchain")) ||
                (theString.Contains("emptyclipboard")) ||
                (theString.Contains("getclipboarddata")) ||
                (theString.Contains("openclipboard")) ||
                (theString.Contains("setclipboarddata")) ||
                (theString.Contains("registerclipboardformat")))
            {
                results.SetResult("Clipboard aware", "yes", 1, 0);
            }
            else if ((theString.Contains("ddesetqualityofservice")) ||
                (theString.Contains("packddeiparam")) ||
                (theString.Contains("reuseddeiparam")) ||
                (theString.Contains("freeddeiparam")) ||
                (theString.Contains("impersonateddeclientwindow")) ||
                (theString.Contains("unpackddeiparam")))
            {
                results.SetResult("DDE aware", "yes", 1, 0);
            }
            else if ((theString.Contains("createmailslot")) ||
                (theString.Contains("getmailslotinfo")) ||
                (theString.Contains("setmailslotinfo")))
            {
                results.SetResult("Mailslot aware", "yes", 1, 0);
            }
            else if ((theString.Contains("createpipe")) ||
                (theString.Contains("callnamedpipe")) ||
                (theString.Contains("connectnamedpipe")) ||
                (theString.Contains("createnamedpipe")) ||
                (theString.Contains("peeknamedpipe")) ||
                (theString.Contains("waitnamedpipe")))
            {
                results.SetResult("Named Pipe aware", "yes", 1, 0);
            }
            else if ((theString.Contains("cocreateinstance")) ||
                (theString.Contains("cogetclassobject")) ||
                (theString.Contains("cogetmalloc")) ||
                (theString.Contains("cogetobject")) ||
                (theString.Contains("coinitialize")) ||
                (theString.Contains("comarshalinterface")))
            {
                results.SetResult("COM aware", "yes", 1, 0);
            }
            else if ((theString.Contains("entercriticalsection")) ||
                (theString.Contains("leavecriticalsection")) ||
                (theString.Contains("initializecriticalsection")))
            {
                results.SetResult("Critical Sections", "yes", 1, 0);
            }
            else if ((theString.Contains("createevent")) ||
                (theString.Contains("openevent")) ||
                (theString.Contains("resetevent")) ||
                (theString.Contains("pulseevent")) ||
                (theString.Contains("setevent")))
            {
                results.SetResult("Events", "yes", 1, 0);
            }
            else if (theString.Contains("interlocked"))
            {
                results.SetResult("Atomic operations", "yes", 1, 0);
            }
            else if ((theString.Contains("createmutex")) ||
                (theString.Contains("openmutex")) ||
                (theString.Contains("releasemutex")))
            {
                results.SetResult("Mutexes", "yes", 1, 0);
            }
            else if ((theString.Contains("createsemaphore")) ||
                (theString.Contains("opensemaphore")) ||
                (theString.Contains("releasesemaphore")))
            {
                results.SetResult("Semaphores", "yes", 1, 0);
            }
            else if ((theString.Contains("createwaitabletimer")) ||
                (theString.Contains("openwaitabletimer")) ||
                (theString.Contains("setwaitabletimer")))
            {
                results.SetResult("WaitableTimers", "yes", 1, 0);
            }
            else if ((theString.Contains("createtimerqueue")) ||
                (theString.Contains("changetimerqueue")) ||
                (theString.Contains("deletetimerqueue")))
            {
                results.SetResult("Timer Queues", "yes", 1, 0);
            }
            else if ((theString.Contains("slisthead")) ||
                (theString.Contains("entryslist")) ||
                (theString.Contains("depthslist")))
            {
                results.SetResult("SList usage", "yes", 1, 0);
            }
            else if (theString.Contains("queueuserapc"))
            {
                results.SetResult("User mode APCs", "yes", 1, 0);
            }
            else if ((theString.Contains("createfile")) ||
                (theString.Contains("readfile")) ||
                (theString.Contains("writefile")))
            {
                if (theString.Contains("ex"))
                {
                    results.AppendResult("File IO", "Win32 EX", 1, 0);
                }
                else
                {
                    results.AppendResult("File IO", "Win32", 1, 0);
                }
            }
            else if ((theString.Contains("deletefile")))
            {
                results.AppendResult("File IO", "delete", 1, 0);
            }
            else if ((theString.Contains("heapalloc")) ||
                (theString.Contains("localalloc")) ||
                (theString.Contains("globalalloc")))
            {
                results.AppendResult("Memory", "Win32", 1, 0);
            }
            else if ((theString.Contains("openprocesstoken")) ||
                (theString.Contains("openthreadtoken")) || 
                (theString.Contains("settokeninformation")) || 
                (theString.Contains("gettokeninformation")))
            {
                results.AppendResult("Privilege", "Get", 1, 0);
            }
            else if ((theString.Contains("adjusttoken")) || 
                (theString.Contains("settokeninformation")))
            {
                results.AppendResult("Privilege", "Set", 1, 0);
            }                
            else if (theString.Contains("seshutdownprivilege"))
            {
                results.AppendResult("Privilege", "Shutdown", 1, 0);
            }
            else if (theString.Contains("sedebugprivilege"))
            {
                results.AppendResult("Privilege", "Debug", 1, 0);
            }
            else if (theString.Contains("privateprofile"))
            {
                results.SetResult("Profile", "private", 1, 0);
            }
            else if (theString.Contains("setfiletime"))
            {
                results.AppendResult("File Time", "Set", 1, 0);
            }
            else if (theString.Contains("getfiletime"))
            {
                results.AppendResult("File Time", "Get", 1, 0);
            }
            else if ((theString.Contains("beginpaint")) ||
                (theString.Contains("defwindowproc")) ||
                (theString.Contains("sendmessage")) ||
                (theString.Contains("postmessage")) || 
                (theString.Contains("postquitmessage")))
            {
                results.AppendResult("Window", "aware", 1, 0);
            }
            else if ((theString.Contains("findwindow")) ||
                (theString.Contains("enumwindow")))
            {
                results.AppendResult("Window", "enum", 1, 0);
            }
            else if ((theString.Contains("openscmanager")) ||
                (theString.Contains("openservice")))
            {
                results.AppendResult("Services", "open", 1, 0);
            }
            else if (theString.Contains("createservice"))
            {
                results.AppendResult("Services", "create", 1, 0);
            }
            else if (theString.Contains("servicemain"))
            {
                results.AppendResult("Services", "main", 1, 0);
            }
            else if (theString.Contains("controlservice"))
            {
                results.AppendResult("Services", "control", 1, 0);
            }
            else if (theString.Contains("startservice"))
            {
                results.AppendResult("Services", "start", 1, 0);
            }
            else if ((theString.Contains("closewindowstation")) ||
                (theString.Contains("createwindowstation")) ||
                (theString.Contains("openwindowstation")))
            {
                results.AppendResult("Window Station", "aware", 1, 0);
            }
            else if ((theString.Contains("enumwindowstations")) ||
                (theString.Contains("getprocesswindowstation")))
            {
                results.AppendResult("Window Station", "enum", 1, 0);
            }
            else if ((theString.Contains("closedesktop")) ||
                (theString.Contains("createdesktop")) ||
                (theString.Contains("opendesktop")))
            {
                results.AppendResult("Desktop", "aware", 1, 0);
            }
            else if ((theString.Contains("enumdesktop")) ||
                (theString.Contains("getthreadesktop")))
            {
                results.AppendResult("Desktop", "enum", 1, 0);
            }
            else if ((theString.Contains("tlsalloc")) ||
                (theString.Contains("tlsfree")) ||
                (theString.Contains("tlsgetvalue")) ||
                (theString.Contains("tlssetvalue")))
            {
                results.AppendResult("TLS", "aware", 1, 0);
            }
            else if (theString.Contains("wow64"))
            {
                results.AppendResult("Wow64", "aware", 1, 0);
            }
            else if ((theString.Contains("setwindowshook")) ||
                (theString.Contains("callnexthook")))
            {
                results.AppendResult("Windows Hook", "aware", 1, 0);
            }
            else if ((theString.Contains("querylicensevalue")) ||
                (theString.Contains("fetchlicensedata")) ||
                (theString.Contains("updatelicensedata")))
            {
                results.AppendResult("Windows Licensing", "aware", 1, 0);
            }
            else if ((theString.Contains("getkeystate")) ||
                (theString.Contains("getasynckeystate")) ||
                (theString.Contains("vkkeyscanex")) ||
                (theString.Contains("mapvirtualkey")))
            {
                results.AppendResult("Virtual Key", "aware", 1, 0);
            }
            else
            {
                Regex r_crt_fileio = new Regex("^(|_)f(open|close|seek|read|write)$");
                Match aMatch = r_crt_fileio.Match(theString);
                if (aMatch.Success)
                {
                    results.AppendResult("File IO", "CRT", 1, 0);
                }

                Regex r_crt_malloc = new Regex("^(|_)malloc_crt$");
                aMatch = r_crt_malloc.Match(theString);
                if (aMatch.Success)
                {
                    results.AppendResult("Memory", "CRT", 1, 0);
                }

                Regex r_exec = new Regex("^_(?<type>exec|wexec|spawn|wspawn)(?<extra>[a-z])$", RegexOptions.IgnoreCase);
                aMatch = r_exec.Match(theString);
                if (aMatch.Success)
                {
                    bool bWide = theString.Contains("_w");

                    string type = aMatch.Groups["type"].ToString();
                    string extra = aMatch.Groups["extra"].ToString();

                    results.AppendResult("CRT exec", type + " " + (bWide ? "wide " + extra : "ansi " + extra), 1, 0);
                }
            }

            return true;
        }


        override public bool OnRegisterPatterns(PatternList theList)
        {

            return true;
        }
    }
}
