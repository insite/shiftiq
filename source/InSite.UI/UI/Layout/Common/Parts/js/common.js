var inSite = {
    common: {

        init: function () {
            if (typeof $.fn.tooltip !== 'undefined')
                $('[data-toggle="tooltip"]').tooltip();
        },

        stringHelper: {
            isNullOrEmpty: function (value) {
                return value == null || typeof value !== 'string' || value.length == 0;
            },
            toBase26: function (num) {
                if (typeof num == 'number' && num >= 0) {
                    let label = '';

                    while (num >= 26) {
                        label = String.fromCharCode(65 + num % 26) + label;
                        num = Math.floor(num / 26) - 1;
                    }

                    return String.fromCharCode(65 + num % 26) + label;
                }

                return '?';
            }
        },

        execFuncByName: function (name) {
            var args = Array.prototype.slice.call(arguments, 1);
            var namespaces = name.split('.');
            var funcName = namespaces.pop();

            var context = window;
            for (var i = 0; i < namespaces.length; i++)
                context = context[namespaces[i]];

            var funcObj = context[funcName];
            if (typeof funcObj !== 'function') {
                alert('\'' + String(name) + '\' is not a function ');
                return;
            }

            return funcObj.apply(context, args);
        },

        getObjByName: function (name) {
            var obj = window;

            var path = name.split('.');
            for (var i = 0; i < path.length; i++) {
                obj = obj[path[i]];
                if (typeof obj == 'undefined')
                    return null;
            }

            return obj;
        },

        setObjProp: function (obj, name, value) {
            var path = name.split('.');

            for (var i = 0; i < path.length - 1; i++) {
                var objName = path[i];
                if (obj.hasOwnProperty(objName))
                    obj = obj[objName];
                else
                    obj = obj[objName] = {};
            }

            obj[path[path.length - 1]] = value;
        },

        cloneObj: function (obj) {
            if (obj === null || typeof obj !== 'object')
                return obj;

            if (obj instanceof Date)
                return new Date(obj.getTime());

            if (Array.isArray(obj)) {
                var result = [];

                for (var i = 0; i < obj.length; i++)
                    result.push(inSite.common.cloneObj(obj[i]));

                return result;
            }

            var result = new obj.constructor();

            for (var prop in obj) {
                if (obj.hasOwnProperty(prop))
                    result[prop] = inSite.common.cloneObj(obj[prop]);
            }

            return result;
        },

        updateQueryString: function (key, value, url) {
            if (!url)
                url = window.location.href;

            var re = new RegExp('([?&])' + key + '=.*?(&|#|$)(.*)', 'gi');

            if (re.test(url)) {
                if (typeof value !== 'undefined' && value !== null) {
                    url = url.replace(re, '$1' + key + '=' + value + '$2$3');
                } else {
                    var hash = url.split('#');

                    url = hash[0].replace(re, '$1$3').replace(/(&|\?)$/, '');

                    if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                        url += '#' + hash[1];
                }
            }
            else if (typeof value !== 'undefined' && value !== null) {
                var separator = url.indexOf('?') !== -1 ? '&' : '?';
                var hash = url.split('#');

                url = hash[0] + separator + key + '=' + value;

                if (typeof hash[1] !== 'undefined' && hash[1] !== null)
                    url += '#' + hash[1];
            }

            return url;
        },

        queryString: (function (a) {
            if (a == '') return {};
            var b = {};
            for (var i = 0; i < a.length; ++i) {
                var p = a[i].split('=', 2);
                if (p.length != 2) continue;
                b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, ' '));
            }
            return b;
        })(window.location.search.substring(1).split('&')),
    }
};

(function () {
    inSite.common.disableButton = disableButton;
    inSite.common.enableButtonGroup = enableButtonGroup;

    const disabledClass = 'disabled';
    const disabledGroups = {};

    function disableButton(button, validationGroup, groupName, enableAfter) {
        button = getButton(button);
        if (!button)
            return true;

        if (validationGroup) {
            Page_ClientValidate(validationGroup);
            if (!Page_IsValid)
                return false;
        }

        if (groupName) {
            const disabledElements = [];

            document.querySelectorAll('[data-btn-group="' + String(groupName) + '"]').forEach(e => {
                if (toggle(e, true))
                    disabledElements.push(e);
            });

            if (disabledElements.length > 0) {
                const key = getGroupKey(groupName);
                disabledGroups[key] = (disabledGroups[key] ?? []).concat(disabledElements);
            }

            if (enableAfter) {
                setTimeout(() => enableButtonGroup(groupName), enableAfter);
            }
        } else {
            toggle(button, true);

            if (enableAfter) {
                setTimeout(() => toggle(button, false), enableAfter);
            }
        }

        return true;
    }

    function enableButtonGroup(groupName) {
        const key = getGroupKey(groupName);
        if (!disabledGroups.hasOwnProperty(key))
            return;

        const elements = disabledGroups[key];

        for (let el of elements) {
            if (document.contains(el))
                toggle(el, false);
        }

        delete disabledGroups[key];
    }

    function getButton(value) {
        if (typeof value == 'string' && value.length > 0)
            value = document.querySelector(value);

        return value instanceof HTMLElement ? value : null;
    }

    function getGroupKey(value) {
        return value.trim().toLowerCase();
    }

    function toggle(element, disable) {
        if (element instanceof HTMLAnchorElement) {
            if (disable) {
                if (!element.classList.contains(disabledClass))
                    element.classList.add(disabledClass);
                else
                    return false;
            } else {
                if (element.classList.contains(disabledClass))
                    element.classList.remove('disabled');
                else
                    return false
            }
        } else if (element instanceof HTMLInputElement || element instanceof HTMLButtonElement) {
            if (disable) {
                if (!element.disabled)
                    element.disabled = true;
                else
                    return false;
            } else {
                if (element.disabled)
                    element.disabled = false;
                else
                    return false
            }
        } else {
            return false;
        }

        return true;
    }
})();

