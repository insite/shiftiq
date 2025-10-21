<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Admin.Records.AchievementLayouts.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register TagPrefix="uc" TagName="Details" Src="./Controls/Details.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" />

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
