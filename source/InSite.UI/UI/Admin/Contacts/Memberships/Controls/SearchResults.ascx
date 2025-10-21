<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Contacts.Memberships.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:BoundField HeaderText="Group Type" DataField="GroupType" />
        <asp:BoundField HeaderText="Group Tag" DataField="GroupLabel" />

        <asp:TemplateField HeaderText="Group Name">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'
                    Text='<%# Eval("GroupName") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Person Name">
            <ItemTemplate>
                <asp:HyperLink runat="server"
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'
                    Text='<%# Eval("UserFullName") %>' />
                <span class="form-text"><%# Eval("PersonCode") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Person Email">
            <ItemTemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Function" DataField="MembershipFunction" />

        <asp:TemplateField HeaderText="Effective" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("MembershipAssigned")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiry" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("MembershipExpiry")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>