<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AchievementExpirationField.ascx.cs" Inherits="InSite.UI.Admin.Records.Achievements.Controls.AchievementExpirationField" %>

<div class="form-group mb-3">
    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ExpiryUpdatePanel" />
    <insite:UpdatePanel runat="server" ID="ExpiryUpdatePanel">
        <ContentTemplate>
            <label class="form-label">
                <asp:Literal runat="server" ID="LabelTextOutput" EnableViewState="false" />
                <insite:RequiredValidator runat="server" ID="ExpirationDateRequiredValidator" FieldName="Fixed Date" ControlToValidate="ExpirationDate" />
                <insite:RequiredValidator runat="server" ID="LifetimeQuantityRequiredValidator" FieldName="Relative Date" ControlToValidate="LifetimeQuantity" />
            </label>
            <div>
                <div class="mb-2">
                    <asp:RadioButton runat="server" ID="ExpirationTypeNoExpiry" Text="No Expiry" GroupName="ExpirationType" Checked="true" />
                </div>
                <div class="mb-2">
                    <div>
                        <asp:RadioButton runat="server" ID="ExpirationTypeFixed" Text="Fixed Date" GroupName="ExpirationType" />
                    </div>
                    <div runat="server" id="ExpirationDateArea" class="ms-4 mt-1">
                        <insite:DateTimeOffsetSelector ID="ExpirationDate" runat="server" Width="300" />
                    </div>
                </div>
                <div class="mb-2">
                    <div>
                        <asp:RadioButton runat="server" ID="ExpirationTypeRelative" Text="Relative Date" GroupName="ExpirationType" />
                    </div>
                    <div runat="server" id="LifetimeArea" class="ms-4 mt-1">
                        <insite:NumericBox runat="server" ID="LifetimeQuantity" Width="80" NumericMode="Integer" MinValue="1" ValueAsInt="3" CssClass="d-inline-block" />
                        <insite:ComboBox runat="server" ID="LifetimeUnit" Width="120px">
                            <Items>
                                <insite:ComboBoxOption Value="Month" Text="Month" />
                                <insite:ComboBoxOption Value="Year" Text="Year" Selected="true" />
                            </Items>
                        </insite:ComboBox>
                    </div>
                </div>
            </div>
            <div runat="server" ID="HelpTextOutput" class="form-text" enableviewstate="false"></div>
        </ContentTemplate>
    </insite:UpdatePanel>
</div>
