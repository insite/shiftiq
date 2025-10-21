<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompletionBars.ascx.cs" Inherits="InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls.CompletionBars" %>

<div class="row">
    <div class="col-sm-6 margin-bottom-general">
        <a class="progress-container" href="/ui/portal/job/candidates/profile/edit#profileTabDiv">
            <span class="text-body-secondary">Profile Completion</span>

            <asp:Literal runat="server" ID="ProfileCompletionBar"></asp:Literal>
        </a>
    </div>

    <div class="col-sm-6 margin-bottom-general">
        <a class="progress-container" href="/ui/portal/job/candidates/profile/edit#resumeTabDiv">
            <span class="text-body-secondary">Education and Experience Completion</span>
                                                            
            <asp:Literal runat="server" ID="ResumeCompletionBar"></asp:Literal>
        </a>
    </div>
</div>
