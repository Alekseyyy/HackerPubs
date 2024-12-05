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
            
            if (args.Length == 1)
            {
                List<string> filesToScan = new List<string>();

                if (Directory.Exists(args[0]))
                {
                    string[] files = Directory.GetFiles(args[0]);
                    foreach (string aFile in files)
                    {
                        if ((aFile != ".") && (aFile != ".."))
                        {
                            filesToScan.Add(aFile);
                        }
                    }
                }
                else
                {
                    filesToScan.Add(args[0]);
                }

                ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                foreach (string file in filesToScan)
                {
                    ScanResultCollection results = new ScanResultCollection(file);

                    Engine.Scan(file, results);

                    Report.PrintFormatted(results);

                    theList.AddResultCollection(results);
                }

                ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
            }
            else if ((args[0] == "-c") && (args.Length == 3))
            {
                ScanResultCollection resultsA = new ScanResultCollection(args[1]);
                Engine.Scan(args[1], resultsA);

                ScanResultCollection resultsB = new ScanResultCollection(args[2]);
                Engine.Scan(args[2], resultsB);

                Report.PrintComparison(resultsA, resultsB);

                ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                theList.AddResultCollection(resultsA);
                theList.AddResultCollection(resultsB);

                ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);

            }
            else if ((args[0] == "-batch") && (args.Length == 2))
            {
                // TODO: process files in batch using a list
            }
            else if (((args[0] == "-db") || (args[0] == "-dball")) && (args.Length == 2))
            {
                bool bOnlyNearMatches = true;

                if (args[0] == "-dball")
                {
                    bOnlyNearMatches = false;
                }
                // Compare to historical log of binaries
                ScanResultCollection resultsA = new ScanResultCollection(args[1]);
                Engine.Scan(args[1], resultsA);

                ScanResultCollectionList theList = ScanResultCollectionList.SerializeFromXML(@"scan_history.xml");

                foreach (ScanResultCollection resultsB in theList.Items)
                {
                    Report.PrintComparison(resultsA, resultsB, bOnlyNearMatches);
                }

                theList.AddResultCollection(resultsA);

                ScanResultCollectionList.SerializeToXML(@"scan_history.xml", theList);
            }
            else
            {
                Usage();
            }
        }
    }
}
