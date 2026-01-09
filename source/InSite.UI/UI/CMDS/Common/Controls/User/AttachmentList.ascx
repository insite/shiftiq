<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AttachmentList.ascx.cs" Inherits="InSite.Cmds.Controls.Training.EmployeeAchievements.AttachmentList" %>

<asp:Repeater ID="List" runat="server">
    <SeparatorTemplate><br /></SeparatorTemplate>
    <ItemTemplate>
        <asp:HyperLink ID="AttachmentLink" runat="server" Target="_blank" />
    </ItemTemplate>
</asp:Repeater>