<%@ Page Language="C#" CodeBehind="Edit.aspx.cs" Inherits="InSite.Admin.Jobs.Candidates.Edit" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/CandidateCard.ascx" TagName="CandidateCard" TagPrefix="uc" %>
<%@ Register Src="Controls/LanguageAbilitySection.ascx" TagName="LanguageAbilitySection" TagPrefix="uc" %>
<%@ Register Src="Controls/ContactCommentGrid.ascx" TagName="ContactCommentGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/ContactUploadsGrid.ascx" TagName="ContactUploadsGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/CandidateExperienceGrid.ascx" TagName="CandidateExperienceGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/CandidateEducationGrid.ascx" TagName="CandidateEducationGrid" TagPrefix="uc" %>
<%@ Register Src="Controls/AchievementGrid.ascx" TagName="AchievementGrid" TagPrefix="uc" %>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">
    <insite:Alert runat="server" ID="StatusAlert" />
    <insite:ValidationSummary runat="server" ValidationGroup="Contact" />

    <insite:Nav runat="server">

        <insite:NavItem runat="server" Title="Profile">
            <div class="row">
                <div class="col-8">

                    <uc:CandidateCard runat="server" ID="CandidateCard" />

                    <div class="card mt-3">
                        <div class="card-body">

                            <h4 class="card-title mb-3">Language Ability</h4>

                            <uc:LanguageAbilitySection runat="server" ID="LanguageAbility" />

                        </div>
                    </div>

                    <div class="card mt-3">
                        <div class="card-body">

                            <h4 class="card-title mb-3">Comments</h4>

                            <uc:ContactCommentGrid runat="server" ID="ContactCommentGrid" />

                        </div>
                    </div>

                    <div runat="server" id="ContactUploadsCard" class="card mt-3">
                        <div class="card-body">

                            <h4 class="card-title mb-3">Uploads</h4>

                            <uc:ContactUploadsGrid runat="server" ID="ContactUploadsGrid" />

                        </div>
                    </div>

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Experience and Education">
            <div class="card mt-3">
                <div class="card-body">

                    <h4 class="card-title mb-3">Experience</h4>

                    <uc:CandidateExperienceGrid runat="server" ID="CandidateExperienceGrid" />

                </div>
            </div>

            <div class="card mt-3">
                <div class="card-body">

                    <h4 class="card-title mb-3">Education</h4>

                    <uc:CandidateEducationGrid runat="server" ID="CandidateEducationGrid" />

                </div>
            </div>
        </insite:NavItem>

        <insite:NavItem runat="server" Title="Credentials">
            <div class="row">
                <div class="col-lg-8">
                    <div class="card">
                        <div class="card-body">

                            <h4 class="card-title mb-3">Shift iQ Achievements</h4>

                            <uc:AchievementGrid runat="server" ID="AchievementGrid" />

                        </div>
                    </div>
                </div>
            </div>
        </insite:NavItem>

    </insite:Nav>

    <div class="mt-3">
        <insite:SaveButton runat="server" ID="SaveButton" ValidationGroup="Contact" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

</asp:Content>
