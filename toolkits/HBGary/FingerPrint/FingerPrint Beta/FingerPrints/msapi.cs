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
                    results.AppendResult("CreateProcess", "AsUser", 1, 0);
                }
                else if (theString.Contains("CreateProcessWithLogon"))
                {
                    results.AppendResult("CreateProcess", "WithLogon", 1, 0);
                }
                else if (theString.Contains("CreateProcessWithToken"))
                {
                    results.AppendResult("CreateProcess", "WithToken", 1, 0);
                }
                else
                {
                    results.AppendResult("CreateProcess", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("ShellExecute"))
            {
                if (theString.Contains("ShellExecute"))
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

                results.AppendResult("CRT shell", bWide ? "wide" : "ansi", 1, 0);
            }
            else if ((theString.Contains("findfirst")) || (theString.Contains("findnext")))
            {
                if (theString.Contains("_find"))
                {
                    results.AppendResult("CRT File Searching", "yes", 1, 0);
                }
                if ((theString.Contains("FindFirstFile")) || (theString.Contains("FindNextFile")))
                {
                    if (theString.Contains("Ex"))
                    {
                        results.AppendResult("Win32 File Searching", "Ex", 1, 0);
                    }
                    else
                    {
                        results.AppendResult("Win32 File Searching", "Generic", 1, 0);
                    }
                }
            }
            else if (theString.Contains("CommandLineToArgv"))
            {
                results.AppendResult("Command line parsing", "Win32", 1, 0);
            }
            else if (theString.Contains("GetCommandLine"))
            {
                results.AppendResult("Command line parsing", "Win32", 1, 0);
            }
            else if ((theString.Contains("_setargv")) || (theString.Contains("_wsetargv")))
            {
                results.AppendResult("Command line parsing", "CRT", 1, 0);
            }
            else if (theString.Contains("LoadLibrary"))
            {
                if (theString.Contains("LoadLibraryEx"))
                {
                    results.AppendResult("LoadLibrary", "Ex", 1, 0);
                }
                else
                {
                    results.AppendResult("LoadLibrary", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("GetProcAddress"))
            {
                results.SetResult("GetProcAddress", "yes", 1, 0);
            }
            else if (theString.Contains("CreateThread"))
            {
                if (theString.Contains("CreateThreadEx"))
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
            else if (theString.Contains("CreateRemoteThread"))
            {
                results.AppendResult("Remote Thread", "Generic", 1, 0);
            }
            else if ((theString.Contains("CreateToolhelp32Snapshot")) || 
                     (theString.Contains("Process32First")) ||
                     (theString.Contains("Process32Next")))
            {
                results.AppendResult("Process Enumeration", "toolhelp library", 1, 0);
            }
            else if (theString.Contains("ReadProcessMemory"))
            {
                if (theString.Contains("Toolhelp32ReadProcessMemory"))
                {
                    results.AppendResult("Read Process memory", "toolhelp library", 1, 0);
                }
                else
                {
                    results.AppendResult("Read Process memory", "Generic", 1, 0);
                }
            }
            else if (theString.Contains("WriteProcessMemory"))
            {
                results.AppendResult("WriteProcessMemory", "Generic", 1, 0);
            }
            else if ((theString.Contains("VirtualQuery")) ||
                     (theString.Contains("VirtualAlloc")) ||
                     (theString.Contains("VirtualFree")))
            {
                results.AppendResult("Virtual Memory", "Generic", 1, 0);
            }
            else if (theString.Contains("VirtualProtect"))
            {
                if (theString.Contains("VirtualProtectEx"))
                {
                    results.AppendResult("Virtual Memory", "ProtectEx", 1, 0);
                }
                else
                {
                    results.AppendResult("Virtual Memory", "Protect", 1, 0);
                }
            }
            else if ((theString.Contains("MapUserPhysicalPages")) ||
                     (theString.Contains("AllocateUserPhysicalPages")) ||
                     (theString.Contains("FreeUserPhysicalPages")))
            {
                results.SetResult("AWE", "Generic", 1, 0);
            }
            else if ((theString.Contains("CreateFileMapping")) ||
                     (theString.Contains("MapViewOfFile")) ||
                     (theString.Contains("OpenFileMapping")))
            {
                results.AppendResult("File Mapping", "Generic", 1, 0);
            }
            else if ((theString.Contains("GetProcessDEPPolicy")) ||
                     (theString.Contains("GetSystemDEPPolicy")) ||
                     (theString.Contains("SetSystemDEPPolicy")))
            {
                results.SetResult("DEP aware", "yes", 1, 0);
            }
            else if ((theString.Contains("DeviceIoControl")) ||
                     (theString.Contains("InstallNewDevice")) ||
                     (theString.Contains("RegisterDeviceNotification")))
            {
                results.SetResult("Device Management", "yes", 1, 0);
            }
            else if ((theString.Contains("FindFirstVolumne")) ||
                     (theString.Contains("FindNextVolume")) ||
                     (theString.Contains("SetVolumeMountPoint")) ||
                     (theString.Contains("GetVolumeInformation")) ||
                     (theString.Contains("GetVolumePathName")))
            {
                results.SetResult("Volume Management", "yes", 1, 0);
            }
            else if ((theString.Contains("GetDriveType")) ||
                     (theString.Contains("GetLogicalDrives")))
            {
                results.SetResult("Drive Query", "yes", 1, 0);
            }
            else if ((theString.Contains("GetTempFileName")) ||
                     (theString.Contains("GetTempPath")))
            {
                results.SetResult("Temp file locations", "yes", 1, 0);
            }
            else if ((theString.Contains("ChangeClipboardChain")) ||
                (theString.Contains("EmptyClipboard")) ||
                (theString.Contains("GetClipboardData")) ||
                (theString.Contains("OpenClipboard")) ||
                (theString.Contains("SetClipboardData")) ||
                (theString.Contains("RegisterClipboardFormat")))
            {
                results.SetResult("Clipboard aware", "yes", 1, 0);
            }
            else if ((theString.Contains("DdeSetQualityOfService")) ||
                (theString.Contains("PackDDEIParam")) ||
                (theString.Contains("ReuseDDEIParam")) ||
                (theString.Contains("FreeDDEIParam")) ||
                (theString.Contains("ImpersonateDdeClientWindow")) ||
                (theString.Contains("UnpackDDEIparam")))
            {
                results.SetResult("DDE aware", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateMailslot")) ||
                (theString.Contains("GetMailslotInfo")) ||
                (theString.Contains("SetMailslotInfo")))
            {
                results.SetResult("Mailslot aware", "yes", 1, 0);
            }
            else if ((theString.Contains("CreatePipe")) ||
                (theString.Contains("CallNamedPipe")) ||
                (theString.Contains("ConnectNamedPipe")) ||
                (theString.Contains("CreateNamedPipe")) ||
                (theString.Contains("PeekNamedPipe")) ||
                (theString.Contains("WaitNamedPipe")))
            {
                results.SetResult("Named Pipe aware", "yes", 1, 0);
            }
            else if ((theString.Contains("CoCreateInstance")) ||
                (theString.Contains("CoGetClassObject")) ||
                (theString.Contains("CoGetMalloc")) ||
                (theString.Contains("CoGetObject")) ||
                (theString.Contains("CoInitialize")) ||
                (theString.Contains("CoMarshalInterface")))
            {
                results.SetResult("COM aware", "yes", 1, 0);
            }
            else if ((theString.Contains("EnterCriticalSection")) ||
                (theString.Contains("LeaveCriticalSection")) ||
                (theString.Contains("InitializeCriticalSection")))
            {
                results.SetResult("Critical Sections", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateEvent")) ||
                (theString.Contains("OpenEvent")) ||
                (theString.Contains("ResetEvent")) ||
                (theString.Contains("PulseEvent")) ||
                (theString.Contains("SetEvent")))
            {
                results.SetResult("Events", "yes", 1, 0);
            }
            else if (theString.Contains("Interlocked"))
            {
                results.SetResult("Atomic operations", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateMutex")) ||
                (theString.Contains("OpenMutex")) ||
                (theString.Contains("ReleaseMutex")))
            {
                results.SetResult("Mutexes", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateSemaphore")) ||
                (theString.Contains("OpenSemaphore")) ||
                (theString.Contains("ReleaseSemaphore")))
            {
                results.SetResult("Semaphores", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateWaitableTimer")) ||
                (theString.Contains("OpenWaitableTimer")) ||
                (theString.Contains("SetWaitableTimer")))
            {
                results.SetResult("WaitableTimers", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateTimerQueue")) ||
                (theString.Contains("ChangeTimerQueue")) ||
                (theString.Contains("DeleteTimerQueue")))
            {
                results.SetResult("Timer Queues", "yes", 1, 0);
            }
            else if ((theString.Contains("SListHead")) ||
                (theString.Contains("EntrySList")) ||
                (theString.Contains("DepthSlist")))
            {
                results.SetResult("SList usage", "yes", 1, 0);
            }
            else if (theString.Contains("QueueUserAPC"))
            {
                results.SetResult("User mode APCs", "yes", 1, 0);
            }
            else if ((theString.Contains("CreateFile")) ||
                (theString.Contains("ReadFile")) ||
                (theString.Contains("WriteFile")))
            {
                results.AppendResult("File IO", "Win32", 1, 0);
            }
            else if ((theString.Contains("HeapAlloc")) ||
                (theString.Contains("LocalAlloc")) ||
                (theString.Contains("GlobalAlloc")))
            {
                results.AppendResult("Memory", "Win32", 1, 0);
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
