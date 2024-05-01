using BenchmarkDotNet.Running;

namespace MFERParser.Benchmarks
{

    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MferParserBenchmark>();
        }
    }

}
