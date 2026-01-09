<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DepartmentCriteriaSelector.ascx.cs" Inherits="InSite.Cmds.Controls.Reporting.Report.DepartmentCriteriaSelector" %>

<%@ Register Src="MultipleDepartmentSelector.ascx" TagName="MultipleDepartmentSelector" TagPrefix="uc" %>

<div runat="server" class="departments" style="width:350px; position:relative;">
    <a href="#filter-list" class="btn-show-filter" 
        data-action="filter-list" data-title="Departments" data-target="#<%= Departments.ClientID + "_Table" %>" data-selectionmax="<%= SelectionLimit %>"
        data-afterupdate="report.afterDepartmentUpdate" 
        data-beforeshow="report.initDepartmentInputs" 
        data-afterfilter="report.afterDepartmentFilter"
        data-afterrefresh="report.afterDepartmentRefresh"><i class="fas fa-filter"></i></a>

    <ul id="departments-output" class="output-list">
    </ul>
</div>

<div runat="server" style="display:none;">
    <uc:MultipleDepartmentSelector ID="Departments" runat="server" />
</div>

<asp:Button runat="server" ID="RefreshButton" Style="display: none;" />

<insite:PageFooterContent runat="server">
    <script type="text/javascript">

        (function () {
            var instance = window.report = window.report || {};
            var isDepartmentChangeEvent = false;
            var groups = null;

            if (typeof instance.initDepartmentInputs == 'function') {
                alert('ERROR: Only one instance of DepartmentCriteriaSelector.ascx is allowed!');
                return;
            }

            instance.initDepartmentInputs = function (data) {
                var groupItems = null;
                var groupStartItem = null;

                groups = [];

                for (var i = 0; i < data.listItems.length; i++) {

                    var listItem = data.listItems[i];

                    if (listItem.$originalInput.parent().attr('index')) {
                        if (groupStartItem !== null)
                            groupItems.push(listItem);
                    } else {
                        if (groupStartItem !== null)
                            createDepartmentGroup(groupStartItem, groupItems);

                        groupStartItem = listItem;
                        groupItems = [];
                    }
                }

                if (groupStartItem !== null)
                    createDepartmentGroup(groupStartItem, groupItems);
            };

            instance.afterDepartmentFilter = function (data) {
                if (groups === null || groups.length == 0)
                    return;

                for (var i = 0; i < groups.length; i++) {
                    var groupItem = groups[i];
                    var hasVisibleItems = groupItem.$item.find('> ul.input-list > li:not(.d-none)').length > 0;

                    if (!groupItem.visible) {
                        if (hasVisibleItems) {
                            groupItem.$item.removeClass('d-none');
                            groupItem.visible = true;
                        }
                    } else {
                        if (!hasVisibleItems) {
                            groupItem.$item.addClass('d-none');
                            groupItem.visible = false;
                        }
                    }

                    if (groupItem.visible) {
                        var allChecked = groupItem.$item.find('> ul.input-list > li:not(.d-none) > input[type="checkbox"]:not(:checked)').length == 0;
                        groupItem.$input.prop('checked', allChecked);
                    }
                }
            }

            instance.afterDepartmentRefresh = function () {
                renderDepartmentOutput();
            }

            instance.afterDepartmentUpdate = function () {
                renderDepartmentOutput();
                MultipleDepartmentSelector_execute_OnClientItemsChanged();
            }

            function renderDepartmentOutput() {
                var $output = $('#departments-output').empty();
                var $inputs = $('#<%= Departments.ClientID + "_Table" %> input[type="checkbox"]');

                var isNoneSelected = true;
                var isAllSelected = true;
                var currentGroup = null;
                var isGroupRendered = false;

                var $listItems = [];

                $inputs.each(function () {
                    var $this = $(this);

                    if ($this.parent().attr('index')) {
                        if (!$this.prop('checked')) {
                            isAllSelected = false;
                            return;
                        }

                        isNoneSelected = false;

                        var $label = $('label[for="' + this.id + '"]');

                        var text = $label.text();
                        if (!text)
                            text = '(Untitled)';

                        if (currentGroup != null) {
                            if (!isGroupRendered) {
                                $listItems.push($('<li>').text(currentGroup));
                                isGroupRendered = true;
                            }

                            $listItems.push($('<li style="padding-left:30px;">').text(text));
                        } else {
                            $listItems.push($('<li>').text(text));
                        }
                    } else {
                        var $label = $('label[for="' + this.id + '"]');

                        var text = $label.text();
                        if (!text)
                            text = '(Untitled)';

                        currentGroup = text;
                        isGroupRendered = false;
                    }
                });

                if (isNoneSelected)
                    $output.append('<li><%= SelectionLimit > 0 ? "None" : "All Departments" %></li>');
                else if (isAllSelected)
                    $output.append('<li>All Departments</li>');
                else
                    $output.append($listItems);
            }

            function createDepartmentGroup(startItem, groupItems) {
                if (groupItems.length === 0)
                    return;

                var $list = $('<ul class="input-list">');

                startItem.$item.append($list);

                for (var i = 0; i < groupItems.length; i++) {
                    groupItems[i].$item.appendTo($list).on('change', onDepartmentGroupItemClick);
                }

                startItem.$input.on('change', onDepartmentGroupClick);

                groups.push(startItem);
            }

            function onDepartmentGroupClick(e) {
                if (isDepartmentChangeEvent)
                    return;

                isDepartmentChangeEvent = true;

                var $this = $(this);
                $this
                    .closest('li')
                    .find('> ul.input-list > li:not(.d-none) > input[type="checkbox"]')
                    .prop('checked', $this.prop('checked'));

                $this.trigger('change');

                isDepartmentChangeEvent = false;
            }

            function onDepartmentGroupItemClick(e) {
                if (isDepartmentChangeEvent)
                    return;

                var $this = $(this);
                var $inputList = $this.closest('ul.input-list');

                var isAllSelected = $inputList.find('> li:not(.d-none) > input[type="checkbox"]:not(:checked)').length === 0;

                isDepartmentChangeEvent = true;

                $inputList.closest('li').find('> input[type="checkbox"]').prop('checked', isAllSelected).trigger('change');

                isDepartmentChangeEvent = false;
            }

        })();

    </script>
</insite:PageFooterContent>
