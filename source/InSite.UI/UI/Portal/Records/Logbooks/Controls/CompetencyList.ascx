<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetencyList.ascx.cs" Inherits="InSite.UI.Portal.Records.Logbooks.Controls.CompetencyList" %>


<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>

        <div class="competencies-selector-filter">
            <a href="#" class="btn-clear" style="right:0;" title="Clear"><i class="fas fa-times"></i></a>
            <insite:TextBox runat="server" ID="FilterText" Width="400px" EmptyMessage="Keyword" />
        </div>

        <div class="pt-3">
            <a class="btn btn-sm btn-default filter-option-expand">Expand All</a>
            <a class="btn btn-sm btn-default filter-option-hide">Collapse All</a>
        </div>

        <div runat="server" id="Container" class="tree-view-container">
            <asp:Repeater runat="server" ID="Repeater">
                <HeaderTemplate>
                    <div class="row pt-3">
                        <div class='col-md-7 px-5 h6'>Competency</div>
                        <div class='col-md-3 h6 text-center'>Number of Logged Entries</div>
                        <div class='col-md-2 pe-5 h6 text-end'><insite:ContentTextLiteral runat="server" ContentLabel="Number of Hours"/></div>
                    </div>
                    <ul class="tree-view" data-init="code">
                </HeaderTemplate>
                <FooterTemplate>
                    </ul>
                </FooterTemplate>
                <ItemTemplate>
                    <li class="outline-item competency-check" data-key="<%# Eval("Name") %>" data-competency="0" />
                        <div>
                            <div>
                                <div class="node-title fw-bold">
                                    <%# Eval("Name") %>
                                </div>
                            </div>
                        </div>

                        <asp:Repeater runat="server" ID="CompetencyRepeater">
                                <HeaderTemplate>
                                    <ul class="tree-view">
                                </HeaderTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            <ItemTemplate>
                                <li class="outline-item competency-check" data-key="<%# EscapeText(Eval("Name")) %>" data-competency="0" />
                                    <div>
                                        <div>
                                            <div class="node-title">
                                                <div class="form-group row competency competency-row">
                                                    <div class="col-md-7 d-flex">
                                                        <insite:CheckBox runat="server" ID="Selected" Text='<%# Eval("Name") %>' Checked='<%# Eval("Selected") %>' />
                                                        <asp:Literal runat="server" ID="Identifier" Visible="false" Text='<%# Eval("Identifier") %>' />
                                                    </div>
                                                    <div class="col-md-3 text-center">
                                                        <asp:Literal runat="server" ID="JournalItems" Text='<%# Eval("JournalItems") %>' />
                                                    </div>
                                                    <div class="col-md-2 text-end">
                                                        <insite:CustomValidator runat="server" ID="HoursValidator" ControlToValidate="Hours" Display="None" ValidationGroup="Journal" />
                                                        <insite:NumericBox runat="server" ID="Hours" Value='<%# Eval("Hours") %>' CssClass="form-control-sm" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>

                    </li>
                </ItemTemplate>
            </asp:Repeater>
            
        </div>

        <asp:Button runat="server" ID="RefreshButton" style="display:none;" />

    </ContentTemplate>
</insite:UpdatePanel>


<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">

        .competencies-selector-filter {
            margin-top: 5px;
            width: 400px;
            display: none;
            position: relative;
        }

        .competencies-selector-filter a.btn-clear {
            display: none;
            position: absolute;
            line-height: 44px;
            padding: 0 16px;
            opacity: 0.8;
        }

        .competencies-selector-filter.filtering a.btn-clear {
            display: block;
        }
        
    </style>

</insite:PageHeadContent>

<insite:PageFooterContent runat="server">

<script type="text/javascript">
    (function () {
        $(".competency-row input[type=checkbox]").on("click", function () {
            var $this = $(this);
            var $input = $this.closest(".competency-row").find("input[type=text]")

            if (this.checked)
                $input.removeAttr("disabled");
            else
                $input.attr("disabled", "disabled");
        });
    })();
</script>

<script type="text/javascript">
    (function () {
        inSite.common.treeView.init($('.tree-view-container > .tree-view'), {
            expand: $('.filter-option-expand'),
            collapse: $('.filter-option-hide'),
            defaultLevel: 2
        });
    })();

    (function () {
        Sys.Application.add_load(onLoad); 

        var filterUpdateHandler = null;

        function onLoad() {
            $('#<%= Container.ClientID %>').each(function () {
                var $selector = $(this);
                if ($selector.data('inited') === true)
                    return;

                var $items = $selector.find('.competency-check');

                if ($items.length > 0) {
                    var $filterInput = $('#<%= FilterText.ClientID %>')
                        .on('keydown change', onFilterTextChange);

                    $filterInput
                        .closest('.competencies-selector-filter').show()
                        .find('.btn-clear').data('input', $filterInput).on('click', onFilterClear);

                    onFilterUpdate($filterInput[0]);
                }

                $selector.data('inited', true);
            });
        }

        function onFilterTextChange() {
            clearFilterTimeout();
            filterUpdateHandler = setTimeout(onFilterUpdate, 500, this);
        }

        function onFilterUpdate(input) {
            clearFilterTimeout();

            if (!input) {
                return;
            }

            const $input = $(input);
            const $filter = $input.closest('.competencies-selector-filter');
            const prvtext = $input.data('prev');
            const curtext = $input.val().toUpperCase();

            if (prvtext && prvtext === curtext) {
                return;
            }

            const $tree = $('#<%= Container.ClientID %> > .tree-view');

            if (curtext.length > 0) {
                inSite.common.treeView.filter($tree, (item) => {
                    const itemText = $(item).data('key').toUpperCase();
                    return itemText?.includes(curtext);
                });

                $filter.addClass('filtering');
            } else {
                $filter.removeClass('filtering');
                inSite.common.treeView.filter($tree);
            }

            $input.data('prev', curtext)
        }

        function onFilterClear(e) {
            e.preventDefault();

            onFilterUpdate($(this).data('input').val(''));
        }

        function clearFilterTimeout() {
            if (filterUpdateHandler === null)
                return;

            clearTimeout(filterUpdateHandler);
            filterUpdateHandler = null;
        }
    })();

</script>

</insite:PageFooterContent>