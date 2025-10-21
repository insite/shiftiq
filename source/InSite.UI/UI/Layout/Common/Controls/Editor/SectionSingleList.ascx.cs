using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Common.Web.UI;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.UI.Layout.Common.Controls.Editor
{
    public partial class SectionSingleList : SectionBase
    {
        #region Classes

        [JsonObject]
        private class JsSettings
        {
            [JsonProperty(PropertyName = "fieldId")]
            public string EditorFieldID { get; set; }

            [JsonProperty(PropertyName = "deleteId")]
            public string DeleteButtonID { get; set; }
        }

        #endregion

        #region Properties

        private string EntityName
        {
            get => (string)ViewState[nameof(EntityName)];
            set => ViewState[nameof(EntityName)] = value;
        }

        private bool IsRequired
        {
            get => (bool)ViewState[nameof(IsRequired)];
            set => ViewState[nameof(IsRequired)] = value;
        }

        #endregion

        #region Fields

        private bool _isUpdated = false;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Repeater.DataBinding += Repeater_DataBinding;
            Repeater.ItemDataBound += Repeater_ItemDataBound;
            Repeater.ItemCommand += Repeater_ItemCommand;

            AddButton.Click += AddButton_Click;

            CommonScript.ContentKey = typeof(SectionSingleList).FullName;
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (_isUpdated || !HttpRequestHelper.IsAjaxRequest)
            {
                var settings = new List<JsSettings>();

                foreach (RepeaterItem item in Repeater.Items)
                {
                    var editorField = item.FindControl("EditorField");
                    var deleteButton = item.FindControl("DeleteButton");

                    settings.Add(new JsSettings
                    {
                        EditorFieldID = editorField.ClientID,
                        DeleteButtonID = deleteButton.ClientID
                    });
                }

                ScriptManager.RegisterStartupScript(
                    Page,
                    typeof(SectionSingleList),
                    UniqueID,
                    $"singleListSection.register('{UniqueID}',{JsonHelper.SerializeJsObject(settings)});",
                    true);
            }

            base.OnPreRender(e);
        }

        #endregion

        #region Event handlers

        private void Repeater_DataBinding(object sender, EventArgs e)
        {
            _isUpdated = true;
        }

        private void Repeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!ControlHelper.IsContentItem(e))
                return;

            var options = (LayoutContentSection.SingleLine)e.Item.DataItem;

            var deleteButton = (IButton)e.Item.FindControl("DeleteButton");
            deleteButton.ToolTip = $"Delete {Shift.Common.Humanizer.TitleCase(options.Label)}";
            deleteButton.OnClientClick = $"if (!confirm('Are you sure you want to remove the {options.Label}?')) return false;";

            var translationField = (Field)e.Item.FindControl("EditorField");
            translationField.SetOptions(options);
        }

        private void Repeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Delete")
            {
                var list = GetValues().ToList();

                list.RemoveAt(e.Item.ItemIndex);

                BindRepeater(list);
            }
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var list = GetValues().ToList();

            list.Add(GetNewTranslationFieldValue());

            BindRepeater(list);

            MultilingualString GetNewTranslationFieldValue()
            {
                var fieldValue = new MultilingualString();

                foreach (var language in CurrentSessionState.Identity.Organization.Languages.Select(x => x.Name))
                    fieldValue[language] = string.Empty;

                return fieldValue;
            }
        }

        #endregion

        #region Methods (set input values)

        public override void SetOptions(LayoutContentSection options)
        {
            if (options is LayoutContentSection.SingleLineList singleList)
            {
                EntityName = singleList.EntityName;
                IsRequired = singleList.IsRequired;

                LabelOutput.InnerText = singleList.Label;
                DescriptionOutput.InnerText = singleList.Description;
                AddButton.Text = $"<i class='far fa-plus'></i> Add {Shift.Common.Humanizer.TitleCase(EntityName)}";

                BindRepeater(singleList.Values);
            }
            else
            {
                throw new NotImplementedException("Section type: " + options.GetType().FullName);
            }
        }

        public override void SetValidationGroup(string groupName)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var editorField = (Field)item.FindControl("EditorField");
                editorField.ValidationGroup = groupName;
            }
        }

        public override void SetLanguage(string lang)
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var editorField = (Field)item.FindControl("EditorField");
                editorField.InputLanguage = lang;
            }
        }

        private void BindRepeater(IEnumerable<MultilingualString> values)
        {
            Repeater.DataSource = values.Select((x, i) => new LayoutContentSection.SingleLine("Item" + i)
            {
                Label = $"{Shift.Common.Humanizer.TitleCase(EntityName)} #{i + 1}",
                IsRequired = IsRequired,
                Value = x
            });
            Repeater.DataBind();
        }

        #endregion

        #region Methods (get input values)

        public override MultilingualString GetValue() => throw new NotImplementedException();

        public override MultilingualString GetValue(string id) => throw new NotImplementedException();

        public override IEnumerable<MultilingualString> GetValues()
        {
            foreach (RepeaterItem item in Repeater.Items)
            {
                var editorField = (Field)item.FindControl("EditorField");
                yield return editorField.Translation;
            }
        }

        public override void GetValues(MultilingualDictionary dictionary) => throw new NotImplementedException();

        #endregion

        #region Methods (tab management)

        public override void OpenTab(string id) { }

        #endregion
    }
}