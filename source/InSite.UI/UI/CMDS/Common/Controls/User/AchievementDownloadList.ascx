<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementDownloadList.ascx.cs" Inherits="InSite.Cmds.Controls.Training.Achievements.AchievementDownloadList" %>

<asp:Repeater ID="List" runat="server">
    <SeparatorTemplate><br /></SeparatorTemplate>
    <ItemTemplate>
        <asp:HyperLink ID="AttachmentLink" runat="server" Target="_blank" /><br />
        <span class="small text-body-secondary">
            <asp:Literal runat="server" ID="AttachmentInfo" />
        </span>
    </ItemTemplate>
</asp:Repeater>