<%@ Page CodeBehind="View.aspx.cs" Inherits="InSite.Admin.Assessments.Attempts.Forms.View" Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	
<%@ Register Src="~/UI/Admin/Assessments/Forms/Controls/FormInfo.ascx" TagName="FormDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/RubricGrid.ascx" TagName="RubricGrid" TagPrefix="uc" %>
<%@ Register Src="../Controls/ViewComposedEssay.ascx" TagName="ViewComposedEssay" TagPrefix="uc" %>
<%@ Register Src="../Controls/ViewComposedVoice.ascx" TagName="ViewComposedVoice" TagPrefix="uc" %>
<%@ Register Src="../Controls/AssessorDetails.ascx" TagName="AssessorDetails" TagPrefix="uc" %>
<%@ Register Src="../Controls/AssessorComment.ascx" TagName="AssessorComment" TagPrefix="uc" %>

<%@ Import Namespace="Shift.Common" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <insite:ResourceLink runat="server" Type="Css" Url="/UI/Admin/assessments/attempts/forms/view.css" />
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ViewerStatus" />
    <insite:ValidationSummary runat="server" />
    <insite:ValidationSummary runat="server" ValidationGroup="FileUpload" />
    
    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="AttemptPanel" Title="Attempt" Icon="far fa-tasks" IconPosition="BeforeText">
            <section>

                <h2 class="h4 mt-4 mb-3">Attempt</h2>

                <div class="row mb-3">
                    <div class="col-lg-12">
                        <insite:Button runat="server" id="AnalyzeCommand" ButtonStyle="Default" Text="Analyze" Icon="fas fa-analytics" />
                        <insite:Button runat="server" id="ViewHistoryLink" ButtonStyle="Default" Text="History" Icon="fas fa-history" />
                        <insite:Button runat="server" ID="CompleteCommand"
                            Icon="fas fa-flag-checkered"
                            Text="Force Submission"
                            ToolTip="Force submission of this attempt"
                            ConfirmText="Are you sure you want to force the submission of this attempt? It will move to a status of Completed, or of Pending if there are composed response questions to be graded."
                        />
                    </div>
                </div>

                <div class="row">
                        
                    <div class="col-lg-3">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Attempt</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Started</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptStartedOutput" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Submitted</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptSubmittedOutput" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Graded</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptGradedOutput" />                            
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Time Taken</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptTimeTaken" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div runat="server" id="SebField" class="form-group mb-3" visible="false">
                                    <label class="form-label">SEB Version</label>
                                    <div>
                                        <asp:Literal runat="server" ID="SebVersion" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <h3>Result</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Grade</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptIsPassing" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Score</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptScore" />
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Points</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptPoints" /> 
                                    </div>
                                    <div class="form-text">
        
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Attempt  Identifier</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptId" /> 
                                        <insite:IconLink runat="server" ID="DeleteAttemptLink" Name="trash-alt" ToolTip="Delete Attempt" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

                    <div runat="server" id="PersonColumn" class="col-lg-3">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Learner</h3>
                                <uc:PersonDetail runat="server" ID="PersonDetail" />
                                <uc:AssessorDetails runat="server" ID="ExamAssessorDetails" Title="Exam Assessor" Visible="false" />
                                <uc:AssessorDetails runat="server" ID="GradingAssessorDetails" Title="Grading Assessor" Visible="false" />
                            </div>
                        </div>
                    </div>
                
                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:FormDetails id="FormDetails" runat="server" />
                            </div>
                        </div>
                    </div>

                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="AnswerPanel" Title="Answers" Icon="far fa-ballot-check" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Answers
                </h2>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <insite:Nav runat="server">

                            <insite:NavItem runat="server" ID="SingleCorrectTab" Visible="false">
                                <div class="questions questions-radiobutton">
                                    <asp:Repeater runat="server" ID="SingleCorrectQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div class="options">
                                                        <asp:Repeater runat="server" ID="OptionRepeater">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <%# (bool?)Eval("AnswerIsSelected") == true ? ((decimal)Eval("OptionPoints") > 0 ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-right: 10px;"><p><%# Eval("OptionLetter") %>. </div></td>
                                                                            <td class="align-top"><%# Eval("OptionText") %></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                            
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="TrueOrFalseTab" Visible="false">
                                <div class="questions questions-radiobutton">
                                    <asp:Repeater runat="server" ID="TrueOrFalseQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div class="options">
                                                        <asp:Repeater runat="server" ID="OptionRepeater">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <%# (bool?)Eval("AnswerIsSelected") == true ? ((decimal)Eval("OptionPoints") > 0 ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-right: 10px;"><p><%# Eval("OptionLetter") %>. </div></td>
                                                                            <td class="align-top"><%# Eval("OptionText") %></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="MultipleCorrectTab" Visible="false">
                                <div class="questions questions-checkbox">
                                    <asp:Repeater runat="server" ID="MultipleCorrectQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div class="options">
                                                        <asp:Repeater runat="server" ID="OptionRepeater">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <%# (bool?)Eval("AnswerIsSelected") == true ? ((bool?)Eval("OptionIsTrue") == true ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-right: 10px;"><p><%# Eval("OptionLetter") %>. </div></td>
                                                                            <td class="align-top"><%# Eval("OptionText") %></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="BooleanTableTab" Title="Boolean Table" Visible="false">
                                <div class="questions questions-boolean">
                                    <asp:Repeater runat="server" ID="BooleanTableQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div class="answer">
                                                        <table class="table table-hover" style="width:50%;">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width:40px;"></th>
                                                                    <th></th>
                                                                    <th style="width:60px; text-align:center;">True</th>
                                                                    <th style="width:60px; text-align:center;">False</th>
                                                                </tr>
                                                            </thead>
                                                            <asp:Repeater runat="server" ID="OptionRepeater">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# Eval("OptionLetter") %>.</td>
                                                                        <td><%# Eval("OptionText") %></td>
                                                                        <td style="text-align:center;"><div><%# (bool?)Eval("AnswerIsSelected") == true ? ((bool?)Eval("OptionIsTrue") == true ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %></div></td>
                                                                        <td style="text-align:center;"><div><%# (bool?)Eval("AnswerIsSelected") == false ? ((bool?)Eval("OptionIsTrue") == false ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %></div></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </table>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="MatchingTab" Title="Matching" Visible="false">
                                <div class="questions questions-matching">
                                    <asp:Repeater runat="server" ID="MatchingQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div class="answer">
                                                        <table class="table table-hover">
                                                            <thead>
                                                                <tr>
                                                                    <th style="width:40px;"></th>
                                                                    <th></th>
                                                                    <th style="width:260px;">Answer</th>
                                                                    <th style="width:60px; text-align:center;"></th>
                                                                </tr>
                                                            </thead>
                                                            <asp:Repeater runat="server" ID="MatchRepeater">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td><%# Eval("MatchLetter") %>.</td>
                                                                        <td><%# Eval("MatchLeftText") %></td>
                                                                        <td><%# Eval("AnswerText") %></td>
                                                                        <td style="text-align:center;"><div><%# (bool)Eval("HasAnswer") ? ((bool)Eval("IsCorrect") == true ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %></div></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                        </table>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="LikertTab" Visible="false">
                                <div class="questions questions-likert">
                                    <asp:Repeater runat="server" ID="LikertQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("Question.CompetencyAreaCode") != null || Eval("Question.CompetencyItemCode") != null %>'>
                                                        <%# Eval("Question.CompetencyAreaCode") %><%# Eval("Question.CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("Question.QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("Question.QuestionText") %></div>
                                                    <div class="rows">
                                                        <table class="table table-striped table-likert" style="visibility:hidden;">
                                                            <thead>
                                                                <asp:Repeater runat="server" ID="LikertColumnRepeater">
                                                                    <HeaderTemplate>
                                                                        <tr>
                                                                            <td style="width:1px"></td>
                                                                            <td></td>
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <td class="text"><%# Eval("OptionText") %></td>
                                                                    </ItemTemplate>
                                                                    <FooterTemplate>
                                                                        </tr>
                                                                    </FooterTemplate>
                                                                </asp:Repeater>
                                                            </thead>
                                                            <tbody>
                                                                <asp:Repeater runat="server" ID="LikertRowRepeater">
                                                                    <ItemTemplate>
                                                                        <tr>
                                                                            <td>
                                                                                <%# Calculator.ToBase26((int)Eval("Question.QuestionNumber")) %>.
                                                                            </td>
                                                                            <td class="text">
                                                                                <%# Eval("Question.QuestionText") %>
                                                                            </td>
                                                                            <asp:Repeater runat="server" ID="LikertOptionRepeater">
                                                                                <ItemTemplate>
                                                                                    <td class="answer">
                                                                                        <div>
                                                                                        <div>
                                                                                            <%# (bool?)Eval("AnswerIsSelected") == true ? ((decimal)Eval("OptionPoints") > 0 ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %>
                                                                                        </div>
                                                                                        </div>
                                                                                    </td>
                                                                                </ItemTemplate>
                                                                            </asp:Repeater>
                                                                        </tr>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tbody>
                                                        </table>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorCommentLikert" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="HotspotTab" Visible="false">
                                <div class="questions questions-hotspot">
                                    <asp:Repeater runat="server" ID="HotspotQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question">
                                                    <div class="title"><%# Eval("QuestionText") %></div>
                                                    <div runat="server" id="HotspotImage" class="my-3 hotspot-image">
                                                    </div>
                                                    <div class="options">
                                                        <asp:Repeater runat="server" ID="OptionRepeater">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <%# (bool?)Eval("AnswerIsSelected") == true ? ((decimal)Eval("OptionPoints") > 0 ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>") : string.Empty %>
                                                                    <table>
                                                                        <tr>
                                                                            <td style="padding-right: 10px;"><p><%# Eval("OptionLetter") %>. </div></td>
                                                                            <td class="align-top"><%# Eval("OptionText") %></td>
                                                                        </tr>
                                                                    </table>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="ComposedEssayTab" Title="Composed Essay" Visible="false">
                                <div class="questions">
                                    <uc:ViewComposedEssay runat="server" ID="ComposedEssayViewer" />
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="ComposedVoiceTab" Title="Composed Voice" Visible="false">
                                <div class="questions">
                                    <uc:ViewComposedVoice runat="server" ID="ComposedVoiceViewer" />
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="OrderingTab" Visible="false">
                                <div class="questions questions-ordering">
                                    <asp:Repeater runat="server" ID="OrderingQuestionRepeater">
                                        <ItemTemplate>
                                            <div>
                                                <div class="float-end">
                                                    <span runat="server" class="badge bg-custom-default" visible='<%# Eval("CompetencyAreaCode") != null || Eval("CompetencyItemCode") != null %>'>
                                                        <%# Eval("CompetencyAreaCode") %><%# Eval("CompetencyItemCode") %>
                                                    </span>
                                                </div>
                                                <div class="sequence"><%# Eval("QuestionNumber") %></div>
                                                <div class="question position-relative">
                                                    <div class="title mb-3"><%# Eval("QuestionText") %></div>
                                                    <div runat="server" class="mb-3" visible='<%# Eval("QuestionTopLabel") != null %>'>
                                                        <%# Eval("QuestionTopLabel") %>
                                                    </div>
                                                    <div class="options">
                                                        <div class="position-absolute end-0 answer">
                                                            <%# (decimal?)Eval("AnswerPoints") > 0 ? "<i class='far fa-check'></i>" : "<i class='far fa-times'></i>" %>
                                                        </div>
                                                        <asp:Repeater runat="server" ID="OptionRepeater">
                                                            <HeaderTemplate>
                                                                <div class="pe-5">
                                                            </HeaderTemplate>
                                                            <FooterTemplate>
                                                                </div>
                                                            </FooterTemplate>
                                                            <ItemTemplate>
                                                                <div class="answer-option bg-white border rounded py-2 px-3 mb-3<%# (bool)Eval("IsCorrect") ? " border-success" : " border-danger" %>">
                                                                    <div class="mb-1 fw-bold">Option <%# Eval("Sequence") %></div>
                                                                    <div><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <div runat="server" class="mb-3" visible='<%# Eval("QuestionBottomLabel") != null %>'>
                                                        <%# Eval("QuestionBottomLabel") %>
                                                    </div>
                                                    <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </insite:NavItem>

                            <insite:NavItem runat="server" ID="CompetenciesNavItem" Title="Competency" Visible="false">
                                <asp:Repeater runat="server" ID="CompetencyAreaRepeater">
                                    <ItemTemplate>

                                        <div class="card">
                                            <div class="card-header">
                                                <div class="float-end" style="font-size:21px;">
                                                    <asp:Literal runat="server" ID="CompetencyAreaScore" />
                                                </div>
                                                <a href="/ui/admin/standards/edit?id=<%# Eval("Identifier") %>">
                                                    <%# Eval("Label") %> <%# Eval("Code") %>. <%# Eval("Title") %>
                                                </a>
                                            </div>
                                            <div class="card-body">

                                                <asp:Repeater runat="server" ID="CompetencyItemRepeater">
                                                    <ItemTemplate>

                                                        <h3>
                                                            <a href="/ui/admin/standards/edit?id=<%# Eval("Identifier") %>">
                                                                <%# Eval("Label") %>
                                                                <%# Eval("Code") %>.
                                                                <%# Eval("Title") %>
                                                            </a>
                                                        </h3>

                                                        <table class="table table-striped">
                                                            <thead>
                                                                <th colspan="2">Question</th>
                                                                <th class="text-end">Points</th>
                                                                <th></th>
                                                            </thead>
                                                            <asp:Repeater runat="server" ID="QuestionRepeater">
                                                                <ItemTemplate>
                                                                    <tr>
                                                                        <td style="width:60px;"><%# Eval("QuestionNumber") %></td>
                                                                        <td style=""><%# Eval("QuestionText") %></td>
                                                                        <td style="width:130px;" class="text-end"><%# Eval("AnswerPoints") %></td>
                                                                        <td style="width:90px;"></td>
                                                                    </tr>
                                                                </ItemTemplate>
                                                            </asp:Repeater>
                                                            <tfoot>
                                                                <th colspan="2"></th>
                                                                <th class="text-end" style="white-space:nowrap;"><asp:Literal runat="server" ID="CompetencyItemPoints" /></th>
                                                                <th class="text-end"><asp:Literal runat="server" ID="CompetencyItemScore" /></th>
                                                            </tfoot>
                                                        </table>
                                        

                                                    </ItemTemplate>
                                                </asp:Repeater>

                                                <uc:AssessorComment runat="server" ID="AssessorComment" Title="Assessor Comment" Visible="false" />

                                            </div>

                                        </div>
                        
                                    </ItemTemplate>
                                    <SeparatorTemplate>
                                        <div class="pt-4"></div>
                                    </SeparatorTemplate>
                                </asp:Repeater>
                            </insite:NavItem>

                        </insite:Nav>

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="RubricPanel" Title="Rubrics" Icon="far fa-table" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    <%= Translate("Rubrics") %>
                </h2>

                <insite:Button runat="server" ID="GradeButton"
                    ButtonStyle="Default"
                    Icon="fas fa-award"
                    Text="Grade"
                    CssClass="mb-3"
                />

                <insite:Button runat="server" ID="RegradeButton"
                    ButtonStyle="Danger"
                    Icon="fas fa-award"
                    Text="Re-grade"
                    CssClass="mb-3"
                />

                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <uc:RubricGrid runat="server" ID="Rubrics" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
    </insite:Nav>

    <insite:Modal runat="server" ID="ComposedConfirmWindow" Title="Confirmation" Width="350px">
        <ContentTemplate>
            <div class="mb-3">
                Please confirm the date attempt completed:
            </div>
            <div class="mb-3">
                <insite:DateTimeOffsetSelector runat="server" ID="ComposedConfirmCompleteDate" />
                <p class="form-text text-danger output-error" style="display:none;"></p>
            </div>
            <div>
                <insite:Button runat="server" Text="OK" ButtonStyle="Success" Icon="fas fa-check" data-action="confirm" />
                <insite:CancelButton runat="server" data-action="cancel" />
            </div>
        </ContentTemplate>
    </insite:Modal>

    <insite:PageFooterContent runat="server">
        <insite:ResourceBundle runat="server" Type="JavaScript">
            <Items>
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.js" />
                <insite:ResourceBundleFile Url="/UI/Portal/assessments/attempts/content/common.hotspot.js" />
            </Items>
        </insite:ResourceBundle>

        <script type="text/javascript">
            (function () {
                var instance = window.submissionViewer = window.submissionViewer || {};

                instance.onRubricMarkChanged = function (el) {
                    setTimeout(function (element) {
                        updateTotal($(element).closest('table.table-rubric'));
                    }, 0, el);
                };

                $(document).ready(function () {
                    $('table.table-rubric').each(function () {
                        updateTotal($(this));
                    });
                });

                function updateTotal($table) {
                    var hasValue = false;
                    var total = 0;

                    $table.find('> tbody > tr > .col-mark input[type="text"].insite-numeric').each(function () {
                        var value = parseFloat(this.value);
                        if (isNaN(value))
                            return;

                        hasValue = true;
                        total += value;
                    });

                    if (hasValue)
                        total = total.toFixed(0);
                    else
                        total = '';

                    $table.find('> tfoot > tr > .col-mark').html(total);
                }
            })();
        </script>

        <script type="text/javascript">

            (function () {
                var instance = window.viewForm = window.viewForm || {};
                var dateCompleted = moment('<%= AttemptGraded.HasValue ? string.Format("{0:yyyy-MM-dd}T{0:HH:mm:sszzz}", AttemptGraded.Value) : string.Empty %>');
                var isImported = <%= IsAttemptImported.ToString().ToLower() %>;

                instance.composed = (function () {
                    var instance = {};

                    var $modal = $(document.getElementById('<%= ComposedConfirmWindow.ClientID %>'));
                    var $dateInput = $(document.getElementById('<%= ComposedConfirmCompleteDate.ClientID %>'));
                    var $error = $modal.find('.output-error').first();
                    var caller = null;

                    $modal.on('hidden.bs.modal', function () {
                        caller = null;
                        $error.empty().hide();
                    }).find('[data-action]').on('click', function (e) {
                        e.preventDefault();
                        e.stopPropagation();

                        $error.hide();

                        var action = $(this).data('action');
                        if (action == 'confirm') {
                            var newDate = moment($dateInput.data('DateTimePicker').date());
                            if (!newDate.isValid()) {
                                $error.show().html('Date Completed is required field.');
                            } else if (!isImported && newDate.isBefore(dateCompleted)) {
                                $error.show().html('The new date completed cannot be less than the current date.');
                            } else {
                                caller.click();
                            }
                        } else if (action == 'cancel') {
                            modalManager.close($modal);
                        }
                    });

                    instance.showConfirm = function (el) {
                        if (!el)
                            return false;

                        if (!dateCompleted.isValid() || caller === el)
                            return true;

                        caller = el;
                        modalManager.show($modal);

                        $dateInput.data('DateTimePicker').date(dateCompleted);

                        return false;
                    };

                    return instance;
                })();

                $(document).ready(function () {
                    $('.questions.questions-text > div > div.question div.question-answer img').each(function () {
                        var $this = $(this);
                        var $anchor = $('<a>')
                            .addClass('img-zoom')
                            .attr('href', $this.prop('src'));

                        var caption = $this.prop('title');
                        if (!caption)
                            caption = $this.prop('alt');
                        if (caption)
                            $anchor.attr('data-caption', caption);

                        $this.wrap($anchor)
                            .tooltip({
                                title: caption,
                                placement: 'left',
                                delay: {
                                    show: 500,
                                    hide: 100
                                }
                            });
                    });

                    $('#desktop .questions .img-zoom').fancybox({});

                    $('.questions.questions-text > div > div.question > div.question-answer a').each(function () {
                        var href = $(this).attr("href");
                        if (!href)
                            return;

                        href = href.toLowerCase();

                        if (href.endsWith(".pdf"))
                            $(this).attr("target", "_blank");
                    });
                });
            })();

        </script>

        <script type="text/javascript">
            (function () {
                $('table.table-likert').each(function () {
                    var $this = $(this);

                    $this.parents('.tab-pane').each(function () {
                        $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                            .off('shown.bs.tab', update)
                            .on('shown.bs.tab', update);
                    });
                });

                $(window).on('resize', update);

                update();

                function update() {
                    $('table.table-likert').each(function () {
                        const $table = $(this);

                        let data = $table.data('likert');
                        if (!data)
                            $table.data('likert', data = {});

                        inSite.common.likert.update($table, data);
                    });
                }
            })();
        </script>

        <script type="text/javascript">
            $(function () {
                const data = [];

                $('.questions.questions-hotspot > div > .question > .hotspot-image').each(function () {
                    const $container = $(this)
                        .parents('.tab-pane').each(function () {
                            $('[data-bs-target="#' + this.id + '"][data-bs-toggle]')
                                .off('shown.bs.tab', resizeAll)
                                .on('shown.bs.tab', resizeAll);
                        })
                        .end();

                    const item = {};
                    item.image = window.attempts.hotspot.createImage($container.data('img'));
                    item.pins = window.attempts.hotspot.createPins(item.image, $container.data('pins'));
                    item.shapes = window.attempts.hotspot.createShapes($container.data('shapes'));
                    item.konva = {
                        container: null,
                        stage: null,
                        layer: null
                    };

                    for (let i = 0; i < item.shapes.length; i++)
                        item.shapes[i].label = inSite.common.stringHelper.toBase26(i);

                    this.append(item.konva.container = document.createElement('div'))

                    const stage = item.konva.stage = new Konva.Stage({
                        container: item.konva.container,
                        visible: false
                    });
                    stage.add(item.konva.layer = new Konva.Layer());
                    stage.on('scaled', () => onStageScaled(item.konva.layer));

                    item.image.initKonva(item.konva.layer);

                    if (item.shapes != null) {
                        for (let i = 0; i < item.shapes.length; i++)
                            item.shapes[i].initKonva(item.konva.layer);
                    }

                    if (item.pins.length > 0) {
                        for (let i = 0; i < item.pins.length; i++)
                            item.konva.layer.add(item.pins[i].obj);
                    }

                    data.push(item);

                    onStageScaled(item.konva.layer);
                });

                $(window).on('resize', resizeAll);

                if (document.fonts)
                    document.fonts.ready.then(function () {
                        for (let i = 0; i < data.length; i++) {
                            const stage = data[i].konva.stage;
                            if (stage)
                                stage.draw();
                        }
                    });

                function resizeAll() {
                    for (let i = 0; i < data.length; i++)
                        data[i].image.updateSize();
                }

                function onStageScaled(layer) {
                    const layerScale = layer.getAbsoluteScale().x;
                    const objScale = {
                        x: 1 / layerScale,
                        y: 1 / layerScale
                    };

                    const pins = layer.find('.pin');
                    for (let i = 0; i < pins.length; i++) {
                        const pin = pins[i];
                        for (let j = 0; j < pin.children.length; j++) {
                            pin.children[j].scale(objScale);
                        }
                    }
                }
            });
        </script>
    </insite:PageFooterContent>
</asp:Content>
