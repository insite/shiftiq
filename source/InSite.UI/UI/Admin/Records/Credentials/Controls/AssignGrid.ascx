<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignGrid.ascx.cs" Inherits="InSite.UI.Admin.Records.Credentials.Controls.AssignGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>
        <asp:TemplateField HeaderStyle-Width="40px">
            <HeaderTemplate>
                <input type="checkbox" onclick="relationshipCreator.selectAll_clicked(this);" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="IsSelected" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Person">
            <ItemTemplate>
                <a href='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'><%# Eval("FullName") %></a>
                <span class="form-text"><%# Eval("PersonCode") %></span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("Email", "<a href='mailto:{0}'>{0}</a>") %>
                <span class="form-text"><%# Eval("EmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>

<insite:PageFooterContent runat="server" ID="ScriptLiteral">
    <script type="text/javascript">

        var relationshipCreator = {
            selectAll_clicked: function (chk) {
                setCheckboxes('<%= Grid.ClientID %>', chk.checked);
            }

        };

    </script>
</insite:PageFooterContent>
