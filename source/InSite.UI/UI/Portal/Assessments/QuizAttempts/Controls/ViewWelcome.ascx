<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewWelcome.ascx.cs" Inherits="InSite.UI.Portal.Assessments.QuizAttempts.Controls.ViewWelcome" %>

<div class="row">
    <div class="col-lg-7">

        <div class="card">
            <div class="card-body">

                <h2><%= Translate("Start Quiz") %></h2>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Quiz Type" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizType" /></div>
                </div>

                <div runat="server" id="QuizTimeLimitField" class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Time Limit" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizTimeLimit" /></div>
                </div>

                <div runat="server" id="QuizAttemptLimitField" class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Attempt Limit" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizAttemptLimit" /></div>
                </div>

                <div runat="server" id="QuizAttemptLeftField" class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Attempts Left" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizAttemptLeft" /></div>
                </div>

                <h3>Learner</h3>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="First Name" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="LearnerFirstName" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Last Name" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="LearnerLastName" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Email" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="LearnerEmail" /></div>
                </div>

            </div>
        </div>

    </div>
</div>

<div class="mt-4">
    <insite:Button runat="server" ID="StartButton" ButtonStyle="Success" Text="Start" Icon="far fa-rocket-launch" />
</div>