<%@ Page Language="C#" CodeBehind="Course.aspx.cs" Inherits="InSite.UI.Portal.Learning.Course" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="CertificateRepeater" Src="./Controls/CertificateRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="UnitRepeater" Src="./Controls/UnitRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentInfo" Src="./Controls/CommentInfo.ascx" %>
<%@ Register TagPrefix="uc" TagName="CommentRepeater" Src="./Controls/CommentRepeater.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">

        .unit-item .accordion-button { padding-left: 0; }
        .widget a { display: block; padding: 3px; white-space: normal; }
        .widget a.active, .widget-link.active { background-color: #F7F7FC !important; }
        
        a.glossary-term { text-decoration: underline double; }

        .lesson h2 { margin-top: 2rem; }
        .lesson h3 { margin-top: 2rem; }
        .lesson h4 { margin-top: 2rem; }
        .lesson h5 { margin-top: 2rem; }

        div.card-text > h2:first-of-type,
        div.card-text > h3:first-of-type { margin-top: 0rem; }

        @media (min-width: 992px) {
            main.portal-container .portal-sidebar {
                min-width: <%= SidebarWidth + "px" %>;
                max-width: <%= SidebarWidth + "px" %>;
            }
        }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="SideContent">

    <h3 class="rounded-top d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">
        <insite:Literal runat="server" Text="Course Outline" />
    </h3>

    <div class="d-block p-4 widget">
        <h3 class="widget-title mb-2"><%= Progress.Course.Content.Title.GetText(CurrentLanguage, true) %></h3>
        <ol runat="server" id="OutlineList">
            <li>

                <a runat="server" id="CourseOverviewLink" class="" href="#">
                    <i class="fas fa-info-circle me-1"></i><insite:Literal runat="server" Text="Overview" />
                </a>

                <a runat="server" id="CourseDiscussionsLink" class="" href="#">
                    <i class="fas fa-comments me-1"></i><insite:Literal runat="server" Text="Discussions" />
                </a>

                <a runat="server" id="CourseAchievementsLink" visible="false" class="" href="#">
                    <i class="fas fa-award me-1"></i><insite:Literal runat="server" Text="Achievements" />
                </a>

                <asp:LinkButton runat="server" ID="RestartCourseButton" CssClass="mt-3">
                    <i class="fas fa-redo me-1"></i><insite:Literal runat="server" Text="Restart" />
                </asp:LinkButton>

                <asp:LinkButton runat="server" ID="UnlockModulesButton" CssClass="">
                    <i runat="server" id="UnlockModulesIcon" class="fas fa-light fa-unlock-keyhole me-1"></i><insite:Literal runat="server" ID="UnlockModulesLiteral" Text="Unlock all units and modules" />
                </asp:LinkButton>

                <asp:LinkButton runat="server" ID="UnlockActivitiesButton" CssClass="">
                    <i runat="server" id="UnlockActivitiesIcon" class="fas fa-light fa-lock-keyhole me-1"></i><insite:Literal runat="server" ID="UnlockActivitiesLiteral" Text="Unlock all visible activities" />
                </asp:LinkButton>

            </li>
        </ol>
    </div>

    <div class="d-block ps-4 pe-4">
        <uc:UnitRepeater runat="server" ID="UnitRepeater" />
    </div>

    <div runat="server" id="ProgramPanel" class="mt-4 alert alert-light shadow" visible="false">
        <h3 runat="server" id="ProgramName" class="widget-title mb-2">Program</h3>
        <div runat="server" id="ProgramNext" class="mb-2">
            <div class="fs-sm text-body-secondary">Next Course:</div>
            <a runat="server" id="ProgramNextLink" href="#"></a>
        </div>
        <div runat="server" id="ProgramPrev">
            <div class="fs-sm text-body-secondary">Previous Course:</div>
            <a runat="server" id="ProgramPrevLink" href="#"></a>
        </div>
    </div>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="CriticalAlert" />

    <asp:PlaceHolder runat="server" ID="CourseStyles" />

    <div runat="server" id="EnabledPanel">
        
        <div class="lesson col-full-height">

            <asp:Literal runat="server" ID="CourseBreadcrumbs" />

            <h1 class="py-1 my-2 pb-2 mb-4"><%= Progress.Course.Content.Title.GetText(CurrentLanguage, true) %></h1>

            <div runat="server" id="ActivitySummary" class="callout fs-md text-body-secondary mb-4"></div>

            <insite:Alert runat="server" ID="ErrorAlert" />

            <div runat="server" id="OverviewPanel" visible="false">
                    
                <div runat="server" id="OverviewAboutPanel" visible="false" class="mb-5">
                    <h2>
                        <insite:Literal runat="server" Text="Overview" />
                    </h2>
                    <div runat="server" ID="OverviewAboutText"></div>
                    <div class="mt-5">
                        <insite:Button runat="server" ID="StartButton" Text="Start" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" ButtonStyle="Primary" />
                    </div>
                </div>

                <div runat="server" id="OverviewDiscussionPanel" visible="false">
                    
                    <h2>
                        <insite:Literal runat="server" Text="Discussions" />
                    </h2>
                    
                    <div class="mb-3">
                        <uc:CommentInfo runat="server" ID="PostCommentInput" />
                        <div>
                            <insite:Button runat="server" ID="PostCommentButton" Text="Post" Icon="fas fa-comment" CssClass="btn btn-sm btn-primary" />
                        </div>
                    </div>

                    <div>
                        <uc:CommentRepeater runat="server" ID="PostCommentRepeater" />
                    </div>

                </div>
                    
                <div runat="server" id="OverviewAchievementPanel" visible="false">
                    
                    <h2>
                        <insite:Literal runat="server" Text="Achievements" />
                    </h2>

                    <insite:Alert runat="server" ID="ValidAlert" Icon="fa-solid fa-circle-check mt-1" Indicator="Success" />

                    <insite:Alert runat="server" ID="ExpiredAlert" Icon="fa-solid fa-circle-exclamation mt-1" Indicator="Warning" />
                    
                    <uc:CertificateRepeater runat="server" ID="CertificateRepeater" />
                    
                    <insite:Alert runat="server" ID="AlertRestartInfo" Icon="fa-solid fa-circle-info mt-1" Indicator="Information" CssClass="mt-4" />

                    <asp:Panel runat="server" ID="ReportsPanel">
                        <h2>
                            <insite:Literal runat="server" Text="Reports" />
                        </h2>
                        <asp:HyperLink runat="server" ID="ProgressSummaryLink" />
                    </asp:Panel>

                </div>

            </div>

            <div runat="server" id="NoAccessPanel" class="card mb-4" visible="false">
                <div class="card-body">
                    <div class="card-text">
                        Access Denied
                    </div>
                </div>
            </div>

            <div runat="server" id="AssessmentPanel" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="AssessmentBody" class="card-text"></div>
                    <div runat="server" id="AssessmentInstructions" class="card-text"></div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <asp:LinkButton runat="server" id="ViewAssessmentLink" class="btn btn-sm btn-primary ms-1"></asp:LinkButton>
                                <asp:LinkButton runat="server" id="StartAssessmentLink" class="btn btn-sm btn-primary"></asp:LinkButton>
                            </div>
                            <div runat="server" id="GradeLabel" style="line-height:2.25;" visible="false">
                            </div>
                        </div>
                        <div runat="server" id="AttemptMetadata" class="fs-sm text-body-secondary mt-2" />
                    </div>
                </div>
            </div>

            <div runat="server" id="QuizPanel" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="QuizBody" class="card-text"></div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                        <div class="flex-grow-1">
                            <a runat="server" id="QuizLaunch" href="#" class="btn btn-primary"><i class="far fa-rocket me-2"></i>Launch</a>
                        </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="DocumentPanel" class="card mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="DocumentDescription" class="card-text">
                        
                    </div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                        <div class="flex-grow-1">
                            <a runat="server" id="DocumentLaunchLink" href="#" class="btn btn-sm btn-primary" visible="false">
                                <i class="far fa-rocket"></i>
                                <insite:Literal runat="server" Text="Launch" />
                            </a>
                            <asp:Literal runat="server" ID="DocumentEmbedContent" Visible="false" />
                        </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="LessonPanel" class="mb-4" visible="false"></div>

            <div runat="server" id="LinkPanel" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="LinkDescription" class="card-text">
                        
                    </div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                        <div class="flex-grow-1">
                            <a runat="server" id="LinkLaunchLink" href="#" visible="false"></a>
                            <asp:Literal runat="server" ID="LinkEmbedContent" Visible="false" />
                        </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="LinkPanelScorm" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="ScormBody" class="card-text"></div>
                    <div class="card-text text-body-secondary my-3"><asp:Literal runat="server" ID="ScormTimeRequired" /></div>
                    <div class="card-text text-body-secondary mt-3"></div>

                    <div class="d-flex">
                        <div class="flex-grow-1">
                            <a runat="server" id="ScormStartUrl" visible="true" href="about:blank" class="btn btn-sm btn-primary">
                                <i class="far fa-rocket me-2"></i><insite:Literal runat="server" Text="Launch" />
                            </a>
                        </div>
                    </div>

                </div>
            </div>

            <div runat="server" id="SurveyPanel" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="SurveyBody" class="card-text"></div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <a runat="server" id="SurveyLink" href="#" class="btn btn-sm btn-primary"></a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="VideoPanel" class="card shadow mb-4" visible="false">
                <div class="card-body">
                    <div runat="server" id="VideoDescription" class="card-text">
                        
                    </div>
                    <div class="card-text text-body-secondary mt-3">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <a runat="server" id="VideoLaunchLink" href="#" class="btn btn-sm btn-primary" visible="false">
                                    <i class='fas fa-video'></i>
                                    <insite:Literal runat="server" Text="Watch" />
                                </a>
                                <asp:Literal runat="server" ID="VideoEmbedContent" Visible="false" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" id="InteractionPanel" class="mb-4" visible="false"></div>
 
            <div runat="server" id="DonePanel" class="alert alert-light mt-2" visible="false">
                <div class="row">
                    <div class="col-md-12">
                        <div runat="server" id="NotDoneContent" class="d-flex align-items-center flex-row" visible="false">
                            <insite:Button runat="server" ID="DoneButton" ButtonStyle="Primary" Icon="fas fa-check-circle" IconPosition="BeforeText" />
                            <div class="text-body-secondary fs-sm ms-2">
                                <insite:Literal runat="server" ID="DoneButtonInstructions" />
                            </div>
                        </div>
                        <div runat="server" id="DoneContent" visible="false">
                            <div class="text-body-secondary fs-sm">
                                <i class="fas fa-check-circle me-2"></i>
                                <insite:Literal runat="server" ID="DoneMarkedInstructions" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div runat="server" class="mt-5" id="ControlButtons">
                <insite:Button runat="server" ID="PrevButton" Text="Previous" Icon="fas fa-arrow-alt-left" IconPosition="BeforeText" ButtonStyle="Default" />
                <insite:NextButton runat="server" ID="NextButton" />
            </div>

        </div>

    </div>

    <div runat="server" id="TermWindow" class="modal" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered" role="document">
            <div class="modal-content">

                <div class="modal-header">
                    <h5 class="modal-title"></h5>
                </div>

                <div class="modal-body" style="min-height:50px;">

                </div>

                <div class="modal-footer" style="display:block;">
                    <button type="button" class="btn btn-default" data-action="close"><i class='fas fa-ban me-2'></i> <insite:Literal runat="server" Text="Close" /></button>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        (function () {
            $(window).on('resize', onWindowResize);
            $(document).ready(onWindowResize);

            onWindowResize();

            function onWindowResize() {
                var $col = $('.col-full-height');

                var $spacer = $col.data('spacer');
                if (!$spacer)
                    $col.append($spacer = $('<div>')).data('spacer', $spacer);

                var footerHeight = $('footer').outerHeight(true);
                if (isNaN(footerHeight))
                    footerHeight = 105;

                var height = $(window).height() - $col.offset().top - $col.outerHeight(true) + $spacer.height() - footerHeight;
                if (height < 0)
                    height = 0;

                $spacer.height(height);
            }
        })();

        (function () {
            var termsData = <%= TermsData %>;
            var $modal = $('#<%= TermWindow.ClientID %>').on('click', onModalClick);
            var modal = new bootstrap.Modal($modal[0]);

            $('a').each(function () {
                var $this = $(this);

                var name = $this.attr('href');
                if (!termsData.hasOwnProperty(name))
                    return;

                $this.addClass('glossary-term').attr('title', 'View Term Definition').on('click', onTermClick);
            });

            function onTermClick(e) {
                e.stopPropagation();
                e.preventDefault();

                var $this = $(this);

                var name = $this.attr('href');
                if (!termsData.hasOwnProperty(name))
                    return;

                var term = termsData[name];

                $modal.find('> .modal-dialog > .modal-content > .modal-header > .modal-title').text(term.title);
                $modal.find('> .modal-dialog > .modal-content > .modal-body').html(term.descr);

                modal.show();
            }

            function onModalClick(e) {
                var $target = $(e.target);
                var action = $target.data('action');

                if (action === 'close')
                    modal.hide();
            }
        })();
    </script>

</asp:Content>
