<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MembershipGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.MembershipGrid" %>

<div class="row mb-3">
    <div class="col-lg-6">
        <div class="w-75 hstack">
            <insite:TextBox runat="server" ID="FilterTextBox" EmptyMessage="Filter" />
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

    <div runat="server" id="CommandsSection" class="col-lg-6 text-end pe-3">
        <insite:Button runat="server" ID="AddButton" ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Add Group" />
    </div>
</div>

<div class="mt-3">
    <insite:Grid runat="server" ID="Grid" DataKeyNames="GroupIdentifier" EmptyDataText='Select a group and click "Add" to assign this person.'>
        <Columns>

            <insite:TemplateField FieldName="Commands" ItemStyle-Width="60px" ItemStyle-CssClass="text-nowrap text-end">
                <itemtemplate>
                    <insite:IconLink runat="server" ID="EditButton"
                        Name="pencil"
                        NavigateUrl='<%# GetEditUrl() %>'
                        ToolTip="Edit" />
                    <insite:IconLink runat="server" ID="DeleteButton"
                        Name="trash-alt"
                        NavigateUrl='<%# GetDeleteUrl() %>'
                        ToolTip="Delete" />
                    <insite:IconLink runat="server" ID="HistoryLink" Name="history" ToolTip="History" />
                </itemtemplate>
            </insite:TemplateField>

            <asp:TemplateField HeaderText="Type">
                <itemtemplate>
                    <%# Eval("GroupType") %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Name" SortExpression="GroupName">
                <itemtemplate>
                    <asp:HyperLink runat="server" ID="GroupEditorLink" />
                    <asp:Literal runat="server" ID="GroupName" />
                    <asp:Literal runat="server" ID="OrganizationName" />
                    <asp:Literal runat="server" ID="GroupParent" />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Function">
                <itemtemplate>
                    <%# Eval("RoleType") %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Effective" ItemStyle-Wrap="false">
                <itemtemplate>
                    <%# LocalizeDate(Eval("Assigned")) %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Expiry" ItemStyle-Wrap="false">
                <itemtemplate>
                    <%# LocalizeDate(Eval("MembershipExpiry")) %>
                </itemtemplate>
            </asp:TemplateField>

        </Columns>
    </insite:Grid>
</div>

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
