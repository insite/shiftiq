using Microsoft.Extensions.Configuration;

namespace Engine.Common
{
    public interface IEngineConfiguration
    {
        string AppName { get; }
        string AssemblyName { get; }
        IConfigurationRoot? Configuration { get; }
        Shift.Common.EngineSettings? Settings { get; }
    }
}
