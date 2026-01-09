using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.UI.WebControls;

using Humanizer;

using InSite.Application.Banks.Write;
using InSite.Common.Web;
using InSite.Common.Web.Infrastructure;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Create : AdminBasePage
    {
        private enum UploadFileType
        {
            [Description("JSON File (*.json)")]
            Json,

            [Description("QTI File (*.xml)")]
            QTI
        }

        private string Action => Request.QueryString["action"];

        protected Guid? BankIdentifier => Guid.TryParse(Request.QueryString["bank"], out var result) ? result : (Guid?)null;

        private string QtiBankJson
        {
            get => (string)ViewState[nameof(QtiBankJson)];
            set => ViewState[nameof(QtiBankJson)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CreationType.AutoPostBack = true;
            CreationType.ValueChanged += CreationType_ValueChanged;

            BankTypeSelector.AutoPostBack = true;
            BankTypeSelector.ValueChanged += BankTypeSelector_ValueChanged;

            BankSelector.AutoPostBack = true;
            BankSelector.ValueChanged += BankSelector_ValueChanged;

            SaveButton.Click += SaveButton_Click;
            CancelButton.NavigateUrl = "/ui/admin/assessments/banks/search";

            UploadFileFormat.AutoPostBack = true;
            UploadFileFormat.ValueChanged += UploadFileFormat_ValueChanged;

            JsonFileUpload.FileUploaded += JsonFileUpload_FileUploaded;

            DownloadQtiBankDefinition.Click += DownloadQtiBankDefinition_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!CanEdit)
                HttpResponseHelper.Redirect("/ui/admin/assessments/home");

            if (IsPostBack)
                return;

            PageHelper.AutoBindHeader(this);

            CreationType.EnsureDataBound();
            CreationType.SetVisibleOptions(CreationTypeEnum.One, CreationTypeEnum.Duplicate, CreationTypeEnum.Upload);
            OnCreationTypeSelected();

            if (Action == "duplicate")
            {
                CreationType.ValueAsEnum = CreationTypeEnum.Duplicate;
                OnCreationTypeSelected();
                if (BankIdentifier.HasValue && BankIdentifier.Value != Guid.Empty)
                {
                    BankSelector.Value = BankIdentifier.Value;
                    OnCopyBankSelectorSelectedIndexChanged();
                }
            }

            OnBankTypeSelected();

            UploadFileFormat.LoadItems(UploadFileType.Json, UploadFileType.QTI);
        }

        private void CreationType_ValueChanged(object sender, EventArgs e)
        {
            OnCreationTypeSelected();
        }

        private void OnCreationTypeSelected()
        {
            var value = CreationType.ValueAsEnum;
            var isUpload = value == CreationTypeEnum.Upload;

            if (isUpload)
            {
                UploadFileFormat.Value = UploadFileType.Json.GetName();

                OnUploadFileFormatChanged();
            }

            if (value == CreationTypeEnum.One)
                MultiView.SetActiveView(OneView);
            else if (value == CreationTypeEnum.Duplicate)
                MultiView.SetActiveView(CopyView);
            else if (value == CreationTypeEnum.Upload)
                MultiView.SetActiveView(UploadView);
        }

        private void BankTypeSelector_ValueChanged(object sender, EventArgs e) => OnBankTypeSelected();

        private void OnBankTypeSelected()
        {
            var isAdvanced = BankTypeSelector.Value.ToEnum<BankType>() == BankType.Advanced;

            BankNameField.Visible = isAdvanced;
            AdvancedSettingsColumn.Visible = isAdvanced;
        }

        private void BankSelector_ValueChanged(object sender, EventArgs e)
        {
            OnCopyBankSelectorSelectedIndexChanged();
        }

        private void OnCopyBankSelectorSelectedIndexChanged()
        {
            if (BankSelector.HasValue)
            {
                var bank = ServiceLocator.BankSearch.GetBankState(BankSelector.Value.Value);

                DuplicateBankName.Text = bank.Name;
                DuplicateBankTitle.Text = bank.Name;
                DuplicateMajorVersion.Text = bank.Edition.Major;
                DuplicateMinorVersion.Text = bank.Edition.Minor;
                DuplicateBankTypeSelector.Value = bank.Type.GetName();
                DuplicateLevelType.Value = bank.Level.Type;
                DuplicateLevelNumber.ValueAsInt = bank.Level.Number;
            }
            else
            {
                DuplicateBankName.Text = null;
                DuplicateBankTitle.Text = null;
                DuplicateMajorVersion.Text = null;
                DuplicateMinorVersion.Text = null;
                DuplicateBankTypeSelector.Value = null;
                DuplicateLevelType.Value = null;
                DuplicateLevelNumber.Value = null;
            }
        }

        private void UploadFileFormat_ValueChanged(object sender, ComboBoxValueChangedEventArgs e) => OnUploadFileFormatChanged();

        private void JsonFileUpload_FileUploaded(object sender, EventArgs e)
        {
            var value = UploadFileFormat.Value.ToEnum<UploadFileType>();
            var isJson = value == UploadFileType.Json;
            var isQti = value == UploadFileType.QTI;

            if (isJson)
            {
                var text = JsonFileUpload.ReadFileText(Encoding.UTF8);

                if (string.IsNullOrWhiteSpace(text))
                {
                    CreatorStatus.AddMessage(AlertType.Error, "Uploaded file is empty");
                    return;
                }
                JsonInput.Text = text;
            }
            else if (isQti)
            {
                using (var stream = JsonFileUpload.OpenFile())
                {
                    var bank = new QtiHelper(CreatorStatus).Read(stream);
                    if (bank == null)
                        return;

                    QtiBankJson = JsonHelper.JsonExport(bank);

                    UploadSaveQtiPanel.Visible = true;

                    UploadSaveQtiExternalName.Text = bank.Content.Title.Default;
                    UploadSaveQtiInternalName.Text = bank.Name;

                    UploadSaveQtiSetCount.Text = bank.Sets.Count.ToString("n0");
                    UploadSaveQtiQuestionCount.Text = bank.Sets.Sum(x => x.Questions.Count).ToString("n0");
                    UploadSaveQtiOptionCount.Text = bank.Sets.Sum(x => x.Questions.Sum(y => y.Options.Count)).ToString("n0");
                    UploadSaveQtiSpecificationCount.Text = 0.ToString("n0");
                    UploadSaveQtiFormCount.Text = 0.ToString("n0");
                    UploadSaveQtiBankSize.Text = QtiBankJson.Length.Bytes().Humanize("0");
                }
            }
        }

        private void OnUploadFileFormatChanged()
        {
            var value = UploadFileFormat.Value.ToEnum<UploadFileType>();
            var isJson = value == UploadFileType.Json;
            var isQti = value == UploadFileType.QTI;

            UploadSaveJsonPanel.Visible = isJson;
            UploadSaveQtiPanel.Visible = false;
            QtiBankJson = null;

            if (isJson)
                JsonFileUploadLabel.Text = "Select and Upload Bank JSON File";
            else if (isQti)
                JsonFileUploadLabel.Text = "Select and Upload QTI File";
            else
                throw new NotImplementedException();
        }

        private void JsonFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".json", StringComparison.OrdinalIgnoreCase);
        }

        private void QtiFileUploadExtensionValidator_ServerValidate(object source, ServerValidateEventArgs e)
        {
            e.IsValid = e.Value == null || e.Value.EndsWith(".xml", StringComparison.OrdinalIgnoreCase);
        }

        private void UploadSaveQtiRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = QtiBankJson.IsNotEmpty();
        }

        private void DownloadQtiBankDefinition_Click(object sender, EventArgs e)
        {
            var bank = JsonConvert.DeserializeObject<BankState>(QtiBankJson);
            var filename = $"{FileHelper.AdjustFileName(bank.Name)}";
            var data = Encoding.UTF8.GetBytes(QtiBankJson);

            Page.Response.SendFile(filename, "json", data);
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var value = CreationType.ValueAsEnum;

            if (value == CreationTypeEnum.One)
                SaveNew();
            else if (value == CreationTypeEnum.Duplicate)
                SaveDuplicate();
            else if (value == CreationTypeEnum.Upload)
                SaveUpload();
        }

        private void SaveUpload()
        {
            if (!Page.IsValid)
                return;

            try
            {
                var format = UploadFileFormat.Value.ToEnum<UploadFileType>();
                var json = format == UploadFileType.Json
                    ? JsonInput.Text
                    : format == UploadFileType.QTI
                        ? QtiBankJson
                        : throw new NotImplementedException();

                var bank = JsonHelper.JsonImport<BankState>(json);

                bank.Comments.Clear();

                if (bank.Tenant != Organization.Identifier)
                {
                    bank.Attachments.Clear();

                    bank.Tenant = Organization.OrganizationIdentifier;
                }

                BankHelper.ResetGuidIdentifiers(bank, GetStandardHooks());

                AssignAssetNumbers(bank);
                RemoveGradebook(bank);

                ServiceLocator.SendCommand(new OpenBank(bank));

                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={bank.Identifier}", true);
            }
            catch (ThreadAbortException)
            {
                throw;
            }
            catch (JsonReaderException ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, $"The file you uploaded has an unexpected format. {ex.Message}");
            }
            catch (JsonSerializationException ex)
            {
                CreatorStatus.AddMessage(AlertType.Error, $"The file you uploaded contains an error. {ex.Message}");
            }
            catch (ApplicationError apperr)
            {
                if (apperr.Message == "Unexpected JSON object type")
                    CreatorStatus.AddMessage(AlertType.Error, $"The file you uploaded is of the wrong type.");
                else if (apperr.Message.StartsWith("Duplicate identifier found: "))
                    CreatorStatus.AddMessage(AlertType.Error, $"The file you uploaded contains entities with the same identifier: {apperr.Message.Substring(28)}.");
                else
                    throw;
            }
            catch (Exception ex)
            {
                if (ex is TargetInvocationException tiex && tiex.InnerException != null)
                    ex = tiex.InnerException;

                CreatorStatus.AddMessage(AlertType.Error, "An error occurred on the server side: " + ex.Message);
            }
        }

        private Dictionary<Guid, Guid> GetStandardHooks()
        {
            var hooks = new Dictionary<Guid, Guid>();

            if (string.IsNullOrWhiteSpace(StandardHooks.Text))
                return hooks;

            var standards = StandardSearch.Bind(
                x => new { x.StandardIdentifier, x.StandardHook },
                x => x.OrganizationIdentifier == Organization.Identifier
                ).ToList();

            try
            {
                var lines = StringHelper.Split(StandardHooks.Text, '\n');
                foreach (var line in lines)
                {
                    var parts = StringHelper.Split(line, new[] { '\t', ',' });
                    if (parts.Length != 2)
                        continue;

                    var id = parts[0].Trim('\"');
                    var hook = parts[1].Trim('\"');
                    if (Guid.TryParse(id, out var key) && !string.IsNullOrEmpty(hook))
                    {
                        var standard = standards.FirstOrDefault(x => x.StandardHook == hook);
                        if (standard != null)
                            hooks.Add(key, standard.StandardIdentifier);
                    }
                }
            }
            catch
            {
                // Ignore exceptions for now.
            }

            return hooks;
        }

        private void RemoveGradebook(BankState bank)
        {
            var forms = bank.Specifications.SelectMany(x => x.Forms);
            foreach (var form in forms)
                form.Gradebook = null;

            var questions = bank.Sets.SelectMany(s => s.Questions);
            foreach (var q in questions)
            {
                q.GradeItems.Clear();

                if (q.Likert == null)
                    return;

                foreach (var row in q.Likert.Rows)
                    row.GradeItems.Clear();
            }
        }

        private void SaveNew()
        {
            if (!Page.IsValid)
                return;

            var bank = CreateQuestionBank();

            ServiceLocator.SendCommand(new OpenBank(bank));

            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={bank.Identifier}");
        }

        private void SaveDuplicate()
        {
            if (!Page.IsValid)
                return;

            if (!BankSelector.HasValue)
                return;

            var bankId = BankSelector.Value.Value;
            if (bankId == Guid.Empty)
                return;

            var bank = CreateDuplicateQuestionBank(ServiceLocator.BankSearch.GetBankState(bankId));

            bank.Tenant = Organization.OrganizationIdentifier;

            BankHelper.ResetGuidIdentifiers(bank, null);
            BankHelper.AssignAssetNumbers(bank, count => Sequence.IncrementMany(bank.Tenant, SequenceType.Asset, count));

            ServiceLocator.SendCommand(new OpenBank(bank));

            HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/outline?bank={bank.Identifier}", true);
        }

        private BankState CreateQuestionBank()
        {
            var b = new BankState
            {
                Identifier = UniqueIdentifier.Create(),
                Type = BankTypeSelector.Value.ToEnum<BankType>(),
                Level = new Level
                {
                    Type = LevelType.Value.NullIfEmpty(),
                    Number = LevelNumber.ValueAsInt
                },
                Standard = Guid.Empty,
                Tenant = Organization.OrganizationIdentifier,
                Edition = new Edition(EditionMajor.Text, EditionMinor.Text)
            };

            b.Name = b.IsAdvanced ? BankName.Text : BankTitle.Text;
            b.Content.Title.Default = BankTitle.Text;

            AssignAssetNumbers(b);

            return b;
        }

        private BankState CreateDuplicateQuestionBank(BankState bank)
        {
            bank.Type = DuplicateBankTypeSelector.Value.ToEnum<BankType>();
            bank.Level = new Level
            {
                Type = DuplicateLevelType.Value.NullIfEmpty(),
                Number = DuplicateLevelNumber.ValueAsInt
            };
            bank.Edition = new Edition(DuplicateMajorVersion.Text, DuplicateMinorVersion.Text);
            bank.Name = bank.IsAdvanced ? DuplicateBankName.Text : DuplicateBankTitle.Text;
            bank.Content.Title.Default = DuplicateBankTitle.Text;

            return bank;
        }

        private static void AssignAssetNumbers(BankState bank)
        {
            BankHelper.AssignAssetNumbers(bank, count => Sequence.IncrementMany(Organization.OrganizationIdentifier, SequenceType.Asset, count));
        }

        private class QtiHelper
        {
            private Alert _status;
            private int _optionNumber;

            public QtiHelper(Alert status)
            {
                _status = status;
            }

            public BankState Read(Stream content)
            {
                var result = new QTIReader().Read(content);

                if (result.Error.IsNotEmpty())
                {
                    var message = result.ErrorLine >= 0 ? $"Error at line {result.ErrorLine}: {result.Error}" : result.Error;
                    _status.AddMessage(AlertType.Error, message);
                    return null;
                }

                if (result.Assessments.Count == 0)
                {
                    _status.AddMessage(AlertType.Error, "The file has no data");
                    return null;
                }

                var assessment = result.Assessments[0];

                var today = DateTime.Today;

                var bank = new BankState
                {
                    Tenant = Organization.OrganizationIdentifier,
                    Identifier = UniqueIdentifier.Create(),
                    Level = new Level(),
                    Name = FileHelper.AdjustFileName(assessment.Title).ToUpper(),
                    Standard = Guid.Empty,
                    Edition = new Edition(today.Year, today.Month)
                };

                bank.Content.Title.Default = assessment.Title;

                _optionNumber = 1;

                AddSets(bank, assessment);

                return bank;
            }

            private void AddSets(BankState bank, QTIReader.Assessment assessment)
            {
                foreach (var section in assessment.Sections)
                {
                    var set = new Set
                    {
                        Identifier = UniqueIdentifier.Create(),
                        Standard = Guid.Empty,
                        Name = section.Title
                    };

                    bank.Sets.Add(set);

                    AddQuestions(set, section);
                }
            }

            private void AddQuestions(Set set, QTIReader.Section section)
            {
                foreach (var qtiQuestion in section.Questions)
                {
                    QuestionItemType questionItemType;
                    QuestionCalculationMethod calculationMethod;

                    switch (qtiQuestion.QuestionType)
                    {
                        case QuestionType.MultipleChoice:
                        case QuestionType.TrueFalse:
                            questionItemType = QuestionItemType.SingleCorrect;
                            calculationMethod = QuestionCalculationMethod.Default;
                            break;
                        case QuestionType.MultipleAnswers:
                            questionItemType = QuestionItemType.MultipleCorrect;
                            calculationMethod = QuestionCalculationMethod.AllOrNothing;
                            break;
                        default:
                            continue;
                    }

                    var question = new Question
                    {
                        Identifier = UniqueIdentifier.Create(),
                        Type = questionItemType,
                        Standard = Guid.Empty,
                        SubStandards = null,
                        CutScore = null,
                        Points = (decimal)qtiQuestion.PointsPossible,
                        CalculationMethod = calculationMethod
                    };

                    question.Content.Title.Default = qtiQuestion.QuestionText;
                    question.Classification.Reference = qtiQuestion.Identifier;

                    set.Questions.Add(question);

                    AddOptions(question, qtiQuestion);
                }
            }

            private void AddOptions(Question question, QTIReader.Question qtiQuestion)
            {
                foreach (var choice in qtiQuestion.Responses[0].Choices)
                {
                    var option = new Option
                    {
                        Number = _optionNumber++,
                        Standard = Guid.Empty,
                        CutScore = null,
                        Points = (decimal)choice.Score,
                        IsTrue = choice.Score > 0
                    };

                    option.Content.Title.Default = choice.Label;

                    question.Options.Add(option);
                }
            }
        }
    }
}