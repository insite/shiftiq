<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ComplianceSummary.ascx.cs" Inherits="InSite.Cmds.User.Achievements.Controls.ComplianceSummary" %>

<div runat="server" ID="SummaryTableWrapper" class="compliance-panel">

    <table class="table table-striped">
        <thead>
            <tr>
                <th colspan="4">Compliance Summary</th>
            </tr>
        </thead>
        <tbody>
        <tr>
            <td><cmds:Flag ID="TimeSensitiveAchievementImage" runat="server" /></td>
            <td>Time-Sensitive Safety Certificates</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="TimeSensitiveAchievementPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="TimeSensitiveAchievementLink" runat="server">
                    <asp:Literal ID="TimeSensitiveAchievementText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td><cmds:Flag ID="AdditionalComplianceRequirementsImage" runat="server" /></td>
            <td>Additional Compliance Requirements</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="AdditionalComplianceRequirementsPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="AdditionalComplianceRequirementsLink" runat="server">
                    <asp:Literal ID="AdditionalComplianceRequirementsText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td><cmds:Flag ID="CriticalCompetencyImage" runat="server" /></td>
            <td>Critical Competencies</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="CriticalCompetencyPercent" runat="server" /></td>
            <td>
                <asp:Literal ID="CriticalCompetencyText" runat="server" />
                <asp:HyperLink ID="CriticalCompetencyLink" runat="server" />
            </td>
        </tr>
        <tr>
            <td><cmds:Flag ID="NonCriticalCompetencyImage" runat="server" /></td>
            <td>Non-Critical Competencies</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="NonCriticalCompetencyPercent" runat="server" /></td>
            <td>
                <asp:Literal ID="NonCriticalCompetencyText" runat="server" />
                <asp:HyperLink ID="NonCriticalCompetencyLink" runat="server" />
            </td>
        </tr>
        <tr>
            <td><cmds:Flag ID="CopImage" runat="server" /></td>
            <td><asp:Literal runat="server" ID="CopTitle" Text="Codes of Practice" /></td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="COPPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="COPLink" runat="server">
                    <asp:Literal ID="COPText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>
        <tr>
            <td><cmds:Flag ID="SopImage" runat="server" /></td>
            <td><asp:Literal runat="server" ID="SopTitle" Text="Safe Operating Practices" /></td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="SOPPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="SOPLink" runat="server">
                    <asp:Literal ID="SOPText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>
        <tr runat="server" ID="HrdRow">
            <td><cmds:Flag ID="HrdImage" runat="server" /></td>
            <td><asp:Literal runat="server" ID="HrdTitle" Text="Human Resources Documents" /></td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="HrdPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="HrdLink" runat="server">
                    <asp:Literal ID="HrdText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>
        <tr runat="server" ID="ElmRow" Visible="False">
            <td><cmds:Flag ID="ElmImage" runat="server" /></td>
            <td>e-Learning Modules</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="ElmPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="ElmLink" runat="server">
                    <asp:Literal ID="ElmText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>

        <tr runat="server" ID="Tr1" Visible="False">
            <td><cmds:Flag ID="Flag1" runat="server" /></td>
            <td>HR Learning Modules</td>
            <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="HrmPercent" runat="server" /></td>
            <td>
                <asp:HyperLink ID="HrmLink" runat="server">
                    <asp:Literal ID="HrmText" runat="server" />
                </asp:HyperLink>
            </td>
        </tr>

        </tbody>
    </table>

</div>