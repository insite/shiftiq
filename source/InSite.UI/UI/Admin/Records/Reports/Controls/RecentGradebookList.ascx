<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentGradebookList.ascx.cs" Inherits="InSite.UI.Admin.Records.Reports.Controls.RecentGradebookList" %>

<asp:Repeater runat="server" ID="GradebookRepeater">    
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
		<dt>
			<a href='/ui/admin/records/gradebooks/outline?id=<%# Eval("GradebookIdentifier") %>'><i class="far fa-spell-check me-2"></i><%# Eval("GradebookTitle") %></a>
		</dt>
	    <dd class="ms-1 ps-3 pb-3">
			<div><%# Eval("LastChangeTimestamp") %></div>
			<%# GetAchievementHtml((Guid?)Eval("AchievementIdentifier")) %>
	    </dd>
    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>
