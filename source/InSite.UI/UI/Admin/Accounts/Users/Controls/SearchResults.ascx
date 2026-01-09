<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Accounts.Users.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="user-check" NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/identity/permissions/grant-access?user={0}") %>' ToolTip="Approve" />
                <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/accounts/users/edit?contact={0}") %>' ToolTip="Edit" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Full Name">
            <ItemTemplate>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/accounts/users/edit?contact={0}") %>' Text='<%# Eval("FullName") %>' ToolTip="Edit User" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href='mailto:<%# Eval("Email") %>'><%# Eval("Email") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Alternate">
            <ItemTemplate>
                <%# Eval("EmailAlternate") %>
            </ItemTemplate>
        </asp:TemplateField>
                                
        <asp:TemplateField HeaderText="Last Authenticated" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeTime(Eval("LastAuthenticated")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Access" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("UserAccessGranted") != null ? "<span class='badge bg-success'>Granted</span>" : "<span class='badge bg-danger'>Not Granted</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<div runat="server" id="BulkUpdatePanel" style="display: none;">
    <div style="padding-top: 20px;">
        
        <div class="row">
            <div class="col-md-4">

                <div runat="server" id="ConfirmMessage" class="alert alert-danger" role="alert">
                    
                    <i class="fas fa-stop-circle"></i>
                    <strong>Confirm:</strong>
                    Are you sure you want to bulk-update these users?

                    <div class="m-3 text-dark">
                        <asp:RadioButtonList runat="server" ID="UserAccess" RepeatDirection="Vertical">
                            <asp:ListItem Value="Access Granted" Text="Access Granted" />
                            <asp:ListItem Value="Access Revoked" Text="Access Revoked" />
                        </asp:RadioButtonList>
                    </div>

                    <div>
                        <insite:SaveButton runat="server" ID="SaveBulkButton" />
                        <insite:CancelButton runat="server" ID="CancelBulkButton" />
                    </div>

                </div>

            </div>
        </div>

    </div>
</div>

<script type="text/javascript">

    $(document).ready(function () {
        $("#<%= StartBulkUpdateButton.ClientID %>").click(function (e) {
            e.preventDefault();

            $("#<%= BulkUpdatePanel.ClientID %>").show();
            $("html, body").animate({ scrollTop: $(document).height() }, 1000);
        });

        $("#<%= CancelBulkButton.ClientID %>").click(function (e) {
            e.preventDefault();

            $("#<%= BulkUpdatePanel.ClientID %>").hide();
            $("html, body").animate({ scrollTop: 0 }, 1000);
        });
    });

</script>

<insite:Alert runat="server" ID="BulkUpdateStatus" />

<div runat="server" id="BulkUpdateButtonPanel" class="row mt-4">
    <div class="col-md-12">
        <insite:Button runat="server" ID="StartBulkUpdateButton" ButtonStyle="Default" Text="Bulk Update" Icon="fas fa-pen-square" />
    </div>
</div>