<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MatchingPairList.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.MatchingPairList" %>

<h3>Answers</h3>

<asp:Repeater runat="server" ID="AnswerRepeater">
    <HeaderTemplate>
        <table id="<%# ClientID %>" class="table table-striped">
            <thead>
                <tr>
                    <th colspan="2">Matching Left Side</th>
                    <th colspan="2">Matching Right Side</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <ItemTemplate>
        <tr data-id='<%# Eval("Sequence") %>'>
            
            <td class="w-50 pe-0">
                <asp:Literal runat="server" ID="AnswerSequence" Text='<%# Eval("Sequence") %>' Visible="false" />
                <insite:TextBox runat="server" ID="LeftText" TranslationControl="LeftOptionText" AllowHtml="true" />
                <insite:EditorTranslation runat="server" ID="LeftOptionText" ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
            </td>

            <td>
                <insite:RequiredValidator runat="server" ID="LeftTextRequiredValidator" FieldName="Matching Left Side Text" ControlToValidate="LeftText" />
            </td>

            <td class="w-50 pe-0">
                <insite:TextBox runat="server" ID="RightText" TranslationControl="RightOptionText" AllowHtml="true" />
                <insite:EditorTranslation runat="server" ID="RightOptionText" ClientEvents-OnSetText="optionWriteRepeater.onSetTitleTranslation" ClientEvents-OnGetText="optionWriteRepeater.onGetTitleTranslation" />
            </td>

            <td>
                <insite:RequiredValidator runat="server" ID="RightTextRequiredValidator" FieldName="Matching Right Side Text" ControlToValidate="RightText" />
            </td>

            <td class="text-nowrap" style="width:30px;">
                <span style="line-height: 28px;">
                    <insite:IconButton runat="server" 
                        CommandName="Delete" CommandArgument='<%# Eval("Sequence") %>' Name="trash-alt" ToolTip="Delete" 
                        ConfirmText="Are you sure you want to delete this answer?" />
                </span>
            </td>

        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
</asp:Repeater>

<div class="mt-3">
    <insite:Button runat="server" ID="AddAnswerButton" Icon="fas fa-plus-circle" Text="Add New Answer" ButtonStyle="Default" />
</div>
