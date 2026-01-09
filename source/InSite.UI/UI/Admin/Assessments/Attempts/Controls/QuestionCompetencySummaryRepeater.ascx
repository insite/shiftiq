<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuestionCompetencySummaryRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Attempts.Controls.QuestionCompetencySummaryRepeater" %>

<div class="position-absolute pe-3 end-0">
    <insite:DownloadButton runat="server" ID="DownloadButton" />
</div>

<insite:Nav runat="server">
    <insite:NavItem runat="server" ID="CompetenciesTab" Title="Competencies">

        <insite:Alert runat="server" ID="CompetenciesStatus" />

        <asp:Repeater runat="server" ID="OccupationRepeater">
            <HeaderTemplate><table class="table table-competency-summary"></HeaderTemplate>
            <FooterTemplate></table></FooterTemplate>
            <ItemTemplate>
                <tr class='<%# "row-occupation" + ((Guid)Eval("ID") == Guid.Empty && Occupations.Length == 1 ? " d-none" : string.Empty) %>'>
                    <td class="cell-title"><%# Eval("Name") %></td>
                    <td></td>
                    <td></td>
                    <td class="cell-score"><%# GetScoreText((decimal)Eval("Score")) %></td>
                </tr>
                <asp:Repeater runat="server" ID="FrameworkRepeater">
                    <ItemTemplate>
                        <tr class="row-framework">
                            <td class="cell-title"><%# Eval("Name") %></td>
                            <td><%# Eval("Count") %></td>
                            <td></td>
                            <td class="cell-score"><%# GetScoreText((decimal)Eval("Score")) %></td>
                        </tr>
                        <asp:Repeater runat="server" ID="GacRepeater">
                            <ItemTemplate>
                                <tr class="row-gac">
                                    <td class="cell-title"><%# Eval("Name") %></td>
                                    <td><%# Eval("Count") %></td>
                                    <td></td>
                                    <td class="cell-score"><%# GetScoreText((decimal)Eval("Score")) %></td>
                                </tr>
                                <asp:Repeater runat="server" ID="CompetencyRepeater">
                                    <ItemTemplate>
                                        <tr class="row-competency">
                                            <td class="cell-title"><%# Eval("Name") %></td>
                                            <td><%# Eval("Count") %> </td>
                                            <td class="cell-points"><%# GetPoints((decimal)Eval("AnswerPoints"), (decimal)Eval("QuestionPoints")) %></td>
                                            <td class="cell-score"><%# GetScoreLabel((decimal)Eval("Score")) %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>
                    </ItemTemplate>
                </asp:Repeater>
            </ItemTemplate>
        </asp:Repeater>

    </insite:NavItem>

    <insite:NavItem runat="server" ID="LearnersTab" Title="Learners">

        <insite:Alert runat="server" ID="LearnersStatus" />

        <div class="mb-3">
            <insite:TextBox runat="server" ID="LearnerFilterTextBox" Width="300" EmptyMessage="Filter" CssClass="d-inline-block" />
            <insite:IconButton runat="server" ID="LearnerFilterButton" Name="filter" ToolTip="Filter" CssClass="p-2" />
            <insite:PageFooterContent runat="server">
                <script type="text/javascript"> 
                    (function () {
                        Sys.Application.add_load(function () {
                            $('#<%= LearnerFilterTextBox.ClientID %>')
                                .off('keydown', onKeyDown)
                                .on('keydown', onKeyDown);
                        });

                        function onKeyDown(e) {
                            if (e.which === 13) {
                                e.preventDefault();
                                $('#<%= LearnerFilterButton.ClientID %>')[0].click();
                            }
                        }
                    })();
                </script>
            </insite:PageFooterContent>
        </div>

        <insite:Grid runat="server" ID="Grid" ShowFooter="true" />

    </insite:NavItem>
