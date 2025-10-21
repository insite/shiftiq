using System;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

using Newtonsoft.Json;

namespace InSite.Api.Settings
{
    public class ApiJsonFormatter : JsonMediaTypeFormatter
    {
        public ApiJsonFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
            SerializerSettings.Formatting = Formatting.Indented;
        }

        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}