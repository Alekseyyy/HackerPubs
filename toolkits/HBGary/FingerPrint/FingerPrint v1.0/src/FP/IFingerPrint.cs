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
    public interface IFingerPrint
    {
        /// <summary>
        /// Called when the FingerPrinter is first loaded, or when the FingerPrinter should reset
        /// </summary>
        /// <param name="theFullPath"></param>
        /// <returns></returns>
        bool OnLoad(string theFullPath);

        /// <summary>
        /// Called after load so the FingerPrinter can register binary search patterns
        /// </summary>
        /// <returns></returns>
        bool OnRegisterPatterns(PatternList theList);

        /// <summary>
        /// Called at the start of scanning
        /// </summary>
        /// <returns></returns>
        bool OnScan(string theFullPath, BinaryReader br, ScanResultCollection results);

        /// <summary>
        /// Called at the end of scanning
        /// </summary>
        /// <returns></returns>
        bool OnScanComplete(string theFullPath, ScanResultCollection results);

        /// <summary>
        /// Called when the scan engine locates a string, Note: this can be slow to use
        /// </summary>
        /// <param name="theString"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        bool OnEvaluateString(string theString, ScanResultCollection results);

        /// <summary>
        /// Called after all strings are located.  Passes a hashtable that includes
        /// all strings.  This is much faster than using OnEvaluateString.
        /// </summary>
        /// <param name="stringSet"></param>
        /// <returns></returns>
        bool OnEvaluateStringSet(Dictionary<string, bool> stringSet, ScanResultCollection results);

        /// <summary>
        /// Called when the scan engine locates a matching binary search pattern
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        bool OnPatternMatch(Pattern thePattern, ScanResultCollection results);
    }

    public class Pattern
    {
        public Pattern(string theName, string theBytePattern)
        {
            Name = theName;
            BytePattern = theBytePattern;
            HitCount = 0;
            Weight = 1;
            Uniqueness = 0;
        }

        public string Name;

        private string _bytePattern = string.Empty;

        /// <summary>
        /// A byte pattern to be searched for, use a space delimited hex notation without prefix and ?? as a wildcard.  
        /// For example: 00 11 22 aa ?? bb cc
        /// Notes: Do not start the pattern with a wildcard.
        /// </summary>
        public string BytePattern
        {
            get
            {
                return _bytePattern;
            }
            set
            {
                _bytePattern = value;
                string[] tokens = _bytePattern.Split(' ');
                _bytePatternLength = tokens.Length;
                byte theByte;
                if ((_bytePatternLength > 0 ) &&
                    (System.Byte.TryParse(tokens[0], System.Globalization.NumberStyles.HexNumber, null, out theByte)))
                {
                    _byteZero = theByte;
                }
            }
        }

        private int _bytePatternLength = 0;
        public int BytePatternLength
        {
            get
            {
                return _bytePatternLength;
            }
        }

        private byte _byteZero = 0;
        public byte ByteZero
        {
            get
            {
                return _byteZero;
            }
        }

        public int HitCount;
        public int Weight;
        public int Uniqueness;

        /// <summary>
        /// Use for whatever purposes are needed
        /// </summary>
        public object Tag;

        /// <summary>
        /// Used internally for tracking which FingerPrint registered this pattern
        /// </summary>
        internal string typeName;
    }

    public class PatternList
    {
        List<Pattern> _patterns = new List<Pattern>();

        public List<Pattern> Patterns { get { return _patterns; } }

        public void AddPattern (Pattern thePattern)
        {
            _patterns.Add(thePattern);
        }

        public void Clear()
        {
            _patterns.Clear();
        }
    }

    public abstract class BaseFingerPrint
    {
        protected PatternList _MyPatterns = new PatternList();

        virtual public void AddPattern(PatternList thePatternList, string Name, string Pattern, int weight, int uniqueness, object Tag)
        {
            Pattern newPattern = new Pattern(Name, Pattern);

            _MyPatterns.AddPattern(newPattern);
            thePatternList.AddPattern(newPattern);
        }

        virtual public void AddPattern(PatternList thePatternList, string Name, string Pattern)
        {
            AddPattern(thePatternList, Name, Pattern, 1, 0, null);
        }

        virtual public bool OnLoad()
        {
            _MyPatterns.Clear();
            return true;
        }

        virtual public bool OnEvaluateString(string theString, ScanResultCollection results)
        {
            return true;
        }

        virtual public bool OnEvaluateStringSet(Dictionary<string, bool> stringSet, ScanResultCollection results)
        {
            return true;
        }

        virtual public bool OnScan(string theString, BinaryReader br, ScanResultCollection results)
        {
            return true;
        }

        virtual public bool OnScanComplete(string theFullPath, ScanResultCollection results)
        {
            foreach (Pattern aPattern in _MyPatterns.Patterns)
            {
                if (aPattern.HitCount > 0)
                {
                    results.SetResult(aPattern.Name, aPattern.HitCount.ToString(), aPattern.Weight, aPattern.Uniqueness);
                }
            }

            return true;
        }

        virtual public bool OnRegisterPatterns(PatternList theList)
        {
            return true;
        }

        virtual public bool OnPatternMatch(Pattern thePattern, ScanResultCollection results)
        {
            return true;
        }
    }
}
