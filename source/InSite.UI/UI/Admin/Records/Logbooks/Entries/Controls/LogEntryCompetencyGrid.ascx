<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LogEntryCompetencyGrid.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Entries.Controls.LogEntryCompetencyGrid" %>

<asp:Repeater runat="server" ID="AreaRepeater">
    <HeaderTemplate>
        <table id="ValidateGridTable" class="table table-striped">
            <thead>
                <tr>
                    <th></th>
                    <th>
                        Competency
                    </th>
                    <th style="text-align:center;">
                        <insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/>
                    </th>
                    <th>
                        Satisfaction Level
                    </th>
                    <th>
                        Skill Rating
                    </th>
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
            <td colspan="6">
                <b><%# Eval("Name") %></b>
            </td>
        </tr>

        <asp:Repeater runat="server" ID="CompetencyRepeater">
            <ItemTemplate>
                <tr class="competency-row" data-identifier='<%# Eval("Identifier") %>'>
                    <td style="width:20px;">

                    </td>
                    <td>
                        <%# Eval("Name") %>
                    </td>
                    <td style="text-align:center;">
                        <%# Eval("Hours", "{0:n2}") %>
                    </td>
                    <td class="satisfaction-level">
                        <%# Eval("SatisfactionLevelName") %> 
                    </td>
                    <td class="skill-rating">
                        <%# Eval("SkillRating") ?? "N/A" %>
                    </td>
                    <td style="width:35px;">
                        <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" NavigateUrl='<%# Eval("DeleteUrl") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </ItemTemplate>
</asp:Repeater>
