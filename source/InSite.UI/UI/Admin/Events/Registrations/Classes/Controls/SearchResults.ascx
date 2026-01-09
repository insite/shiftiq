<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField ItemStyle-Width="70px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit Registration'
                    NavigateUrl='<%# Eval("RegistrationIdentifier", "/ui/admin/registrations/classes/edit?id={0}") %>' />
                <insite:IconLink runat="server" Visible="<%# CanWrite %>" Name="trash-alt" Type="Regular" ToolTip="Delete Registration"
                    NavigateUrl='<%# Eval("RegistrationIdentifier", "/ui/admin/registrations/classes/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event">
            <ItemTemplate>
                <a href='/ui/admin/events/<%# Eval("EventTypePlural") %>/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventTitle") %></a>
                <div class="small">
                    <%# GetScheduledTime() %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
          
        <asp:TemplateField HeaderText="Registrant">
            <ItemTemplate>
                <a href="<%# Eval("CandidateIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>"><%# Eval("UserFullName") %></a>
                <br />
                <span class="form-text">
                    <%# Eval("PersonCode") %>
                    <a href="mailto:<%# Eval("Email") %>"><%# Eval("Email") %></a>
                    <%# (bool)Eval("EmailEnabled") ? "" : "<i class='fa fa-exclamation-triangle' style='color:#faaf41;' title='Email Disabled'></i>" %>
                    <%# Eval("Phone") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Registered On/By" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeTime(Eval("RegistrationRequestedOn")) %>
                <div runat="server" visible='<%# Eval("RegistrationRequestedByIdentifier") != null %>' class="form-text">
                    By:
                    <a href='<%# Eval("RegistrationRequestedByIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>'>
                        <%# Eval("RegistrationRequestedByName") %>
                    </a>
                    <a href='<%# Eval("RegistrationRequestedByEmail", "mailto:{0}") %>'>
                        <%# Eval("RegistrationRequestedByEmail") %>
                    </a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Approval">
            <ItemTemplate>
                <%# Eval("ApprovalStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Attendance">
            <ItemTemplate>
                <%# Eval("AttendanceStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Bill To">
            <ItemTemplate>
                <%# Eval("BillingCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Fee" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
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

        <asp:BoundField HeaderText="Include in T2202" DataField="IncludeInT2202" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" />

        <asp:TemplateField HeaderText="Employer at Time of Registration">
            <ItemTemplate>
                <span title="Employer at the time of registration"><%# Eval("EmployerGroupName") %></span>
                <div class="form-text">
                    <%# Eval("EmployerGroupRegion") %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Department" DataField="Department" />

    </Columns>
</insite:Grid>
