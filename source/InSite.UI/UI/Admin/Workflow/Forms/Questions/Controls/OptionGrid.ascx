<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OptionGrid.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Controls.OptionGrid" %>

<div class="pb-10 mb-3">
    <div class="row">
        <div class="col-lg-6">
            <insite:Button runat="server" ID="AddNewOptionCommand" Icon="fas fa-plus-circle" Text="Add new option" ButtonStyle="Default" />
            <insite:Button runat="server" ID="ImportOptionsCommand" Icon="fas fa-plus-circle" Text="Add multiple options" ButtonStyle="Default" />
        </div>
        <div class="col-lg-6 text-end">
            <span runat="server" id="LanguageOutput" class="badge bg-custom-default text-uppercase"></span>
        </div>
    </div>
</div>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table id='<%# Repeater.ClientID %>' class="table table-striped table-bordered table-survey-options">
            <thead>
                <tr>
                    <th></th>
                    <th></th>
                    <th>Text</th>
                    <th>Points</th>
                    <th>Category</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
            </tbody>
        </table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="cell-move text-center pt-2 pb-2" style="width:40px;">
                <span title="Drag & Drop Row">
                    <i class="fas fa-sort"></i>
                </span>
            </td>
            <td class="pt-2 pb-2" style="width:70px;">
                <insite:TextBox runat="server" ID="LetterInput" Text='<%# Eval("Letter") %>' Enabled="false" CssClass="table-small-column text-center" />
            </td>
            <td class="pt-2 pb-2">
                <div class="row">
                    <div class="col-lg-11">
                        <insite:TextBox runat="server" data-content='<%# ContentLabel.Title %>' />
                    </div>
                    <div class="col-lg-1 align-items-center ps-0 pe-0">
                        <a href="#" class="col-lg-1 survery-option-summary-expander d-none text-decoration-none">
                            <i class="far fa-info-circle" title="Show Summary and Rationale"></i>
                        </a>
                    </div>
                </div>
                <div class="row">
                    <div class="col-lg-11 survey-option-summary d-none">
                        <div>
                            Description:
                            <div>
                                <insite:TextBox runat="server" Rows="3" TextMode="MultiLine" data-content='<%# ContentLabel.Description %>' AllowHtml="true" />
                            </div>
                        </div>
        
                        <div>
                            Feedback when Selected:
                            <div>
                                <insite:TextBox runat="server" Rows="3" TextMode="MultiLine" data-content='<%# ContentLabel.Feedback %>' AllowHtml="true" />
                            </div>
                        </div>
        
                        <div>
                            Feedback when Not Selected:
                            <div>
                                <insite:TextBox runat="server" Rows="3" TextMode="MultiLine" data-content='<%# ContentLabel.FeedbackWhenNotSelected %>' AllowHtml="true" />
                            </div>
                        </div>
        
                        <div runat="server" visible='<%# Eval("Identifier") != null %>'>
                            Option Identifier: <%# Eval("Identifier") %>
                        </div>
                    </div>
                </div>
        
            </td>
            <td class="pt-2 pb-2" style="width:100px;">
                <insite:NumericBox runat="server" ID="PointsInput" MaxValue="100" MinValue="0" DecimalPlaces="2" ValueAsText='<%# Eval("Points") %>' CssClass="table-small-column" />
            </td>
            <td class="pt-2 pb-2" style="width:180px;">
                <insite:TextBox runat="server" ID="CategoryInput" MaxLength="90" Text='<%# Eval("Category") %>' />
            </td>
            <td class="text-center pt-2 pb-2" style="width:40px;">
                <insite:IconButton runat="server" CommandName="Delete" ToolTip="Delete Option" Name="trash-alt" ConfirmText="Are you sure you want to delete this option?" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>

<asp:HiddenField runat="server" ID="ReorderInput" />
<asp:HiddenField runat="server" ID="StateInput" />

<insite:Modal runat="server" ID="ImportOptionsWindow" Title="Add Multiple Options" Width="450px" MinHeight="360px">
    <ContentTemplate>
        <div>

            <div>
                <insite:TextBox runat="server" ID="ImportOptionsText" TextMode="MultiLine" Rows="12" Width="425px" />
                <insite:RequiredValidator runat="server" ControlToValidate="ImportOptionsText" FieldName="Options" ValidationGroup="ImportWindow" Display="None" />
            </div>
            <div>
                <insite:ValidationSummary runat="server" ValidationGroup="ImportWindow" />
            </div>
            <div>
                <br />
                <insite:SaveButton runat="server" ID="ImportOptionsSaveButton" CausesValidation="true" ValidationGroup="ImportWindow" />
                <insite:CloseButton runat="server" ID="ImportOptionsCancelButton" />
            </div>

        </div>
    </ContentTemplate>
