<%@ Page Language="C#" CodeBehind="CompetencyStatusPerPerson.ascx.cs" Inherits="InSite.Cmds.Actions.Reports.CompetencyStatusPerPerson" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

    <insite:CustomValidator runat="server" ID="DepartmentPersonValidator" ErrorMessage="Please select a department or person and click Search." Display="None" ValidationGroup="Report" />

    <insite:Nav runat="server" ID="NavPanel">
        <insite:NavItem runat="server" ID="CriteriaTab" Title="Criteria" Icon="far fa-search" IconPosition="BeforeText">

            <h2 class="h4 my-3">
                Criteria
            </h2>

            <div class="row">
                <div class="col-lg-6">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                    <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="ReportButton" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                        </label>
                                        <cmds:FindDepartment runat="server" ID="DepartmentIdentifier" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Membership
                                        </label>
                                        <div>
                                            <asp:CheckBoxList runat="server" ID="EmploymentTypes" RepeatLayout="Flow" RepeatDirection="Vertical">
                                                <asp:ListItem Text="Administration" Value="Administration" />
                                                <asp:ListItem Text="Organization" Value="Organization" Selected="true" />
                                                <asp:ListItem Text="Department" Value="Department" Selected="true" />
                                            </asp:CheckBoxList>
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Person
                                        </label>
                                        <cmds:FindPerson ID="PersonIdentifier" runat="server" />
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Profiles
                                        </label>
                                        <div>
                                            <insite:ComboBox runat="server" ID="ProfileMode">
                                                <Items>
                                                    <insite:ComboBoxOption Text="View primary profile only " Value="Primary" />
                                                    <insite:ComboBoxOption Text="View one specific profile " Value="Specific" />
                                                    <insite:ComboBoxOption Text="View all profiles " Value="All" Selected="True" />
                                                </Items>
                                            </insite:ComboBox>
                                        </div>
                                    </div>

                                    <div runat="server" id="ProfileField" class="form-group mb-3" visible="false">
                                        <label class="form-label">
                                            Specific Profile
                                            <insite:RequiredValidator runat="server" ControlToValidate="ProfileIdentifier" FieldName="Profile" ValidationGroup="Report" Display="Dynamic" />
                                        </label>
                                        <cmds:FindProfile ID="ProfileIdentifier" runat="server" />
                                    </div>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                            </div>

                        </ContentTemplate>
                    </insite:UpdatePanel>

                </div>
            </div>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportTab" Title="Report" Icon="far fa-chart-bar" IconPosition="BeforeText" Visible="false">

            <h2 class="h4 my-3">
                Report
            </h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:DownloadButton runat="server" ID="DownloadXlsx" Text="Download XLSX" CssClass="mb-3" />

                    <asp:PlaceHolder runat="server" ID="ReportOutput"></asp:PlaceHolder>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>