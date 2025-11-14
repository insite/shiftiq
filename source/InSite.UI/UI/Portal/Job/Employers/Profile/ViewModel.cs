using InSite.Application.Contacts.Read;
using InSite.Persistence;

namespace InSite.UI.Portal.Jobs.Employers.MyProfile
{
    public class ViewModel
    {
        public Person Person { get; set; }
        public QGroup Employer { get; set; }
        public QGroupAddress EmployerAddress { get; set; }
        public TGroupSetting[] Settings { get; set; }
    }
}