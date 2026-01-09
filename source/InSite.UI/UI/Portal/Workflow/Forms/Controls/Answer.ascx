<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Answer.ascx.cs" Inherits="InSite.UI.Portal.Workflow.Forms.Controls.Answer" %>

<div class="row">
    <div class="col-lg-12">
        <h1 runat="server" id="SurveyFormTitle" class="py-1 my-2 pb-2 mb-4"></h1>
    </div>
</div>

<insite:Alert runat="server" ID="StatusAlert" />
<insite:Alert runat="server" ID="ErrorAlert" />

<div runat="server" id="ValidationOutput" class="mb-4 alert alert-danger" style="display:none;">
    <i class="fas fa-stop-circle"></i> <strong><insite:Literal runat="server" Text="Validation Errors:" /></strong>
    <ul style="margin-top:12px;"></ul>
</div>

<div runat="server" id="PageHeaderPanel" class="mt-3">
    <asp:Literal runat="server" ID="PageHeader" />
</div>

<div class="mb-4">
    <div runat="server" id="IeWarning" class="alert alert-warning" visible="false">
        <table>
            <tr>
                <td style="width: 20px; padding-right: 10px; padding-top: 2px; vertical-align: top;">
                    <i class="fas fa-exclamation-triangle"></i>
                </td>
                <td>
                    <strong><insite:Literal runat="server" Text="Your web browser is Internet Explorer." /></strong>
                    <insite:Literal runat="server" Text="IE Warning" />
                </td>
            </tr>
        </table>
    </div>

    <insite:UpdatePanel runat="server" ClientEvents-OnRequestStart="controlBuilder.onRequestStart" ClientEvents-OnResponseEnd="controlBuilder.onResponseEnd" >
        <ContentTemplate>
            <asp:HiddenField runat="server" ID="TermsData" />

            <asp:Repeater runat="server" ID="AnswerGroupRepeater">
                <ItemTemplate>
                    <div class="mt-5 mb-5" data-question-group='<%# Eval("Question") %>'>
                        <div class="card card-hover bg-secondary shadow">

                            <div runat="server" class="card-header border-bottom-0" visible='<%# Eval("IsQuestionHeaderVisible") %>'>
                                <h2 class="mb-0"><%# Eval("QuestionHeader") %></h2>
                            </div>

                            <div class="card-body bg-white">
                                <asp:Repeater runat="server" ID="AnswerItemRepeater">
                                    <ItemTemplate>

                                        <div class="mb-4" data-question-item='<%# Eval("Question") %>'>
                                            <div class="question-body d-flex">
                                                <div>
                                                    <%# Eval("QuestionBody") %>
                                                </div>
                                                <span runat="server" visible='<%# (bool)Eval("QuestionIsRequired") %>' class="text-danger ms-1 required-asterisk">*</span>
                                            </div>

                                            <div class="row">
                                                <div class='col-md-<%# Eval("AnswerColumnSize") %>'>
                                                    <asp:PlaceHolder runat="server" ID="InputPlaceholder" />
                                                </div>
                                            </div>
                                        </div>

                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </ContentTemplate>
    </insite:UpdatePanel>
</div>

<div class="mb-4" runat="server" id="ActionButtons" visible="false">
    <insite:Button runat="server" ID="LaunchButton" Text="Start" Icon="fas fa-arrow-alt-left" CausesValidation="false" ButtonStyle="Default" />
    <insite:Button runat="server" ID="PreviousButton" Text="Previous" Icon="fas fa-arrow-alt-left" CausesValidation="false" ButtonStyle="Default" />
    <insite:NextButton runat="server" ID="NextButton" />
    <insite:Button runat="server" ID="ConfirmButton" Text="Next" Icon="fas fa-arrow-alt-right" IconPosition="AfterText" ButtonStyle="Primary" />
</div>

<div runat="server" id="PageFooterPanel" class="mb-4">
    <asp:Literal runat="server" ID="PageFooter" />
</div>

