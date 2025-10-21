<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Actions.Profile.Employee.Certificate.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/EmployeeCertificateDetails.ascx" TagName="EmployeeCertificateDetails" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="EmployeeCertificate" />

    <section runat="server" ID="CertificateSection" class="mb-3">
        
        <h2 class="h4 mb-3">
            <i class="far fa-file-certificate me-1"></i>
            Certificate
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <uc:EmployeeCertificateDetails ID="Details" runat="server" />

            </div>
        </div>

    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="EmployeeCertificate" />
        <insite:CloseButton runat="server" ID="CloseButton" />
    </div>

</asp:Content>
