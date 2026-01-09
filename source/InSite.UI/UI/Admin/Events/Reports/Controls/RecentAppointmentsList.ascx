<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentAppointmentsList.ascx.cs" Inherits="InSite.UI.Admin.Events.Reports.Controls.RecentAppointmentsList" %>

<asp:Repeater runat="server" ID="EventRepeater">    
	<HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
        
		<dt>
			<a href='/ui/admin/events/appointments/outline?event=<%# Eval("EventIdentifier") %>'><i class="far fa-calendar-alt me-1"></i><%# Eval("EventTitle") %></a>
		</dt>
	    <dd class="ms-1 ps-3 pb-3">
			<div>
				<p class="card-text">Scheduled <%# Eval("EventTime") %></p>
				<%# Eval("LastChangeTimestamp") %>
			</div>
	    </dd>

    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>