<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Assessments.Publications.Forms.Edit" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Resource" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" Title="Assessment" Icon="far fa-balance-scale" IconPosition="BeforeText">

            <div class="row">
                <div class="col-lg-6 mb-3 mb-lg-0">

                    <div class="card h-100">
                        <div class="card-body">

                            <h3>Portal</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Page Title
                                    <insite:RequiredValidator runat="server" ID="PageTitleRequiredValidator" ControlToValidate="PageTitle" FieldName="External Title" ValidationGroup="Resource" />
                                </label>
                                <insite:TextBox ID="PageTitle" runat="server" MaxLength="128" />
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Page Icon
                                    <span class="field-icon">
                                        <asp:Label runat="server" ID="PageIconViewer"></asp:Label>
                                    </span>
                                </label>
                                <div>
                                    <insite:TextBox ID="PageIcon" runat="server" MaxLength="30" />
                                </div>
                                <div class="form-text">
                                    The <a href='https://fontawesome.com/icons'>FontAwesome icon</a> to be used for the program tile on the portal home page.
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Portal Visibility
                                </label>
                                <div>
                                    <asp:CheckBox runat="server" ID="PageIsHidden" Text="Hidden" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Portal Link URL
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="PageUrl" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
                <div class="col-lg-6 mb-3 mb-lg-0">

                    <div class="card h-100">
                        <div class="card-body">

                            <h3>Assessment</h3>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Form Title
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="FormTitle" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Form Name
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="FormName" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Asset Number and Version
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="FormAsset" />
                                </div>
                            </div>
                            
                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Form Code
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="FormCode" />
                                </div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">
                                    Status
                                </label>
                                <div>
                                    <asp:Literal runat="server" ID="PublicationStatus" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" Title="Privacy" Icon="far fa-key" IconPosition="BeforeText">
            <div class="row">
                <div class="col-lg-6 mb-3 mb-lg-0">

                    <div class="card">
                        <div class="card-body">

                            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="PrivacyUpdatePanel" />

                            <insite:UpdatePanel runat="server" ID="PrivacyUpdatePanel">
                                <ContentTemplate>

                                    <insite:Container runat="server" ID="PermissionContainer">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Groups
                                                <insite:RequiredValidator runat="server" ControlToValidate="GroupIdentifier" ValidationGroup="PrivacyGroup" />
                                            </label>
                                            <div class="d-flex">
                                                <div style="flex-basis:150px;">
                                                    <insite:GroupTypeComboBox runat="server" ID="GroupType" />
                                                </div>
                                                <div class="ms-1 flex-grow-1">
                                                    <insite:FindGroup runat="server" ID="GroupIdentifier" />
                                                </div>
                                                <div class="ms-2 align-self-center">
                                                    <insite:Button runat="server" ID="GrantPermission" Size="Default"
                                                        Icon="fas fa-plus-circle" Text="Grant Permission" ButtonStyle="OutlinePrimary" 
                                                        CausesValidation="true" ValidationGroup="PrivacyGroup" />
                                                </div>
                                            </div>
                                        </div>

                                        <div class="mt-3">
                                            <asp:Repeater runat="server" ID="PermissionRepeater">
                                                <HeaderTemplate><table class="table table-striped"><tbody></HeaderTemplate>
                                                <FooterTemplate></tbody></table></FooterTemplate>
                                                <ItemTemplate>
                                                    <tr>
                                                        <td>
                                                            <%# Eval("ContactName") %>
                                                            <span class="badge bg-primary ms-2"><%# Eval("GroupType") %></span>
                                                        </td>
                                                        <td class="text-end" style="width:50px;">
                                                            <insite:IconButton runat="server" Name="trash-alt" ToolTip="Delete Group" ConfirmText="Are you sure you want to remove this group?" 
                                                                CommandName="Delete" CommandArgument='<%# Eval("GroupIdentifier") %>' />
                                                        </td>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>

                                        </div>

                                    </insite:Container>

                                </ContentTemplate>
                            </insite:UpdatePanel>

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ContentTab" Title="Content" Icon="far fa-edit" IconPosition="BeforeText">
            <div class="card border-0 shadow-lg mt-3">
                <div class="card-body">

                    <insite:Nav runat="server" ID="ContentNav">

                        <insite:NavItem runat="server" ID="ContentTitleTab" Title="Title">

                            <div class="d-flex">
                                <div class="flex-grow-1">
                                    <uc:MultilingualStringInfo runat="server" ID="ContentTitle" />
                                </div>
                                <div class="ps-2">
                                    <insite:Button runat="server" id="EditContentTitleLink" ToolTip="Revise Title" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                </div>
                            </div>

                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="ContentSummaryTab" Title="Summary">

                            <div class="d-flex">
                                <div class="flex-grow-1">
                                    <uc:MultilingualStringInfo runat="server" ID="ContentSummary" />
                                </div>
                                <div class="ps-2">
                                    <insite:Button runat="server" id="EditContentSummaryLink" ToolTip="Revise Summary" ButtonStyle="OutlinePrimary" Text="Edit" Icon="far fa-pencil" />
                                </div>
                            </div>

                        </insite:NavItem>

                    </insite:Nav>

                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Resource" />
        <insite:DeleteButton runat="server" ID="DeleteButton" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>