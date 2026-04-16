using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace Shift.Common
{
    public class MailgunServer
    {
        #region Classes

        private class InlineResourceInfo
        {
            public string ContentId { get; set; }
            public string Url { get; }
            public string Name { get; }
            public byte[] Data { get; }

            public InlineResourceInfo(string url, string name, byte[] data)
            {
                if (url.IsEmpty())
                    throw new ArgumentNullException("url", "URL is null");

                Url = url;
                Name = name.NullIfEmpty();
                Data = data.NullIfEmpty();
            }
        }

        private class InlineResourceCollection : IEnumerable<InlineResourceInfo>
        {
            public int Count => _items.Count;

            public bool IsEmpty => _items.Count == 0;

            private readonly List<InlineResourceInfo> _items = new List<InlineResourceInfo>();

            public InlineResourceInfo Get(string url) =>
                _items.Find(x => string.Equals(x.Url, url, StringComparison.OrdinalIgnoreCase));

            public void Add(InlineResourceInfo item)
            {
                if (item.Data != null)
                {
                    var name = StringHelper.Sanitize(Path.GetFileNameWithoutExtension(item.Name), '-') + Path.GetExtension(item.Name);
                    var cid = name;
                    var tries = 20;

                    while (_items.Find(x => x.ContentId.IsNotEmpty() && string.Equals(x.ContentId, cid, StringComparison.OrdinalIgnoreCase)) != null)
                    {
                        cid = RandomStringGenerator.Create(6) + "." + name;
                        tries -= 1;

                        if (tries == 0)
                            throw new InvalidOperationException("Can't generate unqiue CID");
                    }

                    item.ContentId = cid;
                }

                _items.Add(item);
            }

            public IEnumerator<InlineResourceInfo> GetEnumerator() => _items.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion

        #region Fields

        private readonly Mailgun _mailgun;
        private readonly MailgunServerSettings _settings;

        private static readonly Regex _inlineTagPattern;
        private static readonly Dictionary<string, Regex> _inlineAttributePatterns;
        private static readonly Regex _contentDispositionFilename;

        #endregion

        #region Properties

        private MailgunDomain GetDomain(string domain)
        {
            var d = _mailgun.Domains.FirstOrDefault(x => x.Domain == domain) ?? _mailgun.Domains.First();

            if (d == null)
                throw new InvalidOperationException("The application configuration settings for Mailgun are missing.");

            return d;
        }

        #endregion

        #region Constructors

        public MailgunServer(Mailgun mailgun, MailgunServerSettings settings)
        {
            _mailgun = mailgun;
            _settings = settings;
        }

        static MailgunServer()
        {
            var mapping = new (string tag, string attr)[]
            {
                ("audio", "src"),
                ("embed", "src"),
                ("iframe", "src"),
                ("img", "src"),
                ("input", "src"),
                ("script", "src"),
                ("source", "src"),
                ("track", "src"),
                ("video", "src"),
                ("body", "background"),
                ("link", "href")
            };
            var attrRegex = mapping.Select(x => x.attr).Distinct().ToDictionary(x => x, x => new Regex($"(?<Prefix>{x}=)(?:(?<Quote>\")(?<Value>[^\\>\"]+)\"|(?<Quote>')(?<Value>[^\\>\"]+)')", RegexOptions.IgnoreCase | RegexOptions.Compiled));
            var tagEnum = string.Join("|", mapping.Select(x => x.tag).Distinct());

            _inlineTagPattern = new Regex($"\\<(?<Tag>{tagEnum})[^\\>]+\\>", RegexOptions.IgnoreCase | RegexOptions.Compiled);
            _inlineAttributePatterns = mapping.ToDictionary(x => x.tag, x => attrRegex[x.attr], StringComparer.OrdinalIgnoreCase);
            _contentDispositionFilename = new Regex("(?:^|;)\\s*filename=(?:\"(?<Name>[^\\s;]+)\"|'(?<Name>[^\\s;]+)'|(?<Name>[^\\s;]+))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        }

        #endregion

        #region Email Methods

        /// <summary>
        /// Send an email message
        /// </summary>
        /// <param name="email">Pre-built email message with fields like from, to, subject, body</param>
        public MailgunStatus SendEmail(EmailDraft email, string tag, string type, bool isUnitTest = false)
        {
            if (email == null)
                return MailgunStatus.Reject($"{nameof(email)} parameter is null.");

            var forceSend = type.HasValue()
                ? _settings.ForcedTypes.Any(x => x.Equals(type, StringComparison.OrdinalIgnoreCase))
                : false;

            if (_settings.EmailOutboxDisabled && !isUnitTest && !forceSend)
                return MailgunStatus.Reject("Email outbox is disabled.");

            if (_settings.EmailOutboxFiltered && !forceSend)
            {
                email.RecipientListTo = EmailAddress.Filter(email.RecipientListTo, _settings.WhitelistDomains, _settings.WhitelistTesters);
                email.RecipientListCc = EmailAddress.Filter(email.RecipientListCc, _settings.WhitelistDomains, _settings.WhitelistTesters);
                email.RecipientListBcc = EmailAddress.Filter(email.RecipientListBcc, _settings.WhitelistDomains, _settings.WhitelistTesters);
            }

            if (email.RecipientListTo.Count == 0)
                return MailgunStatus.Reject("The email has no recipients.");

            return SendEmail(email, tag);
        }

        private MailgunStatus SendEmail(EmailDraft email, string tag)
        {
            if (email.SenderEmail.IsEmpty())
                return MailgunStatus.Reject("The sender's email address is not specified.");

            if (email.RecipientListTo.IsEmpty())
                return MailgunStatus.Reject("The email has no recipients.");

            if (email.ContentSubject.Default.IsEmpty())
                return MailgunStatus.Reject("The subject of the email is not specified.");

            var form = CreateContent(email, tag);

            var (response, responseContent) = Shift.Common.TaskRunner.RunSync(SendRequest, form, email.SystemMailbox);

            var result = response.IsSuccessStatusCode
                ? MailgunStatus.Queue()
                : MailgunStatus.Reject("Rejected by Mailgun");

            result.Data["code"] = response.StatusCode.ToString();

            if (response.IsSuccessStatusCode)
            {
                result.Data["statusId"] = ExtractMailgunStatusId(responseContent);
            }
            else
            {
                result.Data["reason"] = response.ReasonPhrase;
                result.Data["content"] = responseContent;
            }

            return result;
        }

        private string ExtractMailgunStatusId(string json)
        {
            if (json.IsEmpty())
                return null;

            try
            {
                var status = JToken.Parse(json);
                if (status.Type != JTokenType.Object)
                    return null;

                var id = status["id"]?.ToString();
                if (id.IsEmpty())
                    return null;

                // If the identifier is enclosed in angle brackes then trim the brackets.
                if ('<' == id.First() && id.Length > 3 && id.Last() == '>')
                    id = id.Substring(1, id.Length - 2);

                return id;
            }
            catch
            {
                return null;
            }
        }

        private async Task<(HttpResponseMessage, string)> SendRequest(MultipartFormDataContent content, string systemMailbox)
        {
            var address = new EmailAddress(systemMailbox);
            var domain = GetDomain(address.Domain);

            var url = _mailgun.ApiUrl;
            if (!url.EndsWith("/"))
                url += "/";

            url += $"{domain.Domain}/messages";

            var authorization = Convert.ToBase64String(Encoding.UTF8.GetBytes($"api:{domain.Token}"));

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Content = content;
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authorization);

            var response = await StaticHttpClient.Client.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            return (response, responseContent);
        }

        private MultipartFormDataContent CreateContent(EmailDraft email, string tag)
        {
            if (email.MailoutIdentifier == Guid.Empty)
                throw new InvalidOperationException("Mailout ID is undefined.");

            // An email sent using Mailgun is permitted only if the To recipient list contains one
            // and only one item.

            if (email.RecipientListTo == null || email.RecipientListTo.Count == 0)
                throw new InvalidOperationException("An empty recipient list is not permitted.");

            if (email.RecipientListTo.Count != 1)
                throw new InvalidOperationException($"The recipient list for an email sent using Mailgun is restricted to one item. This list contains {email.RecipientListTo.Count} items.");

            var to = email.RecipientListTo.Single();

            var form = new MultipartFormDataContent();

            var address = new EmailAddress(email.SystemMailbox);
            var domain = GetDomain(address.Domain);
            var (html, resources) = AddInlineResources(email.ContentBody.Default);

            AddParameter(form, "v:mailout-id", email.MailoutIdentifier.ToString().ToLowerInvariant());
            AddParameter(form, "v:sender-domain", address.Domain);
            AddParameter(form, "v:environment-domain", _settings.Domain);

            AddParameter(form, "from", $"{email.SenderName} <{email.SystemMailbox}>");

            if (email.SenderEmail != email.SystemMailbox)
            {
                AddParameter(form, "h:Sender", $"{email.SenderName} <{email.SenderEmail}>");
                AddParameter(form, "h:Reply-To", $"{email.SenderEmail}");
            }

            AddParameter(form, "to", to.Value);
            AddParameter(form, "subject", email.ContentSubject.Default);
            AddParameter(form, "html", html);

            if (email.RecipientListCc != null && email.RecipientListCc.Count > 0)
                AddParameter(form, "cc", StringHelper.Join(email.RecipientListCc.Select(x => x.Value).Distinct()));

            if (email.RecipientListBcc != null && email.RecipientListBcc.Count > 0)
                AddParameter(form, "bcc", StringHelper.Join(email.RecipientListBcc.Select(x => x.Value).Distinct()));

            if (!Calendar.IsEmpty(email.MailoutScheduled))
            {
                if (email.MailoutScheduled.Value.ToUniversalTime() >= DateTimeOffset.UtcNow)
                    AddParameter(form, "o:deliverytime", email.MailoutScheduled.Value.ToUniversalTime().ToString("ddd, dd MMM yyy HH':'mm':'ss '-0000'"));
            }

            if (tag.IsNotEmpty())
                AddParameter(form, "o:tag", tag);

            foreach (var resource in resources)
                AddFileInline(form, resource);

            foreach (var attachment in email.ContentAttachments.EmptyIfNull())
                AddFileAttachment(form, attachment);

            return form;
        }

        private static void AddParameter(MultipartFormDataContent form, string name, string value)
        {
            form.Add(new StringContent(value), name);
        }

        private static void AddFileInline(MultipartFormDataContent form, InlineResourceInfo resource)
        {
            form.Add(new ByteArrayContent(resource.Data, 0, resource.Data.Length), "inline", resource.ContentId);
        }

        private static void AddFileAttachment(MultipartFormDataContent form, string filePath)
        {
            if (!System.IO.File.Exists(filePath))
                return;

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            form.Add(new StreamContent(stream), "attachment", Path.GetFileName(filePath));
        }

        #endregion

        #region Helper Methods

        private static (string Html, InlineResourceInfo[] Resources) AddInlineResources(string html)
        {
            var resources = new InlineResourceCollection();
            var resultHtml = html;

            using (var webClient = new WebClient())
            {
                resultHtml = _inlineTagPattern.Replace(resultHtml, mTag =>
                {
                    var tag = mTag.Groups["Tag"].Value;
                    var pattern = _inlineAttributePatterns[tag];

                    return pattern.Replace(mTag.Value, mAttr =>
                    {
                        var quote = mAttr.Groups["Quote"].Value;
                        var value = mAttr.Groups["Value"].Value;

                        var info = resources.Get(value);
                        if (info == null)
                            resources.Add(info = DownloadInlineResource(webClient, value));

                        return info.Data == null
                            ? mAttr.Value
                            : mAttr.Groups["Prefix"].Value + quote + "cid:" + info.ContentId + quote;
                    });
                });
            }

            return (resultHtml, resources.Where(x => x.Data.IsNotEmpty()).ToArray());
        }

        private static InlineResourceInfo DownloadInlineResource(WebClient client, string url)
        {
            byte[] data = null;
            string name = null;

            if (!url.StartsWith("file://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    data = client.DownloadData(url);
                    name = data.IsNotEmpty() ? GetInlineResourceName(url, client.ResponseHeaders["Content-Disposition"]) : null;
                }
                catch (WebException)
                {
                    data = null;
                }
            }

            return new InlineResourceInfo(url, name, data);
        }

        private static string GetInlineResourceName(string url, string contentDisposition)
        {
            var result = string.Empty;

            if (contentDisposition.IsNotEmpty())
            {
                var fileNameMatch = _contentDispositionFilename.Match(contentDisposition);
                if (fileNameMatch.Success)
                    result = fileNameMatch.Groups["Name"].Value;
            }

            if (result.IsEmpty())
            {
                try
                {
                    var uri = new Uri(url);
                    var name = Path.GetFileName(uri.LocalPath);
                    if (Path.HasExtension(name))
                        result = name;
                }
                catch (UriFormatException)
                {

                }
            }

            if (result.IsEmpty())
                result = Guid.NewGuid().ToString("N");

            return result;
        }

        #endregion
    }
}