<asp:Panel runat="server" ID="DebugPanel" Visible="false">

    <hr class="mt-6" />

    <div class="float-end">
        <insite:Button runat="server" ID="ContinueButton" ButtonStyle="danger" Icon="far fa-rocket-launch" IconPosition="AfterText" Text="Continue" Visible="false" />
    </div>

    <h1 class="text-danger"><i class="far fa-debug me-1"></i> Debug: Answer</h1>

    <ul>
        <li>
            This page shows the questions for a specific page in the form.
        </li>
        <li>
            We need to determine if any Conditions are applicable, and if so then we need to hide the questions that 
            must be "masked" because the respondent has selected "masking" option items on preceding questions.
        </li>
        <li>
            When I click the Previous button or the Next button, this page must save my answers automatically by sending
            the command <code>ChangeResponseAnswer</code>.
        </li>
        <li>
            On the <strong>first</strong> page: If the Launch page is accessible (i.e. the form contains starting instructions, or I can submit more than
            one submission) then an Instructions button is displayed instead of the Previous button.
        </li>
        <li>
            On the <strong>last</strong> page: The Confirm button is displayed instead of the Next button.
        </li>
        <li>
            Any question that displays option items (Likert Table | Dropdown List | Checkbox List | Radio Button List)
            must save my selections automatically. In other words, if I select an item in a list, then this page
            sends the command <code>SelectResponseOption</code> automatically, and if I unselect an item, then this page sends the
            command <code>UnselectResponseOption</code> automatically.
        </li>
    </ul>

</asp:Panel>

<insite:Modal runat="server" ID="TermWindow" Size="Large" Centered="true" MinHeight="50px">
    <ContentTemplate>

    </ContentTemplate>
    <FooterTemplate>
        <button type="button" class="btn btn-default" data-bs-dismiss="modal">
            <i class='fas fa-ban me-2'></i> <insite:Literal runat="server" Text="Close" />
        </button>
    </FooterTemplate>
</insite:Modal>

<insite:PageHeadContent runat="server">

    <style type="text/css">

        .card.shadow.is-invalid { box-shadow: 0 0 0.625rem -0.1875rem rgba(var(--bs-danger-rgb)) !important; }
        a.glossary-term { text-decoration: underline double; }

        .lesson ul li.nav-item { line-height: 1.25rem; }
        .lesson ul li.nav-item::before { display: none; }

        .row-num-options, .row-num-options .form-check-label { font-size: 1rem; }

        .required-asterisk {
              align-self: flex-start;
              position: relative;
              top: -2px;
              font-size: 1.2rem;
            }

    </style>

    <script type="text/javascript">
        (function () {
            var instance = window.controlBuilder = window.controlBuilder || {};

            instance.validateRadioList = function (s, e) {
                $(s).parent().find('input[type="radio"]').each(function () {
                    if ($(this).prop('checked')) {
                        e.IsValid = true;
                        return false;
                    } else {
                        e.IsValid = false;
                    }
                });
            };

            instance.validateRadioTable = function (s, e) {
                $(s).parent().find('table:first > tbody > tr').each(function () {
                    $(this).find('input[type="radio"]').each(function () {
                        if ($(this).prop('checked')) {
                            e.IsValid = true;
                            return false;
                        } else {
                            e.IsValid = false;
                        }
                    });

                    if (!e.IsValid)
                        return;
                });
            };
        })();
    </script>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
