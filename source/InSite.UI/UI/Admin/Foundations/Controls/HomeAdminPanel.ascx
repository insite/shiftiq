<%@ Control Language="C#" CodeBehind="HomeAdminPanel.ascx.cs" Inherits="InSite.UI.Layout.Admin.HomeAdminPanel" %>

<section class="pb-4 mb-md-2">

    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">

        <asp:Repeater runat="server" ID="ToolkitRepeater">
            <ItemTemplate>

                <div class="col">
                    <a class="card card-hover card-tile border-1 shadow" href='<%# Eval("Url") %>'>
                        <div class="card-body text-center">
                            <asp:Label runat="server" ID="CardIcon" />
                            <asp:Literal runat="server" ID="CardTitle" />
                        </div>
                    </a>
                </div>
        
            </ItemTemplate>
        </asp:Repeater>
            
    </div>

</section>

<section runat="server" id="ShortcutPanel" class="pb-4 mb-md-2">

    <h2 class="h4 mb-3"><%= Organization.CompanyName %></h2>

    <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-5 g-4">

        <asp:Repeater runat="server" ID="ShortcutRepeater">
            <ItemTemplate>

                <div class="col">
                    <a class="card card-hover card-tile border-1 shadow" href='<%# Eval("Url") %>'>
                        <div class="card-body text-center text-danger">
                            <asp:Label runat="server" ID="CardIcon" />
                            <asp:Literal runat="server" ID="CardTitle" />
                        </div>
                    </a>
                </div>
            
            </ItemTemplate>
        </asp:Repeater>

    </div>

</section>