<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RegistrationGrid.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.RegistrationGrid" %>

<insite:Alert runat="server" ID="StatusAlert" />

<div class="d-flex justify-content-between mb-2">
    <div class="d-flex align-items-center">
        <insite:TextBox runat="server" ID="FilterTextBox" Width="300" EmptyMessage="Filter" />
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

    <div class="d-flex gap-1 align-items-center">
        <insite:MultiComboBox ID="ApprovalStatusComboBox" runat="server"
            Multiple-CountPluralFormat="{0} Approval Statuses"
            Multiple-CountAllFormat="All Approval Statuses"
            Multiple-ActionsBox="true"
            Width="250"
            EmptyMessage="All Approval Statuses"
        />
        <insite:MultiComboBox ID="AttendanceStatusComboBox" runat="server"
            Multiple-CountPluralFormat="{0} Attendance Statuses"
            Multiple-CountAllFormat="All Attendance Statuses"
            Multiple-ActionsBox="true"
            Width="250"
            EmptyMessage="All Attendance Statuses"
        />
        <insite:Button runat="server" ID="AddButton" Text="Add Registration(s)" Icon="fas fa-plus-circle" ButtonStyle="OutlineSuccess" />
        <insite:DropDownButton runat="server" ID="DownloadDropDown" MenuCssClass="dropdown-menu-end" ButtonStyle="OutlinePrimary" />
    </div>
</div>

<insite:Grid runat="server" ID="Grid" DataKeyNames="RegistrationIdentifier">
    <Columns>
        <asp:TemplateField HeaderText="#" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# Eval("RegistrationSequence") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Registered" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("RegistrationRequestedOn")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Participant">
            <ItemTemplate>
                <a runat="server" href='<%# Eval("CandidateIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>' visible='<%# Eval("HasPerson") %>'><%# Eval("UserFullName") %></a>
                <asp:Literal runat="server" Text='<%# Eval("UserFullName") %>' Visible='<%# !(bool)Eval("HasPerson") %>' />
                <asp:Literal runat="server" Text="Not Found" Visible='<%# Eval("UserFullName") == null %>' />
                <div class="form-text">
                    <%# Eval("PersonCode") %>
                </div>
                <div class="form-text">
                    <%# Eval("OccupationInterest") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <insite:TemplateField FieldName="AssessmentForm" HeaderText="Assessment">
            <ItemTemplate>
                <insite:FindBankForm runat="server" ID="FormIdentifier" Visible='<%# Eval("FormIdentifier") == null %>' />
                <div runat="server" visible='<%# Eval("FormIdentifier") != null %>'>
                    <%# Eval("FormTitle") %>
                    <div class="form-text">
                        Name:
                        <%# Eval("FormName") %>
                    </div>
                    <div>
                        <insite:IconButton runat="server" ID="CancelAssessmentButton"
                            ToolTip="Cancel Assessment"
                            CommandName="DeleteForm"
                            Name="ban"
                            ConfirmText="Are you sure you want to cancel the assessment form selection for this candidate?"
                        />
                    </div>
                </div>
            </ItemTemplate>
        </insite:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <div>
                    <%# Eval("ApprovalStatus") %>
                </div>
                <span runat="server" visible='<%# (string)Eval("ApprovalStatus") == "Invitation Sent" %>'>
                    on <%# GetInvitationSentTime() %>
                </span>
                <div runat="server" visible='<%# (string)Eval("ApprovalStatus") == "Registered" && ShowForms %>'>
                    <div class="text-nowrap form-text text-body-secondary">
                        Password: <%# Eval("Password") %>
                    </div>
                </div>
                <%# Eval("EligibilityStatusHtml") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Attendance">
            <ItemTemplate>
                <%# Eval("AttendanceStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Fee" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end">
            <ItemTemplate>
                <div runat="server" visible='<%# (string)Eval("ApprovalStatus") != "Moved" %>'>
                    <div runat="server" visible='<%# Eval("PaymentStatus") != null %>' class="badge bg-success">
                        <%# Eval("PaymentStatus") %>
                    </div>

                    <div class="text-nowrap">
                        <%# Eval("RegistrationFee", "{0:c2}") %>
                    </div>
                </div>

                <div runat="server" visible='<%# (string)Eval("ApprovalStatus") == "Moved" && Eval("PaymentStatus") != null %>'>
                    <div class="badge bg-warning">
                        Moved
                    </div>

                    <div class="text-nowrap">
                        <%# Eval("RegistrationFee", "({0:c2})") %>
                    </div>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <insite:TemplateField FieldName="Employer">
            <ItemTemplate>
                <a runat="server" href='<%# Eval("EmployerIdentifier", "/ui/admin/contacts/groups/edit?contact={0}") %>'>
                    <%# Eval("EmployerName") %>
                </a>
                <div runat="server" visible='<%# Eval("BillingCode") != null %>' class="fs-sm">
                    <span class="me-1">Bill To:</span><span style="white-space:pre-wrap;"><%# Eval("BillingCode") %></span>
                </div>
            </ItemTemplate>
        </insite:TemplateField>

        <insite:TemplateField FieldName="AccommodationsForm" HeaderText="Accommodations">
            <ItemTemplate>
                 <%# Eval("Accommodations") %>
            </ItemTemplate>
        </insite:TemplateField>

        <asp:TemplateField ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
            <HeaderTemplate>
                T2202
                <div>
                    <asp:CheckBox runat="server" ID="AllT2202" onclick="if (!allT2202_click(event)) return false;" />
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:CheckBox runat="server" ID="T2202" Checked='<%# Eval("IncludeInT2202") %>' Enabled='<%# Eval("HasPerson") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="110px" ItemStyle-CssClass="text-end" ItemStyle-Wrap="false">
            <ItemTemplate>

                <insite:IconButton runat="server" ID="SendInviteButton" CommandName="SendInvite"
                    Name="paper-plane" Type="Regular" ToolTip="Send Invitation"
                    OnClientClick="return confirm('An invitation to complete this registration will be sent to the person who added the individual to the waitlist. Do you want to proceed?');" />

                <insite:IconLink runat="server" ID="CompleteRegisterLink" Name="plus-circle" Type="Regular"
                    NavigateUrl='<%# string.Format("/ui/portal/events/classes/register?event={0}&candidate={1}&adminoutline=1", Eval("EventIdentifier"), Eval("CandidateIdentifier")) %>'
                    ToolTip='Complete Registration'/>
                <insite:IconLink runat="server" ID="CardLink" Name="id-card" Type="Regular"
                    NavigateUrl='<%# GetRedirectUrl(string.Format("/ui/admin/registrations/classes/card?id={0}", Eval("RegistrationIdentifier"))) %>'
                    ToolTip='Registration Card' />
                <insite:IconLink runat="server" ID="EditLink" Name="pencil" Type="Regular" ToolTip='Edit Registration' />
                <insite:IconLink runat="server" ID="VoidItemButton" Name="trash-alt" ToolTip="Delete Registration"
                    NavigateUrl='<%# string.Format("/ui/admin/registrations/classes/delete?id={0}&event={1}", Eval("RegistrationIdentifier"),Eval("EventIdentifier")) %>' />

            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>

<insite:PageFooterContent runat="server">
    <script>
        function allT2202_click(e) {
            const message = e.target.checked
                ? "Are you sure all of these registrations qualify for a T2202 tax credit?"
                : "Are you sure none of these registrations qualify for a T2202 tax credit?";

            return confirm(message);
        }
    </script>
</insite:PageFooterContent>
