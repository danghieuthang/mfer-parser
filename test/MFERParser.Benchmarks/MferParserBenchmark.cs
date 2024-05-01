using BenchmarkDotNet.Attributes;

namespace MFERParser.Benchmarks
{
    [MemoryDiagnoser]
    public class MferParserBenchmark
    {
        [Benchmark]
        public void Parse()
        {
            string currentProjectDirectory = Directory.GetCurrentDirectory();
            string solutionDirectory = Directory.GetParent(currentProjectDirectory).Parent.Parent.Parent.Parent.FullName;

            string filePath = Path.Combine(solutionDirectory, "assets", "sample.mwf");
            var mferParser = new MferParser();
            MferFile result = mferParser.Parse(filePath);
        }
    }
}
