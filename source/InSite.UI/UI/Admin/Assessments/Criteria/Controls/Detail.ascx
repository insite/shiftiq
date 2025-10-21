<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Detail.ascx.cs" Inherits="InSite.Admin.Assessments.Criteria.Controls.Detail" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div class="row">
    <div class="col-lg-6">

        <h3>Identification</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Criteria
                <insite:IconLink Name="trash-alt" runat="server" ID="DeleteCriterionLink" ToolTip="Delete this criterion from the specification" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="CriterionNumber" />
            </div>
            <div class="form-text">
                This criterion applies to questions from the set indicated on the right.
            </div>
        </div>

        <h3>Configuration</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Set Weight
                <insite:IconLink Name="pencil" runat="server" id="EditSetFilter1" ToolTip="Change Filter" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="SetWeight" />
            </div>
            <div class="form-text">
                The desired weighting for the question set to which this criterion applies, within the overall specification.
                The sum of all question set weights for the criteria in a specification must equal 1 (i.e. 100 percent).
            </div>
        </div>

        <div runat="server" id="QuestionLimitField" class="form-group mb-3">
            <label class="form-label">
                Question Limit
                <insite:IconLink Name="pencil" runat="server" id="EditSetFilter2" ToolTip="Change Filter" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="QuestionLimit" />
            </div>
            <div class="form-text">
                The maximum number of items allowed on an exam form from this set.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Filter Type
                <insite:IconLink Name="pencil" runat="server" id="EditSetFilter3" ToolTip="Change Filter" CssClass="ms-1" />
            </label>
            <div>
                <asp:Literal runat="server" ID="FilterType" />
            </div>
            <div class="form-text">
                The type of filter applied to the question items in this set.
            </div>
        </div>

        <insite:Container runat="server" ID="BasicFilterContainer" Visible="false">
            <h3>Tag Filter</h3>
            <div class="ps-1">
                <span runat="server" id="BasicFilterOutput" style="white-space:pre-wrap;"></span>
            </div>
        </insite:Container>

    </div>
    <div class="col-lg-6">

        <h3>Question Sets</h3>

        <div class="form-group mb-3">
            <label class="form-label">
                Set Names
            </label>
            <div>
                <asp:Repeater runat="server" ID="SetRepeater">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <%# Eval("Name") %>
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="form-text">
                The sets from which questions are selected and filtered for this criterion.
            </div>
        </div>

        <div class="form-group mb-3">
            <label class="form-label">
                Standards
            </label>
            <div>
                <asp:Repeater runat="server" ID="StandardRepeater">
                    <HeaderTemplate><ul></HeaderTemplate>
                    <ItemTemplate>
                        <li>
                            <assessments:AssetTitleDisplay runat="server" ID="Standard" AssetID='<%# Eval("Standard") %>' />
                        </li>
                    </ItemTemplate>
                    <FooterTemplate></ul></FooterTemplate>
                </asp:Repeater>
            </div>
            <div class="form-text">
                The standards evaluated by these question sets.
            </div>
        </div>

        <insite:Container runat="server" ID="SectionsContainer">

            <h3>Sections</h3>

            <asp:Repeater runat="server" ID="SectionsRepeater">
                <HeaderTemplate>
                    <table class="table">
                        <thead>
                            <tr>
                                <th>Title</th>
                                <th>Letter</th>
                                <th>Fields</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <FooterTemplate>
                    </tbody></table>
                </FooterTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%# Eval("FormTitle") %></td>
                        <td><%# Eval("Letter") %></td>
                        <td><%# Eval("FieldsCount") %></td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            
        </insite:Container>

    </div>
</div>

<insite:Container runat="server" ID="AdvancedFilterContainer" Visible="false">
    <h3>Pivot Table Filter</h3>
    <div class="pvt-filter invisible">
        <assessments:CriterionPivotTable runat="server" ID="AdvancedFilterOutput" />
    </div>
    <div class="form-text">
        The number of items from this set that match specific criteria.
    </div>
</insite:Container>

