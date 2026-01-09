<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignPeriodGrid.ascx.cs" Inherits="InSite.Admin.Records.Gradebooks.Controls.AssignPeriodGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="LearnerIdentifier">
    <Columns>
        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <a href='<%# Eval("LearnerIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'><%# Eval("UserFullName") %></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                <span class="form-text"><%# Eval("UserEmailAlternate", "<a href='mailto:{0}'>{0}</a>") %></span>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Graded" HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right">
            <ItemTemplate>
                <%# GetLocalDate(Eval("Graded")) %>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Period">
            <ItemTemplate>
                <a href='<%# Eval("PeriodIdentifier", "/ui/admin/records/periods/edit?period={0}") %>'><%# Eval("PeriodName") %></a>
                <span runat="server" visible='<%# Eval("PeriodIdentifier") != null %>' class="form-text"><%# Eval("PeriodStart", "{0:MMM d, yyyy}") %> - <%# Eval("PeriodEnd", "{0:MMM d, yyyy}") %></span>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</insite:Grid>
