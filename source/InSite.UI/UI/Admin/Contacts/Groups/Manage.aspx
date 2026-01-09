<%@ Page Language="C#" CodeBehind="Manage.aspx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Manage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .bootstrap-select > div.dropdown-menu > ul > li > a > span.text {
            color: #333;
        }

        .bootstrap-select > div.dropdown-menu > ul > li.selected {
            background: #eee;
        }

        .tree-view-container div.node-labels {
            padding-left: 0.5rem;
            display: inline-block;
        }

        .tree-view-container .commands {
            position: absolute;
            right: 0.625rem;
            top: 0.4375rem;
        }

        .tree-view-container ul.tree-view li {
            margin-bottom: 0
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />

    <insite:Nav runat="server">
        <insite:NavItem runat="server" ID="OutlineTab" Icon="fas fa-sitemap" Title="Outline">

            <div class="pb-3">
                <button id="expand-all" type="button" class="btn btn-outline-secondary"><i class="fas fa-chevron-down me-1"></i> Expand All</button>
                <select id="expand-level" class="insite-combobox form-select"></select>
                <button id="collapse-all" type="button" class="btn btn-outline-secondary"><i class="fas fa-chevron-up me-1"></i> Collapse All</button>
            </div>

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="TreeViewUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="TreeViewUpdatePanel" ClientEvents-OnResponseEnd="initTreeView">
                <ContentTemplate>
                    <div class="tree-view-container">
                        <asp:Repeater runat="server" ID="TreeViewRepeater">
                            <ItemTemplate>
                                <%# Eval("HtmlPrefix") %>
                                <div>
                                    <div>
                                        <div class="node-title">
                                            <a href='/ui/admin/contacts/groups/edit?contact=<%# Eval("Thumbprint") %>' class='text'><%# Eval("Name") %></a>

                                            <div class="node-labels">
                                                <small title="Subtype"><span class='badge bg-secondary'><i class='<%# (string)Eval("Icon") + " me-1" %>'></i><%# Eval("Subtype") %></span></small>
                                                <small title="Abbreviation" runat="server" visible='<%# !string.IsNullOrEmpty((string)Eval("Abbreviation")) %>'><span class='badge bg-secondary'><%# Eval("Abbreviation") %></span></small>
                                                <small title="Members" runat="server" visible='<%# (int)Eval("MemberCount") > 0 %>'><span class='badge bg-info'><i class="far fa-user me-1"></i><%# Eval("MemberCount") %></span></small>
                                                <small title="Permissions" runat="server" visible='<%# (int)Eval("PermissionCount") > 0 %>'><span class='badge bg-info'><i class="far fa-key me-1"></i><%# Eval("PermissionCount") %></span></small>
                                            </div>
                                        </div>

                                        <div class="commands">
                                            <insite:Button runat="server" ButtonStyle="OutlineSecondary" ToolTip="Edit" Icon="fas fa-pencil"
                                                NavigateUrl='<%# Eval("Thumbprint", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                                            />
                                            <insite:Button runat="server" ButtonStyle="OutlineSecondary" ToolTip="Indent"
                                                ID="IndentButton" Visible='<%# Eval("Previous") != null %>' Icon="fas fa-indent"
                                                CommandName="Indent" CommandArgument='<%# Eval("ContactID") + ":" + Eval("Previous.Identifier") %>'
                                            />
                                            <insite:Button runat="server" ID="OutdentButton" Visible='<%# Eval("Parent") != null && Eval("Parent.Parent") != null %>' 
                                                ButtonStyle="OutlineSecondary" ToolTip="Outdent" Icon="fas fa-outdent"
                                                CommandName="Outdent" CommandArgument='<%# Eval("ContactID") %>'
                                            />
                                        </div>
                                    </div>
                                </div>
                                <%# Eval("HtmlPostfix") %>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:NavItem>
        <insite:NavItem runat="server" ID="CriteriaTab" Icon="fas fa-filter" Title="Criteria">

            <h4>Criteria</h4>

            <div class="row">
                <div class="col-4">
                    <div class="mb-2">
                        <insite:TextBox runat="server" ID="FilterKeyword" EmptyMessage="Keyword" />
                    </div>

                    <div class="mb-2">
                        <insite:FilterButton runat="server" ID="SearchButton" />
                        <insite:ClearButton runat="server" ID="ClearButton" />
                        <asp:HiddenField runat="server" ID="StateValue" />
                    </div>
                </div>
            </div>
            
        </insite:NavItem>
    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                inSite.common.comboBox.init({
                    id: '#expand-level',
                    width: '115px'
                });

                inSite.common.treeView.init('<%= TreeViewRepeater.ClientID %>', {
                    expand: '#expand-all',
                    collapse: '#collapse-all',
                    level: '#expand-level',
                    state: '#<%= StateValue.ClientID %>'
                });
            })();
        </script>

        <script type="text/javascript">
            (function () {
                Sys.Application.add_load(function () {
                    $('#<%= FilterKeyword.ClientID %>')
                        .off('keydown', onKeyDown)
                        .on('keydown', onKeyDown);
                });

                function onKeyDown(e) {
                    if (e.which === 13) {
                        e.preventDefault();
                        $('#<%= SearchButton.ClientID %>')[0].click();
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>

</asp:Content>
