<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="InSite.UI.Admin.Database.Entities.Search" %>

<%@ Register TagPrefix="uc" TagName="SearchCriteria" Src="./SearchCriteria.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchResults" Src="./SearchResults.ascx" %>
<%@ Register TagPrefix="uc" TagName="SearchDownload" Src="~/UI/Layout/Common/Controls/SearchDownload.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style>
        table.subcomponent > tbody > tr > th {
            color: white !important;
            background-color: #6a9bf4;
        }
        table.entities tr td { padding: 10px; }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="SearchResultsTab" Icon="fas fa-database" Title="Results">
            <uc:SearchResults runat="server" ID="SearchResults" />

            <div class="row">
                <div class="col-4">
                    <h5>Components</h5>
                    <asp:Repeater runat="server" ID="ComponentSummary">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Type</th>
                                    <th>Component</th>
                                    <th class="text-end">Entities</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ComponentType") %></td>
                                <td><%# Eval("ComponentName") %></td>
                                <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="col-4">
                    <h5>Subcomponents</h5>
                    <asp:Repeater runat="server" ID="SubcomponentSummary">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Type</th>
                                    <th>Component</th>
                                    <th>Subcomponent</th>
                                    <th class="text-end">Entities</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("ComponentType") %></td>
                                <td><%# Eval("ComponentName") %></td>
                                <td><%# Eval("ComponentPart") %></td>
                                <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>

            </div>

        </insite:NavItem>

        <insite:NavItem runat="server" ID="SearchCriteriaTab" Icon="fas fa-filter" Title="Criteria">
            <uc:SearchCriteria runat="server" ID="SearchCriteria" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="DownloadsTab" Icon="fas fa-download" Title="Downloads">
            <uc:SearchDownload runat="server" ID="SearchDownload" />
        </insite:NavItem>

        <insite:NavItem runat="server" ID="ComponentsTab" Icon="fas fa-boxes-stacked" Title="Component Map">
            
            <asp:Literal runat="server" ID="EntityHierarchy" />

        </insite:NavItem>

        <insite:NavItem runat="server" ID="ProblemsTab" Icon="fas fa-skull-crossbones" Title="Problems">

            <div class="alert alert-success" runat="server" id="NoProblemPanel">
                <strong>Excellent!</strong> You have no problems.
            </div>

            <div class="row">

                <div class="col-4 mb-4 me-4" runat="server" id="DuplicateEntityPanel">

                    <h5>Duplicate Entities</h5>
                    <asp:Repeater runat="server" ID="DuplicateEntityRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Entity</th>
                                    <th class="text-end">Duplicates</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-danger"><%# Eval("Entity") %></td>
                                <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <div class="col-4 mb-4 me-4" runat="server" id="DuplicateCollectionPanel">

                    <h5>Duplicate Collections</h5>
                    <asp:Repeater runat="server" ID="DuplicateCollectionRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Collection</th>
                                    <th class="text-end">Duplicates</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td class="text-danger"><%# Eval("Collection") %></td>
                                <td class="text-end"><%# Eval("Count", "{0:n0}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <div class="col-4 mb-4 me-4" runat="server" id="UnexpectedEntityPanel">

                    <h5>Unexpected Entity Names</h5>
                    <asp:Repeater runat="server" ID="UnexpectedEntityRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Table</th>
                                    <th>Actual Entity</th>
                                    <th>Expected Entity</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("StorageTable") %></td>
                                <td class="text-danger"><%# Eval("ActualEntityName") %></td>
                                <td class="text-success"><%# Eval("ExpectedEntityName") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <div class="col-8 mb-4 me-4" runat="server" id="UnexpectedCollectionPanel">

                    <h5>Unexpected Collection Slugs</h5>

                    <asp:Repeater runat="server" ID="UnexpectedCollectionRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Entity</th>
                                    <th>Actual</th>
                                    <th>Expected</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("EntityName") %></td>
                                <td class="text-danger"><%# Eval("ActualCollectionSlug") %></td>
                                <td class="text-success"><%# Eval("ExpectedCollectionSlug") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <div class="col-12 mb-4" runat="server" id="OrphanEntityPanel">

                    <h5>Orphan Entities <span class="text-muted fs-sm">(entity definitions with no database storage object)</span></h5>

                    <asp:Repeater runat="server" ID="OrphanEntityRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Structure</th>
                                    <th>Schema</th>
                                    <th>Table</th>
                                    <th>Component Type</th>
                                    <th>Component Name</th>
                                    <th>Component Feature</th>
                                    <th>Entity</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("StorageStructure") %></td>
                                <td><%# Eval("StorageSchema") %></td>
                                <td><%# Eval("StorageTable") %></td>
                                <td><%# Eval("ComponentType") %></td>
                                <td><%# Eval("ComponentName") %></td>
                                <td><%# Eval("ComponentPart") %></td>
                                <td><%# Eval("EntityName") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

                <div class="col-12 mb-4" runat="server" id="OrphanTablePanel">

                    <h5>Orphan Tables <span class="text-muted fs-sm">(database storage objects with no entity definition)</span></h5>

                    <asp:Repeater runat="server" ID="OrphanTableRepeater">
                        <HeaderTemplate>
                            <table class="table table-striped">
                                <tr>
                                    <th>Structure</th>
                                    <th>Schema</th>
                                    <th>Table</th>
                                    <th class="text-end">Columns</th>
                                    <th class="text-end">Rows</th>
                                    <th>Created</th>
                                </tr>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td>Table</td>
                                <td><%# Eval("SchemaName") %></td>
                                <td><%# Eval("TableName") %></td>
                                <td class="text-end"><%# Eval("ColumnCount", "{0:n0}") %></td>
                                <td class="text-end"><%# Eval("RowCount", "{0:n0}") %></td>
                                <td><%# Eval("Created", "{0:MMM d, yyyy}") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </table>
                        </FooterTemplate>
                    </asp:Repeater>

                </div>

            </div>

        </insite:NavItem>
    </insite:Nav>

</asp:Content>
