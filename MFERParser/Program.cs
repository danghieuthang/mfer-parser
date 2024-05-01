using MFERParser;
using System.Diagnostics;

string filePath = @"C:\Users\Dang Thang\Downloads\Holter.mwf";

MferParser parser = new MferParser();

Stopwatch stopwatch = Stopwatch.StartNew();

var result = parser.Parse(filePath);

stopwatch.Stop();

Console.WriteLine($"Parsing took {stopwatch.ElapsedMilliseconds} ms");