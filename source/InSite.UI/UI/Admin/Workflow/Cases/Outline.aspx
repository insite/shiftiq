<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Issues.Outlines.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register TagPrefix="uc" TagName="CaseSection" Src="./Controls/CaseSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentsSection" Src="./Controls/CommentsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="AttachmentsSection" Src="./Controls/AttachmentsSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="SurveySection" Src="./Controls/FormSection.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:PageHeadContent runat="server">
        <link href="/UI/Admin/Workflow/Cases/Outline.css" rel="stylesheet" />
    </insite:PageHeadContent>
    
    <insite:Alert runat="server" ID="StatusAlert" />

	<insite:Nav runat="server" ID="NavPanel">

		<insite:NavItem runat="server" ID="IssueNavItem" Title="Details" Icon="fas fa-info">
			
			<section class="pb-5 mb-md-2">
				<uc:CaseSection runat="server" ID="CaseSection" />
			</section>

		</insite:NavItem>

		<insite:NavItem runat="server" ID="CommentNavItem" Title="Comments" Icon="fas fa-comments">
			
			<section class="pb-5 mb-md-2">
				<uc:CommentsSection runat="server" ID="CommentsSection" />
			</section>

		</insite:NavItem>

		<insite:NavItem runat="server" ID="AttachmentNavItem" Title="Attachments" Icon="fas fa-paperclip">
			
			<section class="pb-5 mb-md-2">
				<uc:AttachmentsSection runat="server" ID="AttachmentsSection" />
			</section>

		</insite:NavItem>

		<insite:NavItem runat="server" ID="SurveyNavItem" Title="Forms" Icon="fas fa-check-square" Visible="false">
			
			<section class="pb-5 mb-md-2">
				<uc:SurveySection runat="server" ID="SurveySection" />
			</section>

		</insite:NavItem>

	</insite:Nav>

</asp:Content>
