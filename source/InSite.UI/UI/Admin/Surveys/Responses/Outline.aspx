<%@ Page Language="C#" CodeBehind="Outline.aspx.cs" Inherits="InSite.Admin.Surveys.Responses.Outline" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>
<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .field-summary {
            padding-left: 24px;
        }

            .field-summary > i {
                position: absolute;
                margin-left: -24px;
                line-height: 23px;
            }

            .field-summary + .field-summary {
                margin-top: 8px;
            }

        h3 .label {
            padding: 1px 5px 2px;
        }

        div.settings h3{
            color: #777;
            margin-top: 0px;
            background-color: #f5f5f5;
            padding: 5px;
        }

    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:Alert runat="server" ID="LockStatus" />
    
    <div class="row mb-3">
        <div class="col-lg-12">
            <insite:Button runat="server" id="ViewHistoryLink" ButtonStyle="Default" Text="History" Icon="fas fa-fw fa-history" />
        </div>
    </div>

    <section class="pb-5 mb-md-2">        
        <h2 class="h4 mb-3">Survey Response</h2>

        <div class="row mb-3">
            <div class="col-lg-4">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">

                        <h3>
                            Response
                            <asp:Literal runat="server" ID="SubmissionState" />
                        </h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Started</label>
                            <div><asp:Literal runat="server" ID="ResponseStarted" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Completed</label>
                            <div><asp:Literal runat="server" ID="ResponseCompleted" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Time Taken</label>
                            <div><asp:Literal runat="server" ID="ResponseTimeTaken" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Last Revised By</label>
                            <div><asp:Literal runat="server" ID="LastRevisedBy" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconLink runat="server" id="ChangeResponseGroup" style="padding:8px" ToolTip="Change Response Group" Name="pencil" />
                            </div>
                            <label class="form-label">Group</label>
                            <div><asp:Literal runat="server" ID="ResponseGroup" Text="-" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <div class="float-end">
                                <insite:IconLink runat="server" id="ChangeResponsePeriod" style="padding:8px" ToolTip="Change Response Period" Name="pencil" />
                            </div>
                            <label class="form-label">Period</label>
                            <div><asp:Literal runat="server" ID="ResponsePeriod" Text="-" /></div>
                        </div>

                    </div>
                </div>

            </div>
                
            <div class="col-lg-3">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>
                            Respondent
                        </h3>

                        <insite:Container runat="server" ID="RespondentFields" Visible="false">
                            <div class="form-group mb-3">
                                <label class="form-label">Name</label>
                                <div><asp:Literal runat="server" ID="RespondentName" /></div>
                            </div>

                            <div class="form-group mb-3">
                                <label class="form-label">Email</label>
                                <div>
                                    <asp:Literal runat="server" ID="RespondentEmail" />
                                </div>
                            </div>
                        </insite:Container>
                    </div>
                </div>

            </div>

            <div class="col-lg-5">

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h3>
                            Survey
                        </h3>

                        <div class="form-group mb-3">
                            <label class="form-label">Internal Name</label>
                            <div><asp:Literal runat="server" ID="SurveyName" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">External Title</label>
                            <div><asp:Literal runat="server" ID="SurveyTitle" /></div>
                        </div>

                        <div class="form-group mb-3">
                            <label class="form-label">Survey Form Setup</label>
                            <div><asp:Literal runat="server" ID="SurveySummary" /></div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <h2 class="h4 mb-3">Respondent Answers</h2>
        <div class="row">
            <div class="col-lg-12">
                <div class="card border-0 shadow-lg">
                    <div class="card-body">
                        <div class="float-end">
                            <insite:Button runat="server" ID="EditLink" NavigateTarget="_blank" Text="Edit" Icon="fas fa-pencil" ButtonStyle="Default" />
                            <insite:Button runat="server" ID="LockLink" Text="Lock" Icon="fas fa-lock" ButtonStyle="Default" />
                            <insite:Button runat="server" ID="UnlockLink" Text="Unlock" Icon="fas fa-lock-open" ButtonStyle="Default" />
                            <insite:DownloadButton runat="server" ID="DownloadButton" />
                        </div>
                        <div class="clearfix"></div>           
                        <div class="row settings">
                            <div class="col-lg-12">
                                <asp:Repeater runat="server" ID="AnswerGroupRepeater">
                                    <ItemTemplate>
                                        <div class="mt-5 mb-5">
                                            <div class="card card-hover card-tile shadow bg-secondary">
                                                <div runat="server" class="card-header border-bottom-0" visible='<%# Eval("IsQuestionHeaderVisible") %>'>
                                                    <h2 class="mb-0"><%# Eval("QuestionHeader") %></h2>        
                                                </div>
                                                <div class="card-body bg-white">
                                                    <asp:Repeater runat="server" ID="AnswerItemRepeater">
                                                        <ItemTemplate>
                                                            <div class="mb-4" data-question-item='<%# Eval("Question") %>'>
                                                                <div>
                                                                    <%# Eval("QuestionBody") %>
                                                                </div>
                                                                <asp:Literal runat="server" ID="AnswerHtml" />
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
                    </div>
                </div>
            </div>
        </div>
    </section>
</asp:Content>
