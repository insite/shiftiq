<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationGrid.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.RegistrationGrid" %>

<asp:MultiView runat="server" ID="MultiView">

    <asp:View runat="server" ID="NoRecordsView">
        <div class="alert alert-warning" role="alert">
            There are no registrations
        </div>
    </asp:View>

    <asp:View runat="server" ID="GridView">

        <div class="mb-3">
            <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
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

        <insite:Grid runat="server" ID="Grid" Style="margin-top: 15px;" DataKeyNames="RegistrationIdentifier">
            <Columns>

                <insite:TemplateField FieldName="Commands" ItemStyle-Width="60px" ItemStyle-Wrap="false">
                    <ItemTemplate>
                        <insite:IconLink runat="server" ID="RegistrtaionLink" Name="pencil" Type="Regular" ToolTip="Edit Registration" />
                        <insite:IconLink runat="server" ID="DeleteLink" Name="trash-alt" Type="Regular" ToolTip="Delete Registration" />
                    </ItemTemplate>
                </insite:TemplateField>

                <asp:TemplateField HeaderText="Start" ItemStyle-Wrap="False">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("Event.EventScheduledStart")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="End" ItemStyle-Wrap="False">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("Event.EventScheduledEnd")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Event">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="EventLink" Text='<%# Eval("Event.EventTitle") %>' />
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Registered">
                    <ItemTemplate>
                        <%# LocalizeDate(Eval("RegistrationRequestedOn")) %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Approval" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("ApprovalStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Comments">
                    <ItemTemplate>
                        <%# Eval("RegistrationComment") != null ? ((string)Eval("RegistrationComment")).Replace("\n", "<br>") : "" %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Fee" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
                    <ItemTemplate>
                        <%# Eval("RegistrationFee", "{0:n2}") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Attendance" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("AttendanceStatus") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Score" ItemStyle-Width="50px">
                    <ItemTemplate>
                        <%# Eval("Score", "{0:p0}") %>
                    </ItemTemplate>
                </asp:TemplateField>
                

            </Columns>
        </insite:Grid>
    </asp:View>

</asp:MultiView>
