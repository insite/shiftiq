<%@ Page Language="C#" CodeBehind="Change.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Change" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Workflow/Forms/Questions/Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <uc:Detail runat="server" ID="Detail" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="SurveyQuestion" />
    <insite:CancelButton runat="server" ID="CancelButton" />
</asp:Content>
