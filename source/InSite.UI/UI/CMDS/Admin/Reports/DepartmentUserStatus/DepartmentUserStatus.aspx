<%@ Page Language="C#" CodeBehind="DepartmentUserStatus.aspx.cs" Inherits="InSite.Custom.CMDS.Admin.Reports.DepartmentUserStatus" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/DepartmentUserStatusCriteria.ascx" TagName="SearchCriteria" TagPrefix="uc" %>
<%@ Register Src="Controls/DepartmentUserStatusResults.ascx" TagName="SearchResults" TagPrefix="uc" %>
<%@ Register Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" TagName="SearchDownload" TagPrefix="uc" %>

<%@ Register Src="Controls/DepartmentRepeater.ascx" TagName="DepartmentRepeater" TagPrefix="uc" %>
<%@ Register Src="Controls/ZoomAchievementGrid.ascx" TagName="ZoomAchievementGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/ZoomStandardGrid.ascx" TagName="ZoomStandardGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    
    <insite:Alert runat="server" ID="ScreenStatus" />
    
    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ResultsUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="ResultsUpdatePanel">
                <ContentTemplate>
                    <insite:Nav runat="server">

                        <insite:NavItem runat="server" ID="DepartmentsPanel" Title="Departments">
                            <uc:DepartmentRepeater runat="server" ID="DepartmentRepeater" />
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="UsersPanel" Title="Users">
                            <uc:SearchResults runat="server" ID="SearchResults" />
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="ZoomPanel" Title="Zoom (Current)" Visible="false">
                            <div class="row my-3">
                                <div class="col-md-2">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            As At
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="ZoomAsAt" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Department
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="ZoomDepartmentName" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            User
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="ZoomUserName" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <div class="form-group mb-3">
                                        <label class="form-label">
                                            Statistic
                                        </label>
                                        <div>
                                            <asp:Literal runat="server" ID="ZoomStatisticName" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <uc:ZoomAchievementGrid runat="server" ID="ZoomAchievementGrid" />
                            <uc:ZoomStandardGrid runat="server" ID="ZoomStandardGrid" />
                        </insite:NavItem>

                        <insite:NavItem runat="server" ID="ChartsPanel" Title="Charts">
                            <chart:LineChart runat="server" ID="Chart" DatasetType="DateTime" ToolTipIntersect="false" OnClientPreInit="departmentUserStatus.onChartPreInit" />
                        </insite:NavItem>

                    </insite:Nav>
                </ContentTemplate>
            </insite:UpdatePanel>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SnapshotTab" Icon="fas fa-camera" Title="Snapshots">
            <insite:Alert runat="server" ID="SnapshotAlert" />

            <div class="row">
                <div class="col-md-6">
                    <insite:Button runat="server" ID="SnapshotButton" ButtonStyle="Success" Icon="fas fa-camera" Text="Take Snapshot" />
                    <p class="form-text mt-3">
                        Click this button to recalculate the compliance statistics for the active organization account.
                    </p>
                </div>
                <div class="col-md-6">
                    <asp:Repeater runat="server" ID="SnapshotRepeater">
                        <HeaderTemplate>
                            <table class="table">
                                <thead>
                                    <tr>
                                        <th>As At</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                                </tbody>
                            </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# LocalizeDate((DateTimeOffset)Eval("AsAt")) %></td>
                                <td class="text-nowrap text-end">
                                    <insite:IconButton runat="server" Name="trash-alt" CommandName="Delete" OnClientClick='return confirm("Are you sure you want to remove this snapshot?");'
                                        style="padding:8px" ToolTip="Delete Snapshot" />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>
    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var departmentUserStatus = window.departmentUserStatus = window.departmentUserStatus || {
                    onChartPreInit: function (canvas, config) {
                        if (config.data.datasets.length === 0)
                            return;

                        var dataset = config.data.datasets[0];
                        var months = moment(dataset.data[dataset.data.length - 1].x).diff(moment(dataset.data[0].x), 'months', true);

                        inSite.common.setObjProp(config, 'options.scales.y', {
                            display: true,
                            beginAtZero: true,
                            max: 1,
                            ticks: {
                                callback: function (value, index, values) {
                                    return String(value * 100) + '%';
                                }
                            }
                        });

                        inSite.common.setObjProp(config, 'options.scales.x', {
                            type: "time",
                            time: {
                                unit: months > 6 ? 'month' : 'day',
                                minUnit: 'day',
                                tooltipFormat: 'MMM D, YYYY - h:mm A'
                            },
                        });

                        inSite.common.setObjProp(config, 'options.plugins.tooltip.callbacks.label', function (item, data) {
                            return item.dataset.label + ': ' + (item.raw.y * 100).toFixed(1) + '%';
                        });
                    },
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
