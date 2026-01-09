<%@ Page Language="C#" CodeBehind="Register.aspx.cs" Inherits="InSite.UI.Lobby.Register" MasterPageFile="~/UI/Layout/Lobby/LobbyLogin.master" %>

<%@ Register TagPrefix="uc" TagName="PasswordStrength" Src="./Controls/PasswordStrength.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

<div class="row">
	<div class="col-lg-4 col-md-6 offset-lg-1">
        <div class="view show" id="signin-view" style="position:relative;">

            <table class="login-toggle mb-4">
                <tr>
                    <td class="unselected">
                        <a href="/ui/lobby/signin">
                            <insite:Literal runat="server" Text="Returning Users" />
                        </a>
                    </td>
                    <td class="selected">
                        <insite:Literal runat="server" Text="New Users" />
                    </td>
                </tr>
            </table>

            <h1 class="h2"><insite:Literal runat="server" Text="Register New User" /></h1>

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

            <insite:UpdatePanel runat="server" ID="UpdatePanel" ChildrenAsTriggers="false">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RegisterSubmitButton" />
                </Triggers>
                <ContentTemplate>

                    <insite:Alert runat="server" ID="Status" />
                    
                    <asp:MultiView runat="server" ID="ScreenViews">

                        <asp:View runat="server" ID="RegisterView">

                            <asp:Literal runat="server" ID="RegisterStatusLiteral" ViewStateMode="Disabled" />
                            <asp:HiddenField runat="server" ID="FormKey" />
                            <insite:ValidationSummary runat="server" ValidationGroup="Register" />

                            <insite:Container runat="server" ID="RegisterEmployerField">

                                <div class="alert alert-info"><insite:Literal runat="server" Mode="Markdown" Text="User Register Help" /></div>

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        <insite:Literal runat="server" Text="Company" />
                                    </label>
                                    <insite:MultiField runat="server">

                                        <insite:MultiFieldView runat="server" ID="EmployerGroupSelectorView" Inputs="EmployerGroupSelector">
                                            <span class="multi-field-input">
                                                <insite:FindGroup runat="server" ID="EmployerGroupSelector" PageSize="15" MaxPageCount="15" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-keyboard" ToolTip="Enter a value manually" />
                                        </insite:MultiFieldView>

                                        <insite:MultiFieldView runat="server" ID="EmployerGroupTextView" Inputs="EmployerGroupText">
                                            <span class="multi-field-input">
                                                <insite:TextBox runat="server" ID="EmployerGroupText" MaxLength="90" />
                                            </span>
                                            <insite:Button runat="server" OnClientClick='inSite.common.multiField.nextView(this); return false;'
                                                ButtonStyle="Default" Icon="far fa-list-ul" ToolTip="Select an option from the list" />
                                        </insite:MultiFieldView>

                                    </insite:MultiField>

                                </div>

                            </insite:Container>

                            <div class="form-group mb-3" runat="server" id="LanguageField">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Language"  />
                                </label>
                                <insite:LanguageComboBox runat="server" ID="Language" AllowBlank="false" EnableTranslation="true" />
                            </div>
                            
                            <div class="form-group mb-3" runat="server" id="RolePanel">
                                <label class="form-label mb-3">
                                    <insite:Literal runat="server" Text="Role or Program" />
                                </label>

                                <asp:Repeater runat="server" ID="GroupList">
                                    <ItemTemplate>
                                        <div class="mb-3">
                                            <%# Eval("Heading") %>
                                            <asp:Repeater runat="server" ID="InnerRepeater" DataSource='<%# Eval("Items") %>'>
                                                <ItemTemplate>
                                                    <insite:RadioButton runat="server" ID="GroupIdentifier"
                                                        CssClass="form-check form-switch"
                                                        Value='<%# Eval("Value") %>'
                                                        Text='<%# Eval("Text") %>'
                                                        Checked='<%# Eval("IsSelected") %>'
                                                        GroupName="RoleOrProgram"
                                                    />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Email"  />
                                    <insite:RequiredValidator runat="server" Display="None" ControlToValidate="RegisterEmail" ValidationGroup="Register" />
                                    <insite:EmailValidator runat="server" Display="None" ControlToValidate="RegisterEmail" ValidationGroup="Register" FieldName="Email" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterEmail" MaxLength="128"  />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Confirm Email"  />
                                    <insite:RequiredValidator runat="server" ControlToValidate="RegisterConfirmEmail" Display="None" ValidationGroup="Register" />
                                    <insite:CompareValidator runat="server" ControlToValidate="RegisterConfirmEmail" Display="None" ControlToCompare="RegisterEmail" ValidationGroup="Register" ErrorMessage="Emails do not match" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterConfirmEmail" MaxLength="128"  />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="First Name"  />
                                    <insite:RequiredValidator runat="server" ControlToValidate="RegisterFirstName" Display="None" ValidationGroup="Register" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterFirstName" MaxLength="32"  />
                            </div>

                            <div runat="server" id="MiddleNamePanel" class="form-group mb-3" visible="false">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Middle Name"  />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterMiddleName" MaxLength="38"  />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Last Name"  />
                                    <insite:RequiredValidator runat="server" ControlToValidate="RegisterLastName" Display="None" ValidationGroup="Register" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterLastName" MaxLength="32"  />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Password"  />
                                    <insite:RequiredValidator runat="server" ControlToValidate="RegisterPassword" Display="None" ValidationGroup="Register" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterPassword" TextMode="Password" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Confirm Password"  />
                                    <insite:RequiredValidator runat="server" ControlToValidate="RegisterConfirmPassword" Display="None" ValidationGroup="Register" />
                                    <insite:CompareValidator runat="server" ControlToValidate="RegisterConfirmPassword" ControlToCompare="RegisterPassword" ValidationGroup="Register"
                                        Display="None" ErrorMessage="Password and confirmation does not match" />
                                </label>
                                <insite:TextBox runat="server" ID="RegisterConfirmPassword" TextMode="Password" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    <insite:Literal runat="server" Text="Password Strength" />
                                </label>
                                <div>
                                    <uc:PasswordStrength runat="server" ID="RegisterPasswordStrength" ControlID="RegisterPassword" />
                                </div>
                            </div>

                            <div style="padding-top: 20px;">
                                <insite:Button runat="server" ID="RegisterSubmitButton" ButtonStyle="Success" Icon="fas fa-sign-in-alt" ValidationGroup="Register" />
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="CmdsView">
                            <div>
                                <div class="alert alert-warning">
                                    <strong><insite:Literal runat="server" Text="Admin Use Only" /></strong>
                                    <insite:Literal runat="server" Text="Register Not Allowed (CMDS)" />
                                </div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="DisabledView">
                            <div class="alert d-flex alert-warning">
                                <i class="fas fa-exclamation-triangle fs-xl me-3"></i>
                                <div runat="server" ID="DisabledText"></div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="ClosedOrganizationView">
                            <div class="alert d-flex alert-warning">
                                <i class="fas fa-exclamation-triangle fs-xl me-3"></i>
                                <div runat="server" ID="ClosedOrganizationText"></div>
                            </div>
                        </asp:View>

                        <asp:View runat="server" ID="ExpiredView">
                            <div>
                                <h1><insite:Literal runat="server" Text="Register New User Account" /></h1>
                                <div class="alert alert-warning">
                                    <table>
                                        <tr>
                                            <td style="vertical-align: top; padding-right: 10px;"><i class="fa fa-exclamation-triangle fa-2x"></i></td>
                                            <td>
                                                <strong><insite:Literal runat="server" Text="Warning" />:</strong>
                                                <insite:Literal runat="server" Text="Register Form Expired" />
                                            </td>
                                        </tr>
                                    </table>
                                </div>
                            </div> 
                        </asp:View>

                        <asp:View runat="server" ID="SubmittedView">
                            <div>
                                <h1><insite:Literal runat="server" Text="Thank You" /></h1>

                                <p class="help">
                                    <insite:Literal runat="server" Text="Register Request Received" />
                                </p>

                                <asp:HyperLink runat="server" ID="SubmittedSignInLink" />
                            </div> 
                        </asp:View>

                    </asp:MultiView>

                </ContentTemplate>
            </insite:UpdatePanel>

        </div>
    </div>
    <div runat="server" id="CustomContentCard" class="col-lg-4 col-md-6 offset-lg-2">
        <div class="card shadow">
            <div class="card-body">
                <asp:Literal runat="server" ID="CustomContentHtml" />
            </div>
        </div>
    </div>
</div>

</asp:Content>