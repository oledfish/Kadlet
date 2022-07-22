using System.IO;

namespace Kadlet
{
    public interface IKdlObject
    {
        void Write(TextWriter writer, KdlPrintOptions options);

        string ToKdlString(KdlPrintOptions options);
    }
}