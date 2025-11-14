<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEducation.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.EditEducation" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/EducationDetail.ascx" TagName="EducationDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:ValidationSummary runat="server" ValidationGroup="Detail" />

    <uc:EducationDetail runat="server" ID="Detail" />

    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Detail" />
            <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/job/candidates/profile/edit#education" />
        </div>
    </div>

</asp:Content>