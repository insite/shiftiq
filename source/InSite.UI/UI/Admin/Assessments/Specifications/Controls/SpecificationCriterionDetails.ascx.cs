using System;
using System.Linq;

using Humanizer;

using InSite.Admin.Assessments.Web.UI;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Specifications.Controls
{
    public partial class SpecificationCriterionDetails : BaseUserControl
    {
        #region Properties

        public Guid? SpecificationID
        {
            get => (Guid?)ViewState[nameof(SpecificationID)];
            set => ViewState[nameof(SpecificationID)] = value;
        }

        public bool CanWrite
        {
            get => (bool)ViewState[nameof(CanWrite)];
            set => ViewState[nameof(CanWrite)] = value;
        }

        public Guid? CriterionID => CriterionDetail.CriterionID;

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SpecificationContent.ContentInitialization += SpecificationContent_ContentInitialization;
        }

        private void SpecificationContent_ContentInitialization(object sender, EventArgs e)
        {
            var repeater = (ContentRepeater)sender;
            var control = (SpecificationDetails)repeater.Control;
            var spec = (Specification)repeater.InitializationData;

            if (spec == null)
                return;

            control.SetInputValues(spec, CanWrite);
        }

        public void SetInputValues(Criterion sieve, bool canWrite)
        {
            CanWrite = canWrite;

            CriterionTab.Visible = true;
            CriterionTab.IsSelected = true;
            CriterionTab.SetTitle("Criterion", "Questions".ToQuantity(sieve.Sets.SelectMany(x => x.Questions).Count()));

            CriterionDetail.SetInputValues(sieve, canWrite);

            SpecificationContent.Key = $"spec.{sieve.Specification.Identifier}";
            SpecificationContent.InitializationData = sieve.Specification;
        }

        public void SetInputValues(Specification spec, bool canWrite)
        {
            CanWrite = canWrite;

            CriterionTab.Visible = false;
            SpecificationTab.IsSelected = true;

            SpecificationContent.Key = $"spec.{spec.Identifier}";
            SpecificationContent.InitializationData = spec;
        }

        public void OpenSpecificationTab() => SpecificationTab.IsSelected = true;

        public bool IsSpecificationTabOpened() => SpecificationTab.IsSelected;
    }
}