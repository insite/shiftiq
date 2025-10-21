<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CriterionInput.ascx.cs" Inherits="InSite.Admin.Assessments.Criteria.Controls.CriterionInput" %>

<div class="row mb-3">

    <div class="col-lg-6 mb-3 mb-lg-0">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3>Filter Type</h3>

                <div class="form-group mb-3">
                    <div>
                        <insite:RadioButton runat="server" ID="CriterionTypeNone" GroupName="CriterionType" Checked="true" Text="<strong>Include All Questions</strong><br><div class='form-text'>Include all of the questions in the set.</div>" />
                        <insite:RadioButton runat="server" ID="CriterionTypeTag" GroupName="CriterionType" Text="<strong>Question Tag Filter</strong><br><div class='form-text'>Filter the questions in the set so that only those with matching tags are included.</div>" />
                        <insite:RadioButton runat="server" ID="CriterionTypePivot" GroupName="CriterionType" Text="<strong>Pivot Table Filter</strong><br><div class='form-text'>Filter the questions in the set using a pivot table on multiple question attributes.</div>" />
                    </div>
                </div>

            </div>

        </div>
    </div>

    <div class="col-lg-6 mb-3 mb-lg-0">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3>Configuration</h3>

                <div class="form-group mb-3">
                    <label class="form-label">
                        Question Set Weight
                        <insite:RequiredValidator runat="server" ControlToValidate="SetWeight" ValidationGroup="Assessment" />
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="SetWeight" DecimalPlaces="2" MinValue="0" MaxValue="1" />
                    </div>
                    <div class="form-text">
                        The desired weighting for the question set to which this criterion applies, within the overall specification. 
                        The sum of all question set weights for the criteria in a specification must equal 1 (i.e. 100 percent).
                    </div>
                </div>

                <div runat="server" id="QuestionLimitField" class="form-group mb-3">
                    <label class="form-label">
                        Question Item Limit
                        <insite:RequiredValidator runat="server" ControlToValidate="QuestionLimit" ValidationGroup="Assessment" />
                    </label>
                    <div>
                        <insite:NumericBox runat="server" ID="QuestionLimit" NumericMode="Integer" MinValue="0" />
                    </div>
                    <div class="form-text">
                        The maximum number of question items allowed on an exam form from this question set.
                    </div>
                </div>

            </div>
        </div>

    </div>

</div>

<div runat="server" id="FilterPanel" class="row">

    <div class="col-lg-12 mb-3 mb-lg-0">
        <div class="card border-0 shadow-lg h-100">
            <div class="card-body">

                <h3 runat="server" id="FilterHeading"></h3>

                <div runat="server" id="TagFilter" class="form-group mb-3">
                    <label class="form-label">
                        Count per Question Tag
                <insite:RequiredValidator runat="server" ControlToValidate="CriterionTagFilter" ValidationGroup="Assessment" />

                    </label>
                    <div>
                        <insite:TextBox runat="server" ID="CriterionTagFilter" TextMode="MultiLine" Rows="4" ValidationGroup="Assessment" />
                    </div>
                    <div class="form-text">
                        This is a list of comma-separated values, where the items in the list follow the pattern <strong>Tag:Count</strong>
                    </div>
                </div>

                <insite:Nav runat="server" ID="PivotFilter">

                    <insite:NavItem runat="server" ID="DimensionsSection" Title="Questions Available">

                        <insite:CustomValidator runat="server" ID="RequirementsPivotTableValidator" ErrorMessage="The pivot table is empty" Display="None" ValidateEmptyText="true" ValidationGroup="Assessment" />
                        <div class="row">
                            <div runat="server" id="DimensionsTable" class="col-md-12 pivottable-insite">
                            </div>
                            <asp:HiddenField runat="server" ID="DimensionsState" />
                        </div>

                        <div class="row">
                            <div class="col-md-12">
                                <div style="margin-top: 10px;">
                                    <insite:NextButton runat="server" ID="BuildFilterButton" />
                                    <asp:HiddenField runat="server" ID="FilterData" ViewStateMode="Disabled" />
                                </div>
                            </div>
                        </div>

                    </insite:NavItem>

                    <insite:NavItem runat="server" ID="RequirementsSection" Title="Questions Required">

                        <div class="row">
                            <div runat="server" id="RequirementsPivotTable" class="col-md-12 col-requirements" style="visibility: hidden;">
                            </div>
                            <asp:HiddenField runat="server" ID="RequirementsChanges" />
                        </div>

                    </insite:NavItem>

                </insite:Nav>

            </div>
        </div>
    </div>

