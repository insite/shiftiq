using System;
using System.Net;
using System.Net.Http.Headers;

namespace Shift.Common
{
    public interface IApiRequestLogger
    {
        Guid Insert(Guid? userIdentifier, Guid? organizationIdentifier, HttpWebRequest request, string content);
        Guid Insert(Guid? userIdentifier, Guid? organizationIdentifier, HttpRequestHeaders headers, string requestUrl, string requestMethod, string content);

        void Update(Guid requestKey, IntegrationResponse response);
        void Update(Guid requestKey, string destination, Exception ex);

        void Error(string source, string error);
    }
}
