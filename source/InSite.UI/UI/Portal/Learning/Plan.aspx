<%@ Page Language="C#" CodeBehind="Plan.aspx.cs" Inherits="InSite.UI.Portal.Learning.Plan" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/ComplianceSummary.ascx" TagName="ComplianceSummary" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/SignOff.ascx" TagName="SignOff" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        table.resources td { padding: 5px; vertical-align: top; }
        table.resources tr.selected td {
            background-color: #f5f5f5;
        }
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <section class="mb-3">

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:Alert runat="server" ID="PlanAlert" />

                <div runat="server" id="DisplayTogglePanel" class="mb-3">
                    <insite:Toggle runat="server" ID="DisplayToggle" Size="Mini" TextOn="Show" TextOff="Hide" />
                    the completed achievements in my training plan. 
                    <span class="text-muted fs-sm">(Only incomplete items are shown by default.)</span>
                </div>

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                <insite:UpdatePanel runat="server" ID="UpdatePanel">
                    <ContentTemplate>
                        <div runat="server" ID="AchievementPanel" class="row">
                            <div class="col-md-6">
                                
                                        <asp:Repeater ID="AchievementTypes" runat="server">
                                            <ItemTemplate>

                                                <div class="card mb-4">
                                                    <div class="card-body">

                                                <h3><%# GetAchievementTypeDisplay((string)Eval("SubType")) %></h3>

                                                <div>
                                                    <table class="table table-striped">

                                                        <asp:Repeater ID="Achievements" runat="server">
                                                            <ItemTemplate>
                                                                <tr id="Row" runat="server">
                                                                    <td>
                                                                        <insite:Icon runat="server" ID="FlagIcon" />
                                                                    </td>
                                                                    <td>

                                                                        <div class="mb-1">
                                                                            <asp:HyperLink runat="server" ID="AchievementLink"><%# Eval("AchievementTitle") ?? "N/A" %></asp:HyperLink>
                                                                        </div>

                                                                        <div class="">
                                                                            <asp:Literal runat="server" ID="AchievementLabels" />
                                                                        </div>

                                                                        <div runat="server" ID="WarningPanel" class="alert alert-warning mt-2">
                                                                            <small>
                                                                                <strong><i class="fas fa-exclamation-triangle"></i> Please Note:</strong>
                                                                                The date on which this training was completed has not yet been entered. Please contact your administrator to have this information updated in the system.
                                                                            </small>
                                                                        </div>

                                                                    </td>
                                                                    <td>
                                                                        
                                                                    </td>
                                                                </tr>
                                                            </ItemTemplate>
                                                        </asp:Repeater>

                                                    </table>
                                                </div>

                                                    </div>
                                                </div>

                                            </ItemTemplate>
                                        </asp:Repeater>
                                    
                            </div>
                            <div class="col-md-6">
                                <uc:SignOff runat="server" ID="SignOff" />
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>
                
            </div>
        </div>
    </section>

    <section runat="server" id="ProfilesInTrainingSection">

        <h2 class="h4 mt-5 mb-4">
            Profiles in Training
        </h2>

        <asp:Repeater ID="ProfilesInTraining" runat="server">
            <ItemTemplate>
                <div class="card border-0 shadow-lg mb-4">
                    <div class="card-body">
                        <h3><%# Eval("ProfileNumber") %>: <%# Eval("ProfileTitle") %> (<%# Eval("DepartmentName") %>)</h3>
                        <uc:ComplianceSummary ID="ComplianceSummary" runat="server" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

    </section>
</asp:Content>
