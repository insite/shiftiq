using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using InSite.Admin.Assessments.Forms.Models;
using InSite.Api.Settings;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Assessment")]
    public class FormsController : ApiBaseController
    {
        public static string FormsUrl = "/api/assessments/forms/";

        [HttpPost]
        [Route("api/assessments/forms/setelementvalue")]
        [ApiAuthenticationRequirement(ApiAuthenticationType.Cookie)]
        public HttpResponseMessage SetElementValue([FromBody] FormsElement e)
        {
            try
            {
                var name = e.name;
                var pk = e.pk;
                var value = e.value;

                var newValue = ElementUpdater.SetElementValue(name, pk, value);

                if (name.Equals(ElementUpdater.ElementTypes.QuestionFlag))
                    return JsonSuccess(new { iconHtml = value.ToEnum<FlagType>().ToIconHtml() });

                if (name.Equals(ElementUpdater.ElementTypes.OptionPoints))
                    return JsonSuccess(new { points = $"{newValue:n2}" });

                var isHtml = name.Equals(ElementUpdater.ElementTypes.QuestionTitle)
                    || name.Equals(ElementUpdater.ElementTypes.OptionTitle)
                    || name.Equals(ElementUpdater.ElementTypes.OptionHeaderColumn)
                    || name.Equals(ElementUpdater.ElementTypes.OptionTitleColumn);

                if (isHtml)
                    return JsonSuccess(new { html = Markdown.ToHtml((string)newValue) });

                return JsonSuccess(string.Empty);
            }
            catch (ApplicationError apperr)
            {
                return JsonError(apperr.Message, HttpStatusCode.BadRequest);
            }
        }
    }

    public class FormsElement
    {
        public string name { get; set; }
        public string pk { get; set; }
        public string value { get; set; }
    }
}