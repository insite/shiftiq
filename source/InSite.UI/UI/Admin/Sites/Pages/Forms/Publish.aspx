<%@ Page Language="C#" CodeBehind="Publish.aspx.cs" Inherits="InSite.Admin.Sites.Pages.Publish" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="../Controls/PageInfo.ascx" TagName="PageInfo" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="PageChange" />

        <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-cloud me-1"></i>
            Page
        </h2>

            
        <div class="row">

            <div class="col-lg-6">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>Details</h3>
                        <uc:PageInfo ID="PageDetails" runat="server" />
                    </div>
                </div>
            </div>

        </div>

    </section>


    <div class="row">
        <div class="col-lg-12">
            <insite:Button runat="server" ID="PublishButton" Text="Publish" Icon="fas fa-cloud-upload" ButtonStyle="Success" ValidationGroup="Publish" CausesValidation="true" />
            <insite:Button runat="server" ID="UnpublishButton" Text="Unpublish" Icon="fas fa-eraser" ButtonStyle="Danger" ValidationGroup="Publish" CausesValidation="true" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>


</asp:Content>
