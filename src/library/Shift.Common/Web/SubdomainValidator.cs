namespace Shift.Common
{
    public class SubdomainValidator
    {
        public string ErrorPageUrl => "/ui/lobby/logs/403.aspx";

        public bool Validate(EnvironmentModel environment, string host)
        {
            var prefix = environment.GetSubdomainPrefix();

            if (environment.Name == EnvironmentName.Production && !HostContainsAnyPrefix(host))
                return true;

            return HostContainsSpecificPrefix(host, prefix);
        }

        private bool HostContainsAnyPrefix(string host)
        {
            var environments = Environments.All;

            foreach (var environment in environments)
            {
                var prefix = environment.GetSubdomainPrefix();
                if (HostContainsSpecificPrefix(host, prefix))
                    return true;
            }

            return false;
        }

        private bool HostContainsSpecificPrefix(string host, string prefix)
        {
            return !string.IsNullOrEmpty(host)
                && !string.IsNullOrEmpty(prefix)
                && host.StartsWith(prefix);
        }
    }
}
