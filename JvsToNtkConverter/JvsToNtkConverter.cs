using System;
using System.IO;
using System.Text.RegularExpressions;

class JvsToNtkConverter
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: JvsToNtkConverter.exe <inputJvsFile> <outputNtkFile>");
            return;
        }

        string inputPath = args[0];
        string outputPath = args[1];

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
                string ntkLine = ConvertJvsLineToNtk(line);
                writer.WriteLine(ntkLine);
            }
        }

        Console.WriteLine($"NTK file written to: {outputPath}");
    }

    static string ConvertJvsLineToNtk(string line)
    {
        // Basic mapping rules (expand as needed)
        string result = line;

        // Class and method renaming
        result = Regex.Replace(result, @"Ask\\.", "ConvertedJvsUtils.Ask.");
        result = Regex.Replace(result, @"Table\\.tableNew", "ble. TableNew");
        result = Regex.Replace(result, @"addCol", "AddCol");
        result = Regex.Replace(result, @"addRow", "AddRow");
        result = Regex.Replace(result, @"setString", "SetString");
        result = Regex.Replace(result, @"setTable", "SetTable");
        result = Regex.Replace(result, @"setTextEdit", "SetTextEdit");
        result = Regex.Replace(result, @"viewTable", "ViewTable");
        result = Regex.Replace(result, @"ok", "Ok");
        result = Regex.Replace(result, @"OCalendar\\.today\\(\\)", "OCalendar.FormatDateInt(today)");
        result = Regex.Replace(result, @"COL_TYPE_ENUM\\.COL_STRING", "COL_TYPE_ENUM.COL_STRING");
        result = Regex.Replace(result, @"COL_TYPE_ENUM\\.COL_TABLE", "COL_TYPE_ENUM.COL_TABLE");
        // Remove Java-specific syntax (optional, expand as needed)
        result = result.Replace(";", ";");
        result = result.Replace("public ", "");
        result = result.Replace("private ", "");
        result = result.Replace("protected ", "");
        result = result.Replace("@Override", "");
        result = result.Replace("@PluginType(SCRIPT_TYPE_ENUM.PARAM_SCRIPT)", "");
        result = result.Replace("implements IScript", "");
        result = result.Replace("throws OException", "");
        result = result.Replace("{", "{");
        result = result.Replace("}", "}");
        // Fix method signature (optional, expand as needed)
        result = Regex.Replace(result, @"void execute \(IContainerContext context\)", "void Execute(IContainerContext context)");
        // Add more rules as needed
        return result.TrimEnd();
    }
} 