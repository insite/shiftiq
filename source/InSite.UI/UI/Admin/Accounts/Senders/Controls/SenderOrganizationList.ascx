<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SenderOrganizationList.ascx.cs" Inherits="InSite.Admin.Accounts.Senders.Controls.SenderOrganizationList" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" DisplayAfter="500" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="row">
            <div class="col-lg-4">
                <insite:FindOrganization runat="server" ID="OrganizationSelector" EmptyMessage="Add Organization" ShowFooter="false" />
            </div>
            <div class="col-lg-8"></div>
        </div>

        <div class="row">
            <div class="col-lg-12">

                <asp:Repeater runat="server" ID="Repeater">
                    <HeaderTemplate>
                        <table id="<%# ClientID %>" class="table">
                            <thead>
                                <tr>
                                    <th>Organization</th>
                                    <th>Code</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <a href="/ui/admin/accounts/organizations/edit?<%# Eval("OrganizationIdentifier", "organization={0}") %>">
                                    <%# Eval("CompanyName") %>
                                </a>
                            </td>
                            <td>
                                <%# Eval("OrganizationCode") %>
                            </td>

                            <td class="text-end">
                                <insite:IconButton runat="server" 
                                    CommandName="Delete" CommandArgument='<%# Eval("OrganizationIdentifier") %>' Name="trash-alt" ToolTip="Delete" 
                                    ConfirmText="Are you sure you want to delete this organization?" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </tbody></table>
                    </FooterTemplate>
                </asp:Repeater>

            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        #<%= UpdatePanel.ClientID %> .input-has-error {
            box-shadow: inset 0 1px 1px rgba(0, 0, 0, 0.075), 0 0 6px #ce8483;
        }
    </style>
</insite:PageHeadContent>
