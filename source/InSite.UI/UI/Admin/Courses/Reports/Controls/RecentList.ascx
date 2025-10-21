<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RecentList.ascx.cs" Inherits="InSite.UI.Admin.Courses.Reports.Controls.RecentList" %>

<asp:Repeater runat="server" ID="Course2Repeater">
	<HeaderTemplate><dl></HeaderTemplate>
    <ItemTemplate>
		<dt>
			<a href='/ui/admin/courses/manage?course=<%# Eval("CourseIdentifier") %>'><i class="far fa-chalkboard-teacher me-1"></i><%# Eval("CourseName") %></a>
		</dt>
	    <dd class="ms-1 ps-3 pb-3">
			<div><%# Eval("LastChangeTimestamp") %></div>
	    </dd>
    </ItemTemplate>
	<FooterTemplate></dl></FooterTemplate>
</asp:Repeater>