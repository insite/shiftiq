<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.Achievements.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/AchievementDetails.ascx" TagName="AchievementDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <section runat="server" ID="AchievementSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            Achievement
        </h2>

        <uc:AchievementDetails ID="AchievementDetails" runat="server" />
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
