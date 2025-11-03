<%@ Page Language="C#" CodeBehind="ModifyAchievement.aspx.cs" Inherits="InSite.Admin.Records.Programs.ModifyAchievement" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="ExpirationField" Src="~/UI/Admin/Records/Achievements/Controls/AchievementExpirationField.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Achievement" />

    <section class="mb-3">
        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div runat="server" id="AchievementIdentifierField" class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconButton runat="server" id="AchievementCreateButton" ToolTip="Add a new certificate" Name="plus-square" />
                                <insite:IconLink runat="server" id="AchievementOutlineLink" ToolTip="View details for this certificate" Name="external-link-square" Target="_blank" />
                            </div>
                            <label class="form-label">Achievement</label>
                            <div>
                                <insite:FindAchievement runat="server" ID="AchievementIdentifier" />
                            </div>
                        </div>

                        <insite:Container runat="server" id="AchievementFields">

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement Name
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementName" FieldName="Achievement Name" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Achievement Type
                                    <insite:RequiredValidator runat="server" ControlToValidate="AchievementLabel" FieldName="Achievement Type" ValidationGroup="Achievement" />
                                </label>
                                <div>
                                    <insite:TextBox runat="server" ID="AchievementLabel" />
                                </div>
                            </div>

                            <uc:ExpirationField runat="server" ID="AchievementExpiration" 
                                LabelText="Achievement Expiration" HelpText="" 
                                ValidationGroup="Achievement" />

                            <div class="form-group mb-3">
                                <label class="form-label">Certificate Layout</label>
                                <div>
                                    <insite:CertificateLayoutComboBox CssClass="dropup" runat="server" ID="AchievementLayout" />
                                </div>
                            </div>

                        </insite:Container>

                    </div>
                </div>
            </div>
        </div>
    </section>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Achievement" />
        <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
    </div>

</asp:Content>