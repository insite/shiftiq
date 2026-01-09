<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="InSite.UI.Admin.Sites.Home" %>

<%@ Register Src="./Sites/Controls/RecentList.ascx" TagName="RecentList" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Web Sites</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                        
                    <asp:Repeater runat="server" ID="SiteRepeater">
			            <ItemTemplate>
				            <div class="col">
					            <a class="card card-hover card-tile border-0 shadow" href="<%# Eval("Url") %>">
						            <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Value", "{0:n0}") %></span>
						            <div class="card-body text-center">
							            <i class='far <%# Eval("Icon") %> fa-3x mb-3'></i>
							            <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
						            </div>
					            </a>
				            </div>
			            </ItemTemplate>
		            </asp:Repeater>

                </div>

                <div class="row mt-4">
                    <div class="col-lg-12">
                        
                    </div>
                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2">

        <h2 class="h4 mb-3">Web Pages</h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">

                <div class="row row-cols-1 row-cols-lg-4 g-4">      
                    
                    <asp:Repeater runat="server" ID="PageRepeater">
			            <ItemTemplate>
				            <div class="col">
					            <a class="card card-hover card-tile border-0 shadow" href="<%# Eval("Url") %>">
						            <span class="badge badge-floating badge-pill bg-primary"><%# Eval("Value", "{0:n0}") %></span>
						            <div class="card-body text-center">
							            <i class='far <%# Eval("Icon") %> fa-3x mb-3'></i>
							            <h3 class='h5 nav-heading mb-2 text-break'><%# Eval("Name") %></h3>
						            </div>
					            </a>
				            </div>
			            </ItemTemplate>
		            </asp:Repeater>

                </div>

            </div>                
        </div>

    </section>

    <section class="pb-4 mb-md-2" runat="server" ID="HistoryPanel">
        <h2 class="h4 mb-3">Recent Changes</h2>
        <div class="card border-0 shadow-lg">
            <div class="card-header">
                <ul class="nav nav-tabs card-header-tabs" role="tablist">
                  <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Pages</a></li>
                </ul>
            </div>
            <div class="card-body">
                <uc:RecentList runat="server" ID="RecentPages" />
            </div>
        </div>
    </section>

</asp:Content>
