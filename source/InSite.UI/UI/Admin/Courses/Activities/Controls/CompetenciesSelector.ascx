<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesSelector.ascx.cs" Inherits="InSite.Admin.Courses.Activities.Controls.CompetenciesSelector" %>

<%@ Register Src="./CompetenciesNode.ascx" TagName="CompetenciesNode" TagPrefix="uc" %>
<%@ Register TagPrefix="uc" TagName="Repeater" Src="./CompetenciesSelectorRepeater.ascx" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <div class="row">
            <div class="col-lg-12">

        <div runat="server" id="NoCompetenciesAlert" class="alert alert-warning">
            <i class="fas fa-exclamation-triangle"></i> <strong>Warning:</strong>
            This <span runat="server" id="AssetType" /> does not yet contain any competencies.
        </div>

        <asp:Repeater runat="server" ID="NodeRepeater">
            <HeaderTemplate>
                <div style="margin-left:-32px;">
            </HeaderTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
            <ItemTemplate>
                <uc:CompetenciesNode runat="server" ID="CompetenciesNode" />
            </ItemTemplate>
        </asp:Repeater>

        <hr class="my-3" />

        <h3 runat="server" id="FrameworkName">All Competencies</h3>

        <div class="pt-1">
            <insite:Button runat="server" ID="ExpandAllButton" ButtonStyle="Default" Icon="fas fa-chevron-down" Text="Expand All" />
            <insite:ComboBox runat="server" ID="ExpandLevelSelector" ButtonSize="Small" Width="80px" />
            <insite:Button runat="server" ID="CollapseAllButton" ButtonStyle="Default" Icon="fas fa-chevron-up" Text="Collapse All" />
            <span class="competencies-selector-filter">
                <a href="#" class="btn-clear" style="right:0;" title="Clear"><i class="fas fa-times"></i></a>
                <insite:TextBox runat="server" ID="FilterText" Width="400px" EmptyMessage="Search" />
            </span>
        </div>

        <div runat="server" id="Container" class="tree-view-container competencies-selector">
            <uc:Repeater runat="server" ID="Repeater" />
        </div>

        <asp:Button runat="server" ID="RefreshButton" style="display:none;" />

            </div>
        </div>
    </ContentTemplate>
</insite:UpdatePanel>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .competencies-selector {
            padding: 5px 0px;
        }

        .competencies-selector .competency-check > .competency-check {
            padding-left: 20px;
        }

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

<insite:PageFooterContent runat="server" ID="CommonScript">
    <script type="text/javascript">
        $(function () {
            inSite.common.treeView.init($('#<%= Container.ClientID %> > .tree-view'), {
                expand: '#<%= ExpandAllButton.ClientID %>',
                collapse: '#<%= CollapseAllButton.ClientID %>',
                level: '#<%= ExpandLevelSelector.ClientID %>',
                state: 'admin.courses.activities.controls.competenciesselector.<%= ActivityIdentifier %>',
                defaultLevel: 2
            });
        });

        (function () {
            Sys.Application.add_load(onLoad);

            var filterUpdateHandler = null;

            function onLoad() {
                $('#<%= Container.ClientID %>.competencies-selector').each(function () {
                    var $selector = $(this);
                    if ($selector.data('inited') === true)
                        return;

                    $selector.find('input[type="checkbox"]').on('change', onChanged);

                    var $items = $selector.find('.competency-check').each(function () {
                        var $this = $(this);
                        var $label = $this.find('> div > div > div.node-title');

                        var text = $label.text().replace(/\s\s+/g, ' ').trim().toUpperCase();
                        if (text.length > 0)
                            $this.data('text', text);
                    });

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

            function onChanged() {
                var $checkbox = $(this);
                var checked = $checkbox.prop('checked');

                $checkbox.closest('.competency-check')
                    .find(' > .tree-view > .competency-check input[type="checkbox"]').prop('checked', checked).end()
                    .parents('.competency-check').each(function () {
                        var $this = $(this);
                        var $checkbox = $this.find('> div > div > div > input[type="checkbox"]');

                        if ($this.data('competency') == '1') {
                            if (!$checkbox.prop('checked')) {
                                var checked = false;

                                $this.find('.competency-check input[type="checkbox"]').each(function () {
                                    if (checked = $(this).prop('checked'))
                                        return false;
                                });

                                $checkbox.prop('checked', checked);
                            }
                        } else {
                            var checked = false;

                            $this.find('.competency-check input[type="checkbox"]').each(function () {
                                if (!(checked = $(this).prop('checked')))
                                    return false;
                            });

                            $checkbox.prop('checked', checked);
                        }
                    });
            }

            function onFilterTextChange() {
                clearFilterTimeout();
                filterUpdateHandler = setTimeout(onFilterUpdate, 500, this);
            }

            function onFilterUpdate(input) {
                clearFilterTimeout();

                if (!input)
                    return;

                var $input = $(input);
                var $filter = $input.closest('.competencies-selector-filter');
                var prvText = $input.data('prev');
                var curtext = $input.val().toUpperCase();

                if (prvText && prvText == curtext)
                    return;

                var $tree = $('#<%= Container.ClientID %>.competencies-selector > .tree-view');

                if (curtext.length > 0) {
                    $filter.addClass('filtering');

                    inSite.common.treeView.filter($tree, (item) => {
                        const itemText = $(item).data('text');
                        return itemText && itemText.indexOf(curtext) >= 0;
                    });
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