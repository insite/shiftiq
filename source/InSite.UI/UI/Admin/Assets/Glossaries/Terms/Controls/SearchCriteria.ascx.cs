using InSite.Admin.Assets.Glossaries.Utilities;
using InSite.Application.Glossaries.Read;
using InSite.Common.Web.UI;

namespace InSite.Admin.Assets.Glossaries.Terms.Controls
{
    public partial class SearchCriteria : SearchCriteriaController<GlossaryTermFilter>
    {
        public override GlossaryTermFilter Filter
        {
            get
            {
                var filter = new GlossaryTermFilter
                {
                    GlossaryIdentifier = GlossaryHelper.GlossaryIdentifier,

                    TermIdentifier = TermIdentifier.Text,
                    TermName = TermName.Text,
                    TermTitle = TermTitle.Text,
                    TermDefinition = TermDefinition.Text,
                    TermStatus = TermStatus.Value,
                    IsTranslated = IsTranslated.ValueAsBoolean,

                    RevisionCountFrom = RevisionCountFrom.ValueAsInt,
                    RevisionCountThru = RevisionCountThru.ValueAsInt,

                    ProposedBy = ProposedBy.Text,
                    ProposedSince = DateProposedSince.Value,
                    ProposedBefore = DateProposedBefore.Value,

                    ApprovedBy = ApprovedBy.Text,
                    ApprovedSince = DateApprovedSince.Value,
                    ApprovedBefore = DateApprovedBefore.Value,

                    LastRevisedBy = LastRevisedBy.Text,
                    LastRevisedSince = DateLastRevisedSince.Value,
                    LastRevisedBefore = DateLastRevisedBefore.Value
                };

                GetCheckedShowColumns(filter);

                return filter;
            }
            set
            {
                TermIdentifier.Text = value.TermIdentifier;
                TermName.Text = value.TermName;
                TermTitle.Text = value.TermTitle;
                TermDefinition.Text = value.TermDefinition;
                TermStatus.Value = value.TermStatus;
                IsTranslated.ValueAsBoolean = value.IsTranslated;

                RevisionCountFrom.ValueAsInt = value.RevisionCountFrom;
                RevisionCountThru.ValueAsInt = value.RevisionCountThru;

                ProposedBy.Text = value.ProposedBy;
                DateProposedSince.Value = value.ProposedSince;
                DateProposedBefore.Value = value.ProposedBefore;

                ApprovedBy.Text = value.ApprovedBy;
                DateApprovedSince.Value = value.ApprovedSince;
                DateApprovedBefore.Value = value.ApprovedBefore;

                LastRevisedBy.Text = value.LastRevisedBy;
                DateLastRevisedSince.Value = value.LastRevisedSince;
                DateLastRevisedBefore.Value = value.LastRevisedBefore;
            }
        }

        public override void Clear()
        {
            TermIdentifier.Text = null;
            TermName.Text = null;
            TermTitle.Text = null;
            TermDefinition.Text = null;
            TermStatus.ClearSelection();
            IsTranslated.ClearSelection();

            RevisionCountFrom.ValueAsInt = null;
            RevisionCountThru.ValueAsInt = null;

            ProposedBy.Text = null;
            DateProposedSince.Value = null;
            DateProposedBefore.Value = null;

            ApprovedBy.Text = null;
            DateApprovedSince.Value = null;
            DateApprovedBefore.Value = null;

            LastRevisedBy.Text = null;
            DateLastRevisedSince.Value = null;
            DateLastRevisedBefore.Value = null;
        }
    }
}