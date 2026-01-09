<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SingleCorrectOptionRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Options.Controls.SingleCorrectOptionRepeater" %>

<h3 class="table-write-header">
    <span style="width:335px;">
        <span style="width:110px;">Points</span>
        <span>Cut Score (%)</span>
    </span>
    Options
</h3>

<asp:Repeater runat="server" ID="Repeater">
    <HeaderTemplate>
        <table id="<%# ClientID %>" class="table table-striped table-write-options"><tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr data-id='<%# Eval("Sequence") %>'>
            
            <td class="text-end" style="width:65px;">
                <strong><%# Eval("Letter") %></strong>
                <insite:RequiredValidator runat="server" ID="TextRequiredValidator" FieldName="Option Text" ControlToValidate="OptionText" />
            </td>
            <td>
                <insite:TextBox runat="server" TranslationControl="OptionText" AllowHtml="true" />
                <div class="mt-1">
                    <insite:EditorTranslation runat="server" ID="OptionText" Text='<%# Eval("Text") %>' ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
                </div>
                <div class="mt-1">
                    <insite:FindStandard runat="server" ID="OptionCompetency" Value='<%# Eval("Competency") %>' EmptyMessage="Competency" />
                </div>
            </td>
            <td style="width:110px;">
                <insite:NumericBox runat="server" ID="Points" MinValue="0" MaxValue="999.99" ValueAsDecimal='<%# Eval("Points") %>' />
            </td>
            <td style="width:110px;">
                <insite:NumericBox runat="server" ID="CutScore" MinValue="0" MaxValue="100" ValueAsDecimal='<%# Eval("CutScore") %>' />
            </td>
            <td class="text-end" style="width:76px;">
                <span class="start-sort">
                    <i class="fas fa-sort"></i>
                </span>
            </td>
            <td class="text-nowrap" style="width:30px;">
                <span style="line-height: 28px;">
                    <insite:IconButton runat="server" CommandName="Delete" Name="trash-alt" ToolTip="Delete" Visible='<%# !(bool)Eval("IsReadOnly") %>' />
                </span>
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>

<div class="mt-3">
    <insite:Button runat="server" ID="AddOptionButton" Icon="fas fa-plus-circle" Text="Add New Option" ButtonStyle="Default" />
</div>

<asp:HiddenField runat="server" ID="ItemsOrder" ViewStateMode="Disabled" />

<insite:PageHeadContent runat="server">
    <link rel="stylesheet" href="/UI/Admin/assessments/options/controls/write/style.css">
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script src="/UI/Admin/assessments/options/controls/write/script.js"></script>
</insite:PageFooterContent>
