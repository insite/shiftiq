<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.Admin.Assessments.Reports.Controls.RecentList" %>

<asp:Repeater runat="server" ID="BankRepeater">
    <HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
        
		<dt>
			<a href='/ui/admin/assessments/banks/outline?bank=<%# Eval("BankIdentifier") %>'><i class="far fa-balance-scale me-1"></i><%# Eval("BankName") %></a>
		</dt>
	    <dd class="ms-1 ps-3 pb-3">
			<div>
				<div><asp:Literal runat="server" ID="LastChange" /></div>
			</div>
	    </dd>

    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>
