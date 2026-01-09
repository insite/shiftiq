<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Sites.Pages.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/RelationshipList.ascx" TagName="RelationshipList" TagPrefix="uc" %>
<%@ Register Src="../Controls/PagePopupSelector.ascx" TagName="PagePopupSelector" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>
<%@ Register Src="../../Pages/Controls/PageTreeView.ascx" TagName="PageTreeView" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">

            .content-details .content-cmds {
                position: absolute;
                right: 15px;
            }

            .content-details .content-string {
                padding-right: 80px;
                min-height: 50px;
            }

        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="WebPage" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="HierarchySection" Title="Sitemap" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="mb-3">
                            <insite:Button runat="server" ID="ExpandAllButton" ButtonStyle="Default" Icon="fas fa-chevron-down" Text="Expand All" />
                            <insite:ComboBox runat="server" ID="ExpandLevelSelector" Width="100px"  ButtonSize="Small" />
                            <insite:Button runat="server" ID="CollapseAllButton" ButtonStyle="Default" Icon="fas fa-chevron-up" Text="Collapse All" />
                            <insite:Button runat="server" ID="FindCurrentPage" ButtonStyle="OutlineSecondary" Icon="fas fa-location-crosshairs" Text="Go To Current Page" />
                        </div>

                        <uc:PageTreeView runat="server" ID="TreeView" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
              
        <insite:NavItem runat="server" ID="ContentSection" Title="Page Content" Icon="far fa-check-square" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">   
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div style="position:absolute;right:15px;z-index:1;">
                                    <div>
                                        <insite:Button runat="server" ID="PreviewLink" Text="Preview" Icon="fas fa-external-link" ButtonStyle="Default" NavigateTarget="_blank" />
                                    </div>
                                </div>

                                <div class="row content-details">
                                    <div class="col-md-12">
                                        <uc:ContentEditor runat="server" ID="ContentEditor" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="SubpageSection" Title="Manage Subpages" Icon="far fa-cabinet-filing" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:RelationshipList runat="server" ID="SubpageList" />
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PageSection" Title="Page Setup" Icon="far fa-file" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="row button-group mb-3">
                                    <div class="col-lg-12">

                                        <insite:Button runat="server" ID="NewPageLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" />

                                        <insite:ButtonSpacer runat="server" />

                                        <insite:Button runat="server" ID="CopyLink" Text="Duplicate" Icon="fas fa-copy" ButtonStyle="Default" />
                                        <insite:Button runat="server" ID="PublishLink" Text="Publish" Icon="fas fa-upload" ButtonStyle="Default" />
                                        <insite:Button runat="server" ID="UnpublishLink" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Default" />

                                        <insite:ButtonSpacer runat="server" />

                                        <insite:Button runat="server" ID="DownloadLink" Text="Download JSON" Icon="fas fa-download" ButtonStyle="Primary" />
                                        <insite:Button runat="server" ID="ViewHistoryLink" Visible="false" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                                        <insite:DeleteButton runat="server" ID="DeleteLink" />
                                    </div>
                                </div>

                                <div class="row settings">

                                    <div class="col-md-4">

                                        <h3>Identification</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="PageTypeLink" Style="padding: 8px" ToolTip="Change Type" />
                                            </div>
                                            <label class="form-label">
                                                Type
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="PageType" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="TitleLink" Style="padding: 8px" ToolTip="Change Title" />
                                            </div>
                                            <label class="form-label">
                                                Title
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="TitleOutput" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">Status</label>
                                            <div>
                                                <asp:Literal runat="server" ID="IsPublished" />
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Created By
                                            </label>
                                            <div>
                                                <p class="mb-0">
                                                    <asp:Literal runat="server" ID="AuthorName" /></p>
                                                <p>
                                                    <asp:Literal runat="server" ID="AuthorDate" /></p>
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <label class="form-label">
                                                Page Identifier
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="PageIdentifier" />
                                            </div>
                                            <div class="form-text">
                                                A globally unique identifier for this page.
                                            </div>
                                        </div>



                                    </div>

                                    <div class="col-md-4">

                                        <h3>Content</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="IconLink" Style="padding: 8px" ToolTip="Change Icon" />
                                            </div>
                                            <label class="form-label">
                                                Icon
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="Icon" />
                                            </div>
                                            <div class="form-text">
                                                Used to add a FontAwesome icon to the portal tile. 
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="ContentLabelsLink" Style="padding: 8px" ToolTip="Change Content Tags" />
                                            </div>
                                            <label class="form-label">
                                                Visible Tabs
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="ContentLabels" />
                                            </div>
                                            <div class="form-text">
                                                Controls which tabs appear on the Content Editor screen.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="ContentControlnLink" Style="padding: 8px" ToolTip="Change Content Control" />
                                            </div>
                                            <label class="form-label">
                                                Layout (Content Control) 
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="ContentControl" />
                                                <div runat="server" id="CatalogPanel" class="mt-2" visible="false">
                                                    <asp:Literal runat="server" ID="CatalogIdentifier" />
                                                </div>
                                                <div runat="server" id="CoursePanel" class="mt-2" visible="false">
                                                    <asp:Literal runat="server" ID="CourseIdentifier" />
                                                </div>
                                                <div runat="server" id="ProgramPanel" class="mt-2" visible="false">
                                                    <asp:Literal runat="server" ID="ProgramIdentifier" />
                                                </div>
                                                <div runat="server" id="SurveyPanel" class="mt-2" visible="false">
                                                    <asp:Literal runat="server" ID="SurveyIdentifier" />
                                                </div>
                                            </div>
                                            <div class="form-text">
                                                Used to define the behaviour of a portal page that is linked to a specific type of asset, like a course, program or form.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="HookLink" Style="padding: 8px" ToolTip="Change Hook / Integration Code" />
                                            </div>
                                            <label class="form-label">
                                                Hook / Integration Code
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="Hook" />
                                            </div>
                                        </div>

                                    </div>

                                    <div class="col-md-4">

                                        <h3>Structure</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="PageSlugLink" Style="padding: 8px" ToolTip="Change Page Slug" />
                                            </div>
                                            <label class="form-label">
                                                Page Slug (URL Segment)
                                            </label>
                                            <div>
                                                <asp:Literal runat="server" ID="PageSlug" />
                                            </div>
                                            <div class="form-text">
                                                The part of the URL that specifically refers to this page.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="NavigateUrlLink" Style="padding: 8px" ToolTip="Change External Link" />
                                            </div>
                                            <label class="form-label">External Link</label>
                                            <div>
                                                <asp:Literal runat="server" ID="NavigateUrl" />
                                            </div>
                                            <div style="margin-top: 5px;">
                                                <asp:Literal runat="server" ID="IsNavigateUrlToNewTab" />
                                            </div>
                                            <div class="form-text">
                                                Fully-qualified URL for a web page that is external to this web site.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="WebSiteSelectorLink" Style="padding: 8px" ToolTip="Change Web Site" />
                                            </div>
                                            <label class="form-label">
                                                Portal or Web Site
                                            </label>
                                            <div style="margin-top: 5px;">
                                                <asp:Literal runat="server" ID="WebSiteSelector" />
                                            </div>
                                            <div class="form-text">
                                                The web site that contains this page.
                                            </div>
                                        </div>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" ID="ParentPageIdLink" Style="padding: 8px" ToolTip="Change Web Page Container" />
                                            </div>
                                            <label class="form-label">
                                                Parent Page
                                            </label>
                                            <div style="margin-top: 5px;">
                                                <asp:Literal runat="server" ID="ParentPageId" />
                                            </div>
                                            <div class="form-text">
                                                Folders, pages, and sections are organized into a hierarchy.
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PrivacySection" Title="Privacy" Icon="far fa-shield-keyhole" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <h3>Groups</h3>

                                <div class="form-group mb-3">
                                    <div class="float-end">
                                        <insite:IconLink Name="pencil" runat="server" ID="FilterGroupListLink" Style="padding: 8px" ToolTip="Change Privacy" />
                                    </div>
                                    <div>
                                        <asp:Literal runat="server" ID="FilterGroupList" />
                                    </div>
                                </div>

                                <asp:Repeater runat="server" ID="GroupDataRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-striped">
                                            <thead>
                                                <tr>
                                                    <th>Type</th>
                                                    <th>Name</th>
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
                                            <td class="res-type">
                                                <%# Eval("Group.GroupType") %>
                                            </td>
                                            <td class="res-name">
                                                <%# Eval("Group.GroupName") %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>

                                <div runat="server" id="GroupDataRepeaterFooter" class="res-type" visible="false">
                                    None
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>
    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <style type="text/css">
            #<%= HierarchySection.ClientID %> ul.tree-view {
                --in-tree-view-action-selected-bg: #ffffee;
            }
        </style>

        <script type="text/javascript">
            $(function () {
                const tree = document.querySelectorAll('#<%= HierarchySection.ClientID %> .tree-view')[0];

                inSite.common.treeView.init(tree, {
                    expand: '#<%= ExpandAllButton.ClientID %>',
                    collapse: '#<%= CollapseAllButton.ClientID %>',
                    level: '#<%= ExpandLevelSelector.ClientID %>',
                    state: 'admin.site.treeView.<%= Entity.SiteIdentifier.HasValue ? Entity.SiteIdentifier.ToString() : "unknown" %>',
                    defaultLevel: 2
                });

                const findPageButton = document.getElementById('<%= FindCurrentPage.ClientID %>')
                    .addEventListener('click', onFindCurrent);

                function onFindCurrent(e) {
                    e.preventDefault();

                    const selected = tree.querySelector(':scope > li.selected,ul.tree-view > li.selected');

                    let item = selected.parentElement;
                    while (item != null && item != tree) {
                        if (item.tagName == 'LI' && !item.classList.contains('opened'))
                            item.querySelector(':scope > div > div > .toggle-button').click();

                        item = item.parentElement;
                    }

                    selected.scrollIntoView({ behavior: 'smooth', block: 'center'});
                }
            });
        </script>
    </insite:PageFooterContent>
</asp:Content>
