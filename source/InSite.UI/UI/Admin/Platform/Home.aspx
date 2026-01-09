<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Settings.Dashboard" %>
<%@ Register Src="../Database/DatabaseObjectCounters.ascx" TagName="DatabaseObjectCounters" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:Alert runat="server" ID="HomeStatus" />

    <section class="pb-5 mb-md-2" runat="server">
                        
        <h2 class="h4 mb-3">System Configuration</h2>
        
        <div class="card border-0 shadow-lg">
            <div class="card-body">                
                <div class="row row-cols-1 row-cols-lg-4 g-4">                                     

                    <div runat="server" id="ActionCounter" class="col">
                        <a runat="server" id="ActionLink" class="card card-hover card-tile border-0 shadow" href="#">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ActionCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-location fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Actions</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="CollectionCounter" class="col">
                        <a runat="server" id="CollectionLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="CollectionCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-album-collection fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Collections</h3>
                            </div>
                        </a>
                    </div>

                </div>
            </div>
        </div>

    </section>

    <uc:DatabaseObjectCounters runat="server" />

    <section class="pb-5 mb-md-2">

        <div class="row">
            <div class="col-lg-6 mb-4 mb-lg-0">

                <h2 class="h4 mb-3">Maintenance Windows</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="alert alert-info mb-4">
                            <asp:Literal runat="server" ID="MaintenanceUptime" />
                        </div>

                        <asp:Repeater runat="server" ID="MaintenanceIntervalRepeater">
                            <ItemTemplate>
                                
                                <div class="card mb-4"><div class="card-body">

                                    <h3 class="mb-0" title='<%# Eval("Slug") %>'><%# Eval("Name") %></h3>
                                
                                    <div class="fs-sm text-muted p-2"><%# Eval("Description") %></div>
                                    
                                    <dl class="row">

                                        <dt class="col-sm-3">Start Date (First Interval)</dt>
                                        <dd class="col-sm-9"><%# Eval("Interval.Date") %></dd>

                                        <dt class="col-sm-3">Start Time</dt>
                                        <dd class="col-sm-9"><%# Eval("Interval.Time") %> <%# Eval("Interval.Zone") %></dd>
                                        
                                        <dt class="col-sm-3">Expected Duration</dt>
                                        <dd class="col-sm-9"><%# Eval("Interval.Length") %></dd>

                                        <dt class="col-sm-3">Recurrence</dt>
                                        <dd class="col-sm-9"><%# DescribeRecurrence(Container.DataItem) %></dd>

                                        <dt class="col-sm-3">Filter</dt>
                                        <dd class="col-sm-9"><%# DescribeFilter(Container.DataItem) %></dd>

                                    </dl>

                                </div></div>

                            </ItemTemplate>
                        </asp:Repeater>
                        
                    </div>
                </div>
                
            </div>
            <div class="col-lg-6">

                <h2 class="h4 mb-3">Settings</h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="card mb-4">
                            <div class="card-body">

                                <h3>App Settings</h3>

                                <asp:Repeater runat="server" ID="ConfigurationProviderRepeater">
                                    <ItemTemplate>
                                        <div>
                                            <%# Container.DataItem %>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>
                        </div>

                        <div class="card mb-4">
                            <div class="card-body">

                                <h3>Large Database Command Monitor</h3>

                                <insite:UpdatePanel runat="server" CssClass="row">
                                    <ContentTemplate>
                                        <div class="col">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Monitor Status
                                                </label>
                                                <div>
                                                    <insite:Toggle runat="server" ID="LdcMonitorEnabled" Size="Small" AutoPostBack="true"
                                                        OnClientChange="if (!ldcMonitor.confirmStatusChange(this, event)) return false;"
                                                        TextOn="Enabled" TextOff="Disabled"
                                                        StyleOn="Success" StyleOff="Warning" />
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Reporting Status
                                                </label>
                                                <div runat="server" id="LdcReportStatus">
                                                
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Database Command Size
                                                </label>
                                                <div runat="server" id="LdcMonitorDatabaseCommandSize">
                                                </div>
                                            </div>

                                        </div>
                                        <div class="col">

                                            <div class="form-group mb-3">
                                                <label class="form-label">
                                                    Stack Trace
                                                </label>
                                                <div>
                                                    <insite:Toggle runat="server" ID="LdcMonitorIncludeStackTrace" Size="Small" AutoPostBack="true" TextOn="Include" TextOff="Exclude" />
                                                </div>
                                            </div>

                                        </div>
                                    </ContentTemplate>
                                </insite:UpdatePanel>

                            </div>
                        </div>

                    </div>
                </div>

            </div>
        </div>
    </section>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                if (window.ldcMonitor)
                    return;

                const instance = window.ldcMonitor = {};

                instance.confirmStatusChange = function (element) {
                    if (confirm('Are you sure you want to change the status of a large database command monitor?'))
                        return true;

                    var toggle = $(element).data('bs.toggle');
                    if (element.checked)
                        toggle.off(true);
                    else
                        toggle.on(true);

                    return false;
                };
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>