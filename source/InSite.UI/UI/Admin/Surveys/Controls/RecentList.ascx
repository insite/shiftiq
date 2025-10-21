<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.UI.Admin.Surveys.Controls.RecentList" %>
   
<asp:Repeater runat="server" ID="SurveyRepeater">    
	<HeaderTemplate>
		<dl>
	</HeaderTemplate>
    <ItemTemplate>
        
			<dt>
				<a href='/ui/admin/surveys/forms/outline?survey=<%# Eval("SurveyFormIdentifier") %>'><i class="far fa-check-square me-1"></i><%# Eval("SurveyFormName") %></a>
			</dt>
	        <dd class="ms-1 ps-3 pb-3">
				<div><asp:Literal runat="server" ID="LastChange" /></div>
				<a href="/admin/surveys/forms/report?survey=<%# Eval("SurveyFormIdentifier") %>" class="me-3"><i class="far fa-fw fa-chart-bar me-1"></i>Response Analysis</a>
				<asp:Literal runat="server" ID="ResponseAnalysisLink" />
	        </dd>

    </ItemTemplate>
	<FooterTemplate>
		</dl>
	</FooterTemplate>
</asp:Repeater>