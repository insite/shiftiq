<%@ Control Language="C#" CodeBehind="CmdsCompetencySummary.ascx.cs"
    Inherits="InSite.UI.Portal.Home.Controls.CmdsCompetencySummary" %>

<style>
    div.barchart { float: left; margin-right: 5px; }
</style>

<table class="table competency-summary">
    <thead>
        <tr>
            <th style="text-align:left; width:210px;">Status</th>
            <th style="text-align:left;">Competencies</th>
        </tr>
    </thead>
    <tbody>
        <asp:Repeater ID="Summary" runat="server">
            <ItemTemplate>
                <tr>
                    <td class="align-top">
                        <a runat="server" href='<%# GetSelfAssessmentFinderUrl(Eval("Status")) %>'>
                            <%# Eval("Status") %>
                        </a>
                    </td>
                    <td class="text-nowrap align-top">
                        <cmds:BarChart ID="BarChart" runat="server" Color='<%# GetBarColor(Eval("Status")) %>' Width='<%# Eval("Width") %>' Text='<%# Eval("Value") %>' />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </tbody>
</table>
