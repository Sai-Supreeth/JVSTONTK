using System;
using System.IO;
using System.Text.RegularExpressions;

class JvsToNtkConverter
{
    static bool previousWasOverride = false;

    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: JvsToNtkConverter.exe [jvs2ntk] <inputFile> <outputCsFile>");
            return;
        }

        string mode = (args[0].ToLower() == "jvs2ntk") ? "jvs2ntk" : "java2cs";
        string inputPath = (mode == "jvs2ntk") ? args[1] : args[0];
        string outputPath = (mode == "jvs2ntk") ? args[2] : args[1];
        
        if (!File.Exists(inputPath))
        {
            Console.WriteLine($"Input file not found: {inputPath}");
            return;
        }

        string[] lines = File.ReadAllLines(inputPath);
        using (StreamWriter writer = new StreamWriter(outputPath))
        {
            if (mode == "jvs2ntk")
            {
                foreach (var usingLine in AddUsings())
                {
                    writer.WriteLine(usingLine);
                }
                /*writer.WriteLine(); // Blank line after using directive*/
            }
            foreach (var line in lines)
            {
                string convertedLine = mode switch
                {
                    "java2cs" => ConvertJavaToCSharp(line),
                    "jvs2ntk" => ConvertJvsToNtk(line),
                    _ => throw new ArgumentException("Invalid mode. Use jvs2ntk for JVS-to-NTK conversion.")
                };
                if (!string.IsNullOrWhiteSpace(convertedLine))
                    writer.WriteLine(convertedLine);
            }
            writer.WriteLine("}");


        }

        Console.WriteLine($"C# file written to: {outputPath}");
    }

    // Expanded Java to C# conversion logic
    static string ConvertJavaToCSharp(string line)
    {
        string[] allLines = File.ReadAllLines(Environment.GetCommandLineArgs()[1]);

        line = HandleDiamondOperator(line, allLines);
        line = ConvertEnhancedForLoop(line);
        line = ConvertInheritanceAndInterfaces(line);
        line = ConvertDataTypes(line);
        line = ConvertScannerInput(line);
        line = ConvertCollectionDeclarations(line);
        line = ConvertCollectionInstantiations(line);
        line = ConvertCollectionMethods(line);
        line = ConvertHashMapMethods(line);
        line = ConvertImports(line);
        line = ConvertSystemOut(line);
        line = ConvertMainMethodSignature(line);
        line = ConvertClassAndInterfaceNames(line);
        line = ConvertMethodNames(line);
        line = ConvertMethodCallCasing(line);
        line = ConvertConstants(line);
        line = ConvertPropertyNames(line);
        line = ConvertNamespace(line);
        line = ConvertFileWriter(line);
        line = ConvertPatternMatcher(line);
        line = ConvertStandaloneMethodCalls(line);
        line = ConvertFinalClass(line);
        line = ConvertFinalField(line);
        line = HandleOverride(line);


        return line;
    }

    // JVS to NTK conversion logic (your custom rules)
    static string ConvertJvsToNtk(string line)
    {
       
        line = ConvertEnhancedForLoop(line);
        line = ConvertNamespace(line);
        line = ConvertInheritanceAndInterfaces(line);
        line = ConvertMethodNames(line);
        line = ConvertMethodCallCasing(line);
        line = ConvertStandaloneMethodCalls(line);
        line = ConvertDataTypes(line);
        line = ConvertCollectionDeclarations(line);
        line = ConvertCollectionInstantiations(line);
        line = ConvertCollectionMethods(line);
        line = RemoveImports(line);
        // line = ProcessExecuteReturnStatements(line);
        line = ConvertLoggerDeclaration(line);
        line = ConvertLoggerPrintToInfo(line);

        // --- NTK specific conversions ---

        // Replace @PluginCategory(SCRIPT_CATEGORY_ENUM.SCRIPT_CAT_GENERIC) with [ScriptCategoriesAttribute(EnumScriptCategory.Generic)]
        line = ConvertExecuteMethod(line);
        line = ConvertLineForSimpleReplacements(line);

        //line = Regex.Replace(
        //    line,
        //    @"@PluginCategory\s*\(\s*SCRIPT_CATEGORY_ENUM\.SCRIPT_CAT_GENERIC\s*\)",
        //    "[ScriptCategoriesAttribute(EnumScriptCategory.Generic)]"
        //);

        // Optionally, remove any @PluginCategory annotation (for other categories)
        // line = Regex.Replace(line, @"@PluginCategory\s*\([^)]+\)", "");

       
        return line.TrimEnd();
    }

    // --- HashMap to Dictionary method conversions ---


    // --- Topic-based helper methods ---

    static string HandleDiamondOperator(string line, string[] allLines)
    {
        string FindCollectionType(string varName, string[] lines, out string csType)
        {
            var patterns = new (string javaType, string csType, string pattern)[]
            {
                ("ArrayList", "List", $@"ArrayList<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("LinkedList", "LinkedList", $@"LinkedList<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("HashMap", "Dictionary", $@"HashMap<([\w<>,\s]+),\s*([\w<>,\s]+)>\s+{varName}\s*;"),
                ("HashSet", "HashSet", $@"HashSet<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("TreeMap", "SortedDictionary", $@"TreeMap<([\w<>,\s]+),\s*([\w<>,\s]+)>\s+{varName}\s*;"),
                ("Vector", "List", $@"Vector<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("Stack", "Stack", $@"Stack<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("Queue", "Queue", $@"Queue<([\w<>,\s]+)>\s+{varName}\s*;"),
                ("PriorityQueue", "SortedSet", $@"PriorityQueue<([\w<>,\s]+)>\s+{varName}\s*;")
            };
            foreach (var l in lines)
            {
                foreach (var (javaType, csTypeLocal, pattern) in patterns)
                {
                    var match = Regex.Match(l, pattern);
                    if (match.Success)
                    {
                        csType = csTypeLocal;
                        if (csType == "Dictionary" || csType == "SortedDictionary")
                            return $"{match.Groups[1].Value.Trim()}, {match.Groups[2].Value.Trim()}";
                        else
                            return match.Groups[1].Value.Trim();
                    }
                }
            }
            csType = null;
            return null;
        }

        return Regex.Replace(line, @"this\.(\w+)\s*=\s*new\s+(\w+)\s*<\s*>\s*\(\s*\)", m =>
        {
            var varName = m.Groups[1].Value;
            var javaType = m.Groups[2].Value;
            string csType;
            var type = FindCollectionType(varName, allLines, out csType);
            return (type != null && csType != null)
                ? $"this.{varName} = new {csType}<{type}>()"
                : m.Value;
        });
    }

    static string ConvertEnhancedForLoop(string line)
    {
        return Regex.Replace(
            line,
            @"for\s*\(\s*([\w<>,\s]+)\s+(\w+)\s*:\s*([^)]+)\)",
            "foreach ($1 $2 in $3)"
        );
    }

    static string ConvertInheritanceAndInterfaces(string line)
    {
        if (line.Contains("extends") && line.Contains("implements"))
        {
            // Only when both are present: extends → :, implements → ,
            line = Regex.Replace(line, @"\bextends\b", ":");
            line = Regex.Replace(line, @"\bimplements\b", ",");
        }
        else
        {
            // In all other cases: both extends and implements → :
            line = Regex.Replace(line, @"\bextends\b", ":");
            line = Regex.Replace(line, @"\bimplements\b", ":");
        }

        return line;
    }


    static string ConvertDataTypes(string line)
    {
        line = Regex.Replace(line, @"\bint\b", "int");
        line = Regex.Replace(line, @"\blong\b", "long");
        line = Regex.Replace(line, @"\bfloat\b", "float");
        line = Regex.Replace(line, @"\bdouble\b", "double");
        line = Regex.Replace(line, @"\bboolean\b", "bool");
        line = Regex.Replace(line, @"\bchar\b", "char");
        line = Regex.Replace(line, @"\bbyte\b", "byte");
        line = Regex.Replace(line, @"\bshort\b", "short");
        line = Regex.Replace(line, @"\bInteger\b", "int");
        line = Regex.Replace(line, @"\bDouble\b", "double");
        line = Regex.Replace(line, @"\bBoolean\b", "bool");
        line = Regex.Replace(line, @"\bLong\b", "long");
        line = Regex.Replace(line, @"\bFloat\b", "float");
        line = Regex.Replace(line, @"\bCharacter\b", "char");
        line = Regex.Replace(line, @"\bString\b", "string");
        return line;
    }

    static string ConvertScannerInput(string line)
    {
        line = Regex.Replace(line, @"Scanner\s+(\w+)\s*=\s*new\s+Scanner\s*\(\s*System\.in\s*\)\s*;?", "");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextInt\s*\(\s*\)", "int.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextDouble\s*\(\s*\)", "double.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextFloat\s*\(\s*\)", "float.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextLong\s*\(\s*\)", "long.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextBoolean\s*\(\s*\)", "bool.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextByte\s*\(\s*\)", "byte.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextShort\s*\(\s*\)", "short.Parse(Console.ReadLine())");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*nextLine\s*\(\s*\)", "Console.ReadLine()");
        line = Regex.Replace(line, @"\b(\w+)\s*\.\s*next\s*\(\s*\)", "Console.ReadLine()");
        return line;
    }

    static string ConvertCollectionDeclarations(string line)
    {
        line = Regex.Replace(line, @"\bArrayList\s*<\s*>", "");
        line = Regex.Replace(line, @"\bArrayList<([\w<>,\s]+)>", "List<$1>");
        line = Regex.Replace(line, @"\bLinkedList\s*<\s*>", "");
        line = Regex.Replace(line, @"\bLinkedList<([\w<>,\s]+)>", "LinkedList<$1>");
        line = Regex.Replace(line, @"\bHashMap\s*<\s*>", "");
        line = Regex.Replace(line, @"\bHashMap<([\w<>,\s]+),\s*([\w<>,\s]+)>", "Dictionary<$1, $2>");
        line = Regex.Replace(line, @"\bHashSet\s*<\s*>", "");
        line = Regex.Replace(line, @"\bHashSet<([\w<>,\s]+)>", "HashSet<$1>");
        line = Regex.Replace(line, @"\bTreeMap\s*<\s*>", "");
        line = Regex.Replace(line, @"\bTreeMap<([\w<>,\s]+),\s*([\w<>,\s]+)>", "SortedDictionary<$1, $2>");
        line = Regex.Replace(line, @"\bVector\s*<\s*>", "");
        line = Regex.Replace(line, @"\bVector<([\w<>,\s]+)>", "List<$1>");
        line = Regex.Replace(line, @"\bStack\s*<\s*>", "");
        line = Regex.Replace(line, @"\bStack<([\w<>,\s]+)>", "Stack<$1>");
        line = Regex.Replace(line, @"\bQueue\s*<\s*>", "");
        line = Regex.Replace(line, @"\bQueue<([\w<>,\s]+)>", "Queue<$1>");
        line = Regex.Replace(line, @"\bPriorityQueue\s*<\s*>", "");
        line = Regex.Replace(line, @"\bPriorityQueue<([\w<>,\s]+)>", "SortedSet<$1>");
        return line;
    }

    static string ConvertCollectionInstantiations(string line)
    {
        line = Regex.Replace(line, @"new\s+ArrayList<([\w<>,\s]+)>\s*\(\s*\)", "new List<$1>()");
        line = Regex.Replace(line, @"new\s+LinkedList<([\w<>,\s]+)>\s*\(\s*\)", "new LinkedList<$1>()");
        line = Regex.Replace(line, @"new\s+HashMap<([\w<>,\s]+),\s*([\w<>,\s]+)>\s*\(\s*\)", "new Dictionary<$1, $2>()");
        line = Regex.Replace(line, @"new\s+HashSet<([\w<>,\s]+)>\s*\(\s*\)", "new HashSet<$1>()");
        line = Regex.Replace(line, @"new\s+TreeMap<([\w<>,\s]+),\s*([\w<>,\s]+)>\s*\(\s*\)", "new SortedDictionary<$1, $2>()");
        line = Regex.Replace(line, @"new\s+Vector<([\w<>,\s]+)>\s*\(\s*\)", "new List<$1>()");
        line = Regex.Replace(line, @"new\s+Stack<([\w<>,\s]+)>\s*\(\s*\)", "new Stack<$1>()");
        line = Regex.Replace(line, @"new\s+Queue<([\w<>,\s]+)>\s*\(\s*\)", "new Queue<$1>()");
        line = Regex.Replace(line, @"new\s+PriorityQueue<([\w<>,\s]+)>\s*\(\s*\)", "new SortedSet<$1>()");
        return line;
    }

    static string ConvertCollectionMethods(string line)
    {
        line = Regex.Replace(line, @"Collections\.sort\(([^)]+)\)", "$1.Sort()");
        line = Regex.Replace(line, @"Arrays\.asList\(([^)]+)\)", "new List<$1>()");
        line = Regex.Replace(line, @"Arrays\.copyOf\(([^,]+),\s*([^)]+)\)", "Array.Copy($1, $2)");
        return line;
    }
    static string ConvertHashMapMethods(string line)
    {
        // put: myMap.put(key, value) or myMap.Put(key, value) -> myMap[key] = value;
        line = Regex.Replace(line, @"(\w+)\.(put)\s*\(\s*([^,]+)\s*,\s*([^)]+)\)", "$1[$3] = $4", RegexOptions.IgnoreCase);

        // get: myMap.get(key) or myMap.Get(key) -> myMap[key]
        line = Regex.Replace(line, @"(\w+)\.(get)\s*\(\s*([^)]+)\)", "$1[$3]", RegexOptions.IgnoreCase);

        // containsKey: myMap.containsKey(key) or myMap.ContainsKey(key) -> myMap.ContainsKey(key)
        line = Regex.Replace(line, @"(\w+)\.(containsKey)\s*\(\s*([^)]+)\)", "$1.ContainsKey($3)", RegexOptions.IgnoreCase);

        // containsValue: myMap.containsValue(value) or myMap.ContainsValue(value) -> myMap.ContainsValue(value)
        line = Regex.Replace(line, @"(\w+)\.(containsValue)\s*\(\s*([^)]+)\)", "$1.ContainsValue($3)", RegexOptions.IgnoreCase);

        // remove: myMap.remove(key) or myMap.Remove(key) -> myMap.Remove(key)
        line = Regex.Replace(line, @"(\w+)\.(remove)\s*\(\s*([^)]+)\)", "$1.Remove($3)", RegexOptions.IgnoreCase);

        // clear: myMap.clear() or myMap.Clear() -> myMap.Clear()
        line = Regex.Replace(line, @"(\w+)\.(clear)\s*\(\s*\)", "$1.Clear()", RegexOptions.IgnoreCase);

        // keySet: myMap.keySet() or myMap.KeySet() -> myMap.Keys
        line = Regex.Replace(line, @"(\w+)\.(keySet)\s*\(\s*\)", "$1.Keys", RegexOptions.IgnoreCase);

        // values: myMap.values() or myMap.Values() -> myMap.Values
        line = Regex.Replace(line, @"(\w+)\.(values)\s*\(\s*\)", "$1.Values", RegexOptions.IgnoreCase);

        // entrySet: myMap.entrySet() or myMap.EntrySet() -> myMap
        line = Regex.Replace(line, @"(\w+)\.(entrySet)\s*\(\s*\)", "$1", RegexOptions.IgnoreCase);

        return line;
    }
    static string ConvertImports(string line)
    {
        line = Regex.Replace(line, @"import java\.util\..*;", "using System;");
        line = Regex.Replace(line, @"import java\.io\..*;", "using System.IO;");
        line = Regex.Replace(line, @"import java\.time\..*;", "using System;");
        line = Regex.Replace(line, @"import java\.math\..*;", "using System;");
        line = Regex.Replace(line, @"import java\.sql\..*;", "using System.Data;");
        line = Regex.Replace(line, @"import java\.lang\..*;", "using System;");
        line = Regex.Replace(line, @"import .+;", "");
        return line;
    }

    static string ConvertSystemOut(string line)
    {
        line = Regex.Replace(line, @"System\.out\.println", "Console.WriteLine");
        line = Regex.Replace(line, @"System\.out\.print", "Console.Write");
        return line;
    }

    static string ConvertMainMethodSignature(string line)
    {
        return Regex.Replace(line, @"public static void main\s*\(\s*String\[\]\s*args\s*\)", "public static void Main(string[] args)");
    }

    static string ConvertClassAndInterfaceNames(string line)
    {
        line = Regex.Replace(line, @"class\s+([a-zA-Z_][\w]*)", m => $"class {ToPascalCase(m.Groups[1].Value)}");
        line = Regex.Replace(line, @"interface\s+([a-zA-Z_][\w]*)", m => $"interface I{ToPascalCase(m.Groups[1].Value)}");
        return line;
    }

    static string ConvertMethodNames(string line)
    {
        return Regex.Replace(line, @"(\b(public|private|protected|static|final|synchronized|abstract)\s+)*(\w[\w<>]*)\s+([a-z][\w]*)\s*\(", m =>
        {
            var returnType = m.Groups[3].Value;
            var methodName = m.Groups[4].Value;
            if (returnType != methodName)
                return m.Value.Replace(methodName, ToPascalCase(methodName));
            return m.Value;
        });
    }

    static string ConvertConstants(string line)
    {
        return Regex.Replace(line, @"\b([A-Z][A-Z0-9_]+)\b", m =>
        {
            if (m.Value.Contains("_"))
                return ToPascalCase(m.Value.ToLower());
            return m.Value;
        });
    }

    static string ConvertPropertyNames(string line)
    {
        return Regex.Replace(line, @"public\s+(\w[\w<>]*)\s+([a-z][\w]*)\s*;", m =>
            $"public {m.Groups[1].Value} {ToPascalCase(m.Groups[2].Value)};"
        );
    }

    static string ConvertNamespace(string line)
    {
        return Regex.Replace(line, @"package\s+([a-z0-9_.]+);", m =>
        {
            var parts = m.Groups[1].Value.Split('.');
            var pascal = string.Join('.', parts.Select(ToPascalCase));
            return $"namespace {pascal} {{";
        });

    }

    static string ConvertFileWriter(string line)
    {
        return Regex.Replace(line, @"FileWriter\s+(\w+)\s*=\s*new\s+FileWriter\s*\(\s*(\w+)\s*\)\s*;", "StreamWriter $1 = new StreamWriter($2.FullName);");
    }

    static string ConvertPatternMatcher(string line)
    {
        line = Regex.Replace(line, @"Pattern\s+(\w+)\s*=\s*Pattern\.compile\(([^)]+)\);", "Regex $1 = new Regex($2);");
        line = Regex.Replace(line, @"Matcher\s+(\w+)\s*=\s*(\w+)\.matcher\(([^)]+)\);", "MatchCollection $1 = $2.Matches($3);");
        return line;
    }

    static string ConvertMethodCallCasing(string line)
    {
        return Regex.Replace(line, @"(\b\w+\b)\.([a-z]\w*)\s*\(", m =>
        {
            var objName = m.Groups[1].Value;
            var methodName = m.Groups[2].Value;
            var pascalMethod = char.ToUpper(methodName[0]) + methodName.Substring(1);
            return $"{objName}.{pascalMethod}(";
        });
    }

    //static string ConvertStandaloneMethodCalls(string line)
    //{
    //    string[] keywords = new[]
    //    {
    //        "if", "else", "else if","for", "foreach" , "while", "do", "switch", "case", "default", "break", "continue", "return",
    //        "try", "catch", "finally", "throw", "throws", "public", "private", "protected", "static", "final",
    //        "void", "int", "long", "float", "double", "boolean", "char", "byte", "short", "new", "class",
    //        "interface", "enum", "extends", "implements", "import", "package", "abstract", "synchronized",
    //        "volatile", "const", "goto", "instanceof", "this", "super", "namespace", "using", "readonly",
    //        "sealed", "var", "out", "ref", "in", "params", "get", "set", "add", "remove", "partial", "yield",
    //        "lock", "async", "await", "true", "false", "null"
    //    };

    //    return Regex.Replace(line, @"(?<![\.\w])([a-z]\w*)\s*\(", m =>
    //    {
    //        var methodName = m.Groups[1].Value;
    //        if (char.IsLower(methodName[0]) && Array.IndexOf(keywords, methodName) == -1)
    //        {
    //            var pascalMethod = char.ToUpper(methodName[0]) + methodName.Substring(1);
    //            return $"{pascalMethod}(";
    //        }
    //        return m.Value;
    //    });

    //}
    static string ConvertStandaloneMethodCalls(string line)
    {
        // Fix 'else If' to 'else if'
        line = Regex.Replace(line, @"\belse\s+If\b", "else if", RegexOptions.IgnoreCase);

        var lowercaseKeywords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "if", "else", "for", "foreach", "while", "do", "switch", "case", "default",
        "break", "continue", "return", "try", "catch", "finally", "throw", "throws",
        "public", "private", "protected", "static", "final", "void", "int", "long",
        "float", "double", "boolean", "char", "byte", "short", "new", "class",
        "interface", "enum", "extends", "implements", "import", "package", "abstract",
        "synchronized", "volatile", "const", "goto", "instanceof", "this", "super",
        "namespace", "using", "readonly", "sealed", "var", "out", "ref", "in",
        "params", "get", "set", "add", "remove", "partial", "yield", "lock", "async",
        "await", "true", "false", "null"
    };

        return Regex.Replace(line, @"(?<!\w)([_a-zA-Z]\w*)\s*\(", m =>
        {
            var methodName = m.Groups[1].Value;

            // Lowercase keywords only if they are not part of method calls
            if (lowercaseKeywords.Contains(methodName))
                return methodName.ToLower() + "(";

            // Convert to PascalCase and keep underscores
            return ToPascalCase(methodName) + "(";
        });
    }



    static string ConvertFinalClass(string line)
    {
        return Regex.Replace(line, @"\bfinal\s+class\b", "sealed class");
    }

    static string ConvertFinalField(string line)
    {
        return Regex.Replace(
            line,
            @"(?<=^|\s)final\s+(\w[\w<>,\s]*)\s+([A-Za-z_][A-Za-z0-9_]*)(\s*[;=])",
            "readonly $1 $2$3"
        );
    }

    static string HandleOverride(string line)
    {
        if (Regex.IsMatch(line, @"@\s*Override"))
        {
            previousWasOverride = true;
            line = Regex.Replace(line, @"@\s*Override\s*", "");
        }
        else if (previousWasOverride)
        {
            line = Regex.Replace(
                line,
                @"^(?<indent>\s*)public\s+void\s+(?<name>\w+)\s*\(",
                "${indent}public override void ${name}("
            );
            previousWasOverride = false;
        }
        return line;
    }



    //// Add these helper methods inside your JvsToNtkConverter class
    //static string ToPascalCase(string input)
    //{
    //    return Regex.Replace(input, @"(^|_)(\w)", m => m.Groups[2].Value.ToUpper());


    ////}
    //static string ToPascalCase(string input)
    //{
    //    return Regex.Replace(input, @"(^|_)(\w)", m =>
    //        (m.Groups[1].Value == "_" ? "_" : "") + m.Groups[2].Value.ToUpper());
    //}
    static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return Regex.Replace(input, @"(^|_)(\w)", match =>
        {
            string prefix = match.Groups[1].Value; // "" or "_"
            string letter = match.Groups[2].Value.ToUpper();
            return prefix + letter;
        });
    }


    static string ToCamelCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return char.ToLower(input[0]) + input.Substring(1);
    }
    static string RemoveImports(string line)
    {
        return line.TrimStart().StartsWith("import ") ? string.Empty : line;
    }

    static List<string> AddUsings()
    {
        return new List<string>
    {

        "using System;",
        "using System.Collections.Generic;",
        "using System.Collections.ObjectModel;",
        "using System.Linq;",
        "using System.Text;",
        "using Olf.NetToolkit;",
        "using Olf.NetToolkit.Enums;",
        "using Olf.NetToolkit.Tpm;",
        "using Olf.NetToolkit.ReportBuilder;",
        "using Olf.Embedded.Generic;",
        "using Olf.Embedded.Application;"
    };
    }
    static string ConvertExecuteMethod(string line)
    {
        return Regex.Replace(line, @"public\s+void\s+Execute\s*\(.*?\)\s*throws\s+OException",
            "public override Olf.Openrisk.Table.Table Execute(Context context, EnumScriptCategory category, Olf.Openrisk.Table.ConstTable table)");
    }
    static string ConvertLineForSimpleReplacements(string line)
    {
        // At the very top of ConvertJvsToNtk, before any other processing:
        if (Regex.IsMatch(line.TrimStart(), @"^@?ScriptAttributes", RegexOptions.IgnoreCase))
            return "[ScriptCategoriesAttribute(EnumScriptCategory.Generic)]";



        // Replace Java parse methods with C# Parse equivalents, preserving the parameter
        line = Regex.Replace(line, @"\b(?:Integer|int)\.parseInt\s*\(([^)]*)\)", "int.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Double|double)\.parseDouble\s*\(([^)]*)\)", "double.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Float|float)\.parseFloat\s*\(([^)]*)\)", "float.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Long|long)\.parseLong\s*\(([^)]*)\)", "long.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Short|short)\.parseShort\s*\(([^)]*)\)", "short.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Byte|byte)\.parseByte\s*\(([^)]*)\)", "byte.Parse($1)", RegexOptions.IgnoreCase);
        line = Regex.Replace(line, @"\b(?:Boolean|boolean)\.parseBoolean\s*\(([^)]*)\)", "bool.Parse($1)", RegexOptions.IgnoreCase);

        // Replace 'ToUpperCase()' with 'ToUpper()'
        line = Regex.Replace(line, @"ToUpperCase\(\)", "ToUpper()");

        // Replace 'size' with 'Count'
        line = Regex.Replace(line, @"\bsize\(\)\b", "Count", RegexOptions.IgnoreCase);
        // Replace 'final' with 'readonly'
        line = Regex.Replace(line, @"\bfinal\b", "readonly");
        // Replace GetMessage(...) with Message
        line = Regex.Replace(line, @"\bGetMessage\s*\(.*?\)", "Message");

        // Remove 'throws OException'
        line = Regex.Replace(line, @"\s*throws\b[^;{]*([;{]?)", "$1");


        // Replace any form of 'length', 'Length', 'length()', or 'Length()' with 'Length'
        line = Regex.Replace(line, @"\b(length|Length)\s*(\(\s*\))?", "Length");

        // Replace Java-style 'replace(...)' with C# 'Replace(...)'
        line = Regex.Replace(line, @"\.replace\s*\(", ".Replace(");

        // Replace: Table <var> = context.GetArgumentsTable();
        // With:    Table <var> = ContainerContext.GetGlobalContext().GetReturnTable();
        line = Regex.Replace(
            line,
            @"\bTable\s+(\w+)\s*=\s*context\.GetArgumentsTable\s*\(\s*\)\s*;",
            "Table $1 = ContainerContext.GetGlobalContext().GetReturnTable();"
        );
        //IsEmpty() to string.IsNullOrEmpty()
        line = Regex.Replace(line, @"\b(\w+)\.IsEmpty\s*\(\)", @"string.IsNullOrEmpty($1)");

        //IScript with AbstractGenericScript
        line = Regex.Replace(line, @"(?<=class\s+\w+\s*:\s*)IScript\b", "AbstractGenericScript");

        //Replace <ENUM>.ToInt()to (< primitive - type >) < ENUM >
        line = Regex.Replace(line, @"\b(\w+\.\w+)\.To(\w+)\s*\(\)", m =>
        {
            string enumAccess = m.Groups[1].Value;      // e.g., OLF_RETURN_CODE.OLF_RETURN_SUCCEED
            string castType = m.Groups[2].Value.ToLower(); // e.g., Int → int
            return $"({castType}){enumAccess}";
        });

        //Replace OCalendarBase.<method>(...) to Olf.NetToolkit.Fnd.OCalendarBase.<method>(...)
        line = Regex.Replace(line, @"\b(OCalendarBase\.\w+\s*\()", "Olf.NetToolkit.Fnd.$1");
        ////trim () to Trim()
        line = Regex.Replace(line, @"(?<=\))\s*\.\s*trim\s*\(", ".Trim(");


        return line;
    }
    static string ConvertLoggerDeclaration(string line)
    {

        // Replace access modifier and type
        line = Regex.Replace(line, @"\b(public|private)\s*JVS_INC_LogFunctions", "$1 Logger");

        // Replace field declaration
        line = Regex.Replace(line, @"\b(private|public|protected)\s+JVS_INC_Standard\s+(\w+)\s*;", "$1 Logger $2;");

        // Replace instantiation in constructor
        line = Regex.Replace(line, @"(\w+)\s*=\s*new\s+JVS_INC_Standard\s*\(\s*\)\s*;", "$1 = new Logger(this.Plugin.Name);");

        // Replace 'new Logger()' with 'new Logger(this.Plugin.Name)'
        line = Regex.Replace(line, @"new Logger\(\)", "new Logger(this.Plugin.Name)");


        return line;
    }
    static string ConvertLoggerPrintToInfo(string line)
    {
        // Replace m_INCStandard.Print(...) with m_INCStandard.Info(...), joining all arguments with +
        return Regex.Replace(
            line,
            @"(\bm_INCStandard)\.Print\s*\(([^)]*)\)",
            m =>
            {
                var logger = m.Groups[1].Value;
                var args = m.Groups[2].Value;
                // Replace all commas with +
                var joinedArgs = args.Replace(",", "+");
                return $"{logger}.Info({joinedArgs})";
            });
    }



    /*  static bool insideExecuteMethod = false;

      static string ProcessExecuteReturnStatements(string line)
      {
          // Detect method start
          if (Regex.IsMatch(line, @"public\s+virtual\s+Olf\.Openrisk\.Table\.Table\s+Execute\s*\("))
          {
              insideExecuteMethod = true;
          }

          // Detect method end - very simple approach, assuming method ends at line with just "}"
          if (insideExecuteMethod && line.Trim() == "}")
          {
              insideExecuteMethod = false;
          }

          // Replace 'return;' with 'return null;' only inside Execute method
          if (insideExecuteMethod && Regex.IsMatch(line.Trim(), @"^return\s*;\s*$"))
          {
              return "return null;";
          }

          return line;
      }
  */
}
