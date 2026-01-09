<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkshopQuestionRepeater.ascx.cs" Inherits="InSite.Admin.Assessments.Questions.Controls.WorkshopQuestionRepeater" %>

<%@ Import Namespace="InSite.Admin.Assessments.Forms.Models" %>
<%@ Import Namespace="InSite.Domain.Banks" %>
<%@ Import Namespace="Newtonsoft.Json" %>

<%@ Register TagPrefix="uc" TagName="WorkshopQuestionCommentRepeater" Src="WorkshopQuestionCommentRepeater.ascx" %>
<%@ Register TagPrefix="uc" TagName="WorkshopAssetTable" Src="WorkshopQuestionAssetTable.ascx" %>
<%@ Register TagPrefix="assessments" Assembly="InSite.UI" Namespace="InSite.Admin.Assessments.Web.UI" %>

<div runat="server" id="MainPanel" class="row">
    <div class="col-md-12">
        <asp:Repeater runat="server" ID="Repeater">

            <HeaderTemplate>
                <table id='<%= Repeater.ClientID %>' class="table question-grid">
                    <tbody>
            </HeaderTemplate>

            <FooterTemplate></tbody></table></FooterTemplate>

            <ItemTemplate>
                <asp:Literal runat="server" ID="QuestionIdentifier" Text='<%# Eval("QuestionId") %>' Visible="false" />

                <tr data-question='<%# Eval("QuestionId") %>' data-editable='<%# (PublicationStatus)Eval("Entity.PublicationStatus") == PublicationStatus.Published ? "0" : null %>'>

                    <td style="width:70px;">
                        <div class="sequence" title="Bank Question Number">
                            <%# (int)Eval("Entity.BankIndex") + 1 %>
                        </div>
                        <div runat="server" visible='<%# Eval("Entity.FirstPublished") == null %>' class="mb-1">
                            <insite:IconLink runat="server" Name="pencil" NavigateUrl='<%# GetChangeUrl((DataItem)Container.DataItem) %>' ToolTip="Edit Question" />
                        </div>
                        <div class="mb-1">
                            <insite:IconButton runat="server" Name="comment" ToolTip="New Comment" OnClientClick='<%# string.Format("showCommentWindow(\"{0}\",\"{1}\"); return false;", Eval("QuestionId"), Eval("FieldId")) %>' />
                        </div>
                        <div runat="server" id="CopyField" class="mb-1">
                            <insite:IconButton runat="server" ToolTip="Duplicate" CommandName="Copy" Name="copy" ConfirmText="Are you sure you want to create a copy of this question?" />
                        </div>
                        <div class="mb-1">
                            <insite:IconLink runat="server" Name="chart-bar" NavigateUrl='<%# GetAnalysisUrl((DataItem)Container.DataItem) %>' ToolTip="Question Analysis" />
                        </div>
                         <div class="mb-1" style="<%# Eval("CandidateCommentsVisibility") %>">
                             <a title="Candidate Comments" href='<%# Eval("CandidateCommentUrl") %>' target="_blank">
                                <p class="bubble speech">
                                    <%# Eval("CandidateCommentsCount") %>
                                </p>
                             </a>
                        </div>
                        <div class="mb-1" title="Form Question Number">
                            <span class="badge rounded-pill bg-custom-default fs-5"><%# Eval("FormSequence") %></span>
                        </div>
                        <div>
                            <%# GetFlagHtml((FlagType)Eval("Entity.Flag")) %>
                            <span class="form-text">
                                <a href="#" class="editable-input flag"
                                    data-name='<%# ElementUpdater.ElementTypes.QuestionFlag %>'
                                    data-type="select"
                                    data-pk='<%# BankID + ":" + Eval("QuestionId") %>'
                                    data-value='<%# Eval("Entity.Flag") %>'>
                                    <%# Eval("Entity.Flag") %>
                                </a>
                            </span>
                        </div>
                    </td>

                    <td>
                        <div class="position-relative text-dark">
                            <a runat="server" href="#" class="editable-input question"
                                data-name='<%# ElementUpdater.ElementTypes.QuestionTitle %>'
                                data-type="textarea"
                                data-pk='<%# BankID + ":" + Eval("QuestionId") %>'
                                data-value='<%# "+" + ((MultilingualString)Eval("Entity.Content.Title")).Serialize() %>'>
                                <%# Shift.Common.Markdown.ToHtml(((MultilingualString)Eval("Entity.Content.Title")).Default) %>
                            </a>
                        </div>

                        <asp:MultiView runat="server" ID="QuestionItemsMultiView">

                            <asp:View runat="server" ID="SingleCorrectItemsView">
                                <div class="mb-3 text-dark">
                                    <asp:Repeater runat="server" ID="SingleCorrectOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="option-grid">
                                                <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                                                <td><%# Eval("Letter") %>.</td>
                                                <%# GetOptionRepeaterTitle(Container) %>
                                                <td class="form-text option-points">&bull; <%# GetOptionPointsEditable() %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="TrueOrFalseItemsView">
                                <div class="mb-3 text-dark">
                                    <asp:Repeater runat="server" ID="TrueOrFalseOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="option-grid">
                                                <%# GetOptionRepeaterTableHead("<th></th><th></th>", "<th></th>") %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# GetSingleCorrectOptionIcon((decimal)Eval("Points")) %></td>
                                                <td><%# Eval("Letter") %>.</td>
                                                <%# GetOptionRepeaterTitle(Container) %>
                                                <td class="form-text option-points">&bull; <%# GetOptionPointsEditable() %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="MultipleCorrectItemsView">
                                <div class="mb-3 text-dark">
                                    <asp:Repeater runat="server" ID="MultipleCorrectOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="option-grid">
                                                <%# GetOptionRepeaterTableHead("<th></th><th></th>", null) %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# GetMultipleCorrectOptionIcon((bool?)Eval("IsTrue")) %></td>
                                                <td><%# Eval("Letter") %>.</td>
                                                <%# GetOptionRepeaterTitle(Container) %>
                                                <td class="form-text option-points">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="ComposedRubricItemsView">
                                <div class="mb-3 text-dark">
                                    <asp:Repeater runat="server" ID="ComposedRubricOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="option-grid">
                                                <%# GetOptionRepeaterTableHead("<th></th>", "<th></th>") %>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("Letter") %>.</td>
                                                <%# GetOptionRepeaterTitle(Container) %>
                                                <td class="option-points form-text">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="BooleanTableItemsView">
                                <div class="mb-3 text-dark">
                                    <asp:Repeater runat="server" ID="BooleanTableOptionRepeater">
                                        <HeaderTemplate>
                                            <table class="option-grid">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <%# GetOptionRepeaterTableHeadTitleCols() %>
                                                        <th style="width: 30px;" title="True"><i class='far fa-check'></i></th>
                                                        <th style="width: 30px;" title="False"><i class='far fa-times'></i></th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                        </HeaderTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("Letter") %>.</td>
                                                <%# GetOptionRepeaterTitle(Container) %>
                                                <td title="True"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), true) %></td>
                                                <td title="False"><%# GetBooleanTableOptionIcon((bool?)Eval("IsTrue"), false) %></td>
                                                <td class="form-text option-points">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                    </asp:Repeater>
                                </div>
                            </asp:View>

                            <asp:View runat="server" ID="MatchesItemsView">
                                <div class="mb-3 text-dark">

                                    <asp:Repeater runat="server" ID="MatchingPairsRepeater">
                                        <HeaderTemplate>
                                            <h5 class="p-1 bg-secondary">Matching Pairs</h5>
                                            <table class="table table-condensed matching-pairs">
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td class="w-50"><%# Eval("Left") %></td>
                                                <td class="w-50"><%# Eval("Right") %></td>
                                                <td class="form-text pair-points">&bull; <%# GetOptionPoints((decimal)Eval("Points")) %>
                                                </td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <asp:Repeater runat="server" ID="MatchingDistractorsRepeater">
                                        <HeaderTemplate>
                                            <h5 class="p-1 bg-secondary">Matching Distractors</h5>
                                            <table class="table table-condensed">
                                                <tbody>
                                        </HeaderTemplate>
                                        <FooterTemplate></tbody></table></FooterTemplate>
                                        <ItemTemplate>
                                            <tr>
                                                <td><%# Eval("Value") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                </div>
                            </asp:View>

                        </asp:MultiView>

                        <div class="my-3">
                            <insite:DropDownButton runat="server" ID="ReplaceButton" IconName="tools" Text="Replace" CssClass="d-inline-block">
                                <Items>
                                    <insite:DropDownButtonItem Name="NewVersion" IconType="Regular" IconName="arrow-alt-up" Text="New Version" ToolTip="New Version" />
                                    <insite:DropDownButtonItem Name="NewQuestionAndSurplus" IconType="Regular" IconName="plus-circle" Text="New Question and Surplus" ToolTip="New Question and Surplus" />
                                    <insite:DropDownButtonItem Name="NewQuestionAndPurge" IconType="Regular" IconName="plus-hexagon" Text="New Question and Purge" ToolTip="New Question and Purge" />
                                    <insite:DropDownButtonItem Name="RollbackQuestion" IconType="Regular" IconName="undo-alt" Text="Revert/Rollback" ToolTip="Revert/Rollback" />
                                </Items>
                            </insite:DropDownButton>

                            <button type="button" class="btn btn-default btn-sm move-top">
                                <i class='fas fa-arrow-up me-2'></i> Top
                            </button>
                            <button type="button" class="btn btn-default btn-sm move-prev" style="display: none;">
                                <i class='fas fa-arrow-alt-left me-2'></i> Previous
                            </button>
                            <button type="button" class="btn btn-primary btn-sm move-next" style="display: none;">
                                Next <i class='fas fa-arrow-alt-right ms-2'></i>
                            </button>
                        </div>
                        
                        <div class="row">
                            <div class="col-lg-12 col-sm-6 col-xs-6">
                                <div class="alert alert-warning">
                                    <i class="fas fa-clipboard-list mt-1 position-absolute"></i>
                                    <div class="ms-4">
                                        <%# GetCompetencyTitle((Guid)Eval("Entity.Set.Identifier"), (Guid)Eval("Entity.Standard"), (Guid)Eval("QuestionId"), (PublicationStatus)Eval("Entity.PublicationStatus")) %>
                                    </div>
                                </div>
                                
                                <%# DisplayRationale(Container.DataItem) %> 

                                <div class="row posted-comments-section">
                                    <uc:WorkshopQuestionCommentRepeater runat="server" ID="CommentRepeater" />
                                </div>
                            </div>
                            <div class="col-sm-6 col-xs-6 d-lg-none">
                                <uc:WorkshopAssetTable runat="server" FormIdentifier='<%# EntityID %>' Question='<%# ((DataItem)Container.DataItem).Entity %>' ReturnUrl='<%# _returnUrl %>' />
                            </div>
                        </div>

                    </td>

                    <td class="d-none d-lg-table-cell text-dark" style="width:340px;">
                        <uc:WorkshopAssetTable runat="server" FormIdentifier='<%# EntityID %>' Question='<%# ((DataItem)Container.DataItem).Entity %>' ReturnUrl='<%# _returnUrl %>' />
                    </td>

                </tr>
            </ItemTemplate>

        </asp:Repeater>
    </div>
