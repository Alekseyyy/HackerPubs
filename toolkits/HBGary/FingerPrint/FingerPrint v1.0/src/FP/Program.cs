//------------------------------------------------------------------------
//
// Copyright © 2010 HBGary, Inc.  All Rights Reserved. 
//
//------------------------------------------------------------------------

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
            Console.WriteLine("Usage: \r\n");
            Console.WriteLine("fp.exe <file_path>");
            Console.WriteLine("    Examine a single file for fingerprints");
            Console.WriteLine("fp.exe <directory>");
            Console.WriteLine("    Examine all files in a directory for fingerprints");
            Console.WriteLine("fp.exe -r <directory>");
            Console.WriteLine("    Examine all files in a directory for fingerprints, recursively descend into sub directories");
            Console.WriteLine("fp.exe -c <file_path>");
            Console.WriteLine("    Compare the specified file to all files in the scan history");
            Console.WriteLine("fp.exe -c <directory>");
            Console.WriteLine("    Compare all files in a directory to all files in the scan history, showing summary results");
            Console.WriteLine("fp.exe -c <file a> <file b>");
            Console.WriteLine("    Compare file a to file b");

            Console.WriteLine("fp.exe -db <directory>");
            Console.WriteLine("    Compare a file to all files in the scan history, showing full results for all matches > 80%");

            Console.WriteLine("fp.exe -dball <directory>");
            Console.WriteLine("    Compare a file to all files in the scan history, showing full results for all matches");
            return;
        }

        delegate void BreakHandlerDelegate();

        static void Main(string[] args)
        {
            //
            // Display banner
            //
            Console.WriteLine("Fingerprint v1.0, Copyright © 2010 HBGary, Inc. All Rights Reserved.");

            //
            // Handle help/usage requests
            //
            if ((args.Length == 0) ||
                (args[0].Equals("/?") == true) ||
                (args[0].ToLower().Equals("help") == true) ||
                (args[0].StartsWith("-?") == true) ||
                (args[0].ToLower().StartsWith("-h") == true))
            {
                Usage();
                return;
            }

            //
            // Check for a fingerprints directory three levels up, to support running from the default build location
            //
            string fingerPrintDir = Path.Combine(System.Environment.CurrentDirectory, @"..\..\..\FingerPrints");
            if (false == Directory.Exists(fingerPrintDir))
            {
                // Check for a fingerprints directory under the current directory
                fingerPrintDir = Path.Combine(System.Environment.CurrentDirectory, @"FingerPrints");
                if (false == Directory.Exists(fingerPrintDir))
                {
                    // default to the current directory
                    fingerPrintDir = System.Environment.CurrentDirectory;
                }
            }

            //
            // Load and compile any .cs files in the fingerprints directory
            //
            FingerPrintManager.LoadFingerPrints(fingerPrintDir);
            
            //
            // cheap argument parsing
            // TODO: Upgrade
            //
            if ((args.Length == 1) ||
                ((args.Length == 2) && (args[0] == "-r")) ||
                ((args.Length == 2) && (args[0] == "-c")))
            {
                List<string> filesToScan = new List<string>();
                List<string> directoriesToScan = new List<string>();

                string thePath = args[0];

                bool bRecurse = false;
                bool bCompare = false;

                if (args[0] == "-r")
                {
                    thePath = args[1];
                    bRecurse = true;
                }

                if (args[0] == "-c")
                {
                    thePath = args[1];
                    bCompare = true;
                }

                directoriesToScan.Add(thePath);

                //
                // build the complete file list to examine
                //
                while (directoriesToScan.Count > 0)
                {
                    string curDir = directoriesToScan[0];
                    directoriesToScan.RemoveAt(0);

                    if (Directory.Exists(curDir))
                    {
                        if (bRecurse)
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
                                // handle errors, usually related to permissions
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
                        filesToScan.Add(curDir);
                    }
                }

                //
                // Read in the existing scan history
                //
                ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");                

                //
                // Set the break handler so we can flush output to disk before closing
                //
                BreakHandlerDelegate BreakHandler = delegate { ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList); };
                Console.CancelKeyPress += delegate { BreakHandler(); };

                Console.WriteLine("Scanning {0} file(s)...", filesToScan.Count);

                //
                // Walk all the files
                //
                int count = 0;
                foreach (string file in filesToScan)
                {
                    Console.WriteLine("\r\n{0}/{1}", count.ToString(), filesToScan.Count.ToString());

                    //
                    // Create a new scan result collection for each file
                    //
                    ScanResultCollection results = new ScanResultCollection(file);

                    // Scan
                    if (Engine.Scan(file, results))
                    {
                        if (bCompare)
                        {
                            foreach (ScanResultCollection resultsB in theList.Items)
                            {
                                // Print a summary report
                                Report.PrintComparison(results, resultsB, true, false);
                            }
                        }
                        else
                        {
                            // Print a full report
                            Report.PrintFormatted(results);
                        }

                        // Save the results to the scan history list
                        theList.AddResultCollection(results);
                    }

                    count++;
                }

                //
                // Save the scan history
                //
                ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
            }
            else if ((args.Length == 3) && (args[0] == "-c"))
            {
                //
                // Compare two files
                //

                ScanResultCollection resultsA = new ScanResultCollection(args[1]);

                // Scan the first file
                if ( Engine.Scan(args[1], resultsA) )
                {
                    ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                    theList.AddResultCollection(resultsA);

                    ScanResultCollection resultsB = new ScanResultCollection(args[2]);

                    // Scan the second file
                    if (Engine.Scan(args[2], resultsB))
                    {
                        // Print comparison
                        Report.PrintComparison(resultsA, resultsB);

                        theList.AddResultCollection(resultsB);
                    }

                    ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
                }
            }
            else if (((args[0] == "-db") || (args[0] == "-dball")) || (args[0] == "-dbpercent") && (args.Length == 2))
            {
                //
                // compare a file to the database, showing likely matches (-db) or showing all (-dball)
                //
                bool bOnlyNearMatches = true;
                bool bFullReport = true;

                if (args[0] == "-dball")
                {
                    // Only display > 80% matches
                    bOnlyNearMatches = false;
                }

                if (args[0] == "-dbpercent")
                {
                    // Only display the match percentage, not the side-by-side comparison
                    bFullReport = false;
                }

                // Compare to historical log of binaries
                ScanResultCollection resultsA = new ScanResultCollection(args[1]);

                if (Engine.Scan(args[1], resultsA))
                {
                    ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                    foreach (ScanResultCollection resultsB in theList.Items)
                    {
                        Report.PrintComparison(resultsA, resultsB, bOnlyNearMatches, bFullReport);
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