<insite:PageHeadContent runat="server" ID="CommonHeaderLiteral">
    <style type="text/css">

        .pvt-filter {
            overflow: auto;
            max-height: 75vh;
            position: relative;
        }

            .pvt-filter > table.table-pvt {
                margin-bottom: 0;
                min-width: 50%;
                max-width: 100%;
                visibility: hidden;
            }

                .pvt-filter > table.table-pvt > :not(:last-child) > :last-child > * {
                    border-bottom-color: inherit;
                }

                .pvt-filter > table.table-pvt > thead > tr > th,
                .pvt-filter > table.table-pvt > tbody > tr > th {
                    background-color: #fbfbfb;
                    padding: 5px;
                }

                    .pvt-filter > table.table-pvt > thead tr th.pvt-col {
                        text-align: center;
                    }

                    .pvt-filter > table.table-pvt > thead > tr > th.pvt-col-total,
                    .pvt-filter > table.table-pvt > tbody > tr > th.pvt-row-total {
                        text-align: right;
                    }

                .pvt-filter > table.table-pvt > thead > tr > th {
                    white-space: nowrap;
                    position: sticky;
                    z-index: 3;
                }

                    .pvt-filter > table.table-pvt > thead > tr > th.pvt-spandrel,
                    .pvt-filter > table.table-pvt > thead > tr > th.pvt-col-axis,
                    .pvt-filter > table.table-pvt > thead > tr > th.pvt-row-axis {
                        z-index: 4;
                    }

                .pvt-filter > table.table-pvt > tbody > tr > th {
                    position: sticky;
                    z-index: 2;
                }

                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-col-total-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-row-total-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-grand-total-value {
                    padding: 5px;
                    vertical-align: top;
                    white-space: nowrap;
                }

                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-row-total-value{
                    text-align: center;
                }

                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-col-total-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-grand-total-value {
                    text-align: right;
                }

                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-col-total-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-row-total-value,
                .pvt-filter > table.table-pvt > tbody > tr > td.pvt-grand-total-value {
                    font-weight: bold;
                }

                .pvt-filter > table.table-pvt > thead > tr > th,
                .pvt-filter > table.table-pvt > thead > tr > td {
                    border-bottom-width: 1px !important;
                }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ID="CommonFooterLiteral">
    <script type="text/javascript">
        (function () {
            var pvts = null;

            $('.pvt-filter > table.table-pvt')
                .addClass('table table-bordered visible');

            $(document).ready(function () {
                pvts = $('.pvt-filter').toArray();

                onWindowClick();
            });

            $(window).on('click', onWindowClick).on('resize', onWindowResize);

            function onWindowClick() {
                if (pvts == null)
                    return;

                let hasItems = false;

                for (let i = 0; i < pvts.length; i++) {
                    const pvt = pvts[i];
                    if (pvt == null)
                        continue;

                    hasItems = true;

                    const $pvt = $(pvt);
                    if ($pvt.is(':visible')) {
                        updatePvt($pvt);
                        pvts[i] = null;
                    }
                }

                if (!hasItems)
                    $(window).off('click', onWindowClick);
            }

            function onWindowResize() {
                $('.pvt-filter:visible').each(function () {
                    updatePvt($(this));
                })
            }

            function updatePvt($pvt) {
                const $table = $pvt.find('> table.table-pvt');

                $table.find('> * > tr > th').css({
                    left: '',
                    width: '',
                    height: ''
                })

                const scrollTop = $pvt.scrollTop();
                const scrollLeft = $pvt.scrollLeft();

                $table.find('> thead > tr > th').each(function () {
                    const $cell = $(this);

                    setupVertical($cell, scrollTop);

                    if ($cell.hasClass('pvt-spandrel') || $cell.hasClass('pvt-col-axis') || $cell.hasClass('pvt-row-axis'))
                        setupHorizontal($cell, scrollLeft);
                });


                $table.find('> tbody > tr > th').each(function () {
                    setupHorizontal($(this), scrollLeft);
                });

                function setupHorizontal($cell, scrollLeft) {
                    $cell.css({
                        left: $cell.position().left + scrollLeft,
                        width: $cell.width()
                    });
                }

                function setupVertical($cell, scrollTop) {
                    $cell.css({
                        top: $cell.position().top + scrollTop,
                        height: $cell.height()
                    });
                }
            }
        })();
    </script>
</insite:PageFooterContent>