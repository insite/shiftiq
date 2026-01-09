<%@ Page Language="C#" CodeBehind="DeletePrerequisite.aspx.cs" Inherits="InSite.Admin.Achievements.Achievements.Forms.DeletePrerequisite" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="Status" />

    <section runat="server" ID="AchievementSection" class="mb-3">
        <insite:Alert runat="server" Indicator="Warning">
            Are you sure you want to delete the prerequisite for this achievement?
        </insite:Alert>
    </section>

    <div>
        <insite:DeleteButton runat="server" ID="DeleteButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
