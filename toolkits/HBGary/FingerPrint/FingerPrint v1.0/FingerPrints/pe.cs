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
using System.Runtime.InteropServices;

namespace PEParser
{
    public class PEParser
    {
        #region " Structures "

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_DOS_HEADER
        {
            public UInt16 e_magic;
            public UInt16 e_cblp;
            public UInt16 e_cp;
            public UInt16 e_crc;
            public UInt16 e_cparhdr;
            public UInt16 e_minalloc;
            public UInt16 e_maxalloc;
            public UInt16 e_ss;
            public UInt16 e_sp;
            public UInt16 e_cksum;
            public UInt16 e_ip;
            public UInt16 e_cs;
            public UInt16 e_lfar_c;
            public UInt16 e_ov_no;
            public UInt16 e_reserved_0;
            public UInt16 e_reserved_1;
            public UInt16 e_reserved_2;
            public UInt16 e_reserved_3;
            public UInt16 e_oem_id;
            public UInt16 e_oem_info;
            public UInt16 e_reserved_4;
            public UInt16 e_reserved_5;
            public UInt16 e_reserved_6;
            public UInt16 e_reserved_7;
            public UInt16 e_reserved_8;
            public UInt16 e_reserved_9;
            public UInt16 e_reserved_10;
            public UInt16 e_reserved_11;
            public UInt16 e_reserved_12;
            public UInt16 e_reserved_13;
            public UInt32 e_lfanew;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER32
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt32 BaseOfData;
            public UInt32 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt32 SizeOfStackReserve;
            public UInt32 SizeOfStackCommit;
            public UInt32 SizeOfHeapReserve;
            public UInt32 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_OPTIONAL_HEADER64
        {
            public UInt16 Magic;
            public Byte MajorLinkerVersion;
            public Byte MinorLinkerVersion;
            public UInt32 SizeOfCode;
            public UInt32 SizeOfInitializedData;
            public UInt32 SizeOfUninitializedData;
            public UInt32 AddressOfEntryPoint;
            public UInt32 BaseOfCode;
            public UInt64 ImageBase;
            public UInt32 SectionAlignment;
            public UInt32 FileAlignment;
            public UInt16 MajorOperatingSystemVersion;
            public UInt16 MinorOperatingSystemVersion;
            public UInt16 MajorImageVersion;
            public UInt16 MinorImageVersion;
            public UInt16 MajorSubsystemVersion;
            public UInt16 MinorSubsystemVersion;
            public UInt32 Win32VersionValue;
            public UInt32 SizeOfImage;
            public UInt32 SizeOfHeaders;
            public UInt32 CheckSum;
            public UInt16 Subsystem;
            public UInt16 DllCharacteristics;
            public UInt64 SizeOfStackReserve;
            public UInt64 SizeOfStackCommit;
            public UInt64 SizeOfHeapReserve;
            public UInt64 SizeOfHeapCommit;
            public UInt32 LoaderFlags;
            public UInt32 NumberOfRvaAndSizes;
        }

        UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IMAGE_FILE_HEADER
        {
            public UInt16 Machine;
            public UInt16 NumberOfSections;
            public UInt32 TimeDateStamp;
            public UInt32 PointerToSymbolTable;
            public UInt32 NumberOfSymbols;
            public UInt16 SizeOfOptionalHeader;
            public UInt16 Characteristics;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 40)]
        public struct IMAGE_SECTION_HEADER
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public Byte[] Name;
            public UInt32 Misc_PA_or_VS;
            public UInt32 VirtualAddress;
            public UInt32 SizeOfRawData;
            public UInt32 PointerToRawData;
            public UInt32 PointerToRelocations;
            public UInt32 PointerToLineNumbers;
            public UInt16 NumberOfRelocations;
            public UInt16 NumberofLineNumbers;
            public UInt32 Characteristics;
        }

        #endregion

        private IMAGE_DOS_HEADER dosHeader;
        private IMAGE_FILE_HEADER ntHeader;

        private IMAGE_SECTION_HEADER[] sectionHeaders = new IMAGE_SECTION_HEADER[16];
        private int sectionCount = 0;

        private UInt32 ntHeadersSignature;
        private bool is32bit;

        private IMAGE_OPTIONAL_HEADER32 optionalHeader32;
        private IMAGE_OPTIONAL_HEADER64 optionalHeader64;

        public PEParser(BinaryReader r)
        {
            dosHeader = BinaryReaderTemplate<IMAGE_DOS_HEADER>(r);

            if (dosHeader.e_lfanew < r.BaseStream.Length - 0x100)
            {
                // Add 4 bytes to the offset
                r.BaseStream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);

                ntHeadersSignature = r.ReadUInt32();
                ntHeader = BinaryReaderTemplate<IMAGE_FILE_HEADER>(r);

                long endOfFileHeader = r.BaseStream.Position;
                long startOfSections = endOfFileHeader + ntHeader.SizeOfOptionalHeader;

                if ((IMAGE_FILE_32BIT_MACHINE & NtHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE)
                {
                    optionalHeader32 = BinaryReaderTemplate<IMAGE_OPTIONAL_HEADER32>(r);
                    is32bit = true;
                }
                else
                {
                    optionalHeader64 = BinaryReaderTemplate<IMAGE_OPTIONAL_HEADER64>(r);
                    is32bit = false;
                }

                r.BaseStream.Seek(startOfSections, SeekOrigin.Begin);

                try
                {

                    for (int i = 0; i < ntHeader.NumberOfSections && i < 16; i++)
                    {
                        sectionHeaders[i] = BinaryReaderTemplate<IMAGE_SECTION_HEADER>(r);
                        sectionCount = i;
                    }
                }
                catch
                {
                }
            }
        }

