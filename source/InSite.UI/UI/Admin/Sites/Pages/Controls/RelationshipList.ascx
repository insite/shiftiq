<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RelationshipList.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.RelationshipList" %>

<insite:PageHeadContent runat="server" ID="HeaderContent">
    <style type="text/css">

        .res-cont-list {
            width: 100%;
        }

        .res-cont-list > h4:first-child {
            margin-top: 0;
        }

        .res-cont-list > h4 > span.form-text strong {
            color: #808080;
        }

        .res-cont-list > table  {
            width: 100%;
        }

        .res-cont-list > table > tr > td {
            vertical-align: baseline !important;
        }

        .res-cont-list > table td.res-sequence {
            width: 40px;
            white-space: nowrap;
        }

        .res-cont-list > table td.res-number {
            width: 100px;
            white-space: nowrap;
        }

        .res-cont-list > table td.res-type {
            width: 100px;
            white-space: nowrap;
        }

        .res-cont-list > table td.res-number {
            width: 50px;
            white-space: nowrap;
            text-align: right;
        }

        .res-cont-list .ui-sortable {
        }

        .res-cont-list .ui-sortable > tr,
        .res-cont-list .ui-sortable > tbody > tr {
            cursor: move !important;
        }

        .res-cont-list .ui-sortable > tr > td:first-child i.move,
        .res-cont-list .ui-sortable > tbody > tr > td:first-child i.move{
            display: inline-block !important;
        }

        .res-cont-list .ui-sortable > tr:hover,
        .res-cont-list .ui-sortable > tbody > tr:hover {
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper{
            outline: 1px solid #666666;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-helper > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-helper > td:first-child{
            background-image: none !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #666666 !important;
        }

        .res-cont-list .ui-sortable > tr.ui-sortable-placeholder > td:first-child,
        .res-cont-list .ui-sortable > tbody > tr.ui-sortable-placeholder > td:first-child{
            background-image: none !important;
        }
                
    </style>
</insite:PageHeadContent>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="mb-3">
            <div id="CommandButtons2" runat="server" class="reorder-trigger reorder-hide">
                <insite:Button runat="server" ID="AddResourceLink" Text="New Page" Icon="fas fa-plus-circle" ButtonStyle="Default" />
                <insite:Button runat="server" ID="ReorderButton" Text="Reorder" Icon="fas fa-sort" ButtonStyle="Default" />
            </div>
            <asp:Panel ID="ReorderCommandButtons" runat="server" CssClass="reorder-trigger reorder-visible reorder-inactive">
                <insite:SaveButton runat="server" OnClientClick="inSite.common.gridReorderHelper.saveReorder(true); return false;" />
                <insite:CancelButton runat="server" OnClientClick="inSite.common.gridReorderHelper.cancelReorder(true); return false;" />
            </asp:Panel>
        </div>

        <div runat="server" id="SectionControl">

            <div class="res-cont-list">
                <asp:Repeater runat="server" ID="DataRepeater">
                    <HeaderTemplate>
                        <table class="table table-striped">
                            <thead>
                                <tr>
                                    <th>Type</th>
                                    <th>Title</th>
                                    <th>Slug</th>
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
                                <%# Eval("Type") %>
                            </td>
                            <td class="res-title">
                                <a href="<%# Eval("EditUrl") %><%# Eval("Identifier") %>"><%# Eval("Title") %></a>
                                <span class="form-text"><%# Eval("Control") %></span>
                            </td>
                            <td class="res-name">
                                <a href="<%# Eval("EditUrl") %><%# Eval("Identifier") %>"><%# Eval("Slug") %></a>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<asp:Button runat="server" ID="RefreshButton" style="display:none;" />
