using System;
using System.Collections.Specialized;

namespace Shift.Sdk.UI
{
    public class DefineLikertScalesQueryString
    {
        private readonly NameValueCollection _query;

        public DefineLikertScalesQueryString(NameValueCollection query)
        {
            _query = query;
        }

        public Guid Survey => Guid.TryParse(_query["survey"], out var value) ? value : Guid.Empty;

        public Guid Question => Guid.TryParse(_query["question"], out var value) ? value : Guid.Empty;
    }
}