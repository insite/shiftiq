using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    public partial class FieldList : BaseUserControl
    {
        #region Classes

        private class FieldItem
        {
            public JournalSetupFieldType Type { get; }
            public ExperienceFieldDescription Description { get; }

            public string LabelText => _content[JournalSetupField.ContentLabels.LabelText].Text.Get(Identity.Language).IfNullOrEmpty(_entity.FieldType);
            public string HelpText => _content[JournalSetupField.ContentLabels.HelpText].Text.Get(Identity.Language);
            public bool IsRequired => _entity.FieldIsRequired;

            private readonly QJournalSetupField _entity;
            private readonly ContentContainer _content;

            public FieldItem(QJournalSetupField entity)
            {
                _entity = entity;
                _content = ServiceLocator.ContentSearch.GetBlock(entity.JournalSetupFieldIdentifier)
                    ?? new ContentContainer();

                Type = entity.FieldType.ToEnum<JournalSetupFieldType>();
                Description = ExperienceFieldDescription.Items[Type];
            }
        }

        #endregion

        #region Properties

        private List<JournalSetupFieldType> FieldTypes
        {
            get => (List<JournalSetupFieldType>)ViewState[nameof(FieldTypes)];
            set => ViewState[nameof(FieldTypes)] = value;
        }

        #endregion

        #region Fields

        private QExperience _experience;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            FieldRepeater.DataBinding += FieldRepeater_DataBinding;
            FieldRepeater.ItemDataBound += FieldRepeater_ItemDataBound;
        }

        #endregion

        #region Public methods

        public int LoadData(Guid journalSetupIdentifier, QExperience experience)
        {
            _experience = experience ?? new QExperience
            {
                JournalIdentifier = journalSetupIdentifier
            };

            var fields = ServiceLocator.JournalSearch.GetJournalSetupFields(journalSetupIdentifier);

            FieldRepeater.DataSource = fields.Select(x => new FieldItem(x));
            FieldRepeater.DataBind();

            return fields.Count;
        }

        public decimal? GetHours()
        {
            for (int i = 0; i < FieldRepeater.Items.Count; i++)
            {
                if (FieldTypes[i] != JournalSetupFieldType.Hours)
                    continue;

                var repeaterItem = FieldRepeater.Items[i];
                var container = (DynamicControl)repeaterItem.FindControl("Field");

                return ((ExperienceFields.Number)container.GetControl()).Value;
            }

            return null;
        }

        public void GetChanges(Guid journalSetupIdentifier, Guid journalIdentifier, Guid experienceIdentifier, List<Command> commands)
        {
            var changeActions = new List<ExperienceFieldDescription.ChangeAction>();

            var experience = new QExperience
            {
                JournalIdentifier = journalIdentifier,
                ExperienceIdentifier = experienceIdentifier
            };

            for (int i = 0; i < FieldRepeater.Items.Count; i++)
            {
                var repeaterItem = FieldRepeater.Items[i];
                var container = (DynamicControl)repeaterItem.FindControl("Field");
                var field = (IExperienceField)container.GetControl();

                var fieldType = FieldTypes[i];
                var fieldDescription = ExperienceFieldDescription.Items[fieldType];
                var changeAction = fieldDescription.Save(field, experience);

                if (!changeActions.Contains(changeAction))
                    changeActions.Add(changeAction);
            }

            foreach (var changeAction in changeActions)
            {
                var command = changeAction(experience);
                commands.Add(command);
            }
        }

        #endregion

        #region Event handlers

        private void FieldRepeater_DataBinding(object sender, EventArgs e)
        {
            FieldTypes = new List<JournalSetupFieldType>();
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var fieldItem = (FieldItem)e.Item.DataItem;
            var container = (DynamicControl)e.Item.FindControl("Field");
            var field = (IExperienceField)container.LoadControl($"~/UI/Portal/Records/Logbooks/Controls/ExperienceFields/{fieldItem.Description.ControlName}.ascx");

            field.Title = fieldItem.LabelText;
            field.Help = fieldItem.HelpText;
            field.IsRequired = fieldItem.IsRequired;
            field.ValidationGroup = "Journal";

            fieldItem.Description.Load(field, _experience);

            FieldTypes.Add(fieldItem.Type);
        }

        #endregion
    }
}