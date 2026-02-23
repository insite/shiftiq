<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnrollmentUserGrid.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.EnrollmentUserGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="LearnerIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to remove this enrollment?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Learner.UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                    <%# Eval("Learner.UserFullName") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href="mailto:<%# Eval("Learner.UserEmail") %>"><%# Eval("Learner.UserEmail") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Enrolled" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("EnrollmentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>