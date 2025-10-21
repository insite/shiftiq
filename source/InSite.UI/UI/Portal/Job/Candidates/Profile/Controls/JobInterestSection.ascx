<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="JobInterestSection.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.JobInterestSection" %>

<div id='<%= ClientID %>' class="card mb-3">
    <div class="card-body">

        <h4 class="card-title mb-3">Job Interest Information</h4>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Consent to Share Records
                        <insite:RequiredValidator runat="server" ControlToValidate="ConsentToShare" FieldName="Consent to Share Records" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="ConsentToShare">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Yes" Text="Consent Given" />
                                <insite:ComboBoxOption Value="No" Text="No Consent" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                    <div class="form-text">
                        I accept that my resume, work experience survey,
                        and (if applicable) competency assessment results may be shared with interested employers.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Are you actively seeking employment?
                    </label>
                    <div>
                        <insite:RadioButton runat="server" ID="ActivelySeekingYes" Text="Yes" GroupName="ActivelySeeking" />
                        <insite:RadioButton runat="server" ID="ActivelySeekingNo" Text="No" GroupName="ActivelySeeking" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Current city
                        <insite:RequiredValidator runat="server" ControlToValidate="HomeAddressCity" FieldName="Current city" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <insite:TextBox runat="server" ID="HomeAddressCity" MaxLength="128" />
                </div>

                <div class="form-group mb-3" runat="server" id="RelocateInformation">
                    <label class="form-label">
                        Are you willing to move to another city?
                    </label>
                    <div class="col-sm-5">
                        <insite:RadioButton runat="server" ID="IsWillingToRelocateYes" Text="Yes" GroupName="IsWillingToRelocate" />
                        <insite:RadioButton runat="server" ID="IsWillingToRelocateNo" Text="No" GroupName="IsWillingToRelocate" />
                        <insite:RadioButton runat="server" ID="IsWillingToRelocateUnsure" Text="Not Sure" GroupName="IsWillingToRelocate" />
                    </div>
                    <div class="form-text">
                        Employers looking for workers will be from cities across Canada.
                        Please indicate if you are willing to move to another city or not.
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        LinkedIn Profile
                    </label>
                    <insite:TextBox runat="server" ID="LinkedInUrl" MaxLength="200" />
                    <div class="form-text">
                        Include full URL please.
                    </div>
                </div>

            </div>

            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Select an occupational area
                        <insite:RequiredValidator runat="server" ControlToValidate="OccupationIdentifier" FieldName="Occupation" ValidationGroup="Profile" Display="Dynamic" />
                    </label>
                    <div>
                        <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" />
                    </div>
                    <div class="form-text">
                        In what occupational area are you seeking employment?
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label runat="server" id="AccountStatusLabel" class="form-label">
                        Current Status
                    </label>
                    <insite:ItemIdComboBox runat="server" ID="AccountStatusId" EmptyMessage="Account Status" />
                    <div class="form-text">
                        The current status of your account in the organization.
                    </div>
                </div>

            </div>

        </div>

    </div>
</div>
