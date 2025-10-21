<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.View" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register Src="./Controls/ViewJobInterestSection.ascx" TagName="ViewJobInterestSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/ViewLanguageAbilitySection.ascx" TagName="ViewLanguageAbilitySection" TagPrefix="uc" %>
<%@ Register Src="./Controls/ViewExperienceSection.ascx" TagName="ViewExperienceSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/ViewEducationSection.ascx" TagName="ViewEducationSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/ViewDocumentSection.ascx" TagName="ViewDocumentSection" TagPrefix="uc" %>
<%@ Register Src="./Controls/AchievementSection.ascx" TagName="AchievementSection" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Profile" />

    <insite:Nav runat="server">
        <insite:NavItem ID="ProfileTab" runat="server" Title="Profile" Icon="far fa-fw fa-id-card">

            <div class="alert-info card mb-3">
                <div class="card-body">
                    <div class="row">
                        <div class="fw-light col-12">
                            This is how your portfolio will appear to potential Employers, please ensure it is as complete as possible.
                        </div>
                    </div>
                </div>

            </div>

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
        <insite:SaveButton runat="server" ID="SaveButton" Text="Update" ValidationGroup="Profile" NavigateUrl="/ui/portal/job/candidates/profile/edit" />
    </div>

</asp:Content>
