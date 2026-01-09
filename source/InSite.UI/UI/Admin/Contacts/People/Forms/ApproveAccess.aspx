<%@ Page CodeBehind="ApproveAccess.aspx.cs" Inherits="InSite.Admin.Contacts.People.Forms.ApproveAccess" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>	

<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/OrganizationInfo.ascx" TagName="OrganizationInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ApproveAccess" />

    <div class="row mb-3 mt-3">
        <div class="col-lg-4">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3>Person</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Name
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="PersonName" />
                            <a runat="server" id="PersonLink"></a>
                        </div>
                    </div>
            
                    <div class="form-group mb-3">
                        <label class="form-label">
                            Email
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="PersonEmail" />
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:ContentTextLiteral runat="server" ContentLabel="Person Code"/>
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="PersonCode" />
                        </div>
                    </div>

                    <div runat="server" id="BirthDiv" class="form-group mb-3">
                        <label class="form-label">
                            Birthdate
                        </label>
                        <div>
                            <asp:Literal runat="server" ID="PersonBirthdate" />
                        </div>
                    </div>

                    <insite:UpdatePanel runat="server" CssClass="form-group mb-3">
                        <ContentTemplate>
                            <div runat="server" id="EmployerBadge" class="float-end badge bg-custom-default" visible="false"></div>
                            <label class="form-label">
                                Employer
                            </label>
                            <div>
                                <insite:FindEmployer runat="server" ID="EmployerGroupIdentifier" />
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3>Permission</h3>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Organization Role
                        </label>
                        <div>
                            <asp:CheckBox runat="server" ID="IsAdministrator" Text="Administrator" /><br />
                            <asp:CheckBox runat="server" ID="IsLearner" Text="Learner" /><br />
                        </div>
                        <div class="form-text">
                        </div>
                    </div>

                    <h3>Organization</h3>

                    <uc:OrganizationInfo runat="server" ID="OrganizationInfo" />

                </div>
            </div>
        </div>
        <div class="col-lg-4">
            <div class="card border-0 shadow-lg h-100">
                <div class="card-body">

                    <h3>Group</h3>

                    <insite:UpdatePanel runat="server">
                        <ContentTemplate>

                            <div class="mb-3">
                                <asp:RadioButtonList runat="server" ID="RegisterWithGroup">
                                    <asp:ListItem Value="Attach" Text="Register with an existing group" Selected="True" />
                                    <asp:ListItem Value="Create" Text="Register with a new group" />
                                    <asp:ListItem Value="None" Text="Do not register with a group" />
                                </asp:RadioButtonList>
                            </div>

                            <insite:Container runat="server" ID="AttachGroupContainer" Visible="false">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Group
                                        <insite:RequiredValidator runat="server" ControlToValidate="AttachGroupIdentifier" FieldName="Group" ValidationGroup="ApproveAccess" />
                                    </label>
                                    <div>
                                        <insite:FindGroup runat="server"  ID="AttachGroupIdentifier" Width="100%" />
                                    </div>
                                </div>
                            </insite:Container>

                            <insite:Container runat="server" ID="CreateGroupContainer" Visible="false">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Group Type
                                        <insite:RequiredValidator runat="server" ControlToValidate="CreateGroupType" FieldName="Group Type" ValidationGroup="ApproveAccess" />
                                    </label>
                                    <div>
                                        <insite:GroupTypeComboBox runat="server" ID="CreateGroupType" />
                                    </div>
                                </div>
                                
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Group Name
                                        <insite:RequiredValidator runat="server" ControlToValidate="CreateGroupName" FieldName="Group Name" ValidationGroup="ApproveAccess" />
                                    </label>
                                    <div>
                                        <insite:TextBox runat="server" ID="CreateGroupName" MaxLength="90" />
                                    </div>
                                </div>
                            </insite:Container>

                        </ContentTemplate>
                     </insite:UpdatePanel>

                </div>
            </div>
        </div>
    </div>

    <div class="alert alert-info mb-3" role="alert">
        <i class="fas fa-stop-circle"></i> <strong>Confirm:</strong>
        Do you want to grant or deny access for <asp:Literal runat="server" ID="ConfirmPersonName" /> to the <asp:Literal runat="server" ID="OrganizationName" /> organization account?
        An email notification will be sent to <asp:Literal runat="server" ID="ConfirmPersonNameFirst" /> with your decision.
        (If you deny access then this person will remain in your contact database, but without access to sign in.)
    </div>
    
    <div>
        <insite:Button runat="server" ID="GrantButton" Text="Grant System Access" Icon="fas fa-thumbs-up" ButtonStyle="Success" ValidationGroup="ApproveAccess" />
        <insite:Button runat="server" ID="DenyButton" Text="Deny System Access" Icon="fas fa-thumbs-down" ButtonStyle="Danger" CausesValidation="false" />	
    </div>	

</asp:Content>