using System;

namespace Shift.Common
{
    public class EnvironmentModel
    {
        private EnvironmentName _name;

        public EnvironmentName Name
        {
            get
            {
                return _name;
            }
            set
            {
                Initialize(value);
            }
        }

        public string Slug { get; set; }

        public EnvironmentModel(EnvironmentName name)
        {
            Initialize(name);
        }

        public EnvironmentModel(string name)
        {
            if (!Enum.TryParse<EnvironmentName>(name, true, out var environment))
                throw new ArgumentException($"The environment name {name} is not recognized.", nameof(name));

            Initialize(environment);
        }

        public string GetSubdomainPrefix()
        {
            switch (Name)
            {
                case EnvironmentName.Development:
                case EnvironmentName.Local:
                case EnvironmentName.Sandbox:
                    return Slug + "-";
                default:
                    return string.Empty;
            }
        }

        private void Initialize(EnvironmentName name)
        {
            _name = name;

            switch (Name)
            {
                case EnvironmentName.Production:
                    Slug = "prod";
                    break;

                case EnvironmentName.Sandbox:
                    Slug = "sandbox";
                    break;

                case EnvironmentName.Development:
                    Slug = "dev";
                    break;

                case EnvironmentName.Local:
                default:
                    Slug = "local";
                    break;
            }
        }

        public bool IsLocal()
            => Name == EnvironmentName.Local;

        /// <remarks>
        /// Pre-Production Environments refer to all environments used for developing, integrating, testing, and 
        /// validating software before its deployment to the Live (Production) environment. 
        /// </remarks>
        public bool IsPreProduction()
            => !IsProduction();

        public bool IsProduction()
            => Name == EnvironmentName.Production;

        public override string ToString()
        {
            return _name.ToString();
        }

        /// <summary>
        /// Returns the name of the Bootstrap contextual color for the environment.
        /// </summary>
        /// <remarks>
        /// Production = success
        /// Sandbox = warning
        /// Development = danger
        /// Local = info
        /// </remarks>
        public string Color
        {
            get
            {
                if (Name == EnvironmentName.Production)
                    return "success";

                if (Name == EnvironmentName.Sandbox)
                    return "warning";

                if (Name == EnvironmentName.Development)
                    return "danger";

                return "info";
            }
        }

        public string Icon
        {
            get
            {
                switch (Name)
                {
                    case EnvironmentName.Production:
                        return "home";

                    case EnvironmentName.Sandbox:
                        return "presentation";

                    case EnvironmentName.Development:
                        return "robot";
                }

                return "robot";
            }
        }
    }
}