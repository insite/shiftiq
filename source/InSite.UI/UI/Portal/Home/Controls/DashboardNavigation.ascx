<%@ Control Language="C#" CodeBehind="DashboardNavigation.ascx.cs" Inherits="InSite.UI.Portal.Home.Controls.MyDashboards" %>

<h3 class="d-none d-block bg-secondary fs-sm fw-semibold text-body-secondary mb-0 px-4 py-3">Dashboard</h3>

<asp:Panel runat="server" ID="LinksDefault" CssClass="list-group list-group-flush border-top mb-2">

    <a runat="server" id="A1" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home">
        <i class="fas fa-home fs-lg opacity-60 me-2 me-compact-0" title="Home"></i>
        <span class="hide-compact">Home</span>
    </a>

    <a runat="server" id="MyAccount" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/settings">
        <i class="fas fa-user fs-lg opacity-60 me-2 me-compact-0" title="Account Settings"></i>
        <span class="hide-compact">Account Settings</span>
    </a>

    <a runat="server" id="MyAchievements" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/achievements">
        <i class="fas fa-award fs-lg opacity-60 me-2 me-compact-0" title="Achievements"></i>
        <span class="hide-compact">Achievements</span>
    </a>

    <a runat="server" id="MyAssessments" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/assessments">
        <i class="fas fa-balance-scale fs-lg opacity-60 me-2 me-compact-0" title="Assessments"></i>
        <span class="hide-compact">Assessments</span>
    </a>

    <a runat="server" id="MyCatalogs" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/catalogs">
        <i class="fas fa-books fs-lg opacity-60 me-2 me-compact-0" title="Catalogs"></i>
        <span class="hide-compact">Catalogs</span>
    </a>

    <a runat="server" id="MyChats" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/chats">
        <i class="fas fa-comments fs-lg opacity-60 me-2 me-compact-0" title="Chats"></i>
        <span class="hide-compact">Chats</span>
    </a>

    <a runat="server" id="MyCompetencies" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/competencies">
        <i class="fas fa-ruler-triangle fs-lg opacity-60 me-2 me-compact-0" title="Competencies"></i>
        <span class="hide-compact">Competencies</span>
    </a>

    <a runat="server" visible="false" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/contacts">
        <i class="fas fa-address-book fs-lg opacity-60 me-2 me-compact-0" title="Contacts"></i>
        <span class="hide-compact">Contacts</span>
    </a>

    <a runat="server" id="MyCourses" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/courses">
        <i class="fas fa-users-class fs-lg opacity-60 me-2 me-compact-0" title="Courses"></i>
        <span class="hide-compact">Courses</span>
    </a>

    <a runat="server" id="MyPrograms" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/programs">
        <i class="fas fa-graduation-cap fs-lg opacity-60 me-2 me-compact-0" title="Programs"></i>
        <span class="hide-compact">Programs</span>
    </a>

    <a runat="server" id="MyEvents" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/events">
        <i class="fas fa-calendar-alt fs-lg opacity-60 me-2 me-compact-0" title="Events"></i>
        <span class="hide-compact">Events</span>
    </a>
                        
    <a runat="server" id="MyGrades" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/grades">
        <i class="fas fa-bullseye-arrow fs-lg opacity-60 me-2 me-compact-0" title="Grades"></i>
        <span class="hide-compact">Grades</span>
    </a>

    <a runat="server" visible="false" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/learning-plan">
        <i class="fas fa-map-marked-alt fs-lg opacity-60 me-2 me-compact-0" title="Learning Plan"></i>
        <span class="hide-compact">Learning Plan</span>
    </a>

    <a runat="server" id="MyLogbooks" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/logbooks">
        <i class="fas fa-book-open fs-lg opacity-60 me-2 me-compact-0" title="Logbooks"></i>
        <span class="hide-compact">Logbooks</span>
    </a>

    <a runat="server" id="MyMessages" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/messages">
        <i class="fas fa-paper-plane fs-lg opacity-60 me-2 me-compact-0" title="Messages"></i>
        <span class="hide-compact">Messages</span>
    </a>

    <a runat="server" id="MyReports" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/reports">
        <i class="fas fa-file-chart-column fs-lg opacity-60 me-2 me-compact-0" title="Reports"></i>
        <span class="hide-compact">Reports</span>
    </a>

    <a runat="server" visible="false" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/statistics">
        <i class="fas fa-tachometer-alt fs-lg opacity-60 me-2 me-compact-0" title="Statistics"></i>
        <span class="hide-compact">Statistics</span>
    </a>

    <a runat="server" id="MySurveys" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/home/forms">
        <i class="fas fa-check-square fs-lg opacity-60 me-2 me-compact-0" title="Forms"></i>
        <span class="hide-compact">Forms</span>
    </a>

