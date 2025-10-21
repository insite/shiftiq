<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Custom.CMDS.User.Progressions.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmployeeAchievementDetails.ascx" TagName="EmployeeAchievementDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />

    <asp:ValidationSummary runat="server" ValidationGroup="Education" />

    <section runat="server" ID="AchievementSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-trophy me-1"></i>
            CMDS Information
        </h2>

        <uc:EmployeeAchievementDetails ID="AchievementDetails" runat="server" />
    </section>

    <div class="mb-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Education" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</div>
</asp:Content>
