<%@ Page Language="C#" CodeBehind="AcademicYear.aspx.cs" Inherits="InSite.UI.Admin.Records.Reports.Forms.AcademicYear" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="ReportPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-triangle me-1"></i>
            Academic Year Report
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <div class="row mb-3">
                    <div class="col-lg-6">
                        <insite:FindPeriod runat="server" ID="Period" Width="300" EmptyMessage="Period" />
                        <insite:Button runat="server" id="AddPeriodButton" ButtonStyle="Success" Text="Add Period" Icon="far fa-plus-circle" />
                    </div>
                </div>
                <div class="row mb-3">
                    <div class="col-lg-6">
                        <asp:Repeater runat="server" ID="PeriodRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <%# Eval("PeriodName") %>
                                    </td>
                                    <td style="width:40px;text-align:right;">
                                        <insite:IconButton runat="server" ID="DeletePeriodButton"
                                            CommandName="Delete"
                                            CommandArgument='<%# Eval("PeriodIdentifier") %>'
                                            ToolTip="Delete Period"
                                            Name="trash-alt"
                                        />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

                <insite:Button runat="server" ID="DownloadReportButton" Text="Download Report" Icon="fas fa-download" Visible="false" />

            </div>
        </div>
    </section>

</asp:Content>
