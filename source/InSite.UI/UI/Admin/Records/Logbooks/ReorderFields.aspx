<%@ Page Language="C#" CodeBehind="ReorderFields.aspx.cs" Inherits="InSite.UI.Admin.Records.Logbooks.ReorderFields" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            table#logbook-fields {
                cursor: grab;
            }

            .ui-sortable {
            }

            .ui-sortable > .ui-sortable-placeholder {
                visibility: visible !important;
                outline: 1px dashed #b5b5b5 !important;
            }

            .ui-sortable > .ui-sortable-placeholder {
                background-image: none !important;
            }
        </style>
    </insite:PageHeadContent>

    <div class="card border-0 shadow-lg mb-3">
        <div class="card-body">

            <h2 class="h4 mb-3">
                <i class="far fa-list me-1"></i>
                Fields
            </h2>
            <asp:Repeater runat="server" ID="FieldRepeater">
                <HeaderTemplate>
                    <table id="logbook-fields" class="table table-hover">
                        <thead>
                            <tr>
                                <th>Field Name</th>
                                <th style="text-align:center;">Required Field</th>
                                <th>Tag Text</th>
                                <th>Help Text</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                        </tbody>
                    </table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr class="logbook-field">
                        <td>
                            <%# Eval("FieldType") %>
                        </td>
                        <td style="text-align:center;">
                            <%# Eval("IsRequired") %>
                        </td>
                        <td>
                            <%# Eval("LabelText") %>
                        </td>
                        <td>
                            <%# Eval("HelpText") %>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" OnClientClick="reorder.save(); return false;" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <script type="text/javascript">
        (function () {
            const instance = window.reorder || (window.reorder = {});

            instance.save = function () {
                __doPostBack('<%= SaveButton.UniqueID %>', 'save&' + getData());
            };

            $(document).ready(init);

            function init() {
                initSortable();

                let sequence = 1;

                $('table#logbook-fields tr.logbook-field').each(function () {
                    $(this).attr('itemid', String(sequence));
                    sequence++;
                });
            }

            function initSortable() {
                $('table#logbook-fields > tbody').sortable({
                    items: '> tr.logbook-field',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    axis: 'y',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    start: inSite.common.gridReorderHelper.onSortStart,
                    helper: function (e, $el) {
                        var source = $el[0];
                        var $helper = $el.clone();
                        var helper = $helper[0]

                        for (var i = 0; i < source.cells.length; i++) {
                            $(helper.cells[i]).width($(source.cells[i]).outerWidth());
                        }

                        return $helper;
                    },
                }).disableSelection();
            }

            function getData() {
                let data = '';

                $('table#logbook-fields tr.logbook-field').each(function () {
                    const $this = $(this);
                    const itemid = $this.attr('itemid');

                    data += String(itemid) + ';';
                });

                return data;
            }
        })();
    </script>

</asp:Content>
