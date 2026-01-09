using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using InSite.Common.Web.UI;
using InSite.Domain.Banks;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class QuestionSetDetails : BaseUserControl
    {
        #region Enums

        public enum Tab
        {
            Questions,
            Set
        }

        #endregion

        #region Classes

        [Serializable]
        public class Filter
        {
            public string Keyword { get; set; }

            public bool IsEmpty => Keyword.IsEmpty();

            private static readonly byte _filterId = 85;

            public string Serialize()
            {
                return StringHelper.EncodeBase64Url(stream =>
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(_filterId);
                        writer.WriteNullable(Keyword);
                    }
                });
            }

            public static Filter Deserialize(string data)
            {
                Filter result = null;

                if (data.IsNotEmpty())
                {
                    try
                    {
                        result = StringHelper.DecodeBase64Url(data, stream =>
                        {
                            var filter = new Filter();

                            using (var reader = new BinaryReader(stream))
                            {
                                if (reader.ReadByte() != _filterId)
                                    return null;

                                filter.Keyword = reader.ReadStringNullable();
                            }

                            return filter;
                        });
                    }
                    catch
                    {
                        result = null;
                    }
                }

                return result ?? new Filter();
            }
        }

        #endregion

        #region Properties

        public Guid SetID => SetDetails.SetID;

        private Guid BankID
        {
            get => (Guid)ViewState[nameof(BankID)];
            set => ViewState[nameof(BankID)] = value;
        }

        private bool AllowEdit
        {
            get => (bool)ViewState[nameof(AllowEdit)];
            set => ViewState[nameof(AllowEdit)] = value;
        }

        public Filter DataFilter => (Filter)(ViewState[nameof(DataFilter)]
            ?? (ViewState[nameof(DataFilter)] = new Filter()));

        public int QuestionsCount
        {
            get => (int)(ViewState[nameof(QuestionsCount)] ?? 0);
            set => ViewState[nameof(QuestionsCount)] = value;
        }

        #endregion

        #region Fields

        private IReadOnlyList<Question> _filteredQuestions = null;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LoadQuestionsButton.Click += LoadQuestionsButton_Click;
        }

        #endregion

        #region Event handlers

        private void LoadQuestionsButton_Click(object sender, EventArgs e)
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            var set = bank?.FindSet(SetID);

            if (set != null)
            {
                var questions = ApplyFilter(set);

                QuestionRepeater.LoadData(questions, new QuestionRepeater.BindSettings
                {
                    AllowEdit = AllowEdit,
                    AllowAnalyse = true,
                    ShowProperties = bank.IsAdvanced
                        ? PropertiesVisibility.Advanced
                        : PropertiesVisibility.Basic,
                    ReturnUrl = GetReturnUrl()
                });
            }

            LoadQuestionsButton.Visible = false;
        }

        #endregion

        #region Data binding

        public void SetInputValues(Set set, bool showAllQuestions, bool canWrite)
        {
            BankID = set.Bank.Identifier;
            AllowEdit = canWrite;

            SetDetails.SetInputValues(set, canWrite);

            _filteredQuestions = ApplyFilter(set);

            QuestionsCount = _filteredQuestions.Count;

            QuestionNavItem.SetTitle("Questions", QuestionsCount);

            if (!showAllQuestions && _filteredQuestions.Count > 6)
            {
                LoadQuestionsButton.Visible = true;

                QuestionRepeater.LoadData(_filteredQuestions.Take(4), new QuestionRepeater.BindSettings
                {
                    AllowEdit = AllowEdit,
                    AllowAnalyse = true,
                    ShowProperties = set.Bank.IsAdvanced
                        ? PropertiesVisibility.Advanced
                        : PropertiesVisibility.Basic,
                    ReturnUrl = GetReturnUrl()
                });
            }
            else
            {
                LoadQuestionsButton.Visible = false;

                QuestionRepeater.LoadData(_filteredQuestions, new QuestionRepeater.BindSettings
                {
                    AllowEdit = AllowEdit,
                    AllowAnalyse = true,
                    ShowProperties = set.Bank.IsAdvanced
                        ? PropertiesVisibility.Advanced
                        : PropertiesVisibility.Basic,
                    ReturnUrl = GetReturnUrl()
                });
            }
        }

        private IReadOnlyList<Question> ApplyFilter(Set set)
        {
            if (DataFilter.IsEmpty)
                return set.Questions;

            return QuestionRepeater.ApplyFilter(set.Questions.AsQueryable(), DataFilter.Keyword).ToArray();
        }

        #endregion

        #region Methods (helpers)

        public void SetTab(Tab tab)
        {
            if (tab == Tab.Set)
                SetNavItem.IsSelected = true;
            else
                QuestionNavItem.IsSelected = true;
        }

        public Tab GetTab()
        {
            if (SetNavItem.IsSelected)
                return Tab.Set;
            else
                return Tab.Questions;
        }

        public bool ContainsQuestion(Guid id) => _filteredQuestions != null && _filteredQuestions.Any(x => x.Identifier == id);

        private ReturnUrl GetReturnUrl()
        {
            var data = $"bank&set={SetID}&panel=questions&tab=questions";

            if (!DataFilter.IsEmpty)
                data += "&filter=" + DataFilter.Serialize();

            return new ReturnUrl(data);
        }

        #endregion
    }
}