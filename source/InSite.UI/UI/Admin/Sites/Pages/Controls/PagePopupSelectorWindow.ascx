<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PagePopupSelectorWindow.ascx.cs" Inherits="InSite.Admin.Sites.Pages.Controls.PagePopupSelectorWindow" %>

<insite:PageHeadContent runat="server" ID="StyleLiteral">
    <style type="text/css">
        .asset-popup-selector-window {
        }

            .asset-popup-selector-window .row-filter {
                position: absolute;
                width: 100%;
                left: 0;
                top: 0;
                padding: calc(var(--ar-modal-padding) * 0.5) var(--ar-modal-padding);
                background-color: #fff;
                z-index: 2;
            }

                .asset-popup-selector-window .row-filter a.btn-clear {
                    display: block;
                    position: absolute;
                    line-height: 44px;
                    padding: 0 16px;
                }

                .asset-popup-selector-window .row-filter > div {
                    position: relative;
                }

                    .asset-popup-selector-window .row-filter > div + div {
                        margin-top: 10px;
                    }

            .asset-popup-selector-window .modal-body > table {
                margin-bottom: 20px;
            }

                .asset-popup-selector-window .modal-body > table > tbody > tr[data-id] {
                    cursor: pointer;
                }

                    .asset-popup-selector-window .modal-body > table > tbody > tr[data-id]:hover,
                    .asset-popup-selector-window .modal-body > table > tbody > tr[data-id].selected {
                        background-color: #eee;
                    }

                        .asset-popup-selector-window .modal-body > table > tbody > tr[data-id].selected > td:first-child::before {
                            font-family: "Font Awesome 7 Pro";
                            content: "\f0da";
                            position: absolute;
                            font-weight: 900;
                            margin-left: -22px;
                        }
    </style>
</insite:PageHeadContent>

<insite:Modal runat="server" ID="Window" Title="Find and Select" CssClass="asset-popup-selector-window" Scrollable="true">
    <ContentTemplate>
        <div class="row-filter">
            <div>
                <insite:WebPageTypeComboBox runat="server" ID="PageType" EmptyMessage="Page Type" Width="100%" ZIndex="8501" />
            </div>
            <div>
                <a href="#" data-action="clear-keyword" class="btn-clear" style="right: 0; display: none;"><i class="fas fa-times"></i></a>
                <input type="text" name="keyword" class="insite-text form-control" placeholder="Filter">
            </div>
        </div>

        <div style="margin-top:calc(var(--ar-modal-padding) * -1);"></div>

        <table class="table">
            <tbody>
            </tbody>
        </table>

        <insite:LoadingPanel runat="server" />
    </ContentTemplate>
