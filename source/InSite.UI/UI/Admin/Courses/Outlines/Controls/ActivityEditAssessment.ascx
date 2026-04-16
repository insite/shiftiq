<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ActivityEditAssessment.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ActivityEditAssessment" %>

<%@ Register Src="ActivitySetupTab.ascx" TagName="ActivitySetup" TagPrefix="uc" %>

<insite:Alert runat="server" ID="ScreenStatus" />

<insite:Nav runat="server">

    <insite:NavItem runat="server" ID="QuestionTab" Title="Questions">

        <insite:Alert runat="server" ID="QuestionTabStatus" />

        <div runat="server" id="QuestionCommandsTop" class="mb-3 hstack">
            <insite:DropDownButton runat="server" ID="AddQuestionTopButton" IconName="plus-circle" Text="Add" CssClass="me-2">
                <Items>
                    <insite:DropDownButtonItem Name="AddMultipleChoice" ToolTip="Add a new multiple-choice question to this assessment" IconName="check-circle" Text="Multiple Choice" />
                    <insite:DropDownButtonItem Name="AddTrueOrFalse" ToolTip="Add a new true-or-false question to this assessment" IconName="toggle-on" Text="True or False"  />
                </Items>
            </insite:DropDownButton>
            <insite:Button runat="server" ID="CollapseTopButton" ButtonStyle="Default" Width="95px" Text="Collapse" Icon="fas fa-chevron-up" />
        </div>

        <asp:Repeater runat="server" ID="QuestionRepeater">
            <HeaderTemplate><div id='<%# QuestionRepeater.ClientID %>' class="question-list"></HeaderTemplate>
            <FooterTemplate></div></FooterTemplate>
            <ItemTemplate>
                <div>
                    <div class="question-commands">
                        <span><%# Container.ItemIndex + 1 %></span>
                        <div>
                            <asp:LinkButton runat="server" CommandName="RemoveQuestion" OnClientClick="return confirm('Are you sure you want to delete this question?');"><i class="fas fa-trash-alt"></i></asp:LinkButton>
                            <span runat="server" class="start-reorder" visible='<%# AllowReorder %>'><i class="fas fa-sort"></i></span>
                            <a href='/ui/admin/assessments/banks/outline?bank=<%# Eval("Set.Bank.Identifier") %>&question=<%# Eval("Identifier") %>' target="_blank" title="Assessment Question Bank"><i class="fas fa-external-link-square"></i></a>
                        </div>
                    </div>

                    <div runat="server" id="QuestionViewer" class="question-content">
                        <div style="border:1px dashed #bbb; border-radius:4px; padding:10px; margin-bottom:2px;">
                            <%# Eval("Content.Title.Default") %>
                        </div>
                        <small class="form-text">Question Type: <%# ((QuestionItemType)Eval("Type")).GetDescription() %></small>
                    </div>

                    <div runat="server" id="QuestionEditor" class="question-content">

                        <insite:MarkdownEditor runat="server" ID="QuestionText" UploadControl="QuestionUpload" Value='<%# Eval("Content.Title.Default") %>' />
                        <insite:EditorUpload runat="server" ID="QuestionUpload" Mode="Custom" />

                        <asp:Repeater runat="server" ID="OptionRepeater">
                            <HeaderTemplate><div class="option-list"></HeaderTemplate>
                            <FooterTemplate></div></FooterTemplate>
                            <ItemTemplate>
                                <div>
                                    <div class="option-number"></div>
                                    <div class="option-text">
                                        <insite:TextBox runat="server" ID="OptionText" Text='<%# Eval("Content.Title.Default") %>' />
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                    
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div runat="server" id="QuestionCommandsBottom" style="margin-bottom:15px; margin-top:15px;">
            <insite:DropDownButton runat="server" ID="AddQuestionBottomButton" IconName="plus-circle" Text="Add" CssClass="d-inline-block">
                <Items>
                    <insite:DropDownButtonItem Name="AddMultipleChoice" ToolTip="Add a new multiple-choice question to this assessment" IconName="check-circle" Text="Multiple Choice" />
                    <insite:DropDownButtonItem Name="AddTrueOrFalse" ToolTip="Add a new true-or-false question to this assessment" IconName="toggle-on" Text="True or False"  />
                </Items>
            </insite:DropDownButton>
            <insite:Button runat="server" ID="CollapseBottomButton" ButtonStyle="Default" Width="95px" Text="Collapse" Icon="fas fa-chevron-up" />
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Assessment">
        
        <insite:Alert runat="server" ID="AssessmentTabStatus" />

        <div class="form-group mb-3">
            <div class="float-end">
                <insite:IconLink runat="server" id="AssessmentFormLink" ToolTip="View details for this assessment/quiz form" Name="external-link-square" Target="_blank" />
            </div>
            <label class="form-label">
                Assessment Form
                <insite:RequiredValidator runat="server" ControlToValidate="AssessmentFormIdentifier" />
            </label>
            <div>
                <insite:FindBankForm runat="server" ID="AssessmentFormIdentifier" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Assessment Type</label>
            <div class="radio-buttons">
                <asp:RadioButtonList runat="server" ID="AssessmentSpecType" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="Dynamic" />
                    <asp:ListItem Value="Static" />
                </asp:RadioButtonList>
            </div>
        </div>
        
        <div class="form-group mb-3">
            <label class="form-label">Assessment Title</label>
            <div>
                <insite:TextBox runat="server" ID="AssessmentFormTitle" MaxLength="200" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Question Limit</label>
            <div>
                <insite:NumericBox runat="server" ID="AssessmentSpecQuestionLimit" Width="100px" NumericMode="Integer" MinValue="0" />
            </div>
            <div class="form-text">
                The maximum number of questions allowed on each assessment form.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Passing Score (%)</label>
            <div>
                <insite:NumericBox runat="server" ID="AssessmentSpecPassingScore" Width="100px" NumericMode="Integer" MinValue="0" ValueAsDecimal="50" />
            </div>
            <div class="form-text">
                What is the minimum score required to pass the assessment?
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Score Disclosure</label>
            <div>
                <insite:ComboBox runat="server" ID="AssessmentSpecDisclosureType" Width="100%">
                    <Items>
                        <insite:ComboBoxOption Value="None" Text="No Score or Answers Shown" Selected="True" />
                        <insite:ComboBoxOption Value="Score" Text="Show Score Only" />
                        <insite:ComboBoxOption Value="Answers" Text="Show Answers Only" />
                        <insite:ComboBoxOption Value="Full" Text="Show Score & Correct/Incorrect Answers" />
                    </Items>
                </insite:ComboBox>
            </div>
            <div class="form-text">
                What information is disclosed to a user after completing an assessment attempt?
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">Publication Status</label>
            <div class="radio-buttons">
                <asp:RadioButtonList runat="server" ID="AssessmentFormPublicationStatus" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Value="Drafted" />
                    <asp:ListItem Value="Published" />
                    <asp:ListItem Value="Unpublished" />
                    <asp:ListItem Value="Archived" />
                </asp:RadioButtonList>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Content">

        <div class="form-group mb-3">
            <label class="form-label">Language</label>
            <div>
                <insite:ComboBox runat="server" ID="Language" AllowBlank="false" />
            </div>
        </div>

        <div class="row row-translate">
            <div class="col-md-12">
                <asp:Repeater runat="server" ID="ContentRepeater">
                    <ItemTemplate>
                        <div class="form-group mb-3">
                            <insite:DynamicControl runat="server" ID="Container" />
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </insite:NavItem>

    <insite:NavItem runat="server" Title="Activity">
        <uc:ActivitySetup runat="server" ID="ActivitySetup" />
    </insite:NavItem>

