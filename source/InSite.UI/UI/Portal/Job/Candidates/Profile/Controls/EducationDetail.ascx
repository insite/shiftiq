<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EducationDetail.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.EducationDetail" %>

<div class="row mb-3">

    <div class="col-12">

        <div class="card">

            <div class="card-body">

                <h4 class="card-title mb-3">Details</h4>

                <div class="row">
                    <div class="col-6">
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Qualification
                                <insite:RequiredValidator runat="server" FieldName="Qualification" ControlToValidate="EducationQualification" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:ComboBox runat="server" ID="EducationQualification" />
                            <div class="form-text">
                                Enter the certification you received after completing your courses or program of study. Examples: degree, diploma, certificate.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Area of Study
                                <insite:RequiredValidator runat="server" FieldName="Area of Study" ControlToValidate="EducationName" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:TextBox runat="server" ID="EducationName" MaxLength="300" />
                            <div class="form-text">
                                Enter the name of the degree for which you are studying / studied.
                                Examples: &quot;accounting and finance&quot;, &quot;mechanical engineering&quot; &quot;architecture&quot;.
                                If you finished your education after or during high school, please state GED or High School Equivalent
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Name of School/College/University
                                <insite:RequiredValidator runat="server" FieldName="Name of School/College/University" ControlToValidate="EducationInstitution" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:TextBox runat="server" ID="EducationInstitution" MaxLength="300" />
                            <div class="form-text">
                                The name of the highschool, college, university or educational facility at which you studied.
                            </div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                City
                                <insite:RequiredValidator runat="server" FieldName="City" ControlToValidate="ExperienceCity" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:TextBox runat="server" ID="ExperienceCity" MaxLength="100" />
                            <div class="form-text">
                                What city was this school located in?
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
                    <div class="col-6">

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Attended From
                                <insite:RequiredValidator runat="server" FieldName="Attended From" ControlToValidate="EducationDateFrom" ValidationGroup="Detail" Display="Dynamic" />
                            </label>
                            <insite:DateSelector runat="server" ID="EducationDateFrom" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Attended To
                            </label>
                            <insite:DateSelector runat="server" ID="EducationDateTo" />
                            <div class="form-text">
                                Leave blank if currently in progress.
                            </div>
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

            </div>
            
        </div>

    </div>

</div>