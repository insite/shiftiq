<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionsReport.ascx.cs" Inherits="InSite.Admin.Workflow.Forms.Controls.QuestionsReport" %>

<asp:Literal runat="server" ID="DefaultCss" />

<style type="text/css">
    body {
        font-family: Calibri, Helvetica, Arial;
        font-size: 20px;
    }

    p {
        margin: 0;
    }

    .survey-page {
        padding: 0px;
        margin-top: 1em;
        font-size: 1.2em;
        font-weight: bold;
    }

    .question {
        padding: 0 2em 2em 2em;
        margin-top: 1em;
        width: 1000px;
    }

    .question > .header {
        font-size: 1.2em;
    }

    .question > .header {
        content: ' ';
        display: block;
        clear: both;
    }

    .question > .header > .title:after {
    }

    .question > .header > .title .label {
        border-radius:.25em;
        float: left;
        color:white;
        font-weight:bold;
        font-size:0.9em;
        padding:0 10px 0 10px;
        margin-right:0.4em;
    }
    .question > .header > .title .label.default {
        background-color:#777;
    }
    .question > .header > .title .label.primary {
        background-color:#337ab7;
    }
    .question > .header > .title .label.success {
        background-color:#5cb85c;
    }
    .question > .header > .title .label.info {
        background-color:#5bc0de;
    }
    .question > .header > .title .label.warning {
        background-color:#f0ad4e;
    }
    .question > .header > .title .label.danger {
        background-color:#f74f78;
    }

    .question > .field {
        margin-left: 0.8em;
    }

    .question > .field > .header > .label {
        float: left;
        padding-right: 0.4em;
    }

    .question > .field > table {
        width: 100%;
        border-spacing: 0;
        border-collapse: collapse;
        margin: 1.5em 0 0;
    }

    .question > .field > table > thead > tr > th {
        text-align: center;
        padding: 5px;
    }

    .question > .field > table > tbody > tr > th {
        text-align: left;
        padding: 5px;
    }

    .question > .field > table > tbody > tr > td {
        text-align: center;
        padding: 5px;
    }

    .question > .field > table td.radio-button:before {
        font-family: 'Font Awesome 7 Pro';
        font-weight: 300;
        display: inline-block;
        content: '\f111';
    }

    .question > .field > .header {
        margin-bottom: 0.8em;
    }

    .question > .field > .header > .title {
    }

    .question > .field > .field-comment {
        height: 2em;
    }

    .question > .field > .field-fileupload {
        height: 2em;
    }

    .question > .field > .field-number {
        height: 2em;
    }

    .question > .field > .field-date {
        height: 2em;
    }

    .question > .field > .field-text {
        height: 2em;
    }

    .question > .field > .field-radiolist,
    .question > .field > .field-checklist,
    .question > .field > .field-selection {
        margin-left: 0.8em;
    }

    .question > .field > .field-radiolist > .option,
    .question > .field > .field-checklist > .option,
    .question > .field > .field-selection > .option {
        padding-left: 1.5em;
        line-height: 2em;
    }

    .question > .field > .field-radiolist > .option:before,
    .question > .field > .field-checklist > .option:before,
    .question > .field > .field-selection > .option:before {
        font-family: 'Font Awesome 7 Pro';
        font-weight: 300;
        display: block;
        float: left;
        margin-left: -1.5em;
    }

    .question > .field > .field-radiolist > .option:after,
    .question > .field > .field-checklist > .option:after,
    .question > .field > .field-selection > .option:after {
        content: ' ';
        display: block;
        clear: both;
    }

    .question > .field > .field-radiolist > .option:before {
        content: '\f111';
    }

    .question > .field > .field-checklist > .option:before {
        content: '\f0c8';
    }

    .question > .field > .field-selection > .option:before {
        content: '\f111';
    }

</style>

<h2 runat="server" id="Title"></h2>

<div>

    <p runat="server" id="ErrorMessage" visible="false"></p>

    <asp:Repeater runat="server" ID="PageRepeater">
        <ItemTemplate>
            
            <div class="survey-page">
                
            </div>

            <asp:Repeater runat="server" ID="QuestionGroupRepeater">
                <ItemTemplate>
                    <div class="question" style="page-break-inside:avoid;">
                        <%# Eval("QuestionHeader") %>

                        <asp:Repeater runat="server" ID="QuestionItemRepeater">
                            <ItemTemplate>
                                <%# Eval("QuestionBody") %>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </ItemTemplate>
    </asp:Repeater>

</div>