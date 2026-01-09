<%@ Control AutoEventWireup="true" CodeBehind="PersonGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.PersonGrid" Language="C#" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Name">
            <ItemTemplate>
                <div><asp:Literal runat="server" id="FullName" /></div>
                <div class="form-text"><%# Eval("OrganizationName") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="City" DataField="WorkAddress.City" />
        <asp:BoundField HeaderText="Province" DataField="WorkAddress.Province" />
        <asp:BoundField HeaderText="Company" DataField="SingleCompanyName" />

        <asp:TemplateField HeaderText="Email" SortExpression="Email">
            <ItemTemplate>
                <%# Eval("Email", "<a href='mailto:{0}'>{0}</a>") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organization">
            <ItemTemplate>
                <%# Eval("OrganizationIdentifier") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
