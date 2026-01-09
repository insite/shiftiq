<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Utilities.Actions.Controls.Detail" %>

<div class="row">

    <div class="col-md-6">

        <h3>Identification</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Action Name
                <insite:RequiredValidator runat="server" ControlToValidate="ActionTitle" ValidationGroup="Action" />
            </label>
            <div class="row">
                <div class="col-md-8"><insite:TextBox runat="server" ID="ActionTitle" EmptyMessage="Full Name" /></div>
                <div class="col-md-4"><insite:TextBox runat="server" ID="NavigationText" EmptyMessage="Short Name" /></div>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Action URL
                <insite:RequiredValidator runat="server" ControlToValidate="ActionUrl" ValidationGroup="Action" />
            </label>
            <div>
                <insite:TextBox runat="server" ID="ActionUrl" MaxLength="500" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Action Type
            </label>
            <div class="row">
                
            </div>
            <div class="row">
                <div class="col-md-8">
                    <insite:RouteTypeComboBox runat="server" ID="ActionType" AllowBlank="False" />
                </div>
                <div class="col-md-4">
                    <insite:RouteAuthorityComboBox runat="server" ID="AuthorityType" AllowBlank="True" />
                </div>
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Controller Path
            </label>
            <div>
                <insite:TextBox runat="server" ID="ControllerPath" EmptyMessage="Controller Path" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Extra Breadcrumb
            </label>
            <div>
                <insite:TextBox runat="server" ID="ExtraBreadcrumb" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Action List
            </label>
            <div>
                <insite:TextBox runat="server" ID="ActionList" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Action Icon
            </label>
            <div>
                <insite:TextBox runat="server" ID="ActionIcon" />
            </div>
            <div class="form-text">Refer to the <a target="_blank" href="https://fontawesome.com/icons">Font Awesome</a> icon library.</div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Authorization Requirement
            </label>
            <div>
                <insite:CheckBox runat="server" ID="IsAuthorizationRequirementShiftIqExamEvent" Text="Always allow access to learners who sign in to an exam event" />
            </div>
            <div>
                <insite:CheckBox runat="server" ID="IsAuthenticationNotRequired" Text="Authentication is not required" />
            </div>
            <div class="form-text"></div>
        </div>

    </div>

    <div class="col-md-6">

        <h3>Navigation</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Help URL
            </label>
            <div>
                <insite:TextBox runat="server" ID="HelpUrl" MaxLength="500" EmptyMessage="/help/*" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Permission Parent
            </label>
            <div>
                <insite:FindPermission runat="server" ID="PermissionIdentifier" DropDownWidth="369px" />
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Navigation Upstream
            </label>
            <div>
                <insite:FindAction runat="server" ID="NavigationBack" />
            </div>
            <div class="form-text">
                <asp:Literal runat="server" ID="NavigationBackLink" />
            </div>
        </div>

        <div class="form-group mb-3" runat="server" id="NavigationForwardField">
            <label class="form-label">
                Navigation Downstream
            </label>
            <div>
                <asp:Repeater runat="server" ID="NavigationForwardRepeater">
                    <HeaderTemplate>
                        <ul>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <a href='/ui/admin/platform/routes/edit?id=<%# Eval("ActionIdentifier") %>'>
                                <%# Eval("ActionUrl") %>
                            </a>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate>
                        </ul>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="form-text">
                Actions that navigate back to <asp:Literal runat="server" ID="ActionUrl1" />
            </div>
        </div>

    </div>

</div>