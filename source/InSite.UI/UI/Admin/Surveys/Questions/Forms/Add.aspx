<%@ Page Language="C#" CodeBehind="Add.aspx.cs" Inherits="InSite.Admin.Surveys.Questions.Forms.Add" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="EditorStatus" />

    <uc:Detail runat="server" ID="Detail" />

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="SurveyQuestion" />
    <insite:CancelButton runat="server" ID="CancelButton" />
</asp:Content>
