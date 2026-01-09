<%@ Page Language="C#" CodeBehind="Reorder.aspx.cs" Inherits="InSite.Admin.Assessments.Sections.Forms.Reorder" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <insite:PageHeadContent runat="server">
        <style type="text/css">
            div.section-field {
                margin-bottom: 24px;
                padding: 8px;
                border: 1px solid #f1f1f1;
                cursor: grab;
                transition: border-color ease 0.15s;
                border-radius: 4px;
            }

                div.section-field:hover {
                    border-color: #cccccc;
                }

                div.section-field img {
                    display: none;
                }

                div.section-field > span.question-title {
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

    <insite:Nav runat="server" ID="NavPanel" CssClass="mb-3">

        <insite:NavItem runat="server" ID="SectionPanel" Title="Section" Icon="far fa-th-list" IconPosition="BeforeText">
            <section>
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h2 class="h4 mt-4 mb-3">Section</h2>
                        <div class="row">
                            <div class="col-lg-6">
                                <h3>Form</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Form Name</label>
                                    <div>
                                        <asp:Literal runat="server" ID="FormName" />
                                    </div>
                                    <div class="form-text">
                                        The internal name used to uniquely identify the form for filing purposes.
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6">

                                <h3>Section</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Section Number</label>
                                    <div>
                                        <asp:Literal runat="server" ID="SectionNumber" />
                                        <insite:IconLink Name="sort" runat="server" ID="ReorderSectionsLink" Style="padding: 8px" ToolTip="Reorder the sections on this form" />
                                    </div>
                                    <div class="form-text">
                                        This section of the form takes items from the question set identified below.
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6">

                                <h3>Question Set</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Question Set Name</label>
                                    <div>
                                        <asp:Literal runat="server" ID="SetName" />
                                    </div>
                                    <div class="form-text">
                                        The name that uniquely identifies this question set within the bank.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>


        <insite:NavItem runat="server" ID="FieldsPanel" Title="Questions" Icon="far fa-question" IconPosition="BeforeText">
            <section>
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <h2 class="h4 mt-4 mb-3">Questions</h2>

                        <div id="section-fields">
                            <asp:Repeater runat="server" ID="FieldRepeater">
                                <ItemTemplate>
                                    <div class="section-field">
                                        <span class="question-title">
                                            <%# Eval("BankSequence") %>. <%# Eval("Code") %>
                                        </span>
                                        <%# Eval("Title") %>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

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

                $('div#section-fields > div.section-field').each(function () {
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
                $('div#section-fields').sortable({
                    items: '> div.section-field',
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
                $('div#section-fields').disableSelection().sortable('destroy');
            },

            getData: function () {
                var data = '';

                $('div#section-fields > div.section-field').each(function () {
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
