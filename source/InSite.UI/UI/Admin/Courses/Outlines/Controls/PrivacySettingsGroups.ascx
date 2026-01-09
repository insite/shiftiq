<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PrivacySettingsGroups.ascx.cs" Inherits="InSite.UI.Admin.Courses.Outlines.Controls.PrivacySettingsGroups" %>

<h3>Groups</h3>

<div class="mb-3">
    <insite:Button runat="server" ID="SelectAllButton" ButtonStyle="Default" ToolTip="Select All" style="padding:5px 8px;" Icon="far fa-square" />
    <insite:Button runat="server" ID="UnselectAllButton" ButtonStyle="Default" ToolTip="Deselect All" style="padding:5px 8px; display:none;" Icon="far fa-check-square" />
    <insite:Button runat="server" ID="AddButton" ButtonStyle="Default" ToolTip="Add" style="padding: 5px 8px;" Icon="fas fa-plus-circle" />
    <insite:Button runat="server" ID="DeleteButton" ButtonStyle="Default" ToolTip="Delete" style="padding:5px 8px;" Icon="fas fa-trash-alt" ConfirmText="Are you sure to delete access for selected group(s)?" />
</div>

<div runat="server" id="PrivacyListPanel">
    <asp:Repeater runat="server" ID="ListRepeater">
        <HeaderTemplate>
            <table class="table">
        </HeaderTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
        <ItemTemplate>
            <tr>
                <td style="width:40px;">
                    <asp:Literal runat="server" ID="PrivacyIdentifier" Visible="false" Text='<%# Eval("PrivacyIdentifier") %>' />
                    <asp:CheckBox runat="server" ID="IsSelected" />
                </td>
                <td>
                    <%# Eval("GroupName") %>
                    <span class="form-text">
                        <%# Eval("GroupType") %>
                    </span>
                </td>
            </tr>
        </ItemTemplate>
    </asp:Repeater>
</div>

<asp:Button runat="server" ID="RefreshButton" style="display:none;" />

<insite:Modal runat="server" ID="AddWindow" Title="Add Groups" Width="800px" MinHeight="600px" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        var groupPrivacyList<%= ContainerType %> = {
            selectAll: function () {
                setCheckboxes('<%= PrivacyListPanel.ClientID %>', true);
                $("#<%= SelectAllButton.ClientID %>").hide();
                $("#<%= UnselectAllButton.ClientID %>").show();
                return false;
            },
            unselectAll: function () {
                setCheckboxes('<%= PrivacyListPanel.ClientID %>', false);
                $("#<%= SelectAllButton.ClientID %>").show();
                $("#<%= UnselectAllButton.ClientID %>").hide();
                return false;
            },
            add: function () {
                var url = '/ui/admin/courses/add-privacy-group?container=<%= ContainerIdentifier %>&type=<%= ContainerType %>';
                var wnd = modalManager.load("<%= AddWindow.ClientID %>", url);
                $(wnd).one('closed.modal.insite', function (e, s, a) {
                    if (a != null)
                        __doPostBack("<%= RefreshButton.UniqueID %>", '');
                });

                return false;
            }
        };

    </script>
</insite:PageFooterContent>