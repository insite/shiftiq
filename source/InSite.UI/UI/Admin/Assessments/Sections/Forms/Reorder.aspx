<%@ Page Language="C#" CodeBehind="Reorder.aspx.cs" Inherits="InSite.Admin.Assessments.Forms.Forms.Reorder" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            div.form-section {
                margin-bottom: 24px;
                padding: 8px;
                border: 1px solid #f1f1f1;
                cursor: grab;
                transition: border-color ease 0.15s;
                border-radius: 4px;
            }

                div.form-section:hover {
                    border-color: #cccccc;
                }

                div.form-section img {
                    display: none;
                }

                div.form-section > span.question-title {
                    font-weight: bold;
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

    <section runat="server" id="FormPanel" class="mb-3">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">
                <h2 class="h4 mb-3">
                    <i class="far fa-window me-1"></i>
                    Form
                </h2>
                <div class="row">
                    <div id="form-sections">
                        <asp:Repeater runat="server" ID="SectionRepeater">
                            <ItemTemplate>
                                <div class="form-section">
                                    <%# Eval("SetName") %>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>

            </div>
        </div>
    </section>

    <div>
        <insite:SaveButton runat="server" ID="SaveButton" OnClientClick="reorder.save(); return false;" />
        <insite:CancelButton runat="server" ID="CancelButton" />
    </div>

    <script type="text/javascript">
        var reorder = {
            _callbackControlId: '<%= SaveButton.UniqueID %>',

            init: function () {
                reorder.initSortable();

                var sequence = 1;

                $('div#form-sections > div.form-section').each(function () {
                    $(this).attr('itemid', String(sequence));
                    sequence++;
                });
            },
            save: function () {
                reorder.destroySortable();

                __doPostBack(reorder._callbackControlId, 'save&' + reorder.getData());
            },
            cancel: function () {
                reorder.destroySortable();

                __doPostBack(reorder._callbackControlId, 'cancel');
            },

            // methods

            initSortable: function () {
                $('div#form-sections').sortable({
                    items: '> div.form-section',
                    containment: 'document',
                    cursor: 'grabbing',
                    forceHelperSize: true,
                    axis: 'y',
                    opacity: 0.65,
                    tolerance: 'pointer',
                    start: inSite.common.gridReorderHelper.onSortStart,
                }).disableSelection();
            },
            destroySortable: function () {
                $('div#form-sections').disableSelection().sortable('destroy');
            },

            getData: function () {
                var data = '';

                $('div#form-sections > div.form-section').each(function () {
                    var $this = $(this);
                    var itemid = $this.attr('itemid');

                    data += String(itemid) + ';';
                });

                return data;
            },

            // event handlers

            onSortStart: function (s, e) {
                e.placeholder.height(e.item.height());
            },
        };

        $(document).ready(reorder.init);
    </script>
</asp:Content>
