using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Admin.Assessments.Questions.Controls;
using InSite.Admin.Assessments.Sets.Utilities;
using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.Web.Data;

using Shift.Common;
using Shift.Constant;

namespace InSite.Admin.Assessments.Sets.Forms
{
    public partial class Add : AdminBasePage, IHasParentLinkParameters
    {
        #region Constants

        protected sealed class FormatInfo
        {
            public static FormatInfo ShiftiQMarkdown { get; }
            public static FormatInfo ICEMSAnswerKey { get; }
            public static FormatInfo LxrMerge { get; }

            #region Properties

            public string ID => _id;
            public string Title => _title;
            public string Label => "Upload " + _title;
            public ReadOnlyCollection<string> Extensions => _exts;

            #endregion

            #region Fields

            private string _id;
            private string _title;
            private ReadOnlyCollection<string> _exts;

            #endregion

            #region Construction 

            private FormatInfo()
            {

            }

            static FormatInfo()
            {
                ShiftiQMarkdown = new FormatInfo
                {
                    _id = "Markdown",
                    _title = "Shift iQ Markdown",
                    _exts = Array.AsReadOnly(new[] { ".md", ".txt" }),
                };
                ICEMSAnswerKey = new FormatInfo
                {
                    _id = "ICEMSAnswerKey",
                    _title = "ICEMS Answer Key",
                    _exts = Array.AsReadOnly(new[] { ".xml" }),
                };
                LxrMerge = new FormatInfo
                {
                    _id = "LxrMerge",
                    _title = "LXR Merge",
                    _exts = Array.AsReadOnly(new[] { ".lxrmerge", ".txt" }),
                };
            }

            #endregion
        }

        private static readonly Regex LxrMergeQuestionTagPattern
            = new Regex(@"(?<Area>[A-Za-z0-9\-\.]+)[ ]+(?<Competency>[A-Za-z0-9\-\.]+)|(?<Area>[a-zA-Z]+)[\-\.]?(?<Competency>\d+)",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        #region Properties

        private Guid BankID => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : Guid.Empty;

        private Question[] UploadedQuestions
        {
            get => (Question[])ViewState[nameof(UploadedQuestions)];
            set => ViewState[nameof(UploadedQuestions)] = value;
        }

        private Set[] UploadedSets
        {
            get => (Set[])ViewState[nameof(UploadedSets)];
            set => ViewState[nameof(UploadedSets)] = value;
        }

        #endregion

        #region Fields

        private Dictionary<Question, int> _questionInfo = null;

        #endregion

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            MarkdownFileUploadExtensionValidator.ServerValidate += MarkdownFileUploadExtensionValidator_ServerValidate;
            ICEMSAnswerKeyFileUploadExtensionValidator.ServerValidate += ICEMSAnswerKeyFileUploadExtensionValidator_ServerValidate;
            LxrMergeFileUploadExtensionValidator.ServerValidate += LxrMergeFileUploadExtensionValidator_ServerValidate;

            UploadButton.Click += UploadButton_Click;
            ClearUploadButton.Click += ClearUploadButton_Click;

            SetRepeater.ItemDataBound += SetRepeater_ItemDataBound;

            SaveButton1.Click += SaveButton_Click;
            SaveButton2.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                RedirectToFinder();

            if (!IsPostBack)
                Open();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var format = FileFormat.Value;

            if (format == FormatInfo.ShiftiQMarkdown.ID)
                UploadLabel.Text = FormatInfo.ShiftiQMarkdown.Label;
            else if (format == FormatInfo.ICEMSAnswerKey.ID)
                UploadLabel.Text = FormatInfo.ICEMSAnswerKey.Label;
            else if (format == FormatInfo.LxrMerge.ID)
                UploadLabel.Text = FormatInfo.LxrMerge.Label;

            MarkdownFileUploadExtensionValidator.Enabled = format == FormatInfo.ShiftiQMarkdown.ID;
            ICEMSAnswerKeyFileUploadExtensionValidator.Enabled = format == FormatInfo.ICEMSAnswerKey.ID;
            LxrMergeFileUploadExtensionValidator.Enabled = format == FormatInfo.LxrMerge.ID;
        }

        #endregion

        #region Event handlers

