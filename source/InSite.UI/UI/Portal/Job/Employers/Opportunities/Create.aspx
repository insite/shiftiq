<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Create.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Create" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/Details.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="CreateDetail">

        <insite:ValidationSummary runat="server" ValidationGroup="Opportunity" />

        <div class="row">


            <asp:MultiView runat="server" ID="MultiView">

                <asp:View runat="server" ID="DefaultView">
                    <uc:Details ID="Details" runat="server" />
                </asp:View>

            </asp:MultiView>

        </div>

        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Opportunity" />
                <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/job/employers/profile/view" />
            </div>
        </div>

    </div>

</asp:Content>
