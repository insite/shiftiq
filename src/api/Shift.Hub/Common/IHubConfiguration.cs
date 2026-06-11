using Microsoft.Extensions.Configuration;

namespace Shift.Hub
{
    public interface IHubConfiguration
    {
        string AppName { get; }
        string AssemblyName { get; }
        IConfigurationRoot? Configuration { get; }
        Shift.Common.EngineSettings? Settings { get; }
    }
}
