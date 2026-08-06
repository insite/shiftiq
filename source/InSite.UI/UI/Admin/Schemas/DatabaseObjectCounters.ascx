<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DatabaseObjectCounters.ascx.cs" Inherits="InSite.UI.Admin.Databases.DatabaseObjectCounters" %>

<section class="pb-5 mb-md-2">
    <h2 class="h4 mb-3">Database Objects</h2>
    <div class="card border-0 shadow-lg">
        <div class="card-body">
            <div class="row row-cols-1 row-cols-lg-2 g-4">
                <div class="col">
                    <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/schemas/tables/search">
                        <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="TableCount" Text="0" /></span>
                        <div class="card-body text-center">
                            <i class='far fa-table fa-3x mb-3'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>Tables</h3>
                        </div>
                    </a>
                </div>
                <div class="col">
                    <a class="card card-hover card-tile border-0 shadow" href="/ui/admin/schemas/columns/search">
                        <span class="badge badge-floating badge-pill bg-primary"><asp:Literal runat="server" ID="ColumnCount" Text="0" /></span>
                        <div class="card-body text-center">
                            <i class='far fa-columns fa-3x mb-3'></i>
                            <h3 class='h5 nav-heading mb-2 text-break'>Columns</h3>
                        </div>
                    </a>
                </div>
            </div>
        </div>
    </div>
</section>
