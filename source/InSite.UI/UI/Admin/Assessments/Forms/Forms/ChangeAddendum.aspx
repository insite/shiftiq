<%@ Page Language="C#" CodeBehind="ChangeAddendum.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.ChangeAddendum" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:PageHeadContent runat="server">
        <style type="text/css">
            table.table-addendum > tbody .start-move {
                cursor: grab;
                display: inline-block;
                text-align: center;
            }

            table.table-addendum > tbody td.cmd > span {
                cursor: pointer;
                display: inline-block;
                text-align: center;
                color: #337ab7;
            }

            table.table-addendum > tbody.ui-sortable > tr.ui-sortable-placeholder {
                visibility: visible !important;
                outline: 1px dashed #666666 !important;
            }

            table.table-addendum > tbody > tr.ui-sortable-helper > td {
                border-bottom: 1px solid #ddd;
            }

            table.table-addendum > tbody > tr.spacer > td {
            }
        </style>
    </insite:PageHeadContent>

    <insite:Alert runat="server" ID="EditorStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="Assessment" />

    <insite:Container runat="server" ID="FormContainer">
        <section class="mb-3">
            <h2 class="h4 mb-3">
                <i class="far fa-window"></i>
                Change Form Addendum
            </h2>

            <div class="row">

                <div class="col-lg-12">
                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="row">
                                <div class="col-md-6 col-addendum">
                                    <h3>Addendum</h3>
                                    <asp:HiddenField runat="server" ID="AcronymsValues" />
                                    <asp:HiddenField runat="server" ID="FormulasValues" />
                                    <asp:HiddenField runat="server" ID="FiguresValues" />
                                </div>

                                <div class="col-md-6 col-attachments">
                                    <h3>Attachments</h3>
                                    <asp:Repeater runat="server" ID="AttachmentRepeater">
                                        <HeaderTemplate>
                                            <table class="table table-addendum">
                                                <thead>
                                                    <tr>
                                                        <th>Type</th>
                                                        <th>Asset</th>
                                                        <th>Title</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate>
                                                </tbody>
                                            </table>
                                        </FooterTemplate>
                                        <ItemTemplate>
                                            <tr data-id="<%# $"{Eval("AssetNumber")}.{Eval("AssetVersion")}" %>">
                                                <td><%# Eval("TypeName") %></td>
                                                <td><%# $"{Eval("AssetNumber")}.{Eval("AssetVersion")}" %></td>
                                                <td><a target="_blank" href="<%# Eval("Url") %>?download=1"><%# Eval("Title") %></a></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </section>

        <div class="row">
            <div class="col-lg-12">
                <insite:SaveButton runat="server" ID="SaveButton" OnClientClick="addendumEditor.save();" ValidationGroup="Assessment" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>
    </insite:Container>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var instance = window.addendumEditor = window.addendumEditor || {};

                var $acronymsInput, $formulasInput, $figuresInput, $attachmentsTable, $addendumTable;

                initTables();
                initSortable();

                instance.save = function () {
                    const acronyms = getTableData('acronyms');
                    const formulas = getTableData('formulas');
                    const figures = getTableData('figures');

                    $acronymsInput.val(acronyms);
                    $formulasInput.val(formulas);
                    $figuresInput.val(figures);

                    function getTableData(type) {
                        const result = [];
                        $addendumTable.find('> tbody[data-type="' + type + '"] > tr').each(function () {
                            result.push($(this).data('id'));
                        });
                        return result.join(',');
                    }
                };

                // methods

                function initTables() {
                    $acronymsInput = $('#<%= AcronymsValues.ClientID %>');
                    $formulasInput = $('#<%= FormulasValues.ClientID %>');
                    $figuresInput = $('#<%= FiguresValues.ClientID %>');

                    $attachmentsTable = $('.col-attachments > table.table-addendum');
                    $attachmentsTable.find('> thead > tr')
                        .prepend('<th style="width:36px;"></th>')
                        .append('<th style="width:36px;"></th>');

                    const $attachmentsRows = $attachmentsTable.find('> tbody > tr')
                        .prepend('<td><span class="start-move"><i class="fas fa-arrows-alt"></i></span></td>')
                        .append('<td class="cmd"></td>');

                    $addendumTable = $($attachmentsTable[0].cloneNode(false));
                    $addendumTable.append($attachmentsTable.find('> thead').clone());

                    const rowsMap = new Map();
                    $attachmentsRows.each(function () {
                        rowsMap.set(this.dataset.id, this);
                    });

                    addAddendumBody('acronyms', 'Acronyms', $acronymsInput.val().split(','));
                    addAddendumBody('formulas', 'Formulas', $formulasInput.val().split(','));
                    addAddendumBody('figures', 'Figures', $figuresInput.val().split(','));

                    rowsMap.forEach(row => onRowReceived(row));

                    $('.col-addendum').append($addendumTable);

                    $attachmentsTable.on('click', onAttachmentsClick);
                    $addendumTable.on('click', onAddendumClick);

                    function addAddendumBody(type, title, items) {
                        const $tBody = $('<tbody>').attr('data-type', type).append(
                            $('<tr data-exclude>').append(
                                $('<th class="bg-secondary" colspan="5">').text(title)
                            )
                        );

                        for (const item of items) {
                            if (rowsMap.has(item))
                                $tBody.append($(rowsMap.get(item)).detach());
                        }

                        $addendumTable.append($tBody);

                        return $tBody;
                    }
                }

                function initSortable() {
                    $('table.table-addendum > tbody').sortable({
                        items: '> tr:not([data-exclude])',
                        connectWith: 'table.table-addendum > tbody',
                        containment: 'document',
                        cursor: 'grabbing',
                        forceHelperSize: true,
                        handle: 'span.start-move',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        dropOnEmpty: true,
                        create: function () {
                            ensureTableSpacer(this);
                        },
                        change: onSortableChange,
                        helper: function (e, el) {
                            var $cells = el.children();
                            return el.clone().children().each(function (index) {
                                $(this).width($cells.eq(index).outerWidth());
                            }).end();
                        },
                        receive: function (e, ui) {
                            onRowReceived(ui.item);
                        }
                    }).disableSelection();
                }

                function ensureTableSpacer(container) {
                    var $container = $(container);
                    var $rows = $container.find('tr:visible:not(.ui-sortable-helper)');

                    if ($rows.length > 0) {
                        if ($rows.length > 1 && $container.data('isempty')) {
                            $rows.filter('.spacer').remove();
                        }
                    } else {
                        $container.data('isempty', true);

                        var cols = $container.closest('table').find('> thead > tr:first > th').length;
                        $container.append('<tr class="spacer"><td colspan="' + String(cols) + '">&nbsp;</td></tr>');
                    }
                }

                function destroySortable() {
                    $('table.table-addendum > tbody').disableSelection().sortable('destroy');
                }

                // event handlers

                function onSortableChange() {
                    var data = $(this).data('uiSortable');
                    for (var i = 0; i < data.containers.length; i++)
                        ensureTableSpacer(data.containers[i].element);
                }

                function onAddendumClick(e) {
                    var $target = $(e.target);
                    var action = $target.data('action');

                    if (action === 'remove-addendum') {
                        var $row = $target.closest('tr').detach();
                        var $tBody = $attachmentsTable.find('> tbody').append($row);

                        onRowReceived($row);

                        onSortableChange.apply($tBody);
                    }
                }

                function onAttachmentsClick(e) {
                    var $target = $(e.target);
                    var action = $target.data('action');

                    if (action === 'add-addendum') {
                        var $row = $target.closest('tr').detach();
                        var $tBody = $addendumTable.find('> tbody').append($row);

                        onRowReceived($row);

                        onSortableChange.apply($tBody);
                    }
                }

                function onRowReceived(row) {
                    var $row = $(row);

                    if ($attachmentsTable.has($row).length) {
                        $row.find('> td.cmd').empty();
                    } else {
                        $row.find('> td.cmd').empty().append('<span title="Remove from Addendum"><i data-action="remove-addendum" class="fas fa-trash-alt"></i></span>');
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
