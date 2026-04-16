<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PlanAchievementTypeRepeater.ascx.cs" Inherits="InSite.UI.Portal.Learning.Controls.PlanAchievementTypeRepeater" %>

<asp:Repeater runat="server" ID="TypeRepeater">
    <ItemTemplate>

        <div class="card mb-4">
            <div class="card-body">

                <h3><%# GetAchievementTypeDisplay((string)Eval("Type")) %></h3>

                <asp:Repeater runat="server" ID="AchievementRepeater">
                    <HeaderTemplate>
                        <table class="table table-striped">
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </tbody>
                        </table>
                    </FooterTemplate>
                    <ItemTemplate>
                        <tr id="Row" runat="server">
                            <td>
                                <insite:Icon runat="server" ID="FlagIcon" />
                            </td>
                            <td>

                                <div class="mb-1">
                                    <asp:HyperLink runat="server" ID="AchievementLink"><%# Eval("AchievementTitle") ?? "N/A" %></asp:HyperLink>
                                </div>

                                <div>
                                    <asp:Literal runat="server" ID="AchievementLabels" />
                                </div>

                                <div runat="server" ID="WarningPanel" class="alert alert-warning mt-2">
                                    <small>
                                        <strong><i class="fas fa-exclamation-triangle"></i> Please Note:</strong>
                                        The date on which this training was completed has not yet been entered. Please contact your administrator to have this information updated in the system.
                                    </small>
                                </div>

                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>

            </div>
        </div>

    </ItemTemplate>
</asp:Repeater>
