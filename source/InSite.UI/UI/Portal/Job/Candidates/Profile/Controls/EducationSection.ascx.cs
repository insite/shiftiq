using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Portal.Jobs.Candidates.MyPortfolio.Controls
{
    public partial class EducationSection : UserControl
    {
        private Guid UserId
        {
            get => (Guid)ViewState[nameof(UserId)];
            set => ViewState[nameof(UserId)] = value;
        }

        public int ItemsCount
        {
            get => (int)(ViewState[nameof(ItemsCount)] ?? 0);
            private set => ViewState[nameof(ItemsCount)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ItemsRequiredValidator.ServerValidate += ItemsRequiredValidator_ServerValidate;

            EducationList.ItemCommand += EducationList_ItemCommand;
        }

        private void ItemsRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = ItemsCount > 0;
        }

        private void EducationList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName != "Delete")
                return;

            var id = Guid.Parse(e.CommandArgument.ToString());

            TCandidateEducationStore.Delete(id);

            BindList();
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

            ItemsCount = data.Count;

            EducationList.DataSource = data;
            EducationList.DataBind();

            EducationList.Visible = ItemsCount > 0;
            NoEducationItems.Visible = ItemsCount == 0;
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