</insite:Nav>

<div class="mt-5">
    <insite:ValidationSummary runat="server" />
    <insite:SaveButton runat="server" ID="ActivitySaveButton" ValidationGroup="CourseConfig" />
    <insite:CancelButton runat="server" ID="ActivityCancelButton" />
</div>

<div class="d-none">
    <input type="hidden" runat="server" ID="QuestionsData" data-ctrlstate="true" />
    <asp:Button runat="server" ID="UnpublishFormButton" OnClientClick='return false;' />
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        div.radio-buttons span input + label { padding-right: 10px; }

        .question-list {
        }

        .question-list > div {
            min-height: 110px;
        }

        .question-list > div + div {
            margin-top: 30px;
        }

        .question-list > div > .question-commands {
            width: 40px;
            position: absolute;
            text-align: center;
            z-index: 1;
        }

        .question-list > div > .question-commands > span {
            font-size: 27px;
            color: #265f9f;
            white-space: nowrap;
        }

        .question-list > div > .question-commands > div {
            margin-top: 5px;
        }

        .question-list > div > .question-commands > div > a {
            display: block;
        }

        .question-list > div > .question-commands > div > a + a {
            padding-top: 8px;
        }

        .question-list > div > .question-commands > div > .start-reorder {
            padding-top: 8px;
            cursor: grab;
            display: inline-block;
            width: 32px;
            text-align: center;
            line-height: 32px;
        }

        .question-list > div > .question-content {
            padding-left: 55px;
            position: relative;
        }

        .question-list > div > .question-content > .option-list > div {
            margin-top: 10px;
        }

        .question-list > div > .question-content > .option-list > div > .option-number {
            width: 25px;
            position: absolute;
            text-align: center;
            line-height: 34px;
            cursor: pointer;
        }

        .question-list > div > .question-content > .option-list > div > .option-text {
            padding-left: 35px;
        }

        .question-list.ui-sortable > div.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var $dataInput;
            var _questions;

            Sys.Application.add_load(onLoad);

            function onLoad() {
                $('#<%= CollapseTopButton.ClientID %>,#<%= CollapseBottomButton.ClientID %>').each(function () {
                    var $this = $(this);
                    if ($this.data('inited') === true)
                        return;

                    $this.on('click', onViewToggle);

                    $this.data('inited', true);
                });

                $dataInput = $('#<%= QuestionsData.ClientID %>');
                if ($dataInput.data('inited') == true)
                    return;

                _questions = $dataInput.val();
                if (_questions)
                    _questions = JSON.parse(_questions);

                if (!_questions)
                    _questions = [];

                loadQuestions();
                <% if (AllowReorder) { %>
                    $('#<%= QuestionRepeater.ClientID %>.question-list').sortable({
                        items: '> div',
                        containment: 'document',
                        cursor: 'grabbing',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        axis: 'y',
                        handle: '.start-reorder',
                        forceHelperSize: true,
                        stop: function (e, a) {
                            var isChanged = false;

                            a.item.closest('.question-list').find('> div').each(function (index) {
                                var id = $(this).data('id');
                                if (id.item.index !== index) {
                                    id.item.index = index;
                                    isChanged = true;
                                }
                            });

                            if (isChanged)
                                updateQuestionsData();
                        }
                    });
                <% } %>

                $dataInput.data('inited', true);
            }

            // initialization methods

            function loadQuestions() {
                var reorderQuestions = false;

                var $container = $('#<%= QuestionRepeater.ClientID %>.question-list');
                var $questions = $container.find('> div').each(function (qIndex) {
                    var $this = $(this);
                    var qItem = _questions[qIndex];

                    reorderQuestions = reorderQuestions || qItem.index !== qIndex;

                    $this.data('id', {
                        index: qIndex,
                        item: qItem
                    });

                    $this.find('> .question-content > .option-list > div').each(function (oIndex) {
                        var $this = $(this);

                        $this.data('id', oIndex);
                        $this.find('> .option-number').on('click', onOptionCorrectSelected)
                    });

                    renderCorrectOption($this);
                });

                if (reorderQuestions)
                    $questions.sort(function (a, b) {
                        return $(a).data('id').item.index - $(b).data('id').item.index;
                    }).appendTo($container);
            }

            // methods

            function renderCorrectOption($question) {
                var id = $question.data('id');
                
                $question.find('> .question-content > .option-list > div').each(function () {
                    var $option = $(this);
                    var $number = $option.find('> .option-number');

                    if (id.item.correct === $option.data('id'))
                        $number.html('<i style="color:#5cb85c" class="far fa-check-circle"></i>');
                    else
                        $number.html('<i style="color:#f74f78" class="far fa-times-circle"></i>');
                });
            }

            function updateQuestionsData() {
                $dataInput.val(JSON.stringify(_questions));
            }

            // event handlers

            function onOptionCorrectSelected() {
                var parents = $(this).parentsUntil('.option-list');
                if (parents.length == 0)
                    return;

                var $option = parents.eq(parents.length - 1);
                var optionId = $option.data('id');
                if (typeof optionId === 'undefined')
                    return;

                parents = $option.parentsUntil('.question-list');
                if (parents.length == 0)
                    return;

                var $question = parents.eq(parents.length - 1);

                var id = $question.data('id');

                id.item.correct = optionId;

                renderCorrectOption($question);
                updateQuestionsData();
            }

            function onViewToggle(e) {
                e.preventDefault();
                e.stopPropagation();

                var isCollapsed = $(this).data('collapsed') === true;

                $('#<%= CollapseTopButton.ClientID %>,#<%= CollapseBottomButton.ClientID %>').each(function () {
                    var $this = $(this).data('collapsed', !isCollapsed);

                    if (isCollapsed)
                        $this.html('<i class="fas fa-chevron-up"></i> Collapse');
                    else
                        $this.html('<i class="fas fa-chevron-down"></i> Expand');
                });
                
                $('#<%= QuestionRepeater.ClientID %>.question-list > div > .question-content').each(function () {
                    var $this = $(this);

                    if (!isCollapsed) {
                        $this.hide();

                        var $content = $('<div>').addClass('question-content').data('content', 'collapse');

                        $this.find('textarea:first').each(function () {
                            $content.append($('<p>').text($(this).val()));
                        });

                        var listItems = [];

                        $this.find('> .option-list > div > .option-text > input[type="text"]').each(function () {
                            var text = $(this).val().trim();
                            if (text.length > 0)
                                listItems.push(text);
                        });

                        if (listItems.length > 0) {
                            var $list = $('<ul>');

                            for (var i = 0; i < listItems.length; i++)
                                $list.append($('<li>').text(listItems[i]));

                            $content.append($list);
                        }

                        $this.after($content);
                    } else if ($this.data('content') === 'collapse') {
                        $this.remove();
                    } else {
                        $this.show();
                    }
                });
            }
        })();
    </script>
</insite:PageFooterContent>