<script type="text/javascript">

    (function () {
        var instance = window.answerPage = window.answerPage || {};

        var __originalScrollTop = null;
        var __dummyScrollTop = function () { };

        Sys.Application.add_load(function () {
            $(".file-queue").each(function () {
                var $fileQueue = $(this);
                if ($fileQueue.data('inited') == true)
                    return;

                $fileQueue.find(".file-row").each(function () {
                    var $row = $(this);

                    $row.find("a")
                        .off("click")
                        .on("click", function (e) {
                            e.preventDefault();
                            $row.remove();
                            updateUploadedFiles($fileQueue);
                        });
                });

                $fileQueue.data('inited', true);
            });
        });

        instance.onListRangeMinValidation = function (s, a) {
            const item = s.closest('div[data-question-item]');
            const table = item.querySelector('table[data-list-min]');

            const minValue = parseInt(table.dataset.listMin);
            if (isNaN(minValue) || minValue < 1)
                return;

            const checkboxes = table.querySelectorAll('input[type="checkbox"]');
            if (checkboxes.length === 0)
                return;

            const count = Array.from(checkboxes).filter(x => x.checked).length;

            a.IsValid = count >= minValue;
        };

        instance.onListRangeMaxValidation = function (s, a) {
            const item = s.closest('div[data-question-item]');
            const table = item.querySelector('table[data-list-max]');

            const maxValue = parseInt(table.dataset.listMax);
            if (isNaN(maxValue) || maxValue < 1)
                return;

            const checkboxes = table.querySelectorAll('input[type="checkbox"]');
            if (checkboxes.length === 0)
                return;

            const count = Array.from(checkboxes).filter(x => x.checked).length;

            a.IsValid = count <= maxValue;
        };

        instance.onFileUploaded = function () {
            var $fileQueue = $(this).closest('.file-upload').parent().find(".file-queue");

            var $newRow = $("<div class='file-row'></div>");
            $newRow.data("upload", inSite.common.fileUploadV2.getMetadata(this.id));
            $newRow.append(inSite.common.fileUploadV2.getFileName(this.id));
            $newRow.append("<a href='#' style='margin-left:5px;' title='Delete'><i class='far fa-trash-alt'></i><a/>");

            $fileQueue.append($newRow);

            $newRow.find("a").on("click", function (e) {
                e.preventDefault();
                $newRow.remove();
                updateUploadedFiles($fileQueue);
            });

            updateUploadedFiles($fileQueue);

            inSite.common.fileUploadV2.clearFiles(this.id);
        };

        controlBuilder.onRequestStart = function () {
            __originalScrollTop = window.scrollTo;
            window.scrollTo = __dummyScrollTop;
        };

        controlBuilder.onResponseEnd = function () {
            if (__originalScrollTop) {
                window.scrollTo = __originalScrollTop;
                __originalScrollTop = null;
            }

            answerPageGlossary.refreshLinks();
        };

        function updateUploadedFiles($fileQueue) {
            var files = "";

            $fileQueue.find(".file-row").each(function () {
                if (files.length > 0) {
                    files += "\t";
                }

                const uploadObj = $(this).data("upload");
                const uploadJson = typeof uploadObj === "string" ? uploadObj : JSON.stringify(uploadObj);

                files += uploadJson;
            });

            $fileQueue.find("input[type=hidden]").val(files);
        }
    })();

    (function () {
        const $validationOutput = $('#<%= ValidationOutput.ClientID %>');

        Sys.Application.add_load(setupValidators);

        uiValidation.setCustomSetup(setupValidators);

        function setupValidators() {
            if (typeof Page_Validators != 'object' || !(Page_Validators instanceof Array)) {
                return;
            }

            let topItem = null;
            const invalidItems = [];

            $('div.card').each(function () {
                checkCardValid(this);
            });

            displayInvalidItems();

            function checkCardValid(card) {
                const validators = [];

                for (let i = 0; i < Page_Validators.length; i++) {
                    const validator = Page_Validators[i];
                    if (card.contains(validator)
                        && typeof validator.isvalid === 'boolean'
                        && !validator.isvalid
                    ) {
                        validators.push(validator);
                    }
                }

                if (!validators.length) {
                    card.classList.remove('is-invalid');
                } else {
                    card.classList.add('is-invalid');

                    addCardInvalidItems(card, validators);
                }
            }

            function sanitizeLabel(text) {
                if (!text) {
                    return '';
                }
                return text
                    .replace(/\*/g, '')
                    .replace(/\s+/g, ' ')
                    .trim()
                    .replace(/:+\s*$/, '');
            }

            function truncateLabel(text, max = 50) {
                if (!text) {
                    return '';
                }
                return text.length > max ? text.substring(0, max).trim() + '…' : text;
            }

            function addCardInvalidItems(card, validators) {
                const addedAny = addCardMultiInvalidItems(card, validators);
                if (addedAny) {
                    return;
                }
                const cardMessage = getCardMessage(card);
                if (cardMessage) {
                    const item = {
                        top: card.offsetTop,
                        message: cardMessage
                    };
                    invalidItems.push(item);
                    if (!topItem || topItem.top > card.offsetTop) {
                        topItem = item;
                    }
                }
            }

            function addCardMultiInvalidItems(card, validators) {
                let added = false;
                const lines = card.querySelectorAll("[data-question-item]");
                for (const line of lines) {
                    var vs = validators.filter(v => line.contains(v));
                    if (vs.length === 0) {
                        continue;
                    }

                    const message = getLineMessage(line);
                    if (!message) {
                        continue;
                    }

                    vs.forEach(x => {
                        const error = x.errormessage ?? 'This question is mandatory';
                        const item = {
                            top: line.offsetTop,
                            message: error + ': ' + message
                        };
                        invalidItems.push(item);
                        if (!topItem || topItem.top > line.offsetTop) {
                            topItem = item;
                        }
                    });
                    added = true;
                }
                return added;
            }

            function displayInvalidItems() {
                if (!invalidItems.length) {
                    $validationOutput.hide();
                    return;
                }

                const $list = $validationOutput.find('ul').empty();
                const messages = {};

                for (let i = 0; i < invalidItems.length; i++) {
                    const message = invalidItems[i].message;
                    if (messages.hasOwnProperty(message)) {
                        continue;
                    }

                    $list.append($('<li>').text(message));

                    messages[message] = true;
                }

                $validationOutput.show();

                if (uiValidation.isPageValidation) {
                    $('html, body').scrollTop(0);
                }
            }

            function getLineMessage(line) {
                const body = line.querySelector(".question-body");
                if (!body) {
                    return null;
                }

                const raw = body.innerText || '';
                const clean = truncateLabel(sanitizeLabel(raw), 50);
                if (!clean) {
                    return null;
                }

                return clean;
            }

            function getCardMessage(card) {
                const header = card.querySelector(":scope > .card-header > h2");
                if (!header) {
                    return null;
                }

                let title = header.innerText || '';
                title = title.replace(/\*/g, '');
                title = truncateLabel(sanitizeLabel(title), 50);

                return title ? `This question is mandatory: ${title}` : null;
            }
        }
    })();

    (function () {
        Sys.Application.add_load(function () {
            $("td.likert-scale-col").click(function (e) {
                const input = this.querySelector("input");;
                if (input !== e.target) {
                    e.preventDefault();
                    input.click(e);
                }
            });

            var input = document.querySelectorAll('[data-format]');
            if (input.length === 0) return;

            for (var i = 0; i < input.length; i++) {
                var inputFormat = input[i].dataset.format,
                    blocks = input[i].dataset.blocks,
                    delimiter = input[i].dataset.delimiter;

                blocks = blocks !== undefined ? blocks.split(' ').map(Number) : '';
                delimiter = delimiter !== undefined ? delimiter : ' ';

                switch (inputFormat) {
                    case 'card':
                        var card = new Cleave(input[i], {
                            creditCard: true
                        });
                        break;

                    case 'cvc':
                        var cvc = new Cleave(input[i], {
                            numeral: true,
                            numeralIntegerScale: 3
                        });
                        break;

                    case 'date':
                        var date = new Cleave(input[i], {
                            date: true,
                            datePattern: ['m', 'y']
                        });
                        break;

                    case 'date-long':
                        var dateLong = new Cleave(input[i], {
                            date: true,
                            delimiter: '-',
                            datePattern: ['Y', 'm', 'd']
                        });
                        break;

                    case 'time':
                        var time = new Cleave(input[i], {
                            time: true,
                            datePattern: ['h', 'm']
                        });
                        break;

                    case 'custom':
                        var custom = new Cleave(input[i], {
                            delimiter: delimiter,
                            blocks: blocks
                        });
                        break;

                    default:
                        console.error('Sorry, your format ' + inputFormat + ' is not available. You can add it to the theme object method - inputFormatter in src/js/theme.js or choose one from the list of available formats: card, cvc, date, date-long, time or custom.');
                }
            }
        });
    })();

    (function () {
        var _autoCalcValues = null;

        window.answerPage.setupAutoCalc = function (values) {
            if (_autoCalcValues !== null)
                return;

            _autoCalcValues = values;

            init();
        };

        Sys.Application.add_load(init);

        function init() {
            $('input[data-num-autocalc="1"]').each(function () {
                var $input = $(this);

                var preSum = null;
                var relInputs = [];

                var questions = $input.data('questions');
                if (questions instanceof Array) {
                    for (var i = 0; i < questions.length; i++) {
                        var qId = questions[i];

                        var $relInput = $('[data-question-item="' + qId + '"] input[type="number"].form-control');
                        if ($relInput.length == 0)
                            $relInput = $('[data-question-item="' + qId + '"] input[type="text"].form-control');

                        if ($relInput.length == 1) {
                            var autoCalcInputs = $relInput.data('autoCalcInputs');
                            if (!autoCalcInputs)
                                $relInput.on('change', onAutoCalcFieldChanged).data('autoCalcInputs', autoCalcInputs = []);

                            relInputs.push($relInput);
                            autoCalcInputs.push($input);
                        } else if (_autoCalcValues.hasOwnProperty(qId)) {
                            var value = parseFloat(_autoCalcValues[qId]);
                            if (!isNaN(value)) {
                                if (preSum == null)
                                    preSum = 0;

                                preSum += value;
                            }
                        }
                    }
                }

                if (preSum != null)
                    $input.data('presum', preSum);

                if (relInputs.length > 0)
                    $input.data('relInputs', relInputs);

                this.removeAttribute('data-num-autocalc');
            }).each(function () {
                autoCalcNumberField($(this));
            });
        }

        function onAutoCalcFieldChanged() {
            var autoCalcInputs = $(this).data('autoCalcInputs');
            for (var i = 0; i < autoCalcInputs.length; i++)
                autoCalcNumberField(autoCalcInputs[i]);
        }

        function autoCalcNumberField($input) {
            var relInputs = $input.data('relInputs');
            if (!relInputs)
                return;

            var preSum = $input.data('presum');
            if (typeof preSum != 'number')
                preSum = null;

            var sum = null;

            for (var j = 0; j < relInputs.length; j++) {
                var value = parseFloat(relInputs[j].val());
                if (isNaN(value))
                    continue;

                if (sum == null)
                    sum = 0;

                sum += value;
            }

            if (preSum != null) {
                if (sum == null)
                    sum = preSum;
                else
                    sum += preSum;
            }

            if (sum == null)
                $input.val('');
            else
                $input.val(sum);

            $input.trigger('change');
        }
    })();

    (function () {
        Sys.Application.add_load(init);

        function init() {
            $('input[data-num-permit]').each(function () {
                var $this = $(this);

                var permitItems = []; {
                    var permits = this.dataset.numPermit.split(';');

                    for (var i = 0; i < permits.length; i++) {
                        var value = permits[i];
                        var text = null;
                        if (value == 'N/A' || value == 'N/AP')
                            text = 'Not Applicable';
                        else if (value == 'N/AV')
                            text = 'Not Available';

                        if (text != null)
                            permitItems.push({
                                text: text,
                                value: value
                            });
                    }

                    this.removeAttribute('data-num-permit');
                }

                var $inputs = {};

                if (permitItems.length > 0) {
                    var uniqueIdPostfix = Date.now();
                    var $items = [];

                    for (var i = 0; i < permitItems.length; i++) {
                        var item = permitItems[i];
                        var valueUpper = item.value.toUpperCase();

                        if ($inputs.hasOwnProperty(valueUpper))
                            continue;

                        var uniqueId = 'numoption_' + uniqueIdPostfix++;
                        var $input = $('<input class="form-check-input" type="checkbox">')
                            .attr('id', uniqueId)
                            .attr('value', item.value)
                            .data('input', this)
                            .on('change', onNumPermitOptionChanged);

                        $inputs[valueUpper] = $input;

                        $items.push(
                            $('<div class="form-check">').append(
                                $input,
                                $('<label class="form-check-label">').attr('for', uniqueId).text(item.text)
                            )
                        );
                    }

                    $this.closest('.row').after(
                        $('<div class="row mt-3 row-num-options">').append(
                            $('<div class="col-md-12">').append(
                                $items
                            )
                        )
                    );
                }

                var options = this.value.split(';');
                var hasOptions = false;

                for (var i = 0; i < options.length; i++) {
                    var oValue = options[i].trim().toUpperCase();

                    if (!$inputs.hasOwnProperty(oValue))
                        continue;

                    hasOptions = true;
                    $inputs[oValue].prop('checked', true);
                }

                if (!hasOptions) {
                    this.type = 'number';
                    this.readOnly = false;

                    var value = parseFloat(this.value);
                    if (isNaN(value))
                        this.value = '';
                    else
                        this.value = value;
                } else {
                    this.type = 'text';
                    this.readOnly = true;
                }
            });
        };

        function onNumPermitOptionChanged() {
            var checkInput = this;
            var textInput = $(this).data('input');
            var options = [];
            var isSingle = false;

            if (!isSingle && textInput.type == 'text') {
                var values = textInput.value.split(';');
                for (var i = 0; i < values.length; i++) {
                    var v = values[i].trim();
                    if (v.length > 0)
                        options.push(v);
                }
            }

            if (checkInput.checked) {
                var exists = false;
                var valueUpper = checkInput.value.toUpperCase();

                for (var i = 0; i < options.length; i++) {
                    if (options[i].toUpperCase() == valueUpper) {
                        exists = true;
                        break;
                    }
                }

                if (!exists)
                    options.push(checkInput.value);

                if (isSingle)
                    $(this).closest('.row-num-options').find('input[type="checkbox"]').each(function () {
                        if (this != checkInput)
                            this.checked = false;
                    });
            } else {
                var valueUpper = checkInput.value.toUpperCase();

                for (var i = 0; i < options.length; i++) {
                    if (options[i].toUpperCase() == valueUpper) {
                        options.splice(i, 1);
                        break;
                    }
                }
            }

            if (options.length == 0) {
                textInput.type = 'number';
                textInput.readOnly = false;
                textInput.value = '';
            } else {
                textInput.type = 'text';
                textInput.readOnly = true;
                textInput.value = options.join('; ');
            }

            $(textInput).trigger('change');
        }
    })();

    (function () {
        var instance = window.answerPageGlossary = window.answerPageGlossary || {};

        instance.refreshLinks = function () {
            var termsData = $("#<%= TermsData.ClientID %>").val();
            if (!termsData)
                return;

            var termsData = JSON.parse(termsData);
            var $modal = $('#<%= TermWindow.ClientID %>');

            $('a').each(function () {
                var $this = $(this);

                var name = $this.attr('href');
                if (!termsData.hasOwnProperty(name))
                    return;

                $this
                    .addClass('glossary-term')
                    .attr('title', 'View Term Definition')
                    .off('click', onTermClick)
                    .on('click', onTermClick);
            });

            function onTermClick(e) {
                e.stopPropagation();
                e.preventDefault();

                var $this = $(this);

                var name = $this.attr('href');
                if (!termsData.hasOwnProperty(name))
                    return;

                var term = termsData[name];

                var idPostfix = String((new Date()).getTime());
                var $tabs = $('<ul class="nav nav-tabs justify-content-start fs-sm" role="tablist">');
                var $content = $('<div class="tab-content">');
                var firstTab = null;
                var defaultTab = null;
                var selectedTab = null;
                var tabCount = 0;

                for (var lang in term.descr) {
                    if (!term.descr.hasOwnProperty(lang))
                        continue;

                    var paneId = lang + '-' + idPostfix;
                    var tabId = paneId + '-tab';
                    var $btn, $pane;

                    $tabs.append(
                        $('<li class="nav-item" role="presentation">').append(
                            $btn = $('<button class="nav-link" data-bs-toggle="tab" type="button" role="tab" aria-selected="false">')
                                .attr('id', tabId)
                                .attr('data-bs-target', '#' + paneId)
                                .attr('aria-controls', paneId)
                                .text(lang.toUpperCase())));
                    $content.append(
                        $pane = $('<div class="tab-pane" role="tabpanel">')
                            .attr('id', paneId)
                            .attr('aria-labelledby', tabId)
                            .html(term.descr[lang]));

                    if (selectedTab == null) {
                        if (lang == '<%= Current.Language %>') {
                            selectedTab = getTabInfo();
                        } else {
                            if (firstTab == null)
                                firstTab = getTabInfo();

                            if (defaultTab == null && lang == 'en')
                                defaultTab = getTabInfo();
                        }

                        function getTabInfo() {
                            return {
                                $btn: $btn,
                                $pane: $pane
                            };
                        }
                    }

                    tabCount++;
                }

                if (selectedTab == null) {
                    if (defaultTab != null)
                        selectedTab = defaultTab;
                    else
                        selectedTab = firstTab;
                }

                if (selectedTab != null) {
                    selectedTab.$btn.addClass('active').attr('aria-selected', true);
                    selectedTab.$pane.addClass('show active');
                }

                $modal.find('> .modal-dialog > .modal-content > .modal-header > .modal-title').text(term.title);
                var $modalBody = $modal.find('> .modal-dialog > .modal-content > .modal-body').empty();

                if (tabCount > 1) {
                    $modalBody.append($tabs).append($content);
                } else {
                    $modalBody.html($content.find('> div:first').html());
                }
                
                modalManager.show($modal);
            }
        };

        instance.refreshLinks();
    })();

    (function () {
        Sys.Application.add_load(init);

        function init() {
            document.querySelectorAll('div[data-question-item] table[data-list-max]').forEach(table => {
                if (table.__listRangeMaxInited === true)
                    return;

                const checkboxes = table.querySelectorAll('input[type="checkbox"]');
                if (checkboxes.length === 0)
                    return;

                const maxValue = parseInt(table.dataset.listMax);
                if (isNaN(maxValue) || maxValue < 1)
                    return;

                let isMaxExceeded = false;

                checkboxes.forEach(c => c.addEventListener('change', updateState));

                function updateState() {
                    const count = Array.from(checkboxes).filter(x => x.checked).length;
                    const newValue = count >= maxValue;

                    if (isMaxExceeded === newValue)
                        return;

                    isMaxExceeded = newValue;

                    checkboxes.forEach(checkbox => {
                        if (!checkbox.checked) {
                            checkbox.disabled = isMaxExceeded;
                        }
                    });
                }

                updateState();

                table.__listRangeMaxInited = true;
            });
        }

        init();
    })();

    (function () {
        const counterState = Object.freeze({
            Normal: 0,
            Warning: 1,
            Danger: 2
        });

        Sys.Application.add_load(init);

        function init() {
            document.querySelectorAll('textarea[maxlength],input[type="text"][maxlength]').forEach(createCounter);

            function createCounter(element) {
                if (element.__maxLengthCounter)
                    return;

                const maxLen = element.maxLength;
                if (!maxLen)
                    return;

                const counter = element.__maxLengthCounter = document.createElement('div');
                counter.classList.add('fs-sm', 'mt-2', 'text-end');

                element.after(counter)
                element.addEventListener('input', updateCounter);
                element.addEventListener('keyup', updateCounter);
                element.addEventListener('paste', onPaste);

                updateCounter.apply(element);
            }
        }

        function onPaste() {
            setTimeout(el => updateCounter.apply(el), 10, this)
        }

        function updateCounter() {
            const valueLen = this.value.length;
            const maxLen = this.maxLength;
            const counter = this.__maxLengthCounter;

            counter.textContent = String(maxLen - valueLen) + ' / ' + String(maxLen);

            if (valueLen >= maxLen * 0.9) {
                if (counter.__prevState !== counterState.Danger)
                    counter.classList.remove('text-warning');

                counter.classList.add('text-danger');
                counter.__prevState = counterState.Danger;
            } else if (valueLen >= maxLen * 0.75) {
                if (counter.__prevState !== counterState.Warning)
                    counter.classList.remove('text-danger');

                counter.classList.add('text-warning');
                counter.__prevState = counterState.Warning;
            } else {
                if (counter.__prevState !== counterState.Normal)
                    counter.classList.remove('text-danger', 'text-warning');

                counter.__prevState = counterState.Normal;
            }
        }

        init();
    })();

</script>
</insite:PageFooterContent>

