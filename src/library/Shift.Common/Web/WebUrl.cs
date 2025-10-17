using System;
using System.Collections.Specialized;
using System.Web;

namespace Shift.Common
{
    public class WebUrl
    {
        public string Path { get; set; }
        public string Hash { get; set; }
        public NameValueCollection QueryString { get; private set; }

        public WebUrl(string url)
        {
            if (url.IsEmpty())
                throw new ArgumentNullException(nameof(url));

            var queryIndex = url.IndexOf('?');
            var hashIndex = url.IndexOf("#");

            if (hashIndex == -1 && queryIndex == -1)
            {
                Init(url, string.Empty, string.Empty);
            }
            else if (hashIndex == -1)
            {
                Split(url, queryIndex, out var path, out var query);
                Init(path, query, string.Empty);
            }
            else if (queryIndex == -1)
            {
                Split(url, hashIndex, out var path, out var hash);
                Init(path, string.Empty, hash);
            }
            else
            {
                Split(url, queryIndex, out var path, out var query);
                Split(query, hashIndex - path.Length - 1, out query, out var hash);
                Init(path, query, hash);
            }
        }

        public WebUrl(string path, string query, string hash = null)
        {
            Init(path, query ?? string.Empty, hash ?? string.Empty);
        }

        private void Init(string path, string query, string hash)
        {
            Path = path;
            Hash = hash;
            QueryString = HttpUtility.ParseQueryString(query);
        }

        public WebUrl Copy()
        {
            return new WebUrl(Path, QueryString.ToString(), Hash);
        }

        public override string ToString()
        {
            var result = Path;

            if (QueryString.IsNotEmpty())
                result += "?" + QueryString;

            if (Hash.IsNotEmpty())
                result += "#" + Hash;

            return result;
        }

        private static void Split(string input, int index, out string output1, out string output2)
        {
            output1 = input.Substring(0, index);
            output2 = input.Substring(index + 1);
        }

        public static WebUrl TryCreate(string value) => value.IsEmpty() ? null : new WebUrl(value);
    }
}
