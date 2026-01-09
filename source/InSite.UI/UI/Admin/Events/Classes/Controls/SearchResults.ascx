<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Classes.Controls.SearchResults" %>

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
            
        <asp:TemplateField HeaderText="Scheduled" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetLocalTime("EventScheduledStart") %><br /><%# GetLocalTime("EventScheduledEnd") %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Duration" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# GetDurationHtml() %>
                <div class="form-text">
                    <%# Eval("CreditHours") != null ? Eval("CreditHours", "Credits: {0:n2}"): "" %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Title">
            <ItemTemplate>
                <div>
                    <a href='/ui/admin/events/classes/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("EventTitle") %></a>
                    <span runat="server" visible='<%# Eval("RegistrationLocked") != null %>' title="Hold open seats">
                        <i class="fas fa-lock text-danger"></i>
                    </span>
                </div>
                <div class="form-text"><%# Eval("EventDescription") %></div>
                <div class="form-text">
                    <%# GetInstructors() != null ? "Instructor(s): " + GetInstructors() : "" %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Venue" HeaderStyle-Wrap="False">
            <ItemTemplate>
                <%# Eval("VenueLocation.GroupName") %>
                <div class="form-text">
                    <%# GetVenueAddress() %>
                </div>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Status" HeaderStyle-Wrap="False">
            <ItemTemplate>
                <%# GetStatus() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Capacity" ItemStyle-Wrap="False" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
            <ItemTemplate>
                <%# GetCapacityHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Registrations" ItemStyle-Wrap="False" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
            <ItemTemplate>
                <%# GetRegistrationsHtml() %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Group Permissions">
            <ItemTemplate>
                <%# GetGroupPermissions() %>
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>