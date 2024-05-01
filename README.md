# MFERParser

MFER Parser is a .NET library for parsing MFER (Medical waveform Format Encoding Rules) files.

## Description

MFERParser is a parser library that allows you to parse MFER (My File Extension Rules) files. MFER files are used to define custom rules for file extensions.

## Features

-	Parse MFER files and extract relevant data.
-	Handle different types of MFER files.
-	Provide detailed error messages for malformed MFER files.
-	Unit tests to ensure the correctness of the MFER file parsing.

## Installation

To use MFERParser in your project, you can install it via NuGet. Run the following command in the Package Manager Console:
```Console
Install-Package danghieuthang.mferparser
```
Or via the .NET Core command line interface:
```Console
dotnet add package danghieuthang.mferparser
```
Alternatively, you can manually download the library from the [GitHub repository](https://github.com/danghieuthang/mfer-parser) and reference it in your project.

## Usage

To parse an MFER file and extract the rules, you can use the following code:

```csharp
var parser = new MferParser();
var mferFile = parser.Parse("path/to/your/file.mwf");
// Now you can access the data in the mferFile object
```

## Contributing

Contributions are welcome! If you find any issues or have suggestions for improvements, please open an issue or submit a pull request on the [GitHub repository](https://github.com/danghieuthang/mfer-parser).

## License

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for more information.
