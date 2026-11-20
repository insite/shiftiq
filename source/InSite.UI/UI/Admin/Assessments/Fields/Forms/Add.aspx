<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Assessments.Fields.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="CreatorStatus" />
            <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <section runat="server" id="GeneralSection" class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h2 class="h4 mb-3">
                    <i class="far fa-question me-1"></i>
                    Add Questions to Section
                </h2>
                <div class="row">
                    <div class="col-lg-4">

                        <div runat="server" id="SpecificationField" class="form-group mb-3" visible="false">
                            <label class="form-label">Specification</label>
                            <div>
                                <asp:Literal runat="server" ID="SpecificationName" />
                            </div>
                            <div class="form-text">
                                The internal name used to uniquely identify this specification for filing purposes.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Form</label>
                            <div>
                                <asp:Literal runat="server" ID="FormName" />
                            </div>
                            <div class="form-text">
                                The internal name used to uniquely identify the form for filing purposes.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Section</label>
                            <div>
                                <asp:Literal runat="server" ID="SectionName" />
                            </div>
                            <div class="form-text">
                                The section on the form to contain the selected question items.
                            </div>
                        </div>

                        <div runat="server" id="SetField" class="form-group mb-3" visible="false">
                            <label class="form-label">Question Set</label>
                            <div>
                                <asp:Literal runat="server" ID="SetName" />
                            </div>
                            <div class="form-text">
                                The internal name used to uniquely identify the set for filing purposes.
                            </div>
                        </div>

                    </div>

                    <div class="col-lg-8">

                        <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="QuestionsUpdatePanel" />

                        <insite:UpdatePanel runat="server" ID="QuestionsUpdatePanel">
                            <ContentTemplate>
                                <div class="form-group mb-3">
                                    <insite:RadioButton runat="server" ID="QuestionModeSelect" GroupName="AddMode" Checked="true"
                                        Text="Add questions by selecting Question Items" SubText="Select questions to add to the form." />
                                    <insite:RadioButton runat="server" ID="QuestionModeText" GroupName="AddMode"
                                        Text="Add questions by Code" SubText="Add questions in bulk using question codes." />
                                </div>

                                <asp:MultiView runat="server" ID="QuestionModeMultiView">

                                    <asp:View runat="server" ID="SelectModeView">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Question Items
                                                <insite:CustomValidator runat="server" ID="QuestionIdValidator" ErrorMessage="Required field: Questions" ClientValidationFunction="fieldCreator.onQuestionIdValidation" ValidationGroup="Assessment" />
                                            </label>
                                            <div>
                                                <insite:Container runat="server" ID="QuestionsContainer" Visible="false">
                                                    <asp:Repeater runat="server" ID="QuestionRepeater">
                                                        <HeaderTemplate>
                                                            <table id="<%= QuestionRepeater.ClientID %>" class="table table-condensed">
                                                                <thead>
                                                                    <tr>
                                                                        <th style="width: 35px;">
                                                                            <input type="checkbox" class="input-select-all"></th>
                                                                        <th></th>
                                                                        <th></th>
                                                                        <th></th>
                                                                        <th style="width: 30px;"></th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                        </HeaderTemplate>
                                                        <FooterTemplate>
                                                            </tbody></table>
                                                        </FooterTemplate>
                                                        <ItemTemplate>
                                                            <tr data-input="<%# Container.FindControl("IsSelected").ClientID %>">
                                                                <td>
                                                                    <asp:CheckBox runat="server" ID="IsSelected" Checked='<%# Eval("Checked") %>' Enabled='<%# Eval("FieldIndex") == null %>' /></td>
                                                                <td class="text-end"><%# Eval("BankSequence") %></td>
                                                                <td class="text-nowrap"><%# Eval("Standard") %></td>
                                                                <td>

                                                                    <div class="float-end">
                                                                        <%# GetConditionHtml(Eval("Condition")) %>
                                                                        <%# Eval("Flag") %>
                                                                    </div>

                                                                    <div class="fw-bold"><%# Eval("Code") %></div>

                                                                    <%# HttpUtility.HtmlEncode((string)Eval("Title")) %>

                                                                </td>
                                                                <td class="cell-move"><i class="fas fa-arrows-alt-v"></i></td>
                                                            </tr>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                    <asp:HiddenField runat="server" ID="QuestionsSequence" />
                                                </insite:Container>
                                                <asp:Panel runat="server" ID="NoQuestionsMessage" CssClass="alert alert-info mb-0" Visible="false">
                                                    There are no questions you can assign to the section.
                                                </asp:Panel>
                                            </div>
                                        </div>
                                    </asp:View>

                                    <asp:View runat="server" ID="TextModeView">
                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Question Code
                                                <insite:RequiredValidator runat="server" ID="QuestionCodeValidator" ControlToValidate="QuestionCodes" ValidationGroup="Assessment" />
                                            </label>
                                            <insite:TextBox runat="server" ID="QuestionCodes" TextMode="MultiLine" Rows="5" />
                                            <div class="form-text">
                                                Enter a list of question codes separated by commas or in a new line.
                                            </div>
                                        </div>
                                    </asp:View>

                                </asp:MultiView>
                            </ContentTemplate>
                        </insite:UpdatePanel>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" OnClientClick="fieldCreator.saveSequence();" ValidationGroup="Assessment" CausesValidation="true" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            table#<%= QuestionRepeater.ClientID %> > tbody > tr {
                cursor: default;
            }

                table#<%= QuestionRepeater.ClientID %> > tbody > tr > td.cell-move {
                    cursor: grab;
                    text-align: center;
                    vertical-align: baseline;
                }

                table#<%= QuestionRepeater.ClientID %> > tbody > tr.ui-sortable-helper > td {
                    border-bottom: 1px solid #ddd;
                }

            table#<%= QuestionRepeater.ClientID %> > tbody.ui-sortable > tr.ui-sortable-placeholder {
                visibility: visible !important;
                outline: 1px dashed #666666 !important;
            }
        </style>
    </insite:PageHeadContent>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                const instance = window.fieldCreator = window.fieldCreator || {};
                const tableSelector = 'table#<%= QuestionRepeater.ClientID %>';

                instance.onQuestionIdValidation = function (sender, args) {
                    args.IsValid = false;

                    $rows.each(function () {
                        const $input = getRowInput(this);
                        if ($input != null && $input.is(':enabled:checked')) {
                            args.IsValid = true;
                            return false;
                        }
                    });
                };

                instance.saveSequence = function () {
                    const data = [];

                    $(tableSelector + ' > tbody > tr').each(function () {
                        data.push(parseInt($(this).data('index')));
                    });

                    $('input#<%= QuestionsSequence.ClientID %>').val(data.join(','));
                };

                let questionsTable, $selectAllInput, $rows;

                Sys.Application.add_load(function () {
                    if (questionsTable && document.contains(questionsTable))
                        return;

                    questionsTable = document.querySelector(tableSelector);

                    init();
                });

                function init() {
                    let hasInputs = false;

                    $selectAllInput = $(tableSelector + ' > thead input.input-select-all');

                    $rows = $(tableSelector + ' > tbody > tr').each(function (index) {
                        var $row = $(this).data('index', index);
                        var $input = getRowInput($row);
                        if ($input === null)
                            throw 'Input not found';

                        if ($input.is(':enabled')) {
                            $input.on('change', function () {
                                onInputChange.call(this, arguments);
                                onRowToggle();
                            });

                            onInputChange.call($input);

                            hasInputs = true;
                        } else {
                            $row.find('> .cell-move').each(function () {
                                $(this).removeClass('cell-move').empty();
                            });
                        }
                    });

                    if (!hasInputs)
                        $selectAllInput.remove();
                    else
                        $selectAllInput.on('change', onSelectAll);

                    $(tableSelector + ' > tbody').sortable({
                        items: '> tr',
                        containment: 'document',
                        cursor: 'grabbing',
                        forceHelperSize: true,
                        handle: '> td.cell-move',
                        axis: 'y',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        update: function () {
                            fieldCreator.saveSequence();
                        },
                        helper: function (e, el) {
                            var $cells = el.children();
                            return el.clone().children().each(function (index) {
                                $(this).width($cells.eq(index).outerWidth());
                            }).end();
                        },
                    }).disableSelection();
                }

                function onInputChange() {
                    var $input = $(this);
                    var $row = $input.closest('tr');

                    if ($input.prop('checked'))
                        $row.addClass('active');
                    else
                        $row.removeClass('active');
                }

                function onRowToggle() {
                    var isAllChecked = $rows.length > 0;

                    $rows.each(function () {
                        var $input = getRowInput(this);
                        if ($input === null || !$input.is(':enabled'))
                            return;

                        if (!$input.prop('checked')) {
                            isAllChecked = false;
                            return false;
                        }
                    });

                    $selectAllInput.prop('checked', isAllChecked);
                }

                function onSelectAll() {
                    var isChecked = $selectAllInput.prop('checked');

                    $rows.each(function () {
                        var $input = getRowInput(this);
                        if ($input !== null && $input.is(':enabled')) {
                            $input.prop('checked', isChecked);
                            onInputChange.call($input);
                        }
                    });
                }

                function getRowInput(row) {
                    var inputId = $(row).data('input');
                    if (!inputId)
                        return null;

                    var $input = $('#' + inputId);
                    if ($input.length === 1)
                        return $input.eq(0);

                    return null;
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
