<%@ Page Title="Jobs | Candidates | Opportunities | View" Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.View" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="../../Employers/Opportunities/Controls/ViewDetails.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="StatusAlert" />

    <asp:Label runat="server" ID="Instructions" />

    <div runat="server" id="Content" class="row"></div>

    <uc:Details ID="Details" runat="server" />

    <div class="row">
        <div class="col-10">
            <insite:Button runat="server" ButtonStyle="Default" Icon="fas fa-arrow-left" Text="Back"
                NavigateUrl="/ui/portal/job/candidates/opportunities/search" />
            <span runat="server" id="Approved" visible='<%# CurrentUser != null %>'>
                <insite:Button runat="server" 
                    NavigateUrl='<%# Eval("OpportunityIdentifier", "/ui/portal/job/candidates/opportunities/apply?id={0}") %>'  
                    ButtonStyle="Success" 
                    Text="Apply Now" 
                    ID="Apply" 
                    Icon="fas fa-cloud-upload" 
                    Visible='<%# IsUserApproved %>'
                    />
            </span>
        </div>
    </div>

</asp:Content>