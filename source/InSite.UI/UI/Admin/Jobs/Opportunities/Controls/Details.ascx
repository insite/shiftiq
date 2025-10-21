<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Opportunities.Controls.Details" %>

<div class="row">

    <div class="col-lg-6 mb-3 mb-lg-0">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>Opportunity Details</h3>

                <div class="form-group mb-3">
                    <label class="form-label">Status</label>
                    <div>
                        <asp:CheckBox runat="server" ID="PublishedCheckBox" Text="Published (posted to Shift iQ Job Board for Candidates to Apply)" Checked="true" />
                        <div class="form-text">
                            <asp:Literal ID="PublishedCheckBoxDateTime" runat="server"></asp:Literal>
                        </div>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Occupation Area
                        <insite:RequiredValidator runat="server" FieldName="Occupation Area" ControlToValidate="OccupationIdentifier" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" EmptyMessage="Select One" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Job Position
                        <insite:RequiredValidator runat="server" ControlToValidate="JobPosition" FieldName="Job Position" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:TextBox ID="JobPosition" runat="server" MaxLength="100" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Job Location
                        <insite:RequiredValidator runat="server" ControlToValidate="JobLocation" FieldName="Job Location" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:TextBox ID="JobLocation" runat="server" MaxLength="200" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Type of Employment
                        <insite:RequiredValidator runat="server" ControlToValidate="EmploymentType" FieldName="Type of Employment" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:EmploymentTypeComboBox runat="server" ID="EmploymentType" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Position Type
                        <insite:RequiredValidator runat="server" ControlToValidate="PositionType" FieldName="Position Type" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:PositionTypeComboBox runat="server" ID="PositionType" />
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div class="col-lg-6">

        <div class="card border-0 shadow-lg mb-3">

            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Employer
                        <insite:RequiredValidator runat="server" ControlToValidate="EmployerGroupIdentifier" FieldName="Employer" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:FindGroup runat="server" ID="EmployerGroupIdentifier" EmptyMessage="Employer group" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Employer contact
                        <insite:RequiredValidator runat="server" ControlToValidate="EmployerContactIdentifier" FieldName="Employer contact" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:FindPerson runat="server" ID="EmployerContactIdentifier" EmptyMessage="Employer contact" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        About the company
                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="AboutTheCompany" TextMode="MultiLine" Rows="5" MaxLength="1400" />
                    </div>
                    <div class="form-text">Short description about the employer group</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Web Site URL
                    </label>
                    <div>
                        <insite:TextBox ID="WebSiteUrl" runat="server" MaxLength="500" />
                    </div>
                </div>

            </div>

        </div>

        <div class="card border-0 shadow-lg">

            <div class="card-body">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Email for Application Submissions
                        <insite:EmailValidator runat="server" ControlToValidate="EmailAddress" FieldName="Email for Application Submissions" ValidationGroup="Job" Display="Dynamic" />
                        <insite:RequiredValidator runat="server" ControlToValidate="EmailAddress" FieldName="Email for Application Submissions" ValidationGroup="Job" />
                    </label>
                    <div>
                        <insite:TextBox ID="EmailAddress" runat="server" MaxLength="254" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Resume required with applications?</label>
                    <div>
                        <insite:ComboBox runat="server" ID="SubmitResume" CssClass="w-25">
                            <Items>
                                <insite:ComboBoxOption Text="Yes" Value="True" />
                                <insite:ComboBoxOption Text="No" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">Cover Letter required with applications?</label>
                    <div>
                        <insite:ComboBox runat="server" ID="SubmitCoverLetter" CssClass="w-25">
                            <Items>
                                <insite:ComboBoxOption Text="Yes" Value="True" />
                                <insite:ComboBoxOption Text="No" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

            </div>

        </div>
    </div>

</div>

<div class="row mt-3">

    <div class="col-lg-12 mb-3 mb-lg-0">

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h3>
                    Job Description
                    <asp:CustomValidator runat="server" ID="JobDescriptionValidator" ValidationGroup="Job" Display="None" />
                    <sup class="text-danger"><i class="far fa-asterisk fa-xs"></i></sup>
                </h3>

                <div>
                    <insite:MarkdownEditor runat="server" ID="JobDescriptionText" UploadControl="CommentUpload" />
                    <insite:EditorUpload runat="server" ID="JobDescriptionUpload" />
                </div>

            </div>
        </div>
    </div>

</div>