</insite:Modal>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .table-small-column {
            padding: 0.5625rem 0.7rem !important;
        }

        table.table-survey-options > tbody > tr > td.cell-move {
            text-align: center;
        }

            table.table-survey-options > tbody > tr > td.cell-move span {
                cursor: grab;
                padding: 6px 3px;
                display: inline-block;
            }

        table.table-survey-options > tbody > tr > td .survey-option-summary {
            padding-top: 6px;
        }

            table.table-survey-options > tbody > tr > td .survey-option-summary > div {
                margin-top: 6px;
            }

        table.table-survey-options > tbody > tr.ui-sortable-helper {
            background-color: #abadb3 !important;
            border: 1px solid #666666;
        }

        table.table-survey-options > tbody > tr.ui-sortable-placeholder {
            visibility: visible !important;
        }

            table.table-survey-options > tbody > tr.ui-sortable-placeholder td {
                border: none !important;
            }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.optionGrid = window.optionGrid || {};

            instance.setLanguage = function (lang) {
                setContent(lang);
            };

            var $stateInput = $('input#<%= StateInput.ClientID %>');
            var $reorderInput = $('input#<%= ReorderInput.ClientID %>');
            var $langOutput = $('#<%= LanguageOutput.ClientID %>');

            var state = JSON.parse($stateInput.val());

            init();

            function init() {
                $('table#<%= Repeater.ClientID %> > tbody').sortable({
                    items: '> tr',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    handle: '> td.cell-move',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    start: function (s, e) {
                        e.placeholder.height(e.item.height());
                    },
                    update: function (e, ui) {
                        var data = [];

                        $('table#<%= Repeater.ClientID %> > tbody > tr').each(function () {
                            data.push($(this).data('index'));
                        });

                        $reorderInput.val(JSON.stringify(data));
                    },
                }).disableSelection();

                $('table#<%= Repeater.ClientID %> > tbody > tr').each(function (rowIndex) {
                    const $row = $(this).data('index', rowIndex);
                    const $summary = $row.find('.survey-option-summary:first');

                    if (!state.summary.hasOwnProperty(rowIndex) || state.summary[rowIndex] !== true) {
                        const $expander = $row.find(".survery-option-summary-expander");

                        $expander
                            .removeClass("d-none")
                            .on('click', onSummaryShow);
                    } else {
                        $summary.removeClass('d-none');
                    }
                });

                $('#<%= ImportOptionsCommand.ClientID %>').on('click', onImportOptionsClick);
                $('#<%= ImportOptionsCancelButton.ClientID %>').on('click', onImportOptionsCancel);

                setContent();

                $('table#<%= Repeater.ClientID %> > tbody > tr [data-content]').on('change', onContentChanged);
            }

            function setContent(language) {
                if (typeof language !== 'undefined') {
                    state.lang = language;
                    onStateUpdated();
                } else {
                    language = state.lang;
                }

                $('table#<%= Repeater.ClientID %> > tbody > tr').each(function () {
                    var $row = $(this);
                    var rowIndex = $row.data('index');
                    var rowContent = state.content[rowIndex];

                    $row.find('[data-content]').each(function () {
                        var $input = $(this).val('');
                        var name = $input.data('content');

                        if (rowContent.hasOwnProperty(name)) {
                            var item = rowContent[name];
                            if (item.hasOwnProperty(language)) {
                                $input.val(item[language]);
                            }
                        }
                    });
                });

                $langOutput.text(language);
            }

            function onSummaryShow(e) {
                e.preventDefault();

                var $this = $(this);
                var $row = $this.closest('tr');
                var $column = $this.closest('td');
                var $topDiv = $this.prev();
                var rowIndex = $row.data('index');

                state.summary[rowIndex] = true;

                $row.find('.survey-option-summary:first').removeClass('d-none');
                $column.removeClass('d-flex');
                $topDiv.removeClass('col-lg-11');
                $topDiv.addClass('col-lg-12');
                $this.remove();

                onStateUpdated();
            }

            function onContentChanged() {
                var $input = $(this);
                var $row = $input.closest('tr');

                var name = $input.data('content');
                var rowIndex = $row.data('index');
                var rowContent = state.content[rowIndex];

                if (!rowContent.hasOwnProperty(name))
                    rowContent[name] = {};

                rowContent[name][state.lang] = $input.val();

                onStateUpdated();
            }

            function onStateUpdated() {
                $stateInput.val(JSON.stringify(state));
            }

            function onImportOptionsClick(e) {
                e.preventDefault();

                $('#<%= ImportOptionsText.ClientID %>').val(null);

                modalManager.show('<%= ImportOptionsWindow.ClientID %>');
            }

            function onImportOptionsCancel(e) {
                e.preventDefault();

                $('#<%= ImportOptionsText.ClientID %>').val(null);

                modalManager.close('<%= ImportOptionsWindow.ClientID %>');
            }
        })();
    </script>
</insite:PageFooterContent>
