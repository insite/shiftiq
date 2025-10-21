using System;
using System.Linq;
using System.Net;

using InSite.Admin.Assessments.Banks;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Json;

namespace InSite.UI.Admin.Assessments.Fields.Forms
{
    public partial class Pin : AdminBasePage
    {
        private const string ErrorInvalidRequest = "Invalid request";
        private const string ErrorInvalidInputParameters = "Invalid input parameters";
        private const string ErrorUnexpectedAction = "Unexpected action: {0}";

        [JsonObject(MemberSerialization.OptIn)]
        protected class JsonPinResult
        {
            [JsonProperty(PropertyName = "numberOfPins")]
            public int NumberOfPins { get; set; }

            [JsonProperty(PropertyName = "pinned")]
            public bool Pinned { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }
        }

        protected override void OnLoad(EventArgs e)
        {
            Response.Clear();

            var action = Request["action"];
            if (action == "pin")
                ActionPin();
            else if (action == "unpinall")
                ActionUnpinAll();
            else
                ReturnError(HttpStatusCode.BadRequest, ErrorUnexpectedAction.Format(action));

            base.OnLoad(e);
        }

        private void ActionPin()
        {
            if (Request.HttpMethod != "POST")
                ReturnError(HttpStatusCode.BadRequest, ErrorInvalidRequest);

            if (!Guid.TryParse(Request["bank"], out var bankId))
                ReturnError(HttpStatusCode.BadRequest, ErrorInvalidInputParameters);

            if (!int.TryParse(Request["fieldAssetNumber"], out var fieldAssetNumber))
                ReturnError(HttpStatusCode.BadRequest, ErrorInvalidInputParameters);

            var model = PinModel.GetModel(bankId);
            var pinned = model.FieldAssetNumbers.Contains(fieldAssetNumber);
            var status = "ok";

            if (pinned)
            {
                model.FieldAssetNumbers.Remove(fieldAssetNumber);
            }
            else if (model.FieldAssetNumbers.Count == 2)
            {
                status = "too much";
            }
            else if (model.FieldAssetNumbers.Count > 0)
            {
                var fieldAssetNumber1 = model.FieldAssetNumbers.FirstOrDefault();

                var data = ServiceLocator.BankSearch.GetBankState(bankId);
                var forms = data.Specifications.SelectMany(x => x.EnumerateAllForms()).ToList();
                var sections = forms.SelectMany(x => x.Sections);
                var section1 = sections.FirstOrDefault(x => x.Fields.FirstOrDefault(y => y.Question.Asset == fieldAssetNumber1) != null);
                var section2 = sections.FirstOrDefault(x => x.Fields.FirstOrDefault(y => y.Question.Asset == fieldAssetNumber) != null);

                if (section1 == section2)
                    status = "same section";
                else
                    model.FieldAssetNumbers.Add(fieldAssetNumber);
            }
            else
            {
                model.FieldAssetNumbers.Add(fieldAssetNumber);
            }

            ReturnResult(new JsonPinResult
            {
                NumberOfPins = model.FieldAssetNumbers.Count,
                Pinned = !pinned,
                Status = status
            });
        }

        private void ActionUnpinAll()
        {
            if (Request.HttpMethod != "POST")
                ReturnError(HttpStatusCode.BadRequest, ErrorInvalidRequest);

            if (!Guid.TryParse(Request["bank"], out var bankId))
                ReturnError(HttpStatusCode.BadRequest, ErrorInvalidInputParameters);

            var model = PinModel.GetModel(bankId);

            if (model != null)
                model.FieldAssetNumbers.Clear();

            ReturnResult(new JsonPinResult());
        }

        private void ReturnError(HttpStatusCode status, string text)
        {
            var result = new JsonErrorResult(text);

            Response.StatusCode = (int)status;

            ReturnResult(result);
        }

        private void ReturnResult<T>(T result)
        {
            var json = JsonConvert.SerializeObject(result);

            Response.ContentType = "application/json";
            Response.Write(json);
            Response.End();
        }
    }
}