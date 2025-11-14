<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SelfEmploymentDetail.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.SelfEmploymentDetail" %>

<%@ Register Src="ItemList.ascx" TagName="ItemList" TagPrefix="uc" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">Details</h4>

                <div class="row">
                    <div class="col-6">
                        
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Name of Business
                                <insite:RequiredValidator runat="server" FieldName="Name of Business" ControlToValidate="EmployerName" ValidationGroup="Detail" Display="Dynamic" />
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
                                Self Employed From Year
                                <insite:RequiredValidator runat="server" FieldName="Self Employed From Year" ControlToValidate="ExperienceDateFrom" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:ComboBox runat="server" ID="ExperienceDateFrom" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Self Employed To Year
                            </label>
                            <insite:ComboBox runat="server" ID="ExperienceDateTo" AllowBlank="false" />
                        </div>

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

            </div>
            
        </div>

    </div>

</div>