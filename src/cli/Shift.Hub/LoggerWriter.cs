using System.Text;

namespace Shift.Hub;

internal class LoggerWriter(TextWriter originalOutWriter, bool isError) : TextWriter
{
    private readonly StringBuilder _buffer = new();

    public override Encoding Encoding => Encoding.UTF8;

    public override void Write(char value)
    {
        if (value != '\n')
        {
            _buffer.Append(value);
            return;
        }

        var text = _buffer.ToString().Trim('\r');
        var oldOut = Console.Out;

        Console.SetOut(originalOutWriter);

        try
        {
            if (isError)
                Logger.Instance.Error(text);
            else
                Logger.Instance.Information(text);
        }
        finally
        {
            Console.SetOut(oldOut);
        }

        _buffer.Clear();
    }
}
