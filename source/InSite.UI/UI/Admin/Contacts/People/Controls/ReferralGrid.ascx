<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReferralGrid.ascx.cs" Inherits="InSite.UI.Admin.Contacts.People.Controls.ReferralGrid" %>

<div class="row mb-3">
    <div class="col-lg-6">
        <div runat="server" id="FilterField" class="w-75 hstack">
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
        <insite:Button runat="server" ID="AddButton" ButtonStyle="OutlinePrimary" Icon="fas fa-plus-circle" Text="Add Referral" />
    </div>
</div>

<div class="mt-3">
    <insite:Alert runat="server" ID="EmptyGrid" Indicator="Information" Text="There are no referrals to display." Visible="false" />

    <insite:Grid runat="server" ID="Grid">
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
                    <%# Eval("ReasonType") %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Subtype">
                <itemtemplate>
                    <%# Eval("ReasonSubtype") %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Name">
                <itemtemplate>
                    <asp:HyperLink runat="server" ID="GroupEditorLink" />
                    <asp:Literal runat="server" ID="GroupName" />
                    <asp:Literal runat="server" ID="OrganizationName" />
                    <asp:Literal runat="server" ID="GroupParent" />
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Occupation">
                <itemtemplate>
                    <%# Eval("PersonOccupation") %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Effective" ItemStyle-Wrap="false">
                <itemtemplate>
                    <%# LocalizeDate(Eval("ReasonEffective")) %>
                </itemtemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Expiry" ItemStyle-Wrap="false">
                <itemtemplate>
                    <%# LocalizeDate(Eval("ReasonExpiry")) %>
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
