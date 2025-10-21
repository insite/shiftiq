(function () {
    let instance = null;
    {
        const inSite = window.inSite = window.inSite || {};
        const common = inSite.common = inSite.common || {};

        if (common.treeView)
            return;

        instance = common.treeView = Object.freeze({
            init: function (id, options) {
                return init(id, options);
            },
            filter: function (id, match) {
                filter(id, match);
            },
            getAllTreeItems(id) {
                const data = getTreeRootData(id);
                return data?.allItems;
            },
            getItemPath(id) {
                const element = getElement(id);
                const data = element[dataId];

                return data instanceof ItemData ? data.path : null;
            }
        });
    }

    const itemType = Object.freeze({
        root: 0,
        item: 1,
        subtree: 2,
        toggle: 3
    });

    class StateData {
        #key;
        #loaded;
        #value;
        #input;

        constructor(key) {
            this.#key = key;
            this.#loaded = false;
            this.#value = {};

            this.#input = getElement(key);
            if (this.#input && this.#input.tagName != 'INPUT')
                this.#input = null;
        }

        get loaded() {
            return this.#loaded;
        }

        load() {
            this.#loaded = false;
            this.#value = {};

            const value = this.#input ? this.#input.value : window.localStorage.getItem(this.#key);
            if (typeof value != 'string' || value.length == 0)
                return;

            const values = value.split(',');
            for (let i = 0; i < values.length; i++) {
                const id = values[i];
                if (id != null && id.length > 0) {
                    this.#value[id] = true;
                    this.#loaded = true;
                }
            }
        }

        save() {
            let value = [];

            for (let item in this.#value)
                value.push(item);

            value = value.join(',');

            if (this.#input)
                this.#input.value = value;
            else
                window.localStorage.setItem(this.#key, value);
        }

        restore(root) {
            this.#restoreTreeState(root, '');
        }

        toggle(path, opened) {
            if (opened) {
                if (path != null && !this.#value.hasOwnProperty(path))
                    this.#value[path] = true;
            } else {
                if (path != null && this.#value.hasOwnProperty(path))
                    delete this.#value[path];
            }
        }

        #restoreTreeState(tree, path) {
            const treeData = tree[dataId];
            for (let item of treeData.items) {
                const itemData = item[dataId];

                const subTree = itemData.subTree;
                if (!subTree)
                    continue;

                const key = item.dataset.key;
                const itemPath = path.length == 0 ? key : path + '.' + key;

                toggleItem(item, this.#value.hasOwnProperty(itemPath));

                this.#restoreTreeState(subTree, itemPath);
            }
        }
    }

    class BaseData {
        #type;

        constructor(type) {
            this.#type = type;
        }

        get type() {
            return this.#type;
        }
    }

    class RootData extends BaseData {
        #items;
        #allItems;
        #expand;
        #collapse;
        #level;
        #state;
        #depth;

        constructor(root, options) {
            super(itemType.root);

            this.#items = root.querySelectorAll(':scope > li');
            this.#allItems = root.querySelectorAll(':scope > li, ul.tree-view > li');
            this.#expand = tryInitOption(root, options, 'expand', tryInitHtmlElement);
            this.#collapse = tryInitOption(root, options, 'collapse', tryInitHtmlElement);
            this.#level = tryInitOption(root, options, 'level', tryInitHtmlElement);
            this.#state = tryInitOption(root, options, 'state', (v) => typeof v == 'string' && v.length > 0 ? new StateData(v) : null);
            this.#depth = -1;
        }

        get items() {
            return this.#items;
        }

        get allItems() {
            return this.#allItems;
        }

        get expand() {
            return this.#expand;
        }

        get collapse() {
            return this.#collapse;
        }

        get level() {
            return this.#level;
        }

        get state() {
            return this.#state;
        }

        get depth() {
            return this.#depth;
        }

        updateHierarchy() {
            const path = [];
            const hasState = this.#state != null;
            for (let item of this.#items)
                item[dataId].updateHierarchy(path, hasState);

            this.#depth = -1;
            for (let item of this.#allItems) {
                const itemDepth = item[dataId].depth;
                if (this.#depth < itemDepth)
                    this.#depth = itemDepth;
            }
        }
    }

    class ItemData extends BaseData {
        #root;
        #item;
        #depth;
        #path;
        #container;
        #subTree;
        #toggle;

        constructor(root, item) {
            super(itemType.item);

            this.#root = root;
            this.#item = item;
            this.#depth = -1;
            this.#path = null;
            this.#container = item.querySelector(':scope > div > div');
            this.#subTree = item.querySelector(':scope > ul.tree-view');

            if (this.#subTree) {
                this.#subTree[dataId] = new SubTreeData(root, item, this.#subTree);

                const template = toggleTemplate.content.cloneNode(true);
                const button = this.#toggle = template.firstChild;
                button[dataId] = new ToggleData(root, item);
            } else {
                this.#toggle = null;
            }
        }

        get root() {
            return this.#root;
        }

        get depth() {
            return this.#depth;
        }

        get path() {
            return this.#path;
        }

        get container() {
            return this.#container;
        }

        get subTree() {
            return this.#subTree;
        }

        get toggle() {
            return this.#toggle;
        }

        updateHierarchy(path, hasState) {
            if (hasState) {
                const key = this.#item.dataset.key;

                if (!key)
                    throw 'Tree view initialization error: item has no key';

                path.push(key);

                this.#path = path.join('.');
            } else {
                path.push(0);

                this.#path = null;
            }

            this.#depth = path.length;
            this.#container.style.marginLeft = 'calc(' + String(this.#depth - 1) + ' * var(--in-tree-view-inner-offset))';

            if (this.#subTree) {
                const treeData = this.#subTree[dataId];
                for (let item of treeData.items)
                    item[dataId].updateHierarchy(path, hasState);
            }

            path.pop();
        }
    }

    class SubTreeData extends BaseData {
        #root;
        #parentItem;
        #items;

        constructor(root, item, tree) {
            super(itemType.subtree);

            this.#root = root;
            this.#parentItem = item;
            this.#items = tree.querySelectorAll(':scope > li');
        }

        get root() {
            return this.#root;
        }

        get parentItem() {
            return this.#parentItem;
        }

        get items() {
            return this.#items;
        }
    }

    class ToggleData extends BaseData {
        #root;
        #item;

        constructor(root, item) {
            super(itemType.toggle);

            this.#root = root;
            this.#item = item;
        }

        get root() {
            return this.#root;
        }

        get item() {
            return this.#item;
        }
    }

    const dataId = 'itv' + String(Date.now());
    const toggleTemplate = createTemplate('<span class="toggle-button"><i class="far fa-plus"></i><i class="far fa-minus"></i></span>');

    if (document.readyState !== 'loading')
        onDocLoad();
    else
        document.addEventListener('DOMContentLoaded', onDocLoad);

    function onDocLoad() {
        if (Sys?.Application)
            Sys.Application.add_load(onLoad);

        onLoad();
    }

    function onLoad() {
        const trees = document.querySelectorAll('ul.tree-view');
        const roots = [];

        for (let tree of trees) {
            if (tree[dataId])
                continue;

            const root = getTreeRootElement(tree);

            let isDuplicate = false;
            for (let r of roots) {
                if (isDuplicate = r == root)
                    break;
            }

            if (!isDuplicate)
                roots.push(root);
        }

        for (let root of roots) {
            if (root.dataset.init != 'code' && !root[dataId])
                init(root);
        }
    }

    // initialization

    function init(id, options) {
        const root = getTreeRootElement(id);
        if (!root || typeof root[dataId] != 'undefined')
            return false;

        const treeData = root[dataId] = new RootData(root, options);
        const defaultLevel = tryInitOption(root, options, 'defaultLevel', (v) => {
            if (v == 'all')
                return Number.MAX_SAFE_INTEGER;

            const result = parseInt(v);
            return isNaN(result) || result <= 0 ? 1 : result;
        });

        if (treeData.expand) {
            treeData.expand[dataId] = root;
            treeData.expand.removeEventListener('click', onExpandAll);
            treeData.expand.addEventListener('click', onExpandAll);
        }

        if (treeData.collapse) {
            treeData.collapse[dataId] = root;
            treeData.collapse.removeEventListener('click', onCollapseAll);
            treeData.collapse.addEventListener('click', onCollapseAll);
        }

        if (treeData.level) {
            treeData.level[dataId] = root;
            treeData.level.removeEventListener('change', onLevelChanged);
            treeData.level.addEventListener('change', onLevelChanged);
        }

        initTreeView(root, defaultLevel);

        return true;
    }

    function tryInitOption(element, options, name, action) {
        let value = options ? options[name] : null;
        if (!value)
            value = element.dataset[name];

        return !value ? null : action ? action(value) : value;
    }

    function tryInitHtmlElement(id) {
        const el = getElement(id);
        return el instanceof HTMLElement && typeof el[dataId] == 'undefined' ? el : null;
    }

    function initTreeView(root, defaultLevel) {
        const rootData = root[dataId];

        for (let item of rootData.allItems) {
            const itemData = item[dataId] = new ItemData(root, item);
            if (itemData.toggle) {
                itemData.toggle.addEventListener('click', onToggle);
                itemData.container.insertBefore(itemData.toggle, itemData.container.firstChild);
            }
        }

        rootData.updateHierarchy();

        if (rootData.state != null) {
            rootData.state.load();

            if (!rootData.state.loaded) {
                expand(root, defaultLevel);
            } else {
                rootData.state.restore(root);
            }
        } else {
            expand(root, defaultLevel);
        }

        const treeDepth = rootData.depth;

        if (treeDepth > 1) {
            rootData.expand?.classList.remove('d-none');
            rootData.collapse?.classList.remove('d-none');

            if (rootData.level) {
                let value = parseInt(rootData.level.value);
                if (isNaN(value) || value < 1)
                    value = defaultLevel;
                else if (value > treeDepth)
                    value = treeDepth;

                rootData.level.replaceChildren();

                for (let num = 1; num <= treeDepth; num++) {
                    const numStr = String(num);
                    const option = new Option('Tier ' + numStr, numStr);

                    rootData.level.appendChild(option);

                    if (value === num)
                        option.selected = true;
                }

                const $level = $(rootData.level);
                $level.selectpicker('show');
                $level.selectpicker('refresh');
            }
        } else {
            rootData.expand?.classList.add('d-none');
            rootData.collapse?.classList.add('d-none');

            if (rootData.level) {
                const $level = $(rootData.level);
                $level.selectpicker('hide');
            }
        }
    }

    // event handlers

    function onToggle() {
        const data = this[dataId];
        toggle(data.root, data.item, !data.item.classList.contains('opened'));
        data.root[dataId].state?.save();
    }

    function onLevelChanged(e) {
        const root = this[dataId];
        const rootData = root[dataId];

        let level = parseInt($(rootData.level).selectpicker('val'));
        if (isNaN(level))
            level = -1;

        expand(root, level);
    }

    function onExpandAll(e) {
        e.preventDefault();

        const root = this[dataId];
        const rootData = root[dataId];

        if (rootData.level && rootData.level.options.length > 0) {
            const lastValue = rootData.level.options[rootData.level.options.length - 1].value;
            $(rootData.level).selectpicker('val', lastValue);
            onLevelChanged.call(this, e);
        } else {
            expand(root, Number.MAX_SAFE_INTEGER);
        }
    }

    function onCollapseAll(e) {
        e.preventDefault();

        const root = this[dataId];
        const rootData = root[dataId];

        if (rootData.level && rootData.level.options.length > 0) {
            const firstValue = rootData.level.options[0].value;
            $(rootData.level).selectpicker('val', firstValue);
            onLevelChanged.call(this, e);
        } else {
            collapse(root);
        }
    }

    // node collapse/expand

    function expand(root, level) {
        const rootData = root[dataId];

        for (let item of rootData.allItems) {
            const itemData = item[dataId];
            toggle(root, item, itemData.depth < level);
        }

        rootData.state?.save();
    }

    function collapse(root) {
        const rootData = root[dataId];

        for (let item of rootData.allItems)
            toggle(root, item, false);

        rootData.state?.save();
    }

    function toggle(root, item, opened) {
        const itemData = item[dataId];
        if (!itemData.toggle)
            return;

        toggleItem(item, opened);

        const itemPath = itemData.path;
        if (!itemPath)
            return;

        const state = root[dataId].state;
        state.toggle(itemPath, opened);
    }

    function toggleItem(item, opened) {
        if (opened) {
            if (!item.classList.replace('closed', 'opened'))
                item.classList.add('opened')
        } else {
            if (!item.classList.replace('opened', 'closed'))
                item.classList.add('closed')
        }
    }

    // helpers

    function getElement(id) {
        const idType = typeof id;

        if (idType === 'string') {
            if (id.length > 0)
                return id[0] === '#' ? document.querySelector(id) : document.getElementById(id);
        } else if (idType === 'object') {
            if (id instanceof HTMLElement)
                return id;
            else if ((id instanceof jQuery || id instanceof NodeList) && id.length == 1)
                return id[0];
        }

        return null;
    }

    function createTemplate(html) {
        const result = document.createElement('template');
        result.innerHTML = html;
        return result;
    }

    function getRoot(el, selector) {
        let result = null;
        let current = el;

        do {
            if (current.matches(selector))
                result = current;
        } while (current = current.parentElement);

        return result;
    }

    function getTreeRootElement(id) {
        const element = getElement(id);
        return element instanceof HTMLElement ? getRoot(element, 'ul.tree-view') : null;
    }

    function getTreeRootData(id) {
        const root = getTreeRootElement(id);
        return root ? root[dataId] : null;
    }

    function filter(id, match) {
        const root = getTreeRootElement(id);
        if (!root)
            return;

        const data = root[dataId];
        if (!data)
            return;

        {
            for (let item of data.allItems)
                item.classList.remove('match-search', 'match-contains', 'top-item', 'bottom-item');

            if (!match) {
                root.classList.remove('search-mode');
                return;
            }

            root.classList.add('search-mode');

            const matches = [];

            for (let item of data.allItems) {
                if (match(item)) {
                    item.classList.add('match-search');
                    matches.push(item);
                }
            }

            for (let item of matches) {
                let ancestor = item.parentElement;
                while (ancestor != root) {
                    if (ancestor.tagName == 'LI') {
                        if (ancestor.classList.contains('match-contains'))
                            break;

                        ancestor.classList.add('match-contains')
                    }

                    ancestor = ancestor.parentElement;
                }
            }
        }

        {
            const trees = Array.from(root.querySelectorAll('li > ul.tree-view'));
            trees.push(root);

            for (let t of trees) {
                const items = t.querySelectorAll(':scope > li.match-contains, :scope > li.match-search');
                if (items.length == 0)
                    continue;

                items[0].classList.add('top-item');
                items[items.length - 1].classList.add('bottom-item');
            }
        }
    }
})();