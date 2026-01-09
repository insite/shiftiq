<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Delete.aspx.cs" Inherits="InSite.UI.Admin.Reports.Delete" %>

<%@ Register TagPrefix="uc" TagName="ReportDetail" Src="~/UI/Admin/Reports/Controls/ReportInfo.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="Status" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" />
    
    <div class="row">

        <div class="col-lg-6">

            <div class="card">
                <div class="card-body">
                    <h4 class="card-title">Report</h4>
                    <uc:ReportDetail runat="server" ID="ReportDetail" />
                </div>
            </div>

            <insite:DeleteConfirmAlert runat="server" Name="Report" CssClass="mt-3" />

            <div class="mt-3 ms-3 text-danger">
                <asp:CheckBox runat="server" ID="DeleteCheck" Text="Yes, I understand the consequences, delete this report." />
            </div>

            <div class="mt-3">	
                <insite:DeleteButton runat="server" ID="DeleteButton" Enabled="false" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>

        </div>

        <div class="col-md-6">
            <div class="card">
                <div class="card-body">

                    <h3>Consequences</h3>

                    <insite:DeleteWarningAlert runat="server" Name="Report" />

                    <table class="table table-striped table-bordered table-metrics">
                        <tr>
                            <th>Data</th>
                            <th class="text-end" style="width:80px;">Items</th>
                        </tr>
                        <tr>
                            <td>Reports</td>
                            <td class="text-end"><asp:Literal runat="server" ID="ReportsCount" Text="1"/></td>
                        </tr>
                    </table>

                </div>
            </div>
        </div>

    </div>


</asp:Content>