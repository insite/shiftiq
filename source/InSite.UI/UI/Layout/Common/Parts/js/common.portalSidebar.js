(function () {
    if (inSite.common.portalSidebar)
        return;

    const instance = inSite.common.portalSidebar = Object.freeze({
        init: init
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

    const opacityStepDuration = 300;
    const sizeStepDuration = 600;

    let isMoving = false;
    let state = null;
    let sidebar = null;
    let trigger = null;

    window.addEventListener('resize', onWindowResize);
    window.addEventListener('DOMContentLoaded', onWindowResize);

    function init() {
        if (sidebar && trigger)
            return;

        sidebar = document.querySelector('main.portal-container .portal-sidebar');
        if (!sidebar)
            return;

        trigger = sidebar.querySelector('.portal-sidebar-toggle > div');
        if (!trigger) {
            sidebar = null;
            return;
        }

        const elData = sidebar._portalSidebar = (function () {
            const result = {
                isCompact: getState() === 'close',
                isLg: window.innerWidth >= 992
            };

            result.compactWidth = parseInt(sidebar.dataset.compactWidth);
            if (!isNaN(result.compactWidth) && result.compactWidth > 0) {
                result.pxCompactWidth = String(result.compactWidth) + 'px';
            } else {
                result.compactWidth = null;
                result.pxCompactWidth = null;
            }

            result.defaultWidth = parseInt(sidebar.dataset.width);
            if (!isNaN(result.defaultWidth) && result.defaultWidth > 0) {
                result.pxDefaultWidth = String(result.defaultWidth) + 'px';
            } else {
                result.defaultWidth = null;
                result.pxDefaultWidth = null;
            }

            return result;
        })();

        if (elData.isCompact) {
            state = stateType.close;
            sidebar.classList.add('portal-sidebar-compact', 'compact-hidden', 'compact-size', 'compact-stop');
        } else {
            state = stateType.open;
        }

        setSize();

        trigger.addEventListener('click', function () {
            if (isMoving) {
                return;
            }

            if (state === stateType.open) {
                isMoving = true;
                state = stateType.hiding;
                sidebar.classList.add('portal-sidebar-compact', 'compact-hiding');
                setTimeout(onStepComplete, opacityStepDuration);
                setState('close');
            } else if (state === stateType.close) {
                isMoving = true;
                state = stateType.increasing;
                sidebar.classList.remove('compact-stop', 'compact-size', 'compact-increasing');
                setSize(false);
                setTimeout(onStepComplete, sizeStepDuration);
                setState('open');
            }
        });
    }

    function onStepComplete() {
        if (state == stateType.hiding) {
            state = stateType.decreasing;
            sidebar.classList.add('compact-hidden', 'compact-size');
            setSize(true);
            sidebar.classList.remove('compact-hiding');
            setTimeout(onStepComplete, sizeStepDuration);
        } else if (state == stateType.decreasing) {
            sidebar.classList.add('compact-stop');
            isMoving = false;
            state = stateType.close;
        } else if (state == stateType.increasing) {
            state = stateType.showing;
            sidebar.classList.remove('compact-hidden');
            setTimeout(onStepComplete, opacityStepDuration);
        } else if (state == stateType.showing) {
            isMoving = false;
            state = stateType.open;
            sidebar.classList.remove('portal-sidebar-compact');
        }
    }

    function onWindowResize() {
        if (!sidebar)
            return;

        const isLg = window.innerWidth >= 992;
        const isChanged = sidebar._portalSidebar.isLg !== isLg;

        sidebar._portalSidebar.isLg = isLg;

        if (!isChanged) {
            return;
        }

        if (!isLg) {
            sidebar.style.minWidth = '';
            sidebar.style.maxWidth = '';

            const byHeight = sidebar.querySelectorAll('.hide-height > div');
            for (let i = 0; i < byHeight.length; i++) {
                byHeight[i].style.maxWidth = '';
            }
        } else {
            setSize();
        }
    }

    function setSize(compact) {
        if (!sidebar._portalSidebar.isLg)
            return;

        const data = sidebar._portalSidebar;
        if (typeof compact !== 'boolean') {
            compact = data.isCompact;
        } else {
            data.isCompact = compact;
        }

        if (compact) {
            sidebar.style.minWidth = data.pxCompactWidth;
            sidebar.style.maxWidth = data.pxCompactWidth;
        } else if (compact === false) {
            sidebar.style.minWidth = data.pxDefaultWidth;
            sidebar.style.maxWidth = data.pxDefaultWidth;

            const byHeight = sidebar.querySelectorAll('.hide-height > div');
            for (let i = 0; i < byHeight.length; i++) {
                byHeight[i].style.maxWidth = data.pxDefaultWidth;
            }
        }
    }

    const storageStatePath = 'inSite.common.portalSidebar';

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