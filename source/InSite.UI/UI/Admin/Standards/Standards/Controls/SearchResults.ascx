<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="StandardIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Wrap="False" ItemStyle-Width="92px">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>'><i class="icon far fa-pencil"></i></a>
                <a title="Chart" href='<%# string.Format("/ui/admin/standards/occupations/chart?asset={0}", Eval("StandardIdentifier")) %>' runat="server" visible='<%# Eval("StandardType").ToString() == "Profile" %>'><i class="icon far fa-boxes"></i></a>
                <a title="Outline" href='<%# string.Format("/ui/admin/standards/manage?standard={0}", Eval("StandardIdentifier")) %>' runat="server" visible='<%# Eval("ParentStandardIdentifier") == null %>'><i class="icon far fa-sitemap"></i></a>
                <a runat="server" visible="<%# CanDelete %>" title="Delete" href='<%# Eval("StandardIdentifier", "/admin/standards/delete?asset={0}") %>'><i class="icon far fa-trash-alt"></i></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Type" DataField="StandardType" />
        <asp:BoundField HeaderText="Tier" DataField="StandardTier" />
        <asp:BoundField HeaderText="Tag" DataField="StandardLabel" />
        <asp:BoundField HeaderText="Code" DataField="Code" />
        <asp:BoundField HeaderText="Hook" DataField="StandardHook" />

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>'><%# Eval("ContentTitle") %></a>
                <%# InSite.Admin.Standards.Standards.Utilities.OutlineHelper.GetStatusBadgeHtml((string)Eval("StandardStatus"), null) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Internal Name" DataField="ContentName" />

        <asp:TemplateField HeaderText="Number" HeaderStyle-Wrap="false" 
                           ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="15px">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>'><%# Eval("AssetNumber") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Hierarchy" ItemStyle-Wrap="False" ItemStyle-Width="47px">
            <ItemTemplate>
                <span class="form-text">
                    <%# GetHierarchyHtml() %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Parent Name">
            <ItemTemplate>
                <a href='/ui/admin/standards/edit?id=<%# Eval("ParentStandardIdentifier") %>'><%# Eval("ParentContentTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Child Count" DataField="ParentChildCount" HeaderStyle-Wrap="false" 
            ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="15px"/>

    </Columns>
</insite:Grid>

<asp:Button ID="RefreshButton" runat="server" style="display:none;" />

<insite:Modal runat="server" ID="AssetInfoWindow" />
<insite:Modal runat="server" ID="RelationshipCreatorWindow" Title="Add New Relationships" Width="800px" MinHeight="600px" />

<insite:PageHeadContent runat="server">
    <style type="text/css">

        .grid .btn-group > .dropdown-menu > li > a {
            padding-left: 45px;
            text-decoration: none !important;
        }

            .grid .btn-group > .dropdown-menu > li > a > i.cmd-icon {
                position: absolute;
                margin-left: -28px;
            }

            .grid .btn-group > .dropdown-menu > li > a:hover > .icon  {
                opacity: 1;
            }

            .grid .btn-group > .dropdown-menu > li > a i.inline-icon {
                opacity: 0.7;
            }
            
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var allowCloseInfoWindow = true;

            $(window).on('message', function (e) {
                var eventData = String(e.originalEvent.data);
                if (!eventData.startsWith('insite.assetSummary:'))
                    return;

                var command = eventData.substring(19);

                if (command === 'disable-close')
                    allowCloseInfoWindow = false;
                else if (command === 'enable-close')
                    allowCloseInfoWindow = true;
            });

            $('#<%= Grid.ClientID %> button').on('click', function () {
                var $this = $(this);
                var action = $this.data('action');

                if (action === 'show-asset-menu') {
                    var number = parseInt($this.data('number'));
                    if (isNaN(number))
                        return;

                    loadInfoWindow(number);
                }
            });

            function loadInfoWindow(number) {
                if ($.active > 0)
                    return;

                allowCloseInfoWindow = true;

                var wnd = modalManager.load('<%= AssetInfoWindow.ClientID %>', '/ui/admin/standards/info?asset=' + String(number));

                modalManager.setTitle(wnd, 'Loading...');

                $(wnd)
                    .on('hide.bs.modal', function (e, s, a) {
                        if (!allowCloseInfoWindow) {
                            e.preventDefault();
                            e.stopImmediatePropagation();
                            return false;
                        }
                    })
                    .one('closing.modal.insite', function (e, s, a) {
                        if (a === null)
                            return;

                        if (a.action === 'redirect')
                            window.location = a.url;
                        else if (a.action === 'refresh')
                            __doPostBack('<%= RefreshButton.UniqueID %>', '');
                    })
                    .one('closed.modal.insite', function (e, s, a) {
                        if (a === null)
                            return;

                        if (a.action === 'relate') {
                            var wnd = modalManager.load('<%= RelationshipCreatorWindow.ClientID %>', '/ui/admin/standards/relationships/create?assetId=' + String(a.asset.id));
                            $(wnd)
                                .data('number', a.asset.number)
                                .one('closed.modal.insite', function (e, s, a) {
                                    if (a != null)
                                        __doPostBack('<%= RefreshButton.UniqueID %>', '');
                                    else
                                        loadInfoWindow($(s).data('number'));
                                });
                        }
                    });
            }
        })();
    </script>
</insite:PageFooterContent>