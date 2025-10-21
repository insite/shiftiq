<%@ Page Language="C#" CodeBehind="Create.aspx.cs" Inherits="InSite.Admin.Jobs.Opportunities.Forms.Create" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        li.disabled a span.text {
            color: silver;
        }
    </style>
</asp:Content>

<%@ Register Src="./Controls/Details.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary ID="ValidationSummary" runat="server" ValidationGroup="Job" />

    <section class="mb-3">
        <h2 class="h4 mb-3">
            <i class="far fa-suitcase"></i>
            Opportunity
        </h2>

        <asp:MultiView runat="server" ID="MultiView">

            <asp:View runat="server" ID="DefaultView">
                <uc:Details ID="Details" runat="server" />
            </asp:View>

        </asp:MultiView>
    </section>


    <div class="row">
        <div class="col-md-12">
            <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Job" Visible="false" />
            <insite:CancelButton runat="server" ID="CancelButton" CausesValidation="false" />
        </div>
    </div>

</asp:Content>
