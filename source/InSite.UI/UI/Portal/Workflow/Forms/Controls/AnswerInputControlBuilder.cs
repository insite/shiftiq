using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using Humanizer;

using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Responses.Write;
using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;
using InSite.Domain.Surveys.Sessions;
using InSite.UI.Layout.Lobby;

using Shift.Common;
using Shift.Constant;

using CustomValidator = System.Web.UI.WebControls.CustomValidator;
using InSiteCheckBoxList = InSite.Common.Web.UI.CheckBoxList;
using InSiteRadioButtonList = InSite.Common.Web.UI.RadioButtonList;
using SystemListItem = System.Web.UI.WebControls.ListItem;
using TextBoxMode = Shift.Constant.TextBoxMode;

namespace InSite.UI.Portal.Workflow.Forms.Controls
{
    public class AnswerInputControlBuilder
    {
        private readonly SubmissionSessionState _current;
        private readonly GlossaryHelper _glossary;

        private readonly Dictionary<string, Control> _customValidationMapping = new Dictionary<string, Control>();

        public AnswerInputControlBuilder(SubmissionSessionState current, GlossaryHelper glossary)
        {
            _current = current;
            _glossary = glossary;
        }

        private void InputRadioListValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var controlToValidate = (InSiteRadioButtonList)_customValidationMapping[((Control)source).ID];
            args.IsValid = controlToValidate.Items.Cast<SystemListItem>().Any(x => x.Selected);
        }

