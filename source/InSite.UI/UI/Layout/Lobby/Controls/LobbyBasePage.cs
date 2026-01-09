using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Utilities;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Organizations;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common;

using BreadcrumbItem = Shift.Contract.BreadcrumbItem;

namespace InSite.UI.Layout.Lobby
{
    public class LobbyBasePage : Page, IHasWebRoute, IHasTranslator, IAdminPage
    {
        public Control ActionControl => this;

        protected CultureInfo LanguageCulture => CultureInfo.GetCultureInfo(Identity.Language);
        protected CultureInfo CurrentCulture => new CultureInfo(CurrentLanguage);

        public string CurrentLanguage => CookieTokenModule.Current.Language;

        protected static Domain.Foundations.ISecurityFramework Identity => CurrentSessionState.Identity;
        protected static OrganizationState Organization => Identity.Organization;

        #region Multilanguage

        public bool IsTranslated { get; set; }

        public InputTranslator Translator { get; set; }

        private void InitializeTranslator()
        {
            var language = CookieTokenModule.Current.Language;
            var token = CookieTokenModule.Current;
            var organization = OrganizationSearch.Select(token.OrganizationCode)?.OrganizationIdentifier ?? Guid.Empty;

            Translator = new InputTranslator(organization, language);
        }

        private bool AutoTranslate => ServiceLocator.AppSettings.Application.AutoTranslateEnabled && Translator?.Language != "en";

        public string Translate(string from, string to, MultilingualString item) =>
            Translator.Translate(from, to, item);

        public void Translate(string from, string to, ICollection<MultilingualString> list) =>
            Translator.Translate(from, to, list);

        public string Translate(string text) =>
            AutoTranslate ? Translator.Translate(text) : text;

        public string Translate(string label, string text) =>
            AutoTranslate ? Translator.Translate(label, text) : text;

        public string Translate(string from, string to, string input) =>
            AutoTranslate ? Translator.Translate(from, to, input) : input;

        public void Translate(ComboBox cb) =>
            Translator.Translate(cb);

        public string GetDisplayHtml(string attribute, string @default)
            => Markdown.ToHtml(GetDisplayText(attribute, @default));

        public string GetDisplayText(string attribute, string @default = null)
        {
            var token = CookieTokenModule.Current;
            var organization = OrganizationSearch.Select(token.OrganizationCode)?.OrganizationIdentifier ?? Guid.Empty;
            return LabelSearch.GetTranslation(attribute, CurrentSessionState.Identity.Language, organization, false, true, @default);
        }

        #endregion

        public virtual string ActionUrl => ((System.Web.Routing.Route)Page.RouteData.Route).Url;
        public virtual string RawUrl => Request.RawUrl;

        public List<BreadcrumbItem> BreadcrumbsItems { get; set; }

        public TAction ActionModel { get; set; }
        public WebRoute Route => _route;

        private WebRoute _route;

        public void AddBreadcrumb(string text, string href, bool translate = true)
        {
            if (translate)
                text = Translate(text);

            BreadcrumbsItems.Add(new BreadcrumbItem(text, href));
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            InitializeBreadcrumbs();
            InitializeTranslator();
            InitializeRouting();
        }

        private void InitializeBreadcrumbs()
        {
            BreadcrumbsItems = new List<BreadcrumbItem>();
        }

        private void InitializeRouting()
        {
            ActionModel = TActionSearch.Get(ActionUrl) ?? TActionSearch.GetByControllerPath(Request.CurrentExecutionFilePath);
            if (ActionModel != null)
                _route = new WebRoute(ActionModel);

            // throw new Web.ActionNotFoundException($"The database table settings.TAction is missing a record for this Action URL: {ActionUrl} (Controller Path = {Request.CurrentExecutionFilePath}).");
        }

        protected override void OnLoad(EventArgs e)
        {
            HttpResponseHelper.SetNoCache();

            Form.Action = Request.RawUrl;

            base.OnLoad(e);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (!Page.IsPostBack && !IsTranslated && ServiceLocator.AppSettings.Application.AutoTranslateEnabled)
            {
                IsTranslated = true;
                Translator.Translate(this);
            }

            if (!string.Equals(CurrentLanguage, "en", StringComparison.OrdinalIgnoreCase))
            {
                var lang = CurrentLanguage;
                var flatpickrImport = $"<script src=\"/ui/layout/common/parts/around/js/vendor/flatpickr/dist/l10n/{lang}.js\"></script>";
                var flatpickrLocalize = $"<script>$(document).ready(function () {{ flatpickr.localize(flatpickr.l10ns.{lang}); }});</script>";

                ClientScript.RegisterStartupScript(GetType(), "Flatpickr Translation", flatpickrImport + flatpickrLocalize);
            }

            if (Page.IsPostBack)
                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(LobbyBasePage),
                    "update_history",
                    "if (window.history && window.history.replaceState) { window.history.replaceState(null, null, window.location.href); }",
                    true);
        }

        public static void RedirectToUrl(Page page, string url)
        {
            if (!page.IsCallback)
            {
                const string login = "/ui/lobby/signin";
                if (StringHelper.Equals(page.Request.RawUrl, url) && !StringHelper.Equals(login, url))
                    HttpResponseHelper.Redirect(login);
                else
                    HttpResponseHelper.Redirect(url, true);
            }

            try
            {
                try
                {
                    page.Response.RedirectLocation = url;
                }
                catch
                {
                    var script = $"window.location.href = '{url}';";
                    page.ClientScript.RegisterStartupScript(page.GetType(), "InvalidViewStateRedirect", script, true);
                }
            }
            catch (Exception ex)
            {
                var source = typeof(LobbyBasePage).FullName + "." + nameof(RedirectToUrl);
                AppSentry.SentryError($"Unable to redirect from {page.Request.RawUrl} to {url}. {ex.Message}", source);
            }
        }

