<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Admin.Contacts.Groups.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="GroupSection" Src="Controls/GroupSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkflowSection" Src="Controls/WorkflowSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="OccupationSection" Src="Controls/OccupationSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="PhotoSection" Src="Controls/PhotoSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="AddressSection" Src="Controls/AddressSection.ascx" %>
<%@ Register TagPrefix="uc" TagName="RoleGrid" Src="Controls/RoleGrid.ascx" %>
<%@ Register TagPrefix="uc" TagName="PermissionGrid" Src="~/UI/Admin/Accounts/Permissions/Controls/PermissionGrid.ascx" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <insite:PageHeadContent runat="server">
        <link href="/UI/Admin/Workflow/Cases/Outline.css" rel="stylesheet" />
    </insite:PageHeadContent>
    
    <insite:Alert runat="server" ID="StatusAlert" />

	<section class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Group</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:GroupSection runat="server" ID="GroupSection" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Workflow</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:WorkflowSection runat="server" ID="WorkflowSection" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Occupations</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:OccupationSection runat="server" ID="OccupationSection" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Photos</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:PhotoSection runat="server" ID="PhotoSection" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Addresses</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:AddressSection runat="server" ID="AddressSection" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section runat="server" id="RolesSection" class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">People</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:RoleGrid runat="server" ID="RoleGrid" />
					</div>
				</div>

			</div>        
		</div>

	</section>

	<section runat="server" id="PermissionSection" class="pb-5 mb-md-2">

		<div class="row">
			<div class="col-lg-12">

				<h2 class="h4 mb-3">Permissions</h2>

				<div class="card border-0 shadow-lg">
					<div class="card-body">
						<uc:PermissionGrid runat="server" ID="PermissionGrid" />
					</div>
				</div>

			</div>        
		</div>

	</section>

</asp:Content>
