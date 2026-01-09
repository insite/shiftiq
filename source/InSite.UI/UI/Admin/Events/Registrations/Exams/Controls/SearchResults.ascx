<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Registrations.Exams.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="Event Start" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("EventScheduledStart")) %>
            </ItemTemplate>
        </asp:TemplateField>
           
        <asp:TemplateField HeaderText="Event Number">
            <ItemTemplate>
                <%# Eval("EventNumber") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Title">
            <ItemTemplate>
                <a href='/ui/admin/events/<%# Eval("EventTypePlural") %>/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Venue" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <strong><%# Eval("EventVenueLocation") %></strong>
                <div class="form-text no-wrap"><%# Eval("EventVenueRoom") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invigilating Office">
            <ItemTemplate>
                <%# Eval("EventVenueOffice") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Assessment Form Code" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# (string)Eval("AssessmentFormCode") %>
            </ItemTemplate>
        </asp:TemplateField>
        
        <asp:TemplateField HeaderText="Assessment Form Name" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# (string)Eval("AssessmentFormName") %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Learner">
            <ItemTemplate>
                <a href="<%# Eval("LearnerIdentifier", "/ui/admin/contacts/people/edit?contact={0}") %>"><%# Eval("LearnerName") %></a>
                    <br />
                    <span class="form-text">
                        <%# Eval("LearnerCode") %>
                        <a href="mailto:<%# Eval("LearnerEmail") %>"><%# Eval("LearnerEmail") %></a>
                        <%# (bool)Eval("LearnerEmailEnabled") ? "" : "<i class='fa fa-exclamation-triangle' style='color:#faaf41;' title='Email Disabled'></i>" %>
                    </span>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Registration Type">
            <ItemTemplate>
                <%# Eval("RegistrationType") %>
            </ItemTemplate>
        </asp:TemplateField>
          
        <asp:TemplateField HeaderText="Registered" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeDate(Eval("RegistrationRequestedOn")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Approval Status">
            <ItemTemplate>
                <%# Eval("ApprovalStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Attendance Status">
            <ItemTemplate>
                <%# Eval("AttendanceStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderStyle-Width="60px" ItemStyle-Wrap="false">
            <ItemTemplate>
                <insite:IconLink runat="server" ID="RegistrationLink" Name="pencil" Type="Regular" ToolTip='Edit Registration' />
                <insite:IconLink runat="server" Visible="<%# CanWrite %>" Name="trash-alt" Type="Regular" ToolTip="Delete Registration"
                    NavigateUrl='<%# Eval("RegistrationIdentifier", "/ui/admin/registrations/exams/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>