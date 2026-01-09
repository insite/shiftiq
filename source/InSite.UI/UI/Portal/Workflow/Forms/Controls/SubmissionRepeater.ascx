<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmissionRepeater.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.SubmissionRepeater" %>

<asp:Repeater runat="server" ID="SessionRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <tr>
                <th><%= Translate("Started") %></th>
                <th><%= Translate("Completed") %></th>
                <th><%= Translate("Status") %></th>
                <th></th>
            </tr>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td><%# LocalizeTime(Eval("ResponseSessionStarted")) %></td>
            <td><%# LocalizeTime(Eval("ResponseSessionCompleted")) %></td>
            <td>
                <%# Translate((string)Eval("ResponseSessionStatus")) %>
                <div class="text-body-secondary"><small><%# Eval("FirstAnswerText") %></small></div>
            </td>
            <td class="text-end">
                <insite:Button runat="server" ID="RestartButton" ButtonStyle="Default" Icon="fas fa-undo-alt" ToolTip="Start Again" />
                <insite:Button runat="server" ID="DeleteButton" ButtonStyle="Default" Icon="fas fa-trash-alt" ToolTip="Delete Submission" />
                <insite:Button runat="server" ID="StartButton" ButtonStyle="Default" Width="130px" />
            </td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>