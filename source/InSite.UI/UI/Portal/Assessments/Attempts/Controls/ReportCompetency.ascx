<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCompetency.ascx.cs" Inherits="InSite.UI.Portal.Assessments.Attempts.Controls.ReportCompetency" %>

<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=edge">

    <title runat="server" id="PageTitle"></title>

    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&amp;display=swap" rel="stylesheet">
    <link rel="stylesheet" media="screen" href="/library/fonts/font-awesome/7.1.0/css/all.min.css">

    <style type="text/css">
        body {
            margin: 0;
            padding: 0;
            font-family: "Inter", sans-serif;
            font-size: 16px;
            color: #212529;
        }

        h6, .h6, h5, .h5, h4, .h4, h3, .h3, h2, .h2, h1, .h1 {
            margin-top: 0;
            margin-bottom: 1rem;
            font-weight: 600;
            line-height: 1.3;
            color: #000000;
        }

        p {
            margin-top: 0;
            margin-bottom: 1.125rem;
        }

        .fw-bold {
            font-weight: 700 !important;
        }

        .fw-semibold {
            font-weight: 600 !important;
        }

        .fw-medium {
            font-weight: 500 !important;
        }

        .text-end {
            text-align: right !important;
        }

        .text-center {
            text-align: center !important;
        }

        .mt-1 {
            margin-top: 0.25rem !important;
        }

        .mt-3 {
            margin-top: 1rem !important;
        }

        .mb-0 {
            margin-bottom: 0 !important;
        }

        .mb-1 {
            margin-bottom: 0.25rem !important;
        }

        .mb-2 {
            margin-bottom: 0.5rem !important;
        }

        .mb-3 {
            margin-bottom: 1rem !important;
        }

        .mb-4 {
            margin-bottom: 1.5rem !important;
        }

        .ms-1 {
            margin-left: 0.25rem !important;
        }

        .m2-2 {
            margin-left: 0.5rem !important;
        }

        .m2-3 {
            margin-left: 1rem !important;
        }

        .ps-1 {
            padding-left: 0.25rem !important;
        }

        .ps-2 {
            padding-left: 0.5rem !important;
        }

        .ps-3 {
            padding-left: 1rem !important;
        }

        .ps-4 {
            padding-left: 1.5rem !important;
        }

        .pb-3 {
            padding-bottom: 1rem !important;
        }

        .pb-4 {
            padding-bottom: 1.5rem !important;
        }

        .logo-container {
            font-size: 3rem;
            font-weight: bold;
        }

            .logo-container img {
                max-height: 60px;
            }

        table {
            border-collapse: collapse;
        }

        .table {
            width: 100%;
            vertical-align: top;
        }

            .table > thead {
                vertical-align: bottom;
            }

                .table > thead td {
                    background-color: #f6f9fc;
                    border-bottom: solid 1px #c3d0dc;
                    font-weight: 700;
                }

            .table > :not(caption) > * > * {
                padding: 0.75rem 0.75rem;
                border-bottom: solid 1px #e3e9ef;
            }

        .col-w-200 {
            width: 200px;
        }
    </style>
</head>
<body>

    <div runat="server" id="LogoContainer" class='logo-container ps-3 pb-3'>
        
    </div>

    <h1 runat="server" id="ReportTitle"></h1>

    <asp:Repeater runat="server" ID="FieldsRepeater">
        <HeaderTemplate>
            <div class="mt-1 mb-4">
        </HeaderTemplate>
        <ItemTemplate>
            <div class="mb-2">
                <span class="fw-bold"><%# Eval("Label") %>:</span>
                <span class="ms-1"><%#: Eval("Value") %></span>
            </div>
        </ItemTemplate>
        <FooterTemplate>
            </div>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Repeater runat="server" ID="AreaRepeater">
        <HeaderTemplate>
            <table class="table mb-4">
                <thead>
                    <tr>
                        <td><%# GetLabel("[Attempts.ReportCompetency].[CompetencyGrouping]") %></td>
                        <td class="text-end col-w-200"><%# GetLabel("[Attempts.ReportCompetency].[MaximumPossibleScore]") %></td>
                        <td class="text-end col-w-200"><%# GetLabel("[Attempts.ReportCompetency].[TestTakeScore]") %></td>
                    </tr>
                </thead>
                <tbody>
                    <tr class="fw-semibold">
                        <td><%# GetLabel("[Attempts.ReportCompetency].[OverallScore]") %></td>
                        <td class="text-end"><%# OverallInfo.QuestionsPoints %></td>
                        <td class="text-end"><%# GetAreaScore(OverallInfo.AnswerPoints) %></td>
                    </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("StandardTitle") %></td>
                <td class="text-end"><%# Eval("QuestionsPoints", "{0:n2}") %></td>
                <td class="text-end"><%# GetAreaScore((decimal)Eval("AnswerPoints")) %></td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
                </tbody>
            </table>
        </FooterTemplate>
    </asp:Repeater>

    <asp:Literal runat="server" ID="Footer" />

</body>
</html>
