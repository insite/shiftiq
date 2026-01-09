<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.UI.Portal.Records.Credentials.Instructors.Controls.SearchResults" %>

<insite:Literal runat="server" ID="Instructions" />

<div class="table-responsive">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="OrganizationIdentifier" Translation="Header">
        <Columns>

            <asp:TemplateField HeaderText="Granted" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
                <ItemTemplate>
                      <%# GetDateString(Eval("CredentialGranted") as DateTimeOffset?) %>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="User">
                <ItemTemplate>
                    <%# Eval("UserFullName") %>
                    <div class="form-text">
                        <%# Eval("UserEmail") %>
                    </div>
                    <div class="form-text">
                        <%# Eval("EmployerGroupName") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Achievement">
                <ItemTemplate>
                    <%# Eval("AchievementTitle") %>
                    <div class="form-text no-wrap">
                        <%# Eval("AchievementLabel") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <%# Translate((string)Eval("CredentialStatus")) %>
                    <div class="form-text no-wrap">
                        <%# Eval("CredentialGrantedScore", "{0:p0}") %>
                    </div>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Expiry">
                <ItemTemplate>
                    <%# GetCredentialExpiry(Container.DataItem) %>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField ItemStyle-Width="60px" ItemStyle-Wrap="false">
                <ItemTemplate>

                    <%# Eval("DownloadLink") %>

                    <asp:LinkButton runat="server"
                        CommandName="Expire"
                        CommandArgument='<%# Eval("CredentialIdentifier") %>'
                        OnClientClick="return confirm('Are you sure you want to expire this credential?');"
                        ToolTip="Expire Now"
                        Visible='<%# (string)Eval("CredentialStatus") == "Valid" %>'><i class="fa-solid fa-alarm-clock"></i></asp:LinkButton>

                </ItemTemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>