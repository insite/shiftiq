<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelationshipList.ascx.cs" Inherits="InSite.Admin.Standards.Standards.Controls.RelationshipList" %>

<insite:PageHeadContent runat="server" ID="HeaderContent">

    <style type="text/css">

        .asset-graph-list {
            width: 100%;
        }

        .asset-graph-list > h4:first-child {
            margin-top: 0;
        }

        .asset-graph-list > h4 > span.form-text strong {
            color: #808080;
        }

        .asset-graph-list > table  {
            width: 100%;
        }

        .asset-graph-list > table td.asset-select {
            width: 40px;
        }

        .asset-graph-list > table td.asset-condition {
            width: 100px;
            white-space: nowrap;
        }

        .asset-graph-list > table td.asset-subtype {
            width: 100px;
            white-space: nowrap;
        }

        .asset-graph-list > table td.asset-number {
            width: 65px;
            white-space: nowrap;
        }

        .asset-graph-list > table td.asset-nav {
            width: 80px;
            white-space: nowrap;
        }

        .asset-graph-list > table td.asset-cmd {
            width: 80px;
            text-align: right;
        }
                
        .asset-graph-list > table td.asset-cmd a {
            cursor: pointer;
        }

        .asset-graph-list .ui-sortable {
        }

        .asset-graph-list .ui-sortable > tr,
        .asset-graph-list .ui-sortable > tbody > tr {
            cursor: move !important;
        }

        .asset-graph-list .ui-sortable > tr > td:first-child i.move,
        .asset-graph-list .ui-sortable > tbody > tr > td:first-child i.move{
            display: inline-block !important;
        }

        .asset-graph-list .ui-sortable > tr:hover,
        .asset-graph-list .ui-sortable > tbody > tr:hover {
            outline: 1px solid #666666;
        }

        .asset-graph-list .ui-sortable > tr.ui-sortable-helper,
        .asset-graph-list .ui-sortable > tbody > tr.ui-sortable-helper{
            outline: 1px solid #666666;
        }

        .asset-graph-list .ui-sortable > tr.ui-sortable-helper > td:first-child,
        .asset-graph-list .ui-sortable > tbody > tr.ui-sortable-helper > td:first-child{
            background-image: none !important;
        }

        .asset-graph-list .ui-sortable > tr.ui-sortable-placeholder,
        .asset-graph-list .ui-sortable > tbody > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

        .asset-graph-list .ui-sortable > tr.ui-sortable-placeholder > td:first-child,
        .asset-graph-list .ui-sortable > tbody > tr.ui-sortable-placeholder > td:first-child{
            background-image: none !important;
        }

        .asset-edge-list tr > td {
            vertical-align: baseline !important;
        }

        .dropdown-menu div.asset-item > a {
            display: block;
            padding: 3px 20px;
            clear: both;
            font-weight: normal;
            line-height: 1.42857143;
            color: #333;
            white-space: nowrap;
        }

        .dropdown-menu div.asset-item > a:hover,
        .dropdown-menu div.asset-item > a:focus {
            color: #262626;
            text-decoration: none;
            background-color: #f5f5f5;
        }

        .disable-click{
            pointer-events: none;
        }
    </style>

</insite:PageHeadContent>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div runat="server" id="SectionControl">
            <div style="padding-bottom:10px; height:40px;">
                <div id="CommandButtons2" runat="server" class="reorder-trigger reorder-hide" style="padding-bottom: 10px;">
                    <insite:Button runat="server" ID="SelectAllButton" ButtonStyle="Default" ToolTip="Select All" style="padding:5px 8px;" Icon="far fa-square" />
                    <insite:Button runat="server" ID="UnselectAllButton" ButtonStyle="Default" ToolTip="Deselect All" style="display:none; padding:5px 8px;" Icon="far fa-check-square" />
                    <insite:Button runat="server" ID="ReorderButton" ButtonStyle="Default" ToolTip="Reorder" style="padding:5px 8px;" Icon="fas fa-sort" />
                    <insite:Button runat="server" ID="CreateRelationshipButton" ButtonStyle="Default" ToolTip="Add Relationship" style="padding: 5px 8px;" Icon="fas fa-plus-circle" />
                    <insite:Button runat="server" ID="PreDeleteButton" ButtonStyle="Default" ToolTip="Delete Selected Relationships" style="padding:5px 8px;" Icon="fas fa-trash-alt" />
                </div>
            </div>

            <div>
                <div class="asset-graph-list">
                    <asp:Repeater runat="server" ID="GraphRepeater">
                        <ItemTemplate>
                            <h4>
                                <%# Eval("GraphName") %>
                                <br />
                                <span class="form-text"><%# Eval("GraphDescription") %></span>
                            </h4>

                            <table class="table table-striped asset-edge-list">
                                <tbody>
                                    <asp:Repeater runat="server" ID="EdgeRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <asp:PlaceHolder runat="server" Visible='<%# EdgeDirection == ConnectionDirection.Outgoing %>'>
                                                    <td class="asset-select">
                                                        <asp:Literal ID="FromID" runat="server" Text='<%# Eval("EdgeFromID") %>' Visible="False" />
                                                        <asp:Literal ID="ToID" runat="server" Text='<%# Eval("EdgeToID") %>' Visible="False" />
                                                        <asp:Literal ID="IsEdge" runat="server" Text='<%# Eval("IsEdge") %>' Visible="false" />
                                                        <asp:CheckBox runat="server" ID="IsSelected" />
                                                    </td>
                                                </asp:PlaceHolder>
                                                <td class="asset-subtype"><%# Eval("StandardType") %></td>
                                                <td class="asset-number">
                                                    <a href="/ui/admin/standards/edit?id=<%# Eval("StandardIdentifier") %>"><%# Eval("AssetNumber") %></a>
                                                </td>
                                                <td class="asset-title">
                                                    <%# Eval("StandardTitle") %>
                                                </td>
                                                <td class="asset-cmd">
                                                    <a class="link-button reorder-trigger reorder-hide" title="View Details" data-action="show-asset-menu" data-number="<%# Eval("AssetNumber") %>" data-return-number='<%# AssetNumber.ToString() %>'><i class='icon fas fa-bars'></i></a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

            <div style="padding-top: 20px;">
                <asp:Panel ID="ReorderCommandButtons" runat="server" RenderClientID="true" CssClass="reorder-trigger reorder-visible reorder-inactive">
                    <insite:SaveButton runat="server" OnClientClick="inSite.common.gridReorderHelper.saveReorder(true); return false;" />
                    <insite:CancelButton runat="server" OnClientClick="inSite.common.gridReorderHelper.cancelReorder(true); return false;" />
                </asp:Panel>
            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<asp:Button runat="server" ID="RefreshButton" style="display:none;" />
