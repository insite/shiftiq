<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SignOff.ascx.cs" Inherits="InSite.Custom.CMDS.User.Achievements.Controls.SignOff" %>
<%@ Register Src="AchievementSummary.ascx" TagName="AchievementSummary" TagPrefix="uc" %>
<%@ Register Src="AttachmentList.ascx" TagName="AttachmentList" TagPrefix="uc" %>

<asp:PlaceHolder ID="SelectedAchievement" runat="server">
                        
    <div class="card mb-4">

        <div class="card-body">
            
            <h3><asp:Literal ID="AchievementTitle" runat="server" /></h3>
                                
            <asp:Literal ID="AchievementDescription" runat="server" />

            <div runat="server" id="StatusPanel" class="mb-3"></div>

            <asp:Literal ID="ProgramOnlyAchievementSummary" runat="server" />

            <div runat="server" id="TimeSensitivePanel" class="mb-3"></div>

            <div runat="server" id="ExpiryPanel" class="mb-3"></div>

            <div runat="server" ID="EmployeeDownloadPanel" class="mb-3">
                <table>
                    <tr>
                        <td><strong>Employee Downloads</strong></td>
                    </tr>
                    <tr>
                        <td><uc:AttachmentList ID="EmployeeDownloadList" runat="server" /></td>
                    </tr>
                </table>
            </div>

        </div>

    </div>

    <uc:AchievementSummary ID="AchievementSummary" runat="server" />

</asp:PlaceHolder>