        #region Navigation

        private static readonly string ParentContextKey = typeof(LobbyBasePage).FullName + ".Parent";

        private static string GetActionName(string url)
        {
            var separator = url.IndexOf('?');
            return separator >= 0
                ? url.Substring(1, separator - 1)
                : url.Substring(1);
        }

        protected IWebRoute GetParent()
        {
            var value = Context.Items[ParentContextKey];

            if (value == null)
            {
                WebRoute route = null;

                var returnUrl = GetReturnUrl();
                if (returnUrl.IsNotEmpty())
                {
                    var actionName = GetActionName(returnUrl);
                    route = WebRoute.GetWebRoute(actionName);
                }

                if (route == null && Route.ParentRouteIdentifier != null)
                    route = WebRoute.GetWebRoute(Route.ParentRouteIdentifier.Value);

                Context.Items[ParentContextKey] = value = route ?? (object)false;
            }

            return value as WebRoute;
        }

        private static readonly string ReturnUrlContextKey = typeof(LobbyBasePage).FullName + ".ReturnUrl";

        protected virtual string GetReturnUrl()
        {
            var url = (string)(Context.Items[ReturnUrlContextKey]
                ?? (Context.Items[ReturnUrlContextKey] = new ReturnUrl().GetReturnUrl()
                ?? string.Empty));

            if (string.IsNullOrEmpty(url) && Request.UrlReferrer?.Query != null)
                url = HttpUtility.ParseQueryString(Request.UrlReferrer.Query)["ReturnUrl"];

            return url;
        }

        protected string GetParentUrl(string defaultParameters)
        {
            var url = GetReturnUrl();

            if (url.IsEmpty())
            {
                var parent = GetParent();
                if (parent != null)
                {
                    url = "/" + parent.Name;
                    if (defaultParameters.IsNotEmpty())
                        url += "?" + defaultParameters;
                }
            }

            return url.NullIfEmpty();
        }

        protected string GetParentLinkParameters(IWebRoute parent, string defaultParameters)
        {
            if (parent != null && !string.Equals(parent.Name, GetParent()?.Name, StringComparison.OrdinalIgnoreCase))
                return null;

            var returnUrl = GetReturnUrl();
            if (returnUrl.IsNotEmpty() && (parent == null || string.Equals(parent.Name, GetActionName(returnUrl), StringComparison.OrdinalIgnoreCase)))
            {
                var separator = returnUrl.IndexOf('?');

                return separator > 0
                    ? returnUrl.Substring(separator + 1)
                    : null;
            }

            return defaultParameters;
        }

        #endregion

        protected static string FormatDate(object date) => ControlHelper.FormatDate(date);

        protected string GetEmbededHelpContent(string fragment = null)
        {
            var path = Request.Url.AbsolutePath.TrimStart('/')
                + (fragment ?? string.Empty);

            var help = new Shift.Sdk.UI.Help.InlineHelp(
                Organization.PlatformCustomization.InlineInstructionsUrl,
                Organization.PlatformCustomization.InlineLabelsUrl
                );

            var html = help.GetInstruction(path);

            return html;
        }

        protected void SendZipFile(byte[] data, string fileName, string ext) => ControlHelper.SendZipFile(Response, data, fileName, ext);

        protected static bool IsContentItem(GridViewRowEventArgs e) => ControlHelper.IsContentItem(e.Row);

        protected static bool IsContentItem(GridViewRow row) => ControlHelper.IsContentItem(row);

        protected static bool IsContentItem(RepeaterItemEventArgs e) => ControlHelper.IsContentItem(e.Item);

        protected static bool IsContentItem(RepeaterItem item) => ControlHelper.IsContentItem(item);

        protected string LocalizeDate(object o, bool shortFormat = true) => ControlHelper.LocalizeDate(o, shortFormat);

        protected string LocalizeTime(object time, string element = null, bool isHtml = true) => ControlHelper.LocalizeTime(time, element, isHtml);

        #region PDF

        private bool _isPdfOutput;
        private string _pdfOutputName;

        public void PrintPdf(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            _isPdfOutput = true;
            _pdfOutputName = filename;
        }

        public override void RenderControl(HtmlTextWriter writer)
        {
            if (_isPdfOutput)
            {
                _isPdfOutput = false;

                var sb = new StringBuilder();

                using (var stringWriter = new StringWriter(sb))
                {
                    using (var htmlWriter = new HtmlTextWriter(stringWriter))
                    {
                        base.RenderControl(htmlWriter);
                    }
                }

                var htmlString = HtmlHelper.ResolveRelativePaths(
                    Page.Request.Url.Scheme + "://" + Page.Request.Url.Host + Page.Request.RawUrl, sb);
                var settings = new HtmlConverterSettings(ServiceLocator.AppSettings.Application.WebKitHtmlToPdfExePath);
                var data = HtmlConverter.HtmlToPdf(htmlString, settings);

                Response.SendFile(_pdfOutputName, "pdf", data);
            }
            else
            {
                base.RenderControl(writer);
            }
        }

        #endregion
    }
}