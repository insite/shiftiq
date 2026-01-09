(function () {
    if (inSite.common.adminSidebar)
        return;

    const instance = inSite.common.adminSidebar = Object.freeze({
        init: init,
        load: load,
    });

    const stateType = Object.freeze({
        unknown: 0,
        open: 1,
        hiding: 2,
        decreasing: 3,
        close: 4,
        increasing: 5,
        showing: 6
    });

    const opacityStepDuration = 150;
    const sizeStepDuration = 300;

    let isMoving = false;
    let state = null;
    let sidebar = null;
    let trigger = null;
    let content = null;
    let leftNav = null;

    function init() {
        if (sidebar && trigger)
            return;

        sidebar = document.querySelector('aside.admin-sidebar');
        if (!sidebar)
            return;

        const toggle = sidebar.querySelector('.admin-sidebar-header > .admin-sidebar-toggle');
        if (!toggle)
            return;

        trigger = toggle.querySelector(':scope > div');
        if (!trigger)
            return;

        leftNav = document.querySelector('header .navbar-nav-left');

        if (getState() === 'close') {
            state = stateType.close;
            sidebar.classList.add('admin-sidebar-compact', 'compact-hidden', 'compact-size');
            if (leftNav) leftNav.classList.add('compact-size');
        } else {
            state = stateType.open;
        }

        trigger.addEventListener('click', function () {
            if (isMoving) {
                return;
            }

            if (state === stateType.open) {
                isMoving = true;
                state = stateType.hiding;
                sidebar.classList.add('admin-sidebar-compact', 'compact-hiding');
                if (content) content.classList.add('compact-move');
                if (leftNav) leftNav.classList.add('compact-move');
                setTimeout(onStepComplete, opacityStepDuration);
                setState('close');
            } else if (state === stateType.close) {
                isMoving = true;
                state = stateType.increasing;
                sidebar.classList.remove('compact-size', 'compact-increasing');
                if (content) {
                    content.classList.add('compact-move');
                    content.classList.remove('compact-size');
                }
                if (leftNav) {
                    leftNav.classList.add('compact-move');
                    leftNav.classList.remove('compact-size');
                }
                setTimeout(onStepComplete, sizeStepDuration);
                setState('open');
            }
        });
    }

    function load() {
        content = document.querySelector('main.admin-container');
        if (content && getState() === 'close')
            content.classList.add('compact-size');
    }

    function onStepComplete() {
        if (state == stateType.hiding) {
            state = stateType.decreasing;
            sidebar.classList.add('compact-hidden', 'compact-size');
            sidebar.classList.remove('compact-hiding');
            if (content) content.classList.add('compact-size');
            if (leftNav) leftNav.classList.add('compact-size');
            setTimeout(onStepComplete, sizeStepDuration);
        } else if (state == stateType.decreasing) {
            isMoving = false;
            state = stateType.close;
            if (content) content.classList.remove('compact-move');
            if (leftNav) leftNav.classList.remove('compact-move');
        } else if (state == stateType.increasing) {
            state = stateType.showing;
            sidebar.classList.remove('compact-hidden');
            setTimeout(onStepComplete, opacityStepDuration);
        } else if (state == stateType.showing) {
            isMoving = false;
            state = stateType.open;
            sidebar.classList.remove('admin-sidebar-compact');
            if (content) content.classList.remove('compact-move');
            if (leftNav) leftNav.classList.remove('compact-move');
        }
    }

    const storageStatePath = 'inSite.common.adminSidebar';

    function setState(value) {
        try {
            window.localStorage.setItem(storageStatePath, value);
        } catch (e) {

        }
    }

    function getState() {
        try {
            return window.localStorage.getItem(storageStatePath);
        } catch (e) {
            return null;
        }
    }
})();