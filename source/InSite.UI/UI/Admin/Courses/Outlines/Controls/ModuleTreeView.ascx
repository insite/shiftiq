<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ModuleTreeView.ascx.cs" Inherits="InSite.Admin.Courses.Outlines.Controls.ModuleTreeView" %>

<insite:Container runat="server" ID="TreeWrapper">

    <style>
        .hide-button-text span {
            display: none;
            margin-left: 0.5rem;
        }
        .hide-button-text i {
            margin-right: 0 !important;
        }

        @media (min-width:2400px) {
            .hide-button-text span {
                display: block;
            }
        }
    </style>
    
    <div class="d-flex justify-content-between mb-2">
        <div>

            <insite:DropDownButton runat="server" ID="ActionCommandsDropDown" IconName="screwdriver-wrench" Text="Action" CssClass="d-inline-block">
                <Items>
                    <insite:DropDownButtonItem Name="ActionPreview" ToolTip="Preview" IconType="Regular" IconName="external-link" Text="Preview" />
                    <insite:DropDownButtonItem Name="ActionReorder" ToolTip="Reorder" IconType="Regular" IconName="sort" Text="Reorder" />
                    <insite:DropDownButtonItem Name="ActionPublish" ToolTip="Publish" IconType="Regular" IconName="upload" Text="Publish" />
                    <insite:DropDownButtonItem Name="ActionDownload" ToolTip="Download" IconType="Regular" IconName="download" Text="Download" />
                </Items>
            </insite:DropDownButton>

            <insite:Button runat="server"
                ID="HistoryButton"
                ButtonStyle="Default"
                Text="History"
                Icon="fas fa-history"
            />
        </div>
        <div class="hide-button-text">
            <insite:Button runat="server"
                ID="ExpandAllButton"
                Icon="fas fa-chevron-down"
                PostBackEnabled="false"
                ButtonStyle="Default"
                Text="<span>Expand All</span>"
                ToolTip="Expand All"
            />
            <insite:Button runat="server"
                ID="CollapseAllButton"
                Icon="fas fa-chevron-up"
                PostBackEnabled="false"
                ButtonStyle="Default"
                Text="<span>Collapse All</span>"
                ToolTip="Collapse All"
            />
        </div>
    </div>

    <div runat="server" id="UnitComboBoxWrapper" class="mb-2">
        <insite:UnitComboBox runat="server" ID="UnitComboBox" AllowBlank="False" />
    </div>

    <ul runat="server" id="Tree" class="tree-view">
        <asp:Repeater runat="server" ID="ModuleRepeater">
            <ItemTemplate>
                <asp:Literal runat="server" ID="ModuleID" Text='<%# Eval("Identifier") %>' Visible="false" />
                <li class="item-module" data-key="<%# Eval("Identifier") %>">
                    <div>
                        <div>
                            <div class="node-title">
                                <span class='text'>
                                    <span>
                                        <strong>
                                            <span runat="server" visible='<%# ShowMetadataChecked && Eval("Code") != null %>' class="badge bg-info">
                                                <%# Eval("Code") %>
                                            </span>
                                            <%# Eval("Name") %>
                                        </strong>
                                    </span>
                                    <div><%# CreateMetadataHtmlForModule(Eval("Identifier")) %></div>
                                </span>
                            </div>
                            <div runat="server" id="CommandsContainer" class="node-inputs">
                                <div class="btn-group">
                                    <a runat="server" ID="ModuleEditLink" title="Edit Activity" class="p-1 dropdown-toggle" data-bs-toggle="dropdown" data-popper-strategy="fixed" aria-haspopup="true" aria-expanded="false"><i class="far fa-plus-circle"></i></a>
                                    <ul class="dropdown-menu dropdown-menu-right">
                                        <li><asp:HyperLink runat="server" ID="LessonCreateLink" ToolTip="New Lesson" Text="<i class='far fa-chalkboard-teacher me-1'></i> New Lesson" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="AssessmentCreateLink" ToolTip="New Assessment" Text="<i class='far fa-balance-scale me-1'></i> New Assessment" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="SurveyCreateLink" ToolTip="New Form" Text="<i class='far fa-check-square me-1'></i> New Form" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="DocumentCreateLink" ToolTip="New Document" Text="<i class='far fa-file-pdf me-1'></i> New Document" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="LinkCreateLink" ToolTip="New Link" Text="<i class='far fa-link me-1'></i> New Link" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="VideoCreateLink" ToolTip="New Video" Text="<i class='far fa-video me-1'></i> New Video" CssClass="dropdown-item" /></li>
                                        <li><asp:HyperLink runat="server" ID="QuizCreateLink" ToolTip="New Single-Question Quiz" Text="<i class='far fa-square-question me-1'></i> New Single-Question Quiz" CssClass="dropdown-item" /></li>
                                    </ul>
                                </div>
                                <a runat="server" ID="ModuleDeleteLink" title="Delete Module" class="p-1 align-middle"><i class="far fa-trash-alt"></i></a>
                            </div>
                        </div>
                    </div>

                    <asp:Repeater runat="server" ID="ActivityRepeater">
                        <HeaderTemplate>
                            <ul class="tree-view">
                        </HeaderTemplate>
                        <FooterTemplate>
                            </ul>
                        </FooterTemplate>
                        <ItemTemplate>
                            <asp:Literal runat="server" ID="TaskID" Text='<%# Eval("Identifier") %>' Visible="false" />

                            <li class='item-activity <%# (Guid)Eval("Identifier") == ActivityIdentifier ? "selected" : "" %>' data-key="<%# Eval("Identifier") %>">
                                <div>
                                    <div>
                                        <div class="node-title">
                                            <i class='<%# "me-1 far fa-" + GetTypeIconName((string)Eval("Type")) %>'></i>
                                            <span runat="server" visible='<%# ShowMetadataChecked && Eval("Code") != null %>' class="badge bg-info">
                                                <%# Eval("Code") %>
                                            </span>
                                            <span class='text'><%# Eval("Name") %></span>
                                            <div style="padding-left:30px;"><%# CreateMetadataHtml(Eval("Identifier")) %></div>
                                        </div>

                                        <div class="node-inputs">
                                            <a runat="server" ID="ActivityEditLink" title="Edit Activity" class="p-1"><i class="far fa-pencil-alt"></i></a>
                                            <a runat="server" ID="ActivityDeleteLink" title="Delete Activity" class="p-1"><i class="far fa-trash-alt"></i></a>
                                        </div>
                                    </div>
                                </div>
                            </li>
                        </ItemTemplate>
                    </asp:Repeater>

                </li>
            </ItemTemplate>
        </asp:Repeater>

        <li style="background-color:#fff;" data-key="-1">
            <div>
                <div>
                    <div class="node-title">
                        <div class='text'>
                            <div style="color:#808B96;">
                                <strong>New Module</strong>
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="ModuleCommandsContainer" class="node-inputs">
                        <div class="btn-group" style="margin-right:32px;">
                            <a runat="server" ID="ModuleEditLink" title="New Module and Activity" class="dropdown-toggle p-1" data-bs-toggle="dropdown" data-popper-strategy="fixed" aria-haspopup="true" aria-expanded="false"><i class="far fa-plus-circle"></i></a>
                            <ul class="dropdown-menu dropdown-menu-right">

                                <li><asp:HyperLink runat="server" ID="ModuleLessonCreateLink" ToolTip="New Lesson" Text="<i class='far fa-chalkboard-teacher me-1'></i> New Lesson" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleAssessmentCreateLink" ToolTip="New Assessment" Text="<i class='far fa-balance-scale me-1'></i> New Assessment" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleSurveyCreateLink" ToolTip="New Form" Text="<i class='far fa-check-square me-1'></i> New Form" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleDocumentCreateLink" ToolTip="New Document" Text="<i class='far fa-file-pdf me-1'></i> New Document" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleLinkCreateLink" ToolTip="New Link" Text="<i class='far fa-link me-1'></i> New Link" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleVideoCreateLink" ToolTip="New Video" Text="<i class='far fa-video me-1'></i> New Video" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="ModuleQuizCreateLink" ToolTip="New Single-Question Quiz" Text="<i class='far fa-square-question me-1'></i> New Single-Question Quiz" CssClass="dropdown-item" /></li>

                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </li>
        <li runat="server" id="UnitCommandWrapper" style="background-color:#fff;" data-key="-2">
            <div>
                <div>
                    <div class="node-title">
                        <div class='text'>
                            <div style="color:#808B96;">
                                <strong>New Unit</strong>
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="UnitCommandsContainer" class="node-inputs">
                        <div class="btn-group" style="margin-right:32px;">
                            <a runat="server" ID="UnitEditLink" title="New Unit, Module, and Activity" class="dropdown-toggle p-1" data-bs-toggle="dropdown" data-popper-strategy="fixed" aria-haspopup="true" aria-expanded="false"><i class="far fa-plus-circle"></i></a>
                            <ul class="dropdown-menu dropdown-menu-right">

                                <li><asp:HyperLink runat="server" ID="UnitLessonCreateLink" ToolTip="New Lesson" Text="<i class='far fa-chalkboard-teacher me-1'></i> New Lesson" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitAssessmentCreateLink" ToolTip="New Assessment" Text="<i class='far fa-balance-scale me-1'></i> New Assessment" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitSurveyCreateLink" ToolTip="New Form" Text="<i class='far fa-check-square me-1'></i> New Form" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitDocumentCreateLink" ToolTip="New Document" Text="<i class='far fa-file-pdf me-1'></i> New Document" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitLinkCreateLink" ToolTip="New Link" Text="<i class='far fa-link me-1'></i> New Link" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitVideoCreateLink" ToolTip="New Video" Text="<i class='far fa-video me-1'></i> New Video" CssClass="dropdown-item" /></li>
                                <li><asp:HyperLink runat="server" ID="UnitQuizCreateLink" ToolTip="New Single-Question Quiz" Text="<i class='far fa-square-question me-1'></i> New Single-Question Quiz" CssClass="dropdown-item" /></li>

                            </ul>
                        </div>
                    </div>

                </div>
            </div>
        </li>
    </ul>

    <div class="mb-3" style="margin-top:-10px;">
        <asp:CheckBox runat="server" ID="ShowMetadata" Text="Show Details" />
    </div>

