<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Opportunities.Edit" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="Controls/Details.ascx" TagName="Details" TagPrefix="uc" %>
<%@ Register Src="Controls/CandidatesGrid.ascx" TagName="CandidatesGrid" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />

    <insite:ValidationSummary runat="server" ValidationGroup="Opportunity" />

    <div class="row mb-3">

        <div class="col-12">

            <div class="row">

                <div class="row text-end mb-3 px-3">
                    <div class="col-md-12">
                        <insite:Button runat="server" ID="ViewLink" Text="View" Visible="true" ButtonStyle="Default" Icon="far fa-fw fa-magnifying-glass" />
                    </div>
                    
                </div>

                <asp:MultiView runat="server" ID="MultiView">

                    <asp:View runat="server" ID="DefaultView">
                        <uc:Details ID="Details" runat="server" />
                    </asp:View>

                </asp:MultiView>

            </div>
        </div>
            
    </div>

    <uc:CandidatesGrid runat="server" ID="CandidatesGrid" Visible="true" />

    <div class="row">
        <div class="col-10">
            <insite:SaveButton runat="server" ID="SaveButton"  ValidationGroup="Opportunity" />
            <insite:CancelButton runat="server" ID="CancelButton" />
        </div>
    </div>

</asp:Content>
