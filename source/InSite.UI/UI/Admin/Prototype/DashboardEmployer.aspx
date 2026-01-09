<%@ Page Language="C#" MasterPageFile="~/UI/Layout/Admin/AdminHome.master" CodeBehind="DashboardEmployer.aspx.cs" Inherits="InSite.UI.Admin.Prototype.DashboardEmployer" %>

<%@ Register Src="~/UI/Admin/Events/Reports/Controls/RecentClassesList.ascx" TagName="RecentClassesList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Events/Reports/Controls/RecentExamsList.ascx" TagName="RecentExamsList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Events/Reports/Controls/RecentAppointmentsList.ascx" TagName="RecentAppointmentsList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Workflow/Cases/Controls/RecentList.ascx" TagName="RecentCaseList" TagPrefix="uc" %>
<%@ Register Src="~/UI/Admin/Contacts/People/Controls/RecentList.ascx" TagName="PersonRecentList" TagPrefix="uc" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        #calendar .btn-primary:disabled {
            color: #fff;
        }
    </style>
</asp:Content>

<asp:Content ContentPlaceHolderID="BodyContent" runat="server">

    <section>

        <div class="row">

            <div class="col-lg-6">

                <h3>Recent Activities</h3>

                <div class="card border-0 shadow-lg">

                    <div class="card-header">
                        <ul class="nav nav-tabs card-header-tabs" role="tablist">
                            <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">People</a></li>
                        </ul>
                    </div>
                    <div class="card-body">
                        <div class="tab-content">
                            <div class="tab-pane fade show active" id="result1" role="tabpanel">
                                <uc:PersonRecentList runat="server" ID="RecentPeople" />
                            </div>
                        </div>
                    </div>

                </div>

            </div>

            <div class="col-lg-6">

                <h3>Calendar</h3>

                <div class="card border-0 shadow-lg">
                    <div class="card-body">

                        <div runat="server" id="CalendarPanel" class="main-panel">
                            <div id='calendar' style="min-height: 600px"></div>
                        </div>

                        <insite:PageFooterContent runat="server">
                            <script type="text/javascript">

                                document.addEventListener('DOMContentLoaded', function () {
                                    const calendarEl = document.getElementById('calendar');
                                    const date = '<%= CalendarDate %>';

                                $.when(getEvents(date)).done(function (result) {
                                    generateCalendar(getEventsArray(result));
                                });

                                function getEvents(date) {
                                    return $.ajax({
                                        type: 'GET',
                                        dataType: 'json',
                                        url: '/api/events/list-events',
                                        headers: { 'user': '<%= InSite.Api.Settings.ApiHelper.GetApiKey() %>' },
                    data: {
                        date: date,
                    },
                    success: function (result) {

                    },
                    error: function (xhr) {
                        alert('Error: ' + xhr.status);
                    }
                });
                                }

                                function getEventsArray(data) {
                                    var results = [];

                                    for (var i = 0; i < data.length; i++) {
                                        var dataItem = data[i];

                                        results.push({
                                            id: dataItem.EventID,
                                            title: dataItem.Title,
                                            appointmentType: dataItem.AppointmentType,
                                            description: dataItem.Description,
                                            start: dataItem.StartDate,
                                            end: dataItem.EndDate,
                                            backgroundColor: dataItem.ThemeColor,
                                            start: dataItem.StartDate.toString(),
                                            end: dataItem.EndDate.toString(),
                                            url: dataItem.Url.toString(),
                                            color: dataItem.ThemeColor
                                        });
                                    }
                                    return results;
                                }

                                function generateCalendar(events) {
                                    var calendar = new FullCalendar.Calendar(calendarEl, {
                                        themeSystem: 'bootstrap5',
                                        headerToolbar: {
                                            left: 'customprev,customnext today',
                                            center: 'title',
                                            right: 'dayGridMonth,listWeek'
                                        },
                                        initialView: 'dayGridMonth',
                                        initialDate: new Date().toISOString().slice(0, 10),
                                        editable: false,
                                        dayMaxEventRows: false,
                                        events: events,
                                        eventTimeFormat: {
                                            hour: '2-digit',
                                            minute: '2-digit',
                                            second: '2-digit',
                                            hour12: true
                                        },
                                        eventDidMount: function (info) {
                                            const title = $(info.el).find('.fc-list-item-title > a');
                                            if (info.event.extendedProps.description !== undefined)
                                                title.html(title.text() + '<br><span class=\'small\'>' + info.event.extendedProps.description) + '</span>';

                                            const markerTr = info.el.querySelector(".fc-list-event-graphic");
                                            if (markerTr)
                                                markerTr.innerHTML = info.event.extendedProps.appointmentType || "";
                                        },
                                        customButtons: {
                                            customprev: {
                                                text: '<',
                                                click: function () {
                                                    var eventSources = calendar.getEventSources();
                                                    var len = eventSources.length;
                                                    for (var i = 0; i < len; i++) {
                                                        eventSources[i].remove();
                                                    }
                                                    calendar.prev();
                                                    var date = calendar.view.currentStart.format('yyyy-MM-dd');
                                                    $.when(getEvents(date)).done(function (result) {
                                                        calendar.addEventSource(getEventsArray(result));
                                                        calendar.refetchEvents();

                                                    });
                                                }
                                            },
                                            customnext: {
                                                text: '>',
                                                click: function () {
                                                    var eventSources = calendar.getEventSources();
                                                    var len = eventSources.length;
                                                    for (var i = 0; i < len; i++) {
                                                        eventSources[i].remove();
                                                    }
                                                    calendar.next();
                                                    var date = calendar.view.currentStart.format('yyyy-MM-dd');
                                                    $.when(getEvents(date)).done(function (result) {
                                                        calendar.addEventSource(getEventsArray(result));
                                                        calendar.refetchEvents();
                                                    });
                                                }
                                            }
                                        }
                                    });

                                    calendar.gotoDate('<%= CalendarDate %>');
                                    calendar.render();
                                }
                            });

                            </script>
                        </insite:PageFooterContent>

                    </div>

                </div>

            </div>

        </div>

    </section>

    <section class="pt-5">

        <div class="row">

            <div class="col-lg-6">

                <h3>Shortcuts</h3>

                <div class="row row-cols-1 row-cols-sm-2 row-cols-md-3 row-cols-lg-3 g-4">

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/events/registrations/search">
                            <div class="card-body text-center text-danger">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ShortcutRepeater_ctl00_CardIcon"><i class="far fa-id-card fa-3x mb-3"></i></span>
                                <h3 class="h5 nav-heading mb-2 ">Registrations</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/events/classes/search">
                            <div class="card-body text-center text-danger">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ShortcutRepeater_ctl01_CardIcon"><i class="fas fa-calendar-alt fa-3x mb-3"></i></span>
                                <h3 class="h5 nav-heading mb-2 ">Classes</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/contacts/groups/search?label=Employers">
                            <div class="card-body text-center text-danger">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ShortcutRepeater_ctl03_CardIcon"><i class="far fa-users fa-3x mb-3"></i></span>
                                <h3 class="h5 nav-heading mb-2 ">Employers</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/reports/dashboards?d=0">
                            <div class="card-body text-center text-danger">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ShortcutRepeater_ctl07_CardIcon"><i class="fas fa-comment-alt-edit fa-3x mb-3"></i></span>
                                <h3 class="h5 nav-heading mb-2 ">New Applications</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/messages/home">
                            <div class="card-body text-center">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ToolkitRepeater_ctl06_CardIcon"><i class="fas fa-paper-plane fa-3x mb-3"></i></span>
                                <h3 class="h5 text-nowrap nav-heading mb-2 ">Messages</h3>
                            </div>
                        </a>
                    </div>

                    <div class="col">
                        <a class="card card-hover card-tile border-1 shadow" href="/ui/admin/records/home">
                            <div class="card-body text-center">
                                <span id="ctl00_ctl00_BodyContent_BodyContent_ctl02_ToolkitRepeater_ctl07_CardIcon"><i class="fas fa-pencil-ruler fa-3x mb-3"></i></span>
                                <h3 class="h5 text-nowrap nav-heading mb-2 ">Records</h3>
                            </div>
                        </a>
                    </div>

                </div>

            </div>

            <div class="col-lg-6">

                <h3>Notifications</h3>

                <div class="card border-0 shadow-lg">
                    <div class="card-header">
                        <ul class="nav nav-tabs card-header-tabs" role="tablist">
                            <li class="nav-item"><a class="nav-link active" href="#result1" data-bs-toggle="tab" role="tab" aria-controls="result1" aria-selected="true">Registrations</a></li>
                        </ul>
                    </div>
                    <div class="card-body">
                        <uc:RecentCaseList runat="server" ID="RecentCases" />
                    </div>
                </div>

            </div>

        </div>

    </section>

</asp:Content>
