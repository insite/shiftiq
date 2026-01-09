<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Accounts.Senders.Controls.Details" %>

<div class="row">
    <div class="col-md-6">
        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">

                <h4 class="card-title mb-3">Sender</h4>

                <div runat="server" id="OrganizationField" class="form-group mb-3">
                    <insite:UpdatePanel runat="server">
                        <ContentTemplate>
                            <label class="form-label">
                                Organization
                                <insite:RequiredValidator runat="server" ControlToValidate="OrganizationSelector" FieldName="Organization" ValidationGroup="Sender" />
                            </label>
                            <insite:FindOrganization runat="server" ID="OrganizationSelector" AllowClear="false" />
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Nickname
                        <insite:RequiredValidator runat="server" ControlToValidate="SenderNickname" FieldName="Nickname" ValidationGroup="Sender" />
                    </label>
                    <insite:TextBox runat="server" ID="SenderNickname" Width="100%" MaxLength="100" />
                    <div class="form-text">Nickname is for your reference only; this field will not be displayed to recipients.</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Type
                        <insite:RequiredValidator runat="server" ControlToValidate="SenderType" FieldName="Type" ValidationGroup="Sender" />
                    </label>
                    <div>
                        <asp:RadioButtonList runat="server" ID="SenderType" RepeatDirection="Vertical" RepeatLayout="Table">
                            <asp:ListItem Value="Mailgun" Text="Mailgun" />
                            <asp:ListItem Value="DirectAccess" Text="Direct Access" />
                        </asp:RadioButtonList>
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        From Name
                        <insite:RequiredValidator runat="server" ControlToValidate="SenderName" FieldName="From Name" ValidationGroup="Sender" />
                    </label>
                    <insite:TextBox runat="server" ID="SenderName" Width="100%" MaxLength="100" />
                    <div class="form-text">Friendly name to show recipient rather than the email address.</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        From Email (Reply To)
                        <insite:RequiredValidator runat="server" ControlToValidate="SenderEmail" FieldName="From Email" ValidationGroup="Sender" />
                    </label>
                    <insite:TextBox runat="server" ID="SenderEmail" Width="100%" MaxLength="254" />
                    <div class="form-text">Email address that recipient will reply to.</div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        System Mailbox
                        <insite:RequiredValidator runat="server" ControlToValidate="SystemMailbox" FieldName="System Mailbox" ValidationGroup="Sender" />
                    </label>
                    <insite:TextBox runat="server" ID="SystemMailbox" Width="100%" MaxLength="254" />
                    <div class="form-text">
                        Verification is required. If this email domain doesn't match one of your authenticated domains,
                        then you'll need to verify ownership of this email address before using this sender. 
                        We'll send this email address a verification email after you create this sender.
                    </div>
                </div>

                <div class="form-group mb-3" runat="server" id="SenderIdentifierField" visible="false">
                    <label class="form-label">
                        Identifier
                    </label>
                    <div>
                        <asp:Literal runat="server" ID="SenderIdentifier" />
                    </div>
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Sender Status
                        <insite:RequiredValidator runat="server" ControlToValidate="SenderEnabled" FieldName="Sender Status" ValidationGroup="Sender" />
                    </label>
                    <insite:ComboBox runat="server" ID="SenderEnabled">
                        <Items>
                            <insite:ComboBoxOption Text="Enabled" Value="True"  />
                            <insite:ComboBoxOption Text="Disabled" Value="False" Selected="true" />
                        </Items>
                    </insite:ComboBox>
                </div>

            </div>
        </div>
    </div>

    <div class="col-md-6">
        <div class="card border-0 shadow-lg mb-3">
            <div class="card-body">

                <h4 class="card-title mb-3">Company</h4>

                <insite:UpdatePanel runat="server" ID="CompanyUpdatePanel" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="form-group mb-3">
                            <label class="form-label">
                                Address
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanyAddress" FieldName="Address" ValidationGroup="Sender" />
                            </label>
                            <insite:TextBox runat="server" ID="CompanyAddress" Width="100%" MaxLength="100" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                City
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanyCity" FieldName="City" ValidationGroup="Sender" />
                            </label>
                            <insite:TextBox runat="server" ID="CompanyCity" Width="100%" MaxLength="50" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Postal Code
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanyPostalCode" FieldName="Postal Code" ValidationGroup="Sender" />
                            </label>
                            <insite:TextBox runat="server" ID="CompanyPostalCode" Width="100%" MaxLength="10" />
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">
                                Country
                                <insite:RequiredValidator runat="server" ControlToValidate="CompanyCountry" FieldName="Country" ValidationGroup="Sender" />
                            </label>
                            <insite:TextBox runat="server" ID="CompanyCountry" Width="100%" MaxLength="50" />
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

                <h4>Please Note</h4>

                <div class="form-group mb-3">
                    You are required to include your contact information, including a physical mailing address, 
                    inside every promotional email you send in order to comply with the anti-spam laws such as 
                    <a target="_blank" href="https://www.ftc.gov/tips-advice/business-center/guidance/can-spam-act-compliance-guide-business">CAN-SPAM</a>
                    and 
                    <a target="_blank" href="https://laws-lois.justice.gc.ca/eng/annualstatutes/2010_23/FullText.html">CASL</a>.
                    You'll find replacement tags for this information in the footer of all the email designs InSite provides.
                </div>

            </div>
        </div>

    </div>
</div>