</insite:Container>

<insite:Container runat="server" ID="TreeReorderWrapper" Visible="false">
    <asp:HiddenField runat="server" ID="ReorderState" />
    <div style="margin-bottom:10px">
        <insite:SaveButton runat="server" ID="ReorderSaveButton" />
        <insite:CancelButton runat="server" ID="ReorderCancelButton" />
        <asp:HiddenField runat="server" ID="ReorderModuleState" />
        <asp:HiddenField runat="server" ID="ReorderActivityState" />
    </div>
    <ul runat="server" id="TreeReorder" class="tree-view tree-view-reorder" data-default-level="2"></ul>
</insite:Container>

<insite:PageHeadContent runat="server">
    <style type="text/css">
        .node-inputs a { cursor: pointer; }

        ul.tree-view-reorder > li > div > div,
        ul.tree-view-reorder ul.tree-view > li > div > div {
            cursor: grab;
        }

        ul.tree-view-reorder,
        ul.tree-view-reorder ul.tree-view {
            min-height: 30px;
        }

        .node-inputs .dropdown-toggle {
            text-decoration: none;
        }

            .node-inputs .dropdown-toggle::after {
                content: none;
            }

    </style>
</insite:PageHeadContent>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (function () {
            const treeView = document.getElementById('<%= Tree.ClientID %>');
            if (!treeView)
                return;

            const options = {
                expand: '#<%= ExpandAllButton.ClientID %>',
                collapse: '#<%= CollapseAllButton.ClientID %>',
                state: 'admin.courses.outlines.controls.moduletreeview.<%= CourseIdentifier %>.<%= UnitIdentifier %>'
            };

            if (typeof window.localStorage.getItem(options.state) !== 'string')
                options.defaultLevel = 2;

            inSite.common.treeView.init(treeView, options);

            highlightActiveNode();

            function showActiveNode() {
                const activityNode = treeView.querySelector('[data-key="<%= ActivityIdentifier %>"]');
                if (activityNode) {
                    const moduleNode = activityNode.closest('li.item-module');
                    if (moduleNode && !moduleNode.classList.contains('opened'))
                        moduleNode.querySelector('.toggle-button').click();
                }
            }

            function highlightActiveNode() {
                const activityNode = treeView.querySelector('[data-key="<%= ActivityIdentifier %>"]');
                if (activityNode) {
                    const moduleNode = activityNode.closest('li.item-module');
                    if (moduleNode)
                        moduleNode.classList.add('selected');
                }
            }
        })();

        (function () {
            var $state = $('#<%= ReorderState.ClientID %>');
            if ($state.length != 1)
                return;

            var state = $state.val();
            if (!state)
                return;

            state = JSON.parse(state);

            var startAddress = null;

            init(state);

            function init(state) {
                var classPrefix = 'tree-reorder-' + String((new Date()).getTime());
                var maxDepth = -1;

                var $tree = $('#<%= TreeReorder.ClientID %>');

                bindTree($tree, state, 0);
                
                for (var depth = 0; depth < maxDepth; depth++) {
                    var currentLevelClass = getDepthClass(depth);
                    var nextLevelClass = getDepthClass(depth + 1);
                    $('ul.' + currentLevelClass + ' > li').each(function () {
                        var $item = $(this);
                        if ($item.find('> ul.tree-view').length == 0)
                            $item.append($('<ul class="tree-view">').addClass(nextLevelClass));
                    });
                }

                for (var depth = maxDepth; depth >= 0; depth--) {
                    var selector = 'ul.' + getDepthClass(depth);

                    $(selector).sortable({
                        items: '> li',
                        connectWith: selector,
                        containment: 'document',
                        cursor: 'grabbing',
                        forceHelperSize: true,
                        axis: 'y',
                        opacity: 0.65,
                        tolerance: 'pointer',
                        dropOnEmpty: true,
                        start: onReorderStart,
                        update: onReorderUpdate
                    }).disableSelection();
                }

                function getDepthClass(depth) {
                    return classPrefix + '-' + String(depth);
                }

                function bindTree($tree, items, depth) {
                    if (maxDepth < depth)
                        maxDepth = depth;

                    $tree.addClass(getDepthClass(depth)).data('depth', depth);

                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        var $item = $('<li class="opened">').data('id', item.id).append(
                            $('<div>').append(function () {
                                var $container = $('<div>');

                                if (depth > 0)
                                    $container.css('margin-left', String(depth * 20) + 'px');

                                $container.append(function () {
                                    var $container = $('<div class="node-title">');

                                    if (item.icon)
                                        $container.append(
                                            $('<i class="me-1 far">')
                                                .addClass('fa-' + String(item.icon))
                                        );

                                    return $container.append(
                                        $('<span class="text">')
                                            .text(item.text)
                                    );
                                });

                                return $container;
                            })
                        );

                        if (item.items && item.items.length > 0) {
                            var $subtree = $('<ul class="tree-view">');

                            bindTree($subtree, item.items, depth + 1);

                            $item.append($subtree);
                        }

                        $item.on('mouseenter', onItemMouseEnter).on('mouseleave', onItemMouseLeave);

                        $tree.append($item);
                    }
                }
            }

            function onItemMouseEnter(e) {
                e.stopPropagation();

                $(this).addClass('highlighted')
                    .parentsUntil('ul.tree-view-reorder', 'li')
                    .each(function () {
                        $(this).removeClass('highlighted');
                    });
            }

            function onItemMouseLeave(e) {
                var $parent = $(this).removeClass('highlighted').parent();
                while ($parent.length == 1) {
                    if ($parent.prop("tagName") == 'LI') {
                        $parent.addClass('highlighted')
                        break;
                    } else if ($parent.prop("tagName") == 'UL' && $parent.hasClass('tree-view-reorder')) {
                        break;
                    }

                    $parent = $parent.parent();
                }
            }

            function onReorderStart(e, ui) {
                startAddress = getAddress(ui.item);

                return inSite.common.gridReorderHelper.onSortStart.apply(this, arguments);
            }

            function onReorderUpdate(e, ui) {
                if (!startAddress)
                    return;

                var sourceAddress = startAddress;

                startAddress = null;

                if (sourceAddress.index < 0)
                    return;

                var destinationAddress = getAddress(ui.item);
                if (destinationAddress.index < 0)
                    return;

                var sourceItems = getItemsByPath(sourceAddress.path);
                if (sourceItems == null || sourceAddress.index >= sourceItems.length || sourceItems[sourceAddress.index].id !== ui.item.data('id'))
                    return;

                var destinationItems = getItemsByPath(destinationAddress.path);
                if (destinationItems == null || destinationAddress.index > destinationItems.length)
                    return;

                destinationItems.splice(destinationAddress.index, 0, sourceItems.splice(sourceAddress.index, 1)[0]);

                $state.val(JSON.stringify(state));
            }

            function getAddress($item) {
                var result = {
                    path: [],
                    index: -1
                };

                $item.parentsUntil('ul.tree-view-reorder', 'li').each(function () {
                    result.path.push($(this).data('id'));
                }).end().parent().find('> li').each(function () {
                    var $this = $(this);
                    if ($this.data('id')) {
                        result.index++;
                        if ($this.is($item))
                            return false;
                    }
                });

                return result;
            }

            function getItemsByPath(path) {
                var items = state;

                path = path.slice();

                while (path.length > 0) {
                    var id = path.pop();

                    for (var i = 0; i < items.length; i++) {
                        var item = items[i];
                        if (item.id == id) {
                            if (items = item.items)
                                id = null;

                            break;
                        }
                    }

                    if (id !== null)
                        return null;
                }

                return items;
            }
        })();
    </script>
</insite:PageFooterContent>