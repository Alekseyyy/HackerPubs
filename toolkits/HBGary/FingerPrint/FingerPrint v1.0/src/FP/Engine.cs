//------------------------------------------------------------------------
//
// Copyright © 2010 HBGary, Inc.  All Rights Reserved. 
//
//------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace FP
{
    static class Engine
    {
        static private ulong m_string_min_length = 4;


        //
        // TODO: Improve this code, it is not efficient
        //
        static public bool Scan(string file_path, ScanResultCollection results)
        {
            // Reset for another scan
            FingerPrintManager.ResetFingerPrints();

            Dictionary<string, bool> stringTable = new Dictionary<string, bool>();

            try
            {
                ArrayList string_list = new ArrayList();

                BinaryReader br = new BinaryReader(File.OpenRead(file_path));

                ulong length = (ulong)br.BaseStream.Length;

                //
                // Arbitrary length limit: 16 MB
                //
                if (length > 0x01000000)
                {
                    Console.WriteLine("Skipping {0}, file is too large", file_path);
                    return false;
                }

                FingerPrintManager.OnScan(file_path, br, results);

                //
                // Start from the beginning
                //
                ulong pos = 0;

                bool inString = false;

                UInt64 lastStringBegin = 0;
                string lastString = "";

                try
                {
                    while (pos < length)
                    {
                        byte aByte = br.ReadByte();

                        // acceptable ascii character?
                        if ((aByte >= 0x20 && aByte <= 0x7e) || 
                            aByte == 0x0a || 
                            aByte == 0x0d || 
                            aByte == 0x09 )
                        {
                            if (false == inString)
                            {
                                // Start tracking a new string
                                inString = true;
                                lastStringBegin = pos;
                                lastString = ((char)aByte).ToString();
                            }
                            else
                            {
                                // append to existing string
                                lastString += ((char)aByte).ToString();
                            }
                        }
                        else
                        {
                            // not an acceptable character - terminate any string
                            if (inString)
                            {
                                // report as string only if long enough
                                ulong lastStringEnd = pos;

                                if ((pos - lastStringBegin) > m_string_min_length)
                                {
                                    UInt64 len = lastStringEnd - lastStringBegin;

                                    // Add this string and all token pieces of it to the stringTable
                                    string[] strings = lastString.ToLower().Split(new char[] { ' ', '\n', '\r', '\t' } );
                                    foreach (string aString in strings)
                                    {
                                        stringTable[aString] = true;
                                    }
                                    stringTable[lastString] = true;

                                    FingerPrintManager.OnEvaluateString(lastString, results);
                                }
                            }

                            inString = false;
                        }

                        pos++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed scanning ascii {0}: {1}", file_path, ex.Message);
                }

                // TODO: Combine these into a single scan for efficiency

                // Reset the stream to the beginning
                br.BaseStream.Seek(0, SeekOrigin.Begin);

                try
                {
                    // now wide strings
                    pos = 0;
                    bool expecting_terminator = false;
                    bool expecting_zero = false;
                    byte last_byte = 0;
                    lastString = "";
                    
                    while (pos < length)
                    {
                        byte aByte = br.ReadByte();

                        // Is the next character supposed to be a zero
                        if (expecting_zero == true)
                        {
                            // This is a wide character completion
                            if (aByte == 0)
                            {
                                // expecting string terminator?
                                if (true == expecting_terminator)
                                {
                                    // its a terminated wide string
                                    if ((ulong)lastString.Length >= m_string_min_length)
                                    {
                                        string[] strings = lastString.ToLower().Split(new char[] { ' ', '\n', '\r', '\t' });

                                        foreach (string aString in strings)
                                        {
                                            stringTable[aString] = true;
                                        }

                                        stringTable[lastString] = true;

                                        FingerPrintManager.OnEvaluateString(lastString, results);
                                    }

                                    lastString = "";
                                }
                                else
                                {
                                    // Add the previous character to our string
                                    lastString += ((char)last_byte).ToString();
                                }
                            }
                            else
                            {
                                lastString = "";
                            }

                            expecting_zero = false;
                            expecting_terminator = false;
                        }
                        else  // not expecting zero
                        {
                            // acceptable ascii character?
                            if ((aByte >= 0x20 && aByte <= 0x7e) ||
                                aByte == 0x0a ||
                                aByte == 0x0d ||
                                aByte == 0x09)
                            {
                                last_byte = aByte;
                                expecting_zero = true;
                                expecting_terminator = false;
                            }
                            else if (aByte == 0)
                            {
                                // got a zero when not expecting it, potential string termination
                                expecting_terminator = true;
                                expecting_zero = true;
                            }
                            else
                            {
                                lastString="";
                                expecting_zero = false;
                                expecting_terminator = false;
                            }
                        }

                        pos++;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed scanning wide character {0}: {1}", file_path, ex.Message);
                }

                //
                // Pass the entire string set to any fingerprints
                //
                FingerPrintManager.OnEvaluateStringSet(stringTable, results);
                    
                //
                // Scan for patterns
                //
                try
                {
                    PatternList patterns = FingerPrintManager.GetPatterns();

                    // now binary pattern matches
                    long bytepos = 0;
                    br.BaseStream.Seek(0, SeekOrigin.Begin);

                    while (bytepos < br.BaseStream.Length)
                    {
                        byte aByte = br.ReadByte();
                        bytepos = br.BaseStream.Position;

                        foreach (Pattern aPattern in patterns.Patterns)
                        {

                            if ((aPattern.BytePatternLength > 0) &&
                                (aByte == aPattern.ByteZero))
                            {
                                long savedPos = br.BaseStream.Position;

                                if (savedPos + aPattern.BytePatternLength < br.BaseStream.Length)
                                {
                                    byte[] theBytes = new byte[aPattern.BytePatternLength];
                                    theBytes[0] = aByte;

                                    br.Read(theBytes, 1, aPattern.BytePatternLength - 1);

                                    if ( CompareBytePattern (aPattern.BytePattern, theBytes))
                                    {
                                        FingerPrintManager.OnPatternMatch(aPattern, results);
                                    }

                                    br.BaseStream.Position = savedPos;
                                }
                            }                                
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed byte scanning {0}: {1}", file_path, ex.Message);
                }

                br.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Scan failed on {0}:{1}", file_path, ex.Message);
            }

            FingerPrintManager.OnScanComplete(file_path, results);

            return true;
        }

        // Accepted format for a bytepattern:
        // aa bb 01 02 ?? 03 04
        static bool CompareBytePattern(string BytePattern, byte[] bytes)
        {
            int i = 0;

            // poor man's byte matching with wildcards
            string[] tokens = BytePattern.Split(' ');
            foreach (string token in tokens)
            {
                // length check
                if (i >= bytes.Length) { return false; }

                // wildcard
                if (token == "??") { i++; continue; }

                // convert to a byte
                byte theByte;

                if (System.Byte.TryParse(token, System.Globalization.NumberStyles.HexNumber, null, out theByte))
                {
                    if (theByte != bytes[i])
                    {
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Failed to parse byte pattern at {0}", token);
                }

                i++;
            }

            return true;
        }
    }
}
