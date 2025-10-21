<%@ Page Language="C#" CodeBehind="Migrate.aspx.cs" Inherits="InSite.Admin.Assessments.Banks.Forms.Migrate" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="Shift.Common" %>

<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>
<%@ Register Src="../Controls/BankInfo.ascx" TagName="Details" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        .p-t-10 {
            padding-top: 10px !important;
        }
    
        .bank-list > ul {
            padding-left: 22px;
        }
    
        .bank-list > ul > li {
            margin-bottom: 10px !important;
        }
    
        .bank-list > ul > li > table {
            margin: 5px 0 0 5px;
        }
    
        .bank-list > ul > li > table > tbody > tr > td > input {
            position: absolute;
            margin-top: 5px;
        }
    
        .bank-list > ul > li > table > tbody > tr > td > label {
            padding-left: 25px;
        }
    
        .table-group {
            padding-top: 16px !important;
            padding-bottom: 8px !important;
            font-size: 1.1em !important;
        }
    
    </style>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="WriteStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="DestinationStep" />
    <insite:ValidationSummary runat="server" ValidationGroup="MappingStep" />
    <insite:ValidationSummary runat="server" ValidationGroup="PreviewStep" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="DestinationSection" Title="Destination" Icon="far fa-balance-scale" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Destination</h2>

                <div class="row mb-3">
                    <div class="col-lg-6">

                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:Details runat="server" ID="BankDetails" />

                                <div class="form-group mb-3">
                                    <label class="form-label">
                                        Trade
                                        <insite:RequiredValidator runat="server" FieldName="Trade" ControlToValidate="DestinationTradeSelector" Display="Dynamic" ValidationGroup="DestinationStep" />
                                    </label>
                                    <div>
                                        <insite:FindStandard runat="server" ID="DestinationTradeSelector" />
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="col-lg-6">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <div runat="server" id="BanksField" class="form-group mb-3">
                                    <label class="form-label">
                                        Banks
                                        <insite:CustomValidator runat="server" ID="DestinationBanksValidator" ErrorMessage="Required field: Banks" ClientValidationFunction="bankMigrate.onDestinationBanksValidation" ValidationGroup="DestinationStep" />
                                    </label>
                                    <div class="bank-list">
                                        <asp:Repeater runat="server" ID="DestinationFrameworkRepeater">
                                            <HeaderTemplate>
                                                <ul id='<%= DestinationFrameworkRepeater.ClientID %>'>
                                            </HeaderTemplate>
                                            <FooterTemplate></ul></FooterTemplate>
                                            <ItemTemplate>
                                                <li>
                                                    <span><%# Eval("HtmlTitle") %></span>
                                                    <asp:CheckBoxList runat="server" ID="BankList" />
                                                </li>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                    <asp:Panel runat="server" ID="DestinationNoBanksMessage" CssClass="alert alert-info" style="margin-bottom: 0;" Visible="false">
                                        There are no banks assigned to the selected trade.
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <insite:NextButton runat="server" ID="DestinationNextButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="DestinationStep" />
                <insite:CancelButton runat="server" ID="DestinationCancelButton" />
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="MappingSection" Title="Mapping" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Mapping</h2>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">
                        <div class="row">
                            <div class="col-lg-6">

                                <h3>Source Bank</h3>

                                <div class="form-group mb-3">
                                    <label class="form-label">Bank Name</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingBankName" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Bank Title</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingBankTitle" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Standard</label>
                                    <div>
                                        <div class="float-end">
                                            <asp:Literal runat="server" ID="MappingBankStandardCalculationMethod" />
                                        </div>
                                        <assessments:AssetTitleDisplay runat="server" ID="MappingBankStandard" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Set Count</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingBankSetCount" />
                                    </div>
                                </div>

                                <div class="form-group mb-3">
                                    <label class="form-label">Question Count</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingBankQuestionCount" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h3>
                            Source Bank Standards
                            <insite:CustomValidator runat="server" ID="MappingSelectionValidator"
                                ErrorMessage="To continue you must setup destination at least for one source competency listed in the 'Source Bank Standards' table"
                                ClientValidationFunction="bankMigrate.onMapingValidation" ValidationGroup="MappingStep" />
                        </h3>

                        <asp:Repeater runat="server" ID="MappingGacRepeater">
                            <HeaderTemplate>
                                <table id='<%# MappingGacRepeater.ClientID %>' class="table table-condensed">
                                    <thead>
                                        <tr>
                                            <th>Standard</th>
                                            <th class="text-end">Questions</th>
                                            <th>Bank</th>
                                            <th>GAC</th>
                                            <th>Competency</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <th class="table-group" colspan="5"><%# Eval("HtmlTitle") %></th>
                                </tr>
                                <asp:Repeater runat="server" ID="CompetencyRepeater">
                                    <ItemTemplate>
                                        <tr>
                                            <td class="p-t-10"><%# Eval("HtmlTitle") %></td>
                                            <td class="p-t-10 text-end"><%# Eval("Info.QuestionIdentifiers.Length", "{0:n0}") %></td>
                                            <td>
                                                <asp:HiddenField runat="server" ID="DestinationBankIdentifier" />
                                            </td>
                                            <td>
                                                <asp:HiddenField runat="server" ID="DestinationSetIdentifier" />
                                            </td>
                                            <td>
                                                <asp:HiddenField runat="server" ID="DestinationCompetencyIdentifier" />
                                            </td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </ItemTemplate>
                        </asp:Repeater>

                        <asp:Panel runat="server" ID="MappingNoSetMessage" CssClass="alert alert-info" Visible="false">
                            There are no sets assigned to the source bank.
                        </asp:Panel>

                    </div>
                </div>

                <insite:NextButton runat="server" ID="MappingNextButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="MappingStep" />
                <insite:CancelButton runat="server" ID="MappingCancelButton" />
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="PreviewSection" Title="Preview" Icon="far fa-search" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">Preview</h2>

                <div class="card border-0 shadow-lg mb-3">
                    <div class="card-body">

                        <h3>Questions Destination</h3>

                        <asp:Repeater runat="server" ID="PreviewQuestionRepeater">
                            <HeaderTemplate>
                                <table class="table">
                                    <thead>
                                        <tr>
                                            <th class="text-end"></th>
                                            <th>Question</th>
                                            <th>Bank</th>
                                            <th>Set</th>
                                            <th>Competency</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                            </HeaderTemplate>
                            <FooterTemplate>
                                </tbody>
                                </table>
                            </FooterTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td class="text-end"><%# Eval("QuestionSequence") %></td>
                                    <td><span style="white-space: pre-wrap;"><%# Eval("QuestionTitle") %></span></td>
                                    <td><%# Eval("DestinationBankName") %></td>
                                    <td>
                                        <%# Eval("DestinationSetName") %>
                                        <div style="font-size: 13px;"><%# Eval("DestinationGacHtml") %></div>
                                    </td>
                                    <td>
                                        <%# Eval("DestinationCompetencyHtml") %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>

                    </div>
                </div>

                <insite:SaveButton runat="server" ID="PreviewNextButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="PreviewStep" />
                <insite:CancelButton runat="server" ID="PreviewCancelButton" />
            </section>
        </insite:NavItem>

    </insite:Nav>

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">
            (function () {
                var instance = window.bankMigrate = window.bankMigrate || {};
                var mappingBanks = <%= JsonHelper.SerializeJsObject(DestinationSelectorItems) %>;
                var isInitialization = false;

                instance.onDestinationBanksValidation = function (sender, args) {
                    args.IsValid = $('ul#<%= DestinationFrameworkRepeater.ClientID %> input[type="checkbox"]:checked').length > 0;
                };

                instance.onMapingValidation = function (sender, args) {
                    args.IsValid = false;

                    $('table#<%= MappingGacRepeater.ClientID %> > tbody > tr').each(function () {
                        var $cells = $(this).find('> td');
                        if ($cells.length !== 5)
                            return;

                        var $bankInput = $cells.eq(2).find('> input[type="hidden"]');
                        var $gacInput = $cells.eq(3).find('> input[type="hidden"]');
                        var $competencyInput = $cells.eq(4).find('> input[type="hidden"]');

                        if ($bankInput.length !== 1 || $gacInput.length !== 1 || $competencyInput.length !== 1)
                            return;

                        if (!!$bankInput.val() && !!$gacInput.val() && !!$competencyInput.val()) {
                            args.IsValid = true;
                            return false;
                        }
                    });
                };

                try {
                    isInitialization = true;

                    if (mappingBanks !== null) {
                        $('table#<%= MappingGacRepeater.ClientID %> > tbody > tr').each(function () {
                            var $cells = $(this).find('> td');
                            if ($cells.length !== 5)
                                return;

                            var $bankSelect = $('<select class="insite-combobox form-select">').on('change', onBankChanged);
                            var $bankCell = $cells.eq(2).append($bankSelect);
                            var $bankInput = $bankCell.find('> input[type="hidden"]');

                            var $gacSelect = $('<select class="insite-combobox form-select">').on('change', onGacChanged);
                            var $gacCell = $cells.eq(3).append($gacSelect);
                            var $gacInput = $gacCell.find('> input[type="hidden"]');

                            var $competencySelect = $('<select class="insite-combobox form-select">').on('change', onCompetencyChanged);
                            var $competencyCell = $cells.eq(4).append($competencySelect);
                            var $competencyInput = $competencyCell.find('> input[type="hidden"]');

                            $bankSelect
                                .data('cdat', {
                                    data: mappingBanks,
                                    $input: $bankInput,
                                    $gacSelect: $gacSelect,
                                    $competencySelect: $competencySelect
                                });

                            inSite.common.comboBox.init({
                                id: $bankSelect,
                                width: '200px'
                            });

                            $gacSelect
                                .data('cdat', {
                                    data: null,
                                    $input: $gacInput,
                                    $competencySelect: $competencySelect
                                });

                            inSite.common.comboBox.init({
                                id: $gacSelect,
                                width: '200px'
                            });

                            $competencySelect
                                .data('cdat', {
                                    data: null,
                                    $input: $competencyInput,
                                });

                            inSite.common.comboBox.init({
                                id: $competencySelect,
                                width: '200px'
                            });

                            bindSelector($bankSelect, $bankInput.val());
                            onBankChanged.call($bankSelect[0]);
                        });
                    }
                } finally {
                    isInitialization = false;
                }

                function bindSelector($select, value) {
                    $select.find('> option').remove();

                    var data = $select.data('cdat').data;
                    var hasData = !!data;
                    var hasValue = !!value;

                    $select.prop('disabled', !hasData);

                    if (hasData) {
                        $select.append($('<option>'));

                        for (var i = 0; i < data.length; i++) {
                            var item = data[i];
                            var $option = $('<option>')
                                .attr('value', String(i))
                                .data('content', item.html)
                                .text(item.text);

                            if (hasValue && item.id === value || i == 0 && data.length == 1)
                                $option.attr('selected', 'selected');

                            $select.append($option);
                        }
                    }

                    $select.selectpicker('refresh').trigger('change');
                }

                function onBankChanged() {
                    var $select = $(this);
                    var index = parseInt($select.val());

                    var bankData = $select.data('cdat');
                    var gacData = bankData.$gacSelect.data('cdat');

                    if (!isNaN(index)) {
                        var bank = bankData.data[index];
                        gacData.data = bank.children;
                        bankData.$input.val(bank.id);
                    } else {
                        gacData.data = null;
                        bankData.$input.val('');
                    }

                    if (isInitialization)
                        bindSelector(bankData.$gacSelect, gacData.$input.val());
                    else
                        bindSelector(bankData.$gacSelect);
                }

                function onGacChanged() {
                    var $select = $(this);
                    var index = parseInt($select.val());

                    var gacData = $select.data('cdat');
                    var competencyData = gacData.$competencySelect.data('cdat');

                    if (!isNaN(index)) {
                        var gac = gacData.data[index];
                        competencyData.data = gac.children;
                        gacData.$input.val(gac.id);
                    } else {
                        competencyData.data = null;
                        gacData.$input.val('');
                    }

                    if (isInitialization)
                        bindSelector(gacData.$competencySelect, competencyData.$input.val());
                    else
                        bindSelector(gacData.$competencySelect);
                }

                function onCompetencyChanged() {
                    var $select = $(this);
                    var index = parseInt($select.val());

                    var competencyData = $select.data('cdat');

                    if (!isNaN(index)) {
                        var competency = competencyData.data[index];
                        competencyData.$input.val(competency.id);
                    } else {
                        competencyData.$input.val('');
                    }
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
