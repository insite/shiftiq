<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewResult.ascx.cs" Inherits="InSite.UI.Portal.Assessments.QuizAttempts.Controls.ViewResult" %>

<insite:Button runat="server" ID="BackToCourseButton" CssClass="mb-3" Visible="false"
    Text="Return to Course Outline" Icon="fas fa-sitemap" ButtonStyle="Default" />

<div class="row">
    <div class="col-lg-7">

        <div class="card">
            <div class="card-body">

                <h2><%= Translate("Quiz Result") %></h2>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Quiz Type" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizType" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Time Elapsed" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizTimeElapsed" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Number of attempts" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="QuizAttemptNumber" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Mistakes" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultMistakes" /></div>
                </div>

                <div runat="server" id="FieldKph" class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="KPH" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultKph" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="WPM" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultWpm" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="CPM" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultCpm" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Accuracy" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultAccuracy" /></div>
                </div>

                <div class="mb-3 row">
                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Speed" /></label>
                    <div class="col-md-9"><asp:Literal runat="server" ID="ResultSpeed" /></div>
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
    <insite:Button runat="server" ID="RestartButton" ButtonStyle="Success" Text="Try Again" Icon="far fa-redo" />
</div>