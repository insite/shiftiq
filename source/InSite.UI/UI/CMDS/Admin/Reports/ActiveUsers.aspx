<%@ Page Language="C#" AutoEventWireup="false" CodeBehind="ActiveUsers.aspx.cs" Inherits="InSite.Cmds.Actions.Reports.ActiveUsers" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <style>
        .table.table-striped th {
            border-color: #e8e8e8;
        }
        select.btn {
            text-transform: none;
        }
        .employment-types label + input {
            margin-left: 10px;
        }
    </style>

    <div id="desktop">

        <section class="mb-3">

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

                    <insite:UpdatePanel runat="server" ID="UpdatePanel">
                        <Triggers>
                            <asp:PostBackTrigger ControlID="DownloadButton" />
                        </Triggers>
                        <ContentTemplate>
                            <div class="row">
                                <div class="col-lg-3">
                                    <label class="form-label">Grouping</label>
                                    <insite:ComboBox runat="server" ID="ddlGroupBy">
                                        <Items>
                                            <insite:ComboBoxOption Text="Do not group results" Value="DoNotGroup" />
                                            <insite:ComboBoxOption Text="Group by department" Value="Department" />
                                            <insite:ComboBoxOption Text="Group by role" Value="Role" />
                                        </Items>
                                    </insite:ComboBox>
                                </div>
                                <div class="col-lg-2">
                                    <label class="form-label">Person Name</label>
                                    <insite:TextBox runat="server" ID="NameFilter" />
                                </div>
                                <div class="col-lg-5">
                                    <label class="form-label d-block">Membership Scope</label>
                                    <insite:CheckBoxList runat="server" ID="MembershipFunction" RepeatLayout="Flow" RepeatDirection="Horizontal" />
                                </div>
                                <div class="col-lg-2">
                                    <label class="form-label">Hide Department/Role</label>
                                    <insite:TextBox runat="server" ID="ExcludeGroup" />
                                </div>
                            </div>
                            <div class="row mt-3">
                                <div class="col-lg-12">
                                    <insite:SearchButton runat="server" ID="ReportButton" Text="Search" Icon="fas fa-search" />
                                    <insite:DownloadButton runat="server" ID="DownloadButton" Text="Download" />
                                    <small runat="server" id="ResultCount" class="text-body-secondary ms-2"></small>
                                    <small runat="server" id="HelpSeparator" class="text-body-secondary ms-2">&bull;</small>
                                    <small runat="server" id="DepartmentsHelp" class="text-body-secondary ms-2">Numbers in [brackets] show how many profiles the person has in that department</small>
                                </div>
                            </div>
                            <br />
                            <insite:Alert runat="server" ID="ScreenStatus" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:PlaceHolder runat="server" ID="place" EnableViewState="false"></asp:PlaceHolder>
                                </div>
                            </div>
                        </ContentTemplate>
                    </insite:UpdatePanel>
                </div>
            </div>
        </section>

    </div>

</asp:Content>
