<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProblemRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Outlines.Controls.ProblemRepeater" %>

<%@ Register TagPrefix="uc" TagName="OptionReadRepeater" Src="../../Options/Controls/OptionReadRepeater.ascx" %>

<div runat="server" id="ZeroItemPanel" class="alert alert-success" role="alert">
    <i class="far fa-check"></i> <asp:Literal runat="server" ID="ZeroItemText" />
</div>

<asp:Repeater runat="server" ID="ItemRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Question</th>
                    <th>Description</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>
    </HeaderTemplate>
    <FooterTemplate>
        </tbody></table>
    </FooterTemplate>
    <ItemTemplate>
        <tr>
            <td class="px-4 pb-4">
                <div><i><%# Eval("Question.Set.Name") %></i></div>
                <div><strong>Question #<%# (int)Eval("Question.BankIndex") + 1 %> (<%# string.Format("{0}.{1}", Eval("Question.Asset"), Eval("Question.AssetVersion")) %>)</strong>:</div>
                <div class="my-2" style='white-space:pre-wrap; max-width:100%;'><%# HttpUtility.HtmlEncode((string)Eval("Question.Content.Title.Default")) %></div>
                <div>
                    <uc:OptionReadRepeater runat="server" ID="OptionReadRepeater" AllowHtml="false" />
                </div>
            </td>
            <td><%# Eval("Description") %></td>
            <td class="text-end text-nowrap">
                <insite:IconLink runat="server" Name="pencil" ToolTip="Edit Question" 
                        NavigateUrl='<%# ProblemsReturnUrl.GetRedirectUrl("/ui/admin/assessments/questions/change?bank=" + Eval("Question.Set.Bank.Identifier") + "&question=" + Eval("Question.Identifier")) %>' />
                <insite:IconLink Name="trash-alt" runat="server" ToolTip="Delete Question" visible='<%# (PublicationStatus)Eval("Question.PublicationStatus") == PublicationStatus.Drafted %>'
                        NavigateUrl='<%# ProblemsReturnUrl.GetRedirectUrl("/admin/assessments/questions/delete?bank=" + Eval("Question.Set.Bank.Identifier") + "&question=" + Eval("Question.Identifier")) %>'  />
                <asp:LinkButton runat="server" ToolTip="Jump To Question" CommandName="JumpToQuestion" CommandArgument='<%# Eval("Question.Identifier") %>' Text="<i class='far fa-reply fa-rotate-270'></i>" />
            </td>
        </tr>
    </ItemTemplate>
</asp:Repeater>