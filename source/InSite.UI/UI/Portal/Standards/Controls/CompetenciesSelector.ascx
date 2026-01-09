<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CompetenciesSelector.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.CompetenciesSelector" %>

<%@ Register TagPrefix="uc" TagName="Repeater" Src="CompetenciesSelectorRepeater.ascx" %>

<insite:UpdateProgress runat="server" AssociatedUpdatePanelID="UpdatePanel" />

<insite:UpdatePanel runat="server" ID="UpdatePanel">
    <ContentTemplate>
        <insite:FindStandard runat="server" ID="FrameworkSelector" Width="400" EnableTranslation="true" EmptyMessage="Framework" /> 

        <div class="competencies-selector-filter">
            <a href="#" class="btn-clear" style="right:0;" title="Clear"><i class="fas fa-times"></i></a>
            <insite:TextBox runat="server" ID="FilterText" Width="400px" EmptyMessage="Keyword" />
        </div>

        <div style="padding-top:5px;">
            <insite:Button runat="server" ID="ExpandAllButton" ButtonStyle="OutlineSecondary" CssClass="btn-bg-white" Icon="fas fa-chevron-down" Text="Expand All" />
            <insite:Button runat="server" ID="CollapseAllButton" ButtonStyle="OutlineSecondary" CssClass="btn-bg-white" Icon="fas fa-chevron-up" Text="Collapse All" />
            <insite:ComboBox runat="server" ID="ExpandLevelSelector" Width="100px" style="display:inline;" ButtonSize="Small" />
        </div>

        <div runat="server" id="Container" class="tree-view-container competencies-selector">
            <uc:Repeater runat="server" ID="Repeater" />
        </div>

        <asp:Button runat="server" ID="RefreshButton" style="display:none;" />
    </ContentTemplate>
</insite:UpdatePanel>

<div runat="server" id="ConnectionInfoModal" class="modal" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><insite:Literal runat="server" Text="Content" /></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">
                    <i class='fas fa-ban'></i> <insite:Literal runat="server" Text="Close" />
                </button>
            </div>
        </div>
    </div>
</div>

<div runat="server" id="TermWindow" class="modal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">

			<div class="modal-header">
				<h5 class="modal-title"></h5>
			</div>

            <div class="modal-body" style="min-height:50px;">

            </div>

			<div class="modal-footer" style="display:block;">
                <button type="button" class="btn btn-default" data-action="close"><i class='fas fa-ban me-2'></i> <insite:Literal runat="server" Text="Close" /></button>
			</div>
        </div>
    </div>
</div>

<insite:PageHeadContent runat="server" ID="CommonStyle">
    <style type="text/css">
        .competencies-selector {
            padding: 5px 0px;
        }

        .competencies-selector .competency-check > .competency-check {
            padding-left: 20px;
        }

        .competencies-selector.filtering .competency-check {
            display: none;
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
        (function () {
            Sys.Application.add_load(onLoad);

            var filterUpdateHandler = null;

            function onLoad() {
                inSite.common.treeView.init($('#<%= Container.ClientID %> > .tree-view'), {
                    expand: '#<%= ExpandAllButton.ClientID %>',
                    collapse: '#<%= CollapseAllButton.ClientID %>',
                    level: '#<%= ExpandLevelSelector.ClientID %>',
                    state: 'portal.standards.documents.treeView.state.all',
                    defaultLevel: 2
                });

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

        (function () {
            var $modal = $('#<%= ConnectionInfoModal.ClientID %>');
            var bModal = new bootstrap.Modal($modal[0]);
            var isLocked = false;

            $('.tree-view-container .outline-item [data-action="connection-info"]').on('click', function (e) {
                e.preventDefault();

                var standard = $(this).data('id');
                if (!standard || isLocked)
                    return;

                isLocked = true;

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/contents/html',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: {
                        container: standard,
                        type: 'standard',
                        lang: '<%= Identity.Language %>'
                    },
                    success: function (result) {
                        $container = $('<div>');

                        if (result instanceof Array && result.length > 0) {
                            for (var i = 0; i < result.length; i++) {
                                var item = result[i];
                                if (item.title === 'Title')
                                    $container.append($('<h5>').html(item.value));
                                else
                                    $container.append($('<div>').html(item.value));
                            }
                        } else {
                            $container.html('<p><strong><%= Translate("No Data") %></strong></p>');
                        }

                        $modal.find('> .modal-dialog > .modal-content > .modal-body').empty().append($container);
                        bModal.show();
                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    },
                    complete: function () {
                        isLocked = false;
                    },
                });
            });
        })();
    </script>
</insite:PageFooterContent>