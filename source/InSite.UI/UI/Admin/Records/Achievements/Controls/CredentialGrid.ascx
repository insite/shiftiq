<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialGrid.ascx.cs" Inherits="InSite.Admin.Achievements.Achievements.Controls.CredentialGrid" %>

<asp:MultiView runat="server" ID="MultiView">

    <asp:View runat="server" ID="NoRecordsView">
        <div runat="server" id="NoAchievements" class="alert alert-warning" role="alert">
            There are no achievements
        </div>
    </asp:View>

    <asp:View runat="server" ID="GridView">
        <div class="clearfix mb-3">
            <div class="float-start">
    
                <div class="input-group">
                    <div class="col me-1">
                        <insite:TextBox runat="server" ID="FilterText" Width="300" EmptyMessage="Filter" /> 
                    </div>         
                    <span class="d-flex align-items-center">
                        <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
                    </span>     
                </div>
    
                <insite:PageFooterContent runat="server">
                    <script type="text/javascript"> 
                        (function () {
                            Sys.Application.add_load(function () {
                                $('#<%= FilterText.ClientID %>')
                                    .off('keydown', onKeyDown)
                                    .on('keydown', onKeyDown);
                            });

                            function onKeyDown(e) {
                                if (e.which === 13) {
                                    e.preventDefault();
                                    $('#<%= FilterButton.ClientID %>')[0].click();
                                }
                            }
                        })();
                    </script>
                </insite:PageFooterContent>
            </div>

            <div class="float-end">
                <insite:Button runat="server" ID="AddButton" class="btn btn-default" Text="Issue Achievement" Icon="fas fa-plus-circle" ButtonStyle="Success" />
                <insite:DropDownButton runat="server" ID="DownloadDropDown" ButtonStyle="OutlineSecondary" MenuCssClass="dropdown-menu-end" />
            </div>
        </div>

        <insite:Grid runat="server" ID="Grid">
            <Columns>

                <insite:TemplateField ItemStyle-Width="66px" ItemStyle-Wrap="false" FieldName="EditPanel">
                    <ItemTemplate>
                        <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                            NavigateUrl='<%# Eval("CredentialIdentifier", "/ui/admin/records/credentials/outline?id={0}") %>' />
                        <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                            NavigateUrl='<%# string.Format("/ui/admin/records/credentials/delete?id={0}&return={1}", Eval("CredentialIdentifier"), HttpUtility.UrlEncode(Page.Request.RawUrl)) %>' />
                    </ItemTemplate>
                </insite:TemplateField>

                <asp:TemplateField HeaderText="Achievement">
                    <ItemTemplate>
                        <a href='/ui/admin/records/achievements/outline?<%# Eval("AchievementIdentifier", "id={0}") %>'><%# Eval("AchievementTitle") %></a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Learner">
                    <ItemTemplate>
                        <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("UserFullName") %></a>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <%# Eval("CredentialStatus") %>
                        <div class="text-body-secondary fs-xs">
                            <%# Eval("CredentialNecessity") %>
                            <%# Eval("CredentialPriority") %>
                        </div>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Expiration">
                    <ItemTemplate>
                        <%# GetExpiration(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Assigned">
                    <ItemTemplate>
                        <%# GetLocalDate(Eval("CredentialAssigned")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Granted">
                    <ItemTemplate>
                        <%# GetLocalDate(Eval("CredentialGranted")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Expiry">
                    <ItemTemplate>
                        <%# GetCredentialExpiry(Container.DataItem) %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>
        </insite:Grid>
    </asp:View>

</asp:MultiView>