<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditSelfEmployment.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.EditSelfEmployment" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/SelfEmploymentDetail.ascx" TagName="SelfEmploymentDetail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:ValidationSummary runat="server" ValidationGroup="Detail" />

    <uc:SelfEmploymentDetail runat="server" ID="Detail" />

    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Detail" />
            <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/job/candidates/profile/edit#work-experience" />
        </div>
    </div>

</asp:Content>