        private void MarkdownFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || FormatInfo.ShiftiQMarkdown.Extensions.Any(x => e.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private void ICEMSAnswerKeyFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || FormatInfo.ICEMSAnswerKey.Extensions.Any(x => e.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private void LxrMergeFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || FormatInfo.LxrMerge.Extensions.Any(x => e.Value.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            if (FileUpload.PostedFile == null || FileUpload.PostedFile.ContentLength == 0)
            {
                CreatorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                return;
            }

            ClearUploads();

            try
            {
                var hasData = false;
                var format = FileFormat.Value;

                if (format == FormatInfo.ShiftiQMarkdown.ID)
                {
                    UploadedQuestions = ReadFromMarkdown(FileUpload.PostedFile.InputStream);
                    hasData = UploadedQuestions.Length > 0;
                }
                else if (format == FormatInfo.ICEMSAnswerKey.ID)
                {
                    var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                    UploadedSets = ReadFromICEMS(bank.Standard, FileUpload.PostedFile.InputStream);
                    hasData = UploadedSets.Length > 0;
                }
                else if (format == "LxrMerge")
                {
                    var bank = ServiceLocator.BankSearch.GetBankState(BankID);
                    UploadedSets = ReadFromLxrMerge(bank.Standard, FileUpload.PostedFile.InputStream, Encoding.GetEncoding(Convert.ToInt32(FileEncoding.SelectedValue)));
                    hasData = UploadedSets.Length > 0;
                }

                if (!hasData)
                {
                    ClearUploads();
                    CreatorStatus.AddMessage(AlertType.Error, $@"The file has no data.");
                }
            }
            catch (ApplicationError ex)
            {
                ClearUploads();
                CreatorStatus.AddMessage(AlertType.Error, $@"An unexpected error occurred. " + ex.Message);
            }

            OnQuestionsUploaded();

            void ClearUploads()
            {
                UploadedSets = null;
                UploadedQuestions = null;
            }
        }

        private void ClearUploadButton_Click(object sender, EventArgs e)
        {
            UploadedSets = null;
            UploadedQuestions = null;

            OnQuestionsUploaded();
        }

        private void OnQuestionsUploaded()
        {
            if (UploadedQuestions != null)
            {
                UploadColumn.Visible = false;
                QuestionsColumn.Visible = true;

                _questionInfo = UploadedQuestions
                    .Select((x, i) => new Tuple<Question, int>(x, i))
                    .ToDictionary(x => x.Item1, x => x.Item2);

                QuestionRepeater.Visible = true;
                QuestionRepeater.LoadData(UploadedQuestions, new QuestionRepeater.BindSettings
                {
                    GetBankIndex = GetQuestionBankIndex
                });

                SetRepeater.Visible = false;

                QuestionsColumnHeader.InnerText = "Question".ToQuantity(_questionInfo.Count, "n0");
            }
            else if (UploadedSets != null)
            {
                UploadColumn.Visible = false;
                QuestionsColumn.Visible = true;

                _questionInfo = UploadedSets
                    .SelectMany(x => x.Questions)
                    .Select((x, i) => new Tuple<Question, int>(x, i))
                    .ToDictionary(x => x.Item1, x => x.Item2);

                QuestionRepeater.Visible = false;

                SetRepeater.Visible = true;
                SetRepeater.DataSource = UploadedSets;
                SetRepeater.DataBind();

                QuestionsColumnHeader.InnerText = "Question".ToQuantity(_questionInfo.Count, "n0");
            }
            else
            {
                UploadColumn.Visible = true;
                QuestionsColumn.Visible = false;
            }
        }

        private void SetRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var item = e.Item;
            var set = (Set)item.DataItem;

            var questionRepeater = (QuestionRepeater)item.FindControl("QuestionRepeater");
            questionRepeater.LoadData(set.Questions, new QuestionRepeater.BindSettings
            {
                ShowProperties = PropertiesVisibility.None,
                GetBankIndex = GetQuestionBankIndex
            });
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            Save();
        }

        #endregion

        #region Database operations

        private void Open()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankID);
            if (bank == null)
                RedirectToFinder();

            SetInputValues(bank);
        }

