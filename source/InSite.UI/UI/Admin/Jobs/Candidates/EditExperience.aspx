<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditExperience.aspx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.EditExperience" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<%@ Register Src="Controls/ExperienceDetail.ascx" TagName="ExperienceDetail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <uc:ExperienceDetail runat="server" ID="Detail" />

    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="CandidateExperience" />
            <insite:CloseButton runat="server" ID="CloseButton" OnClientClick="modalManager.closeModal(); return false;" />
        </div>
    </div>
</asp:Content>