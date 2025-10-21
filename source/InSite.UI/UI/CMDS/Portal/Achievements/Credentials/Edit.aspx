<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Custom.CMDS.User.Progressions.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmployeeAchievementDetails.ascx" TagName="EmployeeAchievementDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />

    <asp:ValidationSummary runat="server" ValidationGroup="Education" />

    <section runat="server" ID="AchievementSection" class="mb-3">
        <uc:EmployeeAchievementDetails ID="AchievementDetails" runat="server" />
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Education" />
    <insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this Credential?" />
    <insite:Button runat="server" ID="NewButton" Icon="fas fa-plus-circle" Text="New" ButtonStyle="Default" />
    <insite:CancelButton runat="server" ID="CancelButton" />
</asp:Content>
