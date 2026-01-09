<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Standards.Dashboard" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-5 mb-md-2" runat="server">
        <h2 class="h4 mb-3">Standards</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <asp:Repeater runat="server" ID="StandardRepeater">
                        <ItemTemplate>
                            <div class="col">
                                <a class="card card-hover card-tile border-0 shadow" href="<%# Eval("Url") %>">
                                    <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Count", "{0:n0}") %></span>
                                    <div class="card-body text-center">
                                        <i class='<%# Eval("Icon") %> fa-3x mb-3'></i>
                                        <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Title") %></h3>
                                    </div>
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        <a class="me-3" href="/ui/admin/standards/upload" runat="server" id="UploadStandardsRow"><i class="fas fa-upload me-1"></i>Upload Standards</a>
                        <a class="me-3" href="/ui/admin/standards/troubleshoot" runat="server" id="TroubleshootStanradrs"><i class="fas fa-screwdriver-wrench me-1"></i>Troubleshoot Standards</a>
                    </div>
                </div>

            </div>
        </div>
    </section>
    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Collections</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body"> 
                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <div class="col">
                        <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/standards/collections/search">
                            <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="CollectionCount" /></span>
                            <div class="card-body text-center">
                                <i class='far fa-box-open fa-3x mb-3'></i>
                                <h3 class='h5 nav-heading mb-2 text-break'>Collections</h3>
                            </div>
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </section>
    <section class="pb-5 mb-md-2">
        <h2 class="h4 mb-3">Documents</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-body"> 
                <div class="row row-cols-1 row-cols-lg-4 g-4">
                    <asp:Repeater runat="server" ID="DocumentCounterRepeater">
		                <ItemTemplate>
                            <div class="col">
			                    <a class="card card-hover card-tile border-0 shadow" href="<%# "/ui/admin/standards/documents/search?type=" + HttpUtility.UrlEncode((string)Eval("Name")) %>">
				                    <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Count", "{0:n0}") %></span>
				                    <div class="card-body text-center">
					                    <i class='far fa-file-alt fa-3x mb-3'></i>
					                    <h3 class='h5 nav-heading mb-2 text-break'><%# GetDocumentTypeName((string)Eval("Name")) %></h3>
				                    </div>
			                    </a>
		                    </div>
		                </ItemTemplate>
	                </asp:Repeater>

                    <div class="col" runat="server" id="NoDocuments">
			            <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/standards/documents/search">
				            <span class="badge badge-floating badge-pill bg-primary">0</span>
				            <div class="card-body text-center">
					            <i class='far fa-file-alt fa-3x mb-3'></i>
					            <h3 class='h5 nav-heading mb-2 text-break'>Documents</h3>
				            </div>
			            </a>
		            </div>
                </div>
                <div class="row mt-4">
                <div class="col-lg-12">
                    <a class="me-3" href="/ui/admin/standards/documents/analysis"><i class="fas fa-file-search me-1"></i>Documents Analysis</a>
                </div>
            </div>
            </div>

            
        </div>
    </section>
</asp:Content>