        public PEParser(string filePath)
        {
            // Read in the DLL or EXE and get the timestamp
            using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                BinaryReader r = new BinaryReader(stream);
                dosHeader = BinaryReaderTemplate<IMAGE_DOS_HEADER>(r);

                if (dosHeader.e_lfanew < r.BaseStream.Length - 0x100)
                {
                    // Add 4 bytes to the offset
                    stream.Seek(dosHeader.e_lfanew, SeekOrigin.Begin);

                    ntHeadersSignature = r.ReadUInt32();
                    ntHeader = BinaryReaderTemplate<IMAGE_FILE_HEADER>(r);

                    long endOfFileHeader = r.BaseStream.Position;
                    long startOfSections = endOfFileHeader + ntHeader.SizeOfOptionalHeader;

                    if ((IMAGE_FILE_32BIT_MACHINE & NtHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE)
                    {
                        optionalHeader32 = BinaryReaderTemplate<IMAGE_OPTIONAL_HEADER32>(r);
                        is32bit = true;
                    }
                    else
                    {
                        optionalHeader64 = BinaryReaderTemplate<IMAGE_OPTIONAL_HEADER64>(r);
                        is32bit = false;
                    }

                    r.BaseStream.Seek(startOfSections, SeekOrigin.Begin);

                    try
                    {
                        for (int i = 0; i < ntHeader.NumberOfSections && i < 16; i++)
                        {
                            sectionHeaders[i] = BinaryReaderTemplate<IMAGE_SECTION_HEADER>(r);
                            sectionCount = i;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static T BinaryReaderTemplate<T>(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(Marshal.SizeOf(typeof(T)));

            GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            handle.Free();

            return theStructure;
        }

        public bool IsValid
        {
            get
            {
                return ntHeadersSignature == 0x00004550;
            }
        }

        public IMAGE_FILE_HEADER NtHeader
        {
            get
            {
                return ntHeader;
            }
        }

        public IMAGE_OPTIONAL_HEADER32 OptionalHeader32
        {
            get
            {
                return optionalHeader32;
            }
        }

        public IMAGE_OPTIONAL_HEADER64 OptionalHeader64
        {
            get
            {
                return optionalHeader64;
            }
        }

        private DateTime AdjustUTCTimeZone (DateTime dt)
        {
            return dt + TimeZone.CurrentTimeZone.GetUtcOffset(dt);
        }

        private DateTime DateTimeFromUTC (uint dtstamp)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0);
            dt = dt.AddSeconds(ntHeader.TimeDateStamp);
            return AdjustUTCTimeZone(dt);
        }

        public DateTime TimeStamp
        {
            get
            {
                return DateTimeFromUTC(ntHeader.TimeDateStamp);
            }
        }

        public byte LinkerMajorVersion
        {
            get
            {
                if (is32bit)
                {
                    return optionalHeader32.MajorLinkerVersion;
                }
                else
                {
                    return optionalHeader64.MajorLinkerVersion;
                }
            }
        }

        public byte LinkerMinorVersion
        {
            get
            {
                if (is32bit)
                {
                    return optionalHeader32.MinorLinkerVersion;
                }
                else
                {
                    return optionalHeader64.MinorLinkerVersion;
                }
            }
        }

        public ushort DllCharacteristics
        {
            get
            {
                if (is32bit)
                {
                    return optionalHeader32.DllCharacteristics;
                }
                else
                {
                    return optionalHeader64.DllCharacteristics;
                }
            }
        }

        public uint LoaderFlags
        {
            get
            {
                if (is32bit)
                {
                    return optionalHeader32.LoaderFlags;
                }
                else
                {
                    return optionalHeader64.LoaderFlags;
                }
            }
        }

        private System.Text.ASCIIEncoding _enc = new ASCIIEncoding();
        public string SectionNames
        {
            get
            {
                string allNames = string.Empty;

                for (int i = 0; i < sectionCount; i++)
                {
                    // Has to be a more efficient way to do this, but no time to find out now
                    if (i != 0)
                        allNames += " | ";

                    int x = 0;
                    for (; x < 8; x++)
                    {
                        if (sectionHeaders[i].Name[x] == 0)
                            break;
                    }

                    Byte[] theName = new Byte[x];

                    int j = 0;
                    for (; j < x; j++)
                    {
                        theName[j] = sectionHeaders[i].Name[j];
                    }

                    allNames += _enc.GetString(theName);
                }

                return allNames;
            }
        }
    }
}


namespace FP
{
    class pe : BaseFingerPrint
    {
        override public bool OnScan(string theString, BinaryReader br, ScanResultCollection results)
        {
            try
            {
                PEParser.PEParser reader = new PEParser.PEParser(br);
                if (reader.IsValid)
                {
                    PEParser.PEParser.IMAGE_OPTIONAL_HEADER32 header32 = reader.OptionalHeader32;
                    DateTime linkerTime = reader.TimeStamp;

                    // TODO: review weight
                    results.AddResult("PE Timestamp", linkerTime.ToString(), 2, 0);

                    results.AddResult("Linker version", string.Format("v{0}.{1}", reader.LinkerMajorVersion, reader.LinkerMinorVersion), 1, 0);

                    results.AddResult("DllCharacteristics", reader.DllCharacteristics.ToString("X8"), 1, 0);

                    results.SetResult("PE Sections", reader.SectionNames, 1, 0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to parse PE header for {0}: {1}", theString, ex.Message);
            }

            return true;
        }

        override public bool OnRegisterPatterns(PatternList theList)
        {
            // looking for multiple PE headers
            AddPattern(theList, "PE Headers", "00 00 50 45 00 00", 1, 0, null);

            return true;
        }
    }
}
