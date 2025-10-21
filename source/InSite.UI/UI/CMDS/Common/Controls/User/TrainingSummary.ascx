<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrainingSummary.ascx.cs" Inherits="InSite.Cmds.Controls.User.TrainingSummary" %>

<div runat="server" ID="SummaryTableWrapper" class="training-panel">
    
    <table class="table table-striped">
        <thead>
            <tr>
                <th colspan="4"><strong>Training Summary</strong></th>
            </tr>
        </thead>
        <tbody>
            <tr runat="server" ID="ModuleRow" class="alt2">
                <td><cmds:Flag ID="ModuleImage" runat="server" /></td>
                <td>e-Learning Modules</td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="ModuleProgress" runat="server" /></td>
                <td><asp:HyperLink ID="ModuleLink" runat="server"></asp:HyperLink></td>
            </tr>
            <tr runat="server" ID="HRModuleRow" class="alt2" visible="false">
                <td><cmds:Flag ID="Flag1" runat="server" /></td>
                <td>HR Learning Modules</td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="HRModuleProgress" runat="server" /></td>
                <td><asp:HyperLink ID="HRModuleLink" runat="server"></asp:HyperLink></td>
            </tr>
            <tr class="alt2">
                <td><cmds:Flag ID="GuideImage" runat="server" /></td>
                <td><asp:Literal runat="server" ID="GuideTitle" Text="Training Guides" /></td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="GuideProgress" runat="server" /></td>
                <td><asp:HyperLink ID="GuideLink" runat="server"></asp:HyperLink></td>
            </tr>
            <tr class="alt2">
                <td><cmds:Flag ID="ProcedureImage" runat="server" /></td>
                <td><asp:Literal runat="server" ID="ProcedureTitle" Text="Site-Specific Operating Procedures" /></td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="ProcedureProgress" runat="server" /></td>
                <td><asp:HyperLink ID="ProcedureLink" runat="server" ></asp:HyperLink></td>
            </tr>
        </tbody>
    </table>

    <table class="table table-striped">
        <thead>
            <tr>
                <th colspan="4"><strong>Skills Passport</strong></th>
            </tr>
        </thead>
        <tbody>
            <tr class="alt2">
                <td><cmds:Flag ID="MandatoryOrientationFlag" runat="server" /></td>
                <td>Mandatory Orientations</td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="MandatoryOrientationProgress" runat="server" /></td>
                <td><asp:HyperLink runat="server" ID="MandatoryOrientationLink"></asp:HyperLink></td>
            </tr>
            <tr>
                <td><cmds:Flag ID="OptionalOrientationFlag" runat="server" /></td>
                <td>Optional Orientations</td>
                <td style="text-align:right; padding-right: 10px;"><asp:Literal ID="OptionalOrientationProgress" runat="server" /></td>
                <td><asp:HyperLink runat="server" ID="OptionalOrientationLink"></asp:HyperLink></td>
            </tr>
        </tbody>
    </table>

</div>