<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Accounts.Permissions.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Permission" />

    <div class="row mb-3">
        <div class="col-lg-6">
            <div class="card border-0 shadow-lg">
                <div class="card-body">

                    <h4 class="card-title mb-3">
                        <i class="far fa-key me-1"></i>
                        Permission
                    </h4>

                    <div class="row">
                        <div class="col-lg-12">
                            <uc:Detail ID="Detail" runat="server" />
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>
        
    <div class="row">
        <div class="col-lg-6">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Permission" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
