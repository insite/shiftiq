using System;
using System.Collections.Specialized;
using System.Web.Routing;

namespace InSite.UI.Portal.Workflow.Forms.Models
{
    public class SubmissionSessionQueryString
    {
        private readonly NameValueCollection _query;
        private readonly RouteData _routes;

        public SubmissionSessionQueryString(RouteData routes, NameValueCollection query)
        {
            _routes = routes;
            _query = query;
        }

        public bool Debug
        {
            get
            {
                if (bool.TryParse(_query["debug"], out bool result))
                {
                    CurrentSessionState.EnableDebugMode = result;
                }

                return CurrentSessionState.EnableDebugMode;
            }
        }

        public int? FormAsset => GetNumber("form");

        public Guid? FormIdentifier => GetIdentifier("form");

        public Guid? User => GetIdentifier("user");

        public string Verb => GetString("verb");

        public Guid Session
        {
            get
            {
                if (Guid.TryParse(_query["session"], out Guid result))
                {
                    return result;
                }

                return Guid.Empty;
            }
        }

        public int PageNumber
        {
            get
            {
                if (int.TryParse(_query["page"], out int result))
                {
                    return result;
                }

                return 0;
            }
        }

        public Guid? Question =>
            Guid.TryParse(_query["question"], out var result) ? result : (Guid?)null;

        private Guid? GetIdentifier(string name)
        {
            var id = _routes.Values[name] as string;

            if (id == null)
                id = _query[name];

            if (id != null && Guid.TryParse(id, out Guid i))
                return i;

            return null;
        }

        private int? GetNumber(string name)
        {
            var id = _routes.Values[name] as string;

            if (id == null)
                id = _query[name];

            if (id != null && int.TryParse(id, out int i))
                return i;

            return null;
        }

        private string GetString(string name)
        {
            var id = _routes.Values[name] as string;

            if (id == null)
                id = _query[name];

            if (string.IsNullOrEmpty(name))
                return null;

            return id;
        }
    }
}
