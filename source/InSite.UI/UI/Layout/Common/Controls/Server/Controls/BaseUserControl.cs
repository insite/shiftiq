using System;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Domain.Foundations;
using InSite.Domain.Organizations;
using InSite.Persistence;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Common.Web.UI
{
    public partial class BaseUserControl : UserControl
    {
        #region Classes

        private class NullActionContainer : IHasWebRoute
        {
            public static NullActionContainer Instance { get; } = new NullActionContainer();

            public WebRoute Route => null;

            private NullActionContainer()
            {

            }
        }

        #endregion

        public string CurrentLanguage => CookieTokenModule.Current.Language;

        protected static ISecurityFramework Identity => CurrentSessionState.Identity;
        protected static OrganizationState Organization => Identity.Organization;
        protected static UserModel User => Identity.User;

        protected WebRoute Route => _routeContainer.Route;

        protected bool IsBaseControlInited { get; private set; }
        protected bool IsBaseControlLoaded { get; private set; }

        public string ValidationGroup
        {
            get
            {
                return (string)ViewState[nameof(ValidationGroup)];
            }
            set
            {
                _isValidationGroupChanged = ValidationGroup != value;

                if (!_isValidationGroupChanged)
                    return;

                ViewState[nameof(ValidationGroup)] = value;

                if (IsBaseControlLoaded)
                    SetupValidationGroup();
            }
        }

        protected ControlVisibilityHelper Visibility;

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

        protected string GetDisplayText(string attribute) =>
            LabelSearch.GetTranslation(attribute, CurrentSessionState.Identity.Language, Organization.OrganizationIdentifier);

        private IHasWebRoute _routeContainer;

        private IHasTranslator _translatorContainer;

        private bool _isValidationGroupChanged = false;

        protected override void OnInit(EventArgs e)
        {
            Visibility = new ControlVisibilityHelper();
            _routeContainer = Page as IHasWebRoute ?? NullActionContainer.Instance;
            _translatorContainer = Page as IHasTranslator;

            base.OnInit(e);

            SetupValidationGroup();
            IsBaseControlInited = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            SetupValidationGroup();
            IsBaseControlLoaded = true;
        }

        protected string Translate(string text) => _translatorContainer.Translator.Translate(text);

        protected string Translate(string from, string to, string input) => _translatorContainer.Translator.Translate(from, to, input);

        protected void Translate(ComboBox cb) => _translatorContainer.Translator.Translate(cb);

        protected static bool IsContentItem(GridViewRowEventArgs e) => ControlHelper.IsContentItem(e.Row);

        protected static bool IsContentItem(GridViewRow row) => ControlHelper.IsContentItem(row);

        protected static bool IsContentItem(RepeaterItemEventArgs e) => ControlHelper.IsContentItem(e.Item);

        protected static bool IsContentItem(RepeaterItem item) => ControlHelper.IsContentItem(item);

        protected static bool IsContentItem(DataControlRowType type) => type == DataControlRowType.DataRow;

        protected string LocalizeDate(object o, bool shortFormat = true) => ControlHelper.LocalizeDate(o, shortFormat);

        protected string LocalizeTime(object time, string element = null, bool isHtml = true) => ControlHelper.LocalizeTime(time, element, isHtml);

        protected static string FormatDate(object date) => ControlHelper.FormatDate(date);

        private void SetupValidationGroup()
        {
            if (!_isValidationGroupChanged)
                return;

            SetupValidationGroup(ValidationGroup);

            _isValidationGroupChanged = false;
        }

        protected virtual void SetupValidationGroup(string groupName)
        {

        }
    }
}