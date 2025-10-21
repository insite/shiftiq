<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CredentialGrid.ascx.cs" Inherits="InSite.Admin.Records.Programs.Controls.CredentialGrid" %>

<div class="row mb-1">
    <div class="col-lg-4">
        <insite:ComboBox runat="server" ID="AchievementType" Width="300">
            <Items>
                <insite:ComboBoxOption Value="Program" Text="Program Achievements Only" Selected="true" />
                <insite:ComboBoxOption Value="Learner" Text="All Learner Achievements" />
            </Items>
        </insite:ComboBox>
    </div>
</div>

<div class="row mb-3">
    <div class="col-lg-4">
        <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Learner" CssClass="d-inline-block" />
        <insite:IconButton runat="server" ID="FilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
        <insite:PageFooterContent runat="server">
            <script type="text/javascript"> 
                (function () {
                    Sys.Application.add_load(function () {
                        $('#<%= FilterTextBox.ClientID %>')
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
</div>

<div class="row">
    <div class="col-12">
        <insite:Grid runat="server" ID="Grid" DataKeyNames="AchievementIdentifier">

            <Columns>

                <asp:TemplateField HeaderText="Learner">
                    <itemtemplate>
                        <a href='/ui/admin/contacts/people/edit?<%# Eval("UserIdentifier", "contact={0}") %>'><%# Eval("UserFullName") %></a>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Achievement">
                    <itemtemplate>
                        <a href='/ui/admin/records/achievements/outline?<%# Eval("AchievementIdentifier", "id={0}") %>'><%# Eval("AchievementTitle") %></a>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Status">
                    <itemtemplate>
                        <%# Eval("CredentialStatus") %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Granted" ItemStyle-Wrap="false">
                    <itemtemplate>
                        <%# LocalizeDate(Eval("CredentialGranted")) %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Revoked" ItemStyle-Wrap="false">
                    <itemtemplate>
                        <%# LocalizeDate(Eval("CredentialRevoked")) %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Expired" ItemStyle-Wrap="false">
                    <itemtemplate>
                        <%# LocalizeDate(Eval("CredentialExpired")) %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Email">
                    <itemtemplate>
                        <%# Eval("UserEmail") %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Employer">
                    <itemtemplate>
                        <%# Eval("EmployerGroupName") %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Region">
                    <itemtemplate>
                        <%# Eval("UserRegion") %>
                    </itemtemplate>
                </asp:TemplateField>

                <asp:TemplateField ItemStyle-Width="20px" ItemStyle-Wrap="false">
                    <itemtemplate>
                        <insite:IconLink runat="server" Name="search" ToolTip="Outline"
                            NavigateUrl='<%# Eval("CredentialIdentifier", "/ui/admin/records/credentials/outline?id={0}") %>' />
                        <insite:IconLink runat="server" Name="trash-alt" ToolTip="Delete"
                            NavigateUrl='<%# string.Format("/ui/admin/records/credentials/delete?id={0}&return={1}", Eval("CredentialIdentifier"), HttpUtility.UrlEncode(Page.Request.RawUrl)) %>' />
                    </itemtemplate>
                </asp:TemplateField>

            </Columns>

        </insite:Grid>
    </div>
</div>