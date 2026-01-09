<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PerformanceReportOptions.ascx.cs" Inherits="InSite.UI.Admin.Assessments.Attempts.Controls.PerformanceReportOptions" %>

<h3>Report Options</h3>

<asp:CustomValidator runat="server" ID="ReportValidator" ErrorMessage="Roles to include are required" Display="None" ValidationGroup="Report" />

<div class="form-group mb-3">
    <label class="form-label">
        Select which Roles to include
    </label>
    <div>
        <asp:Repeater runat="server" ID="ReportRepeater">
            <ItemTemplate>
                <insite:RadioButton runat="server"
                    ID="Selected"
                    Text='<%# Container.DataItem %>'
                    Value="<%# Container.ItemIndex %>"
                    Checked="<%# Container.ItemIndex == 0 %>"
                    GroupName="Report"
                />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>

<a runat="server" id="SearchButton" href="#" title="Modify Reports">
    <i class="far fa-magnifying-glass"></i>
</a>