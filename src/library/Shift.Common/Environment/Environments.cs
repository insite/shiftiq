using System.Linq;

namespace Shift.Common
{
    public static class Environments
    {
        public static EnvironmentModel[] All { get; set; }

        public static string[] Slugs
            => All.Select(x => x.Slug).ToArray();

        public static string[] Names
            => All.Select(x => x.Name.ToString()).ToArray();

        static Environments()
        {
            All = new[]
            {
                new EnvironmentModel(EnvironmentName.Local),
                new EnvironmentModel(EnvironmentName.Development),
                new EnvironmentModel(EnvironmentName.Sandbox),
                new EnvironmentModel(EnvironmentName.Production)
            };
        }

        public static EnvironmentModel Production
            => All.Single(e => e.Name == EnvironmentName.Production);

        public static EnvironmentModel Sandbox
            => All.Single(e => e.Name == EnvironmentName.Sandbox);

        public static EnvironmentModel Development
            => All.Single(e => e.Name == EnvironmentName.Development);

        public static EnvironmentModel Local
            => All.Single(e => e.Name == EnvironmentName.Local);
    }
}