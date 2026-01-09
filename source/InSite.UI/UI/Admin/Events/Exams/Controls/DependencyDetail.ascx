<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DependencyDetail.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.DependencyDetail" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>
        <asp:TemplateField  HeaderText="Contact">
            <itemtemplate>
                <a href='/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>'><%# Eval("UserFullName") %></a>
                <span class="form-text">
                    <%# Eval("PersonCode") %>
                </span>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField  HeaderText="Email">
            <itemtemplate>
                <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField  HeaderText="Role" ItemStyle-Width="200px">
            <itemtemplate>
                <%# Eval("AttendeeRole") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assigned" ItemStyle-Width="150px">
            <itemtemplate>
                <%# LocalizeDate(Eval("Assigned")) %>
            </itemtemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
