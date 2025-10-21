<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentCredentialList.ascx.cs" Inherits="InSite.UI.Admin.Records.Reports.Controls.RecentCredentialList" %>

<asp:Repeater runat="server" ID="CredentialRepeater">    
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
		<dt>
			<a href='/ui/admin/records/credentials/outline?id=<%# Eval("CredentialIdentifier") %>'><i class="far fa-award me-2"></i><%# Eval("AchievementTitle") %></a>
		</dt>
	    <dd class="ms-1 ps-3 pb-3">
			<div><%# Eval("LastChangeTimestamp") %></div>
	    </dd>
    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>
