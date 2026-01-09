<%@ Page Language="C#" CodeBehind="Reorder.aspx.cs" Inherits="InSite.Admin.Workflow.Forms.Questions.Reorder" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            div.survey-question {
                margin-bottom: 24px;
                padding: 8px;
                border: 1px solid #f1f1f1;
                cursor: grab;
                transition: border-color ease 0.15s;
                border-radius: 4px;
            }

                div.survey-question:hover {
                    border-color: #cccccc;
                }

                div.survey-question img {
                    display: none;
                }

                div.survey-question > span.question-title {
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

    <div runat="server" id="QuestionCard" class="card border-0 shadow-lg h-100 mb-3">
        <div class="card-body">

            <h2 class="h4 mb-3">
                <i class="far fa-question me-1"></i>
                Questions
            </h2>
            <div>
                <div id="survey-questions">
                    <asp:Repeater runat="server" ID="QuestionRepeater">
                        <ItemTemplate>
                            <div class="survey-question">
                                <span class="question-title">
                                    <%# Eval("Code") %>
                                </span>
                                <%# Eval("TitleHtml") %>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </div>

        </div>
    </div>

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

                $('div#survey-questions > div.survey-question').each(function () {
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
                $('div#survey-questions').sortable({
                    items: '> div.survey-question',
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
                $('div#survey-questions').disableSelection().sortable('destroy');
            },

            getData: function () {
                var data = '';

                $('div#survey-questions > div.survey-question').each(function () {
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
