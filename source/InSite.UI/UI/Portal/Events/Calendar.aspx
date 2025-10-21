<%@ Page Language="C#" CodeBehind="Calendar.aspx.cs" Inherits="InSite.UI.Portal.Events.Calendar" MasterPageFile="~/UI/Layout/Portal/Portal.master" %>

<asp:Content runat="server" ContentPlaceHolderID="HeadContent">
    <style type="text/css">
        #calendar .btn-primary:disabled {
            color: #fff;
        }

        .fc-event-title, .fc-event-time {
            white-space: normal !important;
            word-wrap: break-word !important;
        }
        .fc-event-main {
            padding: 5px;
        }
        .fc-event-title {
          display: -webkit-box;
          -webkit-line-clamp: 2;
          -webkit-box-orient: vertical;
          overflow: hidden;
        }
    </style>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="BodyContent">

    <insite:Alert runat="server" ID="StatusAlert" />

    <div runat="server" id="MainPanel" class="main-panel">
        <div id='calendar'></div>
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

                        var additionalClass = dataItem.ThemeColor ? 'border-' + dataItem.ThemeColor : 'border-info';

                        results.push({
                            id: dataItem.EventID,
                            title: dataItem.Title,
                            appointmentType: dataItem.AppointmentType,
                            description: dataItem.Description,
                            start: dataItem.StartDate,
                            end: dataItem.EndDate,
                            start: dataItem.StartDate.toString(),
                            end: dataItem.EndDate.toString(),
                            url: dataItem.Url.toString(),
                            classNames: ["event-custom-style", "bg-light", "text-nav", "border-0", "rounded-1", "p-2", "ps-3", "border-start", "border-5", additionalClass],
                            extendedProps: {
                                backgroundColor: dataItem.ThemeColor
                            }
                            
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
                            right: 'dayGridMonth,timeGridWeek,listWeek'
                        },
                        initialView: 'dayGridMonth',
                        initialDate: new Date().toISOString().slice(0, 10),
                        editable: false,
                        dayMaxEventRows: false,
                        events: events,
                        eventTimeFormat: {
                            hour: '2-digit',
                            minute: '2-digit',
                            hour12: true
                        },
                        eventDidMount: function (info) {
                            const title = $(info.el).find('.fc-list-item-title > a');
                            if (info.event.extendedProps.description !== undefined)
                                title.html(title.text() + '<br><span class=\'small\'>' + info.event.extendedProps.description) + '</span>';

                            info.el.setAttribute('title', info.event.title);

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
    
</asp:Content>
