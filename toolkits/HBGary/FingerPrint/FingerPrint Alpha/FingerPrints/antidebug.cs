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
                results.SetResult("IsDebuggerPresent", "yes", 1, 0);
            }
            if (theString.Contains("IsDebugged"))
            {
                results.SetResult("IsDebugged", "yes", 1, 0);
            }
            if (theString.Contains("NtGlobalFlags"))
            {
                results.SetResult("NtGlobalFlags", "yes", 1, 0);
            }
            if (theString.Contains("QueryInformationProcess"))
            {
                results.SetResult("QueryInformationProcess", "yes", 1, 0);
            }
            if (theString.Contains("CheckRemoteDebuggerPresent"))
            {
                results.SetResult("CheckRemoteDebuggerPresent", "yes", 1, 0);
            }
            if (theString.Contains("SetUnhandledExceptionFilter"))
            {
                results.SetResult("SetUnhandledExceptionFilter", "yes", 1, 0);
            }
            if (theString.Contains("SetInformationThread"))
            {
                results.SetResult("SetInformationThread", "yes", 1, 0);
            }
            if (theString.Contains("DebugActiveProcess"))
            {
                results.SetResult("DebugActiveProcess", "yes", 1, 0);
            }
            if (theString.Contains("QueryPerformanceCounter"))
            {
                results.SetResult("QueryPerformanceCounter", "yes", 1, 0);
            }
            if (theString.Contains("GetTickCount"))
            {
                results.SetResult("GetTickCount", "yes", 1, 0);
            }
            if (theString.Contains("OutputDebugString"))
            {
                results.SetResult("OutputDebugString", "yes", 1, 0);
            }
            if (theString.Contains("GenerateConsoleCtrlEvent"))
            {
                results.SetResult("GenerateConsoleCtrlEvent", "yes", 1, 0);
            }
            if (theString.Contains("SetConsoleCtrlHandler"))
            {
                results.SetResult("SetConsoleCtrlHandler", "yes", 1, 0);
            }
            if (theString.Contains("SetThreadContext"))
            {
                results.SetResult("SetThreadContext", "yes", 1, 0);
            }

            if (theString.Contains("_invoke_watson"))
            {
                results.SetResult("Invoke Watson", "yes", 1, 0);
            }
            if ((theString.Contains("__except_handler3")) || (theString.Contains("__local_unwind3")))
            {
                // VS < 8.0
                results.SetResult("SEH v3", "yes", 1, 0);
            }
            if ((theString.Contains("__except_handler4")) || (theString.Contains("__local_unwind4")))
            {
                // VS 8.0+
                results.SetResult("SEH v4", "yes", 1, 0);
            }

            if (theString.Contains("_XcptFilter"))
            {
                results.SetResult("SEH v4", "yes", 1, 0);
            }

            if ((theString.Contains("AddVectoredExceptionHandler")) || (theString.Contains("RemoveVectoredExceptionHandler")))
            {
                results.SetResult("VEH", "yes", 1, 0);
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
