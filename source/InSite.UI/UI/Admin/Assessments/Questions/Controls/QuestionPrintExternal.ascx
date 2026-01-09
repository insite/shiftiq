<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPrintExternal.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPrintExternal" %>

<%@ Import Namespace="Shift.Common" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <title runat="server" id="PageTitle"></title>

    <link href="/library/fonts/font-awesome/7.1.0/css/all.min.css" rel="stylesheet" type="text/css">

    <style type="text/css">

        body {
            font-family: Arial;
            font-size: 12pt;
            margin: 0;
        }

        p {
            margin: 0 0 0.49em;
        }

        .text-center {
            text-align: center !important;
        }

        img {
            max-width: 100%;
            max-height: 400px;
        }

        .instructions {
            margin-bottom: 2em;
        }

        .table {
            max-width: 100%;
            margin-bottom: 1em;
            border-collapse: collapse !important;
        }

            .table > thead > tr > th,
            .table > tbody > tr > th,
            .table > tfoot > tr > th,
            .table > thead > tr > td,
            .table > tbody > tr > td,
            .table > tfoot > tr > td {
                padding: 0.35em;
                vertical-align: baseline;
            }

            .table > thead > tr > th {
                vertical-align: bottom;
                text-align: left;
            }

        table.question {
            margin-bottom: 0.5in;
            margin-top: 0.80em;
            width: 100%;
        }

            table.question > tbody > tr.title > td {
                padding-bottom: 0.8em;
                vertical-align: top;
            }

                table.question > tbody > tr.title > td.qnum {
                    width: 0.35in;
                    padding-right: 0.25em;
                    text-align: right;
                }

            table.question > tbody > tr > td > .table-option {
                margin-bottom: 0;
            }

                table.question > tbody > tr > td > .table-option > tbody > tr > td {
                    padding: 0;
                }

                    table.question > tbody > tr > td > .table-option > tbody > tr > td.onum {
                        min-width: 0.26in;
                    }

                        table.question > tbody > tr > td > .table-option > tbody > tr > td.onum + td.otext {
                            padding-left: 0;
                        }

                    table.question > tbody > tr > td > .table-option > tbody > tr > td.otext {
                        padding-left: 0.35em;
                        padding-right: 0.35em;
                        padding-bottom: 0.35em;
                    }

                        table.question > tbody > tr > td > .table-option > tbody > tr > td.otext p {
                            margin: 0;
                        }

            table.question > tbody > tr > td > .table-composed {
            }

                table.question > tbody > tr > td > .table-composed > tbody > tr > td {
                    padding: 0.22em;
                    border-bottom: 1px solid #ddd;
                }

            table.question > tbody > tr > td > .table-boolean {
                width: 100%;
            }

                table.question > tbody > tr > td > .table-boolean > tbody > tr > td > i.fal {
                    font-size: 1.4em;
                    color: #ddd;
                }

            table.question > tbody > tr > td > .table-match {
                width: 100%;
            }

                table.question > tbody > tr > td > .table-match .option-left {
                    padding-left: 1.56em;
                }

                    table.question > tbody > tr > td > .table-match .option-left > .onum {
                        width: 1.1em;
                        margin-left: -1.56em;
                        position: absolute;
                    }

                    table.question > tbody > tr > td > .table-match .option-left > .oanswer {
                        float: left;
                        padding-right: 0.71em;
                    }

                table.question > tbody > tr > td > .table-match .option-right {
                    padding-left: 1.56em;
                }

                    table.question > tbody > tr > td > .table-match .option-right > .onum {
                        width: 1.1em;
                        margin-left: -1.56em;
                        position: absolute;
                    }

            table.question > tbody > tr > td > .table-likert {
                margin-bottom: 0;
            }

                table.question > tbody > tr > td > .table-likert > thead > tr > td.otext {
                    vertical-align: bottom;
                    text-align: center;
                }

                    table.question > tbody > tr > td > .table-likert > thead > tr > td.otext p {
                        margin: 0;
                    }

                table.question > tbody > tr > td > .table-likert > tbody > tr > td.onum {
                    vertical-align: top;
                }

                table.question > tbody > tr > td > .table-likert > tbody > tr > td.otext {
                    vertical-align: top;
                }

                    table.question > tbody > tr > td > .table-likert > tbody > tr > td.otext p {
                        margin: 0;
                    }

                table.question > tbody > tr > td > .table-likert > tbody > tr > td.ovalue {
                    vertical-align: middle;
                    text-align: center;
                }

                    table.question > tbody > tr > td > .table-likert > tbody > tr > td.ovalue > i.fal {
                        font-size: 1.4em;
                        color: #ddd;
                    }

            table.question > tbody > tr > td > .ordering-top-label {
                margin-bottom: 16px;
            }

            table.question > tbody > tr > td > .ordering-bottom-label {
                margin-top: 16px;
            }


            table.question > tbody > tr > td > .table-ordering {
                width: 100%;
            }

                table.question > tbody > tr > td > .table-ordering > tbody > tr > td {
                    padding: 0;
                }

                    table.question > tbody > tr > td > .table-ordering > tbody > tr > td.onum {
                        min-width: 1.5em;
                        width: 1.5em;
                        padding-top: 2px;
                    }

                        table.question > tbody > tr > td > .table-ordering > tbody > tr > td.onum + td.otext {
                            padding-left: 0;
                        }

                    table.question > tbody > tr > td > .table-ordering > tbody > tr > td.oicon {
                        text-align: center;
                        width: 1.5em;
                        vertical-align: top;
                        padding-right: 0.2em;
                    }

                        table.question > tbody > tr > td > .table-ordering > tbody > tr > td.oicon > i.fal {
                            font-size: 1.4em;
                            color: #ddd;
                        }

                    table.question > tbody > tr > td > .table-ordering > tbody > tr > td.otext {
                        padding-left: 0.35em;
                        padding-right: 0.35em;
                        padding-bottom: 0.35em;
                    }

                        table.question > tbody > tr > td > .table-ordering > tbody > tr > td.otext p {
                            margin: 0;
                        }

        .integer-part    { padding-right: 0.05em !important; }
        .fractional-part { padding-left: 0.05em !important; }

        .no-right-pad { padding-right: 0.05em !important; }
        .no-left-pad  { padding-left: 0.05em !important; }

        .small-right-pad { padding-right: 0.25em !important; }
        .small-left-pad  { padding-left: 0.25em !important; }

        .x-wide { padding-right: 5em !important; }

    </style>

    <style runat="server" id="ExcludeImagesStyle" type="text/css">
        table.question > tbody > tr.title > td.qtext img {
            display: none;
        }
    </style>

