<%@ Page Language="C#" CodeBehind="Describe.aspx.cs" Inherits="InSite.Admin.Events.Appointments.Forms.Describe" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<div id="desktop">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Resource" />

    <section runat="server" ID="ContentSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-edit me-1"></i>
            Describe Appointment
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
            <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="Resource" />
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Resource" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</div>
</asp:Content>
