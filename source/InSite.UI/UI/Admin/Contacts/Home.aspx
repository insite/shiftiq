<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Contacts.Dashboard" %>

<%@ Register Src="./People/Controls/RecentList.ascx" TagName="PersonRecentList" TagPrefix="uc" %>

<%@ Import Namespace="InSite.Common.Web.Infrastructure" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Contact People</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">

                    <div runat="server" id="PeopleCounter" class="col">
                        <a runat="server" id="PeopleLink" class="card card-hover card-tile border-0 shadow" href='#'>
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="PeopleCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-user fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>People</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="MembershipCounter" class="col">
                        <a runat="server" id="MembershipLink" class="card card-hover card-tile border-0 shadow">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="MembershipCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-id-card-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Group Memberships</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="UsersConnectionsCounter" class="col">
                        <a runat="server" id="UsersConnectionsLink" class="card card-hover card-tile border-0 shadow">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="UsersConnectionsCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-handshake-alt fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Connections</h3>
                            </div>
                        </a>
                    </div>

                    <div runat="server" id="MembershipReasonCounter" class="col">
                        <a runat="server" id="MembershipReasonLink" class="card card-hover card-tile border-0 shadow">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="MembershipReasonCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-question fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Membership Reasons</h3>
                            </div>
                        </a>
                    </div>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/contacts/people/upload"><i class="fas fa-upload me-2"></i>Upload Contacts</a>
                        <a runat="server" id="DownloadXLSXSection" class="me-3" href="/ui/admin/contacts/reports/tax-form-t2202"><i class="fas fa-download me-2"></i>Canada Revenue Agency (CRA) Tax Form T2202</a>
                        <a class="me-3" href="/ui/admin/contact/persons/anomalies"><i class="fas fa-triangle-exclamation me-2"></i>Data Anomalies</a>
                    </div>
                </div>

                
            </div>
        </div>
    
    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Contact Groups</h2>

        <div class="card border-0 shadow-lg">

            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                    <li class="nav-item"><a class="nav-link active" href="#v1a" data-bs-toggle="tab" role="tab" aria-controls="v1a" aria-selected="true">Groups by Tag</a></li>
                    <li class="nav-item"><a class="nav-link" href="#v1b" data-bs-toggle="tab" role="tab" aria-controls="v1b" aria-selected="true">Groups by Type</a></li>
                </ul>
            </div>

            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="v1a" role="tabpanel">

                        <div class="row row-cols-1 row-cols-lg-4 g-4">

                            <div runat="server" id="GroupCounter" class="col">
                                <a runat="server" id="GroupLink" class="card card-hover card-tile border-0 shadow" href='#'>
                                    <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="GroupCount" /></span>
                                    <div class="card-body text-center">
                                        <i class='far fa-users fa-3x mb-3'></i>
                                        <h3 class='h5 nav-heading mb-2 text-break'>Groups</h3>
                                    </div>
                                </a>
                            </div>

                            <asp:Repeater runat="server" ID="GroupLabelRepeater">
			                    <ItemTemplate>
				                    <div class="col">
					                    <a class="card card-hover card-tile border-0 shadow" href="<%# GetGroupLabelUrl(Container.DataItem) %>">
						                    <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Count","{0:n0}") %></span>
						                    <div class="card-body text-center">
							                    <i class='far <%# (Eval("Icon") as string) ?? "fa-cube" %> fa-3x mb-3'></i>
							                    <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
						                    </div>
					                    </a>
				                    </div>
			                    </ItemTemplate>
		                    </asp:Repeater>

                        </div>

                        <div class="row mt-4">
		                    <div class="col-lg-12">
			                    <a class="me-3" href="/ui/admin/contacts/groups/manage"><i class="fas fa-sitemap me-2"></i>Manage Groups</a>
			                    <a class="me-3" href="/ui/admin/contacts/reports/employers"><i class="fas fa-chart-bar me-2"></i>Employers Report</a>
		                    </div>
	                    </div>

                    </div>
                    <div class="tab-pane fade show" id="v1b" role="tabpanel">

                        <div class="row row-cols-1 row-cols-lg-4 g-4">
                            <asp:Repeater runat="server" ID="GroupTypeRepeater">
			                    <ItemTemplate>
				                    <div class="col">
					                    <a class="card card-hover card-tile border-0 shadow" href="<%# GetGroupTypeUrl(Container.DataItem) %>">
						                    <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Count","{0:n0}") %></span>
						                    <div class="card-body text-center">
							                    <i class='far <%# (Eval("Icon") as string) ?? "fa-cube" %> fa-3x mb-3'></i>
							                    <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
						                    </div>
					                    </a>
				                    </div>
			                    </ItemTemplate>
		                    </asp:Repeater>
                        </div>

                    </div>
                </div>
            </div>

        </div>

    </section>
    
    <section runat="server" id="HistoryPanel" class="pb-4 mb-md-2">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                  <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">People</a></li>
                </ul>
            </div>
            <div class="card-body">
                <div class="tab-content">
                    <div class="tab-pane fade show active" id="result1" role="tabpanel">
                        <uc:PersonRecentList runat="server" ID="PersonRecentList" />
                    </div>
                </div>
                <div class="tab-content">
                    <div class="tab-pane fade show active" runat="server" id="ArchivePanel" role="tabpanel">
                        <asp:Repeater runat="server" ID="ArchiveRepeater">
                            <HeaderTemplate>
                                <table class="table table-striped">
                                    <tbody>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td>
                                        <a href="<%# FileHelper.GetUrl((string)Eval("Path")) %>" target="_blank"><%# Eval("Name") %></a>
                                    </td>
                                    <td style="width: 25px">
                                        <insite:IconButton runat="server" ID="DeleteArchive"
                                            CommandName="Delete"
                                            Name="trash-alt"
                                            ConfirmText="Are you sure you want to permanently delete this archive?" CommandArgument='<%# Eval("Path") %>' />
                                    </td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>                   
                            </FooterTemplate>
                        </asp:Repeater>                    
                    </div>
                </div>
            </div>
        </div>
    </section>

</asp:Content>
