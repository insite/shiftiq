<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EmployeeCertificateDetails.ascx.cs" Inherits="InSite.Cmds.Controls.Profiles.EmployeeCertificates.EmployeeCertificateDetails" %>

<asp:CustomValidator ID="CertificateExistValidator" runat="server" ErrorMessage="Specified certificate already exist" Display="None" ValidationGroup="EmployeeCertificate" />

<div class="row">
    <div class="col-lg-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Worker
                <insite:RequiredValidator runat="server" FieldName="Employee" ControlToValidate="EmployeeID" ValidationGroup="EmployeeCertificate" Display="Dynamic" />
            </label>
            <cmds:FindPerson ID="EmployeeID" runat="server" />
            <div class="mt-2">
                <asp:CheckBox ID="ShowArchivedPeople" runat="server" Text="Show Archived People" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                College Certificate
                <insite:RequiredValidator runat="server" FieldName="Certificate" ControlToValidate="CertificateID" ValidationGroup="EmployeeCertificate" />
            </label>
            <cmds:FindCertificate ID="CertificateID" runat="server" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Institution
                <insite:RequiredValidator runat="server" FieldName="Institution" ControlToValidate="InstitutionID" ValidationGroup="EmployeeCertificate" />
            </label>
            <cmds:SchoolSelector ID="InstitutionID" runat="server" />
        </div>

    </div>
    <div class="col-lg-6">

        <div class="form-group mb-3">
            <label class="form-label">
                Date Requested
            </label>
            <insite:DateSelector ID="DateRequested" runat="server" Width="200px" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Date Granted
            </label>
            <insite:DateSelector ID="DateGranted" runat="server" Width="200px" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Date Submitted
            </label>
            <insite:DateSelector ID="DateSubmitted" runat="server" Width="200px" />
        </div>

    </div>
</div>