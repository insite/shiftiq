(function () {
    if (inSite.common.adminNavigationSearch)
        return;

    const instance = inSite.common.adminNavigationSearch = Object.freeze({
        init: init
    });

    const searchDelayMs = 300;
    const scrollBottomThresholdPx = 1;

    const objectTypes = Object.freeze({
        Person: Object.freeze({ iconName: 'user', className: 'bg-info' }),
        Group: Object.freeze({ iconName: 'users', className: 'bg-success' }),
        Bank: Object.freeze({ iconName: 'balance-scale', className: 'bg-success' }),
        Course: Object.freeze({ iconName: 'users-class', className: 'bg-success' }),
        Event: Object.freeze({ iconName: 'calendar-alt', className: 'bg-success' }),
        Form: Object.freeze({ iconName: "check-square", className: "bg-success" }),
        Achievement: Object.freeze({ iconName: "trophy", className: "bg-success" }),
        Gradebook: Object.freeze({ iconName: "spell-check", className: "bg-success" }),
        Logbook: Object.freeze({ iconName: "pencil-ruler", className: "bg-success" }),
        Program: Object.freeze({ iconName: "graduation-cap", className: "bg-success" }),
        Standard: Object.freeze({ iconName: "ruler-triangle", className: "bg-success" }),
        PlatformApp: Object.freeze({ iconName: 'toolbox', className: 'bg-primary', name: 'Platform App' }),
        App: Object.freeze({ iconName: 'location', className: 'bg-secondary2' })
    });

    const defaultObjectType = Object.freeze({ iconName: 'question', className: 'bg-warning' });

    function init(containerId, searchUrl) {
        const container = document.getElementById(containerId);
        if (!container || container._adminNavigationSearch)
            return;

        const input = container.querySelector('input[role="combobox"]');
        if (!input || typeof searchUrl !== 'string' || searchUrl.length === 0)
            return;

        const state = container._adminNavigationSearch = {
            container: container,
            input: input,
            searchUrl: searchUrl,
            resultsId: input.getAttribute('aria-controls'),
            resultsPanel: null,
            results: [],
            hasCompletedSearch: false,
            selectedIndex: 0,
            isOpen: false,
            loadedQuery: '',
            pageIndex: -1,
            hasMoreResults: false,
            isLoadingNextPage: false,
            hasFocusWithin: false,
            isDismissed: false,
            searchGeneration: 0,
            nextPageRequest: null,
            searchTimer: null,
            helpContainer: null,
            helpButton: null,
            helpPanel: null
        };

        createHelpButton(state);

        input.addEventListener('input', function () { onInput(state); });
        input.addEventListener('keydown', function (event) { onKeyDown(state, event); });
        container.addEventListener('focusin', function () { onFocusIn(state); });
        container.addEventListener('focusout', function (event) { onFocusOut(state, event); });
        document.addEventListener('mousedown', function (event) { onDocumentMouseDown(state, event); });
        document.addEventListener('click', function (event) { onHelpDocumentClick(state, event); });
        document.addEventListener('focusin', function (event) { onHelpDocumentFocusIn(state, event); });
        document.addEventListener('keydown', function (event) { onHelpDocumentKeyDown(state, event); });
    }

    function createHelpButton(state) {
        const helpContainer = document.createElement('div');
        helpContainer.className = 'admin-navigation-search-help d-none d-lg-block ms-2';

        const button = document.createElement('button');
        button.type = 'button';
        button.title = 'Click to see help';
        button.className = 'btn btn-link m-0 p-0 text-decoration-none search-help-button';
        button.setAttribute('aria-expanded', 'false');
        button.setAttribute('aria-controls', state.container.id + '-help');
        button.setAttribute('aria-haspopup', 'dialog');

        const icon = document.createElement('i');
        icon.className = 'fa-solid fa-circle-info text-secondary';
        icon.setAttribute('aria-hidden', 'true');
        button.appendChild(icon);

        button.addEventListener('click', function () { toggleHelp(state); });
        helpContainer.appendChild(button);
        state.container.parentNode.insertBefore(helpContainer, state.container.nextSibling);

        state.helpContainer = helpContainer;
        state.helpButton = button;
    }

    function toggleHelp(state) {
        if (state.helpPanel) {
            closeHelp(state, false);
            return;
        }

        openHelp(state);
    }

    function openHelp(state) {
        const panel = document.createElement('div');
        panel.id = state.container.id + '-help';
        panel.className = 'search-help-popup shadow-lg';
        panel.setAttribute('role', 'dialog');
        panel.setAttribute('aria-label', 'Global Search help');
        panel.tabIndex = -1;

        const content = document.createElement('div');
        content.className = 'm-2';

        const heading = document.createElement('b');
        heading.textContent = 'Global Search';

        const description = document.createElement('div');
        description.className = 'mt-2 mb-2';
        description.textContent = 'Search using keywords to find your Apps and manage your content and contacts across Shift iQ.';

        const examples = document.createElement('div');
        examples.appendChild(document.createTextNode('Combine words to find more specific results, such as '));
        examples.appendChild(createHelpExample('create event'));
        examples.appendChild(document.createTextNode(', '));
        examples.appendChild(createHelpExample('view gradebooks'));
        examples.appendChild(document.createTextNode(', a course or assessment name, or a contact\u2019s first and last name.'));

        content.appendChild(heading);
        content.appendChild(description);
        content.appendChild(examples);
        panel.appendChild(content);
        state.helpContainer.appendChild(panel);
        state.helpPanel = panel;
        state.helpButton.setAttribute('aria-expanded', 'true');

        positionHelpPanel(state);
        panel.focus();

        window.requestAnimationFrame(function () {
            if (state.helpPanel === panel)
                panel.dataset.visible = 'true';
        });
    }

    function createHelpExample(text) {
        const example = document.createElement('i');
        example.className = 'bg-secondary px-2';
        example.textContent = text;
        return example;
    }

    function positionHelpPanel(state) {
        const panel = state.helpPanel;
        const button = state.helpButton;
        if (!panel || !button)
            return;

        const rect = button.getBoundingClientRect();
        const style = window.getComputedStyle(button);
        const marginTop = parseFloat(style.marginTop) || 0;
        const marginBottom = parseFloat(style.marginBottom) || 0;

        panel.style.top = '';
        panel.style.bottom = '';

        if (rect.top > window.innerHeight - rect.top)
            panel.style.bottom = rect.height + marginBottom + 'px';
        else
            panel.style.top = rect.height + marginTop + 'px';

        const panelRect = panel.getBoundingClientRect();
        const viewportMargin = 16;
        if (panelRect.right > window.innerWidth - viewportMargin)
            panel.style.left = -(panelRect.right - window.innerWidth + viewportMargin) + 'px';

        const adjustedRect = panel.getBoundingClientRect();
        if (adjustedRect.left < viewportMargin)
            panel.style.left = parseFloat(panel.style.left || '0') + viewportMargin - adjustedRect.left + 'px';
    }

    function closeHelp(state, restoreFocus) {
        if (!state.helpPanel)
            return;

        state.helpPanel.remove();
        state.helpPanel = null;
        state.helpButton.setAttribute('aria-expanded', 'false');

        if (restoreFocus)
            state.helpButton.focus();
    }

    function onHelpDocumentClick(state, event) {
        if (!state.helpPanel || !(event.target instanceof Node))
            return;

        if (state.helpButton.contains(event.target) || state.helpPanel.contains(event.target))
            return;

        closeHelp(state, false);
    }

    function onHelpDocumentFocusIn(state, event) {
        if (!state.helpPanel || !(event.target instanceof Node))
            return;

        if (state.helpButton.contains(event.target) || state.helpPanel.contains(event.target))
            return;

        closeHelp(state, false);
    }

    function onHelpDocumentKeyDown(state, event) {
        if (!state.helpPanel || event.key !== 'Escape')
            return;

        event.preventDefault();
        closeHelp(state, true);
    }

    function onInput(state) {
        const query = state.input.value.trim();

        state.searchGeneration += 1;
        state.nextPageRequest = null;
        state.isDismissed = false;
        state.selectedIndex = 0;
        state.loadedQuery = '';
        state.pageIndex = -1;
        state.hasMoreResults = false;
        state.isLoadingNextPage = false;

        if (state.searchTimer !== null) {
            clearTimeout(state.searchTimer);
            state.searchTimer = null;
        }

        updateLoadingState(state);
        updateSelection(state);

        if (!query) {
            state.results = [];
            state.hasCompletedSearch = false;
            setOpen(state, false);
            renderResults(state);
            return;
        }

        if (state.hasCompletedSearch && state.hasFocusWithin)
            setOpen(state, true);

        const searchGeneration = state.searchGeneration;
        state.searchTimer = setTimeout(function () {
            state.searchTimer = null;
            loadFirstPage(state, query, searchGeneration);
        }, searchDelayMs);
    }

    async function loadFirstPage(state, query, searchGeneration) {
        try {
            const response = await search(state, query, 1);
            if (searchGeneration !== state.searchGeneration)
                return;

            const results = response.rows;
            state.results = results;
            state.loadedQuery = query;
            state.pageIndex = response.pageIndex;
            state.hasMoreResults = canLoadMore(response, results.length);
            state.hasCompletedSearch = true;
            state.selectedIndex = 0;
            state.isOpen = state.hasFocusWithin && !state.isDismissed;

            renderResults(state);
            updateOpenState(state);

            if (state.isOpen && state.results.length > 0)
                scrollSelectedOption(state, false);
        } catch (error) {
            if (searchGeneration !== state.searchGeneration)
                return;

            state.results = [];
            state.loadedQuery = '';
            state.pageIndex = -1;
            state.hasMoreResults = false;
            state.hasCompletedSearch = false;
            state.selectedIndex = 0;
            state.isOpen = false;
            renderResults(state);
            updateOpenState(state);
        }
    }

    async function onResultsScroll(state) {
        const panel = state.resultsPanel;
        if (!panel)
            return;

        const distanceFromBottom = panel.scrollHeight - panel.scrollTop - panel.clientHeight;
        if (distanceFromBottom > scrollBottomThresholdPx)
            return;

        const query = state.input.value.trim();
        if (!state.hasCompletedSearch
            || !query
            || state.loadedQuery !== query
            || !state.hasMoreResults
            || state.nextPageRequest !== null)
        {
            return;
        }

        const requestToken = {};
        const searchGeneration = state.searchGeneration;
        const nextPageIndex = state.pageIndex + 1;
        state.nextPageRequest = requestToken;
        state.isLoadingNextPage = true;
        updateLoadingState(state);

        try {
            const response = await search(state, query, nextPageIndex + 1);
            if (state.nextPageRequest !== requestToken || searchGeneration !== state.searchGeneration)
                return;

            const scrollTop = panel.scrollTop;
            const nextResults = response.rows;
            const loadedResultCount = state.results.length + nextResults.length;

            state.results = state.results.concat(nextResults);
            state.pageIndex = response.pageIndex;
            state.hasMoreResults = canLoadMore(response, loadedResultCount);
            state.isLoadingNextPage = false;

            renderResults(state);
            updateOpenState(state);
            if (state.resultsPanel)
                state.resultsPanel.scrollTop = scrollTop;
        } catch (error) {
            // Keep the current results and allow another bottom scroll to retry this page.
        } finally {
            if (state.nextPageRequest === requestToken) {
                state.nextPageRequest = null;
                state.isLoadingNextPage = false;
                updateLoadingState(state);
            }
        }
    }

    function onFocusIn(state) {
        state.hasFocusWithin = true;
        state.isDismissed = false;
        if (state.hasCompletedSearch)
            setOpen(state, true);
    }

    function onFocusOut(state, event) {
        if (event.relatedTarget instanceof Node && state.container.contains(event.relatedTarget))
            return;

        state.hasFocusWithin = false;
        state.isDismissed = true;
        setOpen(state, false);
    }

    function onDocumentMouseDown(state, event) {
        if (event.target instanceof Node && state.container.contains(event.target))
            return;

        state.hasFocusWithin = false;
        state.isDismissed = true;
        setOpen(state, false);
    }

    function onKeyDown(state, event) {
        if (event.key === 'Escape') {
            event.preventDefault();
            state.isDismissed = true;
            setOpen(state, false);
            return;
        }

        if (state.results.length === 0)
            return;

        let nextSelectedIndex = state.selectedIndex;

        if (event.key === 'ArrowDown') {
            event.preventDefault();
            state.isDismissed = false;
            nextSelectedIndex = Math.min(state.selectedIndex + 1, state.results.length - 1);
        } else if (event.key === 'ArrowUp') {
            event.preventDefault();
            state.isDismissed = false;
            nextSelectedIndex = Math.max(state.selectedIndex - 1, 0);
        } else if (event.key === 'Enter' && state.isOpen) {
            event.preventDefault();
            const selectedOption = getSelectedOption(state);
            const anchor = selectedOption ? selectedOption.querySelector('a') : null;
            if (anchor)
                anchor.click();
            return;
        } else {
            return;
        }

        const selectionChanged = nextSelectedIndex !== state.selectedIndex;
        state.selectedIndex = nextSelectedIndex;
        setOpen(state, true);
        updateSelection(state);

        if (selectionChanged)
            scrollSelectedOption(state, true);
    }

    async function search(state, searchText, page) {
        const url = new URL(state.searchUrl, window.location.href);
        url.searchParams.set('searchText', searchText);
        url.searchParams.set('page', String(page));

        const headers = { Accept: '*/*' };
        const overrideOrigin = getOverrideOrigin();
        if (overrideOrigin)
            headers['Override-Origin'] = overrideOrigin;

        const response = await fetch(url.toString(), {
            method: 'GET',
            headers: headers,
            credentials: 'include'
        });

        if (!response.ok) {
            redirectForAuthenticationError(response.status);
            throw new Error('Global search request failed with status ' + response.status);
        }

        const rows = await response.json();
        const paginationValue = response.headers.get('X-Query-Pagination');
        if (!paginationValue)
            throw new Error('X-Query-Pagination is empty');

        const pagination = JSON.parse(paginationValue);
        if (pagination.Page === undefined
            || pagination.PageSize === undefined
            || pagination.TotalCount === undefined)
        {
            throw new Error('X-Query-Pagination is invalid');
        }

        return {
            pageIndex: pagination.Page - 1,
            rows: Array.isArray(rows) ? rows : [],
            totalRowCount: pagination.TotalCount,
            rowsPerPage: pagination.PageSize
        };
    }

    function renderResults(state) {
        if (state.resultsPanel) {
            state.resultsPanel.remove();
            state.resultsPanel = null;
        }

        if (!state.hasCompletedSearch) {
            updateOpenState(state);
            return;
        }

        if (state.results.length === 0) {
            const panel = document.createElement('div');
            panel.id = state.resultsId;
            panel.className = 'search-results shadow-lg';
            panel.setAttribute('role', 'status');

            const message = document.createElement('div');
            message.className = 'no-results';
            message.textContent = 'No search results found';
            panel.appendChild(message);

            state.resultsPanel = panel;
            state.container.appendChild(panel);
            updateOpenState(state);
            return;
        }

        const panel = document.createElement('ul');
        panel.id = state.resultsId;
        panel.className = 'search-results shadow-lg';
        panel.setAttribute('role', 'listbox');
        panel.addEventListener('scroll', function () { onResultsScroll(state); });

        for (let index = 0; index < state.results.length; index++)
            panel.appendChild(createResultElement(state, state.results[index], index));

        state.resultsPanel = panel;
        state.container.appendChild(panel);
        updateLoadingState(state);
        updateOpenState(state);
    }

    function createResultElement(state, result, index) {
        const objectType = result.ObjectType || '';
        const objectTypeDescr = objectTypes[objectType] || defaultObjectType;
        const item = document.createElement('li');
        item.id = state.resultsId + '-' + index;
        item.className = 'search-result' + (index === state.selectedIndex ? ' active' : '');
        item.setAttribute('role', 'option');
        item.setAttribute('aria-selected', index === state.selectedIndex ? 'true' : 'false');

        const iconContainer = document.createElement('span');
        iconContainer.className = 'result-icon ' + objectTypeDescr.className;

        const icon = document.createElement('i');
        icon.className = 'fa-solid fa-' + objectTypeDescr.iconName;
        iconContainer.appendChild(icon);

        const content = document.createElement('span');
        content.className = 'result-content';

        const anchor = document.createElement('a');
        anchor.className = 'result-title';
        anchor.href = result.Url || '';
        anchor.textContent = result.Title || '';

        const subtitle = document.createElement('span');
        subtitle.className = 'result-subtitle';
        subtitle.textContent = getSubtitle(objectTypeDescr, result);

        content.appendChild(anchor);
        content.appendChild(subtitle);
        item.appendChild(iconContainer);
        item.appendChild(content);

        return item;
    }

    function getSubtitle(objectTypeDescr, result) {
        const values = [objectTypeDescr.name ?? result.ObjectType];
        if (Array.isArray(result.Attributes))
            values.push.apply(values, result.Attributes);

        return values
            .filter(function (value) { return typeof value === 'string' && value.trim().length > 0; })
            .join(' • ');
    }

    function updateSelection(state) {
        if (!state.resultsPanel)
            return;

        const options = state.resultsPanel.querySelectorAll('[role="option"]');
        for (let index = 0; index < options.length; index++) {
            const isSelected = index === state.selectedIndex;
            options[index].classList.toggle('active', isSelected);
            options[index].setAttribute('aria-selected', isSelected ? 'true' : 'false');
        }

        updateOpenState(state);
    }

    function updateLoadingState(state) {
        if (!state.resultsPanel || state.resultsPanel.getAttribute('role') !== 'listbox')
            return;

        state.resultsPanel.setAttribute('aria-busy', state.isLoadingNextPage ? 'true' : 'false');

        const current = state.resultsPanel.querySelector('.loading-results');
        if (!state.isLoadingNextPage) {
            if (current)
                current.remove();
            return;
        }

        if (current)
            return;

        const item = document.createElement('li');
        item.className = 'loading-results';
        item.setAttribute('role', 'status');

        const spinner = document.createElement('span');
        spinner.className = 'spinner-border spinner-border-sm';
        spinner.setAttribute('aria-hidden', 'true');

        const text = document.createElement('span');
        text.textContent = 'Loading more results...';

        item.appendChild(spinner);
        item.appendChild(text);
        state.resultsPanel.appendChild(item);
    }

    function setOpen(state, isOpen) {
        state.isOpen = isOpen && state.hasCompletedSearch;
        updateOpenState(state);
    }

    function updateOpenState(state) {
        state.input.setAttribute('aria-expanded', state.isOpen ? 'true' : 'false');

        if (state.isOpen && state.results.length > 0)
            state.input.setAttribute('aria-activedescendant', state.resultsId + '-' + state.selectedIndex);
        else
            state.input.removeAttribute('aria-activedescendant');

        if (state.resultsPanel)
            state.resultsPanel.hidden = !state.isOpen;
    }

    function getSelectedOption(state) {
        if (!state.resultsPanel)
            return null;

        return state.resultsPanel.querySelector('#' + state.resultsId + '-' + state.selectedIndex);
    }

    function scrollSelectedOption(state, isKeyboardNavigation) {
        if (!state.isOpen || !state.resultsPanel)
            return;

        const selectedOption = getSelectedOption(state);
        if (!selectedOption)
            return;

        if (isKeyboardNavigation && state.selectedIndex === 0) {
            state.resultsPanel.scrollTop = 0;
        } else if (isKeyboardNavigation && state.selectedIndex === state.results.length - 1) {
            state.resultsPanel.scrollTop = Math.max(
                0,
                state.resultsPanel.scrollHeight - state.resultsPanel.clientHeight
            );
        } else if (selectedOption.scrollIntoView) {
            selectedOption.scrollIntoView({ block: 'nearest' });
        }
    }

    function canLoadMore(response, loadedResultCount) {
        if (!response
            || response.rowsPerPage <= 0
            || response.rows.length < response.rowsPerPage)
        {
            return false;
        }

        return response.totalRowCount >= 0
            ? loadedResultCount < response.totalRowCount
            : response.rows.length >= response.rowsPerPage;
    }

    function getOverrideOrigin() {
        try {
            return window.localStorage.getItem('inSite.testing.overrideOrigin');
        } catch (error) {
            return null;
        }
    }

    function redirectForAuthenticationError(status) {
        const path = encodeURIComponent(window.location.pathname);
        if (status === 401)
            window.location.href = '/ui/lobby/signin?returnUrl=' + path;
        else if (status === 403)
            window.location.href = '/403?path=' + path;
    }
})();
