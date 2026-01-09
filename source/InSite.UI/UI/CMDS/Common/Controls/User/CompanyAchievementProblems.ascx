<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompanyAchievementProblems.ascx.cs" Inherits="InSite.Cmds.Controls.Contacts.Companies.Files.CompanyAchievementProblems" %>

<div class="alert alert-warning alert-dismissible" role="alert">
    <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>

    <strong>Potential Data Problems</strong>

    <br />

    <asp:Literal ID="IssueText" runat="server" />

    <asp:Repeater ID="IssueList" runat="server">
        <HeaderTemplate><ul></HeaderTemplate>
        <FooterTemplate></ul></FooterTemplate>
        <ItemTemplate>
            <li>
                The
                <%# (string)Eval("UploadType") == UploadType.Link ? "link" : "file" %>
                named "<%# Eval("UploadName") %>"
                is assigned to competency <%# Eval("CompetencyNumber") %>
                and to the achievement titled "<%# Eval("AchievementTitle") %>".
                However, the achievement is not assigned to the competency.
            </li>
        </ItemTemplate>
    </asp:Repeater>

    <asp:LinkButton ID="FixButton" runat="server" Text="Click here to fix this." />
</div>