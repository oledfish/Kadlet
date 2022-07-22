using System.IO;

namespace Kadlet
{
    /// <summary>
    /// A class wrapping the <see cref="TextReader"> and the position in a file or string.
    /// </summary>
    public class KdlParseContext
    {
        public TextReader Reader;
        public bool EscapeNext;
        public int LinePosition;
        public int Line;
        public bool Invalid;

        public KdlParseContext(TextReader reader) {
            Reader = reader;
            EscapeNext = false;
            LinePosition = 0;
            Line = 1;
            Invalid = false;
        }

        public string Abort() {
            Invalid = true;
            return $"\nAt line {Line}, in position {LinePosition}.";
        }

        public int Read() {
            if (Invalid) {
                throw new KdlException("Can't operate within an invalid context.", null);
            }

            int c = Reader.Read();

            if (Util.IsNewline(c)) {
                if (c == '\u000D' && Reader.Peek() == '\u000A') {
                    c = Reader.Read();
                }

                LinePosition = 0;
                Line++;
            } else {
                LinePosition++;
            }

            return c;
        }

        public int Peek() {
            if (Invalid) {
                throw new KdlException("Can't operate within an invalid context.", null);
            }

            return Reader.Peek();
        }
    }
}