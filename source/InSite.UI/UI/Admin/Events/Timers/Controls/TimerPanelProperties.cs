using System;
using System.Web;
using System.Web.UI;

namespace InSite.Admin.Events.Timers.Controls
{
    public class TimerPanelProperties
    {
        public Guid EventIdentifier => Guid.Parse(_request.QueryString["event"]);
        public string ValidateAndPublishOption => "Validate and Publish Exam Results";
        public string EventOutlineUrl => $"/ui/admin/events/exams/outline?event={EventIdentifier}&panel=notification";

        private readonly HttpRequest _request;
        private readonly StateBag _state;

        public TimerPanelProperties(HttpRequest request, StateBag state)
        {
            _request = request;
            _state = state;
        }
    }
}