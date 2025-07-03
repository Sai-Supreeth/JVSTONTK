using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

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

    // Java to C# conversion logic (robust, general-purpose)
    static string ConvertJavaToCSharp(string line)
    {
        string trimmed = line.TrimStart();
        // 1. Imports to Usings
        if (trimmed.StartsWith("import java.util.ArrayList"))
            return "using System.Collections.Generic;";
        if (trimmed.StartsWith("import java.util.HashMap"))
            return "using System.Collections.Generic;";
        if (trimmed.StartsWith("import java.util.List"))
            return "using System.Collections.Generic;";
        if (trimmed.StartsWith("import java.util.*"))
            return "using System.Collections.Generic;";
        if (trimmed.StartsWith("import java.io."))
            return "using System.IO;";
        if (trimmed.StartsWith("import java.lang."))
            return "using System;";
        if (trimmed.StartsWith("import java."))
            return "using System;";
        if (trimmed.StartsWith("import "))
            return ""; // Remove other imports

        // 2. Package to Namespace (handled at file level, not line-by-line)
        if (trimmed.StartsWith("package "))
        {
            string ns = trimmed.Substring(8).TrimEnd(';').Trim();
            return $"namespace {ns} {{";
        }

        // 3. Class signature
        line = Regex.Replace(line, @"public class (\w+)", "class $1");
        line = Regex.Replace(line, @"class (\w+) ", "class $1 ");
        // 4. Main method signature (force public static void Main)
        line = Regex.Replace(line, @"(public |private |protected |)static void main\s*\(String\[\]\s*args\)", "public static void Main(string[] args)");

        // 5. Java types to C# types
        line = Regex.Replace(line, @"ArrayList<([A-Za-z0-9_]+)>", "List<$1>");
        line = Regex.Replace(line, @"HashMap<([A-Za-z0-9_]+),\s*([A-Za-z0-9_]+)>", "Dictionary<$1, $2>");
        line = Regex.Replace(line, @"Integer", "int");
        line = Regex.Replace(line, @"Double", "double");
        line = Regex.Replace(line, @"Boolean", "bool");
        line = Regex.Replace(line, @"Long", "long");
        line = Regex.Replace(line, @"Float", "float");
        line = Regex.Replace(line, @"Character", "char");
        line = Regex.Replace(line, @"String", "string");

        // 6. Method and property names
        line = Regex.Replace(line, @"\.add\(", ".Add(");
        line = Regex.Replace(line, @"\.put\(", ".Add("); // HashMap put -> Add
        line = Regex.Replace(line, @"\.get\(", ".Get(");
        line = Regex.Replace(line, @"\.set\(", ".Set(");
        line = Regex.Replace(line, @"\.size\(\)", ".Count");
        line = Regex.Replace(line, @"System\.out\.println", "Console.WriteLine");
        line = Regex.Replace(line, @"System\.out\.print", "Console.Write");

        // 7. For-each loop (simple case)
        line = Regex.Replace(line, @"for \((\w+) (\w+) : (\w+)\)", "foreach ($1 $2 in $3)");

        // 8. Remove semicolons after class/method/namespace blocks
        line = Regex.Replace(line, @"\)\s*;", ")");
        line = Regex.Replace(line, @"\{\s*;", "{");

        // 9. Remove Java access modifiers (public/private/protected) from methods/fields
        line = Regex.Replace(line, @"\bpublic ", "");
        line = Regex.Replace(line, @"\bprivate ", "");
        line = Regex.Replace(line, @"\bprotected ", "");
        line = Regex.Replace(line, @"@Override", "");

        // 10. Array initializers
        line = Regex.Replace(line, @"new ([a-zA-Z0-9_<>]+)\[\] \{([^}]*)\}", "$2.Split(',').Select(x => x.Trim()).ToArray()");

        // 11. Remove throws clause
        line = Regex.Replace(line, @"throws [A-Za-z0-9_, ]+", "");

        // 12. Print lists/arrays in a C#-friendly way
        // Replace Console.WriteLine(var) where var is a List or array
        line = Regex.Replace(line, @"Console\.WriteLine\((\w+)\)", "Console.WriteLine(string.Join(\", \", $1))");

        // 13. Add semicolons at the end of statements where appropriate
        // Don't add to lines ending with {, }, ;, :, or blank lines
        string trimmedLine = line.TrimEnd();
        if (!string.IsNullOrWhiteSpace(trimmedLine)
            && !trimmedLine.EndsWith(";")
            && !trimmedLine.EndsWith("{")
            && !trimmedLine.EndsWith("}")
            && !trimmedLine.EndsWith(":")
            && !trimmedLine.StartsWith("using ")
            && !trimmedLine.StartsWith("namespace ")
            && !trimmedLine.StartsWith("//"))
        {
            line = line.TrimEnd() + ";";
        }

        // 14. Remove trailing whitespace
        return line.TrimEnd();
    }

    // JVS to NTK conversion logic (your custom rules)
    static string ConvertJvsToNtk(string line)
    {
        string result = line;
        result = RemoveImportStatements(result);
        result = ReplaceAskMethods(result);
        result = ReplaceTableMethods(result);
        result = ReplaceColumnMethods(result);
        result = ReplaceOCalendar(result);
        result = ReplaceColTypeEnum(result);
        result = RemoveJavaSpecificSyntax(result);
        result = FixMethodSignature(result);
        // Add more as needed
        return result.TrimEnd();
    }

    static string RemoveImportStatements(string line)
    {
        if (line.TrimStart().StartsWith("import "))
            return "";
        return line;
    }

    static string ReplaceAskMethods(string line)
    {
        line = Regex.Replace(line, @"Ask\\.", "ConvertedJvsUtils.Ask.");
        line = Regex.Replace(line, @"setTextEdit", "SetTextEdit");
        line = Regex.Replace(line, @"viewTable", "ViewTable");
        line = Regex.Replace(line, @"ok", "Ok");
        return line;
    }

    static string ReplaceTableMethods(string line)
    {
        line = Regex.Replace(line, @"Table\\.tableNew", "ble. TableNew");
        return line;
    }

    static string ReplaceColumnMethods(string line)
    {
        line = Regex.Replace(line, @"addCol", "AddCol");
        line = Regex.Replace(line, @"addRow", "AddRow");
        line = Regex.Replace(line, @"setString", "SetString");
        line = Regex.Replace(line, @"setTable", "SetTable");
        return line;
    }

    static string ReplaceOCalendar(string line)
    {
        line = Regex.Replace(line, @"OCalendar\\.today\\(\\)", "OCalendar.FormatDateInt(today)");
        return line;
    }

    static string ReplaceColTypeEnum(string line)
    {
        line = Regex.Replace(line, @"COL_TYPE_ENUM\\.COL_STRING", "COL_TYPE_ENUM.COL_STRING");
        line = Regex.Replace(line, @"COL_TYPE_ENUM\\.COL_TABLE", "COL_TYPE_ENUM.COL_TABLE");
        return line;
    }

    static string RemoveJavaSpecificSyntax(string line)
    {
        line = line.Replace("public ", "");
        line = line.Replace("private ", "");
        line = line.Replace("protected ", "");
        line = line.Replace("@Override", "");
        line = line.Replace("@PluginType(SCRIPT_TYPE_ENUM.PARAM_SCRIPT)", "");
        line = line.Replace("implements IScript", "");
        line = line.Replace("throws OException", "");
        line = line.Replace("{", "{");
        line = line.Replace("}", "}");
        return line;
    }

    static string FixMethodSignature(string line)
    {
        line = Regex.Replace(line, @"void execute \(IContainerContext context\)", "void Execute(IContainerContext context)");
        return line;
    }
} 