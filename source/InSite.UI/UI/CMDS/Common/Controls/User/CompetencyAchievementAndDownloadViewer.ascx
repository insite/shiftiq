<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyAchievementAndDownloadViewer.ascx.cs" Inherits="InSite.Cmds.User.Competencies.Controls.CompetencyAchievementAndDownloadViewer" %>

<style type="text/css">
    table.competency-resource { background-color: White; }
    table.competency-resource td { vertical-align: top; padding: 5px; }
    table.competency-resource td ul { padding: 0 0 5px 20px; margin: 0; }
    table.competency-resource td.flag { width:30px; }
    table.competency-resource tr.group td { border-top: 1px dotted #666; }
    table.competency-resource td strong { color: #666; }
</style>

<div class="row">
    <div class="col-lg-6">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <asp:Repeater ID="AchievementGroups" runat="server">
                    <ItemTemplate>
        
                        <h3><%# GetAchievementGroupTitle(Eval("AchievementGroupName")) %></h3>

                        <table class="competency-resource">

                        <asp:Repeater ID="Achievements" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td class="flag">
                                        <asp:Label ID="SignOffStatusFlag" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Literal ID="AchievementTitle" runat="server" />

                                        <asp:Repeater ID="AchievementDownloads" runat="server">
                                            <HeaderTemplate><ul></HeaderTemplate>
                                            <FooterTemplate></ul></FooterTemplate>
                                            <ItemTemplate>
                                                <li>
                                                    <asp:HyperLink ID="DownloadLink" runat="server" Target="_blank" />
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <insite:Button ID="SignOffButton" runat="server" Icon="fas fa-check-double" Text="Sign Off" ButtonStyle="Success" CommandName="Sign Off" ConfirmText="Are you sure you want to sign off on this training?" />
                                    </td>
                                    <td class="form-text" style="padding-top:7px; white-space: nowrap"><asp:Literal ID="AchievementScore" runat="server" /></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                
                    </table>
                    </ItemTemplate>
                </asp:Repeater>

                <p id="ModuleQuizCompletedPanel" runat="server" visible="false">
                    <asp:CheckBox ID="IsModuleQuizCompleted" runat="server" Text="I have successfully completed the quiz for the above e-Learning Module(s)" />
                </p>

            </div>
        </div>

    </div>
    <div class="col-lg-6" runat="server" id="DownloadsPanel">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Downloads</h3>

                <asp:Repeater ID="GroupWithDownloads" runat="server">
                    <HeaderTemplate>
                        <table class="competency-resource">
                    </HeaderTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr class="group">
                            <td><strong><%# Eval("GroupName") %>:</strong></td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Repeater ID="GroupDownloads" runat="server">
                                    <HeaderTemplate><ul></HeaderTemplate>
                                    <FooterTemplate></ul></FooterTemplate>
                                    <ItemTemplate>
                                        <li>
                                            <asp:HyperLink ID="DownloadLink" runat="server" Target="_blank" />
                                        </li>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

    </div>
</div>