<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGrantGrid.ascx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.Achievements.Controls.UserGrantGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px">
            <HeaderTemplate>
                <insite:CheckBox runat="server" ID="AllSelected" CssClass="root-checkbox" />
            </HeaderTemplate>
            <ItemTemplate>
                <asp:Literal runat="server" ID="UserIdentifier" Visible="false" Text='<%# Eval("UserIdentifier") %>' />
                <insite:CheckBox runat="server" ID="Selected" Checked="<%# IsSelected() %>" Visible='<%# Eval("Validated") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Learner">
            <ItemTemplate>
                <a target="_blank" href='<%# Eval("LearnerUrl") %>'><%# Eval("UserFullName") %></a>
                <span class="form-text"><%# Eval("PersonCode") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("Email", "<a href='mailto:{0}'>{0}</a>") %>
                <span class="form-text"><%# Eval("EmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer">
            <ItemTemplate>
                <a target="_blank" href="/ui/admin/contacts/groups/edit?contact=<%# Eval("EmployerIdentifier") %>">
                    <%# Eval("Employer") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Entries">
            <ItemTemplate>
                <%# Eval("ExperienceCount") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <asp:Literal runat="server" Visible='<%# Eval("Validated") %>'>
                    <span class="text-success">Validated</span>
                </asp:Literal>
                <asp:Literal runat="server" Visible='<%# Eval("NotValidated") %>'>
                    <span class="text-danger">Not Validated</span>
                </asp:Literal>
                <asp:Literal runat="server" Visible='<%# Eval("NoEntries") %>'>
                    <span class="text-danger">No Entries</span>
                </asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:PageFooterContent runat="server">
<script type="text/javascript">
    (function () {
        $(".root-checkbox input[type=checkbox]").on("click", function () {
            var $this = $(this);
            var checked = this.checked;

            $this.closest("table").find("input[type=checkbox]").prop("checked", checked);
        });
    })();
</script>
</insite:PageFooterContent>
