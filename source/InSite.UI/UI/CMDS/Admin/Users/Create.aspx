<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Cmds.Admin.People.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonDetails.ascx" TagName="PersonDetails" TagPrefix="uc" %>
<%@ Register Src="~/UI/CMDS/Common/Controls/User/PersonGrid.ascx" TagName="PersonGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="ContactInfo" />

    <asp:MultiView runat="server" ID="MultiView">

        <asp:View runat="server" ID="CreatorView">
            <insite:Nav runat="server" ID="NavPanel">

                <insite:NavItem runat="server" ID="PersonTab" Title="Person" Icon="far fa-user" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">
                            Person
                        </h2>

                        <div class="card border-0 shadow-lg">
                            <div class="card-body">

                                <uc:PersonDetails ID="PersonDetails" runat="server" />

                            </div>
                        </div>
                    </section>
                </insite:NavItem>

                <insite:NavItem runat="server" ID="CompanyTab" Title="Organization and Department" Icon="far fa-city" IconPosition="BeforeText">
                    <section>
                        <h2 class="h4 mt-4 mb-3">
                            Organization and Department
                        </h2>

                        <div class="row">
                            <div class="col-lg-6">
                                <div class="card border-0 shadow-lg">
                                    <div class="card-body">

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Organization
                                                <insite:RequiredValidator runat="server" FieldName="Organization" ControlToValidate="OrganizationIdentifier" ValidationGroup="ContactInfo" />
                                            </label>
                                            <cmds:FindCompany runat="server" ID="OrganizationIdentifier" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Department
                                                <insite:RequiredValidator runat="server" FieldName="Department" ControlToValidate="DepartmentIdentifier" ValidationGroup="ContactInfo" />
                                            </label>
                                            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                This person
                                            </label>
                                            <div>
                                                <asp:RadioButtonList ID="RoleType" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow">
					                                <asp:ListItem Value="Department" Text="is employed by this department" Selected="True" />
                                                    <asp:ListItem Value="Organization" Text="is employed by this organization" />
					                                <asp:ListItem Value="Administration" Text="has access to the data for this organization/department" />
                                                </asp:RadioButtonList>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </section>
                </insite:NavItem>

            </insite:Nav>

            <div class="mt-3">
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="ContactInfo" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

        </asp:View>

        <asp:View runat="server" ID="DuplicateView">

            <div class="alert alert-warning" role="alert">
                <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
                <asp:Literal ID="DuplicateWarningMessage" runat="server" />
            </div>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h2>Possible Duplicates</h2>

                    <div class="mt-3">
                        <uc:PersonGrid ID="Duplicates" runat="server" />
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:SaveButton ID="AddNewPersonButton" runat="server" Text="Yes - Add New Person" />
                <insite:Button ID="BackToEditButton" runat="server" Icon="fas fa-pencil" Text="No - Edit New Person" ButtonStyle="Default" />
                <asp:Label runat="server" ID="AddNewPersonInstruction" Text="Please contact the CMDS administrator to create this person's record (in order to help prevent duplication of data)" />
            </div>

        </asp:View>
    </asp:MultiView>

</asp:Content>