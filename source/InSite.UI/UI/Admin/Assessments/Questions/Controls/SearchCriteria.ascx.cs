using System;
using System.Text.RegularExpressions;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<QBankQuestionFilter>
    {
        private static readonly Regex AssetAndVersionPattern = new Regex(@"^(?:[0-9]+)?(?:\.(?:[1-9][0-9]|[0-9]))?$", RegexOptions.Compiled);

        [Serializable]
        private class QBankQuestionFilterInternal : QBankQuestionFilter
        {
            public DateRangeShortcut? QuestionChangedShortcut { get; set; }
        }

        public override QBankQuestionFilter Filter
        {
            get
            {
                var filter = new QBankQuestionFilterInternal
                {
                    OrganizationIdentifier = Organization.Identifier,
                    QuestionAsset = AssetAndVersionPattern.Match(QuestionAsset.Text)?.Value,
                    QuestionText = QuestionText.Text,
                    QuestionCode = QuestionCode.Text,
                    QuestionReference = QuestionReference.Text,
                    QuestionTag = QuestionTag.Text,
                    QuestionBank = QuestionBank.Text,
                    QuestionCompetencyTitle = QuestionCompetencyTitle.Text,
                    QuestionType = QuestionType.Value,
                    QuestionDifficulty = QuestionDifficulty.ValueAsInt,
                    RubricIdentifier = RubricComboBox.ValueAsGuid,
                    QuestionFlag = !QuestionFlag.EnumValue.HasValue
                        ? null
                        : QuestionFlag.EnumValue == Indicator.None
                            ? "None"
                            : QuestionFlag.FlagValue.Value.GetName()
                };

                if (QuestionDateRangeSelector.Value.IsNotEmpty())
                {
                    DateTimeRange dateTimeRange;

                    if (QuestionDateRangeSelector.Value != "Custom")
                    {
                        filter.QuestionChangedShortcut = QuestionDateRangeSelector.Value.ToEnum<DateRangeShortcut>();

                        dateTimeRange = Shift.Common.Calendar.GetDateTimeRange(filter.QuestionChangedShortcut.Value);
                    }
                    else
                        dateTimeRange = new DateTimeRange(QuestionDateRangeSince.Value, QuestionDateRangeBefore.Value);

                    filter.QuestionChangedRange = new DateTimeOffsetRange(dateTimeRange.Since, dateTimeRange.Before, User.TimeZone.BaseUtcOffset);
                }

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                QuestionAsset.Text = value.QuestionAsset;
                QuestionText.Text = value.QuestionText;
                QuestionCode.Text = value.QuestionCode;
                QuestionReference.Text = value.QuestionReference;
                QuestionTag.Text = value.QuestionTag;
                QuestionBank.Text = value.QuestionBank;
                QuestionCompetencyTitle.Text = value.QuestionCompetencyTitle;
                RubricComboBox.ValueAsGuid = value.RubricIdentifier;

                if (value is QBankQuestionFilterInternal intFilter && intFilter.QuestionChangedShortcut.HasValue)
                {
                    QuestionDateRangeSelector.Value = intFilter.QuestionChangedShortcut.Value.GetName();
                    QuestionDateRangeSince.Value = null;
                    QuestionDateRangeBefore.Value = null;
                }
                else if (value.QuestionChangedRange?.IsEmpty == false)
                {
                    QuestionDateRangeSelector.Value = "Custom";
                    QuestionDateRangeSince.Value = value.QuestionChangedRange.Since.HasValue
                        ? value.QuestionChangedRange.Since.Value.DateTime
                        : (DateTime?)null;
                    QuestionDateRangeBefore.Value = value.QuestionChangedRange.Before.HasValue
                        ? value.QuestionChangedRange.Before.Value.DateTime
                        : (DateTime?)null;
                }
                else
                {
                    QuestionDateRangeSelector.ClearSelection();
                }

                QuestionType.Value = value.QuestionType;

                QuestionDifficulty.EnsureDataBound();
                QuestionDifficulty.ValueAsInt = value.QuestionDifficulty;

                if (value.QuestionFlag.IsEmpty())
                    QuestionFlag.ClearSelection();
                else if (value.QuestionFlag == "None")
                    QuestionFlag.EnumValue = Indicator.None;
                else
                    QuestionFlag.FlagValue = value.QuestionFlag.ToEnum<FlagType>();
            }
        }

        public override void Clear()
        {
            QuestionAsset.Text = null;
            QuestionText.Text = null;
            QuestionCode.Text = null;
            QuestionReference.Text = null;
            QuestionTag.Text = null;
            QuestionBank.Text = null;
            QuestionDateRangeSelector.ClearSelection();
            QuestionDateRangeSince.Value = null;
            QuestionDateRangeBefore.Value = null;
            QuestionCompetencyTitle.Text = null;
            QuestionType.Value = null;
            QuestionDifficulty.ValueAsInt = null;
            RubricComboBox.ValueAsGuid = null;
            QuestionFlag.ClearSelection();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!IsPostBack)
            {
                QuestionDateRangeSelector.LoadItems(
                    DateRangeShortcut.Today,
                    DateRangeShortcut.Yesterday,
                    DateRangeShortcut.ThisWeek,
                    DateRangeShortcut.LastWeek,
                    DateRangeShortcut.ThisMonth,
                    DateRangeShortcut.LastMonth,
                    DateRangeShortcut.ThisYear,
                    DateRangeShortcut.LastYear);
                QuestionDateRangeSelector.Items.Add(new ComboBoxOption("Custom Dates", "Custom"));

                QuestionType.EnsureDataBound();

                for (var i = 0; i < QuestionType.Items.Count; i++)
                {
                    var option = (ComboBoxOption)QuestionType.Items[i];
                    if (option.Value.IsEmpty())
                        continue;

                    var type = option.Value.ToEnum<QuestionItemType>();
                    if (type.IsHotspot() && type != QuestionItemType.HotspotCustom)
                        option.Visible = false;
                }
            }

            base.OnLoad(e);
        }
    }
}