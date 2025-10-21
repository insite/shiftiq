<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SearchResults.ascx.cs" Inherits="InSite.Admin.Events.Seats.Controls.SearchResults" %>

<asp:Literal ID="Instructions" runat="server" />

<insite:Grid runat="server" ID="Grid">
    <Columns>
            
        <asp:TemplateField HeaderText="Event Start" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Event.EventScheduledStart")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Event End" ItemStyle-Wrap="False">
            <ItemTemplate>
                <%# LocalizeDate(Eval("Event.EventScheduledEnd")) %>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Event Achievement">
            <ItemTemplate>
                <a href='/ui/admin/records/achievements/outline?<%# Eval("Event.AchievementIdentifier", "id={0}") %>'><%# Eval("Event.Achievement.AchievementTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Event Title">
            <ItemTemplate>
                <a href='/ui/admin/events/classes/outline?<%# Eval("EventIdentifier", "event={0}") %>'><%# Eval("Event.EventTitle") %></a>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Seat Name">
            <ItemTemplate>
                <a href="<%# InSite.ReturnUrlHelper.GetRedirectUrl((string)Eval("SeatIdentifier", "/ui/admin/events/seats/edit?id={0}")) %>"><%# Eval("SeatTitle") %></a>
                <p><%# GetDescription(Container.DataItem) %></p>
            </ItemTemplate>
        </asp:TemplateField>
            
        <asp:TemplateField HeaderText="Seat Availability">
            <ItemTemplate>
                <%# (bool)Eval("IsAvailable") ? "Available" : "Hide" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Seat Taxes">
            <ItemTemplate>
                <%# (bool)Eval("IsTaxable") ? "Yes" : "No" %>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Price" HeaderStyle-CssClass="text-end" ItemStyle-CssClass="text-end" >
            <ItemTemplate>
                <asp:Literal runat="server" ID="FreePrice" Text="Free" />
                <asp:Literal runat="server" ID="SinglePrice" />

                <asp:Repeater runat="server" ID="MultiplePrice">
                    <ItemTemplate>
                        <div>
                            <%# Eval("Name") %>: <%# Eval("Amount", "{0:c2}") %>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:TemplateField>

        <asp:TemplateField ItemStyle-Width="60px">
            <ItemTemplate>
                <insite:IconLink runat="server" Name="pencil" Type="Regular" ToolTip='Edit Seat'
                    NavigateUrl='<%# InSite.ReturnUrlHelper.GetRedirectUrl((string)Eval("SeatIdentifier", "/ui/admin/events/seats/edit?id={0}")) %>' />
                <insite:IconLink runat="server" Name="trash-alt" Type="Regular" ToolTip="Delete Seat"
                    NavigateUrl='<%# Eval("SeatIdentifier", "/ui/admin/events/seats/delete?id={0}") %>' />
            </ItemTemplate>
        </asp:TemplateField>

    </Columns>
</insite:Grid>
