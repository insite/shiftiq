<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FollowerSubscriberGrid.ascx.cs" Inherits="InSite.Custom.CMDS.Admin.Messages.FollowerSubscriberGrid" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="ContactIdentifier">
    <Columns>

        <asp:TemplateField HeaderText="Following" ItemStyle-Width="80" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="Following" Checked='<%# Eval("Following") %>' Enabled='<%# Eval("IsAttached") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Subscriber Name" DataField="FullName" />
            
        <asp:TemplateField HeaderText="Subscriber Email">
            <ItemTemplate>
                <a href="mailto:<%# Eval("Email") %>">
                    <%# Eval("Email") %>
                </a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Subscribed" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("Attached") as DateTimeOffset?) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