</insite:Nav>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        table th.col-right { text-align:right; }

        table.table-competency-summary {
        }

            table.table-competency-summary > tbody > tr > td.cell-title > span.toggle-button {
                position: absolute;
                cursor: pointer;
                font-size: 16px;
                color: #545454;
            }

                table.table-competency-summary > tbody > tr > td.cell-title > span.toggle-button.closed > .fa-minus,
                table.table-competency-summary > tbody > tr > td.cell-title > span.toggle-button.opened > .fa-plus {
                    display: none;
                }

                table.table-competency-summary > tbody > tr > td.cell-title > span.toggle-button.opened > .fa-minus,
                table.table-competency-summary > tbody > tr > td.cell-title > span.toggle-button.closed > .fa-plus {
                    display: inline-block;
                }


            table.table-competency-summary > tbody > tr.row-occupation {
            }

                table.table-competency-summary > tbody > tr.row-occupation:first-child > td {
                    padding-top: 16px;
                    border-top: none;
                }

                table.table-competency-summary > tbody > tr.row-occupation > td {
                    font-size: 1.3em;
                    padding-top: 40px;
                    padding-bottom: 16px;
                }

                    table.table-competency-summary > tbody > tr.row-occupation > td.cell-title {
                    }

                        table.table-competency-summary > tbody > tr.row-occupation > td.cell-title > span.toggle-button {
                            line-height: 30px;
                            margin-left: -34px;
                        }

                    table.table-competency-summary > tbody > tr.row-occupation > td.cell-score {
                        text-align: right;
                    }

            table.table-competency-summary > tbody > tr.row-framework {

            }

                table.table-competency-summary > tbody > tr.row-framework > td {
                    font-size: 1.2em;
                }

                    table.table-competency-summary > tbody > tr.row-framework > td.cell-title {
                        color: #337ab7;
                    }

                        table.table-competency-summary > tbody > tr.row-framework > td.cell-title > span.toggle-button {
                            line-height: 28px;
                            margin-left: -34px;
                        }

                    table.table-competency-summary > tbody > tr.row-framework > td.cell-score {
                        text-align: right;
                    }

            table.table-competency-summary > tbody > tr.row-gac {
            }

                table.table-competency-summary > tbody > tr.row-gac > td {
                    font-weight: bold;
                }

                    table.table-competency-summary > tbody > tr.row-gac > td.cell-title {
                    }

                        table.table-competency-summary > tbody > tr.row-gac > td.cell-title > span.toggle-button {
                            margin-left: -34px;
                        }

                    table.table-competency-summary > tbody > tr.row-gac > td.cell-score {
                        text-align: right;
                    }

            table.table-competency-summary > tbody > tr.row-competency {
                position: relative;
            }

                table.table-competency-summary > tbody > tr.row-competency > td.cell-title {
                }

                table.table-competency-summary > tbody > tr.row-competency > td.cell-points {
                    font-weight: bold;
                    text-align: right;
                    width: 140px;
                    white-space: nowrap;
                }

                table.table-competency-summary > tbody > tr.row-competency > td.cell-score {
                    text-align: right;
                    width: 75px;
                }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">

    <script type="text/javascript">

        (function () {
            var root = null;

            initTree();

            function initTree() {
                const current = {
                    item: root = getItem(),
                    level: 0
                };
                const isFirstLevelHidden = $('table.table-competency-summary > tbody > tr.row-occupation').hasClass('d-none');

                $('table.table-competency-summary > tbody > tr').each(function () {
                    const $row = $(this);

                    let level = -1;
                    if ($row.hasClass('row-competency'))
                        level = 4;
                    else if ($row.hasClass('row-gac'))
                        level = 3;
                    else if ($row.hasClass('row-framework'))
                        level = 2;
                    else if ($row.hasClass('row-occupation'))
                        level = 1;

                    if (level == -1)
                        return;

                    const paddingLeft = 40 + (isFirstLevelHidden ? level - 2 : level - 1) * 25;
                    $row.find('> td.cell-title').css('padding-left', String(paddingLeft) + 'px');

                    if (level > current.level) {
                        if (level > (current.level + 1))
                            throw 'Unexpected tree level: ' + String(level) + ' while expected ' + String(current.level + 1);

                        current.item = getItem($row, current.item, level);
                        current.level = level;
                    } else if (level == current.level) {
                        current.item = getItem($row, current.item.parent, level);
                    } else {
                        for (; current.level >= level; current.level--)
                            current.item = current.item.parent;

                        current.item = getItem($row, current.item, level);
                        current.level = level;
                    }
                });

                toggleTreeItem(root.children, true);

                function getItem($row, parent, level) {
                    var item = {
                        $row: null,
                        parent: null,
                        opened: true,
                        children: []
                    };

                    if ($row) {
                        item.$row = $row;
                        item.$row.data('tree-item', item);
                    }

                    if (parent) {
                        item.parent = parent;
                        item.parent.children.push(item);

                        if (item.parent.children.length === 1 && item.parent.$row) {
                            var $button = $('<span class="toggle-button"><i class="fa fa-plus"></i><i class="fa fa-minus"></i></span>').on('click', onRowToggle);

                            item.parent.$row.find('> td.cell-title').prepend($button);

                            if (item.parent.opened)
                                $button.addClass('opened');
                            else
                                $button.addClass('closed');
                        }
                    }

                    return item;
                }
            }

            function onRowToggle() {
                var $btn = $(this);
                var $row = $btn.closest('tr');
                var item = $row.data('tree-item');
                item.opened = !item.opened;

                toggleTreeItem(item.children, item.opened);

                if (item.opened)
                    $btn.removeClass('closed').addClass('opened');
                else
                    $btn.removeClass('opened').addClass('closed');
            }

            function toggleTreeItem(items, isParentOpened) {
                for (var i = 0; i < items.length; i++) {
                    var item = items[i];

                    if (isParentOpened)
                        item.$row.show();
                    else
                        item.$row.hide();

                    toggleTreeItem(item.children, isParentOpened && item.opened);
                }
            }
        })();

    </script>

</insite:PageFooterContent>