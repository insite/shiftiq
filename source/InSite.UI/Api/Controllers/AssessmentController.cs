using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;

using InSite.Api.Settings;
using InSite.Application.Banks.Read;
using InSite.Application.Banks.Write;

using Shift.Common;
using Shift.Sdk.UI;

namespace InSite.Api.Controllers
{
    [DisplayName("Assessment")]
    public partial class AssessmentsController : ApiBaseController
    {
        [HttpGet]
        [Route("api/assessments/hide-comment")]
        public HttpResponseMessage Hide()
        {
            var request = HttpContext.Current.Request;

            if (!Guid.TryParse(request["bank"], out var bankId))
                bankId = Guid.Empty;

            if (!Guid.TryParse(request["comment"], out var commentId))
                commentId = Guid.Empty;

            var isHidden = GetBool(request["hide"]);

            if (bankId == Guid.Empty || commentId == Guid.Empty || !isHidden.HasValue)
                return JsonSuccess("Invalid request");

            SendCommand(new ChangeCommentVisibility(bankId, commentId, isHidden.Value));

            return JsonSuccess("OK");
        }

        private static bool? GetBool(string value)
        {
            return value == "1" ? true : value == "0" ? false : (bool?)null;
        }

        [HttpGet]
        [Route("api/assessments/list-forms")]
        public HttpResponseMessage List()
        {
            var request = HttpContext.Current.Request;

            var keyword = request["keyword"];

            var published = false;
            if (bool.TryParse(request["published"], out bool isPublished))
                published = isPublished;

            var startRow = 1;
            if (int.TryParse(request["start"], out int i))
                startRow = i;

            var endRow = i + 10;
            if (int.TryParse(request["end"], out int j))
                endRow = j;

            var filter = new QBankFormFilter
            {
                OrganizationIdentifier = CurrentOrganization.OrganizationIdentifier,
                IncludeFormStatus = published ? "Published" : null,
                Paging = Paging.SetStartEnd(startRow, endRow),
                OrderBy = "FormTitle"
            };

            if (!string.IsNullOrEmpty(keyword))
                filter.Keyword = keyword;

            var forms = ServiceLocator.BankSearch.GetForms(filter);
            var bankFilter = forms.Length > 0 ? forms.Select(x => x.BankIdentifier).Distinct().ToArray() : new Guid[0];
            var banks = ServiceLocator.BankSearch.GetBanks(bankFilter).ToDictionary(x => x.BankIdentifier);
            var result = forms
                .Select(x =>
                {
                    var bank = banks[x.BankIdentifier];

                    return new ListResultItemModel
                    {
                        FormIdentifier = x.FormIdentifier,
                        FormAsset = x.FormAsset,
                        FormAssetVersion = x.FormAssetVersion,
                        FormName = x.FormName,
                        FormTitle = x.FormTitle ?? x.FormName,

                        BankId = bank.BankIdentifier,
                        BankTitle = bank.BankTitle,
                        BankAsset = bank.AssetNumber
                    };
                })
                .OrderBy(x => x.FormTitle).ThenBy(x => x.FormName)
                .ToArray();

            return JsonSuccess(result);
        }
    }
}