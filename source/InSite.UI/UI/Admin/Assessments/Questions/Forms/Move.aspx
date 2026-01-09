<%@ Page Language="C#" CodeBehind="Move.aspx.cs" Inherits="InSite.Admin.Assessments.Questions.Forms.Move" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Import Namespace="Shift.Common" %>

<%@ Register TagPrefix="uc" TagName="OptionRepeater" Src="../../Options/Controls/OptionReadRepeater.ascx" %>
<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:ValidationSummary runat="server" ValidationGroup="DestinationStep" />
    <insite:ValidationSummary runat="server" ValidationGroup="MappingStep" />

    <insite:Container runat="server" ID="SuccessMessage" Visible="false">
        <div class="row">
            <div class="col-md-6">
                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body">
                        <insite:Alert runat="server" ID="SuccessStatus" />
                    </div>
                </div>
            </div>
        </div>

        <div class="row mt-3">
            <div class="col-lg-12">
                <insite:CloseButton runat="server" ID="SuccessCloseButton" />
            </div>
        </div>
    </insite:Container>
    

    <insite:Container runat="server" ID="MoveContainer" Visible="true">

        <section runat="server" id="DestinationSection" class="mb-3">
            <h2 class="h4 mb-3">
                <i class="far fa-question"></i>
                Destination
            </h2>

            <div class="row">

                <div class="col-lg-12">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="row">

                                <div class="col-md-5">

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

                                <div class="col-md-7">
                                    <div runat="server" id="BanksField" class="form-group mb-3">
                                        <label class="form-label">
                                            Banks
                                        <insite:CustomValidator runat="server" ID="DestinationBanksValidator" ErrorMessage="Required field: Banks" ClientValidationFunction="questionMove.onDestinationBanksValidation" ValidationGroup="DestinationStep" />
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
                                        <asp:Panel runat="server" ID="DestinationNoBanksMessage" CssClass="alert alert-info" Style="margin-bottom: 0;" Visible="false">
                                            There are no banks assigned to the selected trade.
                                        </asp:Panel>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </section>

        <section runat="server" id="MappingSection" class="mb-3">
            <h2 class="h4 mb-3">
                <i class="far fa-sitemap"></i>
                Mapping
            </h2>

            <div class="row">

                <div class="col-lg-12">

                    <div class="card border-0 shadow-lg h-100">
                        <div class="card-body">

                            <div class="row">

                                <h3>Source</h3>

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

                                <div runat="server" id="MappingSpecificationField" class="form-group mb-3">
                                    <label class="form-label">Specification</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingSpecificationName" />
                                    </div>
                                </div>

                                <div runat="server" id="MappingSetField" class="form-group mb-3">
                                    <label class="form-label">Set</label>
                                    <div>
                                        <asp:Literal runat="server" ID="MappingSetName" />
                                    </div>
                                </div>

                                <div runat="server" id="MappingCompetencyField" class="form-group mb-3">
                                    <label class="form-label">Competency</label>
                                    <div>
                                        <assessments:AssetTitleDisplay runat="server" ID="MappingCompetency" />
                                    </div>
                                </div>

                            </div>

                            <div runat="server" id="MappingFilterColumn" class="row">

                                <h3>Filter</h3>

                                <asp:Repeater runat="server" ID="MappingFilterRepeater">
                                    <ItemTemplate>
                                        <div class="form-group mb-3">
                                            <label class="form-label"><%# Eval("Name") %></label>
                                            <div><%# Eval("Value") %></div>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>

                            </div>

                            <div class="row">
                                <h3>Source Bank Questions
                                    <insite:CustomValidator runat="server" ID="MappingSelectionValidator"
                                        ErrorMessage="To continue you must setup destination at least for one question listed in the 'Source Bank Questions' table"
                                        ClientValidationFunction="questionMove.onMappingValidation" ValidationGroup="MappingStep" />
                                </h3>

                                <asp:Repeater runat="server" ID="MappingQuestionRepeater">
                                    <HeaderTemplate>
                                        <div class="row d-none d-md-flex mapping-q-h">
                                            <div class="col-md-6">
                                                Question
                                            </div>
                                            <div class="col-md-2">
                                                Bank
                                            </div>
                                            <div class="col-md-2">
                                                GAC
                                            </div>
                                            <div class="col-md-2">
                                                Competency
                                            </div>
                                        </div>

                                        <div id='<%# MappingQuestionRepeater.ClientID %>'>
                                    </HeaderTemplate>
                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                    <ItemTemplate>
                                        <div class="row mapping-q-vr">
                                            <div class="col-md-6">
                                                <asp:Literal runat="server" ID="QuestionId" Text='<%# Eval("Question.Identifier") %>' Visible="false" />
                                                <div>
                                                    <strong>
                                                        Question #<%# (int)Eval("Question.BankIndex") + 1 %>
                                                        (<%# string.Format("{0}.{1}", Eval("Question.Asset"), Eval("Question.AssetVersion")) %>)
                                                    </strong>:
                                                </div>
                                                <div style='margin: 10px 0 15px;'>
                                                    <%# Shift.Common.Markdown.ToHtml(Eval("Question.Content.Title") == null ? null : (string)Eval("Question.Content.Title.Default")) %>
                                                </div>
                                                <div style="margin-top: 10px;">
                                                    <uc:OptionRepeater runat="server" ID="OptionRepeater" />
                                                </div>
                                            </div>
                                            <insite:Container runat="server" Visible='<%# (bool)Eval("AllowMove") == true %>'>
                                                <div class="col-md-2 mt-3 mb-2 m-md-0">
                                                    <div class="d-md-none mb-1"><strong>Bank</strong></div>
                                                    <asp:HiddenField runat="server" ID="DestinationBankIdentifier" />
                                                </div>
                                                <div class="col-md-2 mb-2 mb-md-0">
                                                    <div class="d-md-none mb-1"><strong>Set</strong></div>
                                                    <asp:HiddenField runat="server" ID="DestinationSetIdentifier" />
                                                </div>
                                                <div class="col-md-2 mb-2 mb-md-0">
                                                    <div class="d-md-none mb-1"><strong>Competency</strong></div>
                                                    <asp:HiddenField runat="server" ID="DestinationCompetencyIdentifier" />
                                                </div>
                                            </insite:Container>
                                            <insite:Container runat="server" Visible='<%# (bool)Eval("AllowMove") == false %>'>
                                                <div class="col-md-2"></div>
                                                <div class="col-md-2"></div>
                                                <div class="col-md-2"></div>
                                            </insite:Container>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div runat="server" id="MappingNoQuestionMessage" class="row" visible="false">
                                <div class="col-md-12">
                                    <div class="alert alert-info mb-0">
                                        There are no questions matching your search criteria.
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>

            </div>

        </section>

        <div class="row">
            <div class="col-lg-12">
                <insite:NextButton runat="server" ID="NextButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="DestinationStep" />
                <insite:SaveButton runat="server" ID="SaveButton" DisableAfterClick="true" CausesValidation="true" ValidationGroup="MappingStep" Visible="false" />
                <insite:CancelButton runat="server" ID="CancelButton" />
            </div>
        </div>

    </insite:Container>


    <insite:PageHeadContent runat="server">
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

        .bootstrap-select.btn-group {

        }

            .bootstrap-select.btn-group > .btn {
                font-size: 16px;
                padding-left: 5px;
                padding-top: 5px;
                padding-bottom: 4px;
                cursor: default;
            }

            .bootstrap-select.btn-group .dropdown-menu {
                font-size: 16px;
            }

                .bootstrap-select.btn-group .dropdown-menu li a {
                    color: #545454 !important;
                    text-decoration: none !important;
                    padding: 2px 12px;
                    min-height: 24px;
                    cursor: default;
                }

        .row.mapping-q-h {
            font-weight: bold;
            padding: 5px 0;
            margin-left: 0;
            margin-right: 0;
            border-bottom: 2px solid #ddd;
        }

            .row.mapping-q-h > div {
                padding-left: 5px;
                padding-right: 5px;
            }

        .row.mapping-q-vr {
            padding: 5px 0 10px;
            margin-left: 0;
            margin-right: 0;
            border-bottom: 1px solid #ddd;
            margin-bottom: 10px;
        }

            .row.mapping-q-vr > div {
                padding-left: 5px;
                padding-right: 5px;
            }


        .filter-option-inner-inner {
            min-height:1.42em;
        }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            var instance = window.questionMove = window.questionMove || {};

            var mappingBanks = <%= JsonHelper.SerializeJsObject(DestinationSelectorItems) %>;
            var isInitialization = false;

            try {
                isInitialization = true;

                if (mappingBanks !== null) {
                    $('div#<%= MappingQuestionRepeater.ClientID %> > div').each(function () {
                        var $cells = $(this).find('> div');
                        if ($cells.length !== 4)
                            return;

                        var $bankInput = $cells.eq(1).find('> input[type="hidden"]');
                        var $setInput = $cells.eq(2).find('> input[type="hidden"]');
                        var $competencyInput = $cells.eq(3).find('> input[type="hidden"]');
                        
                        if ($bankInput.length !== 1 || $setInput.length !== 1 || $competencyInput.length !== 1)
                            return;
                        
                        var $bankSelect = $('<select class="insite-combobox form-select">').on('change', onBankChanged).insertBefore($bankInput);
                        var $setSelect = $('<select class="insite-combobox form-select">').on('change', onSetChanged).insertBefore($setInput);
                        var $competencySelect = $('<select class="insite-combobox form-select">').on('change', onCompetencyChanged).insertBefore($competencyInput);

                        $bankSelect
                            .data('cdat', {
                                data: mappingBanks,
                                $input: $bankInput,
                                $setSelect: $setSelect,
                                $competencySelect: $competencySelect
                            });

                        inSite.common.comboBox.init({
                            id: $bankSelect,
                            width: '100%'
                        });

                        $setSelect
                            .data('cdat', {
                                data: null,
                                $input: $setInput,
                                $competencySelect: $competencySelect
                            });

                        inSite.common.comboBox.init({
                            id: $setSelect,
                            width: '100%'
                        });

                        $competencySelect
                            .data('cdat', {
                                data: null,
                                $input: $competencyInput,
                            });

                        inSite.common.comboBox.init({
                            id: $competencySelect,
                            width: '100%'
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
                    $select.append('<option>');

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

            // event handlers

            instance.onDestinationBanksValidation = function (sender, args) {
                args.IsValid = $('ul#<%= DestinationFrameworkRepeater.ClientID %> input[type="checkbox"]:checked').length > 0;
            };

            instance.onMappingValidation = function (sender, args) {
                args.IsValid = false;

                $('div#<%= MappingQuestionRepeater.ClientID %> > div').each(function () {
                    var $cells = $(this).find('> div');
                    if ($cells.length !== 4)
                        return;

                    var $bankInput = $cells.eq(1).find('> input[type="hidden"]');
                    var $setInput = $cells.eq(2).find('> input[type="hidden"]');
                    var $competencyInput = $cells.eq(3).find('> input[type="hidden"]');

                    if ($bankInput.length !== 1 || $setInput.length !== 1 || $competencyInput.length !== 1)
                        return;

                    if (!!$bankInput.val() && !!$setInput.val() && !!$competencyInput.val()) {
                        args.IsValid = true;
                        return false;
                    }
                });
            };

            function onBankChanged() {
                var $select = $(this);
                var index = parseInt($select.val());

                var bankData = $select.data('cdat');
                var setData = bankData.$setSelect.data('cdat');

                if (!isNaN(index)) {
                    var bank = bankData.data[index];
                    setData.data = bank.children;
                    bankData.$input.val(bank.id);
                } else {
                    setData.data = null;
                    bankData.$input.val('');
                }

                if (isInitialization)
                    bindSelector(bankData.$setSelect, setData.$input.val());
                else
                    bindSelector(bankData.$setSelect);

                onSetChanged.call(bankData.$setSelect[0]);
            }

            function onSetChanged() {
                var $select = $(this);
                var index = parseInt($select.val());

                var setData = $select.data('cdat');
                var competencyData = setData.$competencySelect.data('cdat');

                if (!isNaN(index)) {
                    var set = setData.data[index];
                    competencyData.data = set.children;
                    setData.$input.val(set.id);
                } else {
                    competencyData.data = null;
                    setData.$input.val('');
                }

                if (isInitialization)
                    bindSelector(setData.$competencySelect, competencyData.$input.val());
                else
                    bindSelector(setData.$competencySelect);

                onCompetencyChanged.call(setData.$competencySelect[0]);
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
