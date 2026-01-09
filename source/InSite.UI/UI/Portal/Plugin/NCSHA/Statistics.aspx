<%@ Page Language="C#" CodeBehind="Statistics.aspx.cs" Inherits="InSite.Custom.NCSHA.Analytics.Chart.View" MasterPageFile="~/UI/Layout/Admin/AdminModal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">

    <insite:ResourceLink runat="server" Type="Css" Url="/UI/Layout/common/parts/plugins/bootstrap-listbox/listbox.css" />

    <style type="text/css">
        main {
            padding: 1.3rem;
        }

        #app-growl {
            position: fixed;
            top: 1.3rem;
            right: 1.3rem;
            z-index: 1090;
            width: 400px;
        }

        .form-group > label {
            font-size: 16px;
        }

        .form-group.disabled > label {
            color: #7A8C97;
        }

        .hidden-field {
            position: absolute !important;
            bottom: 0;
            left: 50%;
            display: block !important;
            width: 0.5px !important;
            height: 100% !important;
            padding: 0 !important;
            opacity: 0 !important;
            border: none;
            visibility: hidden;
        }

        .category-filter-clear {
            position: absolute;
            line-height: 40px;
            padding: 0 15px;
            right: 2px;
            margin: 2px 0;
            border-radius: 4px;
            font-size: 13px;
            display: block;
        }

        table.filter-load {
            font-size: 14px;
            background-color: #fff;
        }

        table.filter-load td.cmd {
            text-align: right;
        }

        table.filter-load td.cmd a + a {
            margin-left: 5px;
        }

        #name_window .list-box > ul {
            height: 500px;
        }

        .filter-option-inner-inner {
            min-height: 20px;
        }

        label {
            font-weight: bold;
        }

        h4 {
            margin-bottom: 20px;
        }

        div.row + div.row {
            margin-top: 15px;
        }
    </style>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
    <div id="app-growl">
    </div>

    <div class="card mb-3">
        <div class="card-body">

            <div id="criteria_form" class="clearfix" style="position: relative;">
                <div class="row">

                    <div class="form-group col-lg-3 col-sm-4 mb-3 mb-lg-0">
                        <label class="mb-2">Series Name</label>
                        <div class="bootstrap-select insite-combobox" style="width: 100%;">
                            <button type="button" id="name_button" class="btn dropdown-toggle btn-combobox btn-sm" title="">
                                <div class="filter-option">
                                    <div class="filter-option-inner">
                                        <div class="filter-option-inner-inner"></div>
                                    </div>
                                </div>
                            </button>
                        </div>
                        <input type="text" id="code_input" required class="hidden-field" />
                        <div id="code_input_container" class="d-none">
                        </div>
                    </div>

                    <div class="form-group col-lg-2 col-sm-4 mb-3 mb-lg-0">
                        <label class="mb-2">Region</label>
                        <select id="region" name="region" class="insite-combobox" multiple data-width="100%" data-none-selected-text="" data-actions-box="true"></select>
                    </div>
                    <div class="form-group col-lg-2 col-sm-4 mb-3">
                        <label class="mb-2">Year</label>
                        <div class="clearfix">
                            <div style="width: 50%; float: left; padding-right: 7px;">
                                <select id="from_year" name="fromyear" class="insite-combobox" data-width="100%" data-none-selected-text="From"></select>
                            </div>
                            <div style="width: 50%; float: left; padding-left: 7px;">
                                <select id="to_year" name="toyear" class="insite-combobox" data-width="100%" data-none-selected-text="To"></select>
                            </div>
                        </div>
                    </div>

                    <div class="form-group col-lg-2 col-sm-4 mb-3 mb-lg-0">
                        <label class="mb-2">Options</label>
                        <div class="clearfix">
                            <div style="width: 50%; float: left; padding-right: 7px;">
                                <select id="aggregate_function" name="func" class="insite-combobox" data-width="100%" data-none-selected-text="" required>
                                    <option>Actual</option>
                                    <option>Sum</option>
                                    <option>Average</option>
                                    <option>Maximum</option>
                                    <option>Minimum</option>
                                </select>
                            </div>
                            <div style="width: 50%; float: left; padding-left: 7px;">
                                <select id="dataset_type" name="datasettype" class="insite-combobox" data-width="100%" data-none-selected-text="" required>
                                    <option selected>Line</option>
                                    <option>Bar</option>
                                </select>
                            </div>
                        </div>
                    </div>

                    <div class="form-group col-lg-3 col-sm-4 mb-3 mb-lg-0">
                        <label class="mb-2">Y-Axis</label>
                        <div class="row">
                            <div class="col-lg-6">
                                <div class="clearfix">
                                    <div style="float: left;">
                                        <select id="dataset_axis" name="datasetaxis" class="insite-combobox" data-width="100%" data-none-selected-text="" required>
                                            <option value="y-axis-1" selected>Primary</option>
                                            <option value="y-axis-2">Secondary</option>
                                        </select>
                                        <input type="hidden" id="dataset_axisunit" name="axisunit" />
                                        <input type="hidden" id="dataset_axisname" name="axisname" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-lg-6 d-none d-lg-inline-block text-end">
                                <button type="submit" class="btn btn-sm btn-icon btn-success" title="Add Series">
                                    <i class="fa fa-plus-circle"></i>
                                </button>
                                <button type="button" class="btn btn-sm btn-icon btn-default" title="Clear Chart" data-action="clear-chart">
                                    <i class="fa fa-sync"></i>
                                </button>
                            </div>
                        </div>
                    </div>

                    <div class="form-group col-lg-12 d-block d-lg-none text-end">
                        <label class="col-lg-12" style="margin-bottom: 9px;">&nbsp;</label>
                        <button type="submit" class="btn btn-sm btn-success" title="Add Series">
                            <i class="fa fa-plus-circle me-1"></i>Add Series
                        </button>
                        <button type="button" class="btn btn-sm btn-default" title="Clear Chart" data-action="clear-chart">
                            <i class="fa fa-sync me-1"></i>Clear Chart
                        </button>
                    </div>

                </div>

                <insite:LoadingPanel runat="server" Text="Loading..." VisibleOnLoad="true" />
            </div>
        </div>
    </div>

    <div class="card mb-3">
        <div class="card-body">
            <div class="text-end mb-3">
                <div class="d-md-inline-block mb-2 mb-md-0">
                    <button type="button" data-action="save-filter" class="btn btn-success btn-sm"><i class="far fa-cloud-upload me-1"></i>Save</button>
                    <button type="button" data-action="load-filter" class="btn btn-default btn-sm"><i class="far fa-folder-open me-1"></i>Load</button>
                    <button type="button" data-action="download-csv" class="btn btn-default btn-sm"><i class="fa fa-download me-1"></i>CSV</button>
                    <button type="button" data-action="download-png" class="btn btn-default btn-sm"><i class="fa fa-download me-1"></i>PNG</button>
                </div>
                <div class="d-md-inline-block">
                    <button type="button" data-action="toggle-legend" class="btn btn-default btn-sm">Legend</button>
                    <a href="/ui/portal/home" class="btn btn-sm btn-primary"><i class="fas fa-home me-1"></i> Portal Home</a>
                </div>

                <asp:HiddenField runat="server" ID="DownloadData" />
                <asp:Button runat="server" ID="DownloadCsvButton" Style="display: none;" />
                <asp:Button runat="server" ID="DownloadPngButton" Style="display: none;" />
            </div>

            <div>
                <canvas id="chart"></canvas>
            </div>
        </div>
    </div>

    <div runat="server" id="RefreshPanel" class="card border-0">
        <div class="card-body" style="text-align:right;">
            <insite:Button runat="server" ButtonStyle="Default" NavigateUrl="/ui/admin/plugin/ncsha/refresh-statistics" Icon="fa fa-sync" Text="Refresh Chart Data" />
        </div>
    </div>

    <div class="modal fade" id="name_window" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog" style="max-width: 850px;">
            <div class="modal-content">
                <div class="modal-body">
                    <div class="row">
                        <div class="form-group col-lg-4">
                            <label class="mb-3">Survey</label>
                        </div>
                        <div class="form-group col-lg-4">
                            <label class="mb-3">Category</label>
                            <div style="position: relative;">
                                <a href="#" id="category_filter_clear" title="Clear" class="category-filter-clear" style="display: none;"><i class="fas fa-times"></i></a>
                                <input type="text" id="category_filter" class="form-control" placeholder="Filter" autocomplete="off" />
                            </div>
                        </div>
                        <div class="form-group col-lg-4">
                            <label class="mb-3">Name</label>
                        </div>
                    </div>

                    <div class="row">
                        <div class="form-group col-lg-4">
                            <select id="program_select" required></select>
                        </div>
                        <div class="form-group col-lg-4">
                            <select id="category_select" required></select>
                        </div>
                        <div class="form-group col-lg-4">
                            <select id="code_select" required multiple></select>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12 text-end">
                            <button type="submit" class="btn btn-sm btn-success">OK</button>
                            <button type="button" class="btn btn-sm btn-default" data-bs-dismiss="modal">Cancel</button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="save_filter_window" tabindex="-1" role="dialog" aria-hidden="true" data-bs-backdrop="static" data-bs-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Save Filter</h5>
                </div>
                <div class="modal-body">
                    <div class="form-group mb-3">
                        <label class="form-label">Name</label>
                        <input type="text" name="name" class="form-control" required maxlength="128" autocomplete="off" />
                    </div>

                    <div class="text-end">
                        <button type="submit" class="btn btn-success btn-sm"><i class='fas fa-cloud-upload me-1'></i>Save</button>
                        <button type="button" class="btn btn-default btn-sm" data-bs-dismiss="modal"><i class='fas fa-ban me-1'></i>Cancel</button>
                    </div>

                    <insite:LoadingPanel runat="server" Text="Loading..." />
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-lg fade" id="load_filter_window" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title">Load Filter</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="form-group mb-3">
                        <table class="table filter-load">
                            <thead>
                                <tr>
                                    <th>Name</th>
                                    <th style="width: 100px;">Date</th>
                                    <th style="width: 65px;"></th>
                                </tr>
                            </thead>
                            <tbody></tbody>
                        </table>
                    </div>

                    <div class="text-end">
                        <button type="button" class="btn btn-default btn-sm" data-bs-dismiss="modal"><i class='fas fa-ban me-1'></i>Close</button>
                    </div>

                    <insite:LoadingPanel runat="server" Text="Loading..." />
                </div>
            </div>
        </div>
    </div>

    <div runat="server" id="WelcomeWindowPanel" class="modal fade" tabindex="-1" role="dialog" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title"><asp:Literal runat="server" ID="WelcomeModelTitle" /></h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <asp:Literal runat="server" ID="WelcomeModelBodyHtml" />
                </div>
            </div>
        </div>
    </div>

    <insite:PageFooterContent runat="server">

        <insite:ResourceBundle runat="server" Type="JavaScript">
            <Items>
                <insite:ResourceBundleFile Url="/UI/Layout/common/parts/js/bootstrap-validator.min.js" />
                <insite:ResourceBundleFile Url="/UI/Layout/common/parts/plugins/bootstrap-listbox/listbox.js" />
                <insite:ResourceBundleFile Url="/UI/Portal/Plugin/NCSHA/Statistics.js" />
            </Items>
        </insite:ResourceBundle>

        <script type="text/javascript">

            $(document).ready(function () {
                $('#<%= WelcomeWindowPanel.ClientID %>.modal')
                    .modal('show');
            });

            $(document).ready(function () {
                let isLoadingCriteria = false;
                let criteriaData = null;
                let defaultColorId = 0;

                const defaultColors = [
                    '#FF8080',
                    '#4e4ecc',
                    '#45B39D',
                    '#004980',
                    '#F31F66',
                    '#E97132',
                    '#C39BD3',
                    '#94C800',
                    '#b38a05',
                    '#6495ED',
                    '#008080',
                    '#E29A86',
                    '#333399',
                    '#16A085',
                    '#F0EA00',
                    '#F7CAC9',
                    '#954535',
                    '#FF9900',
                    '#99CCFF',
                    '#993366',
                    '#f5e616',
                    '#3ca658',
                    '#33CCCC',
                    '#e65535',
                    '#969696',
                    '#666699',
                    '#FFBC79',
                    '#070752',
                    '#C6E21E',
                    '#FFBF00',
                    '#3366FF',
                    '#CC99FF',
                    '#D12F7C',
                    '#28AFCA',
                    '#DC143C',
                    '#FFFF99',
                    '#968089',
                    '#9F617A',
                    '#0000FF',
                    '#6F7F7A',
                    '#800080',
                    '#00BFFF',
                    '#0E6251',
                    '#2a4a6b',
                    '#00FF00',
                    '#C0C0C0',
                    '#B9B64A',
                    '#5a8dbf',
                    '#656582',
                    '#FFCC99',
                    '#CCFFCC',
                    '#9c519c',
                    '#FFCC00',
                    '#CF71AF',
                    '#3D9140',
                    '#49759C',
                    '#163D64'
                ];
                const programs = <%= JsonPrograms %>;

                const chartConfig = {
                    type: 'bar',
                    data:
                    {
                        year: { from: null, to: null },
                        labels: [],
                        datasets: []
                    },
                    options: {
                        responsive: true,
                        plugins: {
                            title: {
                                display: false,
                                text: 'NCSHA Chart'
                            },
                            legend: {
                                display: true,
                            },
                            tooltip: {
                                mode: 'point',
                                intersect: false,
                                position: 'nearest',
                                callbacks: {
                                    title: function (items) {
                                        let title = '';

                                        if (items.length > 0) {
                                            const item = items[0];

                                            title += item.label;
                                            title += ' (';
                                            title += formatNumber(item.raw, 2);
                                            title += ')';
                                        }

                                        return title;
                                    },
                                    label: function (item, data) {
                                        return item.dataset.label;
                                    },
                                },
                            },
                        },
                        hover: {
                            mode: 'nearest',
                            intersect: true
                        },
                        scales: {
                            x: {
                                display: true,
                                title: {
                                    display: true,
                                    text: 'Year'
                                }
                            },
                            'y-axis-1': {
                                name: 'Primary',
                                beginAtZero: true,
                                display: false,
                                position: 'right',
                                title: {
                                    display: true,
                                    text: null
                                },
                                ticks: {
                                    callback: yAxisTickCallback
                                },
                            },
                            'y-axis-2': {
                                name: 'Secondary',
                                beginAtZero: true,
                                display: false,
                                position: 'left',
                                title: {
                                    display: true,
                                    text: null
                                },
                                ticks: {
                                    callback: yAxisTickCallback
                                },
                            },
                        },
                    }
                };

                try {
                    chartConfig.options.plugins.legend.display = window.sessionStorage.getItem('ncsha.analysis.chart.legend') !== 'false';
                } catch (e) {

                }

                // initialization

                $('select.insite-combobox')
                    .each(function () {
                        inSite.common.comboBox.init({
                            id: this,
                            style: 'btn-combobox btn-sm'
                        });
                    })
                    .on('loaded.bs.select', onSelectpickerChanged)
                    .on('changed.bs.select', onSelectpickerChanged)
                    .on('updated.bs.select', onSelectpickerChanged);

                $('.panel-ctrl [data-action="minimize"]').on('click', function (e) {
                    e.preventDefault();
                    $(this).closest('.panel').addClass('minimized');
                });

                $('.panel-ctrl [data-action="restore"]').on('click', function (e) {
                    e.preventDefault();
                    $(this).closest('.panel').removeClass('minimized');
                });

                function onSelectpickerChanged(e) {
                    const selectpicker = $(this).data('selectpicker');
                    const value = selectpicker.val();
                    if (value == null || value.length == 0)
                        selectpicker.$button.addClass('empty');
                    else
                        selectpicker.$button.removeClass('empty');
                }

                // form validation

                $.fn.validator.Constructor.FOCUS_OFFSET = 90;
                $.fn.validator.Constructor.INPUT_SELECTOR = ':input:not([type="submit"], [type="reset"], button)';

                // search criteria

                let criteriaStack = [];

                const $codeInput = $('#code_input');
                const $codeInputContainer = $('#code_input_container');
                const $region = $('#region').on('change', onRegionChanged);
                const $fromYear = $('#from_year').on('change', onFromYearChanged);
                const $toYear = $('#to_year').on('change', onToYearChanged);
                const $datasetType = $('#dataset_type');
                const $datasetAxis = $('#dataset_axis');
                const $datasetAxisUnit = $('#dataset_axisunit');
                const $datasetAxisName = $('#dataset_axisname');

                const $formCriteria = $('#criteria_form')
                    .validator({
                        feedback: {
                            success: 'fa fa-check',
                            error: 'fa fa-times'
                        },
                    })
                    .on('invalid.bs.validator', ncsha.validator.onInvalid)
                    .on('valid.bs.validator', ncsha.validator.onValid);

                $formCriteria
                    .find("button[type='submit']")
                    .on('click', onSubmitSearchCriteria);

                bindCriteria();

                $formCriteria.find('.loading-panel').hide();

                function onRegionChanged() {
                    bindFromYear(true);
                    bindToYear(true);
                }

                function onFromYearChanged() {
                    bindToYear(true);
                }

                function onToYearChanged() {

                }

                function onSubmitSearchCriteria(e) {
                    if (e.isDefaultPrevented())
                        return;

                    e.preventDefault();

                    if ($(this).hasClass("disabled")) {
                        $formCriteria.find("[required]").trigger("input");
                        return;
                    }

                    {
                        const yAxis = getYAxis($datasetAxis.selectpicker('val'));
                        if (yAxis !== null) {
                            $datasetAxisUnit.val(yAxis.title.text);
                            $datasetAxisName.val(yAxis.name);
                        }
                    }

                    const $loadingPanel = $formCriteria.find('.loading-panel').show();

                    const data = [{ name: "action", value: "data" }];
                    $formCriteria.find(":input").each(function (index, input) {
                        data.push({ name: input.name, value: $(input).val() });
                    });

                    $.ajax({
                        dataType: 'json',
                        type: 'POST',
                        data: data,
                        success: function (result) {
                            if (result.type === 'ERROR') {
                                ncsha.web.showStatus('danger', 'Error!', '<br>' + result.message);
                            } else {
                                const yAxisId = $datasetAxis.selectpicker('val');
                                const datasetType = $datasetType.selectpicker('val');

                                for (let i = 0; i < result.length; i++)
                                    addDataset(result[i], yAxisId, datasetType);

                                onDatasetAdded();

                                const stackObj = data.reduce(function (o, d) {
                                    if (d.value !== null && d.value !== '') {
                                        if (!o.hasOwnProperty(d.name)) {
                                            o[d.name] = d.value;
                                        } else {
                                            if (!(o[d.name] instanceof Array))
                                                o[d.name] = [o[d.name]];

                                            o[d.name].push(d.value);
                                        }
                                    }

                                    return o;
                                }, {});

                                if (stackObj.code && !(stackObj.code instanceof Array))
                                    stackObj.code = [stackObj.code];

                                if (stackObj.region && !(stackObj.region instanceof Array))
                                    stackObj.region = [stackObj.region];

                                criteriaStack.push(stackObj);
                            }
                        },
                        error: function (xhr) {
                            ncsha.web.showStatus('danger', 'Error!', '<br>An error occured on the server side.');
                        },
                        complete: function () {
                            $loadingPanel.hide();
                        },
                    });
                }

                function bindCriteria() {
                    bindRegion(true);
                    bindFromYear(true);
                    bindToYear(true);
                }

                function bindRegion(preserveValue) {
                    const value = preserveValue ? $region.selectpicker('val') : null;

                    $region.empty();

                    if (criteriaData != null && criteriaData.regions != null) {
                        for (let i = 0; i < criteriaData.regions.length; i++)
                            $region.append($('<option>').text(criteriaData.regions[i].name));
                    }

                    refreshSelector($region, !!criteriaData?.regions, value);
                }

                function bindFromYear(preserveValue) {
                    const value = preserveValue ? $fromYear.selectpicker('val') : null;

                    $fromYear.empty();

                    if (criteriaData != null && criteriaData.years != null) {
                        let region = $region.val();
                        if (region != null && region.length > 0) {
                            let result = $.grep(criteriaData.regions, function (e) { return e.name == region; });
                            if (result.length > 0)
                                region = result[0];
                            else
                                result = null;
                        } else {
                            region = null;
                        }

                        $fromYear.append('<option>');
                        for (let i = 0; i < criteriaData.years.length; i++) {
                            const year = criteriaData.years[i];
                            if (region != null && $.inArray(year, region.exclude) != -1)
                                continue;

                            $fromYear.append($('<option>').text(year));
                        }
                    }

                    refreshSelector($fromYear, !!criteriaData?.years, value);
                }

                function bindToYear(preserveValue) {
                    const value = preserveValue ? $toYear.selectpicker('val') : null;

                    $toYear.empty();

                    if (criteriaData != null && criteriaData.years != null) {
                        let region = $region.val();
                        if (region != null && region.length > 0) {
                            let result = $.grep(criteriaData.regions, function (e) { return e.name == region; });
                            if (result.length > 0)
                                region = result[0];
                            else
                                result = null;
                        } else {
                            region = null;
                        }

                        let fromYear = parseInt($fromYear.val());
                        if (isNaN(fromYear))
                            fromYear = null;

                        $toYear.append('<option>');
                        for (let i = 0; i < criteriaData.years.length; i++) {
                            const year = criteriaData.years[i];
                            if (region != null && $.inArray(year, region.exclude) != -1 || fromYear != null && year < fromYear)
                                continue;

                            $toYear.append($('<option>').text(year));
                        }
                    }

                    refreshSelector($toYear, !!criteriaData?.years, value);
                }

                function loadCriteria() {
                    if (isLoadingCriteria)
                        return;

                    isLoadingCriteria = true;

                    const $loadingPanel = $formCriteria.find('.loading-panel').show();

                    $.ajax({
                        type: 'POST',
                        data: { action: "criteria", code: getCodeInput() },
                        traditional: true,
                        success: function (result) {
                            criteriaData = result;
                            bindCriteria();
                        },
                        error: function (xhr) {
                            ncsha.web.showStatus('danger', 'Error!', '<br>An error occured on the server side.');
                        },
                        complete: function () {
                            $loadingPanel.hide();
                            isLoadingCriteria = false;
                        },
                    });
                }

                // name selector

                const $nameButton = $('#name_button').on('click', function () {
                    $nameWindow.modal('show');
                });
                const $programSelect = $('#program_select').listbox().on('change', function () {
                    bindProgramCategories();
                });
                const $categorySelect = $('#category_select').listbox().on('change', function () {
                    bindCategoryFields();
                });
                const $codeSelect = $('#code_select').listbox();
                const $categoryFilter = $('#category_filter').on('change paste keyup', onCategoryFilterChange);
                const $categoryFilterClear = $('#category_filter_clear').on('click', onCategoryFilterClear);
                const $nameWindow = $('#name_window')
                    .validator({
                        feedback: {
                            success: 'fa fa-check',
                            error: 'fa fa-times'
                        },
                    })
                    .on('invalid.bs.validator', ncsha.validator.onInvalid)
                    .on('valid.bs.validator', ncsha.validator.onValid)
                    .on('shown.bs.modal', function () {
                        const $this = $(this);
                        if ($this.data('inited') !== true) {
                            bindProgramSelector();
                            $this.data('inited', true);
                        }
                    });

                $nameWindow.find("button[type='submit']")
                    .on('click', function (e) {
                        if (e.isDefaultPrevented())
                            return;

                        e.preventDefault();

                        if ($(this).hasClass("disabled")) {
                            $nameWindow.find("[required]").trigger("input");
                            return;
                        }

                        const $options = $codeSelect.listbox('opt');
                        if ($options === null || $options.length < 1)
                            return;

                        let title = '';

                        $options.each(function () {
                            const $this = $(this);
                            title += ', ' + $this.text();
                        });

                        title = title.substring(1);

                        setCodeInput($codeSelect.listbox('val'));
                        $nameButton.attr('title', title).find('.filter-option-inner-inner').text(title);

                        $(this).closest('.modal').modal('hide');

                        loadCriteria();
                    });

                function bindProgramSelector() {
                    $programSelect.empty();

                    for (let i = 0; i < programs.length; i++) {
                        const dataItem = programs[i]
                        $programSelect.append($('<option>').val(dataItem.code).text(dataItem.title));
                    }

                    $programSelect.val(null).listbox('update');

                    bindProgramCategories();
                }

                function bindProgramCategories() {
                    $categorySelect.empty();

                    const programCode = $programSelect.listbox('val');
                    if (programCode) {
                        for (let x = 0; x < programs.length; x++) {
                            const program = programs[x];
                            if (program.code === programCode) {
                                for (let y = 0; y < program.categories.length; y++) {
                                    const category = program.categories[y];
                                    $categorySelect.append($('<option>').text(category.title));
                                }

                                break;
                            }
                        }
                    }

                    const filterValue = $categoryFilter.val();
                    if (filterValue)
                        $categoryFilterClear.show();
                    else
                        $categoryFilterClear.hide();

                    $categorySelect.val(null).listbox('setFilter', filterValue).listbox('update');

                    bindCategoryFields();
                }

                function bindCategoryFields() {
                    $codeSelect.empty();

                    const programCode = $programSelect.listbox('val');
                    const categoryTitle = $categorySelect.listbox('val');

                    if (programCode && categoryTitle) {
                        for (let x = 0; x < programs.length; x++) {
                            const program = programs[x];
                            if (program.code === programCode) {
                                for (let y = 0; y < program.categories.length; y++) {
                                    const category = program.categories[y];
                                    if (category.title === categoryTitle) {
                                        for (let z = 0; z < category.fields.length; z++) {
                                            const field = category.fields[z];
                                            $codeSelect.append($('<option>').val(field.code).text(field.title).attr('title', field.code));
                                        }

                                        break;
                                    }
                                }

                                break;
                            }
                        }
                    }

                    $codeSelect.val(null).listbox('update');
                }

                let categoryFilterTimeoutHandler = null;

                function onCategoryFilterClear(e) {
                    e.preventDefault();

                    $categoryFilter.val('');

                    onCategoryFilterTimeout();
                }

                function onCategoryFilterChange() {
                    if (categoryFilterTimeoutHandler !== null)
                        clearTimeout(categoryFilterTimeoutHandler);

                    categoryFilterTimeoutHandler = setTimeout(onCategoryFilterTimeout, 500);
                }

                function onCategoryFilterTimeout() {
                    if (categoryFilterTimeoutHandler !== null)
                        clearTimeout(categoryFilterTimeoutHandler);

                    categoryFilterTimeoutHandler = null;

                    const value = $categoryFilter.val();

                    if (value)
                        $categoryFilterClear.show();
                    else
                        $categoryFilterClear.hide();

                    $categorySelect.listbox('filter', value);
                }

                // chart

                const chart = new Chart('chart', chartConfig);

                $('[data-action="clear-chart"]').on('click', function () {
                    defaultColorId = 0;
                    chartConfig.data.year.from = null;
                    chartConfig.data.year.to = null;
                    chartConfig.data.labels = [];
                    chartConfig.data.datasets = [];

                    for (let n in chartConfig.options.scales) {
                        if (!n.startsWith('y'))
                            continue;

                        const yAxis = chartConfig.options.scales[n];
                        yAxis.display = false;
                        yAxis.title.text = null;
                    }

                    chart.update();

                    $downloadCsvButton.attr('disabled', true);
                    $downloadPngButton.attr('disabled', true);
                    $saveFilterButton.attr('disabled', true);

                    criteriaStack = [];
                });

                function addDataset(source, yAxisId, datasetType) {
                    let minYear = null, maxYear = null;

                    for (let key in source.data) {
                        if (!source.data.hasOwnProperty(key))
                            continue;

                        const year = parseInt(key);

                        if (minYear == null || minYear > year)
                            minYear = year;

                        if (maxYear == null || maxYear < year)
                            maxYear = year;
                    }

                    if (minYear == null || maxYear == null)
                        return;

                    const yAxis = getYAxis(yAxisId);
                    if (yAxis === null)
                        return;

                    if (!yAxis.title.text)
                        yAxis.title.text = source.unit;

                    yAxis.display = true;

                    const chartData = chartConfig.data;
                    const chartYear = chartData.year;

                    if (chartYear.from == null || chartYear.from > minYear) {
                        if (chartYear.from != null) {
                            for (let year = chartYear.from - 1; year >= minYear; year--) {
                                chartData.labels.unshift(String(year));

                                for (let i = 0; i < chartData.datasets.length; i++)
                                    chartData.datasets[i].data.unshift(null);
                            }
                        }

                        chartYear.from = minYear;
                    }

                    if (chartYear.to == null || chartYear.to < maxYear) {
                        for (let year = chartYear.to == null ? minYear : chartYear.to + 1; year <= maxYear; year++) {
                            chartData.labels.push(String(year));

                            for (let i = 0; i < chartData.datasets.length; i++)
                                chartData.datasets[i].data.push(null);
                        }

                        chartYear.to = maxYear;
                    }

                    const color = source.color == null ? defaultColors[defaultColorId++ % defaultColors.length] : source.color;
                    const isBar = datasetType === 'Bar';
                    const dataset = isBar
                        ? {
                            xAxisID: 'x',
                            yAxisID: yAxisId,
                            type: 'bar',
                            label: source.name,
                            backgroundColor: color,
                            borderColor: color,
                            borderWidth: 2,
                            data: [],
                        } : {
                            xAxisID: 'x',
                            yAxisID: yAxisId,
                            type: 'line',
                            label: source.name,
                            fill: false,
                            backgroundColor: color,
                            borderColor: color,
                            borderWidth: 3,
                            data: [],
                            tension: 0.3
                        };

                    for (let i = 0; i < chartData.labels.length; i++) {
                        const year = chartData.labels[i];
                        dataset.data.push(source.data.hasOwnProperty(year) ? source.data[year] : null);
                    }

                    if (!isBar) {
                        let insertIndex = -1;

                        for (let i = 0; i < chartData.datasets.length; i++) {
                            if (chartData.datasets[i].type == 'bar') {
                                insertIndex = i;
                                break;
                            }
                        }

                        if (insertIndex >= 0)
                            chartData.datasets.splice(insertIndex, 0, dataset);
                        else
                            chartData.datasets.push(dataset);
                    } else {
                        chartData.datasets.push(dataset);
                    }
                }

                function onDatasetAdded() {
                    chart.update();

                    $downloadCsvButton.attr('disabled', false);
                    $downloadPngButton.attr('disabled', false);
                    $saveFilterButton.attr('disabled', false);
                }

                function yAxisTickCallback(value, index, values) {
                    let isDecimal = false;

                    for (let i = 0; i < values.length; i++) {
                        if (values[i].value % 1 != 0) {
                            isDecimal = true;
                            break;
                        }
                    }

                    return formatNumber(value, isDecimal ? 1 : 0);
                }

                // chart commands

                let isFilterLoading = false;

                const $toggleLegendButton = $('[data-action="toggle-legend"]').on('click', onToggleLegend);
                const $downloadCsvButton = $('[data-action="download-csv"]').on('click', onDownloadCsv).attr('disabled', true);
                const $downloadPngButton = $('[data-action="download-png"]').on('click', onDownloadPng).attr('disabled', true);
                const $saveFilterButton = $('[data-action="save-filter"]').on('click', function () { $saveFilterWindow.modal('show'); }).attr('disabled', true);
                const $loadFilterButton = $('[data-action="load-filter"]').on('click', function () { $loadFilterWindow.modal('show'); });
                const $saveFilterWindow = $('#save_filter_window')
                    .validator({
                        feedback: {
                            success: 'fa fa-check',
                            error: 'fa fa-times'
                        },
                    })
                    .on('invalid.bs.validator', ncsha.validator.onInvalid)
                    .on('valid.bs.validator', ncsha.validator.onValid)
                    .on('show.bs.modal', function () {
                        $saveFilterWindow.find("input[name='name']").val("");
                    });

                $saveFilterWindow
                    .find("button[type='submit']")
                    .on('click', function (e) {
                        if (e.isDefaultPrevented())
                            return;

                        e.preventDefault();

                        const nameValue = $saveFilterWindow.find('input[name="name"]').val();

                        if (nameValue == null || nameValue.length == 0) {
                            $saveFilterWindow.find("[required]").trigger("input");
                            return;
                        }

                        const $loadingPanel = $saveFilterWindow.find('.loading-panel').show();

                        saveFilter(nameValue).always(function () { $loadingPanel.hide(); });
                    });

                const $loadFilterWindow = $('#load_filter_window')
                    .validator({
                        feedback: {
                            success: 'fa fa-check',
                            error: 'fa fa-times'
                        },
                    })
                    .on('invalid.bs.validator', ncsha.validator.onInvalid)
                    .on('valid.bs.validator', ncsha.validator.onValid)
                    .on('show.bs.modal', function () {
                        if (isFilterLoading)
                            return;

                        isFilterLoading = true;

                        const $form = $(this);
                        const $loadingPanel = $form.find('.loading-panel').show();
                        const $tbody = $form.find('table > tbody');

                        loadFilterList($tbody).always(function () { $loadingPanel.hide(); isFilterLoading = false; });
                    });

                $loadFilterWindow
                    .find("button[type='submit']")
                    .on('click', function (e) {
                        if (e.isDefaultPrevented())
                            return;

                        e.preventDefault();

                        alert('Not Implemented!');
                    });

                updateToggleLegend();

                function onToggleLegend() {
                    chartConfig.options.plugins.legend.display = !chartConfig.options.plugins.legend.display;
                    chart.update();

                    try {
                        window.sessionStorage.setItem('ncsha.analysis.chart.legend', chartConfig.options.plugins.legend.display);
                    } catch (e) {

                    }

                    updateToggleLegend();
                }

                function updateToggleLegend() {
                    if (chartConfig.options.plugins.legend.display) {
                        $toggleLegendButton.html('<i class="far fa-eye-slash me-1"></i> Hide Legend');
                    } else {
                        $toggleLegendButton.html('<i class="far fa-eye me-1"></i> Show Legend');
                    }
                }

                function onDownloadCsv() {
                    if (chartConfig.data.labels.length == 0 || chartConfig.data.datasets.length == 0)
                        return;

                    const postData = { model: { Datasets: [] }, criteriaJson: criteriaStack };

                    for (let i = 0; i < chartConfig.data.datasets.length; i++) {
                        const dataset = { Title: chartConfig.data.datasets[i].label, Items: [] };

                        for (let j = 0; j < chartConfig.data.labels.length; j++) {
                            dataset.Items.push({ Year: chartConfig.data.labels[j], Value: chartConfig.data.datasets[i].data[j] });
                        }

                        postData.model.Datasets.push(dataset);
                    }

                    $downloadCsvButton.blur();

                    $("#<%= DownloadData.ClientID %>").val(JSON.stringify(postData));

                    __doPostBack("<%= DownloadCsvButton.UniqueID %>", "");
                }

                function onDownloadPng() {
                    const pxRatio = chart.options.devicePixelRatio;
                    
                    chart.options.devicePixelRatio = 2;
                    chart.resize(1200, 600);

                    const data = chart.toBase64Image();
                    const isValid = typeof data === 'string' && data.length > 0 && data !== 'data:,';

                    if (isValid) {
                        const postData = { image: data, criteriaJson: criteriaStack };

                        $("#<%= DownloadData.ClientID %>").val(JSON.stringify(postData));

                        __doPostBack("<%= DownloadPngButton.UniqueID %>", "");
                    }

                    chart.options.devicePixelRatio = pxRatio;
                    chart.resize();
                }

                function saveFilter(name, overwrite) {
                    const defer = $.Deferred();

                    if (criteriaStack instanceof Array && criteriaStack.length > 0) {
                        const data = {
                            action: "filter-post",
                            name: name,
                            data: JSON.stringify(criteriaStack)
                        };

                        if (typeof overwrite === 'boolean')
                            data.overwrite = overwrite;

                        $.ajax({
                            dataType: 'json',
                            type: 'POST',
                            data: data,
                            success: function (result) {
                                if (result.type === 'ERROR') {
                                    ncsha.web.showStatus('danger', 'Error!', result.message);
                                } else if (result.type === 'FILTER') {
                                    if (result.code === 'EXISTS') {
                                        if (confirm('The filter criteria "' + String(name) + '" already exists. Do you want to overwrite it?'))
                                            saveFilter(name, true)
                                                .done(defer.resolve)
                                                .fail(defer.reject);
                                        else
                                            defer.resolve();
                                    } else if (result.code === 'OK') {
                                        ncsha.web.showStatus('success', 'Saved!', 'The filter criteria have been successfully saved.');
                                        defer.resolve();
                                        $saveFilterWindow.modal('hide');
                                    } else {
                                        alert('Unexpected result type: ' + JSON.stringify(result));
                                        defer.reject();
                                    }
                                } else {
                                    alert('Unexpected result type: ' + JSON.stringify(result));
                                    defer.reject();
                                }
                            },
                            error: function (xhr) {
                                defer.reject();
                                ncsha.web.showStatus('danger', 'Error!', '<br>An error occured on the server side.');
                            },
                        });
                    } else {
                        defer.reject();
                        ncsha.web.showStatus('danger', 'Error!', '<br>Invalid request.');
                    }

                    return defer.promise();
                }

                function loadFilterList($table) {
                    const defer = $.Deferred();

                    $table.empty();

                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { action: "filter-get" },
                        success: function (result) {
                            if (result.type === 'ERROR') {
                                defer.reject();
                                ncsha.web.showStatus('danger', 'Error!', result.message);
                            } else if (result.type === 'FILTER') {
                                defer.resolve();
                                if (!result.data || result.data.length === 0) {
                                    $table.append('<tr><th colspan="3" style="text-align:center;">No Data</th></tr>');
                                } else {
                                    for (let i = 0; i < result.data.length; i++) {
                                        const group = result.data[i];

                                        $table.append(
                                            $('<tr>').append(
                                                $('<th colspan="3">').text(group.name)
                                            )
                                        );

                                        for (let j = 0; j < group.items.length; j++) {
                                            const item = group.items[j];

                                            const $deleteCommand = $('<a href="#delete" title="Delete"><i class="far fa-trash-alt"></i></a>');
                                            if (!group.allowDelete)
                                                $deleteCommand.css('visibility', 'hidden');
                                            else
                                                $deleteCommand.on('click', onFilterDelete);

                                            $table.append(
                                                $('<tr>').data('id', item.id).append(
                                                    $('<td>').text(item.name),
                                                    $('<td>').addClass('text-nowrap text-end').text(item.date),
                                                    $('<td class="cmd">').append(
                                                        $('<a title="Open"><i class="far fa-folder-open"></i></a>').attr('href', '?filter=' + item.id),
                                                        $deleteCommand
                                                    )
                                                )
                                            );
                                        }
                                    }
                                }
                            }
                        },
                        error: function (xhr) {
                            defer.reject();
                            ncsha.web.showStatus('danger', 'Error!', '<br>An error occured on the server side.');
                        },
                    });

                    return defer.promise();
                }

                function onFilterDelete(e) {
                    e.preventDefault();

                    const $row = $(this).closest('tr');
                    const id = $row.data('id');

                    if (!id || !confirm('Are you sure you want to delete this filter?'))
                        return;

                    isFilterLoading = true;

                    const $form = $loadFilterWindow;
                    const $loadingPanel = $form.find('.loading-panel').show();

                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        data: { action: "filter-delete", id: id },
                        success: function (result) {
                            if (result.type === "ERROR") {
                                onRequestCompleted();
                                ncsha.web.showStatus('danger', 'Error!', result.message);
                            } else {
                                const $tbody = $form.find('table > tbody');

                                loadFilterList($tbody).always(onRequestCompleted);
                            }
                        },
                        error: function (xhr) {
                            onRequestCompleted();
                            ncsha.web.showStatus('danger', 'Error!', '<br>An error occured on the server side.');
                        },
                    });

                    function onRequestCompleted() {
                        $loadingPanel.hide();
                        isFilterLoading = false;
                    }
                }

                // helpers

                function setCodeInput(values) {
                    $codeInputContainer.empty();

                    if (values && values instanceof Array && values.length > 0) {
                        for (let i = 0; i < values.length; i++)
                            $codeInputContainer.append($('<input type="hidden" name="code" />').val(values[i]));

                        $codeInput.val('value');
                    } else {
                        $codeInput.val('');
                    }

                    $codeInput.trigger('change');
                }

                function getCodeInput() {
                    const result = [];
                    $codeInputContainer.find('input').each(function () {
                        result.push(this.value);
                    });
                    return result.length == 0 ? null : result;
                }

                function getYAxis(id) {
                    const result = typeof id == 'string' && id.length > 0 && id.startsWith('y') ? chartConfig.options.scales[id] : null;

                    if (result === null)
                        console.error(String(arguments.callee.name) + ': Y axis not found');
                    else
                        return result;
                }

                function refreshSelector($selectElement, enabled, value) {
                    $selectElement.selectpicker("destroy");

                    inSite.common.comboBox.init({
                        id: $selectElement,
                        style: 'btn-combobox btn-sm'
                    });

                    if (enabled) {
                        $selectElement.removeAttr('disabled').closest('.form-group').removeClass('disabled');
                        $selectElement.selectpicker("setStyle", "disabled", "remove");
                    } else {
                        $selectElement.attr('disabled', '').closest('.form-group').addClass('disabled');
                        $selectElement.selectpicker("setStyle", "disabled", "add");
                    }

                    if (value) {
                        $selectElement.selectpicker("val", value);
                    }

                    $selectElement.selectpicker().trigger('updated.bs.select');

                    if ($selectElement.prop('tagName') === 'SELECT') {
                        $selectElement.trigger('change');
                    }
                }

                function formatNumber(value, fraction) {
                    let result = '';

                    if (value < 0)
                        result = '-';

                    value = Math.abs(value);

                    const integer = Math.floor(value).toString();
                    for (let i = 0; i < integer.length; i++) {
                        if (i != 0 && (integer.length - i) % 3 == 0)
                            result += ',';

                        result += integer[i];
                    }

                    if (fraction) {
                        const fractional = (value % 1).toFixed(fraction);
                        result += fractional.substring(1);
                    }

                    return result;
                }

                // init

                {
                    const defaultChartData = <%= DefaultChartData %>;
                    if (defaultChartData !== null) {
                        criteriaStack = JSON.parse(defaultChartData.filter);
                        if (criteriaStack && criteriaStack.length == defaultChartData.datasets.length) {
                            const axisIdName = $datasetAxis.attr('name');
                            const axisTypeName = $datasetType.attr('name');
                            for (let i = 0; i < defaultChartData.datasets.length; i++) {
                                const stack = criteriaStack[i];
                                const dataset = defaultChartData.datasets[i];
                                for (let j = 0; j < dataset.length; j++)
                                    addDataset(dataset[j], stack[axisIdName], stack[axisTypeName]);
                                onDatasetAdded();
                            }
                        } else {
                            criteriaStack = [];
                        }
                    }
                }
            });

        </script>
    </insite:PageFooterContent>

</asp:Content>
