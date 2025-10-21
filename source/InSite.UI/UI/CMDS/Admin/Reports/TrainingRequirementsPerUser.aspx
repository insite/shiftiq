<%@ Page Language="C#" CodeBehind="TrainingRequirementsPerUser.aspx.cs" Inherits="InSite.Cmds.Actions.Reporting.Report.TrainingRequirementsPerUser" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        table.check-list td {
            vertical-align: top;
        }

        table.check-list input {
            position: absolute;
            margin-top: 5px;
        }

        table.check-list label {
            padding-left: 27px;
        }

        table.table-competency > tbody > tr:last-child > td {
            border-bottom: 0;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <section>
        <h2 class="h4 mt-4 mb-3">
            Criteria
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ReportUpdatePanel" />
                
                <insite:UpdatePanel runat="server" ID="ReportUpdatePanel">
                    <ContentTemplate>
                        <div class="row">
                            <div class="col-lg-9">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Departments
                                    </label>
                                    <div>
                                        <asp:CheckBoxList ID="Departments" runat="server" RepeatColumns="3" CssClass="check-list" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-3">
                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Statuses
                                    </label>
                                    <div>
                                        <asp:RadioButtonList ID="Status" runat="server">
                                            <asp:ListItem Value="Expired" Text="Expired" />
                                            <asp:ListItem Value="Not Completed" Text="Not Completed" />
                                            <asp:ListItem Value="Not Applicable" Text="Not Applicable" />
                                            <asp:ListItem Value="Needs Training" Text="Needs Training" Selected="True" />
                                            <asp:ListItem Value="Self-Assessed" Text="Self-Assessed" />
                                            <asp:ListItem Value="Submitted for Validation" Text="Submitted for Validation" />
                                            <asp:ListItem Value="Validated" Text="Validated" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </insite:UpdatePanel>

                <div class="mt-3">
                    <insite:Button runat="server" ID="DownloadXlsx" ButtonStyle="Primary" Text="Download XLSX" Icon="far fa-download" />
                </div>
            </div>
        </div>

    </section>
</asp:Content>
