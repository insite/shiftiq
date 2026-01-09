<%@ Page Language="C#" CodeBehind="DepartmentProfileDetails.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.DepartmentProfileDetail" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:ValidationSummary runat="server" ValidationGroup="Report" />

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
                            <asp:PostBackTrigger ControlID="DownloadXlsx1" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="card border-0 shadow-lg">
                                <div class="card-body">

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                            <insite:RequiredValidator runat="server" ControlToValidate="DepartmentIdentifier" FieldName="Department" ValidationGroup="Report" />
                                        </label>
                                        <div>
                                            <cmds:FindDepartment ID="DepartmentIdentifier" runat="server" />
                                        </div>
                                    </div>

                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Profile
                                        </label>
                                        <div>
                                            <asp:RadioButtonList ID="ProfileMode" runat="server" RepeatLayout="Flow">
                                                <asp:ListItem Text="View primary profiles only" Value="Primary" Selected="True" />
                                                <asp:ListItem Text="View profiles that require compliance" Value="Compliance" />
                                                <asp:ListItem Text="View one specific profile" Value="Specific" />
                                            </asp:RadioButtonList>
                                        </div>
                                    </div>

                                    <div runat="server" id="ProfileField" class="form-group mb-3" visible="false">
                                        <label class="form-label">
                                            Specific Profile
                                            <insite:RequiredValidator runat="server" ControlToValidate="ProfileIdentifier" FieldName="Specific Profile" ValidationGroup="Report" />
                                        </label>
                                        <cmds:FindProfile ID="ProfileIdentifier" runat="server" />
                                    </div>

                                    <p>
                                        Please remember that this report allows you to report on both primary and secondary profiles.  
                                        Furthermore, this report counts competencies assigned to all of the workers in the selected organization/department, including those that are assigned to managers and supervisors.
                                    </p>

                                </div>
                            </div>

                            <div class="mt-3">
                                <insite:SearchButton runat="server" ID="ReportButton" Text="Report" Icon="fas fa-chart-bar" ValidationGroup="Report" CausesValidation="true" />
                                <insite:DownloadButton runat="server" ID="DownloadXlsx1" ValidationGroup="Report" CausesValidation="true" />
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

                    <insite:DownloadButton runat="server" ID="DownloadXlsx2" CssClass="mb-3" />

                    <div class="overflow-scroll" style="height:60vh; min-height:500px;">

                        <asp:Repeater runat="server" ID="DataRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                    </tbody>
                                </table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <asp:Literal runat="server" ID="RowLiteral" />
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>

                </div>
            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
