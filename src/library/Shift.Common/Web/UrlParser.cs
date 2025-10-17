using System;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Shift.Common
{
    public class UrlParser
    {
        #region Enums

        public enum Convention { One, Two };
        public enum UrlType { Absolute, Relative };

        #endregion

        #region Classes

        public class Url
        {
            public bool IsValid { get; set; }

            public EnvironmentName Environment { get; set; }
            public Convention Convention { get; set; }
            public UrlType Type { get; set; }

            public string Action { get; set; }
            public string Domain { get; set; }
            public string Organization { get; set; }

            public string Query { get; set; }
            public string Resource { get; set; }

            public NameValueCollection Parameters { get; set; }

            public bool Exists(string name)
            {
                if (Parameters.AllKeys.Any(k => StringHelper.Equals(name, k)))
                    return true;
                return false;
            }

            public string Get(string name)
            {
                if (Parameters.AllKeys.Any(k => StringHelper.Equals(name, k)))
                    return Parameters[name];
                return null;
            }

            public Guid? GetGuid(string name)
            {
                if (Parameters != null && Parameters.AllKeys.Any(k => StringHelper.Equals(name, k)))
                    return Guid.TryParse(Parameters[name], out var id) ? id : (Guid?)null;
                return null;
            }

            public void AddParameter(string name, string value)
            {
                if (!Exists(name))
                    Parameters.Add(name, value);
                else
                    Parameters[name] = value;
            }

            public string Relative
            {
                get
                {
                    var url = Resource;

                    if (Parameters.IsNotEmpty())
                        url += $"?{Parameters}";

                    return url;
                }
            }
        }

        #endregion

        private Url _output;

        public Url Parse(string url)
        {
            _output = new Url();

            var parts = url.Split(new char[] { '?', });
            if (parts.Length == 2)
            {
                _output.Resource = parts[0];
                _output.Query = parts[1];
                _output.Parameters = System.Web.HttpUtility.ParseQueryString(_output.Query);
            }
            else
            {
                _output.Resource = parts[0];
            }

            _output.IsValid = ParseA(_output.Resource) || ParseB(_output.Resource)
                           || ParseC(_output.Resource) || ParseD(_output.Resource)
                           || ParseE(_output.Resource) || ParseF(_output.Resource);

            return _output;
        }

        private bool ParseA(string url)
        {
            const string TestPattern1 = @"^https://(local|dev|sandbox|prod)-(.+)\.(insite\.com)/(.+)$";
            var match = Regex.Match(url, TestPattern1);

            if (match.Success)
            {
                _output.Convention = Convention.One;
                _output.Type = UrlType.Absolute;

                switch (match.Groups[1].Value)
                {
                    case "local":
                        _output.Environment = EnvironmentName.Local;
                        break;
                    case "dev":
                        _output.Environment = EnvironmentName.Development;
                        break;
                    case "sandbox":
                        _output.Environment = EnvironmentName.Sandbox;
                        break;
                    default:
                        _output.Environment = EnvironmentName.Production;
                        break;
                }

                _output.Organization = match.Groups[2].Value;
                _output.Domain = match.Groups[3].Value;
                _output.Action = match.Groups[4].Value;

                return true;
            }

            return false;
        }

        private bool ParseB(string url)
        {
            const string ProdPattern1 = @"^https://((?!local|dev|sandbox|prod).+)\.(insite\.com)/(.+)$";
            var match = Regex.Match(url, ProdPattern1);

            if (match.Success)
            {
                _output.Convention = Convention.One;
                _output.Type = UrlType.Absolute;

                _output.Environment = EnvironmentName.Production;

                _output.Organization = match.Groups[1].Value;
                _output.Domain = match.Groups[2].Value;
                _output.Action = match.Groups[3].Value;

                return true;
            }

            return false;
        }

        private bool ParseC(string url)
        {
            const string TestPattern2 = @"^https://(local|dev|sandbox|prod)\.(insite\.com)/([^/]+)/(.+)$";
            var match = Regex.Match(url, TestPattern2);

            if (match.Success)
            {
                _output.Convention = Convention.Two;
                _output.Type = UrlType.Absolute;

                switch (match.Groups[1].Value)
                {
                    case "local":
                        _output.Environment = EnvironmentName.Local;
                        break;
                    case "dev":
                        _output.Environment = EnvironmentName.Development;
                        break;
                    case "sandbox":
                        _output.Environment = EnvironmentName.Sandbox;
                        break;
                    case "prod":
                        _output.Environment = EnvironmentName.Production;
                        break;
                }

                _output.Domain = match.Groups[2].Value;
                _output.Organization = match.Groups[3].Value;
                _output.Action = match.Groups[4].Value;

                return true;
            }

            return false;
        }

        private bool ParseD(string url)
        {
            const string ProdPattern2 = @"^https://(insite\.com)/([^/]+)/(.+)$";
            var match = Regex.Match(url, ProdPattern2);

            if (match.Success)
            {
                _output.Convention = Convention.Two;
                _output.Type = UrlType.Absolute;

                _output.Environment = EnvironmentName.Production;

                _output.Domain = match.Groups[1].Value;
                _output.Organization = match.Groups[2].Value;
                _output.Action = match.Groups[3].Value;

                return true;
            }

            return false;
        }

        private bool ParseE(string url)
        {
            const string Pattern3 = @"^/(admin|api|cmds|content|custom|files|hub|mvc|ui|uploads|x|x-files)/(.+)$";
            var match = Regex.Match(url, Pattern3);

            if (match.Success)
            {
                _output.Convention = Convention.One;
                _output.Type = UrlType.Relative;

                _output.Action = match.Groups[1].Value + "/" + match.Groups[2].Value;

                return true;
            }

            return false;
        }

        private bool ParseF(string url)
        {
            const string Pattern4 = @"^/([^/]+)/(.+)$";
            var match = Regex.Match(url, Pattern4);

            if (match.Success)
            {
                _output.Convention = Convention.Two;
                _output.Type = UrlType.Relative;

                _output.Organization = match.Groups[1].Value;
                _output.Action = match.Groups[2].Value;

                return true;
            }

            return false;
        }

        public static string BuildRelativeUrl(string rawUrl, string parameterName, string parameterValue)
        {
            var parser = new UrlParser();
            var url = parser.Parse(rawUrl);
            url.AddParameter(parameterName, parameterValue);
            return url.Relative;
        }

        public static string BuildRelativeUrl(string rawUrl, string parameter1Name, string parameter1Value, string parameter2Name, string parameter2Value)
        {
            var parser = new UrlParser();
            var url = parser.Parse(rawUrl);
            url.AddParameter(parameter1Name, parameter1Value);
            url.AddParameter(parameter2Name, parameter2Value);
            return url.Relative;
        }

        public static string GetAction(string url)
        {
            var parser = new UrlParser();
            return parser.Parse(url).Action;
        }
    }
}
