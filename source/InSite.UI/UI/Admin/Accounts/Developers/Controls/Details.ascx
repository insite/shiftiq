<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Details.ascx.cs" Inherits="InSite.Admin.Accounts.Developers.Controls.Details" %>

<div class="card border-0 shadow-lg">
    <div class="card-body">

        <h4 class="card-title mb-3">
            <i class="far fa-user-cog me-1"></i>
            Developer
        </h4>

        <div class="row">
            <div class="col-lg-4 col-md-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        User
                        <insite:RequiredValidator runat="server" ControlToValidate="UserIdentifier" FieldName="User" ValidationGroup="Developer" />
                    </label>
                    <insite:FindUser runat="server" ID="UserIdentifier" EmptyMessage="Test" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Developer Name
                    </label>
                    <insite:TextBox runat="server" ID="TokenName" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Developer Email
                    </label>
                    <insite:TextBox runat="server" ID="TokenEmail" />
                </div>

            </div>

            <div class="col-lg-4 col-md-6">

                <div class="form-group mb-3">
                    <label class="form-label">
                        API Access Token (Secret)
                    </label>
                    <insite:TextBox runat="server" ID="TokenSecret" />
                </div>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Expiry
                    </label>
                    <insite:DateTimeOffsetSelector runat="server" ID="TokenExpired" />
                </div>

            </div>

            <div class="col-lg-4">

                <div class="form-group mb-3">
                    <label class="form-label">
                        Organizations
                    </label>
                    <insite:FindOrganization runat="server" ID="OrganizationIdentifier" EmptyMessage="Any Organization" Output="List" MaxSelectionCount="0" />
                </div>

                <div runat="server" class="form-group mb-3" visible="false">
                    <label class="form-label">
                        IP Addresses
                        <insite:CustomValidator runat="server" ID="AddressesValidator" Display="None" ValidationGroup="Developer" />
                    </label>
                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AddressUpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="AddressUpdatePanel">
                        <ContentTemplate>
                            <div style="position:absolute; right:15px;">
                                <insite:Button runat="server" ID="AddAddress" CausesValidation="false" ButtonStyle="Default" Icon="fas fa-plus-circle" />
                            </div>
                            <asp:Repeater runat="server" ID="AddressRepeater">
                                <ItemTemplate>
                                    <div style="width:calc(100% - 45px); margin-bottom:5px;">
                                        <insite:TextBox runat="server" ID="Address" Text='<%# (string)Container.DataItem %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </div>

    </div>
</div>