<%@ Page Language="C#" CodeBehind="View.aspx.cs" Inherits="InSite.UI.Admin.Assessments.QuizAttempts.View" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="~/UI/Admin/Contacts/People/Controls/PersonInfo.ascx" TagName="PersonDetail" TagPrefix="uc" %>	

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    
    <insite:Alert runat="server" ID="ScreenStatus" />
    
    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="AttemptPanel" Title="Attempt" Icon="far fa-tasks" IconPosition="BeforeText">
            <section>

                <h2 class="h4 mt-4 mb-3">Attempt</h2>

                <div class="row">
                        
                    <div class="col-lg-3">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">

                                <h3>Learner</h3>
                                <uc:PersonDetail runat="server" ID="PersonDetail" />
                            </div>
                        </div>
                    </div>
                
                    <div class="col-lg-3">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <h3>Quiz</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Quiz Type</label>
                                    <div>
                                        <asp:Literal runat="server" ID="QuizType" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Quiz Name</label>
                                    <div>
                                        <asp:HyperLink runat="server" ID="QuizName" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>

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
                                    <label class="form-label">Completed</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptCompletedOutput" />
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
                                    <label class="form-label">Mistakes</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptMistakes" />
                                    </div>
                                </div>

                                <div runat="server" id="AttemptKphField" class="form-group mb-3">
                                    <label class="form-label">KPH</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptKph" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">WPM</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptWpm" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">CPM</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptCpm" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Accuracy</label>
                                    <div>
                                        <asp:Literal runat="server" ID="AttemptAccuracy" />
                                    </div>
                                </div>

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

                <asp:MultiView runat="server" ID="AnswerMultiView">
                    <asp:View runat="server" ID="TypingSpeedView">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <div class="row">
                                    <div class="col-lg-6 mb-3 mb-lg-0">
                                        <div class="card">
                                            <div class="card-body">

                                                <h3>Question</h3>

                                                <div runat="server" id="TypingSpeedQuestion" class="fs-lg"></div>

                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="card">
                                            <div class="card-body">

                                                <h3>Answer</h3>

                                                <div runat="server" id="TypingSpeedAnswer" class="fs-lg quiz-text"></div>

                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <insite:PageFooterContent runat="server">
                            <script type="text/javascript">
                                (function () {
                                    const questionEl = document.getElementById('<%= TypingSpeedQuestion.ClientID %>');
                                    const answerEl = document.getElementById('<%= TypingSpeedAnswer.ClientID %>');

                                    attemptHelper.highlightDifference(questionEl, answerEl);

                                    answerEl.classList.add('inited');
                                })();
                            </script>
                        </insite:PageFooterContent>
                    </asp:View>
                    <asp:View runat="server" ID="TypingAccuracyView">
                        <asp:Repeater runat="server" ID="TypingAccuracyQuestionRepeater">
                            <ItemTemplate>
                                <div class="card border-0 shadow-lg mb-3">
                                    <div class="card-body">

                                        <h3>Question <%# Eval("Number") %></h3>

                                        <asp:Repeater runat="server" ID="QuestionColumnRepeater">
                                            <HeaderTemplate>
                                                <div class="row quiz-sample">
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                </div>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <div class="col mb-3">
                                                    <div class="card h-100">
                                                        <div class="card-body">

                                                            <asp:Repeater runat="server" ID="RowRepeater">
                                                                <ItemTemplate>
                                                                    <div class="form-group mb-3">
                                                                        <label class="form-label">
                                                                            <%# Eval("Label") %>
                                                                        </label>
                                                                        <div class="fs-lg form-control" data-index='<%# Eval("Index") %>'><%# Eval("Value") %></div>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>

                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                        <h3>Answer</h3>

                                        <asp:Repeater runat="server" ID="AnswerColumnRepeater">
                                            <HeaderTemplate>
                                                <div class="row quiz-answer">
                                            </HeaderTemplate>
                                            <FooterTemplate>
                                                </div>
                                            </FooterTemplate>
                                            <ItemTemplate>
                                                <div class="col mb-3">
                                                    <div class="card h-100">
                                                        <div class="card-body">

                                                            <asp:Repeater runat="server" ID="RowRepeater">
                                                                <ItemTemplate>
                                                                    <div class="form-group mb-3">
                                                                        <label class="form-label">
                                                                            <%# Eval("Label") %>
                                                                        </label>
                                                                        <div class="fs-lg form-control quiz-text" data-index='<%# Eval("Index") %>'><%# Eval("Answer") %></div>
                                                                    </div>
                                                                </ItemTemplate>
                                                            </asp:Repeater>

                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>

                        <insite:PageFooterContent runat="server">
                            <script type="text/javascript">
                                (function () {
                                    const mapping = {};
                                    mapItem(document.querySelectorAll('.quiz-sample [data-index]'), 'sample')
                                    mapItem(document.querySelectorAll('.quiz-answer [data-index]'), 'answer')

                                    for (const index in mapping) {
                                        if (!mapping.hasOwnProperty(index))
                                            continue;

                                        const item = mapping[index];
                                        attemptHelper.highlightDifference(item.sample, item.answer);
                                        item.answer.classList.add('inited');
                                    }

                                    function mapItem(list, prop) {
                                        for (const item of list) {
                                            const index = item.dataset.index;

                                            if (!mapping.hasOwnProperty(index))
                                                mapping[index] = {};

                                            mapping[index][prop] = item;
                                        }
                                    }
                                })();
                            </script>
                        </insite:PageFooterContent>
                    </asp:View>
                </asp:MultiView>

            </section>
        </insite:NavItem>
    </insite:Nav>

    <div class="mt-3">
        <insite:CancelButton runat="server" ID="CloseButton" Text="Close" />
    </div>

    <insite:PageHeadContent runat="server">
        <link rel="stylesheet" href="/ui/portal/assessments/quizattempts/content/typing-speed.css" />

        <style type="text/css">
            .quiz-text {
                overflow-x: auto;
            }

            .quiz-text.inited::before {
                display: none;
            }
        </style>

        <script type="text/javascript">
            (function () {
                if (window.attemptHelper)
                    return;

                const instance = window.attemptHelper = {
                    highlightDifference: highlightDifference
                };

                function highlightDifference(elSample, elAnswer) {
                    const sampleText = clearText(elSample.innerText);

                    let answerText = clearText(elAnswer.innerText);
                    let wordEl = null, charEl = null, prevCh = null;

                    elAnswer.replaceChildren();

                    if (answerText.length == 0) {
                        elAnswer.innerHTML = '&nbsp;';
                        return;
                    }

                    createWord();

                    for (let i = 0; i < answerText.length; i++) {
                        const ch = answerText[i];
                        const isValid = i < sampleText.length && sampleText[i] == ch;

                        createChar(isValid);

                        if (ch === '\n') {
                            flushWord();
                            charEl.innerHTML = '&nbsp;\n';
                            elAnswer.appendChild(charEl);
                        } else if (ch === ' ') {
                            charEl.innerHTML = '&nbsp;';
                            wordEl.appendChild(charEl);
                        } else {
                            charEl.innerText = ch;
                            if (prevCh === ' ')
                                flushWord();
                            wordEl.appendChild(charEl);
                        }

                        prevCh = ch;
                    }

                    flushWord();

                    function createWord() {
                        wordEl = document.createElement('div');
                        wordEl.className = 'qword';
                    }

                    function createChar(isValid) {
                        charEl = document.createElement('span');
                        charEl.className = 'qchar ' + (isValid ? 'correct' : 'incorrect');
                    }

                    function flushWord() {
                        if (wordEl.childNodes.length > 0)
                            elAnswer.appendChild(wordEl);

                        createWord();
                    }

                    function clearText(value) {
                        return value
                            .replaceAll('\r', '')
                            .replaceAll('\f', '')
                            .replaceAll('\t', '')
                            .replaceAll('\v', '')
                            .replaceAll('\0', '');
                    }
                }
            })();
        </script>
    </insite:PageHeadContent>
</asp:Content>