</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var dimensionsData = <%= Shift.Common.JsonHelper.SerializeJsObject(DimensionsData) %>;
            var questionsData = <%= Shift.Common.JsonHelper.SerializeJsObject(QuestionsData) %>;

            if (!dimensionsData || !questionsData)
                return;

            var difficultyMapping = getOptionMapping(dimensionsData.difficulty);
            var taxonomyMapping = getOptionMapping(dimensionsData.taxonomy);
            var competencyMapping = getCompetencyMapping(dimensionsData.competency);

            var isActiveFilterBoxSetup = false;
            var $activeFilterBox = null;

            var $filterConfig = $('#<%= FilterData.ClientID %>');
            var $dimensionsState = $('#<%= DimensionsState.ClientID %>');
            var $dimensionsOutput = (function () {
                var questionsMapping = {};
                var hasDifficulty = false;
                var hasTaxonomy = false;
                var hasCompetency = false;

                for (var i = 0; i < questionsData.length; i++) {
                    var data = questionsData[i];
                    var key = '';

                    if (data.difficulty !== null && difficultyMapping.hasOwnProperty(data.difficulty)) {
                        key += String(data.difficulty) + ';';
                        hasDifficulty = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.taxonomy !== null && taxonomyMapping.hasOwnProperty(data.taxonomy)) {
                        key += String(data.taxonomy) + ';';
                        hasTaxonomy = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.competency !== null && competencyMapping.hasOwnProperty(data.competency)) {
                        key += String(data.competency) + ';';
                        hasCompetency = true;
                    } else {
                        key += 'null;';
                    }

                    questionsMapping[key] = data;
                }

                var options;

                var state = $dimensionsState.val();
                if (!state) {
                    options = { unusedAttrsVertical: true, rows: [], cols: [] };

                    if (hasCompetency)
                        options.rows.push('Competency');

                    if (hasTaxonomy)
                        options.cols.push('Taxonomy');

                    if (hasDifficulty)
                        options.cols.push('Difficulty');
                } else {
                    options = JSON.parse(state);
                }

                options.hiddenAttributes = ['Count'];
                options.aggregators = {
                    'Count': function () {
                        return function (data, rowKey, colKey) {
                            return {
                                count: 0,
                                push: function (record) {
                                    this.count += record.Count;
                                },
                                value: function () {
                                    if (this.count == 0)
                                        return null;
                                    else
                                        return this.count;
                                },
                                format: function (x) {
                                    return x;
                                },
                            };
                        };
                    },
                };

                options.onRefresh = function (config) {
                    var state = jQuery.extend({}, config);

                    delete state['aggregators'];
                    delete state['renderers'];
                    delete state['derivedAttributes'];
                    delete state['hiddenAttributes'];
                    delete state['hiddenFromAggregators'];
                    delete state['hiddenFromDragDrop'];

                    delete state['rendererOptions'];
                    delete state['localeStrings'];

                    var json = JSON.stringify(state);

                    $dimensionsState.val(json);

                    $dimensionsOutput
                        .find('table.pvtUi td.pvtRendererArea > table.pvtTable')
                        .find('tr > th.pvtColLabel,tr > th.pvtRowLabel')
                        .each(function () {
                            var $this = $(this);
                            setEntityTitle($this, true);
                        });

                    $dimensionsOutput
                        .find('table.pvtUi td.pvtAxisContainer.pvtUnused > div.pvtFilterBox > div.pvtCheckContainer > p > label').each(function () {
                            var filter = $(this).find('> input.pvtFilter').data('filter');
                            var filterName = filter[0];
                            var filterValue = filter[1];
                            var filterText = filterValue;
                            var filterCount = 0;

                            if (filterValue && filterValue.length > 2) {
                                var entityId = parseInt(filterValue.substring(2));
                                if (!isNaN(entityId)) {
                                    var valueId = filterValue.substring(0, 2);
                                    if (filterName === 'Difficulty') {
                                        if (valueId === 'a:' && difficultyMapping.hasOwnProperty(entityId)) {
                                            filterText = difficultyMapping[entityId];

                                            for (var i = 0; i < questionsData.length; i++) {
                                                var data = questionsData[i];
                                                if (data.difficulty === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Taxonomy') {
                                        if (valueId === 'b:' && taxonomyMapping.hasOwnProperty(entityId)) {
                                            filterText = taxonomyMapping[entityId];

                                            for (var i = 0; i < questionsData.length; i++) {
                                                var data = questionsData[i];
                                                if (data.taxonomy === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Competency') {
                                        if (valueId === 'c:' && competencyMapping.hasOwnProperty(entityId)) {
                                            filterText = String(competencyMapping[entityId].text);

                                            for (var i = 0; i < questionsData.length; i++) {
                                                var data = questionsData[i];
                                                if (data.competency === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    }
                                }
                            }

                            $(this).find('> span.value').text(filterText);
                            $(this).find('> span.count').text('(' + String(filterCount) + ')');
                        });
                };
                options.sorters = {
                    Competency: function (a, b) {
                        var valueA = a;
                        var valueB = b;

                        if (typeof valueA === 'string') {
                            if (a && a.length > 2 && a.substring(0, 2) === 'c:') {
                                var num = a.substring(2);
                                if (competencyMapping.hasOwnProperty(num))
                                    valueA = competencyMapping[num].text;
                            }

                            valueA = valueA.toUpperCase();
                        }

                        if (typeof valueB === 'string') {
                            if (b && b.length > 2 && b.substring(0, 2) === 'c:') {
                                var num = b.substring(2);
                                if (competencyMapping.hasOwnProperty(num))
                                    valueB = competencyMapping[num].text;
                            }

                            valueB = valueB.toUpperCase();
                        }

                        if (valueA < valueB)
                            return -1;
                        else if (valueA > valueB)
                            return 1;
                        else
                            return 0;
                    }
                };

                return $('#<%= DimensionsTable.ClientID %>').pivotUI(inputFunc, options);

                function inputFunc(callback) {
                    if (!hasDifficulty && !hasTaxonomy && !hasCompetency)
                        return;

                    var dimensions = [];

                    if (hasDifficulty) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.difficulty.length; i++)
                            objArr.push({ Difficulty: dimensionsData.difficulty[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasTaxonomy) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.taxonomy.length; i++)
                            objArr.push({ Taxonomy: dimensionsData.taxonomy[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasCompetency) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.competency.length; i++)
                            objArr.push({ Competency: dimensionsData.competency[i].num });
                        dimensions.push(objArr);
                    }

                    var records = [];

                    if (dimensions.length > 0) {
                        var firstDimension = dimensions[0];
                        for (var i = 0; i < firstDimension.length; i++)
                            records.push(firstDimension[i]);

                        for (var x = 1; x < dimensions.length; x++) {
                            var recs = [];
                            var dim = dimensions[x];

                            for (var y = 0; y < dim.length; y++) {
                                for (var z = 0; z < records.length; z++) {
                                    recs.push($.extend({}, records[z], dim[y]));
                                }
                            }

                            records = recs;
                        }
                    }

                    for (var i = 0; i < records.length; i++) {
                        var record = records[i];
                        record.Count = 0;

                        var key = '';

                        if (hasDifficulty && record.Difficulty) {
                            key += String(record.Difficulty) + ';';
                            record.Difficulty = 'a:' + record.Difficulty;
                        } else {
                            key += 'null;';
                        }

                        if (hasTaxonomy && record.Taxonomy) {
                            key += String(record.Taxonomy) + ';';
                            record.Taxonomy = 'b:' + record.Taxonomy;
                        } else {
                            key += 'null;';
                        }

                        if (hasCompetency && record.Competency) {
                            key += String(record.Competency) + ';';
                            record.Competency = 'c:' + record.Competency;
                        } else {
                            key += 'null;';
                        }

                        if (questionsMapping.hasOwnProperty(key)) {
                            record.Count = questionsMapping[key].count;
                        }

                        callback(record);
                    }
                }

                function setEntityTitle($el, html) {
                    var elText = $el.text();
                    if (!elText || elText.length < 3)
                        return;

                    var entityId = parseInt(elText.substring(2));
                    if (isNaN(entityId))
                        return;

                    var result = '';

                    var valueId = elText.substring(0, 2);
                    if (valueId === 'a:') {
                        if (difficultyMapping.hasOwnProperty(entityId))
                            result = difficultyMapping[entityId];
                    } else if (valueId === 'b:') {
                        if (taxonomyMapping.hasOwnProperty(entityId))
                            result = taxonomyMapping[entityId];
                    } else if (valueId === 'c:') {
                        if (competencyMapping.hasOwnProperty(entityId)) {
                            var entity = competencyMapping[entityId];

                            result = String(entity.text);
                        }
                    } else {
                        return;
                    }

                    $el.html(result);
                }
            })();

            var $pvtRenderer = $dimensionsOutput.find('select.pvtRenderer');
            var $pvtAgreagator = $dimensionsOutput.find('select.pvtAggregator');
            var $pvtRowOrder = $dimensionsOutput.find('a.pvtRowOrder');
            var $pvtColOrder = $dimensionsOutput.find('a.pvtColOrder').css('float', 'left');
            var $nextButton = $('#<%= BuildFilterButton.ClientID %>').on('click', onBuildFilterClick);

            $dimensionsOutput.on('pvt.filterbox.shown', onFilterBoxShown);
            $(document).on('click', onDocumentClick);

            { // init
                $('<div class="text-center">Dimensions</div>').insertBefore($pvtRenderer);
                $pvtRenderer.detach().insertAfter($pvtAgreagator).addClass('insite-combobox');
                inSite.common.comboBox.init({
                    id: $pvtRenderer,
                    width: '130px',
                    style: 'btn-combobox btn-sm'
                });
                $pvtAgreagator.hide();

                $dimensionsOutput.find('.pvtFilterBox button').addClass('btn btn-sm btn-primary');
                $dimensionsOutput.find('.pvtFilterBox input[type="text"].pvtSearch').addClass('insite-text form-control');
                $dimensionsOutput.css('visibility', '');
            }

            // event handlers

            function onFilterBoxShown(e) {
                closeActiveFilterBox();

                $activeFilterBox = $(e.target);

                isActiveFilterBoxSetup = true;
                setTimeout(function () {
                    isActiveFilterBoxSetup = false;
                }, 0);
            }

            function onDocumentClick(e) {
                if (isActiveFilterBoxSetup === true || $activeFilterBox === null)
                    return;

                if (!$activeFilterBox.is(':visible')) {
                    $activeFilterBox = null;
                    return;
                }

                if ($activeFilterBox.is(e.target) || $.contains($activeFilterBox[0], e.target))
                    return;

                closeActiveFilterBox();
            }

            function onBuildFilterClick(e) {
                var data = $dimensionsOutput.data('pivotUIOptions');
                var config = {
                    row: {
                        fields: data.rows,
                        orderBy: getOrderBy(data.rowOrder),
                    },
                    col: {
                        fields: data.cols,
                        orderBy: getOrderBy(data.colOrder),
                    },
                    exclusions: []
                };

                for (var prop in data.exclusions) {
                    if (!data.exclusions.hasOwnProperty(prop))
                        continue;

                    var exclusion = data.exclusions[prop];

                    var item = { name: prop, values: [] };
                    for (var i = 0; i < exclusion.length; i++)
                        item.values.push(exclusion[i].substring(2));

                    config.exclusions.push(item);
                }

                var json = JSON.stringify(config);

                $filterConfig.val(json);

                function getOrderBy(value) {
                    var index = value.indexOf('_');
                    var result = {
                        field: value.substring(0, index),
                        order: value.substring(index + 1),
                    }

                    if (result.order === 'a_to_z')
                        result.order = 'asc';
                    else if (result.order === 'z_to_a')
                        result.order = 'desc';

                    return result;
                }
            }

            // methods

            var basePivot = $.fn.pivot;
            $.fn.pivot = function () {
                $pvtRowOrder.detach();
                $pvtColOrder.detach();

                basePivot.apply(this, arguments);

                lastPivotArgs = arguments;

                var $pvtTable = $dimensionsOutput.find('table.pvtTable').addClass('table table-bordered');

                $pvtRowOrder.appendTo($pvtTable.find('th.pvtRowTotalLabel'));
                $pvtColOrder.prependTo($pvtTable.find('th.pvtColTotalLabel'));
            };

            function closeActiveFilterBox() {
                if ($activeFilterBox === null)
                    return;

                $activeFilterBox.find('.btn-cancel').click();
                $activeFilterBox = null;
            }

            function getOptionMapping(array) {
                var result = {};
                for (var i = 0; i < array.length; i++) {
                    var item = array[i];
                    result[item.value] = item.text;
                }
                return result;
            }

            function getCompetencyMapping(array) {
                var result = {};
                for (var i = 0; i < array.length; i++) {
                    var item = array[i];
                    result[item.num] = item;
                }
                return result;
            }
        })();

        (function () {
            var $changedCells = [];
            var $container = $('#<%= RequirementsPivotTable.ClientID %>');
            var $filterChanges = $('#<%= RequirementsChanges.ClientID %>');

            $container.find('table.table-pvt').addClass('table table-bordered').each(function () {
                var $table = $(this);
                var colMaxs = null;

                $table.find('> tbody > tr').each(function () {
                    var $row = $(this);
                    var rowTotalMax = 0;
                    var isInit = !colMaxs;
                    var cellIndex = 0;

                    if (isInit)
                        colMaxs = [];

                    $row.find('> td.pvt-value').each(function () {
                        var $cell = $(this);
                        var data = JSON.parse($cell.html())

                        $cell.prop('contenteditable', true)
                            .data('max', data.max)
                            .data('value', data.value)
                            .data('row', data.row)
                            .data('col', data.col)
                            .data('take', getValue(data.value.toFixed(0)));

                        updateCell($cell);

                        rowTotalMax += data.max;
                        if (isInit)
                            colMaxs.push(data.max);
                        else
                            colMaxs[cellIndex++] += data.max;
                    }).on('focus', onValueFocus).on('blur', onValueBlur);

                    $row.find('> td.pvt-col-total-value').each(function () {
                        var $cell = $(this);
                        $cell.data('max', rowTotalMax);
                        $cell.data('take', $cell.text());

                        updateCell($cell);
                    });
                });

                if (!colMaxs)
                    return;

                $table.find('> tbody > tr:last').each(function () {
                    var $row = $(this);

                    var grandMax = 0;
                    var cellIndex = 0;

                    $row.find('> td.pvt-row-total-value').each(function () {
                        var colMax = colMaxs[cellIndex++];

                        var $cell = $(this);
                        $cell.data('max', colMax);
                        $cell.data('take', $cell.text());

                        grandMax += colMax;

                        updateCell($cell);
                    });

                    $row.find('> td.pvt-grand-total-value').each(function () {
                        var $cell = $(this);
                        $cell.data('max', grandMax);
                        $cell.data('take', $cell.text());

                        updateCell($cell);
                    });
                });
            });

            $container.css('visibility', 'visible');

            // event handlers

            function onValueFocus() {
                var $this = $(this);

                var take = getValue($this.data('take'));
                if (take == null)
                    take = 0;

                $this.removeClass('text-success text-danger').html(take.toFixed(0));

                setTimeout(function (el) {
                    if (window.getSelection && document.createRange) {
                        var range = document.createRange();
                        range.selectNodeContents(el);

                        var sel = window.getSelection();
                        sel.removeAllRanges();
                        sel.addRange(range);
                    } else if (document.body.createTextRange) {
                        var range = document.body.createTextRange();
                        range.moveToElementText(el);
                        range.select();
                    }
                }, 1, this);
            }

            function onValueBlur() {
                var $this = $(this);

                var take = getValue($this.text());
                $this.data('take', take);

                updateCell($this);
                updateTable();
            }

            // methods

            function updateTable() {
                var colTotals = null;

                $container.find('table.table-pvt > tbody > tr').each(function () {
                    var $row = $(this);
                    var rowTotalTake = 0;
                    var isInit = !colTotals;
                    var cellIndex = 0;

                    if (isInit)
                        colTotals = [];

                    $row.find('> td.pvt-value').each(function () {
                        var $cell = $(this);
                        var take = getValue($cell.data('take'));
                        if (!take)
                            take = 0;

                        rowTotalTake += take;
                        if (isInit)
                            colTotals.push(take);
                        else
                            colTotals[cellIndex++] += take;
                    });

                    $row.find('> td.pvt-col-total-value').each(function () {
                        var $this = $(this).data('take', rowTotalTake);
                        updateCell($this);
                    });
                });

                if (!colTotals)
                    return;

                $container.find('table.table-pvt > tbody > tr:last').each(function () {
                    var $row = $(this);

                    var totalGrand = 0;
                    var cellIndex = 0;

                    $row.find('> td.pvt-row-total-value').each(function () {
                        var colTotalTake = colTotals[cellIndex++];
                        var $cell = $(this).data('take', colTotalTake);
                        totalGrand += colTotalTake;

                        updateCell($cell);
                    });

                    $row.find('> td.pvt-grand-total-value').each(function () {
                        var $cell = $(this).data('take', totalGrand);

                        updateCell($cell);
                    });
                });
            }

            function getValue(input) {
                var value = parseFloat(input);
                if (!isNaN(value) && value >= 0)
                    return Math.round(value);
                else
                    return null;
            }

            function updateCell($cell) {
                var max = getValue($cell.data('max'));
                var take = getValue($cell.data('take'));

                if (max == null)
                    max = 0;

                if (take == null)
                    take = 0;

                $cell.removeClass('text-success text-danger');

                if (take == 0) {
                    $cell.html('');
                } else if (take == max) {
                    $cell.html(take.toFixed(0));
                } else {
                    $cell.html(String(take.toFixed(0)) + ' of ' + String(max.toFixed(0)));

                    if (take <= max)
                        $cell.addClass('text-success');
                    else
                        $cell.addClass('text-danger');
                }

                if ($cell.hasClass('pvt-value')) {
                    var value = getValue($cell.data('value'));

                    if (take !== value) {
                        var isFound = false;

                        for (var i = 0; i < $changedCells.length; i++) {
                            if ($cell.is($changedCells[i])) {
                                isFound = true;
                                break;
                            }
                        }

                        if (!isFound)
                            $changedCells.push($cell);
                    } else {
                        for (var i = 0; i < $changedCells.length; i++) {
                            if ($cell.is($changedCells[i])) {
                                $changedCells.splice(i, 1);
                                break;
                            }
                        }
                    }

                    updateFilterChanges();
                }
            }

            function updateFilterChanges() {
                var data = [];

                for (var i = 0; i < $changedCells.length; i++) {
                    var $cell = $changedCells[i];

                    var take = getValue($cell.data('take'));
                    if (take == null)
                        take = 0;

                    data.push({
                        row: $cell.data('row'),
                        col: $cell.data('col'),
                        value: take
                    });
                }

                var json = JSON.stringify(data);

                $filterChanges.val(json);
            }
        })();

    </script>
</insite:PageFooterContent>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .bootstrap-select.btn-group .dropdown-menu li a {
            color: #545454 !important;
            text-decoration: none;
        }

        .col-requirements {
            visibility: hidden;
        }

            .col-requirements table.table-pvt {
                text-align: left;
                margin-bottom: 0;
                width: inherit;
                min-width: 50%;
                max-width: 100%;
            }

                .col-requirements table.table-pvt > :not(:last-child) > :last-child > * {
                    border-bottom-color: inherit;
                }

                .col-requirements table.table-pvt thead tr th,
                .col-requirements table.table-pvt tbody tr th {
                    background-color: #fbfbfb;
                    padding: 5px;
                }

                    .col-requirements table.table-pvt thead tr th.pvt-col {
                        text-align: center;
                    }

                .col-requirements table.table-pvt tbody tr td.pvt-value {
                    padding: 5px;
                    vertical-align: top;
                    text-align: center;
                }

                    .col-requirements table.table-pvt tbody tr td.pvt-value:focus {
                        outline: 1px dotted #000000;
                    }

                .col-requirements table.table-pvt thead tr th.pvt-col-total,
                .col-requirements table.table-pvt tbody tr th.pvt-row-total {
                    text-align: right;
                }

                .col-requirements table.table-pvt tbody tr td.pvt-col-total-value,
                .col-requirements table.table-pvt tbody tr td.pvt-row-total-value,
                .col-requirements table.table-pvt tbody tr td.pvt-grand-total-value {
                    padding: 5px;
                    vertical-align: top;
                    font-weight: bold;
                }

                .col-requirements table.table-pvt tbody tr td.pvt-row-total-value {
                    text-align: center;
                }

                .col-requirements table.table-pvt tbody tr td.pvt-col-total-value,
                .col-requirements table.table-pvt tbody tr td.pvt-grand-total-value {
                    text-align: right;
                }

                .col-requirements table.table-pvt > thead > tr > th,
                .col-requirements table.table-pvt > thead > tr > td {
                    border-bottom-width: 1px !important;
                }
    </style>
</insite:PageHeadContent>
