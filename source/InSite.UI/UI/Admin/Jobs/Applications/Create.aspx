<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Jobs.Applications.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

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
        </div>
    </div>

</asp:Content>
