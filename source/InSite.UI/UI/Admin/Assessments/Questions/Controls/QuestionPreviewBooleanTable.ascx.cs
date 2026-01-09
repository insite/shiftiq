using System.Collections.Generic;

using InSite.Admin.Assessments.Questions.Utilities;
using InSite.Domain.Attempts;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionPreviewBooleanTable : QuestionPreviewControl
    {
        private class RowInfo : IRowDataItem
        {
            public bool IsTrue { get; private set; }
            public string GroupName { get; private set; }
            public IEnumerable<CellInfo> Cells { get; set; }

            private RowInfo() { }

            public static RowInfo Create(int qSequence, int oSequence, AttemptOption option)
            {
                return new RowInfo
                {
                    IsTrue = option.IsTrue == true,
                    GroupName = $"group_{qSequence}_{oSequence}"
                };
            }
        }

        public override void LoadData(PreviewQuestionModel model)
        {
            BindTable(HeaderRepeater, RowRepeater, model, RowInfo.Create);
        }
    }
}