</asp:Panel>

<!-- MANAGER MENU -->
<asp:Panel runat="server" ID="LinksManager" Visible="false" CssClass="list-group list-group-flush border-top">
    <a runat="server" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/management/dashboard/home">
        <i class="fas fa-home fs-lg opacity-60 me-2 me-compact-0" title="Home"></i>
        <span class="hide-compact">Home</span>
    </a>
    <a runat="server" id="ManagerCatalog" class="d-flex align-items-center list-group-item list-group-item-action"  href="/ui/portal/management/dashboard/catalog">
        <i class="fas fa-check fs-lg opacity-60 me-2 me-compact-0" title="SkillsCheck Catalog"></i>
        <span class="hide-compact">SkillsCheck Catalog</span>
    </a>
    <a runat="server" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/management/dashboard/reports">
        <i class="fas fa-copy fs-lg opacity-60 me-2 me-compact-0" title="Reports"></i> 
        <span class="hide-compact">Reports</span>
    </a>
</asp:Panel>

<!-- LEARNER MENU  -->
<asp:Panel runat="server" ID="LinksLearner" Visible="false" CssClass="list-group list-group-flush border-top">
    <a runat="server" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/home">
        <i class="fas fa-home fs-lg opacity-60 me-2 me-compact-0" title="My SkillsChecks"></i>
        <span class="hide-compact">My SkillsChecks</span>
    </a>
    <a runat="server" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/badges">
        <i class="fas fa-copy fs-lg opacity-60 me-2 me-compact-0" title="My Badges"></i>
        <span class="hide-compact">My Badges</span>
    </a>
    <a runat="server" class="d-flex align-items-center list-group-item list-group-item-action" href="/ui/portal/learning/dashboard/catalog">
        <i class="fas fa-check fs-lg opacity-60 me-2 me-compact-0" title="SkillsCheck Catalog"></i>
        <span class="hide-compact">SkillsCheck Catalog</span>
    </a>
</asp:Panel>

<insite:PageFooterContent runat="server">
    <script type="text/javascript">
        (() => {
            class CounterStorage {
                #path = null;

                constructor(key) {
                    if (typeof key === 'string' && key.length > 0)
                        this.#path = 'ui.portal.mydashboard.counters:' + key;
                }

                load() {
                    if (this.#path != null) {
                        try {
                            const json = window.localStorage.getItem(this.#path);
                            if (json)
                                return JSON.parse(json);
                        } catch (e) {

                        }
                    }

                    return {};
                }

                save(value) {
                    if (this.#path == null)
                        return;

                    try {
                        const json = JSON.stringify(value);

                        window.localStorage.setItem(this.#path, json);
                    } catch (e) {

                    }
                }
            }

            const storage = new CounterStorage('<%= GetLocalStorageKey() %>');
            const counters = storage.load();

            const root = document.getElementById('<%= ActiveLinksClientID %>');
            if (!root) return;

            $(root).find('a').each(function () {
                const $this = $(this);
                const name = this.pathname;

                var isActive = name === window.location.pathname;
                if (isActive)
                    $this.addClass('active');

                var linkCount = parseInt($this.data('count'));
                if (isNaN(linkCount))
                    return;

                var showCount = linkCount;

                if (Object.prototype.hasOwnProperty.call(counters, name)) {
                    var lastCount = parseInt(counters[name]);
                    if (!isNaN(lastCount) && lastCount > 0)
                        showCount = linkCount - lastCount;
                }

                if (showCount <= 0 || linkCount <= 0)
                    return;

                if (isActive)
                    counters[name] = linkCount

                $this.append(
                    $('<span class="fs-sm fw-normal ms-auto badge-container">').append(
                        $('<span class="badge bg-info">').text(linkCount.toLocaleString())
                    )
                );
            });

            storage.save(counters);
        })();
    </script>
</insite:PageFooterContent>