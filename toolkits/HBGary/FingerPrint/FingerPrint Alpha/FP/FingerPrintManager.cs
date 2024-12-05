using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.CSharp;
using System.CodeDom.Compiler;
using System.Globalization;
using System.Reflection;

namespace FP
{
    class FingerPrintManager
    {
        static CodeDomProvider _provider = null;

        static List<string> _sourceFiles = new List<string>();
        static List<string> _requiredAssemblies = new List<string>();

        static Dictionary<string, object> _objectInstances = new Dictionary<string, object>();
        static Dictionary<string, bool> _loadResults = new Dictionary<string, bool>();

        static Dictionary<string, MethodInfo> _loadMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> _scanMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> _scanCompleteMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> _evaluateStringMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> _registerPatternMethods = new Dictionary<string, MethodInfo>();
        static Dictionary<string, MethodInfo> _patternMatchMethods = new Dictionary<string, MethodInfo>();

        static PatternList _patterns = new PatternList();

        static public PatternList GetPatterns() { return _patterns; }

        static private void Clear()
        {
            _provider = null;

            _sourceFiles.Clear();
            _requiredAssemblies.Clear();

            _objectInstances.Clear();
            _loadResults.Clear();

            _loadMethods.Clear();
            _scanMethods.Clear();
            _scanCompleteMethods.Clear();
            _evaluateStringMethods.Clear();
            _registerPatternMethods.Clear();
            _patternMatchMethods.Clear();

            _patterns.Clear();
        }