        private void Save()
        {
            Set[] sets;

            if (UploadedSets.IsNotEmpty())
            {
                sets = UploadedSets;
            }
            else
            {
                var set = new Set { Name = SetName.Text };

                if (UploadedQuestions != null)
                    set.Questions.AddRange(UploadedQuestions);

                sets = new[] { set };
            }

            var questionsCount = sets.Sum(x => x.Questions.Count);
            var assetNumbers = questionsCount > 0 ? new Queue<int>(Sequence.IncrementMany(Organization.OrganizationIdentifier, SequenceType.Asset, questionsCount)) : null;

            foreach (var set in sets)
            {
                set.Identifier = UniqueIdentifier.Create();

                ServiceLocator.SendCommand(new AddSet(BankID, set.Identifier, set.Name, set.Standard));

                Guid? defaultCompetency = null;

                foreach (var question in set.Questions)
                {
                    question.Identifier = UniqueIdentifier.Create();
                    question.Asset = assetNumbers.Dequeue();

                    if (question.Standard == Guid.Empty && set.Standard != Guid.Empty)
                    {
                        if (!defaultCompetency.HasValue)
                        {
                            var parentId = StandardSearch.BindFirst(x => (Guid?)x.StandardIdentifier, x => x.StandardIdentifier == set.Standard);

                            if (parentId.HasValue)
                            {
                                var asset = StandardFactory.Create(StandardType.Competency);
                                asset.StandardIdentifier = UniqueIdentifier.Create();
                                asset.ParentStandardIdentifier = parentId.Value;
                                asset.ContentTitle = $"Competency placeholder for set '{set.Name}'";

                                StandardStore.Insert(asset);

                                defaultCompetency = asset.StandardIdentifier;
                            }
                            else
                            {
                                defaultCompetency = Guid.Empty;
                            }
                        }

                        question.Standard = defaultCompetency.Value;
                        question.SubStandards = null;
                    }

                    ServiceLocator.SendCommand(new AddQuestion(BankID, set.Identifier, question));
                }
            }

            if (sets.Length == 1)
                RedirectToReader(sets[0].Identifier);
            else
                RedirectToReader();
        }

        #endregion

        #region File reading

        private Question[] ReadFromMarkdown(Stream stream)
        {
            var result = new List<Question>();
            var items = MarkdownQuestion.Read(stream);

            foreach (var item in items)
            {
                var question = new Question
                {
                    Points = 1,
                    Type = QuestionItemType.SingleCorrect,
                };

                question.Content.Title.Default = item.Text;

                if (!string.IsNullOrEmpty(item.Feedback))
                    question.Content.Rationale.Default = item.Feedback;

                foreach (var itemOption in item.Options)
                {
                    var option = itemOption.ToBankOption();
                    question.Options.Add(option);
                    option.Question = question;
                }

                result.Add(question);
            }

            return result.ToArray();
        }

