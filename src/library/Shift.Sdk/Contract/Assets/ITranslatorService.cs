using System;

namespace Shift.Contract
{
    public interface ITranslatorService
    {
        string Translate(string source, string language, Guid organization);
    }
}