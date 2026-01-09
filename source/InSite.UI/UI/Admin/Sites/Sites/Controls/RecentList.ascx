<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.UI.Admin.Sites.Sites.Controls.RecentList" %>

<asp:Repeater runat="server" ID="Repeater"> 
    
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
        
			<dt>
				<a href='<%# GetEditUrl() %>'><i class="far <%# GetIconClass() %> me-1"></i><%# Eval("Title") %></a>
			</dt>
	        <dd class="ms-1 ps-3 pb-3">
				<div>
					<%# GetTimestampHtml((string)Eval("Type")) %>
				</div>
	        </dd>

    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>

</asp:Repeater>