<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExperienceDetail.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.ExperienceDetail" %>

<insite:PageHeadContent runat="server">
    
    <style type="text/css">

        input[type="checkbox"] {
            margin-top: 10px;
        }

    </style>

</insite:PageHeadContent>

<div class="row">
    <div class="col-12">

        <div class="form-group mb-3">
            <label class="form-label">
                Employed From
                <insite:RequiredValidator runat="server" ControlToValidate="DateFrom" FieldName="Date From" ValidationGroup="CandidateExperience" />
            </label>
            <insite:DateSelector runat="server" ID="DateFrom" Width="350" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Employed To
            </label>
            <insite:DateSelector runat="server" ID="DateTo" Width="350" />
            <div class="form-text">
                Leave blank if currently working in this role.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Employer Name
                <insite:RequiredValidator runat="server" ControlToValidate="EmployerName" FieldName="Employer Name" ValidationGroup="CandidateExperience" />
            </label>
            <insite:TextBox runat="server" ID="EmployerName" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Brief description of duties (500 character limit)
            </label>
            <insite:TextBox runat="server" ID="EmployerDescription" MaxLength="500" TextMode="MultiLine" Rows="2" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Job Title
                <insite:RequiredValidator runat="server" ControlToValidate="JobTitle" FieldName="Job Title" ValidationGroup="CandidateExperience" />
            </label>
            <insite:TextBox runat="server" ID="JobTitle" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Country
                <insite:RequiredValidator runat="server" ControlToValidate="CountrySelector" FieldName="Country" ValidationGroup="CandidateExperience" />
            </label>
            <insite:FindCountry ID="CountrySelector" runat="server" EmptyMessage="Country" />
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                City
                <insite:RequiredValidator runat="server" ControlToValidate="City" FieldName="City" ValidationGroup="CandidateExperience" />
            </label>
            <insite:TextBox runat="server" ID="City" />
        </div>

    </div>
</div>