<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPrintCompact.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPrintCompact" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>

    <link href="/library/fonts/font-awesome/7.1.0/css/all.min.css" rel="stylesheet" type="text/css">
    <link href="/library/fonts/roboto/roboto-v20-latin-ext_latin.css" rel="stylesheet" type="text/css">

    <style type="text/css">
        body {
            font-family: 'Roboto';
            font-size: 16px;
            margin: 0;
        }

        .form-text {
            font-size: 0.8em;
            color: #999;
        }

        table {
            border-collapse: collapse !important;
        }

            table.question {
                margin-bottom: 20px;
            }

                table.question > tbody > tr > td {
                    width: 45px;
                    font-size: 1.25em;
                    text-align: left;
                }

                    table.question > tbody > tr > td:first-child {
                        text-align: right;
                        padding-right: 20px;
                    }

    </style>

</head>
<body>
    <p runat="server" id="NoDataMessage" visible="false">No Questions.</p>

    <asp:Repeater runat="server" ID="ItemRepeater" Visible="false">
        <ItemTemplate>
            <table class='question' style="page-break-inside: avoid;">
                <tr>
                    <td>
                        <div><%# Eval("Sequence") %>.</div>
                    </td>
                    <td>
                        <div><%# Eval("Letter") %></div>
                    </td>
                </tr>
            </table>
        </ItemTemplate>
    </asp:Repeater>

    <script type="text/javascript">
        (function () {
            var questions = document.querySelectorAll('table.question');
            if (questions.length > 0)
                questions[questions.length - 1].style.marginBottom = '0 !important';
        })();
    </script>
</body>
</html>