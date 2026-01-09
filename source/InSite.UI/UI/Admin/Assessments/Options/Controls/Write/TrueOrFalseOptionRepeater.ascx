<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrueOrFalseOptionRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Options.Controls.TrueOrFalseOptionRepeater" %>

<h3 class="table-write-header">
    <span style="width:293px;">
        <span style="width:110px;">Points</span>
        <span>Cut Score (%)</span>
        <i class="clearfix"></i>
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
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>

<asp:HiddenField runat="server" ID="ItemsOrder" ViewStateMode="Disabled" />

<insite:PageHeadContent runat="server">
    <link rel="stylesheet" href="/UI/Admin/assessments/options/controls/write/style.css">
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script src="/UI/Admin/assessments/options/controls/write/script.js"></script>
</insite:PageFooterContent>
