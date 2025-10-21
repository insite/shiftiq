<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Exams.Controls.SearchResults" %>

<style type="text/css">
    .dot {
        float: left;
        height: 12px;
        width: 12px;
        border-radius: 50%;
        display: inline-block;
        margin: 5px;
    }
</style>

<asp:Literal id="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="EventIdentifier">
    <Columns>
            
        <asp:TemplateField HeaderText="Class/Session" HeaderStyle-Wrap="false" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# Eval("EventClassCode") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled Date" ItemStyle-Wrap="false">
            <ItemTemplate>
                <%# LocalizeTime(Eval("EventScheduledStart")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Format" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetEventFormat((string)Eval("EventFormat")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <div>
                    <a href='/ui/admin/events/exams/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventTitle") %></a>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Form Name" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <%# GetFormName((string)Eval("ExamForms")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Invigilating Office">
            <ItemTemplate>
                <%# Eval("VenueOfficeName") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Venue/Location" HeaderStyle-Wrap="false">
            <ItemTemplate>
                <strong><%# Eval("VenueLocationName") %></strong>
                <div class="form-text no-wrap"><%# Eval("VenueRoom") %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Capacity" ItemStyle-Wrap="False" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
            <ItemTemplate>
                <%# GetCandidateLimit((int)Eval("RegistrationCount"), (int?)Eval("CapacityMaximum")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Invigilators" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
            <ItemTemplate>
                <%# Eval("InvigilatorMinimum") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event #" HeaderStyle-Wrap="False" HeaderStyle-HorizontalAlign="Center" ItemStyle-Wrap="False" ItemStyle-HorizontalAlign="Center" >
            <ItemTemplate>
                <a href='/ui/admin/events/exams/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventNumber") %>-<%# Eval("EventBillingType") %></a> 
            </ItemTemplate>
        </asp:TemplateField>

        <asp:BoundField HeaderText="Request Status" DataField="EventRequisitionStatus" />
        <asp:BoundField HeaderText="Scheduling Status" DataField="EventSchedulingStatus" />
        <asp:BoundField HeaderText="Material Tracking" DataField="ExamMaterialReturnShipmentCondition" />

        <asp:TemplateField HeaderText="Accommodation">
            <ItemTemplate>
                <%# GetAccommodationsHtml((Guid)Eval("EventIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Grading">
            <ItemTemplate>
                <%# GetGradingStatusHtml((Guid)Eval("EventIdentifier")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>