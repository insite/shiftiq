<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Accounts.Organizations.Controls.Detail" %>

<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/TranslationField.ascx" TagName="TranslationField" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailOrganization.ascx" TagName="DetailOrganization" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/DetailConfiguration.ascx" TagName="DetailConfiguration" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Accounts/Organizations/Controls/CollectionManager.ascx" TagName="CollectionManager" TagPrefix="uc" %>
<%@ Register Src="DivisionGrid.ascx" TagName="DivisionGrid" TagPrefix="uc" %>
<%@ Register Src="DepartmentGrid.ascx" TagName="DepartmentGrid" TagPrefix="uc" %>
<%@ Register Src="PortalFieldsGrid.ascx" TagName="PortalFieldsGrid" TagPrefix="uc" %>

<insite:Nav runat="server" ID="NavPanel">

    <insite:NavItem runat="server" ID="OrganizationSection" Title="Organization" Icon="far fa-city" IconPosition="BeforeText">
        <section>
            
            <h2 class="h4 mt-4 mb-3">Organization</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <uc:DetailOrganization runat="server" ID="DetailOrganization" />

                </div>
            </div>

        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="ConfigurationSection" Title="Configuration" Icon="far fa-cogs" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Configuration</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <uc:DetailConfiguration runat="server" ID="DetailConfiguration" />

                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="CollectionSection" Title="Collections" Icon="far fa-album-collection" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Collections</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-6">

                            <uc:CollectionManager runat="server" ID="CollectionManager" />

                        </div>
                    </div>

                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="SubOrganizationsSection" Title="Suborganizations" Icon="far fa-sitemap" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Suborganizations</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <div class="row">
                        <div class="col-md-12">
                            <div class="mb-3 text-end position-relative" style="width:300px;">
                                <a runat="server" id="ClearButton" href="#" class="filter-cmd" style="display:none;" title="Clear"><i class="fas fa-times"></i></a>
                                <insite:TextBox runat="server" ID="SubOrganizationFilterText" EmptyMessage="Filter" />
                            </div>

                            <asp:Repeater runat="server" ID="SubOrganizationRepeater">
                                <HeaderTemplate><ul id="<%# SubOrganizationRepeater.ClientID %>" class="tree-view"></HeaderTemplate>
                                <FooterTemplate></ul></FooterTemplate>
                                <ItemTemplate>
                                    <%# GetTreeViewPrefix(Container.DataItem) %>
                                    <div>
                                        <div>
                                            <a href='<%# Eval("DataItem.OrganizationIdentifier", "/ui/admin/accounts/organizations/edit?organization={0}") %>'><%# Eval("DataItem.OrganizationName") %></a>
                                        </div>
                                    </div>
                                    <%# GetTreeViewPostfix(Container.DataItem) %>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>

                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="DivisionsSection" Title="Divisions" Icon="far fa-industry" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Divisions</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:DivisionGrid runat="server" ID="DivisionGrid" />
                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="DepartmentsSection" Title="Departments" Icon="far fa-building" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Departments</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <uc:DepartmentGrid runat="server" ID="DepartmentGrid" />
                </div>
            </div>
        </section>
    </insite:NavItem>

    <insite:NavItem runat="server" ID="FieldsSection" Title="Fields" Icon="far fa-list-alt" IconPosition="BeforeText">
        <section>
            <h2 class="h4 mt-4 mb-3">Fields</h2>

            <div class="card border-0 shadow-lg">
                <div class="card-body">
                    <insite:Nav runat="server">

                        <insite:NavItem runat="server" Title="Portal Profile">
                            <div class="row">
                                <div class="col-xl-6">
                                    <uc:PortalFieldsGrid runat="server" ID="UserProfileFieldsGrid" />
                                </div>
                            </div>
                        </insite:NavItem>

                        <insite:NavItem runat="server" Title="Class Registration">
                            <div class="row">
                                <div class="col-xl-6">
                                    <uc:PortalFieldsGrid runat="server" ID="ClassRegistrationFieldsGrid" HideMasked="true" HideEditable="false" />
                                </div>
                            </div>
                        </insite:NavItem>

                        <insite:NavItem runat="server" Title="Learner Dashboard">
                            <div class="row">
                                <div class="col-xl-6">
                                    <uc:PortalFieldsGrid runat="server" ID="LearnerDashboardFieldsGrid" HideMasked="true" HideRequired="true" />
                                </div>
                            </div>
                        </insite:NavItem>

                        <insite:NavItem runat="server" Title="Invoice Billing Address">
                            <div class="row">
                                <div class="col-xl-6">
                                    <uc:PortalFieldsGrid runat="server" ID="InvoiceBillingAddressGrid" HideMasked="true" />
                                </div>
                            </div>
                        </insite:NavItem>

                    </insite:Nav>
                </div>
            </div>
        </section>
    </insite:NavItem>

</insite:Nav>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        a.filter-cmd {
            display: block;
            position: absolute;
            line-height: 44px;
            padding: 0 16px;
            right: 0;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const treeId = '#<%= SubOrganizationRepeater.ClientID %>';
            if ($(treeId).length == 0)
                return;

            const $filterInput = $('#<%= SubOrganizationFilterText.ClientID %>').on('keyup', function () {
                if (filterTimeoutHandler)
                    clearTimeout(filterTimeoutHandler);

                filterTimeoutHandler = setTimeout(onFilterTimeout, 500);
            });
            const $filterClear = $('#<%= ClearButton.ClientID %>').on('click', function (e) {
                e.preventDefault();
                e.stopPropagation();

                $filterInput.val('');
                filterTree();
            });
          
            let filterTimeoutHandler = null;

            $(function () {
                filterTree();

                const items = inSite.common.treeView.getAllTreeItems(treeId);
                for (let item of items)
                    item.dataset.text = item.querySelector(':scope > div > div > a').innerText.trim().toUpperCase();
            });            

            function onFilterTimeout() {
                filterTimeoutHandler = null;
                filterTree();
            }

            function filterTree() {
                const searchText = $filterInput.val().trim().toUpperCase();

                const prevText = $filterInput.data('prev');
                if (prevText === searchText)
                    return;

                if (searchText.length > 0) {
                    inSite.common.treeView.filter(treeId, (item) => {
                        const itemText = item.dataset.text;
                        return itemText && itemText.indexOf(searchText) >= 0;
                    });
                    $filterClear.show();
                } else {
                    inSite.common.treeView.filter(treeId);
                    $filterClear.hide();
                }

                $filterInput.data('prev', searchText);
            }
        })();
    
    </script>
</insite:PageFooterContent>