
using NUnit.Framework;

namespace MFERParser.Tests
{
    [TestFixture(Category = nameof(MferParser))]
    public class MferParserTests
    {
        private MferParser mferParser;

        [SetUp]
        public void Setup()
        {
            mferParser = new MferParser();
        }

        [Test]
        public void Parse_ValidFilePath_ReturnsMferFile()
        {
            // Arrange

            string currentProjectDirectory = Directory.GetCurrentDirectory();
            string solutionDirectory = Directory.GetParent(currentProjectDirectory).Parent.Parent.Parent.Parent.FullName;

            string filePath = Path.Combine(solutionDirectory, "assets", "sample.mwf");

            // Act
            MferFile result = mferParser.Parse(filePath);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Channel, Is.EqualTo(2));
        }

        [Test]
        public void Parse_InvalidFilePath_ReturnsNull()
        {
            // Arrange
            string filePath = "invalid.mfer";

            // Act
            MferFile result = mferParser.Parse(filePath);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}
