using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FP
{
    static class Report
    {
        static public void PrintFormatted(ScanResultCollection results)
        {
            Console.WriteLine("");
            Console.WriteLine("Name: {0}", results.Name);
            Console.WriteLine("Hash: {0}", results.Hash);

            foreach (ScanResult result in results.GetResultList())
            {
                Console.WriteLine(
                    String.Format("{0,-30} {1,-48}",
                        result.Name,
                        result.Value));
            }
        }

        static public void PrintComparison(ScanResultCollection resultsA, ScanResultCollection resultsB)
        {
            PrintComparison(resultsA, resultsB, false);
        }

        static public void PrintComparison(ScanResultCollection resultsA, ScanResultCollection resultsB, bool bOnlyCloseMatches)
        {
            int total_name_mismatches = 0;
            int total_name_match = 0;
            int total_value_mismatches = 0;
            int total_value_match = 0;
            int total_weight = 0;
            int total_possible_weight = 0;

            StringBuilder output = new StringBuilder();

            output.AppendLine();
            output.AppendFormat("1 = {0} / {1}\r\n", resultsA.Name, resultsA.Hash);
            output.AppendFormat("2 = {0} / {1}\r\n", resultsB.Name, resultsB.Hash);

            output.AppendLine();
            output.AppendFormat("1 2\r\n");

            ScanResultCollection leftoverResults = new ScanResultCollection(resultsB);

            foreach (ScanResult result in resultsA.GetResultList())
            {
                bool bNameMatch = false;
                bool bValueMatch = false;

                total_possible_weight += result.Weight;

                ScanResult checkResult = resultsB.GetResult(result.Name);

                if (null != checkResult)
                {
                    bNameMatch = true;
                    total_name_match++;

                    if (checkResult.Value == result.Value)
                    {
                        bValueMatch = true;
                        total_value_match++;
                        total_weight += result.Weight;

                        // TODO: use uniqueness?
                    }
                    else
                    {
                        total_value_mismatches++;
                    }

                    leftoverResults.RemoveResult(checkResult);
                }
                else
                {
                    total_name_mismatches++;
                }

                if (bNameMatch)
                {
                    if (bValueMatch)
                    {
                        output.AppendFormat("+ + {0,-30} {1,-48}\r\n", result.Name, result.Value);
                    }
                    else
                    {
                        output.AppendFormat("+ - {0,-30} {1,-48}\r\n", result.Name, result.Value);
                        output.AppendFormat("- + {0,-30} {1,-48}\r\n", checkResult.Name, checkResult.Value);
                    }
                }
                else
                {
                    output.AppendFormat("+   {0,-30} {1,-48}\r\n", result.Name, result.Value);
                }
            }
            
            // Results that existed in B but not in A
            foreach (ScanResult result in leftoverResults.GetResultList())
            {
                total_name_mismatches++;
                total_possible_weight += result.Weight;
                output.AppendFormat("  + {0,-30} {1,-48}\r\n", result.Name, result.Value);
            }

            output.AppendLine();
            output.AppendFormat("[Total Name Matches : Mismatches]: {0} : {1}\r\n", total_name_match.ToString(), total_name_mismatches.ToString());
            output.AppendFormat("[Total Value Matches : Mismatches]: {0} : {1}\r\n", total_value_match.ToString(), total_value_mismatches.ToString());
            output.AppendFormat("[Total Match Weight : Possible Weight]: {0} : {1}\r\n", total_weight.ToString(), total_possible_weight.ToString());

            // name matches account for a 25% share
            // value matches account for a 50% share
            // weight accounts for a 25% share

            double name_match_percentage = (double)total_name_match / (double)(total_name_match + total_name_mismatches);
            double value_match_percentage = (double)total_value_match / (double)(total_value_match + total_value_mismatches);
            double weight_match_percentage = (double)total_weight / (double)(total_possible_weight);
            
            double match_percentage = ((name_match_percentage * 0.25) + (value_match_percentage * 0.50) + (weight_match_percentage * 0.25)) * 100;

            output.AppendFormat("[Match percentage]: {0}\r\n", match_percentage.ToString("F2"));

            if (bOnlyCloseMatches)
            {
                if (match_percentage > 80)
                {
                    Console.WriteLine(output.ToString());
                }
            }
            else
            {
                Console.WriteLine(output.ToString());
            }
        }

    }
}
