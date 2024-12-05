using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FP
{
    class compiler : BaseFingerPrint
    {
        override public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            if (theString.Contains("Run-Time Check Failure #%d"))
            {
                results.AppendResult("RTTI", "enabled", 1, 0);
            }

            Regex r_source_path = new Regex("^(?<path>[A-Za-z]:\\\\.+\\.(cpp|c|h|hpp|cs|pdb))$", RegexOptions.IgnoreCase);
            Match aMatch = r_source_path.Match(theString);
            if (aMatch.Success)
            {
                string matchstring = aMatch.Groups["path"].ToString();
                string file_path = Path.GetDirectoryName(matchstring);

                results.AppendResult("Source Path", file_path, 1, 0);
            }
            Regex r_orig_source_path = new Regex("^(?<path>[A-Za-z]:\\\\.+\\.(pdb))$", RegexOptions.IgnoreCase);
            aMatch = r_orig_source_path.Match(theString);
            if (aMatch.Success)
            {
                // store source code path
                string matchstring = aMatch.Groups["path"].ToString();

                string file_name = Path.GetFileNameWithoutExtension(matchstring);
                results.AppendResult("Original Project Name", file_name, 1, 0);

                string file_path = Path.GetDirectoryName(matchstring);
                results.AppendResult("Original Source Path", file_path, 1, 0);
            }

            Regex r_delphi = new Regex("^(This program must be run under Win32|SOFTWARE\\\\Borland\\\\Delphi\\\\RTL)", RegexOptions.IgnoreCase);
            aMatch = r_delphi.Match(theString);
            if (aMatch.Success)
            {
                results.SetResult("Delpi", "yes", 1, 0);
            }

            Regex r_assemblyname = new Regex("assemblyIdentity version=.+\\<description\\>(?<description>.+)\\</description\\>", RegexOptions.IgnoreCase);
            aMatch = r_assemblyname.Match(theString);
            if (aMatch.Success)
            {
                string description = aMatch.Groups["description"].ToString();
                results.SetResult("Assembly Description", description, 1, 0);
            }

            Regex r_assembly = new Regex("assemblyIdentity version=\\\"(?<version>[0-9\\.]+)\\\" processorArchitecture=\\\"(?<proc>.+)\\\" name=\\\"(?<asm_name>.+)\\\" type", RegexOptions.IgnoreCase);
            aMatch = r_assembly.Match(theString);
            if (aMatch.Success)
            {
                string version = aMatch.Groups["version"].ToString();
                string processor = aMatch.Groups["proc"].ToString();
                string asm_name = aMatch.Groups["asm_name"].ToString();

                string assembly_name = asm_name + " version " + version + " for " + processor;
                results.AppendResult("Assembly Info", assembly_name, 1, 0);
            }

            Regex r_dassembly = new Regex("\\<dependentAssembly\\>\\<assemblyIdentity .+ name=\\\"(?<asm_name>.+)\\\" version=\\\"(?<version>[0-9\\.]+)\\\" processorArchitecture=\\\"(?<proc>.+)\\\" publicKeyToken=\\\"(?<token>[0-9a-fA-F]+)\\\" ", RegexOptions.IgnoreCase);
            aMatch = r_dassembly.Match(theString);
            if (aMatch.Success)
            {
                string version = aMatch.Groups["version"].ToString();
                string processor = aMatch.Groups["proc"].ToString();
                string asm_name = aMatch.Groups["asm_name"].ToString();
                string token = aMatch.Groups["token"].ToString();
                
                string dep = asm_name + " Version " + version + " for " + processor + " Key: " + token;

                results.AppendResult("Dependent Manifest", dep, 1, 0);
            }

            Regex r_MFC_dll = new Regex("^MFC(?<type>(|O|D|N|S))(?<version>[0-9]+)(?<debug>(|U|D|UD))\\.DLL", RegexOptions.IgnoreCase);
            aMatch = r_MFC_dll.Match(theString);
            if (aMatch.Success)
            {
                string s_type = aMatch.Groups["type"].ToString();
                string version = aMatch.Groups["version"].ToString();
                string s_debug = aMatch.Groups["debug"].ToString();

                //Console.WriteLine("Detected MFC version: " + version);
                string mfc_version = string.Empty;

                if (2 == version.Length)
                {
                    version = (version[0] + "." + version[1]);
                }

                switch (s_type.ToLower())
                {
                    case "o":
                        mfc_version = "Microsoft Foundation Classes (MFC) for Active Technologies, version: " + version;
                        break;
                    case "d":
                        mfc_version = "Microsoft Foundation Classes (MFC) for database, version: " + version;
                        break;
                    case "n":
                        mfc_version = "Microsoft Foundation Classes (MFC) for network (sockets), version: " + version;
                        break;
                    case "s":
                        mfc_version = "Microsoft Foundation Classes (MFC) statically linked code, version: " + version;
                        break;
                    default:
                        mfc_version = "Microsoft Foundation Classes (MFC) standard, version: " + version;
                        break;
                }

                switch (s_debug.ToLower())
                {
                    case "d":
                        mfc_version += " ANSI Debug";
                        break;
                    case "ud":
                        mfc_version += " Unicode Debug";
                        break;
                    case "u":
                        mfc_version += " Unicode Release";
                        break;
                    default:
                        mfc_version += " ANSI Release";
                        break;
                }

                results.AppendResult("MFC", mfc_version, 1, 0);
            }

            Regex r_MSVCRT_dll = new Regex("^MSVCPRT(|D).DLL$", RegexOptions.IgnoreCase);
            aMatch = r_MSVCRT_dll.Match(theString);
            if (aMatch.Success)
            {
                results.AppendResult("Compiler", "Microsoft Visual C++ 4.2", 1, 0);
            }

            Regex r_MSVCRT0_dll = new Regex("^MSVCRT(?<debug>(|D)).DLL$", RegexOptions.IgnoreCase);
            aMatch = r_MSVCRT0_dll.Match(theString);
            if (aMatch.Success)
            {
                string visual_c_version = string.Empty;

                // only mark this version if it's not set already (default out to this if MSVCPxx not detected below, in other words)
                if (visual_c_version == "")
                {
                    visual_c_version = "Microsoft Visual C++ 4.2";

                    string s_debug = aMatch.Groups["debug"].ToString();
                    switch (s_debug.ToLower())
                    {
                        case "d":
                            visual_c_version += " debug";
                            break;
                    }

                    results.AppendResult("Compiler", visual_c_version, 1, 0);
                }
            }

            Regex r_MSVCRT2_dll = new Regex("^MSVC(P|R)(?<version>[0-9]+(|D)).DLL$", RegexOptions.IgnoreCase);
            aMatch = r_MSVCRT2_dll.Match(theString);
            if (aMatch.Success)
            {
                string visual_c_version = string.Empty;

                string version = aMatch.Groups["version"].ToString();
                switch (version.ToLower())
                {
                    case "60d":
                        visual_c_version = "Microsoft Visual C++ 6.0 debug";
                        break;
                    case "60":
                        visual_c_version = "Microsoft Visual C++ 6.0 release";
                        break;
                    case "50d":
                        visual_c_version = "Microsoft Visual C++ 5.0 debug";
                        break;
                    case "50":
                        visual_c_version = "Microsoft Visual C++ 5.0 release";
                        break;
                    case "70d":
                        visual_c_version = "Microsoft Visual C++ 2002 debug";
                        break;
                    case "70":
                        visual_c_version = "Microsoft Visual C++ 2002 release";
                        break;
                    case "71d":
                        visual_c_version = "Microsoft Visual C++ 2003 debug";
                        break;
                    case "71":
                        visual_c_version = "Microsoft Visual C++ 2003 release";
                        break;
                    case "80d":
                        visual_c_version = "Microsoft Visual C++ 2005 debug";
                        break;
                    case "80":
                        visual_c_version = "Microsoft Visual C++ 2005 release";
                        break;
                    case "90d":
                        visual_c_version = "Microsoft Visual C++ 2008 debug";
                        break;
                    case "90":
                        visual_c_version = "Microsoft Visual C++ 2008 release";
                        break;
                }

                results.AppendResult("Compiler", visual_c_version, 1, 0);

            }

            return true;
        }

        override public bool OnRegisterPatterns(PatternList theList)
        {
            // 0041140F   8B 4D FC                          mov ecx,dword ptr [ebp-0x4]
            // 00411412   33 CD                             xor ecx,ebp
            // 00411414   E8 05 FC FF FF                    call 0x0041101E▲ // sub_0041101E
            AddPattern(theList, "Buffer Security Checks", "8B 4D FC 33 CD E8", 1, 0, null);

            // 0040105A   C7 44 24 08 00 00 00 00           mov dword ptr [esp+0x8],0x0
            AddPattern(theList, "FPO count", "C7 44 24 ?? 00 00 00 00", 1, 0, null);

            return true;
        }
    }
}
