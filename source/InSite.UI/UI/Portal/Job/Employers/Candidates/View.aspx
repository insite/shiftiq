<%@ Page Title="" Language="C#" MasterPageFile="~/UI/Layout/Portal/Portal.master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Employers.Candidates.View" %>

<%@ Register Src="../../Candidates/Profile/Controls/ViewJobInterestSection.ascx" TagName="ViewJobInterestSection" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Profile/Controls/ViewLanguageAbilitySection.ascx" TagName="ViewLanguageAbilitySection" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Profile/Controls/ViewExperienceSection.ascx" TagName="ViewExperienceSection" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Profile/Controls/ViewEducationSection.ascx" TagName="ViewEducationSection" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Profile/Controls/ViewDocumentSection.ascx" TagName="ViewDocumentSection" TagPrefix="uc" %>
<%@ Register Src="../../Candidates/Profile/Controls/AchievementSection.ascx" TagName="AchievementSection" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <insite:Nav runat="server">
        <insite:NavItem ID="ProfileTab" runat="server" Title="Profile" Icon="far fa-fw fa-id-card">

            <uc:ViewJobInterestSection runat="server" ID="JobInterest" />

            <uc:ViewLanguageAbilitySection runat="server" ID="LanguageAbility" />

        </insite:NavItem>
        <insite:NavItem ID="EducationTab" runat="server" Title="Experience and Education" Icon="far fa-fw fa-user-graduate">

            <uc:ViewExperienceSection runat="server" ID="Experience" />

            <uc:ViewEducationSection runat="server" ID="Education" />

            <uc:ViewDocumentSection runat="server" ID="Document" />

        </insite:NavItem>
        <insite:NavItem ID="CredentialsTab" runat="server" Title="Credentials" Icon="far fa-fw fa-trophy">

            <uc:AchievementSection runat="server" ID="Achievement" />

        </insite:NavItem>
    </insite:Nav>

    <div class="mt-3">
        <insite:Button runat="server" ID="RequestContactLink" Text="Request Candidate Contact" Visible="false" ButtonStyle="Success" Icon="far fa-fw fa-envelope" />
        <insite:CancelButton runat="server" ID="CancelButton" NavigateUrl="/ui/portal/job/employers/candidates/search" />
    </div>

    <insite:Modal runat="server" ID="RequestContactWindow" Title="Request Candidate Contact" Width="550px" MinHeight="300px" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            var candidateRequestContact = {
                showRequestContact: function (contactId) {
                    var wnd = modalManager.load('<%= RequestContactWindow.ClientID %>', '/ui/portal/job/employers/candidates/request-contact?candidate=' + String(contactId));
                    $(wnd).one('closed.modal.insite', candidateRequestContact.onWindowClose);
                }
            };

        </script>
    </insite:PageFooterContent>

</asp:Content>