        /// <summary>
        /// Compile all .cs files in the provided directory into a single assembly
        /// </summary>
        /// <param name="theDirectory"></param>
        static public bool LoadFingerPrints(string theDirectory)
        {
            Clear();

            string[] theFiles = Directory.GetFiles(theDirectory);

            foreach (string file in theFiles)
            {
                if ((file == ".") || (file == ".."))
                    continue;

                string FullPath = Path.Combine(theDirectory, file);

                FileInfo sourceFile = new FileInfo(FullPath);

                // Ignore non .cs files
                if (sourceFile.Extension.ToUpper(CultureInfo.InvariantCulture) != ".CS")
                {
                    //Console.WriteLine("Source file must have a .cs extension");
                    continue;
                }

                _sourceFiles.Add(FullPath);
            }

            _patterns.Patterns.Clear();

            CompilerResults cr;

            if (CompileAssembly(out cr))
            {
                if ((null == cr) || (null == cr.CompiledAssembly))
                {
                    Console.WriteLine("Invalid compiled assembly");
                    return false;
                }                

                Module[] modules = cr.CompiledAssembly.GetModules();

                foreach (Module mod in modules)
                {
                    Type[] types = mod.GetTypes();

                    foreach (Type type in types)
                    {
                        if ((null != type.GetInterface("IFingerPrint")) || (type.BaseType.Name == "BaseFingerPrint"))
                        {
                            MethodInfo[] methods = type.GetMethods();

                            object objectInstance = cr.CompiledAssembly.CreateInstance(type.FullName);

                            _objectInstances[type.FullName] = objectInstance;

                            bool bLoad = false;
                            bool bEval = false;
                            bool bRegister = false;
                            bool bMatch = false;
                            bool bScan = false;
                            bool bScanComplete = false;

                            foreach (MethodInfo method in methods)
                            {
                                if (method.Name == "OnLoad")
                                {
                                    _loadMethods[type.FullName] = method;
                                    bLoad = true;
                                }
                                else if (method.Name == "OnEvaluateString")
                                {
                                    _evaluateStringMethods[type.FullName] = method;
                                    bEval = true;
                                }
                                else if (method.Name == "OnRegisterPatterns")
                                {
                                    _registerPatternMethods[type.FullName] = method;
                                    bRegister = true;
                                }
                                else if (method.Name == "OnPatternMatch")
                                {
                                    _patternMatchMethods[type.FullName] = method;
                                    bMatch = true;
                                }
                                else if (method.Name == "OnScan")
                                {
                                    _scanMethods[type.FullName] = method;
                                    bScan = true;
                                }
                                else if (method.Name == "OnScanComplete")
                                {
                                    _scanCompleteMethods[type.FullName] = method;
                                    bScanComplete = true;
                                }
                            }

                            if (bLoad && bRegister && bMatch && bEval && bScan && bScanComplete)
                            {
                                //Console.WriteLine("{0} loaded", type.FullName);
                            }
                            else
                            {
                                Console.WriteLine("{0}: Did not find all required methods", type.FullName);
                                _loadMethods.Remove(type.FullName);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Failed to compile");
                return false;
            }

            OnLoad();

            return true;
        }

        static public void ResetFingerPrints()
        {
            OnLoad();
        }

        static public void OnLoad()
        {
            foreach (KeyValuePair<string, MethodInfo> pair in _loadMethods)
            {
                if (_objectInstances.ContainsKey(pair.Key))
                {
                    object theObject = _objectInstances[pair.Key];

                    if (_loadMethods.ContainsKey(pair.Key))
                    {
                        MethodInfo method = _loadMethods[pair.Key];
                        object result = method.Invoke(theObject, null );
                        bool bLoad = (result == null) ? false : (bool)result;
                        _loadResults[pair.Key] = bLoad;
                    }
                }                
            }

            OnRegisterPatterns();

            return;
        }

        static public void OnRegisterPatterns()
        {
            _patterns.Clear();

            foreach (KeyValuePair<string, bool> pair in _loadResults)
            {
                if (pair.Value == true)
                {
                    if ( _objectInstances.ContainsKey(pair.Key))
                    {
                        object theObject = _objectInstances[pair.Key];

                        if (_registerPatternMethods.ContainsKey(pair.Key))
                        {
                            MethodInfo method = _registerPatternMethods[pair.Key];

                            PatternList patternList = new PatternList();

                            object result = method.Invoke(theObject, new object[] { patternList });

                            foreach (Pattern aPattern in patternList.Patterns)
                            {
                                // Set the typename for lookups later
                                aPattern.typeName = pair.Key;
                                aPattern.HitCount = 0;

                                // add to the global pattern list
                                _patterns.AddPattern(aPattern);
                            }
                        }
                    }
                }
            }

            return;
        }

        static public void OnPatternMatch(Pattern thePattern, ScanResultCollection results)
        {
            thePattern.HitCount++;

            string typeName = thePattern.typeName;

            if ( _loadResults.ContainsKey(typeName))
            {
                bool bRes = _loadResults[typeName];
                if (true == bRes)
                {
                    if (_objectInstances.ContainsKey(typeName))
                    {
                        object theObject = _objectInstances[typeName];

                        if (_patternMatchMethods.ContainsKey(typeName))
                        {
                            MethodInfo method = _patternMatchMethods[typeName];

                            object result = method.Invoke(theObject, new object[] { thePattern, results });
                        }
                    }
                }
            }

            return;
        }

        static public void OnEvaluateString(string theString, ScanResultCollection results)
        {
            foreach (KeyValuePair<string, bool> pair in _loadResults)
            {
                if (pair.Value == true)
                {
                    if (_objectInstances.ContainsKey(pair.Key))
                    {
                        object theObject = _objectInstances[pair.Key];

                        if (_evaluateStringMethods.ContainsKey(pair.Key))
                        {
                            MethodInfo method = _evaluateStringMethods[pair.Key];

                            object result = method.Invoke(theObject, new object[] { theString, results });
                        }
                    }
                }
            }

            return;
        }

        static public void OnScan(string theFullPath, BinaryReader br, ScanResultCollection results)
        {
            foreach (KeyValuePair<string, bool> pair in _loadResults)
            {
                if (pair.Value == true)
                {
                    if (_objectInstances.ContainsKey(pair.Key))
                    {
                        object theObject = _objectInstances[pair.Key];

                        if (_scanMethods.ContainsKey(pair.Key))
                        {
                            MethodInfo method = _scanMethods[pair.Key];

                            object result = method.Invoke(theObject, new object[] { theFullPath, br, results });

                            // Reset binary reader to the beginning
                            br.BaseStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                }
            }

            return;
        }

        static public void OnScanComplete(string theFullPath, ScanResultCollection results)
        {
            foreach (KeyValuePair<string, bool> pair in _loadResults)
            {
                if (pair.Value == true)
                {
                    if (_objectInstances.ContainsKey(pair.Key))
                    {
                        object theObject = _objectInstances[pair.Key];

                        if (_scanCompleteMethods.ContainsKey(pair.Key))
                        {
                            MethodInfo method = _scanCompleteMethods[pair.Key];

                            object result = method.Invoke(theObject, new object[] { theFullPath, results });
                        }
                    }
                }
            }

            return;
        }


        /// <summary>
        /// Compile all the located source files into a single assembly 
        /// </summary>
        /// <returns></returns>
        static private bool CompileAssembly(out CompilerResults results)
        {
            results = null;

            if (null == _provider)
            {
                _provider = new CSharpCodeProvider(new Dictionary<string, string>() { { "CompilerVersion", "v3.5" } });
                if (null == _provider)
                {
                    Console.WriteLine("Failed to create a provider");
                    return false;
                }

                _requiredAssemblies.Add("System.dll");
                _requiredAssemblies.Add("System.Core.dll");
                _requiredAssemblies.Add("System.Data.dll");
                _requiredAssemblies.Add("System.Xml.dll");
                _requiredAssemblies.Add("FP.exe");
            }

            bool compileSuccess = false;

            CompilerParameters cp = new CompilerParameters(_requiredAssemblies.ToArray(), "", true);

            //String dllName = String.Format(@"{0}_{1}.dll",
            //    Path.GetTempFileName(),
            //    sourceFile.Name.Replace(".","_"));
            cp.IncludeDebugInformation = true;
            cp.GenerateExecutable = false;
            cp.GenerateInMemory = true;
            cp.TreatWarningsAsErrors = false;

            CompilerResults cr = _provider.CompileAssemblyFromFile(cp, _sourceFiles.ToArray());
            
            if (cr.Errors.Count > 0)
            {
                //Console.WriteLine("Source files {0}:", theSourceFiles);
                foreach (CompilerError ce in cr.Errors)
                {
                    Console.WriteLine("Error: {0}", ce.ToString());
                }
            }
            else
            {
                foreach (string aFile in _sourceFiles)
                {
                    Console.WriteLine("{0} compiled successfully", Path.GetFileName(aFile));
                }

                results = cr;

                compileSuccess = true;
            }

            return compileSuccess;
        }
    }
}
