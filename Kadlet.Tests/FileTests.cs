#pragma warning disable CS8618

using NUnit.Framework;

namespace Kadlet.Tests 
{
    public class FileTests 
    {
        static object[] TestCases = Directory
            .GetFiles("test_cases/input")
            .Select(filename => new object[] { filename })
            .ToArray();

        static object[] AdditionalTests = Directory
            .GetFiles("additional_tests/input")
            .Select(filename => new object[] { filename })
            .ToArray();

        private KdlReader _reader;

        [SetUp]
        public void Setup() {
            _reader = new KdlReader();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [TestCaseSource(nameof(AdditionalTests))]
        public void FileTest(string filename) 
        {
            string? expected = null;
            string expectedFile = filename.Replace("input", "expected_kdl");

            if (File.Exists(expectedFile)) {
                expected = File.ReadAllText(expectedFile);
            };

            using (FileStream fs = File.OpenRead(filename))
            {
                KdlDocument? document = null;

                if (expected != null) {
                    document = _reader.Parse(fs);
                } else {
                    try {
                        document = _reader.Parse(fs);
                    } catch (Exception) {
                        Assert.Pass();
                    }

                    Assert.Fail("Test expected to fail but succeeded.");
                }

                using (StringWriter sw = new StringWriter()) {
                    KdlDocument doc = (KdlDocument) document;
                    doc.Write(sw, KdlPrintOptions.Testing);

                    string output = sw.ToString();
                    Assert.That(output, Is.EqualTo(expected));
                }
            }
        }
    }
}