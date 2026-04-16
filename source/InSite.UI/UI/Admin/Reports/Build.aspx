<%@ Page Language="C#" CodeBehind="Build.aspx.cs" Inherits="InSite.UI.Admin.Reports.Build" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="Controls/BuildDataSet.ascx" TagName="BuildDataSet" TagPrefix="uc" %>
<%@ Register Src="Controls/BuildColumns.ascx" TagName="BuildColumns" TagPrefix="uc" %>
<%@ Register Src="Controls/BuildConditions.ascx" TagName="BuildConditions" TagPrefix="uc" %>
<%@ Register Src="Controls/BuildAggregate.ascx" TagName="BuildAggregate" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">
<insite:PageHeadContent runat="server">
    <style type="text/css">
        .column-selector input {
            margin: revert !important;
        }

        ul.column-list {
            list-style-type: none;
            padding-left: 0;
        }

        ul.tree-view.ui-sortable {
            min-height: 42px;
        }

        .ui-sortable {
        }

        .ui-sortable > li {
            cursor: grab;
        }

        .ui-sortable > li.ui-sortable-placeholder {
            visibility: visible !important;
            outline: 1px dashed #b5b5b5 !important;
        }

        .ui-sortable > li.ui-sortable-placeholder {
            background-image: none !important;
        }

        .result-condition {
            vertical-align: middle;
        }

    </style>
</insite:PageHeadContent>

