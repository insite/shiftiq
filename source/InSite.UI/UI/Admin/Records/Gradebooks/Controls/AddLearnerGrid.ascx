<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AddLearnerGrid.ascx.cs" Inherits="InSite.UI.Admin.Records.Gradebooks.Controls.AddLearnerGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="50px"> 
            <HeaderTemplate>
                <input id="SelectAll" type="checkbox" onclick="onSelectAll()" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="Selected" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Person">
            <ItemTemplate>
                <a href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'><%# Eval("UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                <span class="form-text"><%# Eval("UserEmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("ApprovalStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>