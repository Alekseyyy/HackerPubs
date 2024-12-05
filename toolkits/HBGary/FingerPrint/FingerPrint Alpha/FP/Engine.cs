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


        // TODO: Clean up this code
        static public bool Scan(string file_path, ScanResultCollection results)
        {
            // Reset for another scan
            FingerPrintManager.ResetFingerPrints();

            try
            {
                ArrayList string_list = new ArrayList();
                BinaryReader br = new BinaryReader(File.OpenRead(file_path));

                FingerPrintManager.OnScan(file_path, br, results);

                // Reset binary reader to the beginning
                br.BaseStream.Seek(0, SeekOrigin.Begin);

                ulong length = (ulong)br.BaseStream.Length;
                ulong pos = 0;

                bool inString = false;
                UInt64 lastStringBegin = 0;
                string lastString = "";

                try
                {
                    while (pos < length)
                    {
                        uint intchar = (uint)br.ReadByte();
                        if (intchar >= 0x20 && intchar <= 0x7e || (intchar == 0x0a || intchar == 0x0d || intchar == 0x09))
                        {
                            // valid character ...

                            if (false == inString)
                            {
                                inString = true;
                                lastString = "";
                                lastStringBegin = pos;
                                lastString += (char)intchar;
                                //Console.WriteLine("\t --- Starting new string at offset " + lastStringBegin.ToString());
                            }
                            else
                            {
                                // in string, add character...
                                lastString += (char)intchar;
                            }
                        }
                        else
                        {
                            // not a valid character - terminate any string
                            if (inString)
                            {
                                // report as string only if long enough
                                ulong lastStringEnd = pos;
                                if ((pos - lastStringBegin) > m_string_min_length)
                                {
                                    UInt64 len = lastStringEnd - lastStringBegin;
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
                }

                try
                {
                    // now UNICODE
                    pos = 0;
                    bool next_is_zero = false;
                    byte last_byte = 0;
                    lastString = "";
                    br.BaseStream.Seek(0, SeekOrigin.Begin);
                    while (pos < length)
                    {
                        uint intchar = (uint)br.ReadByte();

                        // Is the next character supposed to be a zero
                        if (next_is_zero == true)
                        {
                            // This is a unicode character completion
                            if (intchar == 0)
                            {
                                // Add the previous character to our string
                                lastString += ((char)(last_byte)).ToString();
                            }
                            else
                            {
                                lastString = "";
                            }

                            next_is_zero = false;
                        }
                        else
                        {
                            // Is this a byte aligned unicode null termination? do we have the proper min length of characters? make a string!

                            // poor man's peek
                            try
                            {
                                uint nextchar = (uint)br.ReadByte();
                                br.BaseStream.Seek(-1, SeekOrigin.Current);

                                if ((intchar == 0) && (nextchar == 0))
                                {
                                    // its a terminated unicode string
                                    if ((ulong)lastString.Length >= m_string_min_length)
                                    {
                                        FingerPrintManager.OnEvaluateString(lastString, results);
                                    }
                                    lastString = "";
                                    // Skip the next character also, we know it's a zero for the unicode null termination
                                    pos++;
                                    br.ReadByte();
                                }
                                else
                                {
                                    if (intchar >= 0x20 && intchar <= 0x7e || (intchar == 0x0a || intchar == 0x0d || intchar == 0x09))
                                    {
                                        // valid character
                                        if (next_is_zero == false)
                                            next_is_zero = true;

                                        last_byte = (byte)intchar;
                                    }
                                    else
                                    {
                                        // not a valid character, skip this as a string
                                        lastString = "";
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }

                        pos++;
                    }
                }
                catch (Exception ex)
                {
                }

                try
                {
                    PatternList patterns = FingerPrintManager.GetPatterns();

                    // now binary pattern matches
                    pos = 0;
                    br.BaseStream.Seek(0, SeekOrigin.Begin);

                    while (pos < length)
                    {
                        byte aByte = br.ReadByte();

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
                }

                br.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