(function () {
    inSite.common.contentToIFrame = init;

    const iFrames = [];
    let isDocumentLoaded = false;

    window.addEventListener('DOMContentLoaded', () => {
        isDocumentLoaded = true;
        onDocumentLoaded();
    });

    window.addEventListener('resize', updateHeight);
    window.addEventListener('shown.bs.tab', updateHeight);
    window.addEventListener('shown.bs.modal', updateHeight);
    window.addEventListener('shown.bs.collapse', updateHeight);

    function init(container, options) {
        if (typeof container == 'string' && container.length > 0)
            container = document.querySelector(container);

        if (!container || !(container instanceof HTMLElement))
            return;

        let containerExists = false;

        forEachFrame(item => {
            if (item.container === container) {
                containerExists = true;
                return false;
            }
        });

        if (containerExists)
            return;

        let html = options?.html;
        if (!html)
            html = container.innerText.trim();

        if (!html)
            return;

        container.replaceChildren();
        container.classList.add('invisible');
        container.classList.remove('d-none');

        const iframe = document.createElement('iframe');
        iframe.className = 'w-100 border-0 overflow-hidden m-0 p-0';
        container.appendChild(iframe);

        iframe.addEventListener('load', () => {
            onIFrameLoaded();
        });

        const doc = iframe.contentDocument;
        doc.open();
        doc.write(html);
        doc.close();

        if (options?.disableAnchors) {
            for (let a of doc.getElementsByTagName('a'))
                a.onclick = onAnchorClick;
        }

        const item = {
            container: container,
            iFrame: iframe,
            options: {
                extraHeight: 20
            }
        };

        if (options) {
            if (typeof options.extraHeight == 'number' && options.extraHeight >= 0)
                item.options.extraHeight = options.extraHeight;
        }

        iFrames.push(item);

        if (isDocumentLoaded)
            onDocumentLoaded();
    }

    function onDocumentLoaded() {
        updateHeight();
    }

    function onIFrameLoaded() {
        updateHeight();
    }

    function onAnchorClick() {
        return false;
    }

    function forEachFrame(fn) {
        for (var i = 0; i < iFrames.length; i++) {
            var item = iFrames[i];

            if (!document.contains(item.iFrame)) {
                iFrames.splice(i--, 1);
                continue;
            }

            if (fn(item) == false)
                break;
        }
    }

    function updateHeight() {
        forEachFrame(item => {
            const iFrame = item.iFrame;
            if (iFrame.offsetParent == null)
                return;

            const body = iFrame.contentDocument.body;
            const isInit = !item.width;
            if (!isInit && item.width == body.scrollWidth)
                return;

            iFrame.style.height = '';

            let height = body.scrollHeight;
            if (height <= 0)
                return;

            const style = getComputedStyle(body);

            const marginTop = parseFloat(style.marginTop);
            if (!isNaN(marginTop) && marginTop > 0)
                height += Math.ceil(marginTop);

            const marginBottom = parseFloat(style.marginBottom);
            if (!isNaN(marginBottom) && marginBottom > 0)
                height += Math.ceil(marginBottom);

            iFrame.style.height = String(height + item.options.extraHeight) + 'px';

            item.width = body.scrollWidth;

            if (isInit)
                item.container.classList.remove('invisible');
        });
    }
})();

$(inSite.common.init);

// Utilities

(function () {
    window.setCheckboxes = function (containerId, checked) {
        var checkboxes = document.getElementById(containerId).getElementsByTagName('input');

        for (var i = 0; i < checkboxes.length; i++) {
            if (checkboxes[i].type.toLowerCase() == 'checkbox' && !checkboxes[i].disabled)
                checkboxes[i].checked = checked;
        }

        return false;
    };
})();

// MathQuill
$(function () {
    var MQ = MathQuill.getInterface(2);

    Sys.Application.add_load(init);

    init();

    function init() {
        $("span.math-eq").each(function (i, el) {
            MQ.StaticMath(el);
            $(el).removeClass("math-eq");
        });
    }
});

// Extend BS Dropdown

window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-bs-toggle="dropdown"][data-popper-strategy="fixed"]').forEach(el => {
        if (bootstrap.Dropdown.getInstance(el))
            bootstrap.Dropdown.dispose(el);

        new bootstrap.Dropdown(el, {
            popperConfig(bsConfig) {
                return {
                    ...bsConfig,
                    strategy: 'fixed'
                };
            }
        })
    });
});
