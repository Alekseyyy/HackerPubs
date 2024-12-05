using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FP
{
    class Program
    {
        static void Usage()
        {
            Console.WriteLine("[*] HELP [*]\n");
            Console.WriteLine("    Usage: fingerprint.exe <file_path>\n");
            Console.WriteLine("    or:    fingerprint.exe -c <file_path_1> <file_path_2>\n");
            return;
        }

        static void Main(string[] args)
        {
            // Display banner
            Console.WriteLine("Software Fingerprint Utility v0.1, Copyright © 2010 HBGary, Inc.");

            if ((args.Length == 0) ||
                (args[0].Equals("/?") == true) ||
                (args[0].Equals("help") == true) ||
                (args[0].StartsWith("-?") == true) ||
                (args[0].ToLower().StartsWith("-h") == true))
            {
                Usage();
                return;
            }

            // For testing purposes only
            string fingerPrintDir = Path.Combine(System.Environment.CurrentDirectory, @"..\..\..\FingerPrints");
            if (false == Directory.Exists(fingerPrintDir))
            {
                fingerPrintDir = Path.Combine(System.Environment.CurrentDirectory, @"FingerPrints");
                if (false == Directory.Exists(fingerPrintDir))
                {
                    fingerPrintDir = System.Environment.CurrentDirectory;
                }
            }

            FingerPrintManager.LoadFingerPrints(fingerPrintDir);
            
            // single file or directory specified
            // or -r (recrusive) and a directory
            if ((args.Length == 1) ||
                ((args.Length == 2) && (args[0] == "-r")))
            {
                List<string> filesToScan = new List<string>();
                List<string> directoriesToScan = new List<string>();

                string thePath = args[0];
                bool bRecurse = true;

                if (args[0] == "-r")
                {
                    thePath = args[1];
                    bRecurse = true;
                }

                directoriesToScan.Add(thePath);

                // build the complete file list
                while (directoriesToScan.Count > 0)
                {
                    string curDir = directoriesToScan[0];
                    directoriesToScan.RemoveAt(0);

                    if (Directory.Exists(curDir))
                    {
                        if ( bRecurse )
                        {
                            try
                            {
                                string[] dirs = Directory.GetDirectories(curDir);
                                foreach (string aDir in dirs)
                                {
                                    if ((aDir != ".") && (aDir != ".."))
                                    {
                                        directoriesToScan.Add(aDir);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Failed to recurse into {0}: {1}", curDir, ex.Message);
                            }
                        }

                        try
                        {
                            string[] files = Directory.GetFiles(curDir);
                            foreach (string aFile in files)
                            {
                                if ((aFile != ".") && (aFile != ".."))
                                {
                                    filesToScan.Add(aFile);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Failed to load directory {0}: {1}", curDir, ex.Message);
                        }
                    }
                    else
                    {
                        filesToScan.Add(args[0]);
                    }
                }

                ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                int count = 0;

                Console.WriteLine("Scanning {0} file(s)...", filesToScan.Count);

                foreach (string file in filesToScan)
                {
                    ScanResultCollection results = new ScanResultCollection(file);

                    if (Engine.Scan(file, results))
                    {
                        Report.PrintFormatted(results);

                        theList.AddResultCollection(results);
                    }

                    count++;
                    // Serialize occasionally
                    if (count > 50)
                    {
                        ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
                    }
                }

                ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
                
            }
            // Compare two files
            else if ((args[0] == "-c") && (args.Length == 3))
            {
                ScanResultCollection resultsA = new ScanResultCollection(args[1]);

                if ( Engine.Scan(args[1], resultsA) )
                {
                    ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                    theList.AddResultCollection(resultsA);

                    ScanResultCollection resultsB = new ScanResultCollection(args[2]);
                    if (Engine.Scan(args[2], resultsB))
                    {
                        Report.PrintComparison(resultsA, resultsB);
                        theList.AddResultCollection(resultsB);
                    }

                    ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
                }
            }
            else if ((args[0] == "-batch") && (args.Length == 2))
            {
                // TODO: process files in batch using a list
            }
            // compare a file to the database, showing likely matches (-db) or showing all (-dball)
            else if (((args[0] == "-db") || (args[0] == "-dball")) && (args.Length == 2))
            {
                bool bOnlyNearMatches = true;

                if (args[0] == "-dball")
                {
                    bOnlyNearMatches = false;
                }
                // Compare to historical log of binaries
                ScanResultCollection resultsA = new ScanResultCollection(args[1]);
                if (Engine.Scan(args[1], resultsA))
                {
                    ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                    foreach (ScanResultCollection resultsB in theList.Items)
                    {
                        Report.PrintComparison(resultsA, resultsB, bOnlyNearMatches);
                    }

                    theList.AddResultCollection(resultsA);

                    ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
                }
            }
            else
            {
                Usage();
            }
        }
    }
}
