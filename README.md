# Java to C# Converter

A C# command-line tool for converting Java files to C# format.

## Features

- Converts Java files to C# format
- Supports Java source files (.java)
- PowerShell integration with `ConvertInputJVS` command
- Automatic input/output folder management
- Batch conversion support
- Converts Java types to C# equivalents
- Handles package declarations and imports

## Prerequisites

- .NET 6.0 SDK or later
- PowerShell 5.1 or later

## Setup Instructions

### 1. Build the Project

```powershell
# Build the project
dotnet build --configuration Release

# Publish as single executable
dotnet publish --configuration Release --runtime win-x64 --self-contained true
```

### 2. Set up PowerShell Integration

To use the `ConvertInputJVS` command in PowerShell:

```powershell
# Import the PowerShell script
. .\ConvertInputJVS.ps1

# Or add to your PowerShell profile for permanent access
# Add this line to your PowerShell profile:
# . "C:\path\to\your\project\ConvertInputJVS.ps1"
```

## Usage

### Using PowerShell Command

```powershell
# Convert all Java files in the input folder
ConvertInputJVS

# Convert a specific Java file
ConvertInputJVS "Sample.java"
```

### Using Direct Command Line

```powershell
# Convert a specific Java file
.\JVSConverter.exe "input\Sample.java" "output\Sample.cs"

# Or if published as single file
.\bin\Release\net6.0\win-x64\publish\JVSConverter.exe "input\Sample.java" "output\Sample.cs"
```

## File Structure

```
JVS to NTK/
├── input/                 # Place your Java files here
│   └── Sample.java       # Sample input file
├── output/               # Converted C# files will be saved here
├── JVSConverter.cs       # Main converter class
├── Program.cs            # Entry point
├── JVSConverter.csproj   # Project file
├── ConvertInputJVS.ps1   # PowerShell integration script
├── build.bat            # Build script
└── README.md            # This file
```

## Supported File Formats

### Input Formats
- `.java` - Java source files

### Output Format
- `.cs` - C# source files

## Java to C# Conversion Features

### Type Conversions
- `String` → `string`
- `Integer` → `int`
- `Boolean` → `bool`
- `Double` → `double`
- `Float` → `float`
- `Long` → `long`
- `Short` → `short`
- `Byte` → `byte`
- `Character` → `char`

### Structure Conversions
- `package` declarations → `namespace`
- `import` statements → `using` statements
- Java classes → C# classes
- Java methods → C# methods (with TODO placeholders for implementation)

### Example Conversion

**Input Java file:**
```java
package com.example;

import java.util.List;

public class Sample {
    private String name;
    private Integer age;
    
    public Sample(String name, Integer age) {
        this.name = name;
        this.age = age;
    }
    
    public String getName() {
        return name;
    }
}
```

**Output C# file:**
```csharp
using java.util.List;

namespace com_example
{
    public class Sample
    {
        private string name;
        private int age;
        
        public Sample(string name, int age)
        {
            // TODO: Implement method body
        }
        
        public string getName()
        {
            // TODO: Implement method body
        }
    }
}
```

## Customization

You can customize the conversion logic by modifying the following methods in `JVSConverter.cs`:

- `ParseJavaContent()` - Adjust Java parsing logic
- `ConvertToCSharp()` - Modify C# output format
- `ConvertJavaMethodToCSharp()` - Customize method conversion
- `ConvertJavaFieldToCSharp()` - Customize field conversion
- `ValidateInputFile()` - Add custom validation rules

## Troubleshooting

### Common Issues

1. **"JVSConverter.exe not found"**
   - Make sure you've built the project first
   - Run `dotnet build --configuration Release`

2. **"No Java files found"**
   - Check that your input files have `.java` extension
   - Ensure files are placed in the `input` folder

3. **PowerShell execution policy error**
   - Run: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`

4. **.NET SDK not found**
   - Install .NET 6.0 SDK from: https://dotnet.microsoft.com/download
   - Restart your command prompt after installation

## Limitations

- Method bodies are converted to TODO placeholders
- Complex Java constructs may need manual adjustment
- Some Java-specific features may not have direct C# equivalents
- The converter focuses on basic structure and type conversions

## License

This project is provided as-is for educational and development purposes. 