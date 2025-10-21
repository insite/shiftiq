<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Contacts.People.Controls.SearchResults" %>

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier" Translation="Header">
        <Columns>
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:HyperLink runat="server" Text='<%# Eval("FullName") %>' ToolTip="View Person"
                        NavigateUrl='<%# Eval("UserIdentifier", "/ui/portal/contacts/people/outline?learner={0}") %>' />
                    <div class="fs-sm">
                        <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>

            <insite:BoundField FieldName="PersonCode" DataField="PersonCode" HeaderStyle-Wrap="false" />

            <asp:TemplateField HeaderText="Created">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("Created")) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Last Authenticated">
                <ItemTemplate>
                    <%# LocalizeDate(Eval("LastAuthenticated")) %>
                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>