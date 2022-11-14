namespace Kadlet
{
    public struct SourceSpan
    {
        public SourceOffset Start;
        public SourceOffset End;

        public SourceSpan(SourceOffset start, SourceOffset end) {
            Start = start;
            End = end;
        }
    }
}