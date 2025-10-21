<%@ Page Language="C#" CodeBehind="RefreshReports.aspx.cs" Inherits="InSite.UI.Desktops.Custom.Ncsha.Reports.Forms.Refresh" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="AlertMessage" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="RefreshSection" Title="Refresh Reports" Icon="far fa-sync-alt" IconPosition="BeforeText">

            <section class="pb-4 mb-md-2">

                <h2 class="h4 mb-3">Refresh Reports</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-lg-6">

                                <table class="table table-striped">
                                    <thead>
                                        <tr>
                                            <th>Report Programs</th>
                                            <th style="width: 85px;"></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater runat="server" ID="SurveyRepeater">
                                            <ItemTemplate>
                                                <tr>
                                                    <td><%# Eval("Title") %></td>
                                                    <td>
                                                        <asp:Literal runat="server" ID="Code" Text='<%# Eval("Code") %>' Visible="false" />

                                                        <label class="switch">
                                                            <asp:CheckBox runat="server" AutoPostBack="true" ID="IsSelected" Checked='<%# Eval("IsSelected") %>' />
                                                            <div class="slider round"></div>
                                                        </label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            
                            </div>
                            <div class="col-lg-2">
                            
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Report Year
                                    </label>
                                    <div>
                                        <insite:YearComboBox runat="server" ID="YearComboBox1" />
                                    </div>
                                </div>

                            </div>
                        </div>

                        <div style="text-align: right;">
                            <insite:Button runat="server" ID="RefreshButton" ButtonStyle="Success" Icon="fas fa-cloud-upload" Text="Refresh" />
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ReportSection" Title="View Reports" Icon="far fa-chart-bar" IconPosition="BeforeText">

            <section class="pb-4 mb-md-2">

                <h2 class="h4 mb-3">View Reports</h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div>
                            <asp:Repeater runat="server" ID="Reports">
                                <ItemTemplate>
                                    <div>
                                        <asp:HyperLink runat="server" ID="ReportLink" Target="_blank" Text='<%# Eval("Code") + ": " + Eval("Title") %>' NavigateUrl='<%# Eval("NavigateUrl") %>' />
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="NavItem1" Title="Visibility Toggles" Icon="far fa-tasks" IconPosition="BeforeText">

            <section class="pb-4 mb-md-2">

                <h2 class="h4 mb-3">Visibility Toggles</h2>

                <div class="row">
                    <div class="col-lg-12">
                        <div class="alert alert-warning">Uncheck the box to hide the selected agency from the selected report for the selected year. Note that these will be reset if reports are refreshed for the selected year, and will need to be unchecked again.</div>
                    </div>
                </div>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <div class="row">
                            <div class="col-lg-6">

                                <div class="mb-3">
                                    <insite:GroupComboBox runat="server" ID="AgencyComboBox" EmptyMessage="Agency" />
                                </div>

                                <div class="mb-3">
                                    <insite:CheckBox runat="server" ID="MR05" Text="Visible on MR05" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MR06" Text="Visible on MR06" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MR07" Text="Visible on MR07" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MR08" Text="Visible on MR08" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MR09" Text="Visible on MR09" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MF09" Text="Visible on MF09" AutoPostBack="true" />
                                    <insite:CheckBox runat="server" ID="MF10" Text="Visible on MF10" AutoPostBack="true" />
                                </div>

                            </div>
                            <div class="col-lg-2">

                                <div class="mb-3">
                                    <insite:YearComboBox runat="server" ID="YearComboBox2" EmptyMessage="Year" />
                                </div>

                            </div>
                        </div>

                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

</asp:Content>
