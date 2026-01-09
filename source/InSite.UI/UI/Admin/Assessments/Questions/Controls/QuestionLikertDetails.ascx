<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionLikertDetails.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Questions.Controls.QuestionLikertDetails" %>

<div class="row">
    <div class="col-lg-6">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    Rows
                </h3>

                <asp:Repeater runat="server" ID="RowRepeater">
                    <HeaderTemplate>
                        <table id="<%# RowRepeater.ClientID %>" class="table table-striped table-likert-rows"><tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr data-id='<%# Eval("InfoId") %>'>
                            <td class="text-end" style="width:65px;">
                                <strong><%# (int)Eval("Sequence") %></strong>
                                <insite:RequiredValidator runat="server" ID="TextValidator" FieldName="Row Text" ControlToValidate="Text" />
                            </td>
                            <td>
                                <insite:TextBox runat="server" TranslationControl="Text" AllowHtml="true" />
                                <div class="mt-1">
                                    <insite:EditorTranslation runat="server" ID="Text" Text='<%# Eval("Content.Title") %>' ClientEvents-OnSetText="questionLikertDetails.onSetTitleTranslation" ClientEvents-OnGetText="questionLikertDetails.onGetTitleTranslation" />
                                </div>
                                <div runat="server" id="StandardField" class="mt-1">
                                    <insite:FindStandard runat="server" ID="StandardIdentifier" Value='<%# Eval("Standard") %>' EmptyMessage="Competency" />
                                </div>
                            </td>
                            <td class="text-end" style="width:76px;">
                                <span class="start-sort">
                                    <i class="fas fa-sort"></i>
                                </span>
                            </td>
                            <td class="text-nowrap" style="width:30px;">
                                <span style="line-height: 28px;">
                                    <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" Visible='<%# !(bool)Eval("IsLocked") %>'
                                        CausesValidation="false" ConfirmText="Are you sure you want to remove this row?" />
                                </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="mt-auto">
                    <asp:HiddenField runat="server" ID="RowsOrder" ViewStateMode="Disabled" />
                    <insite:Button runat="server" ID="AddRowButton" Icon="fas fa-plus-circle" Text="Add New Row" ButtonStyle="Default" CausesValidation="false"  />
                </div>

            </div>
        </div>
    </div>
    <div class="col-lg-6">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body d-flex flex-column">

                <h3 class="table-write-header">
                    Columns
                </h3>

                <asp:Repeater runat="server" ID="ColumnRepeater">
                    <HeaderTemplate>
                        <table id="<%# ColumnRepeater.ClientID %>" class="table table-striped table-likert-columns"><tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr data-id='<%# Eval("InfoId") %>'>
                            <td class="text-end" style="width:65px;">
                                <strong><%# Eval("Letter") %></strong>
                                <insite:RequiredValidator runat="server" ID="TextValidator" FieldName="Column Text" ControlToValidate="Text" />
                            </td>
                            <td>
                                <insite:TextBox runat="server" TranslationControl="Text" AllowHtml="true" />
                                <div class="mt-1">
                                    <insite:EditorTranslation runat="server" ID="Text" Text='<%# Eval("Content.Title") %>' ClientEvents-OnSetText="questionLikertDetails.onSetTitleTranslation" ClientEvents-OnGetText="questionLikertDetails.onGetTitleTranslation" />
                                </div>
                            </td>
                            <td class="text-end" style="width:76px;">
                                <span class="start-sort">
                                    <i class="fas fa-sort"></i>
                                </span>
                            </td>
                            <td class="text-nowrap" style="width:30px;">
                                <span style="line-height: 28px;">
                                    <insite:IconButton runat="server"
                                        CommandName="Delete"
                                        Name="trash-alt"
                                        ToolTip="Delete"
                                        Visible='<%# !(bool)Eval("IsLocked") %>'
                                        CausesValidation="false"
                                        ConfirmText="Are you sure you want to remove this column?"
                                    />
                                </span>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

                <div class="mt-auto">
                    <asp:HiddenField runat="server" ID="ColumnsOrder" ViewStateMode="Disabled" />
                    <insite:Button runat="server" ID="AddColumnButton" Icon="fas fa-plus-circle" Text="Add New Column" ButtonStyle="Default" CausesValidation="false" />
                </div>

            </div>
        </div>
    </div>
</div>

<div runat="server" id="OptionsCard" class="card border-0 shadow-lg mt-3">
    <div class="card-body">

        <h3 class="table-write-header">
            Points
        </h3>

        <table class="table table-striped">
            <thead>
                <tr>
                    <td>
                    </td>
                    <asp:Repeater runat="server" ID="OptionsColumnRepeater">
                        <ItemTemplate>
                            <th>
                                <%# Eval("Letter") %>
                            </th>
                        </ItemTemplate>
                    </asp:Repeater>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater runat="server" ID="OptionsRowRepeater">
                    <ItemTemplate>
                        <tr>
                            <th>
                                <%# (int)Eval("Sequence") %>
                            </th>
                            <asp:Repeater runat="server" ID="OptionRepeater">
                                <ItemTemplate>
                                    <td>
                                        <insite:NumericBox runat="server" ID="Points" MinValue="0" MaxValue="999.99" ValueAsDecimal='<%# Container.DataItem %>' />
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        table.table-likert-rows .start-sort,
        table.table-likert-columns .start-sort {
            cursor: grab;
            display: inline-block;
            width: 32px;
            text-align: center;
            line-height: 32px;
        }

        table.table-likert-rows .ui-sortable > tr.ui-sortable-placeholder,
        table.table-likert-columns .ui-sortable > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.questionLikertDetails = window.questionLikertDetails || {};

            instance.initReorder = function (tableId, hiddenId) {
                var $tbody = $('table#' + tableId + ' > tbody');
                if ($tbody.data('ui-sortable'))
                    return;

                $tbody.data('hidden-id', hiddenId);

                $tbody.sortable({
                    items: '> tr',
                    cursor: 'grabbing',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    axis: 'y',
                    handle: 'span.start-sort',
                    forceHelperSize: true,
                    start: function () {
                        if (document.activeElement)
                            document.activeElement.blur();
                    },
                    stop: function (e, a) {
                        var $tbody = a.item.closest('tbody');
                        var hiddenId = $tbody.data('hidden-id');
                        var $input = $('#' + String(hiddenId));

                        if ($input.length > 0) {
                            var result = '';

                            $tbody.find('> tr').each(function () {
                                result += ';' + String($(this).data('id'));
                            });

                            $input.val(result.length > 0 ? result.substring(1) : '');
                        }
                    },
                });
            };

            instance.onSetTitleTranslation = function (o) {
                o.text = questionTextEditor.fromInSiteMarkdown(o.text);
            };

            instance.onGetTitleTranslation = function (o) {
                o.text = questionTextEditor.toInSiteMarkdown(o.text);
            };
        })();
    </script>
</insite:PageFooterContent>