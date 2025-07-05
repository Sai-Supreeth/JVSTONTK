using System;
using System.IO;
using System.Text.RegularExpressions;

class JvsToNtkConverter
{
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
        }

        Console.WriteLine($"C# file written to: {outputPath}");
    }

    // Expanded Java to C# conversion logic
    static string ConvertJavaToCSharp(string line)
    {
        // Data Types & Primitives
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



        // Collections / Generics
        line = Regex.Replace(line, @"ArrayList<([\w<>]+)>", "List<$1>");
        line = Regex.Replace(line, @"LinkedList<([\w<>]+)>", "LinkedList<$1>");
        line = Regex.Replace(line, @"HashMap<([\w<>]+),\s*([\w<>]+)>", "Dictionary<$1, $2>");
        line = Regex.Replace(line, @"Hashtable<([\w<>]+),\s*([\w<>]+)>", "Hashtable");
        line = Regex.Replace(line, @"HashSet<([\w<>]+)>", "HashSet<$1>");
        line = Regex.Replace(line, @"TreeMap<([\w<>]+),\s*([\w<>]+)>", "SortedDictionary<$1, $2>");
        line = Regex.Replace(line, @"Vector<([\w<>]+)>", "List<$1>");
        line = Regex.Replace(line, @"Stack<([\w<>]+)>", "Stack<$1>");
        line = Regex.Replace(line, @"Queue<([\w<>]+)>", "Queue<$1>");
        line = Regex.Replace(line, @"PriorityQueue<([\w<>]+)>", "SortedSet<$1>");
        line = Regex.Replace(line, @"Collections\.sort\(([^)]+)\)", "$1.Sort()");
        line = Regex.Replace(line, @"Arrays\.asList\(([^)]+)\)", "new List<$1>()");
        line = Regex.Replace(line, @"Arrays\.copyOf\(([^,]+),\s*([^)]+)\)", "Array.Copy($1, $2)");

        // Imports to usings
        line = Regex.Replace(line, @"import java\.util\..*;", "using System.Collections.Generic;");
        line = Regex.Replace(line, @"import java\.io\..*;", "using System.IO;");
        line = Regex.Replace(line, @"import java\.time\..*;", "using System;");
        line = Regex.Replace(line, @"import java\.math\..*;", "using System;");
        line = Regex.Replace(line, @"import java\.sql\..*;", "using System.Data;");
        line = Regex.Replace(line, @"import java\.lang\..*;", "using System;");
        line = Regex.Replace(line, @"import .+;", ""); // Remove other imports

        // System.out.println to Console.WriteLine
        line = Regex.Replace(line, @"System\.out\.println", "Console.WriteLine");
        line = Regex.Replace(line, @"System\.out\.print", "Console.Write");

        // Main method signature
        line = Regex.Replace(line, @"public static void main\s*\(\s*String\[\]\s*args\s*\)", "public static void Main(string[] args)");

        // Add more idiomatic conversions as needed
        return line;
    }

    // JVS to NTK conversion logic (your custom rules)
    static string ConvertJvsToNtk(string line)
    {
        string result = line;
        result = Regex.Replace(result, @"Ask\.", "ConvertedJvsUtils.Ask.");
        result = Regex.Replace(result, @"Table\.tableNew", "ble. TableNew");
        result = Regex.Replace(result, @"addCol", "AddCol");
        result = Regex.Replace(result, @"addRow", "AddRow");
        result = Regex.Replace(result, @"setString", "SetString");
        result = Regex.Replace(result, @"setTable", "SetTable");
        result = Regex.Replace(result, @"setTextEdit", "SetTextEdit");
        result = Regex.Replace(result, @"viewTable", "ViewTable");
        result = Regex.Replace(result, @"ok", "Ok");
        result = Regex.Replace(result, @"OCalendar\.today\(\)", "OCalendar.FormatDateInt(today)");
        result = Regex.Replace(result, @"COL_TYPE_ENUM\.COL_STRING", "COL_TYPE_ENUM.COL_STRING");
        result = Regex.Replace(result, @"COL_TYPE_ENUM\.COL_TABLE", "COL_TYPE_ENUM.COL_TABLE");
        // Remove Java-specific syntax
        result = result.Replace("public ", "");
        result = result.Replace("private ", "");
        result = result.Replace("protected ", "");
        result = result.Replace("@Override", "");
        result = result.Replace("@PluginType(SCRIPT_TYPE_ENUM.PARAM_SCRIPT)", "");
        result = result.Replace("implements IScript", "");
        result = result.Replace("throws OException", "");
        result = result.Replace("{", "{");
        result = result.Replace("}", "}");
        result = Regex.Replace(result, @"void execute \(IContainerContext context\)", "void Execute(IContainerContext context)");
        return result.TrimEnd();
    }
} 