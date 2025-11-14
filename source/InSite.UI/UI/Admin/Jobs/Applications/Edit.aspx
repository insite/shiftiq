<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Jobs.Applications.Forms.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="StatusAlert" />

    <div class="row mb-3">

        <div class="col-lg-6">

            <div class="card">

                <div class="card-body">

                    <h4 class="card-title mb-3">Job Application</h4>

                    <uc:Detail runat="server" ID="Detail" />

                </div>
            
            </div>

        </div>

    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Job Application" />
            <insite:CancelButton runat="server" ID="CancelButton" />

			<div class="float-end">
				<insite:DeleteButton runat="server" ID="DeleteButton" ConfirmText="Are you sure you want to delete this job application?" />
			</div>
        </div>
    </div>

</asp:Content>