</insite:Modal>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var root = window.pagePopupSelectorWindow = window.pagePopupSelectorWindow || {};
            var instance = root.<%= ClientID %> = root.<%= ClientID %> || {};

            var $window = $(window);
            var $modal = $('#<%= Window.ClientID %>')
                .on('show.bs.modal', onModalShow)
                .on('hide.bs.modal', onModalHide);
            var $modalBody = $modal.find('> .modal-dialog > .modal-content > .modal-body');
            var $loadingPanel = $modalBody.find('.loading-panel');
            var $filterContainer = $modalBody.find('.row-filter');
            var $filterKeywordInput = $modalBody.find('.row-filter input[name="keyword"]')
                .on('change paste keyup', onFilterChange)
                .on('keydown', onFilterKeyDown);
            var $filterKeywordClear = $modalBody.find('.row-filter a[data-action="clear-keyword"]').on('click', onFilterClear);
            var $tableBody = $modalBody.find('> table.table > tbody');

            var $filterType = null;
            var currentRowNumber = null;
            var previousKeyword = null;
            var itemsPerRequest = 50;
            var openerData = null;

            // public methods

            instance.open = function (data) {
                previousKeyword = null;

                openerData = data;

                if ($filterType == null) {
                    $filterType = $modalBody.find('.row-filter #<%= PageType.ClientID %>').on('change', onTypeChanged);
                }

                $filterType.selectpicker('val', '');

                var type;
                var isFilterTypeEnabled;

                if (data.fixedType != null && data.fixedType.length > 0) {
                    type = data.fixedType;
                    isFilterTypeEnabled = false;
                } else {
                    type = data.type;
                    isFilterTypeEnabled = true;
                }

                if (type != null && type.length > 0)
                    $filterType.selectpicker('val', type);

                if ($filterType.prop('disabled') == isFilterTypeEnabled) {
                    $filterType.prop('disabled', !isFilterTypeEnabled);
                    $filterType.selectpicker('refresh');
                }

                onFilterClear();

                modalManager.show($modal);

                onModalScroll();

                setTimeout(function () {
                    $tableBody.closest('table').css('margin-top', String($filterContainer.outerHeight() - 10) + 'px')
                }, 0);
            };

            // event handlers

            function onModalShow() {
                $modal.on('scroll', onModalScroll);
            }

            function onModalHide() {
                $tableBody.empty();

                $modal.off('scroll', onModalScroll);
            }

            function onModalScroll() {
                var top = $modalBody.offset().top - $window.scrollTop();
                if (top >= 0)
                    top = 0;

                $filterContainer.css('margin-top', Math.abs(top) + 'px');
            }

            function onTypeChanged() {
                search();
            }

            function onFilterKeyDown(e) {
                if (e.which === 13) {
                    e.preventDefault();
                    e.stopPropagation();

                    onFilterChange(true);
                }
            }

            function onFilterChange(e) {
                var value = $filterKeywordInput.val();
                if (value.length > 0)
                    $filterKeywordClear.show();
                else
                    $filterKeywordClear.hide();

                if (e === true && (previousKeyword === null || previousKeyword.toUpperCase() !== value.toUpperCase()))
                    search();
            }

            function onFilterClear(e) {
                if (e) e.preventDefault();

                $filterKeywordInput.val('');

                onFilterChange(true);
            }

            function onRowClick(e) {
                var $this = $(this);
                if (typeof openerData.onSelected === 'function')
                    openerData.onSelected({
                        key: $this.data('id'),
                        text: $this.data('text'),
                        type: $this.data('type')
                    });

                modalManager.close($modal);
            }

            // private methods

            function search() {
                currentRowNumber = 1;

                $modal.animate({ scrollTop: 0 }, 500);

                doSearchRequest();
            }

            function bindTable(data) {
                if (currentRowNumber === 1)
                    $tableBody.empty();

                $tableBody.find('> tr:last').each(function () {
                    var $this = $(this);

                    if (!$this.data('identifier'))
                        $this.remove();
                });

                if (data.length === 0) {
                    if (currentRowNumber === 1)
                        $tableBody.append('<tr><td colspan="1" class="text-center"><h3>No Matches</h3></td></tr>');

                    return;
                }

                currentRowNumber += data.length;
                
                var selectedId = openerData.value; // parseInt(openerData.value);

                for (var i = 0; i < data.length; i++) {
                    var dataItem = data[i];
                    var $row = $('<tr>').attr('data-id', dataItem.id).data('text', dataItem.title).data('type', dataItem.type).append(
                        (function () {
                            var $cell = $('<td>').html(dataItem.title + '<div class="form-text">' + dataItem.type + ' ' + (dataItem.name != dataItem.title ? ' - ' + dataItem.slug : '') + '</div>');

                            if (typeof dataItem.parentNumber !== "undefined" && dataItem.parentNumber !== null) {
                                $cell.prepend(
                                    $('<div>').html('<small class="text-body-secondary"><i class="far fa-level-up"></i> ' + dataItem.parentType + ' Asset #' + String(dataItem.parentNumber) + ': ' + dataItem.parentTitle + '</small>')
                                );
                            }

                            return $cell;
                        })()
                    ).on('click', onRowClick);

                    if (dataItem.id === selectedId)
                        $row.addClass('selected');

                    $tableBody.append($row);
                }

                if (data.length >= itemsPerRequest) {
                    $tableBody.append(
                        $('<tr>').append(
                            $('<td colspan="1" class="text-center">').append(
                                $('<button type="button" class="btn btn-sm btn-primary"><i class="fas fa-spinner me-2"></i> Load More ...</button>').on('click', doSearchRequest)
                            )
                        )
                    );
                }
            }

            function doSearchRequest() {
                if ($.active > 0)
                    return;

                $loadingPanel.show();

                var data = {
                    site: openerData.site,
                    type: $filterType.selectpicker('val'),
                    keyword: $filterKeywordInput.val(),
                    start: currentRowNumber,
                    end: currentRowNumber + itemsPerRequest - 1,
                };

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/sites/pages/list',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: data,
                    success: function (result) {
                        bindTable(result);
                        previousKeyword = data.keyword;
                        onModalScroll();
                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    },
                    complete: function () {
                        $loadingPanel.hide();
                    },
                });
            }
        })();
    </script>
</insite:PageFooterContent>
