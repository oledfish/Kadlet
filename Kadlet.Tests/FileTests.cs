#pragma warning disable CS8618

using NUnit.Framework;

namespace Kadlet.Tests 
{
    public class FileTests 
    {
        static HashSet<string> IgnoredTests = new HashSet<string>(new string[] {
            "sci_notation_large",
            "sci_notation_small"
        });

        static object[] TestCases = Directory
            .GetFiles("test_cases/input")
            .Select(filename => new object[] { filename })
            .Where(filename => !IgnoredTests.Contains(Path.GetFileNameWithoutExtension((string) filename[0])))
            .ToArray();

        static object[] TypeTests = Directory
            .GetFiles("type_tests/input")
            .Select(filename => new object[] { filename })
            .Where(filename => !IgnoredTests.Contains(Path.GetFileNameWithoutExtension((string)filename[0])))
            .ToArray();

        static object[] EofTests = Directory
            .GetFiles("eof_tests/input")
            .Select(filename => new object[] { filename })
            .Where(filename => !IgnoredTests.Contains(Path.GetFileNameWithoutExtension((string)filename[0])))
            .ToArray();

        private KdlReader _reader;

        [SetUp]
        public void Setup() {
            _reader = new KdlReader();
        }

        [Test]
        [TestCaseSource(nameof(TestCases))]
        [TestCaseSource(nameof(TypeTests))]
        [TestCaseSource(nameof(EofTests))]
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

                try {
                    document = _reader.Parse(fs);
                } catch (Exception e) {
                    if (expected == null) {
                        Assert.Pass();
                    } else {
                        throw e;
                    }
                }

                if (expected == null) {
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