<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditEducation.aspx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.EditEducation" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<%@ Register Src="Controls/EducationDetail.ascx" TagName="EducationDetail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <uc:EducationDetail runat="server" ID="Detail" />

    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CandidateEducation" />
            <insite:CloseButton runat="server" ID="CloseButton" OnClientClick="modalManager.closeModal(); return false;" />
        </div>
    </div>
</asp:Content>