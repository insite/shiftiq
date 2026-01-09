<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Achievements.Credentials.Controls.SearchResults" %>

<insite:Grid runat="server" ID="Grid" DataKeyNames="AchievementIdentifier,CredentialIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="100px">
            <ItemTemplate>

                <asp:HyperLink runat="server" Text='<i class="fas fa-pencil"></i>' ToolTip="View Achievement" 
                    NavigateUrl='<%# Eval("CredentialIdentifier", "/ui/admin/records/credentials/outline?id={0}") %>' />

                <asp:HyperLink runat="server" Text='<i class="fas fa-trash-alt"></i>' ToolTip="Delete Achievement"
                    NavigateUrl='<%# Eval("CredentialIdentifier", "/ui/admin/records/credentials/delete?id={0}") %>' />

                <asp:HyperLink runat="server" Text='<i class="fas fa-award"></i>' ToolTip="Download Certificate"
                    NavigateUrl='<%# Eval("CredentialIdentifier", "/ui/portal/records/credentials/certificate?credential={0}") %>' 
                    Visible='<%# IsCredentialDownloadable() %>' />

                <insite:IconButton runat="server" Name="fas fa-award" ToolTip="Download Badge"
                    CommandName="DownloadBadge"
                    Visible='<%# IsBadgeConfigured() %>' />

            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="User">
            <ItemTemplate>
                <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("UserFullName") %></a>
                <div class="form-text">
                    <%# Eval("UserEmail", "<a href='mailto:{0}'>{0}</a>") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <insite:BoundField FieldName="PersonCode" DataField="PersonCode" HeaderText="Person Code" HeaderStyle-Wrap="false" />

        <asp:TemplateField HeaderText="Achievement">
            <ItemTemplate>
                <a href='/ui/admin/records/achievements/outline?<%# Eval("AchievementIdentifier", "id={0}") %>'><%# Eval("AchievementTitle") %></a>
                <div class="form-text ms-3">
                    <%# Eval("AchievementTag") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# Eval("CredentialStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Granted" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime(Eval("CredentialGranted")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Revoked" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# GetLocalTime(Eval("CredentialRevoked")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Expiry" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("CredentialExpiryHtml") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Employer">
            <ItemTemplate>
                <asp:HyperLink runat="server" Text='<%# Eval("EmployerGroupName") %>' ToolTip="Open Employer Edit page"
                    NavigateUrl='<%# Eval("EmployerGroupIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>' />
                <div class="form-text">
                    <%# Eval("EmployerGroupStatus") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Region">
            <ItemTemplate>
                <%# Eval("EmployerGroupRegion") %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>

</insite:Grid>