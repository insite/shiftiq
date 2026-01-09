using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;

using Shift.Common.Timeline.Commands;

using InSite.Application.Messages.Read;
using InSite.Application.Messages.Write;
using InSite.Application.Surveys.Write;
using InSite.Domain.Surveys.Forms;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

using LinkEntity = InSite.Application.Messages.Read.QLink;

namespace InSite.Persistence
{
    public static class MessageHelper
    {
        #region Classes

        private class VMessageJsonBase
        {
            public string MessageAttachments { get; set; }
            public string ApplicationChangeType { get; set; }
            public string MessageType { get; set; }

            public Sender Sender { get; set; }

            public string SystemMailbox { get; set; }
            public bool IsDisabled { get; set; }
        }

        private class VMessageJson : VMessageJsonBase
        {
            public string ContentHtml { get; set; }
            public string ContentText { get; set; }
            public string MessageTitle { get; set; }
        }

        private class VMessageJsonV2 : VMessageJsonBase
        {
            public ContentContainer Content { get; set; }
        }

        private class Sender
        {
            public string SenderEmail { get; set; }
            public string SenderName { get; set; }
            public string SenderNickname { get; set; }
        }

        public class MessageInfo
        {
            public string Subject { get; set; }
            public string Body { get; set; }
        }

        #endregion

        #region Variables

        public static Action<string> LogToSentry { get; set; }

        #endregion

        #region Constants

        private static readonly Regex RegexVariablePattern = new Regex("\\$(?<FullName>(?<VarName>[a-zA-Z0-9]+)(?::(?<VarLang>ar|en|fr|de|it|ja|pt|ru|es|uk))?)", RegexOptions.Compiled);
        private static readonly Regex RegexTemplatePlaceholderPattern = new Regex("(?:\\<p\\>\\$(?<Name>[a-zA-Z0-9-]+)(?:#(?<Lang>[a-zA-Z]+))?\\</p\\>|\\$(?<Name>[a-zA-Z0-9-]+)(?:#(?<Lang>[a-zA-Z]+))?)", RegexOptions.Compiled);

        public static readonly IDictionary<string, IDictionary<string, string>> Templates = new Dictionary<string, IDictionary<string, string>>
        {
            {
                "Survey-Link",
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["en"] = $@"
<table style=""border-collapse: collapse;"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""380"">
    <tbody>
        <tr>
            <td>
                <table style=""border-collapse: collapse;"" align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                    <tbody>
                        <tr>
                            <td style=""font-family: verdana,sans-serif; font-size: 28px; font-weight: bold; line-height: 34px;"" align=""center"" width=""100%"">
                                <table style=""border-collapse: collapse;"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""160"">
                                    <tbody>
                                        <tr>
                                            <td style=""padding: 12px 12px 12px 12px; text-align: center;"" align=""center"" bgcolor=""#ffa500"" width=""100%"">
                                                <a href=""${MessageVariable.AppUrl}/$SurveyPath"" style=""color: #FFFFFF; font-family: verdana,sans-serif; font-size: 16px; font-weight: bold; text-decoration: none;"">Begin Survey</a>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style=""color: #888; font-family: verdana,sans-serif; font-size: 12px; font-style: italic; line-height: 22px;"" align=""left"" width=""100%"">If the &laquo; Begin Survey &raquo; button does not work to start the survey from your email program, then please copy the URL below and paste it into your web browser's address window.</td>
                        </tr>
                        <tr>
                            <td style=""color: #ffa500; font-family: verdana,sans-serif; font-size: 12px; line-height: 22px;"" align=""left"" width=""100%"">
                                <a style=""color: #ffa500;"" href=""${MessageVariable.AppUrl}/$SurveyPath"">${MessageVariable.AppUrl}/$SurveyPath</a>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
",
                    ["fr"] = $@"
<table style=""border-collapse: collapse;"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""380"">
    <tbody>
        <tr>
            <td>
                <table style=""border-collapse: collapse;"" align=""left"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"">
                    <tbody>
                        <tr>
                            <td style=""font-family: verdana,sans-serif; font-size: 28px; font-weight: bold; line-height: 34px;"" align=""center"" width=""100%"">
                                <table style=""border-collapse: collapse;"" align=""center"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""160"">
                                    <tbody>
                                        <tr>
                                            <td style=""padding: 12px 12px 12px 12px; text-align: center;"" align=""center"" bgcolor=""#ffa500"" width=""100%"">
                                                <a href=""${MessageVariable.AppUrl}/$SurveyPath"" style=""color: #FFFFFF; font-family: verdana,sans-serif; font-size: 16px; font-weight: bold; text-decoration: none;"">Commencer le sondage</a>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td style=""color: #888; font-family: verdana,sans-serif; font-size: 12px; font-style: italic; line-height: 22px;"" align=""left"" width=""100%"">Si le bouton &laquo; Commencer le sondage &raquo; ne démarre pas, copiez l'adresse ci-après et collez-la dans la fenêtre adresse de votre fureteur web.</td>
                        </tr>
                        <tr>
                            <td style=""color: #ffa500; font-family: verdana,sans-serif; font-size: 12px; line-height: 22px;"" align=""left"" width=""100%"">
                                <a style=""color: #ffa500;"" href=""${MessageVariable.AppUrl}/$SurveyPath"">${MessageVariable.AppUrl}/$SurveyPath</a>
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
",
                }
            },

