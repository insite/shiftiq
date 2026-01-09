<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExamEventScheduleResults.ascx.cs" Inherits="InSite.Admin.Events.Reports.Controls.ExamEventScheduleResults" %>

<insite:Grid runat="server" ID="Grid">
    <Columns>

        <asp:TemplateField HeaderText="Venue">
            <ItemTemplate>
                <%# Eval("VenueName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="City">
            <ItemTemplate>
                <%# Eval("PhysicalCity") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Trades">
            <ItemTemplate>
                <%# Eval("Trades") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Exam Type">
            <ItemTemplate>
                <%# Eval("FormBankTypes") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Session ID">
            <ItemTemplate>
                <%# Eval("EventClassCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Exam Format">
            <ItemTemplate>
                <%# Eval("EventFormat") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Exam Date">
            <ItemTemplate>
                <%# GetDateString((DateTimeOffset)Eval("EventScheduledStart")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Schedule Status">
            <ItemTemplate>
                <%# Eval("EventSchedulingStatus") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Candidates">
            <ItemTemplate>
                <%# Eval("CandidateCount") %> of <%# Eval("CapacityMaximum") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Eligibility">
            <ItemTemplate>
                <%# Eval("Eligibility") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invigilators">
            <ItemTemplate>
                <%# Eval("InvigilatorMinimum") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Address">
            <ItemTemplate>
                <%# Eval("PhysicalAddress") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Room">
            <ItemTemplate>
                <%# Eval("VenueRoom") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invigilating Office">
            <ItemTemplate>
                <%# Eval("VenueOffice") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event ID">
            <ItemTemplate>
                <%# Eval("EventNumber") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Accommodations">
            <ItemTemplate>
                <%# GetAccommodationsHtml((Guid)Eval("EventIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
