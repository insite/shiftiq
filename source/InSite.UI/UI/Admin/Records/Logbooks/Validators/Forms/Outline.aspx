<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.UI.Admin.Records.Validators.Forms.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Records/Logbooks/Controls/UserGrid.ascx" TagName="UserGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <section runat="server" ID="UsersPanel" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-users me-1"></i>
            Learners
        </h2>

        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <div class="mb-3">
                    <insite:Button runat="server" ID="AddUsers" Text="Add Learners" Icon="fas fa-plus-circle" ButtonStyle="Default" CssClass="me-1" />
                </div>

                <uc:UserGrid runat="server" ID="Users" IsValidator="true" />
            </div>
        </div>
    </section>

</asp:Content>
