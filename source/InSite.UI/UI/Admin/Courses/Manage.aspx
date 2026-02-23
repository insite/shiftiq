<%@ Page Language="C#" CodeBehind="Manage.aspx.cs" Inherits="InSite.UI.Admin.Courses.Manage" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" %>

<%@ Register Src="./Outlines/Controls/CourseSetup.ascx" TagName="CourseSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/UnitSetup.ascx" TagName="UnitSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ModuleSetup.ascx" TagName="ModuleSetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ModuleTreeView.ascx" TagName="ModuleTreeView" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/ActivitySetup.ascx" TagName="ActivitySetup" TagPrefix="uc" %>
<%@ Register Src="./Outlines/Controls/NotificationSetup.ascx" TagName="NotificationSetup" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent"></asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="ScreenStatus" />
    <insite:ValidationSummary runat="server" ValidationGroup="CourseSetup" />
    <insite:ValidationSummary runat="server" ValidationGroup="NotificationSetup" />
    <insite:ValidationSummary runat="server" ValidationGroup="ActivitySetup" />
    <asp:Panel runat="server" ID="TrackChangesAlert" />

    <insite:Nav runat="server" ID="NavPanel">

        <insite:NavItem runat="server" ID="OutlinePanel" Title="Course Outline" Icon="far fa-sitemap" IconPosition="BeforeText">
            <section>

                <h2 class="h4 mt-4 mb-3">Course Outline</h2>

                <div class="row">
                    <div class="col-lg-5">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body">
                                <uc:ModuleTreeView runat="server" ID="ModuleTreeView" />
                                <div runat="server" id="NoGradebookReminder" class="alert alert-warning">
                                    Remember to create a gradebook to enable learner enrollment and progress tracking for this course.
                                    <a runat="server" id="CreateGradebookLink" href="#">Click here to create a gradebook.</a>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-lg-7">
                        <div class="card border-0 shadow-lg h-100">
                            <div class="card-body" data-track-id="outline" data-track-title="Course Outline tab">
                                <asp:PlaceHolder runat="server" ID="ActivityPlaceHolder" />
                            </div>
                        </div>
                    </div>
                </div>

            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ActivityPanel" Title="Activity Setup" Icon="far fa-cube" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Activity Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body" data-track-id="activity" data-track-title="Activity Setup tab">
                        <uc:ActivitySetup runat="server" ID="ActivitySetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="ModulePanel" Title="Module Setup" Icon="far fa-cubes" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Module Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body" data-track-id="module" data-track-title="Module Setup tab">

                        <uc:ModuleSetup runat="server" ID="ModuleSetup" />

                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="UnitPanel" Title="Unit Setup" Icon="far fa-cubes" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Unit Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body" data-track-id="unit" data-track-title="Unit Setup tab">
                        <uc:UnitSetup runat="server" ID="UnitSetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="NotificationPanel" Title="Notifications" Icon="far fa-bell" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Notifications
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body" data-track-id="notifications" data-track-title="Notifications tab">
                        <uc:NotificationSetup runat="server" ID="NotificationSetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>
        <insite:NavItem runat="server" ID="CoursePanel" Title="Course Setup" Icon="far fa-chalkboard-teacher" IconPosition="BeforeText">
            <section>
                <h2 class="h4 mt-4 mb-3">
                    Course Setup
                </h2>

                <div class="card border-0 shadow-lg h-100">
                    <div class="card-body" data-track-id="course" data-track-title="Course Setup tab">
                        <uc:CourseSetup runat="server" ID="CourseSetup" />
                    </div>
                </div>
            </section>
        </insite:NavItem>

    </insite:Nav>

    <asp:HiddenField runat="server" ID="TrackChangesState" />

    <insite:PageFooterContent runat="server">
        <script type="text/javascript">

            (function () {
                const input = document.getElementById('<%= TrackChangesState.ClientID %>');
                const alertContainer = document.getElementById('<%= TrackChangesAlert.ClientID %>');
                const states = new Map();

                let submitHandler = null;

                document.addEventListener('DOMContentLoaded', function () {
                    setTimeout(onLoad, 10);
                });

                function onLoad() {
                    const isInit = input.value.length === 0;
                    const inputObj = isInit ? null : JSON.parse(input.value);

                    document.querySelectorAll('[data-track-id]').forEach(el => {
                        const id = el.dataset.trackId;
                        if (typeof id !== 'string' || id.length === 0) {
                            console.error('The changes track ID is empty');
                            return;
                        }

                        if (states.has(id)) {
                            console.error('The changes track ID is already exist');
                            return;
                        }

                        states.set(id, {
                            container: el,
                            title: el.dataset.trackTitle,
                            trigger: el.dataset.trackTrigger,
                            data: !isInit && inputObj.hasOwnProperty(id) ? inputObj[id] : serializeData(el)
                        });
                    });

                    if (isInit) {
                        const obj = {};

                        for (const [key, value] of states) {
                            obj[key] = value.data;
                        }

                        input.value = JSON.stringify(obj);
                    }
                }

                document.querySelectorAll('#<%= NavPanel.ClientID %> > ul.nav-tabs').forEach(el => {
                    el.addEventListener('show.bs.tab', onTabShow);
                });

                if (typeof __doPostBack === 'function') {
                    const originalDoPostBack = window.__doPostBack;
                    window.__doPostBack = function (eventTarget, eventArgument) {
                        return allowDoPostback(eventTarget) && originalDoPostBack.apply(this, arguments);
                    };
                }

                if (typeof WebForm_DoPostBackWithOptions === 'function') {
                    const originalDoPostBackWithOptions = window.WebForm_DoPostBackWithOptions;
                    window.WebForm_DoPostBackWithOptions = function (options) {
                        return allowDoPostback(options.eventTarget) && originalDoPostBackWithOptions.apply(this, arguments);
                    };
                }

                window.onbeforeunload = function (e) {
                    if (submitHandler !== null) {
                        return;
                    }

                    if (updateWarning()) {
                        e.preventDefault();
                        e.returnValue = '';
                    }
                };

                function onTabShow(e) {
                    updateWarning();

                    if (!allowProceed()) {
                        e.preventDefault();
                        e.stopPropagation();
                        return false;
                    }
                }

                function allowDoPostback(eventTarget, triggerAll) {
                    onSubmitHandlerTimeout();
                    submitHandler = setTimeout(onSubmitHandlerTimeout, 10);

                    updateWarning();

                    return allowProceed(eventTarget, triggerAll);
                }

                function allowProceed(eventTarget, triggerAll) {
                    const targets = typeof eventTarget === 'string' ? document.getElementsByName(eventTarget) : [];
                    const warnStates = [];

                    for (const state of states.values()) {
                        let isTriggered = false;

                        if (triggerAll === true || state.trigger === 'always') {
                            isTriggered = true;
                        } else if (state.container.offsetParent !== null) {
                            isTriggered = true;
                        }

                        if (!isTriggered || !isStateChange(state)) {
                            continue;
                        }

                        if (targets.length == 0 || !isTargetInsideContainer(state, targets)) {
                            warnStates.push(state);
                        }
                    }

                    if (warnStates.length > 0 && !confirm('Changes you made may not be saved. Are you sure you want to proceed?')) {
                        return false;
                    }

                    return true;
                }

                function updateWarning() {
                    const titles = [];

                    for (const state of states.values()) {
                        if (isStateChange(state)) {
                            titles.push(state.title ? state.title : '<i>' + state.id + '</i>');
                        }
                    }

                    setWarning(titles);

                    return titles.length > 0;
                }

                function onSubmitHandlerTimeout() {
                    if (submitHandler === null) {
                        return;
                    }

                    clearTimeout(submitHandler);
                    submitHandler = null;
                }

                function getVisibleStates() {
                    const result = [];

                    for (const value of states.values()) {
                        if (value.container.offsetParent !== null) {
                            result.push(value);
                        }
                    }

                    return result;
                }

                function isTargetInsideContainer(state, targets) {
                    for (const target of targets) {
                        if (state.container.contains(target)) {
                            return true;
                        }
                    }

                    return false;
                }

                function isStateChange(state) {
                    const data = serializeData(state.container);
                    return data !== state.data;
                }

                function serializeData(root) {
                    const state = [];

                    root.querySelectorAll('input,select,textarea').forEach(el => {
                        if (!el.name || el.disabled || el.closest('[data-track-skip]') !== null)
                            return;

                        let name, value;

                        if (el.tagName === 'TEXTAREA') {
                            if (el.style.visibility === 'hidden')
                                return;

                            addState(el.name, el.value);
                        } else if (el.tagName === 'INPUT') {
                            if (el.type === 'button' || el.type === 'file')
                                return;

                            if (el.type === 'checkbox') {
                                if (el.closest('.mdh-toggle-wrapper'))
                                    return;

                                addState(el.name, el.checked ? (el.value || 'on') : 'off');
                            } else if (el.type === 'radio') {
                                if (!el.checked)
                                    return;

                                addState(el.name, el.value);
                            } else if (el.type === 'hidden') {
                                if (el.id.endsWith('_state')) {
                                    const prevSib = el.previousSibling;
                                    if (prevSib && prevSib.classList.contains('tab-content')) {
                                        prevPrevSib = prevSib.previousSibling;
                                        if (prevPrevSib && prevPrevSib.tagName === 'UL' && prevPrevSib.classList.contains('nav-tabs')) {
                                            return;
                                        }
                                    }
                                } else if (el.hasAttribute('data-editor-translation')) {
                                    let value = '{}';
                                    
                                    try {
                                        const obj = JSON.parse(el.value);
                                        if (obj.data) {
                                            const data = obj.data;
                                            for (const n in data) {
                                                if (data.hasOwnProperty(n) && typeof data[n] === 'string' && data[n].length === 0) {
                                                    delete data[n];
                                                }
                                            }

                                            value = JSON.stringify(data);
                                        }
                                    } catch (e) {
                                        
                                    }

                                    addState(el.name, value);

                                    return;
                                }

                                addState(el.name, el.value);
                            } else {
                                addState(el.name, el.value);
                            }
                        } else if (el.tagName === 'SELECT' && el.multiple) {
                            Array.from(el.selectedOptions).forEach(option => {
                                addState(el.name, option.value);
                            });
                        } else {
                            addState(el.name, el.value);
                        }
                    });

                    function addState(name, value) {
                        state.push(name + '=' + encodeURIComponent(value));
                    }

                    return state.join('&');
                }

                function setWarning(items) {
                    if (!(items instanceof Array) || items.length == 0) {
                        alertContainer.replaceChildren();
                        return;
                    }

                    let itemsHtml = '';
                    for (const i of items) {
                        itemsHtml += '<li>' + i + '</li>';
                    }

                    alertContainer.innerHTML = `
<div class="alert d-flex alert-warning alert-dismissible fade show">
    <i class="fas fa-exclamation-triangle pe-1 me-2"></i>
    <div>
        <strong>Warning:</strong> You have unsaved changes:
        <ul class="my-2">` + itemsHtml + `</ul>
        Please save your work before navigating away.
    </div>
    <button type='button' class='btn-close' data-bs-dismiss='alert' aria-label='Close'></button>
</div>`;
                }
            })();
        </script>
    </insite:PageFooterContent>
</asp:Content>
