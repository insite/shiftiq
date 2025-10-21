using InSite.Persistence;

using Shift.Common;

using TextType = InSite.Common.Web.UI.FindStandardHelper.TextType;

namespace InSite.Common.Web.UI
{
    public class StandardComboBox : ComboBox
    {
        public StandardFilter ListFilter => (StandardFilter)(ViewState[nameof(ListFilter)]
            ?? (ViewState[nameof(ListFilter)] = new StandardFilter { OrganizationIdentifier = CurrentSessionState.Identity.Organization.Identifier }));

        public TextType TextType
        {
            get => (TextType)(ViewState[nameof(TextType)] ?? TextType.TypeNumberTitle);
            set => ViewState[nameof(TextType)] = value;
        }

        private string Language => CurrentSessionState.Identity.Language;

        protected override ListItemArray CreateDataSource()
        {
            var data = FindStandardHelper.Select(ListFilter, TextType, (value, text) => new ListItem
            {
                Value = value.ToString(),
                Text = text
            }, Language);

            return new ListItemArray(data);
        }
    }
}