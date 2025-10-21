<%@ Control Language="C#" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Admin.Contacts.MembershipReasons.Controls.SearchResults" %>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>

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
                <span class="form-text"><%# GetPersonCode() %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Reason Type" DataField="ReasonType" />
        <asp:BoundField HeaderText="Reason Subtype" DataField="ReasonSubtype" />

        <asp:TemplateField HeaderText="Reason Effective" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("ReasonEffective")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Reason Expiry" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("ReasonExpiry")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Occupation" DataField="PersonOccupation" />

        <asp:TemplateField HeaderText="Created By">
            <ItemTemplate>
                <%# GetCreatedBy() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="False">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# Eval("ReasonIdentifier", "/ui/admin/contacts/memberships/reasons/edit?reason={0}") %>' ToolTip="Edit" />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>