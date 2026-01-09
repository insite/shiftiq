<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SubmissionReport.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Submissions.Controls.SubmissionReport" %>

<style>
    * {
        -webkit-box-sizing: border-box;
        -moz-box-sizing: border-box;
        box-sizing: border-box;
    }

    body {
        font-family: "Helvetica Neue", Helvetica, Arial, sans-serif;
        font-size: 14px;
        line-height: 1.42857143;
        color: #333;
        background-color: #fff;
    }

    body {
        margin: 0;
    }

    h1, h2 {
        margin-top: 20px;
        margin-bottom: 10px;
        font-family: inherit;
        font-weight: 500;
        line-height: 1.1;
        color: inherit;
    }

    h1 {
        font-size: 36px;
    }

    h2 {
        font-size: 30px;
    }

    a {
        color: #457897;
        text-decoration: none;
        background-color: transparent;
    }

    p {
        margin: 0 0 10px;
    }

    ul, ol {
        margin-top: 0;
        margin-bottom: 10px;
    }

    .row {
        margin-right: -15px;
        margin-left: -15px;
    }

        .row::after {
            clear: both;
            content: " ";
            display: block;
        }

        .row::before {
            display: table;
            content: " ";
        }

    .col-md-6, .col-md-12 {
        float: left;
        position: relative;
        min-height: 1px;
        padding-right: 15px;
        padding-left: 15px;
    }

    .col-md-6 {
        width: 50%;
    }

    .badge.bg-primary {
        background-color: #457897;
    }

    .label {
        display: inline;
        padding: .2em .6em .3em;
        font-size: 75%;
        font-weight: bold;
        line-height: 1;
        color: #fff;
        text-align: center;
        white-space: nowrap;
        vertical-align: baseline;
        border-radius: .25em;
    }

    .form-group {
        margin-bottom: 15px;
    }

    label {
        display: inline-block;
        max-width: 100%;
        margin-bottom: 5px;
        font-weight: bold;
    }
</style>

<h1 class="form-title" runat="server" id="FormTitle"></h1>

<div runat="server" id="RespondentRow" class="row settings">
    <div class="col-md-6">

        <div class="form-group mb-3">
            <label class="form-label"><asp:Literal runat="server" ID="RespondentFullName" /></label>
            <div>
                <asp:Literal runat="server" ID="RespondentEmail" />
            </div>
        </div>

    </div>
</div>

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
