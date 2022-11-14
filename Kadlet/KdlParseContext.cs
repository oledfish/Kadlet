using System.IO;
using System.Text;

namespace Kadlet
{
    /// <summary>
    /// A class wrapping the <see cref="TextReader"> and the position in a file or string.
    /// </summary>
    public class KdlParseContext
    {
        public PushBackReader Reader;
        public SourceOffset Offset;
        public bool Invalid;
        private char[] _char;

        public KdlParseContext(TextReader reader) {
            Reader = new PushBackReader(reader, 2);
            Offset = new SourceOffset{ Line = 1, Position = 1, Offset = 0 };
            Invalid = false;

            _char = new char[1];
        }

        public string Abort() {
            Invalid = true;
            return $"\nAt line {Offset.Line}, in position {Offset.Position}.";
        }

        public int Read() {
            if (Invalid) {
                throw new KdlException("Can't operate within an invalid context.", null);
            }

            int c = Reader.Read();

            _char[0] = (char) c;
            uint bytes = (uint) Encoding.UTF8.GetByteCount(_char);

            Offset.Offset += bytes;

            if (Util.IsNewline(c)) {
                if (c == '\u000D' && Reader.Peek() == '\u000A') {
                    c = Reader.Read();
                    Offset.Offset++;
                }

                Offset.Position = 1;
                Offset.Line++;
            } else {
                Offset.Position++;
            }

            return c;
        }

        public int Peek() {
            if (Invalid) {
                throw new KdlException("Can't operate within an invalid context.", null);
            }

            return Reader.Peek();
        }

        public void Unread(int c) {
            if (Invalid) {
                throw new KdlException("Can't operate within an invalid context.", null);
            }

            if (Util.IsNewline(c)) {
                throw new KdlException("Attempted to Unread() a newline.", null);
            } else if (c == Util.EOF) {
                throw new KdlException("Attempted to Unread() EOF.", null);
            } else {
                _char[0] = (char)c;
                uint bytes = (uint) Encoding.UTF8.GetByteCount(_char);
                
                Offset.Position -= bytes;
                Offset.Offset -= bytes;
            }

            try {
                Reader.Unread(c);
            } catch (IOException) {
                throw new KdlException("Attempted to unread more than 2 characters in sequence", null);
            }
        }
    }

    public class PushBackReader 
    {
        private readonly TextReader _reader;
        private readonly int[] _queue;
        private int _position = 0;

        public PushBackReader(TextReader reader, int pushbackLimit) {
            _reader = reader;
            _queue = new int[pushbackLimit];
        }

        public int Peek() {
            if (_position > 0) {
                return _queue[_position - 1];
            }

            return _reader.Peek();
        }

        public int Read() {
            if (_position > 0) {
                return _queue[(_position--) - 1];
            }

            return _reader.Read();
        }

        public void Unread(int value) {
            if (_position >= _queue.Length) {
                throw new IOException("PushBackReader buffer length exceeded");
            }

            _queue[_position++] = value;
        }
    }
}