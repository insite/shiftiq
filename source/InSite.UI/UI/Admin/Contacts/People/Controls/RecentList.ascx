<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.Admin.Contacts.People.Controls.RecentList" %>

<asp:Repeater runat="server" ID="PersonRepeater">  
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
        
			<dt>
				<a href='/ui/admin/contacts/people/edit?contact=<%# Eval("UserIdentifier") %>'><i class="far fa-user me-1"></i><%# Eval("FullName") %></a>
				<asp:HyperLink runat="server" ID="Email" />
			</dt>
	        <dd class="ms-1 ps-3 pb-3">
				<div>
					<asp:Literal runat="server" ID="LastChange" /> <%# GetTimestampHtml(Eval("UserIdentifier")) %>
				</div>
	        </dd>

    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
   
</asp:Repeater>