        private void InputRadioTableValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var controlToValidate = (Common.Web.UI.RadioButtonTable)_customValidationMapping[((Control)source).ID];
            args.IsValid = controlToValidate.Rows.All(row => row.Items.Any(item => item.Selected));
        }

        public IEnumerable<Control> CreateInputControls(Guid sessionIdentifier, SurveyQuestion question, string answer, AnswerItem item, bool consent = false)
        {
            var inputID = $"Question_{question.Sequence}";

            if (item.AnswerInputType == SurveyQuestionType.BreakQuestion)
            {
                yield return new System.Web.UI.WebControls.Literal();
            }
            else if (item.AnswerInputType == SurveyQuestionType.CheckList)
            {
                var input = new InSiteCheckBoxList
                {
                    ID = inputID,
                    AutoPostBack = true,
                    DisableTranslation = true,
                };

                if (question.ListSelectionRange.Enabled)
                {
                    input.Attributes["data-list-min"] = question.ListSelectionRange.Min.ToString();
                    input.Attributes["data-list-max"] = question.ListSelectionRange.Max.ToString();
                }

                input.SelectedIndexChanged += CheckListOptionChanged;
                BindList(question, input);
                yield return input;

                if (question.ListSelectionRange.Enabled)
                {
                    if (question.ListSelectionRange.Min.HasValue)
                    {
                        var minValidator = new Common.Web.UI.CustomValidator
                        {
                            ID = inputID + "_MinValidator",
                            Display = ValidatorDisplay.None,
                            ClientValidationFunction = "answerPage.onListRangeMinValidation",
                            ErrorMessage = $"Select at least {"option".ToQuantity(question.ListSelectionRange.Min.Value)}"
                        };

                        yield return minValidator;
                    }

                    if (question.ListSelectionRange.Max.HasValue)
                    {
                        var maxValidator = new Common.Web.UI.CustomValidator
                        {
                            ID = inputID + "_MaxValidator",
                            Display = ValidatorDisplay.None,
                            ClientValidationFunction = "answerPage.onListRangeMaxValidation",
                            ErrorMessage = $"Maximum number of selectable options exceeded"
                        };

                        yield return maxValidator;
                    }
                }
            }
            else if (item.AnswerInputType == SurveyQuestionType.Comment)
            {
                var input = new Common.Web.UI.TextBox
                {
                    ID = inputID,
                    Text = answer,
                    TextMode = TextBoxMode.MultiLine,
                    MaxLength = question.TextCharacterLimit > 0 ? question.TextCharacterLimit.Value : 200
                };
                if (question.TextLineCount > 1)
                    input.Rows = question.TextLineCount.Value;
                yield return input;

                if (question.IsRequired)
                    yield return CreateRequiredValidator();
            }
            else if (item.AnswerInputType == SurveyQuestionType.Date)
            {
                var input = new DateSelector
                {
                    ID = inputID,
                    Value = answer.IsNotEmpty() && DateTime.TryParse(answer, ValueConverter.DefaultCulture, DateTimeStyles.None, out var value)
                        ? value
                        : (DateTime?)null,
                };
                yield return input;

                if (question.IsRequired)
                    yield return CreateRequiredValidator();
            }
            else if (item.AnswerInputType == SurveyQuestionType.Number)
            {
                var input = new HtmlInputText
                {
                    ID = inputID,
                    Value = answer,
                };

                input.Attributes["type"] = "number";
                input.Attributes.Add("class", "form-control");

                if (question.NumberEnableAutoCalc)
                {
                    input.Attributes["readonly"] = "readonly";
                    input.Attributes["data-num-autocalc"] = "1";

                    if (question.NumberAutoCalcQuestions.IsNotEmpty())
                        input.Attributes["data-questions"] = "[" + string.Join(",", question.NumberAutoCalcQuestions.Select(x => "\"" + x.ToString() + "\"")) + "]";
                }
                else if (question.NumberEnableNotApplicable)
                {
                    input.Attributes["type"] = "text";
                    input.Attributes["data-num-permit"] = question.Form.Tenant == OrganizationIdentifiers.NCSHA ? "N/AP;N/AV" : "N/A";
                }

                yield return input;

                if (question.IsRequired)
                    yield return CreateRequiredValidator();
            }
            else if (item.AnswerInputType == SurveyQuestionType.RadioList)
            {
                var input = new InSiteRadioButtonList
                {
                    ID = inputID,
                    AutoPostBack = true,
                    DisableTranslation = true,
                };
                input.SelectedIndexChanged += RadioListOptionChanged;
                BindList(question, input);
                yield return input;

                if (question.IsRequired)
                {
                    var validator = new CustomValidator
                    {
                        ID = inputID + "_Validator",
                        Display = ValidatorDisplay.None,
                        ClientValidationFunction = "controlBuilder.validateRadioList",
                    };

                    validator.ServerValidate += InputRadioListValidator_ServerValidate;

                    _customValidationMapping.Add(validator.ClientID, input);

                    yield return validator;
                }
            }
            else if (item.AnswerInputType == SurveyQuestionType.Likert)
            {
                var input = new Common.Web.UI.RadioButtonTable
                {
                    ID = inputID,
                    DisableColumnHeadingWrap = question.ListDisableColumnHeadingWrap,
                };
                foreach (var list in question.Options.Lists)
                {
                    var title = "-";

                    if (list.Content != null)
                    {
                        var value = list.Content.Title.GetText(_current.Language, true);
                        if (value.IsNotEmpty())
                            title = _glossary.Process(question.Identifier, ContentLabel.Title, value);
                    }

                    input.AddRow(title, CreateListItemArray(question.Identifier, list.Items, true));
                }
                input.AutoPostBack = true;
                input.CheckedChanged += OptionItemChanged;
                input.DataBind();
                yield return input;

                if (question.IsRequired)
                {
                    var validator = new CustomValidator
                    {
                        ID = inputID + "_Validator",
                        Display = ValidatorDisplay.None,
                        ClientValidationFunction = "controlBuilder.validateRadioTable",
                    };

                    validator.ServerValidate += InputRadioTableValidator_ServerValidate;

                    _customValidationMapping.Add(validator.ClientID, input);

                    yield return validator;
                }
            }
            else if (item.AnswerInputType == SurveyQuestionType.Selection)
            {
                var list = question.Options.Lists.Single();
                var inputItems = CreateListItemArray(question.Identifier, list.Items, false);
                var input = new FormComboBox
                {
                    ID = inputID,
                    AllowBlank = true,
                    EmptyMessage = Translate("Please select one") + "...",
                    ListIdentifier = list.Identifier
                };
                input.LoadItems(inputItems);
                input.Value = inputItems.FirstOrDefault(x => x.Selected)?.Value;
                input.AutoPostBack = true;
                input.ValueChanged += OptionItemChanged;
                input.DataBind();
                yield return input;

                if (question.IsRequired)
                    yield return CreateRequiredValidator();
            }
            else if (item.AnswerInputType == SurveyQuestionType.Text)
            {
                var input = new Common.Web.UI.TextBox
                {
                    ID = inputID,
                    Text = answer,
                };
                yield return input;

                if (question.IsRequired)
                    yield return CreateRequiredValidator();
            }
            else if (item.AnswerInputType == SurveyQuestionType.Upload)
            {
                var input = new FormFileUpload
                {
                    ID = inputID,
                    SessionIdentifier = sessionIdentifier,
                    QuestionIdentifier = question.Identifier,
                    ExistingRelativeUrls = answer,
                    IsRequired = question.IsRequired
                };
                input.RemoveClicked += FileUpload_RemoveClicked;
                input.DataBind();
                yield return input;
            }

            yield return new System.Web.UI.WebControls.Literal { Text = $"<span class='d-none badge bg-info'>{item.AnswerInputType}</span>" };

            Control CreateRequiredValidator()
            {
                return new Common.Web.UI.RequiredValidator
                {
                    ID = inputID + "_Validator",
                    ControlToValidate = inputID,
                    Display = ValidatorDisplay.None,
                    RenderMode = ValidatorRenderModeEnum.Exclamation,
                    ErrorMessage = "This question is mandatory"
                };
            }
        }

        public Control CreateOtherControl(SurveyQuestion question, string answer)
        {
            var otherID = $"Other_{question.Sequence}";

            var other = new Common.Web.UI.TextBox
            {
                ID = otherID,
                Text = answer,
                EmptyMessage = "Other",
                CssClass = "form-control mt-3"
            };

            return other;
        }

        private void BindList(SurveyQuestion question, ListControl list)
        {
            list.Items.Clear();

            var items = CreateListItemArray(question.Identifier, question.Options.Lists.Single().Items, true);
            foreach (var item in items.Items)
            {
                list.Items.Add(new SystemListItem
                {
                    Text = item.Text,
                    Value = item.Value,
                    Selected = item.Selected
                });
            }
        }

        private ListItemArray CreateListItemArray(Guid questionId, List<SurveyOptionItem> items, bool allowTermLinks)
        {
            var selections = _current.Session.QResponseOptions
                .OrderBy(x => x.OptionSequence)
                .Where(x => items.Any(item => item.Identifier == x.SurveyOptionIdentifier))
                .ToList();

            var list = new List<Shift.Common.ListItem>();

            foreach (var item in items)
            {
                var i = new Shift.Common.ListItem
                {
                    Index = GetItemIndex(item),
                    Value = item.Identifier.ToString(),
                    Text = "-",
                    Selected = IsSelected(item.Identifier)
                };

                if (item.Content?.Title != null)
                {
                    var value = item.Content.Title.GetText(_current.Language, true);
                    if (value.IsNotEmpty())
                        i.Text = allowTermLinks
                            ? _glossary.Process(questionId, ContentLabel.Title, value)
                            : value;
                }

                list.Add(i);
            }

            return new ListItemArray(list.OrderBy(x => x.Index));

            int GetItemIndex(SurveyOptionItem item)
            {
                var selection = selections.FirstOrDefault(x => x.SurveyOptionIdentifier == item.Identifier);
                return selection != null ? selections.IndexOf(selection) : items.IndexOf(item);
            }
        }

        private bool IsSelected(Guid item)
        {
            return _current.Session.QResponseOptions
                .FirstOrDefault(x => x.SurveyOptionIdentifier == item)?.ResponseOptionIsSelected == true;
        }

        private void OptionItemChanged(object sender, EventArgs e)
        {
            if (sender is ICheckBox check)
            {
                if (Guid.TryParse(check.Value, out Guid item))
                    SelectionChanged(item, check.Checked);
            }
            else if (sender is IRadioButton radio)
            {
                var value = radio.Value.IsNotEmpty() ? Guid.Parse(radio.Value) : Guid.Empty;

                SelectionChanged(value);
            }
            else if (sender is FormComboBox select)
            {
                var value = select.ValueAsGuid;

                if (value.HasValue)
                    SelectionChanged(value.Value);
                else if (select.ListIdentifier.HasValue)
                    SelectionCleared(select.ListIdentifier.Value);
            }
            else
            {
                throw new Exception($"Unexpected item type: {sender.GetType()}");
            }
        }

        private void CheckListOptionChanged(object sender, EventArgs e)
        {
            var list = (InSiteCheckBoxList)sender;
            var changed = (InSiteCheckBoxList.ChangedArgs)e;
            var changedItem = list.Items[changed.ChangedItemIndex];

            if (!Guid.TryParse(changedItem.Value, out Guid itemId))
                return;

            SelectionChanged(itemId, changedItem.Selected);
        }

        private void RadioListOptionChanged(object sender, EventArgs e)
        {
            var list = (InSiteRadioButtonList)sender;
            var changed = (InSiteRadioButtonList.ChangedArgs)e;
            var changedItem = list.Items[changed.ChangedItemIndex];

            if (!Guid.TryParse(changedItem.Value, out Guid itemId))
                return;

            SelectionChanged(itemId);
        }

        private void SelectionChanged(Guid item)
        {
            var option = _current.Survey.FindOptionItem(item);
            var selections = _current.Session.QResponseOptions
                .Where(x => option.List.Items.Any(i => i.Identifier == x.SurveyOptionIdentifier))
                .ToArray();

            // Unselect commands need to be sent first...
            foreach (var selection in selections)
                if (selection.ResponseOptionIsSelected && selection.SurveyOptionIdentifier != item)
                    OptionUnselected(selection.SurveyOptionIdentifier);

            // ... and then we can send Select commands.
            foreach (var selection in selections)
                if (!selection.ResponseOptionIsSelected && selection.SurveyOptionIdentifier == item)
                    OptionSelected(selection.SurveyOptionIdentifier);
        }

        private void SelectionCleared(Guid list)
        {
            var optionList = _current.Survey.FindOptionList(list);
            var selections = _current.Session.QResponseOptions
                .Where(x => optionList.Items.Any(i => i.Identifier == x.SurveyOptionIdentifier))
                .ToArray();

            foreach (var selection in selections)
                if (selection.ResponseOptionIsSelected)
                    OptionUnselected(selection.SurveyOptionIdentifier);
        }

        private void SelectionChanged(Guid item, bool @checked)
        {
            if (@checked)
                OptionSelected(item);
            else
                OptionUnselected(item);
        }

        private void OptionSelected(Guid item)
        {
            SendCommand(_current.Respondent.UserIdentifier, new SelectResponseOption(_current.Session.ResponseSessionIdentifier, item));
        }

        private void OptionUnselected(Guid item)
        {
            SendCommand(_current.Respondent.UserIdentifier, new UnselectResponseOption(_current.Session.ResponseSessionIdentifier, item));
        }

        private void FileUpload_RemoveClicked(object sender, EventArgs e)
        {
            var fileUpload = (FormFileUpload)sender;

            var urls = StringHelper.Split(fileUpload.ExistingRelativeUrls);

            foreach (var url in urls)
            {
                var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(url);
                if (fileIdentifier == null)
                    continue;

                ServiceLocator.StorageService.Delete(fileIdentifier.Value);
            }

            fileUpload.ExistingRelativeUrls = null;
            fileUpload.DataBind();

            SendCommand(_current.Respondent.UserIdentifier, new ChangeResponseAnswer(_current.Session.ResponseSessionIdentifier, fileUpload.QuestionIdentifier, null));
        }

        private void SendCommand(Guid respondent, Command command)
        {
            var user = CurrentSessionState.Identity?.User?.Identifier;

            command.OriginUser = user ?? respondent;
            ServiceLocator.SendCommand(command);
        }

        private string Translate(string text) =>
            HttpContext.Current.Handler is LobbyBasePage page ? page.Translate(text) : text;
    }
}