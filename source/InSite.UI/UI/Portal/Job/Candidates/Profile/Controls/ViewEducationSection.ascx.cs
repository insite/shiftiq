using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;


namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class ViewEducationSection : UserControl
    {
        private Guid UserId
        {
            get => (Guid)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

        public void BindModelToControls(Person person)
        {
            UserId = person.UserIdentifier;

            BindList();
        }

        private void BindList()
        {
            var data = TCandidateEducationSearch
                .Select(x => x.UserIdentifier == UserId, null)
                .OrderByDescending(x => x.EducationDateTo != null ? x.EducationDateTo.Value : DateTime.Now)
                .ThenBy(x => x.EducationInstitution)
                .Select(x => new
                {
                    x.EducationIdentifier,
                    x.EducationDateFrom,
                    x.EducationDateTo,
                    x.EducationStatus,
                    x.EducationName,
                    x.EducationInstitution,
                    x.EducationQualification,
                    x.EducationCity,
                    x.EducationCountry
                })
                .ToList();

            EducationList.DataSource = data;
            EducationList.DataBind();

            EducationList.Visible = data.Count > 0;
            NoEducationItems.Visible = data.Count == 0;
        }

        protected string GetYearString(object o)
        {
            var date = (DateTime?)o;

            return date.HasValue
                ? date.Value.Year.ToString()
                : "Current";
        }

        protected string EvalEncode(string name)
        {
            var dataItem = Page.GetDataItem();
            var text = (string)DataBinder.Eval(dataItem, name);

            return text.IsEmpty() ? string.Empty : HttpUtility.HtmlEncode(text);
        }

        protected string EvalAddress(string cityName, string countryName)
        {
            var dataItem = Page.GetDataItem();
            var arguments = new[]
            {
                (string)DataBinder.Eval(dataItem, cityName),
                (string)DataBinder.Eval(dataItem, countryName)
            };

            return string.Join(", ", arguments.Where(x => x.IsNotEmpty()).Select(x => HttpUtility.HtmlEncode(x)));
        }

        protected string EvalEducationStatus()
        {
            var dataItem = Page.GetDataItem();
            var value = (string)DataBinder.Eval(dataItem, "EducationStatus");

            return value.IsEmpty()
                ? string.Empty
                : $"({value})";
        }
    }
}