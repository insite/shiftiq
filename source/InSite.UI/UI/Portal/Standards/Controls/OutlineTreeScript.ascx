<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OutlineTreeScript.ascx.cs" Inherits="InSite.UI.Portal.Standards.Controls.OutlineTreeScript" %>

<%@ Register Src="OutlineNode.ascx" TagName="OutlineNode" TagPrefix="uc" %>

<div id='<%= ClientID %>' style="visibility: hidden;">
    <uc:OutlineNode runat="server" ID="OutlineNode" />
</div>

<div runat="server" id="ConnectionInfoModal" class="modal" tabindex="-1">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title"><insite:Literal runat="server" Text="Content" /></h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-default" data-bs-dismiss="modal">
                    <i class='fas fa-ban'></i> <insite:Literal runat="server" Text="Close" />
                </button>
            </div>
        </div>
    </div>
</div>

<div runat="server" id="TermWindow" class="modal" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">

            <div class="modal-header">
                <h5 class="modal-title"></h5>
            </div>

            <div class="modal-body" style="min-height:50px;">

            </div>

            <div class="modal-footer" style="display:block;">
                <button type="button" class="btn btn-default" data-action="close"><i class='fas fa-ban me-2'></i> <insite:Literal runat="server" Text="Close" /></button>
            </div>
        </div>
    </div>
</div>

