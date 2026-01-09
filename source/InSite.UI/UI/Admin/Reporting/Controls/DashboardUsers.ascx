<%@ Control Language="C#" CodeBehind="DashboardUsers.ascx.cs" Inherits="InSite.UI.Admin.Reporting.Controls.OnlineUsersGrid" %>

<p runat="server" id="EmptyGrid" class="help">There are no online users.</p>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderStyle-Width="30px">
            <ItemTemplate>
                <%# (bool)Eval("IsOnline") ? "<span class='badge bg-success'>Online</span>" : "<span class='badge bg-danger'>Offline</span>" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Started">
            <ItemTemplate>
                <%# Eval("StartedHumanize") %>
                <div class="fs-sm text-body-secondary">
                    <%# Eval("Started") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <%# Eval("UserName") %>
                <span class="fs-sm text-body-secondary">
                    <%# Eval("PersonCode") %>
                </span>
                <div class="fs-sm text-body-secondary">
                    <a href="mailto:<%# Eval("UserEmail") %>"><%# Eval("UserEmail") %></a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Organization">
            <ItemTemplate>
                <%# Eval("OrganizationName") %>
                <div class="fs-sm text-body-secondary"><%# Eval("OrganizationCode") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Browser">
            <ItemTemplate>
                <%# Eval("Browser") %>
                <div class="fs-sm text-body-secondary">version <%# Eval("BrowserVersion") %></div>
            </ItemTemplate>
        </asp:TemplateField>


    </Columns>
</insite:Grid>