        private Set[] ReadFromICEMS(Guid bankStandard, Stream stream)
        {
            var result = new List<Set>();

            var file = IcemsFile.Read(stream);

            foreach (var group in file.Groups)
            {
                if (group.Questions.Count == 0)
                    continue;

                var set = new Set();

                if (file.Version == 2)
                {
                    if (GetGacIdentifierByTitle(bankStandard, group.Name, out var gacId, out var code))
                    {
                        set.Name = $"Items for GAC {code ?? "??"}";
                        set.Standard = gacId;
                    }
                    else
                    {
                        set.Name = SetName.Text;
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(group.Label))
                    {
                        set.Name = $"Items for GAC {group.Label}";
                        if (GetGacIdentifierByCode(bankStandard, group.Label, out var gacId))
                            set.Standard = gacId;
                    }
                    else
                    {
                        set.Name = SetName.Text;
                    }
                }

                foreach (var item in group.Questions)
                {
                    var question = new Question
                    {
                        Points = 1,
                        Type = QuestionItemType.SingleCorrect,
                    };

                    question.Content.Title.Default = item.Text;
                    question.Classification.Code = group.Name;
                    question.Classification.Tag = group.Label;

                    foreach (var itemOption in item.Options)
                    {
                        var option = itemOption.ToBankOption();
                        question.Options.Add(option);
                        option.Question = question;
                    }

                    set.Questions.Add(question);
                    question.Set = set;
                }

                result.Add(set);
            }

            return result.ToArray();

            bool GetGacIdentifierByCode(Guid parent, string code, out Guid gacId)
            {
                gacId = Guid.Empty;

                if (!string.IsNullOrEmpty(code))
                {
                    var gac = StandardSearch.BindFirst(
                        x => new { x.StandardIdentifier },
                        x => x.Parent.StandardIdentifier == parent && x.StandardType == Shift.Constant.StandardType.Area && x.Code == code);

                    if (gac != null)
                        gacId = gac.StandardIdentifier;
                }

                return gacId != Guid.Empty;
            }

            bool GetGacIdentifierByTitle(Guid parent, string title, out Guid gacId, out string code)
            {
                gacId = Guid.Empty;
                code = null;

                if (!string.IsNullOrEmpty(title))
                {
                    var gac = StandardSearch.BindFirst(
                        x => new { x.StandardIdentifier, x.Code },
                        x => x.Parent.StandardIdentifier == parent
                          && x.StandardType == Shift.Constant.StandardType.Area
                          && CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title) == title);

                    if (gac != null)
                    {
                        gacId = gac.StandardIdentifier;
                        code = gac.Code;
                    }
                }

                return gacId != Guid.Empty;
            }
        }

        private Set[] ReadFromLxrMerge(Guid bankStandard, Stream stream, Encoding encoding)
        {
            var result = new List<Set>();

            Set lastSet = null;
            var items = LxrMergeQuestion.Read(stream, encoding);

            foreach (var item in items)
            {
                var question = new Question
                {
                    Points = 1,
                    Type = QuestionItemType.SingleCorrect,
                };

                question.Content.Title = new MultilingualString { Default = item.Text };

                question.Classification.Code = item.Code;
                question.Classification.Tag = item.Tag;

                if (int.TryParse(item.Taxonomy, out int taxonomy))
                    question.Classification.Taxonomy = taxonomy;

                question.Classification.LikeItemGroup = item.LikeItemGroup;
                question.Classification.Reference = item.Reference;

                question.Content.RationaleOnCorrectAnswer.Default = item.CorrectRationale;
                question.Content.RationaleOnIncorrectAnswer.Default = item.IncorrectRationale;

                if (int.TryParse(item.Difficulty, out int difficulty))
                    question.Classification.Difficulty = difficulty;

                var gac = "?";
                var competency = "?";

                if (!string.IsNullOrEmpty(item.Competency))
                {
                    var match = LxrMergeQuestionTagPattern.Match(item.Competency);
                    if (match.Success)
                    {
                        gac = match.Groups[1].Value;
                        competency = match.Groups[2].Value;
                    }
                }

                foreach (var itemOption in item.Options)
                {
                    var option = itemOption.ToBankOption();
                    question.Options.Add(option);
                    option.Question = question;
                }

                var setName = $"Items for {Shift.Constant.StandardType.Area} {gac}";

                if (lastSet == null || lastSet.Name != setName)
                {
                    lastSet = result.Find(x => x.Name == setName);
                    if (lastSet == null)
                        result.Add(lastSet = new Set
                        {
                            Name = setName,
                            Standard = GetStandardIdentifier(bankStandard, Shift.Constant.StandardType.Area, gac)
                        });
                }

                question.Standard = GetStandardIdentifier(lastSet.Standard, Shift.Constant.StandardType.Competency, competency);
                question.SubStandards = null;

                lastSet.Questions.Add(question);
                question.Set = lastSet;
            }

            return result.ToArray();

            Guid GetStandardIdentifier(Guid parent, string subtype, string code) =>
                StandardSearch.BindFirst(x => (Guid?)x.StandardIdentifier, x => x.Parent.StandardIdentifier == parent && x.StandardType == subtype && x.Code == code) ?? Guid.Empty;
        }

        #endregion

        #region Setting and getting input values

        private void SetInputValues(BankState bank)
        {
            var title =
                $"{(bank.Content.Title?.Default).IfNullOrEmpty(bank.Name)} <span class=\"form-text\">Asset #{bank.Asset}</span>";

            PageHelper.AutoBindHeader(this, null, title);

            BankDetails.BindBank(bank);

            FileFormat.LoadItems(
                new[]
                {
                    FormatInfo.ShiftiQMarkdown,
                    FormatInfo.ICEMSAnswerKey,
                    FormatInfo.LxrMerge
                },
                "ID", "Title");

            OnQuestionsUploaded();

            CancelButton.NavigateUrl = GetReaderUrl();
        }

        #endregion

        #region Methods (helpers)

        private int GetQuestionBankIndex(Question q) => _questionInfo[q];

        #endregion

        #region IHasParentLinkParameters

        private void RedirectToFinder() =>
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

        private void RedirectToReader(Guid? set = null)
        {
            var url = GetReaderUrl(set);

            HttpResponseHelper.Redirect(url, true);
        }

        private string GetReaderUrl(Guid? set = null)
        {
            return new ReturnUrl().GetReturnUrl($"set={set}")
                ?? $"/ui/admin/assessments/banks/outline?bank={BankID}";
        }

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"bank={BankID}"
                : null;
        }

        #endregion
    }
}