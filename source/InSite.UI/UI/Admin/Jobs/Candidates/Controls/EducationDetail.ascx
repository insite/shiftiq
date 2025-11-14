<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EducationDetail.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.EducationDetail" %>

<div class="row">
    <div class="col-12">

        <div class="form-group mb-3">
            <label class="form-label">
                Date From
                <insite:RequiredValidator runat="server" ControlToValidate="DateFrom" FieldName="Date From" ValidationGroup="CandidateEducation" />
            </label>
            <insite:DateSelector runat="server" ID="DateFrom" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Date To
            </label>
            <insite:DateSelector runat="server" ID="DateTo" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Institution Name
                <insite:RequiredValidator runat="server" ControlToValidate="InstitutionName" FieldName="Institution Name" ValidationGroup="CandidateEducation" />
            </label>
            <insite:TextBox runat="server" ID="InstitutionName" MaxLength="300" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Course Name
                <insite:RequiredValidator runat="server" ControlToValidate="CourseName" FieldName="Course Name" ValidationGroup="CandidateEducation" />
            </label>
            <insite:TextBox runat="server" ID="CourseName" MaxLength="300" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Country
                <insite:RequiredValidator runat="server" ControlToValidate="CountrySelector" FieldName="Country" ValidationGroup="CandidateEducation" />
            </label>
            <insite:FindCountry ID="CountrySelector" runat="server" EmptyMessage="Country" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                City
                <insite:RequiredValidator runat="server" ControlToValidate="City" FieldName="City" ValidationGroup="CandidateEducation" />
            </label>
            <insite:TextBox runat="server" ID="City" MaxLength="100" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Qualification
            </label>
            <insite:ComboBox runat="server" ID="EducationQualification" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Status
            </label>
            <insite:RadioButton runat="server" ID="StatusComplete" Text="Complete" GroupName="Status" />
            <insite:RadioButton runat="server" ID="StatusIncomplete" Text="Incomplete" GroupName="Status" />
            <insite:RadioButton runat="server" ID="StatusInProgress" Text="In Progress" GroupName="Status" />
        </div>

    </div>
</div>
