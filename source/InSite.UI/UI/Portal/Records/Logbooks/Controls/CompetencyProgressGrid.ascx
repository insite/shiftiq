<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyProgressGrid.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.CompetencyProgressGrid" %>

<asp:Repeater runat="server" ID="AreaRepeater">
    <HeaderTemplate>
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Competency</th>
                    <th style="text-align:center;"><insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/></th>
                    <th style="text-align:center;">Number of Log Entries</th>
                    <th runat="server" visible='<%# ShowSkillRating %>' style="text-align:center;">Required Skill Rating</th>
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
                <b><%# Eval("Name") %></b>
                <span runat="server" class="ms-1 d-inline-block" visible='<%# Eval("HasRequiredHours") %>'>
                    <%# Eval("HoursCompleted", "({0:p0} Completed)") %>
                </span>
            </td>
        </tr>

        <asp:Repeater runat="server" ID="CompetencyRepeater">
            <ItemTemplate>
                <tr>
                    <td style="padding-left:45px;">
                        <%# Eval("Name") %>
                        <%# EvalSatisfactionLevelHtml("SatisfactionLevel") %>
                    </td>
                    <td style="text-align:center;">
                        <asp:Literal runat="server" ID="Hours" />
                    </td>
                    <td style="text-align:center;">
                        <asp:Literal runat="server" ID="JournalItems" />
                    </td>
                    <td runat="server" visible='<%# ShowSkillRating %>' style="text-align:center;"><%# Eval("SkillRating") != null ? Eval("SkillRating", "{0:n0}") : "None" %></td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
