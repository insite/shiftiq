<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="StatisticsRegistrationsPivotTable.ascx.cs" Inherits="InSite.Admin.Events.Reports.Controls.StatisticsRegistrationsPivotTable" %>

<div class="row">
    <div class="col-lg-12">
        <div runat="server" id="PivotTable" class="pivottable-insite"></div>
        <asp:HiddenField runat="server" ID="PivotState" />

        <div class="mt-3">
            <insite:SaveButton runat="server" ID="PivotSave" Text="Save as Default Settings" />
        </div>
    </div>
</div>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var dimensionsData = <%= Shift.Common.JsonHelper.SerializeJsObject(DimensionsData) %>;
            var tableData = <%= Shift.Common.JsonHelper.SerializeJsObject(TableData) %>;

            if (!dimensionsData || !tableData)
                return;

            var attendanceMapping = getOptionMapping(dimensionsData.attendance);
            var formatMapping = getOptionMapping(dimensionsData.format);
            var examTypeMapping = getOptionMapping(dimensionsData.examType);
            var levelTypeMapping = getOptionMapping(dimensionsData.levelType);
            var venueMapping = getOptionMapping(dimensionsData.venue);

            var isActiveFilterBoxSetup = false;
            var $activeFilterBox = null;

            var $dimensionsState = $('#<%= PivotState.ClientID %>');
            var $dimensionsOutput = (function () {
                var dataItemMapping = {};

                var hasAttendance = false;
                var hasFormat = false;
                var hasExamType = false;
                var hasLevelType = false;
                var hasVenue = false;

                for (var i = 0; i < tableData.length; i++) {
                    var data = tableData[i];
                    var key = '';

                    if (data.attendance !== null && attendanceMapping.hasOwnProperty(data.attendance)) {
                        key += String(data.attendance) + ';';
                        hasAttendance = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.format !== null && formatMapping.hasOwnProperty(data.format)) {
                        key += String(data.format) + ';';
                        hasFormat = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.examType !== null && examTypeMapping.hasOwnProperty(data.examType)) {
                        key += String(data.examType) + ';';
                        hasExamType = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.levelType !== null && levelTypeMapping.hasOwnProperty(data.levelType)) {
                        key += String(data.levelType) + ';';
                        hasLevelType = true;
                    } else {
                        key += 'null;';
                    }

                    if (data.venue !== null && venueMapping.hasOwnProperty(data.venue)) {
                        key += String(data.venue) + ';';
                        hasVenue = true;
                    } else {
                        key += 'null;';
                    }

                    dataItemMapping[key] = data;
                }

                var options;

                var state = $dimensionsState.val();
                if (!state) {
                    options = { unusedAttrsVertical: true, rows: [], cols: [] };

                    //if (hasAttendance)
                    //    options.rows.push('Attendance');

                    //if (hasFormat)
                    //    options.cols.push('Format');

                    //if (hasExamType)
                    //    options.cols.push('Exam Type');

                    //if (hasLevelType)
                    //    options.cols.push('Level Type');

                    //if (hasVenue)
                    //    options.cols.push('Venue');
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

                    var $pvtUiRows = $dimensionsOutput.find('table.pvtUi > tr');
                    if ($pvtUiRows.length == 0)
                        $pvtUiRows = $dimensionsOutput.find('table.pvtUi > tbody > tr');

                    $pvtUiRows
                        .find('> td.pvtRendererArea > table.pvtTable')
                        .find('tr > th.pvtColLabel,tr > th.pvtRowLabel')
                        .each(function () {
                            var $this = $(this);
                            setEntityTitle($this, true);
                        });

                    $pvtUiRows
                        .find('> td.pvtUnused > div.pvtFilterBox > div.pvtCheckContainer > p > label')
                        .each(function () {
                            var filter = $(this).find('> input.pvtFilter').data('filter');
                            var filterName = filter[0];
                            var filterValue = filter[1];
                            var filterText = filterValue;
                            var filterCount = 0;

                            if (filterValue && filterValue.length > 2) {
                                var entityId = parseInt(filterValue.substring(2));
                                if (!isNaN(entityId)) {
                                    var valueId = filterValue.substring(0, 2);
                                    if (filterName === 'Attendance') {
                                        if (valueId === 'a:' && attendanceMapping.hasOwnProperty(entityId)) {
                                            filterText = attendanceMapping[entityId];

                                            for (var i = 0; i < tableData.length; i++) {
                                                var data = tableData[i];
                                                if (data.attendance === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Format') {
                                        if (valueId === 'b:' && formatMapping.hasOwnProperty(entityId)) {
                                            filterText = formatMapping[entityId];

                                            for (var i = 0; i < tableData.length; i++) {
                                                var data = tableData[i];
                                                if (data.format === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Exam Type') {
                                        if (valueId === 'c:' && examTypeMapping.hasOwnProperty(entityId)) {
                                            filterText = examTypeMapping[entityId];

                                            for (var i = 0; i < tableData.length; i++) {
                                                var data = tableData[i];
                                                if (data.examType === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Level Type') {
                                        if (valueId === 'd:' && levelTypeMapping.hasOwnProperty(entityId)) {
                                            filterText = levelTypeMapping[entityId];

                                            for (var i = 0; i < tableData.length; i++) {
                                                var data = tableData[i];
                                                if (data.levelType === entityId)
                                                    filterCount += data.count;
                                            }
                                        }
                                    } else if (filterName === 'Venue') {
                                        if (valueId === 'f:' && venueMapping.hasOwnProperty(entityId)) {
                                            filterText = venueMapping[entityId];

                                            for (var i = 0; i < tableData.length; i++) {
                                                var data = tableData[i];
                                                if (data.venue === entityId)
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

                return $('#<%= PivotTable.ClientID %>').pivotUI(inputFunc, options);

                function inputFunc(callback) {
                    if (!hasAttendance && !hasFormat && !hasExamType && !hasLevelType && !hasVenue)
                        return;

                    var dimensions = [];

                    if (hasAttendance) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.attendance.length; i++)
                            objArr.push({ Attendance: dimensionsData.attendance[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasFormat) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.format.length; i++)
                            objArr.push({ Format: dimensionsData.format[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasExamType) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.examType.length; i++)
                            objArr.push({ 'Exam Type': dimensionsData.examType[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasLevelType) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.levelType.length; i++)
                            objArr.push({ 'Level Type': dimensionsData.levelType[i].value });
                        dimensions.push(objArr);
                    }

                    if (hasVenue) {
                        var objArr = [];
                        for (var i = 0; i < dimensionsData.venue.length; i++)
                            objArr.push({ Venue: dimensionsData.venue[i].value });
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

                        if (hasAttendance && record.Attendance) {
                            key += String(record.Attendance) + ';';
                            record.Attendance = 'a:' + record.Attendance;
                        } else {
                            key += 'null;';
                        }

                        if (hasFormat && record.Format) {
                            key += String(record.Format) + ';';
                            record.Format = 'b:' + record.Format;
                        } else {
                            key += 'null;';
                        }

                        if (hasExamType && record['Exam Type']) {
                            key += String(record['Exam Type']) + ';';
                            record['Exam Type'] = 'c:' + record['Exam Type'];
                        } else {
                            key += 'null;';
                        }

                        if (hasLevelType && record['Level Type']) {
                            key += String(record['Level Type']) + ';';
                            record['Level Type'] = 'd:' + record['Level Type'];
                        } else {
                            key += 'null;';
                        }

                        if (hasVenue && record.Venue) {
                            key += String(record.Venue) + ';';
                            record.Venue = 'f:' + record.Venue;
                        } else {
                            key += 'null;';
                        }

                        if (dataItemMapping.hasOwnProperty(key)) {
                            record.Count = dataItemMapping[key].count;
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
                        if (attendanceMapping.hasOwnProperty(entityId))
                            result = attendanceMapping[entityId];
                    } else if (valueId === 'b:') {
                        if (formatMapping.hasOwnProperty(entityId))
                            result = formatMapping[entityId];
                    } else if (valueId === 'c:') {
                        if (examTypeMapping.hasOwnProperty(entityId))
                            result = examTypeMapping[entityId];
                    } else if (valueId === 'd:') {
                        if (levelTypeMapping.hasOwnProperty(entityId))
                            result = levelTypeMapping[entityId];
                    } else if (valueId === 'f:') {
                        if (venueMapping.hasOwnProperty(entityId))
                            result = venueMapping[entityId];
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

            $dimensionsOutput.on('pvt.filterbox.shown', onFilterBoxShown);
            $(document).on('click', onDocumentClick);

            { // init
                $('<div style="text-align:center; font-size:1.1em;">Dimensions</div>').insertBefore($pvtRenderer);
                $pvtRenderer.detach().insertAfter($pvtAgreagator).selectpicker({ width: '130px' });
                $pvtAgreagator.hide();

                $dimensionsOutput.find('.pvtFilterBox button').addClass('btn btn-primary');
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
        })();
    </script>
</insite:PageFooterContent>
