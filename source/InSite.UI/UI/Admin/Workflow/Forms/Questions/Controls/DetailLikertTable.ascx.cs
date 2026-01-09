using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web.UI;
using InSite.Domain.Surveys.Forms;

using Shift.Common;

namespace InSite.Admin.Workflow.Forms.Questions.Controls
{
    public partial class DetailLikertTable : UserControl
    {
        #region Classes

        private class TranslationList : ICollection<MultilingualString>
        {
            #region Properties

            public string FromLanguage { get; }

            public string ToLanguage { get; }

            public int Count => _list.Count;

            public bool IsReadOnly => false;

            #endregion

            #region Fields

            private List<MultilingualString> _list = new List<MultilingualString>();

            #endregion

            #region Construction 

            public TranslationList(string fromLang, string toLang)
            {
                FromLanguage = fromLang;
                ToLanguage = toLang;
            }

            #endregion

            #region Methods

            public void Add(MultilingualString item)
            {
                if (item != null && !string.IsNullOrEmpty(item[FromLanguage]) && string.IsNullOrEmpty(item[ToLanguage]))
                    _list.Add(item);
            }

            public void Clear()
            {
                _list.Clear();
            }

            public bool Contains(MultilingualString item)
            {
                return _list.Contains(item);
            }

            public void CopyTo(MultilingualString[] array, int arrayIndex)
            {
                _list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<MultilingualString> GetEnumerator()
            {
                return _list.GetEnumerator();
            }

            public bool Remove(MultilingualString item)
            {
                return _list.Remove(item);
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

            #endregion
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RowRequiredValidator.ServerValidate += RowRequiredValidator_ServerValidate;
        }

        private void RowRequiredValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = LikertTableRowGrid.HasItems || !LikertTableColumnGrid.HasItems;
        }

        internal void SetDefaultInputValues(string lang)
        {
            var question = new SurveyQuestion();
            LikertTableRowGrid.LoadData(question, lang);
            LikertTableColumnGrid.LoadData(question, lang);
        }

        public string SetInputValues(SurveyQuestion question, string lang)
        {
            ListEnableBranch.Checked = question.ListEnableBranch;
            ListDisableColumnHeadingWrap.Checked = question.ListDisableColumnHeadingWrap;

            // IsHidden.Checked = question.IsHidden;
            if (!string.IsNullOrEmpty(question.LikertAnalysis))
                LikertTableReporting.SelectedValue = question.LikertAnalysis;
            else if (question.IsHidden)
                LikertTableReporting.SelectedValue = "Preceding Questions, All Scales";

            var columns = LikertTableColumnGrid.LoadData(question, lang);
            var rows = LikertTableRowGrid.LoadData(question, lang);

            if (columns > 0 && rows > 0)
                return $"{columns} column(s), {rows} row(s)";

            if (columns > 0)
                return $"{columns} column(s)";

            if (rows > 0)
                return $"{rows} row(s)";

            return null;
        }

        public void GetInputValues(SurveyQuestion question)
        {
            if (question.Content == null)
                question.Content = new ContentContainer();

            question.ListEnableBranch = ListEnableBranch.Checked;
            question.ListDisableColumnHeadingWrap = ListDisableColumnHeadingWrap.Checked;

            question.IsHidden = LikertTableReporting.SelectedValue == "Preceding Questions, All Scales";
            question.LikertAnalysis = LikertTableReporting.SelectedValue;

            LikertTableRowGrid.Save(question);
            LikertTableColumnGrid.Save(question);
        }

        internal void Translate(string fromLang, string[] toLangs)
        {
            foreach (var toLang in toLangs)
            {
                var data = new TranslationList(fromLang, toLang);

                LikertTableRowGrid.AddContent(data);
                LikertTableColumnGrid.AddContent(data);

                if (data.Count > 0)
                    ((IHasTranslator)Page).Translator.Translate(fromLang, toLang, data);
            }
        }
    }
}