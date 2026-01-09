<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserGrid.ascx.cs" Inherits="InSite.Admin.Records.Logbooks.Controls.UserGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField ItemStyle-Width="80px" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="search" ToolTip="View Learner's Logbook" NavigateUrl='<%# Eval("OutlineUrl") %>' />
                <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete" NavigateUrl='<%# Eval("DeleteUrl") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Learner">
            <ItemTemplate>
                <a href='<%# Eval("LearnerUrl") %>'><%# Eval("UserFullName") %></a>
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
                <a href="/ui/admin/contacts/groups/edit?contact=<%# Eval("EmployerIdentifier") %>">
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
                <asp:Literal runat="server" Visible='<%# Eval("HasAchievement") %>'>
                    <span class="text-info">Has Achievement</span>
                </asp:Literal>
                <asp:Literal runat="server" Visible='<%# Eval("Validated") %>'>
                    <span class="text-success">Validated</span>
                </asp:Literal>
                <asp:Literal runat="server" Visible='<%# Eval("NotValidated") %>'>
                    <span class="text-danger">Not Validated</span>
                </asp:Literal>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
