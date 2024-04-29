using MFERParser;

string filePath = @"C:\Users\Dang Thang\Downloads\Holter2.mwf";

MferParser parser = new MferParser();

var result = parser.Parse(filePath);


Console.WriteLine(result.Wave);