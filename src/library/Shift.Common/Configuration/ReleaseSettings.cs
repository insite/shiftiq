using System.Collections.Generic;

namespace Shift.Common
{
    public class ReleaseSettings
    {
        public string Brand { get; set; }
        public string Environment { get; set; }
        public string Partition { get; set; }
        public string Version { get; set; }

        public EnvironmentModel GetEnvironment()
            => new EnvironmentModel(Environment);

        public List<string> ConfigurationProviders { get; set; } = new List<string>();
    }
}
