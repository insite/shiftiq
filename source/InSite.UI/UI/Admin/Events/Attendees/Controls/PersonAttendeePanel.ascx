<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PersonAttendeePanel.ascx.cs" Inherits="InSite.Admin.Events.Attendees.Controls.PersonAttendeePanel" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="UserIdentifier">
    <Columns>
                    
        <asp:TemplateField HeaderText="Contact">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("UserFullName") %>'
                    NavigateUrl='<%# Eval("UserIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>' />
                <span class="form-text">
                    <%# Eval("UserCode") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>
                    
        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <a href="mailto:<%# Eval("UserEmail") %>"><%# Eval("UserEmail") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Role" ItemStyle-Width="200px">
            <ItemTemplate>
                <%# Eval("AttendeeRole") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assigned" ItemStyle-Width="150px">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Assigned")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="40px">
            <ItemTemplate>
                <insite:IconLink runat="server" Visible="<%# CanWrite %>" Name="trash-alt" Type="Regular" ToolTip="Delete Attendee"
                    NavigateUrl='<%# string.Format("/ui/admin/events/attendees/delete?event={0}&contact={1}", Eval("EventIdentifier"),Eval("UserIdentifier")) %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
