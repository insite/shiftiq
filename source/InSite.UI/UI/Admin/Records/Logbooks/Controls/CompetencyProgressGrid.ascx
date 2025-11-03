<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyProgressGrid.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.CompetencyProgressGrid" %>

<h3><asp:Literal runat="server" ID="FrameworkTitle" /></h3>

<asp:Repeater runat="server" ID="AreaRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Competency</th>
                    <th style="text-align:center;"><insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/></th>
                    <th style="text-align:center;">Satisfaction Level</th>
                    <th style="text-align:center;">Number of Log Entries</th>
                    <th style="text-align:center;">Required Skill Rating</th>
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
            <td colspan="5">
                <b><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a> </b>
            </td>
        </tr>

        <asp:Repeater runat="server" ID="CompetencyRepeater">
            <ItemTemplate>
                <tr>
                    <td style="padding-left:45px;"><a href='<%# "/ui/admin/standards/edit?id=" + Eval("Identifier") %>'><%# Eval("Name") %> </a></td>
                    <td style="text-align:center;">
                        <asp:Literal runat="server" ID="Hours" />
                    </td>
                    <td style="text-align:center;">
                         <%# Eval("SatisfactionLevel") %>
                    </td>
                    <td style="text-align:center;">
                        <asp:Literal runat="server" ID="JournalItems" />
                    </td>
                    <td style="text-align:center;"><%# Eval("SkillRating") != null ? Eval("SkillRating", "{0:n0}") : "None" %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
