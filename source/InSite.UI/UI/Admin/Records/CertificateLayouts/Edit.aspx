<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Admin.Records.AchievementLayouts.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Controls/Details.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ID="ValidationSummary" />

    <section runat="server" ID="GeneralSection" class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-ruler-combined me-1"></i>
            Achievement Layout
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
                <uc:Details runat="server" ID="Details" />
            </div>
        </div>
    </section>

    <div class="row">
        <div class="col-lg-12">
            <insite:SaveButton runat="server" ID="SaveButton" />
            <insite:CancelButton runat="server" ID="CancelLink" />
        </div>
    </div>

</asp:Content>