<section class="mb-2 mb-sm-0 pb-sm-5">

    <insite:UpdatePanel runat="server" UpdateMode="Always">
        <ContentTemplate>
            <insite:Alert runat="server" ID="Status" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary1" ValidationGroup="DataSet" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary5" ValidationGroup="Aggregate" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary6" ValidationGroup="AggregateEdit" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary2" ValidationGroup="Column" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary3" ValidationGroup="Condition" />
            <insite:ValidationSummary runat="server" ID="ValidationSummary4" ValidationGroup="ConditionEdit" />
        </ContentTemplate>
    </insite:UpdatePanel>

    <insite:Accordion runat="server" ID="MainAccordion">
        
        <insite:AccordionPanel ID="DataSetSection" runat="server" Title="Data Set" Icon="far fa-database">

            <div class="row settings">
                <div class="col-md-3">
                    <uc:BuildDataSet runat="server" ID="DataSetSelector" ValidationGroup="DataSet" />
                    <div class="mt-2">
                        <insite:CheckBox runat="server" ID="DataSetStatistic" Text="Calculate summary statistics" />
                    </div>
                </div>
            </div>

            <div class="row mt-3">
                <div class="col-lg-3">
                    <insite:NextButton runat="server" ID="NextButtonDataSet" ValidationGroup="DataSet" />
                </div>
            </div>

        </insite:AccordionPanel>

        <insite:AccordionPanel ID="AggregateSection" runat="server" Title="Summary Statistic" Icon="far fa-function" Visible="false">

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="AggregateUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="AggregateUpdatePanel">
                <Triggers>
                    <asp:PostBackTrigger ControlID="NextButtonAggregate" />
                </Triggers>
                <ContentTemplate>
                    <uc:BuildAggregate runat="server" ID="Aggregates" ValidationGroup="AggregateEdit" />

                    <div class="row">
                        <div class="col-lg-6">
                            <insite:Button runat="server" ID="AddAggregateButton" Icon="far fa-plus-circle" Text="Add Column" ButtonStyle="Success" CausesValidation="false" />
                            <insite:NextButton runat="server" ID="NextButtonAggregate" ValidationGroup="Aggregate" />

                            <insite:Button runat="server" ID="SaveAggregateButton" Icon="fas fa-cloud-upload" Text="Save Column" ButtonStyle="Success" ValidationGroup="AggregateEdit" />
                            <insite:Button runat="server" ID="CancelSaveAggregateButton" Icon="fas fa-times" Text="Close" ButtonStyle="Default" CausesValidation="false" />
                        </div>
                    </div>
                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:AccordionPanel>

        <insite:AccordionPanel ID="ColumnSection" runat="server" Title="Columns" Icon="far fa-columns" Visible="false">

            <insite:CustomValidator runat="server" ID="ColumnRequiredValidator" ErrorMessage="At least one column must be added" Display="None" ValidationGroup="Column" />

            <uc:BuildColumns runat="server" ID="Columns" ValidationGroup="Column" />

            <div class="row mt-3">
                <div class="col-lg-6">
                    <insite:NextButton runat="server" ID="NextButtonColumn" ValidationGroup="Column" />
                </div>
            </div>

        </insite:AccordionPanel>

        <insite:AccordionPanel ID="ConditionSection" runat="server" Title="Conditions" Icon="far fa-filter" Visible="false">

            <insite:UpdateProgress runat="server" AssociatedUpdatePanelID="ConditionUpdatePanel" />

            <insite:UpdatePanel runat="server" ID="ConditionUpdatePanel">
                <Triggers>
                    <asp:PostBackTrigger ControlID="NextButtonCondition" />
                </Triggers>
                <ContentTemplate>
                    
                    <uc:BuildConditions runat="server" ID="Conditions" ValidationGroup="ConditionEdit" />

                    <div class="row">
                        <div class="col-lg-6">
                            <insite:Button runat="server" ID="AddConditionButton" Icon="far fa-plus-circle" Text="Add Condition" ButtonStyle="Success" CausesValidation="false" />
                            <insite:NextButton runat="server" ID="NextButtonCondition" ValidationGroup="Condition" />

                            <insite:Button runat="server" ID="SaveConditionButton" Icon="fas fa-cloud-upload" Text="Save Condition" ButtonStyle="Success" ValidationGroup="ConditionEdit" />
                            <insite:Button runat="server" ID="CancelSaveConditionButton" Icon="fas fa-times" Text="Close" ButtonStyle="Default" CausesValidation="false" />
                        </div>
                    </div>
                </ContentTemplate>
            </insite:UpdatePanel>

        </insite:AccordionPanel>

        <insite:AccordionPanel ID="ResultSection" runat="server" Title="Results" Icon="far fa-database" Visible="false">

            <div class="row settings">

                <asp:Panel runat="server" ID="SqlTextResult">
                    <div class="row mb-2">
                        <div class="col-lg-8">
                            <insite:ComboBox runat="server" ID="ResultCondition" AllowBlank="false" Width="260px" CssClass="result-condition" EmptyMessage="Condition" />
                            <asp:Label runat="server" ID="ResultCount" CssClass="ms-2" />
                        </div>
                        <div class="col-lg-4 text-end">
                            <insite:DropDownButton runat="server" ID="ResultDownloadButton" ButtonStyle="Primary" IconType="Solid" IconName="download" Text="Download" DefaultAction="PostBack">
                                <Items>
                                    <insite:DropDownButtonItem Name="Excel" IconType="Solid" IconName="file-excel" Text="Excel (.xlsx)" />
                                    <insite:DropDownButtonItem Name="Csv" IconType="Solid" IconName="file-csv" Text="Text (.csv)" />
                                </Items>
                            </insite:DropDownButton>
                        </div>
                    </div>

                    <div class="table-responsive">
                        <table class="table table-striped table-hover">
                            <asp:Literal runat="server" ID="TableHtml" />
                        </table>
                    </div>

                    <div class="mt-3">
                        <insite:Pagination runat="server" ID="ResultPagination" />
                    </div>
                </asp:Panel>

            </div>

        </insite:AccordionPanel>

        <insite:AccordionPanel ID="SettingsSection" runat="server" Title="Settings" Icon="far fa-cogs" Visible="false">

            <div class="row settings">
                <div class="col-lg-12">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="SQL" />
                        </label>
                        <insite:TextBox runat="server" ID="SqlText" TextMode="MultiLine" Rows="6" ReadOnly="true" AllowHtml="true" />
                    </div>

                </div>
                <div class="col-lg-12">

                    <div class="form-group mb-3">
                        <label class="form-label">
                            <insite:Literal runat="server" Text="JSON" />
                        </label>
                        <insite:TextBox runat="server" ID="JsonText" TextMode="MultiLine" Rows="6" ReadOnly="true" />
                    </div>

                </div>

            </div>
            
        </insite:AccordionPanel>

    </insite:Accordion>

    <insite:SaveButton runat="server" ID="SaveReportButton" ValidationGroup="DataSet" Visible="false" />

</section>

</asp:Content>
