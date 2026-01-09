<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserConnectionGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.UserConnectionGrid" %>

<asp:Panel ID="CommandButtons" runat="server" CssClass="pb-3" Visible="false">
    <insite:Button runat="server" ID="SelectAllButton" ButtonStyle="OutlinePrimary" ToolTip="Select All" style="padding:5px 8px;" Icon="far fa-square" />
    <insite:Button runat="server" ID="UnselectAllButton" ButtonStyle="OutlinePrimary" ToolTip="Deselect All" style="display:none; padding:5px 8px;" Icon="far fa-check-square" />
    <insite:Button  runat="server" ID="AddConnectionButton" ButtonStyle="OutlinePrimary" ToolTip="New Connection" style="padding:5px 8px;" Icon="fas fa-plus-circle" />
    <insite:Button runat="server" ID="PreDeleteButton" ButtonStyle="OutlinePrimary" ToolTip="Delete Selected Users" style="padding:5px 8px;" Icon="fas fa-trash-alt" />
</asp:Panel>

<insite:Grid runat="server" ID="Grid" EnableSorting="false" DataKeyNames="ToUserIdentifier">
    <Columns>
        <insite:TemplateField FieldName="Select" ItemStyle-Width="40px">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="IsSelected" />
            </ItemTemplate>
        </insite:TemplateField>
                            
        <asp:TemplateField HeaderText="From">
            <ItemTemplate>
                <a href="<%# Eval("FromUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>"><%# Eval("FromUserFullName") %></a>
                <div class="form-text">
                    <%# Eval("FromUserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Relationship">
            <ItemTemplate>
                <div style="max-width:400px">
                    <asp:Repeater runat="server" ID="RelationshipRepeater">
                        <ItemTemplate>
                            <span class="badge bg-<%# Eval("Category") %>"><%# Eval("Title") %></span>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="To">
            <ItemTemplate>
                <a href="<%# Eval("ToUserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>"><%# Eval("ToUserFullName") %></a>
                <div class="form-text">
                    <%# Eval("ToUserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <insite:TemplateField FieldName="Commands" ItemStyle-Width="65px" ItemStyle-Wrap="false" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <insite:IconLink runat="server" ID="EditButton" Name="pencil" ToolTip="Edit"
                    NavigateUrl='<%# string.Format("/ui/admin/contacts/people/edit-user-connection?from={0}&to={1}", Eval("FromUserIdentifier"), Eval("ToUserIdentifier")) %>' />
                <insite:IconLink runat="server" ID="DeleteButton" Name="trash-alt" ToolTip="Delete"
                    NavigateUrl='<%# string.Format("/ui/admin/contacts/people/delete-user-connection?from={0}&to={1}", Eval("FromUserIdentifier"), Eval("ToUserIdentifier")) %>' />
            </ItemTemplate>
        </insite:TemplateField>
    </Columns>
</insite:Grid>

<div class="d-none">
    <asp:Button runat="server" ID="RefreshButton" />
    <asp:Button runat="server" ID="DeleteConnectionButton" />
</div>

<insite:PageFooterContent runat="server" ID="FooterScript">
    <script type="text/javascript">

        (function () {
            var userConnection = window.userConnection = window.userConnection || {};

            userConnection.selectAll = function (containerId, selectId, unselectId) {
                $('#' + selectId).css('display', 'none');
                $('#' + unselectId).css('display', '');

                return setCheckboxes(containerId, true);
            };

            userConnection.unselectAll = function (containerId, selectId, unselectId) {
                $('#' + selectId).css('display', '');
                $('#' + unselectId).css('display', 'none');

                return setCheckboxes(containerId, false);
            };

            userConnection.initSelectAll = function (containerId, selectId, unselectId) {
                var selectDisplay = 'none';
                var unselectDisplay = 'none';
            
                var $checkboxes = $('#' + containerId + ' input[type="checkbox"]');
                if ($checkboxes.length > 0) {
                    var total = $checkboxes.length;
                    var checked = $checkboxes.filter(':checked').length;

                    if (checked >= total)
                        unselectDisplay = '';
                    else
                        selectDisplay = '';
                }
            
                $('#' + selectId).css('display', selectDisplay);
                $('#' + unselectId).css('display', unselectDisplay);
            };
        })();

    </script>
</insite:PageFooterContent>
