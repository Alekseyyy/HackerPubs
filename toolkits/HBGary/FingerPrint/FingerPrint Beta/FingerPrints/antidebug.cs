using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class antidebug : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if (theString.Contains("IsDebuggerPresent"))
            {
                results.AppendResult("Debugger Check", "API", 1, 0);
            }
            else if (theString.Contains("IsDebugged"))
            {
                results.AppendResult("Debugger Check", "PEB", 1, 0);
            }
            else if (theString.Contains("NtGlobalFlags"))
            {
                results.AppendResult("Debugger Check", "Global Flags", 1, 0);
            }
            else if (theString.Contains("QueryInformationProcess"))
            {
                results.AppendResult("Debugger Check", "QueryInfo", 1, 0);
            }
            else if (theString.Contains("CheckRemoteDebuggerPresent"))
            {
                results.AppendResult("Debugger Check", "RemoteAPI", 1, 0);
            }

            else if (theString.Contains("SetInformationThread"))
            {
                results.AppendResult("Debugger Hiding", "Thread", 1, 0);
            }
            else if (theString.Contains("DebugActiveProcess"))
            {
                results.AppendResult("Debugger Hiding", "Active", 1, 0);
            }
            else if (theString.Contains("QueryPerformanceCounter"))
            {
                results.AppendResult("Debugger Timing", "PerformanceCounter", 1, 0);
            }
            else if (theString.Contains("GetTickCount"))
            {
                results.AppendResult("Debugger Timing", "Ticks", 1, 0);
            }
            else if (theString.Contains("OutputDebugString"))
            {
                results.AppendResult("Debugger Output", "String", 1, 0);
            }

            else if (theString.Contains("SetUnhandledExceptionFilter"))
            {
                results.AppendResult("Debugger Exception", "UnhandledFilter", 1, 0);
            }
            else if (theString.Contains("GenerateConsoleCtrlEvent"))
            {
                results.AppendResult("Debugger Exception", "ConsoleCtrl", 1, 0);
            }
            else if (theString.Contains("SetConsoleCtrlHandler"))
            {
                results.AppendResult("Debugger Exception", "SetConsoleCtrl", 1, 0);
            }

            else if (theString.Contains("SetThreadContext"))
            {
                results.AppendResult("Thread Control", "Context", 1, 0);
            }
            else if (theString.Contains("_invoke_watson"))
            {
                results.AppendResult("Debugger Check", "DrWatson", 1, 0);
            }
            else if ((theString.Contains("__except_handler3")) || (theString.Contains("__local_unwind3")))
            {
                // VS < 8.0
                results.AppendResult("SEH", "v3", 1, 0);
            }
            else if ((theString.Contains("__except_handler4")) || 
                (theString.Contains("__local_unwind4")) ||
                (theString.Contains("_XcptFilter"))
                )
            {
                // VS 8.0+
                results.AppendResult("SEH", "v4", 1, 0);
            }
            else if ((theString.Contains("AddVectoredExceptionHandler")) || (theString.Contains("RemoveVectoredExceptionHandler")))
            {
                results.AppendResult("VEH", "vectored", 1, 0);
            }

            return true;
        }


        override public bool OnRegisterPatterns(PatternList theList)
        {

            AddPattern(theList, "RDTSC", "0F 31", 1, 0, null);
            AddPattern(theList, "CPUID", "0F A2", 1, 0, null);

            AddPattern(theList, "SEH saves", "64 ff 35 00 00 00 00", 1, 0, null);
            AddPattern(theList, "SEH inits", "64 89 25 00 00 00 00", 1, 0, null);

            // todo:
            // anti-debugging
            // push offset @handler
            // push dword fs:[0]
            // mov fs:[0], esp
            // db 0f1h          // generates a single step exception

            // todo:
            // anti-debugging
            // push offset @handler
            // push dword fs:[0]
            // mov fs:[0], esp
            // db 0f1h          // generates a single step exception

            // pushf
            // mov dword [esp], 0x100
            // popf

            // push ss
            // *
            // pop ss
            // pushf
            // *
            // pop eax
            // and eax, 0x100
            // or eax, eax
            // jnz @debugger

            return true;
        }
    }
}
