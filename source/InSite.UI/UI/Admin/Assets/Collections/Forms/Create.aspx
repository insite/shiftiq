<%@ Page CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Utilities.Collections.Forms.Create" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="CreatorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" ValidationGroup="Collection" />

    <div class="row mb-3">
        <div class="col-12">

            <section runat="server" ID="GeneralSection">
                <h2 class="h4 mb-3">
                    <i class="far fa-album-collection me-1"></i>
                    Collection
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <uc:Detail runat="server" ID="Details" />

                    </div>
                </div>
            </section>

        </div>
    </div>
        
    <div class="row">
        <div class="col-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Collection" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
