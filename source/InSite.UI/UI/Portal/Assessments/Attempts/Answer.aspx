<%@ Page Language="C#" CodeBehind="Answer.aspx.cs" Inherits="InSite.Portal.Assessments.Attempts.Answer" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<%@ Register TagPrefix="uc" TagName="QuestionList" Src="Controls/AnswerQuestionList.ascx" %>
<%@ Register TagPrefix="uc" TagName="QuestionNav" Src="Controls/AnswerQuestionNav.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

<insite:PageHeadContent runat="server">

    <insite:ResourceBundle runat="server" Type="Css">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Layout/common/parts/plugins/bootstrap-listbox/listbox.css" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.css" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.css" />
        </Items>
    </insite:ResourceBundle>

    <asp:Literal runat="server" ID="InitFieldScript" />

    <asp:Literal runat="server" ID="ContentStyle" />

</insite:PageHeadContent>

<insite:Container runat="server" ID="KioskMode" Visible="false">
    <script type="text/javascript">
        const _isAnswerKioskMode = true;

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

    <div class="position-relative">
        <div id="attempt-commands" class="mt-4 attempt-commands">
            <asp:Panel runat="server" ID="CommandsPanel">
                <a runat="server" id="ViewAttachmentsLink" title="View Attachments" data-action="view-addendum" class="btn btn-default">
                    <i class="fal fa-download"></i>
                </a>
                <a runat="server" id="ViewAcronymsLink" title="View Acronyms" class="btn btn-default">
                    <i class="fal fa-th-list"></i>
                </a>
                <a runat="server" id="ViewFormulasLink" title="View Formulas" class="btn btn-default">
                    <i class="fal fa-square-root-alt"></i>
                </a>
                <div id="timer" style="display:none;"></div>
                <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-tab" PostBackEnabled="false" />
                <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-section" PostBackEnabled="false" />
                <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-question" PostBackEnabled="false" />
                <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="End Break" data-action="end-break" PostBackEnabled="false" />
                <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fa-cloud-upload" Text="Complete" data-action="complete" PostBackEnabled="false" />
            </asp:Panel>
        </div>
        <div>
            <p class="text-danger m-0">
                <asp:Literal ID="PageLearner" runat="server"></asp:Literal>
            </p>
            <h2 runat="server" id="PageTitle" class="pt-4 mb-4"></h2>
        </div>
    </div>

    <insite:Alert runat="server" ID="AlertMessage" />

    <asp:Literal runat="server" ID="Notification" />

    <div class="growl" id="app-growl"></div>

    <insite:Container runat="server" ID="MainPanel">

        <div class="accordion accordion-bookmarks mb-4" id="bookmarks-top" data-position="top" style="display:none;">
            <div class="accordion-item">
                <h2 class="accordion-header" id="bookmarks-top-heading">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#bookmarks-top-panel" aria-expanded="false" aria-controls="bookmarks-top-panel">
                        <%= Translate("Bookmarked Questions") %>
                    </button>
                </h2>
                <div id="bookmarks-top-panel" class="accordion-collapse collapse" aria-labelledby="accordion-header" data-bs-parent="#bookmarks-top">
                    <div class="accordion-body">
                        <table class="table table-hover" style="background-color:transparent;">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <uc:QuestionNav runat="server" ID="QuestionNav" Visible="false" />
        <uc:QuestionList runat="server" ID="QuestionList" Visible="false" />

        <div class="accordion accordion-bookmarks" id="bookmarks-bottom" data-position="bottom" style="display:none;">
            <div class="accordion-item">
                <h2 class="accordion-header" id="bookmarks-bottom-heading">
                    <button class="accordion-button collapsed" type="button" data-bs-toggle="collapse" data-bs-target="#bookmarks-bottom-panel" aria-expanded="false" aria-controls="bookmarks-bottom-panel">
                        <%= Translate("Bookmarked Questions") %>
                    </button>
                </h2>
                <div id="bookmarks-bottom-panel" class="accordion-collapse collapse" aria-labelledby="accordion-header" data-bs-parent="#bookmarks-bottom">
                    <div class="accordion-body">
                        <table class="table table-hover" style="background-color:transparent;">
                            <tbody></tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <div runat="server" id="ButtonPanel" class="mt-4 attempt-commands">
            <insite:Button runat="server" ButtonStyle="Default" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-left" Text="Previous" data-action="prev-tab" PostBackEnabled="false" />
            <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-tab" PostBackEnabled="false" />
            <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-section" PostBackEnabled="false" />
            <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="Next" data-action="next-question" PostBackEnabled="false" />
            <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fas fa-arrow-alt-right" Text="End Break" data-action="end-break" PostBackEnabled="false" />
            <insite:Button runat="server" ButtonStyle="Success" CssClass="exam-locked" Icon="fas fa-cloud-upload" Text="Complete" data-action="complete" PostBackEnabled="false" />
            <insite:Button runat="server" ID="FeedbackLink" ButtonStyle="Primary" Icon="fas fa-search" Text="Review Feedback" NavigateUrl="#review-feedback" data-action="review-feedback" PostBackEnabled="false" />
            <div class="float-end d-none single-question-count" style="line-height: 36px;"></div>
        </div>

        <div class="modal fade" id="add-question-feedback" tabindex="-1" aria-labelledby="add-question-feedback-label" aria-hidden="true" data-bs-backdrop="static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">

                    <div class="modal-header">
                        <h5 class="modal-title" id="add-question-feedback-label"><%= Translate("Post Feedback") %></h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        <textarea name="text" class="form-control" rows="2" cols="20" style="width:100%;height:150px;resize:none;" required></textarea>
                    </div>

                    <div class="modal-footer">
                        <insite:Button runat="server" ButtonStyle="Success" CssClass="btn-submit" Icon="fas fa-comment-plus" Text="Post" PostBackEnabled="false" />
                        <insite:CancelButton runat="server" Text="Cancel" data-bs-dismiss="modal" PostBackEnabled="false" />
                    </div>

                    <insite:LoadingPanel runat="server" />

                </div>
            </div>
        </div>

        <div class="modal fade" id="review-feedback-dialog" tabindex="-1" aria-labelledby="review-feedback-label" aria-hidden="true" data-bs-backdrop="static">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">

                    <div class="modal-header">
                        <h5 class="modal-title" id="review-feedback-label"><%= Translate("Feedback") %></h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        <div id="feedback-list" class="feedbacks">
                        </div>
                    </div>

                    <div class="modal-footer">
                        <insite:CloseButton runat="server" data-bs-dismiss="modal" PostBackEnabled="false" />
                    </div>

                    <insite:LoadingPanel runat="server" />

                </div>
            </div>
        </div>

        <div class="modal fade" id="confirm-dialog" tabindex="-1" aria-labelledby="confirm-label" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="confirm-label"><%= Translate("Confirmation") %></h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                    
                    </div>
                    <div class="modal-footer">
                        <insite:Button runat="server" ButtonStyle="Success" CssClass="btn-confirm me-2" Icon="fas fa-cloud-upload" Text="Confirm" data-action="confirm" PostBackEnabled="false" />
                        <insite:CancelButton runat="server" data-action="cancel" PostBackEnabled="false" />
                    </div>
                </div>
            </div>
        </div>

        <insite:Container runat="server" ID="AddendumDialogPanel">

            <div class="modal fade" id="addendum-dialog" tabindex="-1" aria-labelledby="addendum-label" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">

                        <div class="modal-header">
                            <h5 class="modal-title" id="addendum-label"><%= Translate("Attachments") %></h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>

                        <div class="modal-body">
                            <table class="table table-hover">
                                <tbody>
                                    <asp:Repeater runat="server" ID="DocumentRepeater">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="addendum-icon">
                                                    <i class='<%# Eval("Icon", "fal fa-{0}") %>'></i>
                                                </td>
                                                <td class="addendum-title" title='<%# HttpUtility.HtmlEncode((string)Eval("Title")) %>'><%# Eval("Title") %></td>
                                                <td class="addendum-size"><%# Eval("Size") %></td>
                                                <td class="addendum-cmd">
                                                    <a target="_blank" href='<%# Eval("Url") %>' title="Open in a new window"><i class="far fa-folder-open"></i></a>
                                                    <a href='<%# Eval("Url") + "?download=1" %>' title="Download" class="ms-2"><i class="far fa-download"></i></a>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>

                        <div class="modal-footer">
                            <insite:CloseButton runat="server" data-bs-dismiss="modal" PostBackEnabled="false" />
                        </div>

                        <insite:LoadingPanel runat="server" />

                    </div>
                </div>
            </div>
        </insite:Container>

        <div class="modal fade" id="info-dialog" tabindex="-1" aria-labelledby="info-label" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
            <div class="modal-dialog">
                <div class="modal-content">

                    <div class="modal-header">
                        <h5 class="modal-title" id="info-label"></h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>

                    <div class="modal-body">
                        
                    </div>

                    <div class="modal-footer">
                        <insite:CloseButton runat="server" data-action="close" PostBackEnabled="false" />
                    </div>

                </div>
            </div>
        </div>

    </insite:Container>

</section>

<insite:PageFooterContent runat="server" ID="FooterScript" Visible="false">
    <insite:ResourceBundle runat="server" Type="JavaScript">
        <Items>
            <insite:ResourceBundleFile Url="/UI/Layout/common/parts/js/bootstrap-validator.min.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/timer.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.likert.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.hotspot.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.helper.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.submit.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.input.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.markdown.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.feedback.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.bookmark.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.other.js" />
            <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/answer.hotspot.js" />
        </Items>
    </insite:ResourceBundle>
</insite:PageFooterContent>
</asp:Content>
