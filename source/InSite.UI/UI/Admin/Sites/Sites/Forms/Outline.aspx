<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Sites.Sites.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../../Pages/Controls/RelationshipList.ascx" TagName="RelationshipList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/ContentEditor.ascx" TagName="ContentEditor" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Assets/Contents/Controls/MultilingualStringInfo.ascx" TagName="MultilingualStringInfo" TagPrefix="uc" %>
<%@ Register Src="../../Pages/Controls/PageTreeView.ascx" TagName="PageTreeView" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">

            .content-details {

            }

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
    <insite:ValidationSummary runat="server" ValidationGroup="WebSite" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="HierarchySection" Title="Sitemap" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div class="mb-3">
                            <insite:Button runat="server" ID="ExpandAllButton" ButtonStyle="Default" Icon="fas fa-chevron-down" Text="Expand All" />
                            <insite:ComboBox runat="server" ID="ExpandLevelSelector" Width="100px"  ButtonSize="Small" />
                            <insite:Button runat="server" ID="CollapseAllButton" ButtonStyle="Default" Icon="fas fa-chevron-up" Text="Collapse All" />
                        </div>

                        <uc:PageTreeView runat="server" ID="TreeView" />

                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="GeneralSection" Title="Site Setup" Icon="far fa-cloud" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">        
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div class="row button-group mb-3">
                                    <div class="col-lg-12">
                                        <insite:Button runat="server" ID="NewSiteLink" Text="New" Icon="fas fa-file" ButtonStyle="Default" NavigateUrl="/ui/admin/sites/create" />

                                        <insite:ButtonSpacer runat="server" />
                                        <insite:Button runat="server" ID="DuplicateButton" Text="Duplicate" ButtonStyle="Default" Icon="fas fa-copy" CausesValidation="false" Visible="true" />
                                        <insite:Button runat="server" ID="DownloadLink" Text="Download JSON" icon="fas fa-download" ButtonStyle="Primary" />
                                        <insite:Button runat="server" ID="ViewHistoryLink" Visible="false" Text="History" Icon="fas fa-history" ButtonStyle="Default" />
                                        <insite:DeleteButton runat="server" ID="DeleteLink" />
                                    </div>
                                </div>
                                <div class="row settings">
                                    <div class="col-md-6">
                                        <h3>Identification</h3>

                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="SiteTitleLink" style="padding:8px" ToolTip="Change Title" />
                                            </div>
                                            <asp:Label runat="server" ID="SiteTitleLabel" AssociatedControlID="SiteTitle" Text="Title" CssClass="form-label" />
                                            <div>
                                                <asp:Literal runat="server" ID="SiteTitle" />                               
                                            </div>
                                            <div class="form-text">
                                                A descriptive title that uniquely identifies this web site.
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <div class="float-end">
                                                <insite:IconLink Name="pencil" runat="server" id="SiteDomainLink" style="padding:8px" ToolTip="Change Domain" />
                                            </div>
                                            <asp:Label runat="server" ID="SiteDomainLabel" AssociatedControlID="SiteDomain" Text="Domain" CssClass="form-label" />
                                            <div>
                                                <asp:Literal runat="server" ID="SiteDomain" />                                
                                            </div>
                                        </div>
                                        <div class="form-group mb-3">
                                            <asp:Label runat="server" ID="SiteIdentifierLabel" AssociatedControlID="SiteIdentifier" Text="Site Identifier" CssClass="form-label" />
                                            <div>
                                                <asp:Literal runat="server" ID="SiteIdentifier" />
                                            </div>
                                            <div class="form-text">
                                                A globally unique identifier for this site.
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

        <insite:NavItem runat="server" ID="ContentSection" Title="Site Content" Icon="far fa-edit" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <div style="display:none"><div style="position:absolute; z-index:1; right:15px;">
                                    <insite:Button runat="server" ID="ViewSiteButton" ToolTip="View Web Site" ButtonStyle="Default" Icon="fas fa-external-link" Text="View Web Site" />
                                </div>
            
                                <uc:ContentEditor runat="server" ID="ContentEditor" ValidationGroup="WebSite" /></div>

                                <div style="position:absolute;right:15px;z-index:1;">
                                    <div>
                                        <insite:Button runat="server" ID="PreviewLink" Text="Preview" Icon="fas fa-external-link" ButtonStyle="Default" NavigateTarget="_blank" />
                                    </div>
                                </div>

                                <div class="row content-details">
                                    <div class="col-md-12">
                                        <insite:Nav runat="server" ID="ContentNavigation">
                                            <insite:NavItem runat="server" ID="TitleTab" Title="Title">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentTitleLink" ToolTip="Revise Title" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentTitle" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="SummaryTab" Title="Summary">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentSummaryLink" ToolTip="Revise Summary" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentSummary" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="ShortcutsTab" Title="Shortcuts">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentShortcutsLink" ToolTip="Revise Shortcuts" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentShortcuts" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="CopyrightTab" Title="Copyright">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentCopyrightlsLink" ToolTip="Revise Copyright" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <uc:MultilingualStringInfo runat="server" ID="ContentCopyright" />
                                                </div>
                                            </insite:NavItem>

                                            <insite:NavItem runat="server" ID="HtmlHeadTab" Title="HtmHead">
                                                <div class="content-cmds">
                                                    <insite:Button runat="server" id="EditContentHtmlHeadLink" ToolTip="Revise Head" ButtonStyle="Default" Text="Edit" Icon="far fa-pencil" />
                                                </div>
                                                <div class="content-string">
                                                    <asp:Literal runat="server" ID="ContentHead" />
                                                </div>
                                            </insite:NavItem>
                                        </insite:Nav>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

        <insite:NavItem runat="server" ID="PageSection" Title="Site Content > Pages" Icon="far fa-file" IconPosition="BeforeText">
            <section class="pb-5 mb-md-2">
                <h2 class="h4 mb-1"></h2>
                <div class="row">
                    <div class="col-lg-12">
                        <div class="card border-0 shadow-lg">
                            <div class="card-body">
                                <uc:RelationshipList runat="server" ID="PageList" />
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
                    state: 'admin.site.treeView.<%= Entity.SiteIdentifier %>',
                    defaultLevel: 2
                });
            });
        </script>

    </insite:PageFooterContent>
</asp:Content>
