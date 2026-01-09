<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionPrintInternal.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.QuestionPrintInternal" %>

<%@ Import Namespace="Shift.Common" %>

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
            font-size: 18px;
            margin: 0;
        }

        img {
            max-width: 100%;
            max-height: 400px;
        }

            img.hotspot-image {
                margin-bottom: 15px;
            }

        .text-center {
            text-align: center !important;
        }

        .form-text {
            font-size: 0.8em;
            color: #999;
        }

        table {
            border-collapse: collapse !important;
        }

            table.question {
                width: 100%;
                margin-bottom: 100px;
            }

                table.question > tbody > tr > td {
                    vertical-align: top;
                }

                table.question > tbody > tr.row-title > td.num {
                    text-align: center;
                    width: 45px;
                }

                    table.question > tbody > tr.row-title > td.num > .primary {
                        font-weight: bold;
                        font-size: 1.25em;
                        margin-bottom: 8px;
                    }

                    table.question > tbody > tr.row-title > td.num > .secondary {
                        font-size: 0.75em;
                        font-weight: bold;
                        display: inline-block;
                        min-width: 10px;
                        padding: 3px 7px;
                        line-height: 1;
                        color: #fff;
                        text-align: center;
                        white-space: nowrap;
                        vertical-align: middle;
                        background-color: #777;
                        border-radius: 10px;
                    }

                table.question > tbody > tr.row-title > td.title {
                    padding-left: 20px;
                    padding-top: 3px;
                }

                    table.question > tbody > tr.row-title > td.title p {
                        margin-top: 0;
                    }

                table.question > tbody > tr.row-title > td.props {
                    width: 35%;
                    padding-left: 20px;
                }

                    table.question > tbody > tr.row-title > td.props table {
                        float: right;
                        width: 100%;
                    }

                        table.question > tbody > tr.row-title > td.props table > tbody > tr:nth-child(even) {
                            background-color: #F5F5F5;
                        }

                        table.question > tbody > tr.row-title > td.props table > tbody > tr > td {
                            border: 1px solid #ddd;
                            padding: 5px;
                            font-size: 0.85em;
                            vertical-align: top;
                        }

                            table.question > tbody > tr.row-title > td.props table > tbody > tr > td.name {
                                white-space: nowrap;
                            }

                            table.question > tbody > tr.row-title > td.props table > tbody > tr > td.value {
                                padding-left: 10px;
                            }

                table.question > tbody > tr.row-body {
                }

                    table.question > tbody > tr.row-body > td {
                        padding-left: 20px;
                    }

                        table.question > tbody > tr.row-body > td p {
                            margin: 0;
                        }

                        table.question > tbody > tr.row-body > td > .table-option {
                            margin-bottom: 0;
                        }

                            table.question > tbody > tr.row-body > td > .table-option > thead > tr > th.otext + th.otext {
                                padding-left: 15px;
                            }

                            table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td {
                                padding: 0 0 5px 0;
                                vertical-align: top;
                            }

                                table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.oicon {
                                    min-width: 30px;
                                }

                                table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.onum {
                                    min-width: 25px;
                                }

                                    table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.onum + td.otext {
                                        padding-left: 0;
                                    }

                                table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.otext {
                                }

                                    table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.otext p {
                                        margin: 0;
                                    }

                                    table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.otext + td.otext {
                                        padding-left: 15px;
                                    }

                                table.question > tbody > tr.row-body > td > .table-option > tbody > tr > td.opoints {
                                    white-space: nowrap;
                                    padding-left: 15px;
                                    line-height: 1.45;
                                }

                        table.question > tbody > tr.row-body > td > .table-composed {
                        }

                        table.question > tbody > tr.row-body > td > .table-boolean {
                        }

                            table.question > tbody > tr.row-body > td > .table-boolean > tbody > tr > td {
                                padding: 0 0 5px 0;
                                vertical-align: top;
                            }

                                table.question > tbody > tr.row-body > td > .table-boolean > tbody > tr > td.onum {
                                    min-width: 25px;
                                }

                                table.question > tbody > tr.row-body > td > .table-boolean > tbody > tr > td.oicon {
                                    text-align: center;
                                    width: 3em;
                                }

                                table.question > tbody > tr.row-body > td > .table-boolean > tbody > tr > td.opoints {
                                    white-space: nowrap;
                                    padding-left: 15px;
                                    line-height: 1.45;
                                }

                        table.question > tbody > tr.row-body > td > .table-match {
                        }

                            table.question > tbody > tr.row-body > td > .table-match > tbody > tr > td,
                            table.question > tbody > tr.row-body > td > .table-match > tbody > tr > th {
                                vertical-align: top;
                                text-align: left;
                            }

                            table.question > tbody > tr.row-body > td > .table-match > tbody > tr > th {
                                padding-bottom: 10px;
                            }

                            table.question > tbody > tr.row-body > td > .table-match > tbody > tr + tr > th {
                                padding-top: 15px;
                            }

                            table.question > tbody > tr.row-body > td > .table-match > tbody > tr > td {
                                padding: 0 0 5px 0;
                            }

                                table.question > tbody > tr.row-body > td > .table-match > tbody > tr > td + td {
                                    padding-left: 15px;
                                }

                                table.question > tbody > tr.row-body > td > .table-match > tbody > tr > td.mpoints {
                                    white-space: nowrap;
                                    padding-left: 15px;
                                    line-height: 1.45;
                                }


                        table.question > tbody > tr.row-body > td > .table-likert {
                            margin-bottom: 0;
                        }

                            table.question > tbody > tr.row-body > td > .table-likert > thead > tr > td {
                                vertical-align: bottom;
                                text-align: center;
                            }

                                table.question > tbody > tr.row-body > td > .table-likert > thead > tr > td.otext {
                                    padding: 5px;
                                }

                                    table.question > tbody > tr.row-body > td > .table-likert > thead > tr > td.otext p {
                                        margin: 0;
                                    }

                            table.question > tbody > tr.row-body > td > .table-likert > tbody > tr > td.onum {
                                vertical-align: top;
                                padding: 5px 0;
                            }

                            table.question > tbody > tr.row-body > td > .table-likert > tbody > tr > td.otext {
                                vertical-align: top;
                                padding: 5px;
                            }

                                table.question > tbody > tr.row-body > td > .table-likert > tbody > tr > td.otext p {
                                    margin: 0;
                                }

                            table.question > tbody > tr.row-body > td > .table-likert > tbody > tr > td.ovalue {
                                text-align: center;
                                vertical-align: middle;
                                border-color: #ddd;
                                border-style: dashed;
                                border-width: 1px;
                            }

                        table.question > tbody > tr.row-body > td > .ordering-top-label {
                            margin-bottom: 16px;
                        }

                        table.question > tbody > tr.row-body > td > .ordering-bottom-label {
                            margin-top: 16px;
                        }

                        table.question > tbody > tr.row-body > td > .table-ordering-solution {
                            margin-bottom: 0;
                        }

                            table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td {
                                padding: 0 0 5px 0;
                                vertical-align: top;
                            }

                                table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td.onum {
                                    min-width: 25px;
                                }

                                table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td.otext {
                                }

                                    table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td.otext p {
                                        margin: 0;
                                    }

                                table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td.opoints {
                                    white-space: nowrap;
                                    padding-left: 15px;
                                    line-height: 1.45;
                                }

                                table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td.separator {
                                    height: 0;
                                }

                                table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td > table.table-ordering-option td {
                                    vertical-align: top;
                                }

                                    table.question > tbody > tr.row-body > td > .table-ordering-solution > tbody > tr > td > table.table-ordering-option td.onum {
                                        padding-right: 5px;
                                    }

                table.question > tbody > tr.row-comments > td > div.comment {
                    position: relative;
                    margin: 0 8px 20px 8px;
                }

                    table.question > tbody > tr.row-comments > td > div.comment + div.comment {
                        border-top: 1px solid #eee;
                        padding-top: 20px;
                    }

                    table.question > tbody > tr.row-comments > td > div.comment .comment-author {
                        font-size: 1.125em;
                    }

                    table.question > tbody > tr.row-comments > td > div.comment .comment-posted-on {
                        font-size: 0.8125em;
                        color: #999;
                        font-weight: 400;
                    }

                    table.question > tbody > tr.row-comments > td > div.comment .comment-flag {
                        position: absolute;
                        right: 0;
                    }

        .integer-part    { padding-right: 0.05em !important; }
        .fractional-part { padding-left: 0.05em !important; }

        .no-right-pad { padding-right: 0.05em !important; }
        .no-left-pad  { padding-left: 0.05em !important; }

        .small-right-pad { padding-right: 0.25em !important; }
        .small-left-pad  { padding-left: 0.25em !important; }

        .x-wide { padding-right: 5em !important; }

        .text-primary { color: #457897 !important; }
        .text-secondary { color: #9db6cb !important; }
        .text-success { color: #16c995 !important; }
        .text-info { color: #6a9bf4 !important; }
        .text-warning { color: #ffb15c !important; }
        .text-danger { color: #f74f78 !important; }
        .text-light { color: #fff !important; }
        .text-dark { color: #121519 !important; }
        .text-black { color: #000 !important; }
        .text-white { color: #fff !important; }
        .ms-1 { margin-left: 0.25rem !important; }

    </style>

    <style runat="server" id="ExcludeImagesStyle" type="text/css">
        table.question > tbody > tr.row-title > td.title img {
            display: none;
        }
    </style>

</head>
<body>
    <p runat="server" id="NoDataMessage" visible="false">No Questions.</p>

    <asp:Repeater runat="server" ID="QuestionRepeater">
        <ItemTemplate>
            <table class='question' style="page-break-inside: avoid;">
                <tr class="row-title">
                    <td rowspan="2" class="num">
                        <div class="primary"><%# Eval("PrimarySequence") %></div>
                        <div runat="server" class="secondary" visible='<%# Eval("SecondarySequence") != null %>'><%# Eval("SecondarySequence") %></div>
                    </td>
                    <td class="title"><%# Shift.Common.Markdown.ToHtml((string)Eval("AttemptQuestion.Text")) %></td>
                    <td rowspan="2" class="props">
                        <asp:Repeater runat="server" ID="PropertyRepeater">
                            <HeaderTemplate>
                                <table><tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody></table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="name"><%# Eval("Item1") %></td>
                                    <td class="value" style='<%# (string)Eval("Item2") == "Reference" ? "word-break:break-word; white-space:pre-wrap;" : string.Empty %>'><%# Eval("Item2") %></td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
                <tr class="row-body">
                    <td>
                        <asp:MultiView runat="server" ID="ItemsMultiView">

                            <asp:View runat="server" ID="SingleCorrectItemsView">
                                <asp:Repeater runat="server" ID="SingleCorrectItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-option">
                                            <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="oicon"><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <%# GetOptionRepeaterText(Container) %>
                                            <td class="opoints form-text">
                                                &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>
                            </asp:View>

                            <asp:View runat="server" ID="TrueOrFalseItemsView">
                                <asp:Repeater runat="server" ID="TrueOrFalseItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-option">
                                            <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="oicon"><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <%# GetOptionRepeaterText(Container) %>
                                            <td class="opoints form-text">
                                                &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>
                            </asp:View>

                            <asp:View runat="server" ID="MultipleCorrectItemsView">
                                <asp:Repeater runat="server" ID="MultipleCorrectItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table-option">
                                            <%# GetOptionRepeaterTableHead("<th></th><th></th>", null) %>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="oicon"><%# GetMultipleCorrectOptionIcon((bool?)Eval("IsTrue")) %></td>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <%# GetOptionRepeaterText(Container) %>
                                            <td class="opoints form-text">
                                                &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>
                            </asp:View>

                            <asp:View runat="server" ID="BooleanTableItemsView">
                                <asp:Repeater runat="server" ID="BooleanTableItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table-boolean">
                                            <thead>
                                                <tr>
                                                    <th></th>
                                                    <%# GetOptionRepeaterTableHeadTitleCols() %>
                                                    <th class="oicon">True</th>
                                                    <th class="oicon">False</th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <%# GetOptionRepeaterText(Container) %>
                                            <td class="oicon"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), true) %></td>
                                            <td class="oicon"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), false) %></td>
                                            <td class="opoints form-text">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %></td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>
                            </asp:View>

                            <asp:View runat="server" ID="MatchesItemsView">
                                <table class="table table-match"><tbody>

                                    <asp:Repeater runat="server" ID="MatchingPairsRepeater">
                                        <HeaderTemplate>
                                            <tr>
                                                <th colspan="3">Matching Pairs</th>
                                            </tr>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="mleft"><%# Shift.Common.Markdown.ToHtml((string)Eval("LeftText")) %></td>
                                                <td class="mright"><%# Shift.Common.Markdown.ToHtml((string)Eval("RightText")) %></td>
                                                <td class="mpoints form-text">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <asp:Repeater runat="server" ID="MatchingDistractorsRepeater">
                                        <HeaderTemplate>
                                            <tr>
                                                <th colspan="3">Matching Distractors</th>
                                            </tr>
                                        </HeaderTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td colspan="3" class="mdistractor"><%# Shift.Common.Markdown.ToHtml((string)Container.DataItem) %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </tbody></table>
                            </asp:View>

                            <asp:View runat="server" ID="LikertItemsView">
                                <table class="table table-likert">
                                    <thead>
                                        <asp:Repeater runat="server" ID="LikertColumnRepeater1">
                                            <HeaderTemplate>
                                                <tr>
                                                    <td rowspan="2"></td>
                                                    <td rowspan="2"></td>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <td><%# Container.ItemIndex + 1 %>.</td>
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                </tr>
                                            </FooterTemplate>
                                        </asp:Repeater>
                
                                        <asp:Repeater runat="server" ID="LikertColumnRepeater2">
                                            <HeaderTemplate>
                                                <tr>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <td class="otext"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
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
                                                            <td class="ovalue"><%# Eval("Points") %></td>
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

                                <asp:Repeater runat="server" ID="HotspotItemRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-option">
                                            <tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="oicon"><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <td class="otext"><%# Shift.Common.Markdown.ToHtml((string)Eval("Text")) %></td>
                                            <td class="opoints form-text">
                                                &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <FooterTemplate></tbody></table></FooterTemplate>
                                </asp:Repeater>
                            </asp:View>

                            <asp:View runat="server" ID="OrderingItemsView">
                                <div runat="server" id="OrderingTopLabel" class="ordering-top-label" visible="false"></div>

                                <asp:Repeater runat="server" ID="OrderingSolutionRepeater">
                                    <HeaderTemplate>
                                        <table class="table table-ordering-solution"><tbody>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <tr>
                                            <td class="onum"><%# Calculator.ToBase26(Container.ItemIndex + 1) %>.</td>
                                            <td>
                                                <asp:Repeater runat="server" ID="OptionRepeater">
                                                    <HeaderTemplate><table class="table-ordering-option"><tbody></HeaderTemplate>
                                                    <FooterTemplate></tbody></table></FooterTemplate>
                                                    <ItemTemplate>
                                                        <tr>
                                                            <td class="onum"><%# Eval("Sequence") %>.</td>
                                                            <td><%# Shift.Common.Markdown.ToHtml((string)Eval("Option.Text")) %></td>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </td>
                                            <td class="opoints form-text">
                                                &bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                    <SeparatorTemplate>
                                        <tr>
                                            <td colspan="3" class="separator"></td>
                                        </tr>
                                    </SeparatorTemplate>
                                    <FooterTemplate>
                                        </tbody></table>
                                    </FooterTemplate>
                                </asp:Repeater>

                                <div runat="server" id="OrderingBottomLabel" class="ordering-bottom-label" visible="false"></div>
                            </asp:View>

                        </asp:MultiView>
                    </td>
                </tr>
                <asp:Repeater runat="server" ID="CommentRepeater" Visible="false">
                    <HeaderTemplate>
                        <tr class="row-comments">
                            <td></td>
                            <td>
                                <h3>Administrator Comments</h3>
                    </HeaderTemplate>
                    <FooterTemplate>
                            </td>
                            <td></td>
                        </tr>
                    </FooterTemplate>
                    <ItemTemplate>
                        <div class="comment" style="page-break-inside: avoid;">
                            <div runat="server" class="comment-flag" visible='<%# (bool)Eval("HasFlag") %>'>
                                <%# Eval("IconHtml") %>
                            </div>
                            <div class="comment-author">
                                <%# Eval("AuthorName") %>
                            </div>
                            <div class="comment-posted-on">
                                <%# FormatDateTime("PostedOn") %>
                            </div>

                            <div class="comment-text"><%# Eval("Text") %></div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
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