<asp:Button runat="server" ID="DeleteRelationshipButton" style="display:none;" />

<insite:Modal runat="server" ID="AssetInfoWindow" />
<insite:Modal runat="server" ID="CreatorWindow" Title="Create Standard Relationship" Width="800px" MinHeight="600px" />

<insite:PageFooterContent runat="server" ID="FooterScript">
<script type="text/javascript">

    (function () {
        var _callbackId = null;

        function onWindowClose(e, s, a) {
            if (_callbackId != null && a != null)
                __doPostBack(_callbackId, '');

            _callbackId = null;
        }

        var assetGraphList = window.assetGraphList = window.assetGraphList || {};

        assetGraphList.showCreator = function (assetId, edgeType, windowId, callbackId) {
            _callbackId = callbackId;

            var wnd = modalManager.load(windowId, '/ui/admin/standards/relationships/create?assetId=' + assetId + "&edgetype=" + edgeType);
            $(wnd).one('closed.modal.insite', onWindowClose);

            return false;
        };
    })();

</script>
</insite:PageFooterContent>

<insite:PageFooterContent runat="server">
<script type="text/javascript">

    (function () {
        var allowCloseInfoWindow = true;

        Sys.Application.add_load(onLoad);

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

        function onLoad() {
            initSectionButtons();

            $("#<%= PreDeleteButton.ClientID %>")
                .off('click', onDeleteRelationships)
                .on('click', onDeleteRelationships);
        }

        function initSectionButtons() {
            $('#<%= SectionControl.ClientID %> button')
                .off('click', onSectionButtonClick)
                .on('click', onSectionButtonClick);

            $('#<%= SectionControl.ClientID %> .link-button')
                .on('click', onSectionButtonClick);
        }

        function onSectionButtonClick() {
            var $this = $(this);
            var action = $this.data('action');

            if (action === 'show-asset-menu') {
                var number = parseInt($this.data('number'));
                if (isNaN(number))
                    return;

                var returnNumber = parseInt($this.data('return-number'));
                if (isNaN(returnNumber))
                    returnNumber = null;

                loadInfoWindow(number, returnNumber);
            }
        }

        function loadInfoWindow(number, returnNumber) {
            if ($.active > 0)
                return;

            allowCloseInfoWindow = true;

            var returnUrl = window.parent.location.pathname + inSite.common.updateQueryString('panel', 'relationships', window.location.search);
            var wnd = modalManager.load('<%= AssetInfoWindow.ClientID %>', '/ui/admin/standards/info?asset=' + String(number) + '&returnUrl=' + encodeURIComponent(returnUrl));

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
                        var wnd = modalManager.load('<%= CreatorWindow.ClientID %>', '/ui/admin/standards/relationships/create?assetId=' + String(a.asset.id));
                        $(wnd)
                            .data('number', a.asset.number)
                            .one('closed.modal.insite', function (e, s, a) {
                                if (a != null)
                                    __doPostBack('<%= RefreshButton.UniqueID %>', '');
                                else
                                    loadInfoWindow($(s).data('number'), returnNumber);
                            });
                    }
                });
        }

        function onDeleteRelationships() {
            if ($("#<%= SectionControl.ClientID %> input[id$='IsSelected']:checked").length > 0) {
                if (confirm('Are you sure you want to delete the selected relationship(s)?'))
                    __doPostBack('<%= DeleteRelationshipButton.UniqueID %>', '');
            }
            else {
                alert("Please select the relationship(s) you want to delete.");
            }

            return false;
        }
    })();

</script>
</insite:PageFooterContent>