</div>

<insite:PageHeadContent runat="server" ID="GridStyle">
    <style type="text/css">
        /* Question Grid */

        p.bubble {
	        position: relative;
	        width: 30px;
	        text-align: center;
	        color:white;
	        line-height: 1.3em;
	        margin: 5px auto;
            margin-bottom: 15px;
	        background-color: #f74f78;
	        border-radius: 7px;
	        font-size:18px;
        }

        p.thought {
	        width: 30px;
	        border-radius: 20px;
	        padding: 3px;	
        }

        p.bubble:before,
        p.bubble:after {
	        content: ' ';
	        position: absolute;
	        width: 0;
	        height: 0;
        }

        p.speech:before {
	        left: 13px;
	        bottom: -9px;
	        border: 5px solid;
	        border-color: #f74f78 #f74f78 transparent transparent;
        }


        p.thought:before,
        p.thought:after {
	        left: 20px;
	        bottom: -30px;
	        width: 5px;
	        height: 5px;
	        background-color: #f74f78;
	        border: 8px solid #f74f78;
	        -webkit-border-radius: 28px;
	        -moz-border-radius: 28px;
	        border-radius: 28px;
        }

        p.thought:after {
	        width: 5px;
	        height: 5px;
	        left: 5px;
	        bottom: -40px;
	        -webkit-border-radius: 18px;
	        -moz-border-radius: 18px;
	        border-radius: 18px;
        }

        table.question-grid > tbody > tr > td {
            padding: 20px 0 20px 20px;
        }

        .question-grid img,
        .question-grid iframe {
            max-width: 100% !important;
        }

        .question-grid .labels a {
            margin-right: 3px;
            color: white !important;
        }

            .question-grid .labels a:hover {
                text-decoration: none;
            }

                .question-grid .labels a:hover span {
                    background-color: #333;
                }

            .question-grid .labels a.label {
                white-space: normal !important;
            }

        .question-grid > thead > tr > th:first-child {
            text-align: center;
        }

        .question-grid > tbody > tr > td:first-child {
            vertical-align: top;
            text-align: center;
            width: 140px;
            border-left: 1px solid transparent;
        }

            .question-grid > tbody > tr > td:first-child > div.sequence {
                font-size: 27px;
                color: #265F9F;
                white-space: nowrap;
            }

        .question-grid > tbody > tr:first-child > td {
            border-top: none;
        }

        .question-grid .option-grid td {
            vertical-align: top;
            padding: 5px;
        }

        .question-grid .option-grid th {
            vertical-align: bottom;
            padding: 5px;
        }

        .question-grid .option-grid {
            margin-left: 10px;
        }

        table.option-grid .option-points,
        table.matching-pairs .pair-points {
            padding-top: 8px;
            padding-left: 0;
            white-space: nowrap;
        }

        .question-grid table.property-grid {
            width: 100%;
        }

            .question-grid table.property-grid > tbody > tr:nth-child(even) {
                background-color: #F5F5F5;
            }

            .question-grid table.property-grid > tbody > tr > td {
                border: 1px solid #ddd;
                padding: 5px !important;
                vertical-align: top;
            }

            table.option-grid .no-right-pad {
                padding-right: 0 !important;
            }

            table.option-grid .no-left-pad {
                padding-left: 0 !important;
            }

            table.option-grid .small-right-pad {
                padding-right: 0.25em !important;
            }

            table.option-grid .small-left-pad {
                padding-left: 0.25em !important;
            }

            table.option-grid .x-wide {
                padding-right: 5em !important;
            }

        /* Inline editor */

        main a.editable-input,
        main a.editable-input:hover {
            color: var(--ar-dark);
            text-decoration: none;
            border-bottom: none;
            white-space: normal;
        }

        main a.editable-input:hover {
            background-color: lightyellow !important;
        }

        main table.question-grid .editable-container.editable-inline div.editable-input {
            max-width: 650px;
        }

        main table.question-grid .editable-container.editable-inline div.editable-input .form-control,
        main table.question-grid .editable-container.editable-inline div.editable-input .form-select {
            width: auto;
        }

        main .option-points div.editable-input input {
            width: 80px !important;
        }

        main .option-title div.editable-input textarea {
            width: 400px !important;
            height: 35px;
        }

        main .asset-table div.editable-input input {
            width: 130px !important;
        }

        main .alert div.editable-input select {
            width: 420px !important;
        }

        @media only screen and (max-width: 1700px) {
            main .alert div.editable-input select {
                width: 200px !important;
            }
        }

        @media only screen and (max-width: 992px) {
            main .alert div.editable-input select {
                width: 160px !important;
            }
        }

        main .editable-container .editable-error-block {
            color: #a94442;
            background-color: #f2dede;
            border: 1px solid #ebccd1;
            border-radius: 4px;
            padding: 5px 10px;
            margin: 15px 0 0px;
            font-weight: bold;
            max-width: 471px;
        }
    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server" ContentKey="InitEditors">
    <script type="text/javascript">
        (function () {
            var instance = window.workshopQuestionRepeater = window.workshopQuestionRepeater || {};

            instance.scrollToQuestion = function (id) {
                if (!id)
                    return;

                var $row = $('#<%= Repeater.ClientID %> > tbody > tr[data-question="' + String(id) + '"]');
                if ($row.length !== 1)
                    return;

                scrollToSetQuestion($row);
            };

            Sys.Application.add_load(onLoad);

            function onLoad() {
                var $table = $('table#<%= Repeater.ClientID %>');
                if ($table.length !== 1 || $table.data('inited') === true)
                    return;

                $.fn.editable.defaults.mode = 'inline';
                $.fn.editable.defaults.url = '/api/assessments/forms/setelementvalue';
                $.fn.editable.defaults.onblur = 'ignore';

                <%= CompetencyLists %>

                var $activeEditable = null;

                $table.find('> tbody > tr[data-question][data-editable="0"] a.editable-input').each(function () {
                    var $anchor = $(this);
                    var name = $anchor.data('name');

                    if (name === 'Question' || name === 'Option' || name === 'Points' || name === 'Header' || name === 'Column')
                        $anchor.replaceWith($anchor[0].childNodes);
                });

                $table.find('a.editable-input').on('shown', function (e, editable) {
                    editable.container.$form.find('.editable-buttons .btn').addClass('btn-icon');
                    
                    const maxLength = editable.container.$element.data('maxlength');
                    if (maxLength)
                        editable.input.$input.attr('maxlength', maxLength);

                    var $this = $(this);
                    if ($activeEditable && !$this.is($activeEditable))
                        $activeEditable.editable('hide');

                    $activeEditable = $this;
                }).each(function () {
                    var $editable = $(this);
                    var name = $editable.data('name');

                    var options = {};

                    if (name == '<%= ElementUpdater.ElementTypes.QuestionFlag %>') {
                        options = {
                            source: [<%= FlagList %>],
                            success: function (data) {
                                var $parent = $(this).closest('div');

                                $parent.find('span:first-child').not('.form-text').remove();
                                $parent.prepend(data.iconHtml);
                            }
                        };
                    } else if ($editable.hasClass('status')) {
                        options = {
                            source: [<%= StatusList %>],
                            success: function (data, newValue) {
                                syncAssetTableFields($(this), newValue);
                            }
                        };
                    } else if (name == '<%= ElementUpdater.ElementTypes.QuestionTaxonomy %>') {
                        options = {
                            source: [<%= TaxonomyList %>],
                            success: function (data, newValue) {
                                syncAssetTableFields($(this), newValue);
                            }
                        };
                    } else if (name == '<%= ElementUpdater.ElementTypes.OptionPoints %>') {
                        options = {
                            success: function (data) {
                                var $icon = $(this).closest('tr').find('td i').first();

                                if (Number(data.points) > 0) {
                                    $icon.removeClass('fa-times-circle text-danger').addClass('fa-check-circle');
                                    $icon.css('color', '#5CB85C');
                                } else {
                                    $icon.removeClass('fa-check-circle').addClass('fa-times-circle text-danger');
                                    $icon.css('color', '#f74f78');
                                }

                                return { newValue: data.points };
                            }
                        };
                    } else if (name == '<%= ElementUpdater.ElementTypes.QuestionTitle %>') {
                        options = {
                            display: function (value, response) {
                                return false;
                            },
                            success: function (data, newValue) {
                                $(this).html(data.html);
                                $(this).data('value', newValue);
                            },
                            validate: function () {
                                var result = {
                                    newValue: workshopQuestion.markdown.getValue()
                                };

                                if (result.newValue != null)
                                    return result;
                            }
                        };

                        (function () {
                            var _hide = null;

                            $editable.on('shown', function (e, editable) {
                                if (editable.container.hide !== hide) {
                                    _hide = editable.container.hide;
                                    editable.container.hide = hide;
                                }

                                editable.container.$form
                                    .find('.editable-input textarea').hide().end()
                                    .one('show', function () {
                                        var $container = $(this).find('div.editable-input').css('width', 'calc(100% - 80px)');
                                        var $textarea = $container.find('textarea');
                                        workshopQuestion.markdown.move($textarea);
                                        $(window).triggerHandler('shown.xeditable');
                                        workshopQuestion.markdown.set($textarea);
                                    });
                            });

                            function hide() {
                                workshopQuestion.markdown.remove();
                                _hide.call(this, arguments);
                            }
                        })();
                    } else if (name == '<%= ElementUpdater.ElementTypes.OptionTitle %>') {
                        options = {
                            display: function (value, response) {
                                return false;
                            },
                            success: function (data, newValue) {
                                $(this).html(data.html);
                                $(this).data('value', newValue);
                            },
                            validate: function (text) {
                                return {
                                    newValue: questionTextEditor.fromInSiteMarkdown(text)
                                };
                            }
                        };

                        $editable.on('shown', function (e, editable) {
                            var $input = editable.input.$input;
                            var text = $input.val();
                            text = questionTextEditor.toInSiteMarkdown(text);
                            $input.val(text);
                        });
                    } else if (name == '<%= ElementUpdater.ElementTypes.OptionHeaderColumn %>' || name == '<%= ElementUpdater.ElementTypes.OptionTitleColumn %>') {
                        options = {
                            display: function (value, response) {
                                return false;
                            },
                            success: function (data, newValue) {
                                $(this).html(data.html);
                                $(this).data('value', newValue);
                            }
                        };
                    }

                    if ($editable.closest('.asset-table').length > 0) {
                        options.success = function (data, newValue) {
                            syncAssetTableFields($(this), newValue);
                        };
                    }

                    $editable.editable(options);
                });

                {
                    var $prev = null;

                    $table.find('> tbody > tr[data-question]').each(function () {
                        var $this = $(this);

                        if ($prev != null) {
                            $prev.data('next', $this);
                            $this.data('prev', $prev);
                        }

                        $prev = $this;
                    });
                }

                $table.find('.move-top').on('click', function () {
                    scrollTop();
                });

                $table.find('.move-prev').on('click', function () {
                    var $row = $(this).closest('tr[data-question]').data('prev');
                    if ($row)
                        scrollToSetQuestion($row);
                }).each(function () {
                    var $this = $(this);
                    if (!$this.closest('tr[data-question]').data('prev'))
                        $this.remove();
                    else
                        $this.show();
                });

                $table.find('.move-next').on('click', function () {
                    var $row = $(this).closest('tr[data-question]').data('next');
                    if ($row)
                        scrollToSetQuestion($row);
                }).each(function () {
                    var $this = $(this);
                    if (!$this.closest('tr[data-question]').data('next'))
                        $this.remove();
                    else
                        $this.show();
                });

                $table.data('inited', true);
            }

            function syncAssetTableFields($link, newValue) {
                var $tr = $link.closest("tr[data-question]");
                var $editors = $tr.find("a.editable-input[data-name='" + $link.data("name") + "']").not($link);

                $editors.editable("setValue", newValue, false);
            }

            function scrollToSetQuestion($row) {
                var headerHeight = $('header.navbar:first').outerHeight();
                var scrollTo = $row.offset().top - headerHeight;

                if (scrollTo < 0)
                    scrollTo = 0;

                $('html, body').animate({ scrollTop: scrollTo }, 250);
            }

            function scrollTop() {
                var $content = $('table#<%= Repeater.ClientID %>').closest('.card-body');
                var scrollTo = $content.offset().top;

                var headerHeight = $('header.navbar:first').outerHeight();
                var scrollTo = $content.offset().top - headerHeight;

                if (scrollTo < 0)
                    scrollTo = 0;

                $('html, body').animate({ scrollTop: scrollTo }, 250);
            }
        })();
    </script>
</insite:PageFooterContent>
