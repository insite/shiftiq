<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Appointments.Controls.SearchResults" %>

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

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid" DataKeyNames="EventIdentifier">
    <Columns>

        <asp:TemplateField ItemStyle-Width="80px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="sitemap" Type="Regular" ToolTip='Outline'
                    NavigateUrl='<%# Eval("EventIdentifier", "/ui/admin/events/appointments/outline?event={0}") %>' />
                <insite:IconLink runat="server" Name="trash-alt" Type="Regular" ToolTip="Delete Appointment"
                    NavigateUrl='<%# Eval("EventIdentifier", "/ui/admin/events/appointments/delete?event={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime(Eval("EventScheduledStart")) %> - <%# GetLocalTime(Eval("EventScheduledEnd")) %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <div>
                    <a href='/ui/admin/events/appointments/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventTitle") %></a>
                </div>
                <div class="form-text ms-2"><%# FormatDescription(Eval("EventDescription")) %></div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Appointment Type">
            <ItemTemplate>
                <%# Eval("AppointmentType") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status">
            <ItemTemplate>
                <%# GetPublicationStatus(Eval("EventPublicationStatus")) %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
