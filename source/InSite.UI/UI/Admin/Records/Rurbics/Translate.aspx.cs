using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Admin.Assets.Contents.Controls;
using InSite.Application.Records.Read;
using InSite.Application.Rubrics.Write;
using InSite.Common.Web;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Common.Events;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.UI.Admin.Records.Rurbics
{
    public partial class Translate : AdminBasePage, IHasParentLinkParameters
    {
        #region NavigateUrl

        public const string NavigateUrl = "/ui/admin/records/rubrics/translate";

        public static string GetNavigateUrl(Guid rubricId, string tab = null)
        {
            var url = NavigateUrl + "?rubric=" + rubricId;

            if (tab.IsNotEmpty())
                url += "&tab=" + HttpUtility.UrlEncode(tab);

            return url;
        }

        public static void Redirect(Guid rubricId, string tab = null) =>
            HttpResponseHelper.Redirect(GetNavigateUrl(rubricId, tab));

        #endregion

        #region Classes

        private enum EntityType { None, Rubric, Criterion, Rating }

        protected enum FieldLabel { None, Title, Description }

        private class ControlData
        {
            public QRubric Rubric { get; set; }
            public QRubricCriterion[] Criteria { get; set; }
            public IDictionary<Guid, ContentContainer> Containers { get; set; }
            public IDictionary<Guid, EntityType> ContainerMapping { get; set; }
        }

        [Serializable]
        private class FieldKey
        {
            public Guid EntityId { get; private set; }
            public EntityType EntityType { get; private set; }
            public FieldLabel FieldLabel { get; private set; }

            public static string Serialize(Guid id, EntityType entityType, FieldLabel fieldLabel)
            {
                return StringHelper.EncodeBase64Url(stream =>
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(id);
                        writer.Write((byte)entityType);
                        writer.Write((byte)fieldLabel);
                    }
                });
            }

            public static FieldKey Deserialize(string data)
            {
                return StringHelper.DecodeBase64Url(data, stream =>
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        return new FieldKey
                        {
                            EntityId = reader.ReadGuid(),
                            EntityType = (EntityType)reader.ReadByte(),
                            FieldLabel = (FieldLabel)reader.ReadByte()
                        };
                    }
                });
            }
        }

        #endregion

        #region Properties

        private Guid RubricIdentifier => Guid.TryParse(Request.QueryString["rubric"], out var id) ? id : Guid.Empty;

        private string Tab => Request.QueryString["tab"];

        private string FromLanguage => Language.Default;

        protected bool ToLanguageSelected
        {
            get => (bool)ViewState[nameof(ToLanguageSelected)];
            set => ViewState[nameof(ToLanguageSelected)] = value;
        }

        private FieldKey CurrentFieldKey
        {
            get => (FieldKey)ViewState[nameof(CurrentFieldKey)];
            set => ViewState[nameof(CurrentFieldKey)] = value;
        }

        #endregion

        #region Fields

        private ControlData _data;

        #endregion

        #region Intitialization and Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ToLanguage.AutoPostBack = true;
            ToLanguage.ValueChanged += ToLanguage_ValueChanged;

            TranslationWindow.LoadTranslation += TranslationWindow_LoadTranslation;
            TranslationWindow.SaveTranslation += TranslationWindow_SaveTranslation;

            CriteriaRepeater.ItemDataBound += CriteriaRepeater_ItemDataBound;

            UpdateButton.Click += UpdateButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            ToLanguage.Settings.ExcludeLanguage = new[] { Language.Default };
            ToLanguage.RefreshData();

            ToLanguageSelected = false;

            Open();

            CloseButton.NavigateUrl = Outline.GetNavigateUrl(RubricIdentifier);
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var data = LoadData();
            if (data.Rubric == null || data.Rubric.OrganizationIdentifier != Organization.Identifier)
                Search.Redirect();

            var isLocked = ServiceLocator.RubricSearch.RubricHasAttempts(data.Rubric.RubricIdentifier);
            if (isLocked)
                Outline.Redirect(data.Rubric.RubricIdentifier);

            BindData(data);
        }

        private ControlData LoadData()
        {
            var rubric = ServiceLocator.RubricSearch.GetRubric(RubricIdentifier);
            var criteria = ServiceLocator.RubricSearch.GetCriteria(RubricIdentifier, x => x.RubricRatings);

            var allIds = criteria.SelectMany(x => x.RubricRatings).Select(x => x.RubricRatingIdentifier)
                .Concat(criteria.Select(x => x.RubricCriterionIdentifier))
                .Append(rubric.RubricIdentifier)
                .ToArray();

            var containers = ServiceLocator.ContentSearch.GetBlocks(allIds);

            return new ControlData
            {
                Rubric = rubric,
                Criteria = criteria,
                Containers = containers,
                ContainerMapping = criteria
                    .SelectMany(x => x.RubricRatings).Select(x => (Id: x.RubricRatingIdentifier, Type: EntityType.Rating))
                    .Concat(criteria.Select(x => (Id: x.RubricCriterionIdentifier, Type: EntityType.Criterion)))
                    .Append((Id: rubric.RubricIdentifier, Type: EntityType.Rubric))
                    .ToDictionary(x => x.Id, x => x.Type)
            };
        }

        private void BindData(ControlData data)
        {
            _data = data;

            FromLanguageOutput.InnerText = Language.GetEnglishName(FromLanguage);

            FieldTitle.TextOriginal = GetTitleFrom(data.Rubric.RubricIdentifier);
            FieldTitle.TextTranslated = GetTitleTo(data.Rubric.RubricIdentifier);
            FieldTitle.AllowTranslate = ToLanguageSelected;
            FieldTitle.OnButtonClientClick = GetWindowOpenScript(data.Rubric.RubricIdentifier, FieldLabel.Title);

            FieldDescription.TextOriginal = GetDescriptionFrom(data.Rubric.RubricIdentifier);
            FieldDescription.TextTranslated = GetDescriptionTo(data.Rubric.RubricIdentifier);
            FieldDescription.AllowTranslate = ToLanguageSelected;
            FieldDescription.OnButtonClientClick = GetWindowOpenScript(data.Rubric.RubricIdentifier, FieldLabel.Description);

            CriteriaRepeater.DataSource = data.Criteria.OrderBy(x => x.CriterionSequence);
            CriteriaRepeater.DataBind();
        }

        private void TryAutoTranslateAndSave(ControlData data, string fromLang, string toLang)
        {
            var strings = new List<MultilingualString>();
            var containers = new Dictionary<Guid, ContentContainer>();

            foreach (var pair in data.Containers)
            {
                foreach (var item in pair.Value.GetItems())
                {
                    if (item.Text[fromLang].IsEmpty() || item.Text[toLang].IsNotEmpty())
                        continue;

                    strings.Add(item.Text);

                    if (!containers.ContainsKey(pair.Key))
                        containers.Add(pair.Key, pair.Value);
                }
            }

            if (strings.Count > 0)
            {
                var rubricId = data.Rubric.RubricIdentifier;
                var commands = new List<ICommand>();

                foreach (var pair in containers)
                {
                    var type = data.ContainerMapping[pair.Key];
                    var command = GetTranslateCommand(type, rubricId, pair.Key, pair.Value);

                    commands.Add(command);
                }

                Translate(fromLang, toLang, strings);

                ServiceLocator.SendCommand(new Application.Rubrics.Write.RunCommands(data.Rubric.RubricIdentifier, commands.ToArray()));

                var fromName = Language.GetEnglishName(fromLang);
                var toName = Language.GetEnglishName(toLang);

                ScreenStatus.AddMessage(AlertType.Success, $"Google Translate has been used to translate this rubric from {fromName} to {toName}. Please review and revise the translation as needed.");
            }
        }

        private void SaveTranslation(FieldKey key, MultilingualString mString)
        {
            var labelName = GetContentLabel(key.FieldLabel);

            var container = new ContentContainer();
            container[labelName].Text = mString;

            var command = GetTranslateCommand(key.EntityType, RubricIdentifier, key.EntityId, container);

            ServiceLocator.SendCommand(command);

            ScriptManager.RegisterStartupScript(
                Page,
                typeof(Translate),
                "update",
                TranslationWindow.GetCloseScript() + $" $('#{UpdateButton.ClientID}').click();",
                true);
        }

        private static ICommand GetTranslateCommand(EntityType type, Guid aggregateId, Guid? childId, ContentContainer container)
        {
            if (type == EntityType.Rubric)
                return new TranslateRubric(aggregateId, container);
            else if (type == EntityType.Criterion)
                return new TranslateRubricCriterion(aggregateId, childId.Value, container);
            else if (type == EntityType.Rating)
                return new TranslateRubricCriterionRating(aggregateId, childId.Value, container);
            else
                throw ApplicationError.Create("Unexpected container type: {0}", type);
        }

        #endregion

        #region Event handlers

        private void ToLanguage_ValueChanged(object sender, ComboBoxValueChangedEventArgs e)
        {
            ToLanguageSelected = e.NewValue.IsNotEmpty();

            var data = LoadData();

            if (ToLanguageSelected)
                TryAutoTranslateAndSave(data, FromLanguage, ToLanguage.Value);

            BindData(data);
        }

        private void CriteriaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var criteria = (QRubricCriterion)e.Item.DataItem;

            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");
            ratingRepeater.DataSource = criteria.RubricRatings.OrderBy(x => x.RatingSequence);
            ratingRepeater.DataBind();
        }

        private void TranslationWindow_LoadTranslation(object sender, TranslationWindow.LoadEventArgs e)
        {
            var key = CurrentFieldKey = FieldKey.Deserialize(e.Key);

            var label = GetContentLabel(key.FieldLabel);
            var content = ServiceLocator.ContentSearch.GetBlock(key.EntityId, labels: new[] { label });

            e.Translation = content[label].Text.Serialize();
            e.Language = ToLanguage.Value;
        }

        private void TranslationWindow_SaveTranslation(object sender, StringValueArgs e)
        {
            if (CurrentFieldKey == null)
                return;

            var key = CurrentFieldKey;

            SaveTranslation(key, MultilingualString.Deserialize(e.Value));
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            CurrentFieldKey = null;

            var data = LoadData();

            BindData(data);
        }

        #endregion

        #region IHasParentLinkParameters

        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? Outline.GetNavigateParams(RubricIdentifier, Tab)
                : null;
        }

        #endregion

        #region Methods (helpers)

        private static string GetContentLabel(FieldLabel value)
        {
            if (value == FieldLabel.Title)
                return ContentLabel.Title;

            if (value == FieldLabel.Description)
                return ContentLabel.Description;

            throw ApplicationError.Create("Unexpected field label type: {0}", value.GetName());
        }

        protected string GetWindowOpenScript(string idField, FieldLabel fieldType)
        {
            var dataItem = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(dataItem, idField);
            var entityType = _data.ContainerMapping[id];
            var key = FieldKey.Serialize(id, entityType, fieldType);

            return TranslationWindow.GetOpenScript(key) + " return false;";
        }

        private string GetWindowOpenScript(Guid containerId, FieldLabel fieldType)
        {
            var entityType = _data.ContainerMapping[containerId];
            var key = FieldKey.Serialize(containerId, entityType, fieldType);

            return TranslationWindow.GetOpenScript(key) + " return false;";
        }

        protected string GetTitleFrom(string idField)
        {
            var dataItem = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(dataItem, idField);

            return GetTitleFrom(id);
        }

        protected string GetDescriptionFrom(string idField)
        {
            var dataItem = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(dataItem, idField);

            return GetDescriptionFrom(id);
        }

        protected string GetTitleTo(string idField)
        {
            var dataItem = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(dataItem, idField);

            return GetTitleTo(id);
        }

        protected string GetDescriptionTo(string idField)
        {
            var dataItem = Page.GetDataItem();
            var id = (Guid)DataBinder.Eval(dataItem, idField);

            return GetDescriptionTo(id);
        }

        private string GetTitleFrom(Guid containerId) =>
            GetContentText(containerId, ContentLabel.Title, FromLanguage);

        private string GetDescriptionFrom(Guid containerId) =>
            GetContentText(containerId, ContentLabel.Description, FromLanguage);

        private string GetTitleTo(Guid containerId) =>
            GetContentText(containerId, ContentLabel.Title, ToLanguage.Value);

        private string GetDescriptionTo(Guid containerId) =>
            GetContentText(containerId, ContentLabel.Description, ToLanguage.Value);

        private string GetContentText(Guid containerId, string label, string lang) =>
            lang.IsNotEmpty() && _data.Containers.TryGetValue(containerId, out var container) ? container[label].Text[lang] : string.Empty;

        #endregion
    }
}