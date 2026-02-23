<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EnrollmentGroupGrid.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.EnrollmentGroupGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="GroupEnrollmentIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="40px" ItemStyle-Wrap="false" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <insite:IconButton runat="server" ID="DeleteItemButton" Name="trash-alt" ToolTip="Remove"
                    CommandName="Delete"
                    ConfirmText="Are you sure you want to remove this enrollment?" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group">
            <ItemTemplate>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("GroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'>
                    <%# Eval("Group.GroupName") %>
                </asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Size" DataField="Group.QMemberships.Count" DataFormatString="{0:n0}" 
            HeaderStyle-HorizontalAlign="Right" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="80px" />

        <asp:TemplateField HeaderText="Enrolled" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("EnrollmentStarted")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>