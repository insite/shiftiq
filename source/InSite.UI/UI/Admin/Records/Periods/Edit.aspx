<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Records.Periods.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<%@ Register Src="Controls/Detail.ascx" TagName="Detail" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="AlertStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Period" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-clock me-1"></i>
            Period
        </h2>

        <div class="card border-0 shadow-lg">
            <div class="card-body">
            <div class="row">
                <div class="col-lg-6">
                    <uc:Detail runat="server" ID="Detail" />
                </div>
            </div>
            </div>
        </div>
    </section>

    <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Period" />
    <insite:CancelButton runat="server" ID="CancelButton" />

</asp:Content>
