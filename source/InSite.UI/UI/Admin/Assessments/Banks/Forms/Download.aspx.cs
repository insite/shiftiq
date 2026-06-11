using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using InSite.Application.Banks.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.UI.Layout.Admin;

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using Shift.Common;

namespace InSite.Admin.Assessments.Banks.Forms
{
    public partial class Download : AdminBasePage, IHasParentLinkParameters
    {
        private class IgnoreJsonIgnoreResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(
                MemberInfo member,
                MemberSerialization memberSerialization)
            {
                var property = base.CreateProperty(member, memberSerialization);
                var propertyAttr = member.GetCustomAttribute<JsonPropertyAttribute>();

                if (propertyAttr != null
                    && propertyAttr.PropertyName == "QuestionRef"
                    && member.Name == nameof(Field.Question)
                    && member.GetCustomAttribute<JsonIgnoreAttribute>() != null
                    )
                {
                    property.Ignored = false;
                    property.ShouldSerialize = _ => true;
                }

                return property;
            }
        }

        private Guid? BankId => Guid.TryParse(Request.QueryString["bank"], out var value) ? value : (Guid?)null;
        private Guid? FormId => Guid.TryParse(Request.QueryString["form"], out var value) ? value : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DownloadButton.Click += DownloadButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (IsPostBack)
                return;

            var bankQuery = BankId.HasValue ? ServiceLocator.BankSearch.GetBank(BankId.Value) : null;
            if (bankQuery == null || bankQuery.OrganizationIdentifier != Organization.Identifier)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

            var bank = ServiceLocator.BankSearch.GetBankState(BankId.Value);
            var form = FormId.HasValue ? bank.FindForm(FormId.Value) : null;

            if (form == null && FormId.HasValue)
                HttpResponseHelper.Redirect($"/ui/admin/assessments/banks/search", true);

            if (form != null)
            {
                FormPanel.Visible = true;
                FormDetails.BindForm(form, BankId.Value);
            }
            else
            {
                BankPanel.Visible = true;
                BankDetails.BindBank(bank);
            }

            Setup(bank, form);
        }

        private void Setup(BankState bank, Form form)
        {
            FileName.Text = form != null ? form.Name : bank.Name;

            var title = form != null ? form.Name : (bank.Content.Title.Default ?? bank.Name);
            var assetNumber = form != null ? form.Asset : bank.Asset;

            PageHelper.AutoBindHeader(
                this,
                qualifier: $"{title} <span class='form-text'>Asset #{assetNumber}</span>");

            CancelLink.NavigateUrl = $"/ui/admin/assessments/banks/outline?{GetParams()}";
        }

        private void DownloadButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var fileFormat = FileFormatSelector.SelectedValue;

            if (fileFormat == "JSON")
            {
                SendJson();
            }
        }

        private void SendJson()
        {
            var bank = ServiceLocator.BankSearch.GetBankState(BankId.Value);

            string json;

            if (FormId.HasValue)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new IgnoreJsonIgnoreResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore
                };

                var form = bank.FindForm(FormId.Value) ?? throw new ArgumentException($"Form {FormId} is not found");

                json = JsonConvert.SerializeObject(form, Formatting.Indented, settings);
            }
            else
                json = JsonHelper.JsonExport(bank);

            var bytes = Encoding.UTF8.GetBytes(json);

            if (CompressionMode.Value == "ZIP")
                SendZipFile(bytes, FileName.Text, "json");
            else
                Response.SendFile(FileName.Text, "json", bytes);
        }



        string IHasParentLinkParameters.GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? GetParams()
                : null;
        }

        private string GetParams()
        {
            return FormId.HasValue
                ? $"bank={BankId}&form={FormId}"
                : $"bank={BankId}";
        }
    }
}