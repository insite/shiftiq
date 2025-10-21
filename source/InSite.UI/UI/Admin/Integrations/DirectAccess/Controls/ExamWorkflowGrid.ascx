<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamWorkflowGrid.ascx.cs" Inherits="InSite.UI.Desktops.Custom.SkilledTradesBC.Individuals.Controls.ExamWorkflowGrid" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Event #">
            <itemtemplate>
                <%# Eval("Registration.Event.EventType") %> <%# Eval("Registration.Event.EventNumber") %>-<%# Eval("Registration.Event.EventBillingCode") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Event Class" DataField="Event.EventClassCode" />

        <asp:TemplateField HeaderText="Event Scheduled">
            <itemtemplate>
                <insite:Container runat="server" Visible='<%# Eval("Registration.Event") != null %>'>
                    <span class="form-text">Start:</span>
                    <div style="white-space: nowrap;"><%# Eval("Registration.Event.EventScheduledStart", "{0:MMM d, yyyy}") %> at <%# Eval("Registration.Event.EventScheduledStart", "{0:h:mm tt}") %></div>
                    <span class="form-text">End:</span>
                    <div style="white-space: nowrap;"><%# Eval("Registration.Event.EventScheduledEnd", "{0:MMM d, yyyy}") %> at <%# Eval("Registration.Event.EventScheduledEnd", "{0:h:mm tt}") %></div>
                </insite:Container>
            </itemtemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Form Title">
            <itemtemplate>
                <%# Eval("Form.FormCode") %>: <%# Eval("Form.FormTitle") %>
            </itemtemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Registration Status" DataField="RegistrationStatus" />
        <asp:BoundField HeaderText="Verification Errors" DataField="VerificationErrors" />
        <asp:BoundField HeaderText="Attempt Score" DataField="AttemptScore" DataFormatString="{0:n0}" ItemStyle-CssClass="text-end" />

    </Columns>
</insite:Grid>
