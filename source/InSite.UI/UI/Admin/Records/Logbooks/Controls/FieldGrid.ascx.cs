using System;
using System.Linq;
using System.Web.UI;

using InSite.Domain.Records;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Records.Logbooks.Controls
{
    public partial class FieldGrid : UserControl
    {
        public int LoadData(Guid journalSetupIdentifier)
        {
            AddFields.NavigateUrl = $"/ui/admin/records/logbooks/add-fields?journalsetup={journalSetupIdentifier}";

            var fields = ServiceLocator.JournalSearch.GetJournalSetupFields(journalSetupIdentifier);

            FieldRepeater.Visible = fields.Count > 0;

            ReorderFields.NavigateUrl = $"/ui/admin/records/logbooks/reorder-fields?journalsetup={journalSetupIdentifier}";
            ReorderFields.Visible = fields.Count > 1;

            if (fields.Count == 0)
                return 0;

            var data = fields
                .Select(x =>
                {
                    var content = ServiceLocator.ContentSearch.GetBlock(x.JournalSetupFieldIdentifier, Shift.Common.ContentContainer.DefaultLanguage);
                    var labelText = content?[JournalSetupField.ContentLabels.LabelText].Text.Default;
                    var helpText = content?[JournalSetupField.ContentLabels.HelpText].Text.Default;

                    var fieldType = x.FieldType.ToEnum<JournalSetupFieldType>();

                    return new
                    {
                        Identifier = x.JournalSetupFieldIdentifier,
                        Sequence = x.Sequence,
                        FieldType = fieldType.GetDescription(),
                        IsRequired = x.FieldIsRequired ? "Yes" : string.Empty,
                        LabelText = labelText,
                        HelpText = helpText
                    };
                })
                .OrderBy(x => x.Sequence)
                .ToList();

            FieldRepeater.DataSource = data;
            FieldRepeater.DataBind();

            return data.Count;
        }
    }
}