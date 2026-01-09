using Shift.Common;
using Shift.Constant;

namespace InSite.Common.Web.UI
{
    public class EventTypeMultiComboBox : MultiComboBox
    {
        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add(EventType.Appointment.ToString(), EventType.Appointment.ToString());
            list.Add(EventType.Class.ToString(), EventType.Class.ToString());
            list.Add(EventType.Exam.ToString(), EventType.Exam.ToString());

            return list;
        }
    }
}