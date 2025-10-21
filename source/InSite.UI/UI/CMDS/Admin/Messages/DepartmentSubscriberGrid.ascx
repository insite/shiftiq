<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentSubscriberGrid.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Messages.DepartmentSubscriberGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="ContactIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Subscribed" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="Subscribed" Checked='<%# Eval("IsSubscribed") %>' AutoPostBack="true" />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Name" DataField="FullName" ItemStyle-Wrap="false" />

        <asp:TemplateField HeaderText="Email">
            <ItemTemplate>
                <span <%# (bool)Eval("EmailEnabled") ? "" : "class='text-decoration-line-through' title='Email Notification Disabled'" %>>
                    <a href="mailto:<%# Eval("Email") %>">
                        <%# Eval("Email") %>
                    </a>
                </span>
                <div class="form-text"><%# Eval("FollowerEmails") != DBNull.Value ? Eval("FollowerEmails") : "" %></div>
            </ItemTemplate>
        </asp:TemplateField>
                    
        <asp:TemplateField HeaderText="Subscribed" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Subscribed") as DateTimeOffset?) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Last Notified" HeaderStyle-Wrap="False" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Delivered") as DateTimeOffset?) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
