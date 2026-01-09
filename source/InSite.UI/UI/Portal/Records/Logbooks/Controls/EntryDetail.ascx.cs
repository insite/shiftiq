using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    public partial class EntryDetail : BaseUserControl
    {
        private abstract class FieldItem
        {
            public string Title { get; set; }

            public abstract void BindValue(DynamicControl ctrl);
        }

        private class ValueFieldItem : FieldItem
        {
            public string Value { get; set; }
            public override void BindValue(DynamicControl ctrl)
            {
                var literal = ctrl.LoadControl<System.Web.UI.WebControls.Literal>();
                literal.Text = Value;
            }
        }

        private class ExperienceFieldItem : FieldItem
        {
            public ExperienceFieldDescription Description { get; set; }
            public QExperience Experience { get; set; }

            public override void BindValue(DynamicControl ctrl)
            {
                Description.AddValue(Experience, ctrl);
            }
        }

        public void LoadData(Guid journalSetupIdentifier, QExperience experience)
        {
            IEnumerable<FieldItem> data1, data2;

            var data = GetData(journalSetupIdentifier, experience);
            if (data.Length > 3)
            {
                var count1 = (int)Math.Ceiling(data.Length / 2m);
                var count2 = data.Length - count1;

                data1 = data.Take(count1);
                data2 = data.Skip(count1).Take(count2);
            }
            else
            {
                data1 = data;
                data2 = null;
            }

            FieldRepeater1.ItemDataBound += FieldRepeater_ItemDataBound;
            FieldRepeater1.DataSource = data1;
            FieldRepeater1.DataBind();

            FieldRepeater2.ItemDataBound += FieldRepeater_ItemDataBound;
            FieldRepeater2.DataSource = data2;
            FieldRepeater2.DataBind();
        }

        private void FieldRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var fieldItem = (FieldItem)e.Item.DataItem;
            var valueControl = (DynamicControl)e.Item.FindControl("Value");
            fieldItem.BindValue(valueControl);
        }

        private FieldItem[] GetData(Guid journalSetupIdentifier, QExperience experience)
        {
            var result = new List<FieldItem>
            {
                new ValueFieldItem
                {
                    Title = Translate("Entry Number"),
                    Value = experience.Sequence.ToString()
                },
                new ValueFieldItem
                {
                    Title = Translate("Entry Created"),
                    Value = TimeZones.Format(experience.ExperienceCreated, CurrentSessionState.Identity.User.TimeZone, true)
                }
            };

            var fields = ServiceLocator.JournalSearch.GetJournalSetupFields(journalSetupIdentifier);
            foreach (var field in fields)
            {
                var content = ServiceLocator.ContentSearch.GetBlock(field.JournalSetupFieldIdentifier);
                var fieldType = field.FieldType.ToEnum<JournalSetupFieldType>();

                result.Add(new ExperienceFieldItem
                {
                    Title = content[JournalSetupField.ContentLabels.LabelText].Text.Get(Identity.Language).IfNullOrEmpty(field.FieldType),
                    Description = ExperienceFieldDescription.Items[fieldType],
                    Experience = experience,
                });
            }

            return result.ToArray();
        }
    }
}