using System;
using System.IO;
using System.Text.RegularExpressions;

class JvsToNtkConverter
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: JvsToNtkConverter.exe <mode: java2cs|jvs2ntk> <inputFile> <outputCsFile>");
            return;
        }

        string mode = args[0].ToLower();
        string inputPath = args[1];
        string outputPath = args[2];

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
                    _ => throw new ArgumentException("Invalid mode. Use java2cs or jvs2ntk.")
                };
                if (!string.IsNullOrWhiteSpace(convertedLine))
                    writer.WriteLine(convertedLine);
            }
        }

        Console.WriteLine($"C# file written to: {outputPath}");
    }

    // Java to C# conversion logic (expand as needed)
    static string ConvertJavaToCSharp(string line)
    {
        // Example: import to using, ArrayList to List, System.out.println to Console.WriteLine, etc.
        line = Regex.Replace(line, @"import java\.util\.ArrayList;", "using System.Collections.Generic;");
        line = Regex.Replace(line, @"import java\.util\.List;", "using System.Collections.Generic;");
        line = Regex.Replace(line, @"import java\.io\..*;", "using System.IO;");
        line = Regex.Replace(line, @"import .+;", ""); // Remove other imports
        line = Regex.Replace(line, @"ArrayList<(\w+)>", "List<$1>");
        line = Regex.Replace(line, @"System\.out\.println", "Console.WriteLine");
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