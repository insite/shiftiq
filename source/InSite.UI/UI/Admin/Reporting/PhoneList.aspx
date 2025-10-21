<%@ Page Language="C#" CodeBehind="PhoneList.aspx.cs" Inherits="InSite.UI.Admin.Reporting.PhoneList" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentIdentifierValidator" ErrorMessage="At least one department must be selected" Display="None" ValidationGroup="Report" />

    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
        <Triggers>
            <asp:PostBackTrigger ControlID="DownloadXlsx" />
        </Triggers>
        <ContentTemplate>
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div runat="server" ID="DepartmentsField" class="form-group mb-3">
                        <label class="form-label">
                            Departments
                        </label>
                        <div class="ms-2 mt-1">
                            <insite:CheckBoxList ID="Departments" runat="server" RepeatColumns="3" CssClass="check-box-list" />
                            <div class="mt-2">
                                <insite:Button ID="SelectAllDepartmentsButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                <insite:Button ID="DeselectAllDepartmentsButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                            </div>
                        </div>
                    </div>

                    <div runat="server" ID="MembershipField" class="form-group mb-3">
                        <label class="form-label">
                            Membership
                        </label>
                        <div class="ms-2 mt-1">
                            <insite:CheckBoxList ID="RoleTypeSelector" runat="server">
    	                        <asp:ListItem Value="Department" Text="Department Employment" Selected="True" />
                                <asp:ListItem Value="Organization" Text="Organization Employment" Selected="True" />
    	                        <asp:ListItem Value="Administration" Text="Data Access" Selected="True" />
                            </insite:CheckBoxList>
                        </div>
                    </div>

                    <div runat="server" ID="RolesField" class="form-group mb-3">
                        <label class="form-label">
                            Roles
                        </label>
                        <div class="ms-2 mt-1">
                            <insite:CheckBoxList ID="Roles" runat="server" RepeatColumns="3" CssClass="check-box-list" />
                            <div class="mt-2">
                                <insite:Button ID="SelectAllRolesButton" runat="server" Icon="far fa-check-square" Text="Select All" ButtonStyle="OutlinePrimary" />
                                <insite:Button ID="DeselectAllRolesButton" runat="server" Icon="far fa-square" Text="Deselect All" ButtonStyle="OutlinePrimary" />
                            </div>
                        </div>
                    </div>

                    <div class="form-group mb-3">
                        <label class="form-label">
                            Status
                        </label>
                        <div class="ms-2 mt-1">
                            <insite:CheckBox ID="IsApproved" runat="server" Text="Approved" Checked="true" />
                        </div>
                    </div>

                </div>
            </div>

            <div class="mt-3">
                <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download XLSX" ButtonStyle="Primary" ValidationGroup="Report" CausesValidation="true" />
                <insite:CloseButton runat="server" NavigateUrl="/ui/cmds/reports" />
            </div>

        </ContentTemplate>
    </insite:UpdatePanel>

</asp:Content>