<insite:PageHeadContent runat="server">
    <style type="text/css">

        ul.asset-tree {
            list-style: none;
            padding-left: 0px;
        }

        ul.asset-tree > li.asset-branch > ul.asset-tree > li.asset-branch {
            padding-left: 60px;
        }

        ul.asset-tree > li.asset-branch > .card .tree-commands {
            margin: 3px 17px 0 0;
            float: right;
        }

        ul.asset-tree > li.asset-branch > .card .tree-commands > .btn-expand {
            display: none;
        }

        ul.asset-tree > li.asset-branch.collapsed > .card .tree-commands > .btn-collapse {
            display: none;
        }

        ul.asset-tree > li.asset-branch.collapsed > .card .tree-commands > .btn-expand {
            display: inline-block;
        }

        ul.asset-tree > li.asset-branch.collapsed > ul.asset-tree {
            display: none;
        }

        ul.asset-tree > li.asset-branch.collapsed div.competency-content {
            display: none;
        }

            ul.asset-tree > li.asset-branch div.competency-content {
                margin-top: 1.25rem;
            }

            ul.asset-tree > li.asset-branch div.competency-content p {
                margin-bottom: 1rem;
            }

        ul.asset-tree .stfc-status {
            display: none;
        }

        ul.asset-tree.status-show .stfc-status {
            display: inline !important;
        }

        .title-1 {
            font-size: 1.5rem;
        }

        .title-2 {
            font-size: 1.25rem;
        }

        .title-3 {
            font-size: 1rem;
        }

        .title-4 {
            font-size: 0.8rem;
        }

        .card {
            margin-top: 20px;
        }
        .card .card-body {
            padding: 15px;
        }

        .asset-branch h2 {
            font-weight: normal;
            font-size: 18px;
            margin-top: 20px;
        }

        .asset-branch .card-body ul + p { margin-top: 30px; }

        a.glossary-term { text-decoration: underline double; }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            if (window.outlineTree) {
                alert('outlineTree is already registered!');
                return;
            }

            window.outlineTree = { register: register };

            const registered = {};

            function register(elementId, standardId, allowSaveState) {
                if (typeof elementId !== 'string' || registered.hasOwnProperty(elementId) || typeof standardId !== 'string' || standardId.length !== 36)
                    return;

                const elem = document.getElementById(elementId);
                if (!elem || elem.tagName !== 'DIV')
                    return;

                return registered[elementId] = new OutlineTreeObj(elementId, standardId, allowSaveState);
            }

            class OutlineTreeState {
                #standardId;
                #timeoutHandler = null;
                #expanded = [];
                #collapsed = [];

                constructor(standardId) {
                    this.#standardId = standardId;
                }

                save(id, expanded) {
                    if (expanded === true) {
                        if (id instanceof Array) {
                            for (let i = 0; i < id.length; i++)
                                this.#expand(id[i]);
                        } else {
                            this.#expand(id);
                        }
                    } else {
                        if (id instanceof Array) {
                            for (let i = 0; i < id.length; i++)
                                this.#collapse(id[i]);
                        } else {
                            this.#collapse(id);
                        }
                    }

                    if (this.#timeoutHandler != null)
                        clearTimeout(this.#timeoutHandler);

                    this.#timeoutHandler = setTimeout(function (obj) {
                        obj.#timeoutHandler = null;
                        obj.#submitTreeState();
                    }, 350, this);
                }

                #expand(id) {
                    const expandedIndex = $.inArray(id, this.#expanded);
                    if (expandedIndex == -1)
                        this.#expanded.push(id);

                    const collapsedIndex = $.inArray(id, this.#collapsed);
                    if (collapsedIndex >= 0)
                        this.#collapsed.splice(collapsedIndex, 1);
                }

                #collapse(id) {
                    const collapsedIndex = $.inArray(id, this.#collapsed);
                    if (collapsedIndex == -1)
                        this.#collapsed.push(id);

                    const expandedIndex = $.inArray(id, this.#expanded);
                    if (expandedIndex >= 0)
                        this.#expanded.splice(expandedIndex, 1);
                }

                #clear() {
                    this.#expanded = [];
                    this.#collapsed = [];
                }

                #submitTreeState() {
                    $.ajax({
                        type: 'POST',
                        dataType: 'json',
                        url: '/api/standards/treestate?program=' + this.#standardId,
                        headers: {
                            'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>'
                        },
                        data: {
                            expanded: this.#expanded,
                            collapsed: this.#collapsed
                        },
                    });

                    this.#clear();
                }
            }

            class OutlineTreeObj {
                #elementId;
                #treeState;
                #allowExpandTree;
                #allowSaveState = true;
                #displayCompetencyStatus = false;
                #displayCompetencyStatusKey;

                constructor(elementId, standardId, allowSaveState) {
                    const $elem = $(document.getElementById(elementId));
                    const $collapseNode = $elem.find('.tree-commands .btn-collapse').on('click', (e) => this.#onNodeCollapse(e));
                    const $expandNode = $elem.find('.tree-commands .btn-expand').on('click', (e) => this.#onNodeExpand(e));

                    $elem.find('.tree-commands').each(function () {
                        var $commands = $(this);
                        var $branch = $commands.closest('li.asset-branch');
                        if ($branch.find('ul.asset-tree').length === 0) {
                            $commands.hide();
                            $branch.removeClass('collapsed').addClass('tree-leaf');
                        }
                    });

                    this.#elementId = elementId;
                    this.#allowSaveState = allowSaveState !== false;
                    this.#allowExpandTree = $collapseNode.length > 0 && $expandNode.length > 0;

                    if (this.#allowSaveState) {
                        this.#treeState = new OutlineTreeState(standardId);

                        this.#displayCompetencyStatus = true;
                        this.#displayCompetencyStatusKey = 'portal.outlinetree.' + standardId;

                        try {
                            const json = window.localStorage.getItem(this.#displayCompetencyStatusKey);
                            if (json) {
                                const obj = JSON.parse(json);
                                if (typeof obj === 'boolean')
                                    this.#displayCompetencyStatus = obj;
                            }
                        } catch (e) {

                        }
                    }

                    this.isShowCompetencyStatus = null;

                    $elem.css('visibility', 'visible');
                }

                get allowExpandTree() {
                    return this.#allowExpandTree;
                }

                get isShowCompetencyStatus() {
                    return this.#displayCompetencyStatus;
                }

                set isShowCompetencyStatus(value) {
                    if (typeof value === 'boolean')
                        this.#displayCompetencyStatus = value;

                    const $tree = $(document.getElementById(this.#elementId)).find('> ul.asset-tree:first');

                    if (this.#displayCompetencyStatus)
                        $tree.addClass('status-show');
                    else
                        $tree.removeClass('status-show');

                    if (this.#allowSaveState) {
                        try {
                            window.localStorage.setItem(this.#displayCompetencyStatusKey, this.#displayCompetencyStatus);
                        } catch (e) {

                        }
                    }
                }

                expandAll() {
                    const ids = [];

                    $(document.getElementById(this.#elementId)).find('ul.asset-tree > li.asset-branch:not(.tree-leaf)').each(function () {
                        ids.push($(this).removeClass('collapsed').data('id'));
                    });

                    if (this.#allowSaveState === true)
                        this.#treeState.save(ids, true);
                }

                collapseAll() {
                    const ids = [];

                    $(document.getElementById(this.#elementId)).find('ul.asset-tree > li.asset-branch:not(.tree-leaf)').each(function () {
                        const $li = $(this);

                        if (!$li.hasClass('collapsed'))
                            $li.addClass('collapsed');

                        ids.push($li.data('id'));
                    });

                    if (this.#allowSaveState === true)
                        this.#treeState.save(ids, false);
                }

                #onNodeCollapse(e) {
                    e.preventDefault();

                    const $li = $(e.currentTarget).closest('li.asset-branch').addClass('collapsed');

                    if (this.#allowSaveState === true)
                        this.#treeState.save($li.data('id'), false);
                }

                #onNodeExpand(e) {
                    e.preventDefault();

                    const $li = $(e.currentTarget).closest('li.asset-branch').removeClass('collapsed');

                    if (this.#allowSaveState === true)
                        this.#treeState.save($li.data('id'), true);
                }
            }
        })();

        (function () {
            const $modal = $('#<%= ConnectionInfoModal.ClientID %>');
            const bModal = new bootstrap.Modal($modal[0]);
            let isLocked = false;

            $('.asset-tree .table-connections [data-action="connection-info"]').on('click', function (e) {
                e.preventDefault();

                const standard = $(this).data('id');
                if (!standard || isLocked)
                    return;

                isLocked = true;

                $.ajax({
                    type: 'GET',
                    dataType: 'json',
                    url: '/api/contents/html',
                    headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: {
                        container: standard,
                        type: 'standard',
                        lang: '<%= Identity.Language %>'
                    },
                    success: function (result) {
                        $container = $('<div>');

                        if (result instanceof Array && result.length > 0) {
                            for (let i = 0; i < result.length; i++) {
                                const item = result[i];
                                if (item.title === 'Title')
                                    $container.append($('<h5>').html(item.value));
                                else
                                    $container.append($('<div>').html(item.value));
                            }
                        } else {
                            $container.html('<p><strong><%= Translate("No Data") %></strong></p>');
                        }

                        $modal.find('> .modal-dialog > .modal-content > .modal-body').empty().append($container);
                        bModal.show();
                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    },
                    complete: function () {
                        isLocked = false;
                    },
                });
            });
        })();

        (function () {
            const instance = window.termWindow = window.termWindow || {};
            const $modal = $('#<%= TermWindow.ClientID %>').on('click', onModalClick);
            const modal = new bootstrap.Modal($modal[0]);

            let termsData = <%= TermsData.IfNullOrEmpty("null") %>;

            onTermsUpdated();

            instance.updateTerms = function (data) {
                termsData = data;

                onTermsUpdated();
            };

            function onTermsUpdated() {
                $('a').each(function () {
                    const $this = $(this);
                    if ($this.data('terms') === true)
                        return;

                    const name = $this.attr('href');
                    if (!termsData.hasOwnProperty(name))
                        return;

                    $this.addClass('glossary-term').attr('title', 'View Term Definition').on('click', onTermClick);

                    $this.data('terms', true);
                });
            }

            function onTermClick(e) {
                e.stopPropagation();
                e.preventDefault();

                const $this = $(this);

                const name = $this.attr('href');
                if (!termsData.hasOwnProperty(name))
                    return;

                const term = termsData[name];

                $modal.find('> .modal-dialog > .modal-content > .modal-header > .modal-title').text(term.title);
                $modal.find('> .modal-dialog > .modal-content > .modal-body').html(term.descr);

                modal.show();
            }

            function onModalClick(e) {
                const $target = $(e.target);
                const action = $target.data('action');

                if (action === 'close')
                    modal.hide();
            }
        })();
    </script>
</insite:PageFooterContent>