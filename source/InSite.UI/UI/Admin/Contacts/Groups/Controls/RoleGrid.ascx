<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleGrid.ascx.cs" Inherits="InSite.Admin.Contacts.Groups.Controls.RoleGrid" %>

<div class="row" style="padding-bottom: 18px;">
    <div class="col-md-6">
        <insite:ComboBox runat="server" ID="SortType" Width="200" CssClass="d-inline-block">
            <Items>
                <insite:ComboBoxOption Text="Sort by Full Name" Value="ByFullName" Selected="true" />
                <insite:ComboBoxOption Text="Sort by Effective Date" Value="ByEffectiveDate" />
                <insite:ComboBoxOption Text="Sort by Membership Function" Value="ByMembershipFunction" />
                <insite:ComboBoxOption Text="Sort by Last Name" Value="ByLastName" />
            </Items>
        </insite:ComboBox>
        <insite:TextBox runat="server" ID="FilterTextBox" Width="200" EmptyMessage="Filter" CssClass="d-inline-block" />
        <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
    </div>
    <div class="col-md-6 text-end">
        <insite:Button runat="server" ButtonStyle="Default" OnClientClick="roleGrid.onViewMembershipHistoryClick(); return false;" Text="Group Membership History" Icon="fas fa-history" />
        <insite:ButtonSpacer runat="server" />
        <insite:Button runat="server" ID="AddButton" ButtonStyle="OutlinePrimary" Text="Add People" Icon="fas fa-plus-circle" />
        <insite:DropDownButton runat="server" ID="DownloadBtn" CssClass="d-inline-block" ButtonStyle="OutlinePrimary" DefaultAction="None" IconName="download" Text="Download">
            <Items>
                <insite:DropDownButtonItem Name="Contacts" IconName="users" Text="Contacts" ToolTip="Download group contacts" />
                <insite:DropDownButtonItem Name="ShippingMailingLabels" IconName="mail-bulk" Text="Shipping Mailing Labels" ToolTip="Download group shipping mailing labels" />
                <insite:DropDownButtonItem Name="BillingMailingLabels" IconName="mail-bulk" Text="Billing Mailing Labels" ToolTip="Download group billing mailing labels" />
                <insite:DropDownButtonItem Name="WorkMailingLabels" IconName="mail-bulk" Text="Work Mailing Labels" ToolTip="Download group work mailing labels" />
                <insite:DropDownButtonItem Name="HomeMailingLabels" IconName="mail-bulk" Text="Home Mailing Labels" ToolTip="Download group home mailing labels" />
            </Items>
        </insite:DropDownButton>
    </div>
</div>

<div class="row">
    <div class="col-xs-12">

        <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
            <Columns>

                <asp:TemplateField ItemStyle-Wrap="false" ItemStyle-Width="60px">
                    <ItemTemplate>
                        <insite:IconLink runat="server" ID="EditButton" Name="pencil" ToolTip="Edit"
                            NavigateUrl='<%# string.Format("/ui/admin/contacts/people/edit-membership?from={0}&to={1}", GroupIdentifier, Eval("UserIdentifier")) %>' />
                        <insite:IconLink runat="server" Name="trash-alt" ToolTip='Delete'
                            NavigateUrl='<%# string.Format("/ui/admin/contacts/people/delete-membership?from={0}&to={1}", GroupIdentifier, Eval("UserIdentifier")) %>' />
                        <insite:IconLink runat="server" ID="HistoryLink" Name="history" ToolTip="History" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Name" ItemStyle-Wrap="False">
                    <ItemTemplate>
                        <div><a title="Name" href='/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>'><%# Eval("Name") %></a></div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Email">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="EmailLink" />
                        <asp:Label runat="server" ID="EmailLabel" CssClass="email-disabled" />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField HeaderText="Person Code" DataField="AccountNumber" HeaderStyle-Wrap="false" />
                <asp:BoundField HeaderText="Function" DataField="RoleType" />

                <asp:TemplateField HeaderText="Effective">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("Assigned")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Expiry">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("MembershipExpiry")) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>

    </div>
</div>

<insite:Modal runat="server" ID="HistoryViewerWindow" Title="History" Width="710px" MinHeight="520px" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            Sys.Application.add_load(function () {
                $('#<%= FilterTextBox.ClientID %>')
                    .off('keydown', onKeyDown)
                    .on('keydown', onKeyDown);
            });

            function onKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    $('#<%= FilterButton.ClientID %>')[0].click();
                }
            }
        })();

        (function () {
            var instance = window.roleGrid = window.roleGrid || {};
            instance.onViewMembershipHistoryClick = function () {
                modalManager.load('<%= HistoryViewerWindow.ClientID %>', '/ui/admin/reports/changes/history?id=<%= GroupIdentifier %>&type=group_membership');
            };
        })();

    </script>
</insite:PageFooterContent>
