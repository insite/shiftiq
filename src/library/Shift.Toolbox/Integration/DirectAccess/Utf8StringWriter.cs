using System.IO;
using System.Text;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding => Encoding.UTF8;
    }
}