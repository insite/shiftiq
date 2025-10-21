<%@ Page Language="C#" CodeBehind="Start.aspx.cs" Inherits="InSite.Portal.Assessments.Attempts.Start" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:PageHeadContent runat="server">
    <asp:Literal runat="server" ID="ContentStyle" />
</insite:PageHeadContent>

<insite:Container runat="server" ID="KioskMode" Visible="false">
    <script type="text/javascript">
        (function () {
            $('header.navbar').remove();

            $(document).ready(function () {
                $('#attempt-commands').find('> div').addClass('position-fixed');
                $('footer.footer').remove();
                $('body').css('margin-bottom', '0px');
                $('.breadcrumb').closest('section.container').remove();
            });
        })();
    </script>
</insite:Container>

<section class="container mb-2 mb-sm-0 pb-sm-5">

    <h1 runat="server" id="AssessmentTitle" class="mb-4"></h1>

    <insite:Alert runat="server" ID="AlertMessage" ShowClose="true" />

    <asp:Literal runat="server" ID="Notification" />

    <asp:MultiView runat="server" ID="MultiView">

        <asp:View runat="server" ID="SelectLearnerView">
            <div class="row">
                <div class="col-lg-7">

                    <div class="card">
                        <div class="card-body">

                            <h2><%= Translate("Select Learner") %></h2>
                            <p><%= Translate("Please select the person you are assessing:") %></p>
                            <div class="mb-3">
                                <insite:FindPersonCodeUser runat="server" ID="LearnerSelector"/>
                            </div>
                            <div class="fw-bold">
                                <asp:Literal ID="PersonFullName" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>

                </div>
            </div>

            <div class="mt-4">
                <insite:NextButton runat="server" ID="ConfirmLearnerButton" />
            </div>
        </asp:View>

        <asp:View runat="server" ID="StartExamView">
            <div class="row">
                <div class="col-lg-7">

                    <div class="card">
                        <div class="card-body">

                            <h2><%= Translate("Start Assessment") %></h2>

                            <asp:Literal runat="server" ID="InstructionsLiteral" />

                            <p><%= Translate("Please confirm your name and email address") %>:</p>

                            <insite:Container runat="server" ID="AssessorContainer">
                                <h3><%= Translate("Assessor") %></h3>

                                <div class="mb-3 row">
                                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="First Name" /></label>
                                    <div class="col-md-9"><asp:Literal runat="server" ID="AssessorFirstName" /></div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Last Name" /></label>
                                    <div class="col-md-9"><asp:Literal runat="server" ID="AssessorLastName" /></div>
                                </div>

                                <div class="mb-3 row">
                                    <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Email" /></label>
                                    <div class="col-md-9"><asp:Literal runat="server" ID="AssessorEmail" /></div>
                                </div>

                                <h3><%= Translate("Learner") %></h3>
                            </insite:Container>

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

                            <div runat="server" id="LearnerCodeField" class="mb-3 row">
                                <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Candidate Code" /></label>
                                <div class="col-md-9"><asp:Literal runat="server" ID="LearnerCode" /></div>
                            </div>

                            <div runat="server" id="EventScheduledField" class="mb-3 row">
                                <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Exam Start Time" /></label>
                                <div class="col-md-9"><asp:Literal runat="server" ID="EventScheduled" /></div>
                            </div>

                            <div runat="server" id="EventTimeLimitField" class="mb-3 row">
                                <label class="col-md-3 form-label"><insite:Literal runat="server" Text="Time Limit" /></label>
                                <div class="col-md-9"><asp:Literal runat="server" ID="EventTimeLimit" /></div>
                            </div>

                            <div runat="server" id="TimeLimitField" class="mb-3 row">
                                Limited exam attempts are permitted within a time period of <asp:Literal runat="server" ID="TimeLimitPerSession" />,
                                and any access to the exam is counted as an attempt.
                                <br />
                                After <asp:Literal runat="server" ID="AttemptLimitPerSession" />
                                the exam is locked for <asp:Literal runat="server" ID="TimeLimitPerLockout" />.
                            </div>

                            <div runat="server" id="TimerField" class="mt-3 mb-3">
                                <div class="alert alert-warning">
                                    <strong><%= Translate("Please Note:") %></strong><br />
                                    <asp:Literal runat="server" ID="TimerLiteral" />
                                </div>
                            </div>

                        </div>
                    </div>

                </div>
            </div>

            <div class="mt-4">
                <insite:Button runat="server" ID="StartButton" ButtonStyle="Success" Text="Start" Icon="far fa-rocket-launch" />
            </div>
        </asp:View>

    </asp:MultiView>

</section>
</asp:Content>
