<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkExperienceDetail.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.WorkExperienceDetail" %>

<%@ Register Src="ItemList.ascx" TagName="ItemList" TagPrefix="uc" %>

<div class="card mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">Details</h4>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Job Title
                        <insite:RequiredValidator runat="server" FieldName="Job Title" ControlToValidate="ExperienceJobTitle" ValidationGroup="Detail" Display="Dynamic" />
                    </label>
                    <insite:TextBox runat="server" ID="ExperienceJobTitle" MaxLength="200" />
                    <div class="form-text">
                        What was your position during this time? Can be a volunteer or unpaid work experience role as well.
                    </div>
                </div>

            </div>
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Employed From
                        <insite:RequiredValidator runat="server" ControlToValidate="DateFrom" FieldName="Employed From" ValidationGroup="Detail" />
                    </label>
                    <insite:DateSelector runat="server" ID="DateFrom" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Employed To
                    </label>
                    <insite:DateSelector runat="server" ID="DateTo" />
                    <div class="form-text">
                        Leave blank if currently working in this role.
                    </div>
                </div>

            </div>
        </div> 


        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Company Name
                        <insite:RequiredValidator runat="server" FieldName="Company Name" ControlToValidate="EmployerName" ValidationGroup="Detail" Display="Dynamic" />
                    </label>
                    <insite:TextBox runat="server" ID="EmployerName" MaxLength="300" />
                    <div class="form-text">
                        What was the name of the company for which you worked?
                    </div>
                </div>

            </div>

            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        City
                        <insite:RequiredValidator runat="server" FieldName="City" ControlToValidate="ExperienceCity" ValidationGroup="Detail" Display="Dynamic" />
                    </label>
                    <insite:TextBox runat="server" ID="ExperienceCity" MaxLength="100" />
                    <div class="form-text">
                        What city was this job primarily located in?
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Country
                        <insite:RequiredValidator runat="server" FieldName="Country" ControlToValidate="CountrySelector" ValidationGroup="Detail" Display="Dynamic" />
                    </label>
                    <insite:FindCountry ID="CountrySelector" runat="server" EmptyMessage="Country" />
                </div>

            </div>
        </div>

        <div class="row mt-3">
            <div class="col-12">
                <div class="form-group mb-3">
                    <label class="form-label">
                        Brief description of duties (500 character limit)
                    </label>
                    <insite:TextBox runat="server" ID="EmployerDescription" MaxLength="500" TextMode="MultiLine" Rows="2" />
                </div>
            </div>
        </div>

    </div>
</div>