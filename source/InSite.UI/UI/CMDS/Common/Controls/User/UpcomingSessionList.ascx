<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UpcomingSessionList.ascx.cs" Inherits="InSite.Custom.CMDS.User.Programs.Controls.UpcomingSessionList" %>

<ul class="list-group list-group-flush">
    <asp:Repeater runat="server" ID="EventRepeater">
        <ItemTemplate>
            <li class="list-group-item">
                <a href="<%# Eval("Href") %>"><%# Eval("Title") %></a>
                <div class="mt-1"><%# Eval("Badge") %></div>
            </li>
        </ItemTemplate>
    </asp:Repeater>
    <li runat="server" id="EventRegistrationItem" class="list-group-item">
        <div class="mb-2">
            <strong>Event Registration:</strong>
            You can view a full listing of training sessions and other events on the schedule.
        </div>
        <div><a href="/ui/portal/events/classes/search" class="btn btn-primary"><i class="fas fa-calendar me-2"></i>Class Schedule</a></div>
    </li>
</ul>