            {
                "Unsubscribe-Link",
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["en"] = $"<a href='${MessageVariable.AppUrl}/ui/lobby/subscribe?user=$RecipientIdentifier'>Manage Subscriptions</a>"
                }
            },

            {
                "Social-Media-Links",
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["en"] = $@"
<!-- begin: social-media-links -->

<table style=""background-color:#8D9093;padding:10px; width:100%""><tr><td>
<table cellpadding=""5"" style=""background-color:#8D9093"">
<tr>

    <!-- begin: facebook-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""Facebook"" src=""${MessageVariable.AppUrl}/images/mail/facebook-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: facebook-social-link -->

    <!-- begin: twitter-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""Twitter"" src=""${MessageVariable.AppUrl}/images/mail/twitter-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: twitter-social-link -->

    <!-- begin: linkedin-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""LinkedIn"" src=""${MessageVariable.AppUrl}/images/mail/linkedin-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: linkedin-social-link -->

    <!-- begin: instagram-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""Instagram"" src=""${MessageVariable.AppUrl}/images/mail/instagram-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: instagram-social-link -->

    <!-- begin: youtube-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""Youtube"" src=""${MessageVariable.AppUrl}/images/mail/youtube-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: youtube-social-link -->

    <!-- begin: other-social-link -->
    <td>
    <a href=""{{Social-Media-URL}}"" style=""display:block; float:left; line-height:1; margin:5px; color:#ffffff;"">
        <img alt=""Other"" src=""${MessageVariable.AppUrl}/images/mail/other-link-white.png"" width=""40"" style=""width:40px;"" />
    </a>
    </td>
    <!-- end: other-social-link -->
    </tr>
    </table></td></tr></table>
    <div style=""clear:both;""></div>

</div>

<!-- end: social-media-links -->
"
                }
            }
        };

        #endregion

        #region Methods (content rendering)

        public static MessageInfo BuildMessage(EmailDraft draft, string language)
        {
            var result = new MessageInfo();

            var recipientPlaceholders = draft.Recipients
                .GetVariableNames()
                .Distinct()
                .ToDictionary(x => x, x => x, StringComparer.OrdinalIgnoreCase);

            result.Subject = ReplaceVariables(draft.ContentVariables, draft.ContentSubject.Get(language));

            result.Body = draft.ContentBody.Get(language);
            result.Body = ReplaceVariables(draft.ContentVariables, result.Body);
            result.Body = CreateHtmlBody(result.Subject, result.Body, false);
            result.Body = ReplaceTemplates(result.Body, language);
            result.Body = ReplacePlaceholdersForSmarterMail(draft.OrganizationIdentifier, draft.SenderIdentifier, draft.SurveyNumber, result.Body);

            if (draft.MessageIdentifier != null
                && draft.ContentVariables.IsNotEmpty()
                && TryGetVariableValue(draft.ContentVariables, MessageVariable.AppUrl, out var _)
                )
            {
                result.Body = ReplaceLinks(draft.MessageIdentifier.Value, "$" + MessageVariable.AppUrl, result.Body);
            }

            result.Body = ReplaceVariables(draft.ContentVariables, result.Body);
            result.Body = HtmlBuilder.MoveCssInline(result.Body);

            return result;
        }

        public static string BuildPreviewHtml(Guid organizationId, Guid senderId, int? surveyFormAsset, string text)
        {
            var variables = new MessageVariableList().ToDictionary();

            var html = ReplaceVariables(variables, text);
            html = CreateHtmlBody(null, html, false);
            html = ReplaceTemplates(html, MultilingualString.DefaultLanguage);
            html = ReplacePlaceholdersForSmarterMail(organizationId, senderId, surveyFormAsset, html);
            html = ReplaceVariables(variables, html);
            html = HtmlBuilder.MoveCssInline(html);
            return html;
        }

        public static string ReplacePlaceholdersForMailgun(Guid organizationId, Guid senderId, int? surveyNumber, string body, EmailVariables envelope)
        {
            if (envelope == null)
                return body;

            var items = envelope.GetItems();

            foreach (var variable in items)
            {
                var value = variable.Value;

                if (StringHelper.EndsWith(variable.Key, "Markdown"))
                    value = CreateHtmlSnippet(variable.Value, true);

                ReplaceBodyPlaceholder("$" + variable.Key, value);
            }

            var organization = OrganizationSearch.Select(organizationId) ?? throw new ApplicationError($"Organization not found: {organizationId}");

            ReplaceBodyPlaceholder("$RecipientName", envelope.RecipientName);
            ReplaceBodyPlaceholder("$RecipientEmail", envelope.RecipientEmail);
            ReplaceBodyPlaceholder("$RecipientIdentifier", envelope.RecipientIdentifier.ToString());

            ReplaceBodyPlaceholder("$RecipientFirstName", envelope.RecipientNameFirst);
            ReplaceBodyPlaceholder("$RecipientLastName", envelope.RecipientNameLast);
            ReplaceBodyPlaceholder("$RecipientPersonCode", envelope.RecipientCode);

            ReplaceBodyPlaceholder("$SurveyPath", $"surveys/{surveyNumber ?? 0}/{envelope.RecipientIdentifier}");

            body = ReplacePlaceholder(body, "social-media-links", content =>
            {
                var hasContent = false;

                content = ReplacePlaceholder(
                    content,
                    "facebook-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.Facebook));

                content = ReplacePlaceholder(
                    content,
                    "twitter-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.Twitter));

                content = ReplacePlaceholder(
                    content,
                    "linkedin-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.LinkedIn));

                content = ReplacePlaceholder(
                    content,
                    "instagram-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.Instagram));

                content = ReplacePlaceholder(
                    content,
                    "youtube-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.YouTube));

                content = ReplacePlaceholder(
                    content,
                    "other-social-link",
                    subContent => ProcessSocialLink(subContent, organization.PlatformCustomization.TenantUrl.Other));

                return hasContent ? content : string.Empty;

                string ProcessSocialLink(string input, string url)
                {
                    if (string.IsNullOrEmpty(url))
                        return string.Empty;

                    hasContent = true;

                    return input.Replace("{Social-Media-URL}", url);
                }
            });

            return body;

            void ReplaceBodyPlaceholder(string placeholder, string value)
            {
                var encoded = HttpUtility.UrlEncode(placeholder);
                var pattern = encoded == placeholder
                    ? Regex.Escape(placeholder)
                    : $"({Regex.Escape(placeholder)}|{Regex.Escape(encoded)})";
                body = Regex.Replace(body, pattern + "\\b", value.EmptyIfNull(), RegexOptions.IgnoreCase);
            }
        }

        public static string ReplacePlaceholdersForSmarterMail(Guid organizationId, Guid senderId, int? surveyFormAsset, string body)
        {
            var organization = OrganizationSearch.Select(organizationId) ?? throw new ApplicationError($"Organization not found: {organizationId}");

            var organizationLogoUrl = organization.PlatformCustomization.PlatformUrl.Logo;
            if (!string.IsNullOrEmpty(organizationLogoUrl) && organizationLogoUrl.StartsWith("/"))
                organizationLogoUrl = $"${MessageVariable.AppUrl}" + organizationLogoUrl;

            ReplaceBodyPlaceholder("$Recipient-Email", "$RecipientEmail");
            ReplaceBodyPlaceholder("$Recipient-First-Name", "$RecipientFirstName");
            ReplaceBodyPlaceholder("$Recipient-Last-Name", "$RecipientLastName");
            ReplaceBodyPlaceholder("$Recipient-Person-Code", "$RecipientPersonCode");
            ReplaceBodyPlaceholder("$SurveyPath", $"surveys/{surveyFormAsset ?? 0}/$RecipientIdentifier");

            return body;

            void ReplaceBodyPlaceholder(string placeholder, string value)
            {
                body = body
                    .Replace(placeholder, value, StringComparison.OrdinalIgnoreCase)
                    .Replace(HttpUtility.UrlEncode(placeholder), value, StringComparison.OrdinalIgnoreCase);
            }
        }

        public static string ReplaceLinks(Guid messageId, string appUrl, string body)
        {
            var links = MessageSearch.Instance.FindLinks(messageId);

            return ReplaceLinks(appUrl, body, links);
        }

        public static string ReplaceLinks(string appUrl, string body, IEnumerable<LinkEntity> links)
        {
            if (string.IsNullOrEmpty(body))
                return body;

            var linkItems = HtmlHelper.ExtractLinks(body);

            var newBody = new StringBuilder(body);

            foreach (var linkItem in linkItems)
            {
                var info = links.FirstOrDefault(i => StringHelper.Equals(i.LinkUrl, linkItem.Href));
                if (info == null)
                    continue;

                var newLink = GetLinkUrl(info.LinkIdentifier, "$RecipientIdentifier", appUrl);
                var newTag = linkItem.HtmlTag.Replace(linkItem.Href, newLink);

                newBody.Replace(linkItem.HtmlTag, newTag);
            }

            return newBody.ToString();
        }

        public static string GetLinkUrl(Guid linkIdentifier, string userIdentifier, string siteUrl) =>
            siteUrl + $"/ui/lobby/messages/links/click?link={linkIdentifier}&user={userIdentifier}";

        public static string ReplaceTemplates(string input, string defaultLang)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (defaultLang.HasNoValue())
                defaultLang = "en";

            return RegexTemplatePlaceholderPattern.Replace(input, m =>
            {
                var name = m.Groups["Name"].Value;
                if (!Templates.ContainsKey(name))
                    return m.Value;

                var values = Templates[name];

                var lang = m.Groups["Lang"]?.Value;
                if (string.IsNullOrEmpty(lang) || !values.ContainsKey(lang))
                    lang = defaultLang;

                return values.ContainsKey(lang) ? values[lang] : values[MultilingualString.DefaultLanguage];
            });
        }

        public static string ReplaceVariables(IDictionary<string, string> variables, string value)
        {
            if (value.IsEmpty() || variables.IsEmpty())
                return value;

            return RegexVariablePattern.Replace(value, m =>
            {
                var name = m.Groups["FullName"].Value;
                if (TryGetVariableValue(variables, name, out string dictionaryValue1))
                    return dictionaryValue1;

                name = m.Groups["VarName"].Value;
                if (TryGetVariableValue(variables, name, out string dictionaryValue2))
                    return dictionaryValue2;

                return m.Value;
            });
        }

        private static bool TryGetVariableValue(IDictionary<string, string> variables, string key, out string dictionaryValue)
        {
            dictionaryValue = null;

            foreach (var dictionaryKey in variables.Keys)
            {
                if (string.Equals(dictionaryKey, key, StringComparison.OrdinalIgnoreCase))
                {
                    dictionaryValue = variables[dictionaryKey];
                    return true;
                }
            }

            return false;
        }

        public static string CreateHtmlBody(string subject, string body, bool moveCssInline = true)
        {
            if (IsHtml(body))
                return body;

            const string font = "Segoe";

            var html = Markdown.ToHtml(body);
            var builder = new HtmlBuilder(subject, html);

            AddCssFile("~/UI/Layout/common/parts/css/markdown/common.css");

            AddCssFile($"~/UI/Layout/common/parts/css/markdown/fonts/{font}.css");

            return builder.ToHtml(moveCssInline);

            void AddCssFile(string virtualPath)
            {
                var physicalPath = HostingEnvironment.MapPath(virtualPath);
                if (File.Exists(physicalPath))
                    builder.AddCssFile(physicalPath);
                else
                {
                    if (LogToSentry != null)
                        LogToSentry($"File not found: Physical: {physicalPath} , Virtual: {virtualPath}");
                }
            }
        }

        private static bool IsHtml(string text)
            => text != null && text.Contains("<!DOCTYPE html>");

        public static string CreateHtmlSnippet(string markdown, bool moveCssInline = true)
        {
            var inline = CreateHtmlBody(null, markdown, moveCssInline);

            var startIndex = inline.IndexOf("<body");
            if (startIndex != -1)
            {
                startIndex = inline.IndexOf('>', startIndex);
                if (startIndex != -1)
                    startIndex++;
            }

            var endIndex = inline.LastIndexOf("</body>");

            return endIndex > startIndex ? inline.Substring(startIndex, endIndex - startIndex) : inline;
        }

        #endregion

        #region Methods (commands)

        public static IEnumerable<Command> CreateMessage(VMessage message, ContentContainer content, bool isNewsleter = false, SurveyMessageType surveyType = SurveyMessageType.Undefined) // add type
        {
            if (message.MessageType.IsEmpty())
                throw new ApplicationError("Message type is undefined");

            if (message.MessageName.IsEmpty())
                throw new ApplicationError("Message name is undefined");

            if (content.Title.Text.IsEmpty)
                throw new ApplicationError("Message title is undefined");

            if (content.Title.Text.Default.IsEmpty())
                throw new ApplicationError("Message title has no value for the default language");

            if (!content.Body.Text.IsEmpty && content.Body.Text.Default.IsEmpty())
                throw new ApplicationError("Message content has no value for the default language");

            message.MessageIdentifier = UniqueIdentifier.Create();

            var commands = new List<Command>();

            commands.Add(new CreateMessage(
                message.MessageIdentifier,
                message.OrganizationIdentifier,
                message.SenderIdentifier,
                message.MessageType,
                message.MessageName,
                content.Title.Text,
                content.Body.Text,
                message.SurveyFormIdentifier));

            if (message.SurveyFormIdentifier.HasValue)
            {
                if (message.MessageType == MessageTypeName.Invitation)
                {
                    if (new SurveySearch(null).Exists(message.SurveyFormIdentifier.Value))
                        commands.Add(new AddSurveyFormMessage(message.SurveyFormIdentifier.Value, new SurveyMessage { Identifier = message.MessageIdentifier, Type = SurveyMessageType.Invitation }));
                }
                else if ((message.MessageType == MessageTypeName.Newsletter || message.MessageType == MessageTypeName.Notification) && isNewsleter && surveyType != SurveyMessageType.Undefined)
                {
                    if (new SurveySearch(null).Exists(message.SurveyFormIdentifier.Value))
                        commands.Add(new AddSurveyFormMessage(message.SurveyFormIdentifier.Value, new SurveyMessage { Identifier = message.MessageIdentifier, Type = surveyType }));
                }
            }

            return commands.ToArray();
        }

        #endregion

        #region Helpers

        public static byte[] Serialize(VMessage message)
        {
            var content = TContentSearch.Instance.GetBlock(message.MessageIdentifier);

            var data = new VMessageJsonV2
            {
                Content = content,
                MessageAttachments = message.MessageAttachments,
                ApplicationChangeType = message.MessageName,
                MessageType = message.MessageType,
                Sender = new Sender()
                {
                    SenderEmail = message.SenderEmail,
                    SenderName = message.SenderName,
                    SenderNickname = message.SenderNickname,
                },
                SystemMailbox = message.SystemMailbox,
                IsDisabled = message.IsDisabled,
            };
            var json = JsonHelper.JsonExport(data);

            return Encoding.UTF8.GetBytes(json);
        }

        public static bool Deserialize(string json, out VMessage message, out ContentContainer content)
        {
            message = null;
            content = null;

            VMessageJsonBase data;

            try
            {
                if (JsonHelper.IsSameType<VMessageJson>(json))
                    data = JsonHelper.JsonImport<VMessageJson>(json);
                else
                    data = JsonHelper.JsonImport<VMessageJsonV2>(json);
            }
            catch (JsonReaderException)
            {
                return false;
            }
            catch (ApplicationError)
            {
                return false;
            }

            var messageIdentifier = UniqueIdentifier.Create();

            message = new VMessage
            {
                MessageIdentifier = messageIdentifier,
                MessageAttachments = data.MessageAttachments,
                MessageName = data.ApplicationChangeType,
                MessageType = data.MessageType,
                SenderEmail = data.Sender.SenderEmail,
                SenderName = data.Sender.SenderName,
                SenderNickname = data.Sender.SenderNickname,
                SystemMailbox = data.SystemMailbox,
                IsDisabled = data.IsDisabled
            };

            if (data is VMessageJson v1)
            {
                content = new ContentContainer();
                content.Title.Text.Default = v1.MessageTitle;
                content.Body.Text.Default = v1.ContentText;
                content.Body.Html.Default = v1.ContentHtml;
            }
            else if (data is VMessageJsonV2 v2)
            {
                content = v2.Content ?? new ContentContainer();
            }
            else
                throw new NotImplementedException();

            return true;
        }

        private static string ReplacePlaceholder(string input, string name, Func<string, string> process)
        {
            var beginTag = $"<!-- begin: {name} -->";
            var endTag = $"<!-- end: {name} -->";

            var beginTagStart = input.IndexOf(beginTag);
            var endTagStart = input.IndexOf(endTag);

            if (beginTagStart < 0 || endTagStart < 0)
            {
                return input;
            }

            var content = input.Substring(beginTagStart + beginTag.Length, endTagStart - beginTagStart - beginTag.Length);
            content = process(content);

            return input.Substring(0, beginTagStart)
                   + content
                   + input.Substring(endTagStart + endTag.Length, input.Length - endTagStart - endTag.Length);
        }

        #endregion
    }
}