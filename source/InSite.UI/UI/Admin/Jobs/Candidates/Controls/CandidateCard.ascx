<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CandidateCard.ascx.cs" Inherits="InSite.UI.Admin.Jobs.Candidates.Controls.CandidateCard" %>

<div class="card">
    <div class="card-body">

        <h4 class="card-title mb-0">
            <asp:HyperLink runat="server" ID="CandidateName" />
        </h4>

        <p class="form-text">
            <asp:HyperLink runat="server" ID="CandidateEmail" />
        </p>

        <div class="row">
            <div class="col-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Consent to Share
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="ConsentToShare" Enabled="false">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Value="Yes" Text="Consent Given" />
                                <insite:ComboBoxOption Value="No" Text="No Consent" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Actively Seeking
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="IsActivelySeeking" Width="150px">
                            <Items>
                                <insite:ComboBoxOption />
                                <insite:ComboBoxOption Text="Yes" Value="True" />
                                <insite:ComboBoxOption Text="No" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Current City
                    </label>
                    <insite:TextBox runat="server" ID="AddressCity" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Willing to Relocate
                    </label>
                    <div>
                        <insite:ComboBox runat="server" ID="IsWillingToRelocate" Width="150px">
                            <Items>
                                <insite:ComboBoxOption Text="Not Sure" />
                                <insite:ComboBoxOption Text="Yes" Value="True" />
                                <insite:ComboBoxOption Text="No" Value="False" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        LinkedIn
                    </label>
                    <insite:TextBox runat="server" ID="LinkedInUrl" MaxLength="500" />
                </div>

            </div>
            <div class="col-6">
                
                <div class="form-group mb-3">
                    <label class="form-label">
                        Occupation Interest
                    </label>
                    <div>
                        <insite:OccupationListComboBox runat="server" ID="OccupationIdentifier" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label runat="server" id="AccountStatusLabel" class="form-label">
                        Current Status
                    </label>
                    <insite:ItemIdComboBox runat="server" ID="AccountStatusId" EmptyMessage="Account Status" />
                </div>

                <div runat="server" id="JobsAccessField" class="form-group mb-3">
                    <label class="form-label">
                        Jobs Access
                    </label>
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="JobsAccessUpdatePanel" />
                    <insite:UpdatePanel runat="server" ID="JobsAccessUpdatePanel">
                        <ContentTemplate>
                            <insite:CheckBox ID="IsJobsApproved" runat="server" Text="Job Profile Reviewed and Approved" />
                            <div runat="server" id="IsJobsApprovedDateTime" class="ps-4 ms-1" visible="false"></div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>

            </div>
        </div>

    </div>
</div>
