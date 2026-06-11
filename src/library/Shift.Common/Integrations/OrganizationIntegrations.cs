using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Shift.Common
{
    public class KeyValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [Serializable]
    public class BaseEndpoint
    {
        public EnvironmentName[] Environments { get; set; }
        public string HostName { get; set; }
        public string Endpoint { get; set; }

        public virtual KeyValue[] GetHeaders() => null;
        public virtual KeyValue[] GetParameters() => null;

        public bool IsEqual(BaseEndpoint other)
        {
            return HostName.NullIfEmpty() == other.HostName.NullIfEmpty()
                && Endpoint.NullIfEmpty() == other.Endpoint.NullIfEmpty()
                && Environments.Length == other.Environments.Length
                && Environments.Zip(other.Environments, (a, b) => a == b).All(x => x);
        }

        public static bool IsEqual(ICollection<BaseEndpoint> collection1, ICollection<BaseEndpoint> collection2)
        {
            return collection1.Count == collection2.Count
                && collection1.Zip(collection2, (a, b) => a.IsEqual(b)).All(x => x);
        }

        public BaseEndpoint Clone()
        {
            var clone = CreateClone();
            clone.Environments = Environments.EmptyIfNull().Select(x => x).ToArray();
            clone.HostName = HostName;
            clone.Endpoint = Endpoint;
            return clone;
        }

        protected virtual BaseEndpoint CreateClone() => new BamboraEndpoint();
    }

    [Serializable]
    public class BaseIntegration<T> where T : BaseEndpoint
    {
        [DefaultValue(true)]
        public bool Enabled { get; set; } = true;
        public T[] Endpoints { get; set; }

        public T Get(EnvironmentName env)
        {
            return Enabled ? Endpoints?.FirstOrDefault(x => x.Environments != null && x.Environments.Contains(env)) : null;
        }

        public bool IsShallowEqual(BaseIntegration<T> other)
        {
            return Enabled == other.Enabled;
        }

        public bool ShouldSerializeEndpoints() => Endpoints.IsNotEmpty();
    }

    [Serializable]
    public sealed class BamboraEndpoint : BaseEndpoint
    {
        public string Merchant { get; set; }
        public string Passcode { get; set; }

        public BamboraEndpoint()
        {
            HostName = "api.na.bambora.com";
            Endpoint = "v1";
        }

        public override KeyValue[] GetHeaders()
        {
            var bytes = Encoding.UTF8.GetBytes($"{Merchant}:{Passcode}");
            var base64 = Convert.ToBase64String(bytes);

            return new[]
            {
                new KeyValue { Name = "Authorization", Value = $"Passcode {base64}" }
            };
        }

        public bool IsEqual(BamboraEndpoint other)
        {
            return base.IsEqual(other)
                && Merchant.NullIfEmpty() == other.Merchant.NullIfEmpty()
                && Passcode.NullIfEmpty() == other.Passcode.NullIfEmpty();
        }

        public static bool IsEqual(ICollection<BamboraEndpoint> collection1, ICollection<BamboraEndpoint> collection2)
        {
            return collection1.Count == collection2.Count
                && collection1.Zip(collection2, (a, b) => a.IsEqual(b)).All(x => x);
        }

        protected override BaseEndpoint CreateClone()
        {
            return new BamboraEndpoint
            {
                Merchant = Merchant,
                Passcode = Passcode
            };
        }
    }

    [Serializable]
    public sealed class PrometricEndpoint
    {
        public string ClientCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsEqual(PrometricEndpoint other)
        {
            return ClientCode == other.ClientCode
                && UserName == other.UserName
                && Password == other.Password;
        }

        public PrometricEndpoint Clone()
        {
            return new PrometricEndpoint
            {
                ClientCode = ClientCode,
                UserName = UserName,
                Password = Password
            };
        }
    }

    [Serializable]
    public sealed class ScormCloudApiConfiguration
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public bool IsEqual(ScormCloudApiConfiguration other)
        {
            return UserName == other.UserName
                && Password == other.Password;
        }

        public ScormCloudApiConfiguration Clone()
        {
            return new ScormCloudApiConfiguration
            {
                UserName = UserName,
                Password = Password
            };
        }
    }

    [Serializable]
    public sealed class BamboraIntegration : BaseIntegration<BamboraEndpoint>
    {
        public BamboraIntegration()
        {
            Endpoints = new BamboraEndpoint[0];
        }

        public bool IsEqual(BamboraIntegration other)
        {
            return IsShallowEqual(other)
                && BamboraEndpoint.IsEqual(Endpoints, other.Endpoints);
        }

        public BamboraIntegration Clone()
        {
            return new BamboraIntegration
            {
                Enabled = Enabled,
                Endpoints = Endpoints.Select(x => (BamboraEndpoint)x.Clone()).ToArray()
            };
        }
    }

    [Serializable]
    public sealed class RecaptchaIntegration : BaseIntegration<BaseEndpoint>
    {
        public RecaptchaIntegration()
        {
            Endpoints = new BaseEndpoint[0];
        }

        public bool IsEqual(RecaptchaIntegration other)
        {
            return IsShallowEqual(other)
                && BaseEndpoint.IsEqual(Endpoints, other.Endpoints);
        }

        public RecaptchaIntegration Clone()
        {
            return new RecaptchaIntegration
            {
                Enabled = Enabled,
                Endpoints = Endpoints.Select(x => x.Clone()).ToArray()
            };
        }
    }

    [Serializable]
    public sealed class BCMailIntegration : BaseIntegration<BaseEndpoint>
    {
        public BCMailIntegration()
        {
            Endpoints = new BaseEndpoint[0];
        }

        public bool IsEqual(BCMailIntegration other)
        {
            return IsShallowEqual(other)
                && BaseEndpoint.IsEqual(Endpoints, other.Endpoints);
        }

        public BCMailIntegration Clone()
        {
            return new BCMailIntegration
            {
                Enabled = Enabled,
                Endpoints = Endpoints.Select(x => x.Clone()).ToArray()
            };
        }
    }

    [Serializable]
    public class OrganizationIntegrations
    {
        public BamboraIntegration Bambora { get; set; }
        public BCMailIntegration BCMail { get; set; }
        public RecaptchaIntegration Recaptcha { get; set; }
        public PrometricEndpoint Prometric { get; set; }
        public ScormCloudApiConfiguration ScormCloud { get; set; }

        public OrganizationIntegrations()
        {
            Bambora = new BamboraIntegration();
            BCMail = new BCMailIntegration();
            Recaptcha = new RecaptchaIntegration();
            Prometric = new PrometricEndpoint();
            ScormCloud = new ScormCloudApiConfiguration();
        }
    }
}