(function () {
    const instance = inSite.common.recentLinks = inSite.common.recentLinks || {};

    const observers = Object.freeze({
        '/ui/admin/assessments/banks/outline': { name: 'Bank', idParam: 'bank' },
        '/ui/admin/contacts/groups/edit': { name: 'Group', idParam: 'contact' },
        '/ui/admin/contacts/people/edit': { name: 'Person', idParam: 'contact' },
        '/ui/admin/courses/manage': { name: 'Course', idParam: 'course' },
        '/ui/admin/events/classes/outline': { name: 'Class Event', idParam: 'event' },
        '/ui/admin/events/exams/outline': { name: 'Exam Event', idParam: 'event' },
        '/ui/admin/messages/outline': { name: 'Message', idParam: 'message' },
        '/ui/admin/records/achievements/outline': { name: 'Achievement', idParam: 'id' },
        '/ui/admin/records/credentials/outline': { name: 'Credential', idParam: 'id' },
        '/ui/admin/records/gradebooks/outline': { name: 'Gradebook', idParam: 'id' },
        '/ui/admin/records/logbooks/outline': { name: 'Logbook', idParam: 'journalsetup' },
        '/ui/admin/learning/programs/outline': { name: 'Program', idParam: 'id' },
        '/ui/admin/records/rubrics/outline': { name: 'Rubric', idParam: 'rubric' },
        '/ui/admin/sales/invoices/outline': { name: 'Invoice', idParam: 'id' },
        '/ui/admin/sales/payments/outline': { name: 'Payment', idParam: 'id' },
        '/ui/admin/sites/outline': { name: 'Site', idParam: 'id' },
        '/ui/admin/sites/pages/outline': { name: 'Page', idParam: 'id' },
        '/ui/admin/standards/manage': { name: 'Standard', idParam: 'standard' },
        '/ui/admin/workflow/forms/outline': { name: 'Form', idParam: 'form' },
        '/ui/admin/workflow/forms/submissions/outline': { name: 'Submission', idParam: 'session' },
        '/ui/admin/workflow/cases/outline': { name: 'Case', idParam: 'case' },
        '/ui/admin/reports/dashboards': { name: 'Dashboards', idParam: '' },
        '/ui/admin/accounts/organizations/edit': { name: 'Organization', idParam: 'organization' },
        '/ui/admin/accounts/senders/edit': { name: 'Sender', idParam: 'id' },
        '/ui/admin/assessment/quizzes/edit': { name: 'Quiz', idParam: 'quiz' },
        '/ui/admin/assets/collections/edit': { name: 'Collection', idParam: 'collection' },
        '/ui/admin/assets/collections/edit-item': { name: 'Collection', idParam: 'item' },
        '/ui/admin/assets/labels/edit': { name: 'Label', idParam: 'label' },
        '/ui/admin/contacts/people/edit-membership': { name: 'Person', idParam: 'to' },
        '/ui/admin/database/columns/outline': { name: 'Column', idParam: 'columnName' },
        '/ui/admin/events/appointments/outline': { name: 'Appointment', idParam: 'event' },
        '/ui/admin/integrations/api-requests/outline': { name: 'API', idParam: 'request' },
        '/ui/admin/logs/aggregates/outline': { name: 'Aggregate', idParam: 'aggregate' },
        '/ui/admin/records/gradebooks/instructors/gradebook-outline': { name: 'Gradebook', idParam: 'id' },
        '/ui/admin/records/gradebooks/instructors/person-outline': { name: 'Instructor', idParam: 'contact' },
        '/ui/admin/records/logbooks/validators/outline': { name: 'Validator', idParam: 'journalsetup' },
        '/ui/admin/records/logbooks/validators/outline-journal': { name: 'Logbook', idParam: 'journalsetup' },
        '/ui/admin/records/rubrics/edit': { name: 'Rubric', idParam: 'rubric' },
        '/ui/admin/registrations/classes/edit': { name: 'Class Event', idParam: 'id' },
        '/ui/admin/registrations/exams/edit': { name: 'Exam Event', idParam: 'registration' },
        '/ui/admin/reports/edit': { name: 'Report', idParam: 'id' },
        '/ui/admin/sales/products/edit': { name: 'Product', idParam: 'id' },
        '/ui/admin/standards/documents/outline': { name: 'Standard', idParam: 'asset' },
        '/ui/admin/standards/edit': { name: 'Standard', idParam: 'id' }
    });

    const settings = {
        key: null,
        container: {
            id: null,
            elem: null,
            placeholder: null
        },
        maxItems: 15
    };

    // public

    instance.init = function (key, containerId) {
        if (settings.key !== null)
            return;

        if (typeof key !== 'string' || key.length === 0)
            return;

        if (typeof containerId === 'string' && containerId.length > 0) {
            const container = settings.container.elem = document.getElementById(containerId);
            if (container) {
                settings.container.id = containerId;
                const placeholder = settings.container.placeholder = document.createComment('recent-links-placeholder');

                container.parentNode.replaceChild(placeholder, container);
            }
        }

        settings.key = key;

        add(window.location, document.title);
        renderMenu();
    }

    instance.getAll = function () {
        return loadItems();
    };

    instance.add = function (url, title) {
        const result = add(url, title);
        if (result)
            renderMenu();
        return result;
    };

    instance.clear = function () {
        saveItems([]);
        renderMenu();
    };

    // private

    function add(url, title) {
        if (typeof url === 'string') {
            url = new URL(url);
        } else if (url instanceof Location) {
            url = new URL(url.href);
        } else if (!(url instanceof URL)) {
            return false;
        }

        const observer = observers[url.pathname];
        if (!observer)
            return false;

        const key = normalizeKey(url, observer.idParam);

        let list = loadItems();
        list = list.filter(function (i) { return i.key !== key; });
        list.unshift({
            pageUrl: url.href,
            observerName: observer.name,
            pageTitle: title ? snip(title, 100) : null,
            key: key
        });

        if (list.length > settings.maxItems)
            list = list.slice(0, settings.maxItems);

        saveItems(list);

        return true;
    }

    function renderMenu() {
        if (settings.container.elem === null)
            return;

        const data = loadItems();
        if (data.length === 0) {
            if (document.contains(settings.container.elem))
                settings.container.elem.parentNode.replaceChild(settings.container.placeholder, settings.container.elem);
            return;
        }

        let list = settings.container.elem.querySelector(':scope > ul.dropdown-menu');
        if (!list) {
            list = document.createElement('ul');
            list.className = 'dropdown-menu';
            settings.container.elem.appendChild(list);
        }

        list.replaceChildren();

        for (let i = 0; i < data.length; i++) {
            const item = data[i];

            const anchor = document.createElement('a');
            anchor.className = 'dropdown-item';
            anchor.href = item.pageUrl;
            anchor.innerText = item.pageTitle || item.observerName;

            const listItem = document.createElement('li');
            listItem.appendChild(anchor);

            list.appendChild(listItem);
        }

        if (!document.contains(settings.container.elem))
            settings.container.placeholder.parentNode.replaceChild(settings.container.elem, settings.container.placeholder);
    }

    // storage

    function loadItems() {
        try {
            const json = window.localStorage.getItem(getStorageKey());
            return json ? JSON.parse(json) : [];
        } catch (e) {
            return [];
        }
    }

    function saveItems(list) {
        try {
            localStorage.setItem(getStorageKey(), JSON.stringify(list));
        } catch (e) {

        }
    }

    function getStorageKey() {
        if (!settings.key)
            throw 'recentLinks: not inited';

        return 'recentLinks.' + settings.key;
    }

    // helpers

    function normalizeKey(url, idParam) {
        const baseUrl = url.protocol + '//' + url.host + url.pathname;

        if (idParam) {
            var value = url.searchParams.get(idParam);
            if (value)
                return baseUrl + '|' + value;
        }

        return baseUrl;
    }

    function snip(text, maxLength) {
        const elipsis = '...';

        if (typeof text !== 'string')
            return null;

        text = text.trim();
        if (text.length === 0)
            return null;

        if (text.length <= maxLength)
            return text;

        return text.substring(0, maxLength - elipsis.length) + elipsis;
    }
})();