</head>
<body>
    <p runat="server" id="NoDataMessage" visible="false">There are no questions in this form.</p>

    <insite:Container runat="server" ID="FormContainer" Visible="false">
        <h1 runat="server" id="FormTitle"></h1>
        <div runat="server" id="FormDescription" style="page-break-after:always;" class="instructions"></div>

        <asp:Repeater runat="server" ID="QuestionRepeater">
            <ItemTemplate>
                <div runat="server" id="SectionContainer" style="page-break-before:always;" visible="false">
                    <h3 runat="server" id="SectionTitle" visible="false"></h3>
                    <div runat="server" id="SectionSummary" visible="false" style="margin:0.49em 0 1.34em 0;"></div>
                </div>

                <table class="question" style="page-break-inside:avoid !important;"><tbody>
                    <tr class="title">
                        <td class="qnum"><%# Eval("SecondarySequence") ?? Eval("PrimarySequence") %>.</td>
                        <td class="qtext"><%# Shift.Common.Markdown.ToHtml((string)Eval("AttemptQuestion.Text")) %></td>
                    </tr>
                    <tr>
                        <td></td>
                        <td>
                            <asp:MultiView runat="server" ID="ItemsMultiView">

                                <asp:View runat="server" ID="SingleCorrectItemsView">
                                    <asp:Repeater runat="server" ID="SingleCorrectItemRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-option">
                                                <%# GetOptionRepeaterTableHead("<th></th>", null) %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                <%# GetOptionRepeaterText(Container) %>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </asp:View>

                                <asp:View runat="server" ID="TrueOrFalseItemsView">
                                    <asp:Repeater runat="server" ID="TrueOrFalseItemRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-option">
                                                <%# GetOptionRepeaterTableHead("<th></th>", null) %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                <%# GetOptionRepeaterText(Container) %>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </asp:View>

                                <asp:View runat="server" ID="MultipleCorrectItemsView">
                                    <asp:Repeater runat="server" ID="MultipleCorrectItemRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-option">
                                                <%# GetOptionRepeaterTableHead("<th></th>", null) %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                <%# GetOptionRepeaterText(Container) %>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </asp:View>

                                <asp:View runat="server" ID="ComposedItemsView">
                                    <table class="table table-composed" style="width: 100%;">
                                        <tbody>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                            <tr><td>&nbsp;</td></tr>
                                        </tbody>
                                    </table>
                                </asp:View>

                                <asp:View runat="server" ID="BooleanTableItemsView">
                                    <asp:Repeater runat="server" ID="BooleanTableItemRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-boolean">
                                                <thead>
                                                    <tr>
                                                        <th style="width:1.25em;"></th>
                                                        <%# GetOptionRepeaterTableHeadTitleCols() %>
                                                        <th class="text-center" style="width:3.125em;">True</th>
                                                        <th class="text-center" style="width:3.125em;">False</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                <%# GetOptionRepeaterText(Container) %>
                                                <td class="text-center">
                                                    <i class="fal fa-square"></i>
                                                </td>
                                                <td class="text-center">
                                                    <i class="fal fa-square"></i>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </asp:View>

                                <asp:View runat="server" ID="MatchesItemsView">
                                    <table class="table table-match">
                                        <thead>
                                            <tr>
                                                <th>Left</th>
                                                <th>Right</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            <tr>
                                                <td>
                                                    <asp:Repeater runat="server" ID="MatchesLeftRepeater">
                                                        <ItemTemplate>
                                                            <div class="option option-left">
                                                                <div class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</div>
                                                                <div class="oanswer">___</div>
                                                                <div class="otext"><%# Shift.Common.Markdown.ToHtml((string)Eval("LeftText")) %></div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>
                                                <td>
                                                    <asp:Repeater runat="server" ID="MatchesRightRepeater">
                                                        <ItemTemplate>
                                                            <div class="option option-right">
                                                                <div class="onum"><%# Container.ItemIndex + 1 %>.</div>
                                                                <div class="otext"><%# Shift.Common.Markdown.ToHtml((string)Container.DataItem) %></div>
                                                            </div>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </asp:View>

                                <asp:View runat="server" ID="LikertItemsView">
                                    <table class="table table-likert">
                                        <thead>
                                            <asp:Repeater runat="server" ID="LikertColumnRepeater">
                                                <HeaderTemplate>
                                                    <tr>
                                                        <td></td>
                                                        <td></td>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <td class="otext"><%# Shift.Common.Markdown.ToHtml((string)Container.DataItem) %></td>
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
                                                        <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                        <td class="otext"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                                                        <asp:Repeater runat="server" ID="LikertOptionRepeater">
                                                            <ItemTemplate>
                                                                <td class="ovalue"><i class="fal fa-square"></i></td>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </tr>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </tbody>
                                    </table>
                                </asp:View>

                                <asp:View runat="server" ID="HotspotItemsView">
                                    <asp:Literal runat="server" ID="HotspotImage" />
                                </asp:View>

                                <asp:View runat="server" ID="OrderingItemsView">
                                    <div runat="server" id="OrderingTopLabel" class="ordering-top-label" visible="false"></div>

                                    <asp:Repeater runat="server" ID="OrderingOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-ordering">
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                                <td class="oicon">
                                                    <i class="fal fa-square"></i>
                                                </td>
                                                <td class="otext"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>

                                    <div runat="server" id="OrderingBottomLabel" class="ordering-bottom-label" visible="false"></div>
                                </asp:View>

                            </asp:MultiView>
                        </td>
                    </tr>
                </tbody></table>
            </ItemTemplate>
        </asp:Repeater>
    </insite:Container>

    <script type="text/javascript">
        (function () {
            var questions = document.querySelectorAll('table.question');
            if (questions.length > 0) {
                var lastTable = questions[questions.length - 1];
                lastTable.style.marginBottom = '0 !important';

                var tBody = lastTable.querySelector('tbody');
                if (!tBody)
                    tBody = lastTable;

                var lastTr = document.createElement('tr');
                lastTr.innerHTML = '<td colspan="2" style="padding-top:2.23em; text-align:center; font-weight:bold;">THIS IS THE FINAL PAGE OF QUESTIONS IN THIS EXAM</td>';
                tBody.appendChild(lastTr);
            }
        })();
    </script>

    <script type="text/javascript">
        (function () {
            var likertTables = document.querySelectorAll('table.table-likert');
            for (var i = 0; i < likertTables.length; i++) {
                var table = likertTables[i];
                var headerCells = table.tHead.rows[0].cells;

                table.style.width = '100%';
                headerCells[0].style.width = '1px';

                {
                    var minWidth = null;

                    for (var j = 2; j < headerCells.length; j++) {
                        var cell = headerCells[j];

                        cell.style.width = '1px';

                        var style = getComputedStyle(cell);
                        var cellWidth = cell.clientWidth - parseFloat(style.paddingLeft) - parseFloat(style.paddingRight)

                        if (minWidth == null || minWidth < cellWidth)
                            minWidth = Math.floor(cellWidth);
                    }

                    for (var j = 2; j < headerCells.length; j++)
                        headerCells[j].style.width = String(minWidth) + 'px';
                }
            }
        })();
    </script>
    
</body>
</html>