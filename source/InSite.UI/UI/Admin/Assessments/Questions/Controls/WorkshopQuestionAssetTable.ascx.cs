using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

using InSite.Application.Banks.Read;
using InSite.Common.Web.UI;
using InSite.Domain.Banks;
using InSite.Persistence;

using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Controls
{
    public partial class WorkshopQuestionAssetTable : BaseUserControl
    {
        #region Peoperties

        public Guid? FormIdentifier { get; set; }

        public Question Question { get; set; }

        public ReturnUrl ReturnUrl { get; set; }

        private List<TCollectionItem> Taxonomies => (List<TCollectionItem>)(Application[nameof(WorkshopQuestionAssetTable)]
            ?? (Application[nameof(WorkshopQuestionAssetTable)] = WorkshopQuestionRepeater.GetCollectionItems(CollectionName.Assessments_Questions_Classification_Taxonomy)));

        private static List<WorkshopQuestionAssetTable> AllTables
        {
            get
            {
                var key = typeof(WorkshopQuestionAssetTable) + "." + nameof(Controls);
                return (List<WorkshopQuestionAssetTable>)(HttpContext.Current.Items[key]
                    ?? (HttpContext.Current.Items[key] = new List<WorkshopQuestionAssetTable>()));
            }
        }

        private static Dictionary<Guid, QBankQuestion> Sources
        {
            get => (Dictionary<Guid, QBankQuestion>)HttpContext.Current.Items[typeof(WorkshopQuestionAssetTable) + "." + nameof(Sources)];
            set => HttpContext.Current.Items[typeof(WorkshopQuestionAssetTable) + "." + nameof(Sources)] = value;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            AllTables.Add(this);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Question != null && Question.Source.HasValue)
            {
                if (Sources == null)
                {
                    var ids = AllTables
                        .Where(x => x.Question?.Source != null)
                        .Select(x => x.Question.Source.Value)
                        .Distinct();
                    Sources = ServiceLocator.BankSearch.GetQuestions(ids).ToDictionary(x => x.QuestionIdentifier);
                }

                if (Sources.TryGetValue(Question.Source.Value, out var source))
                {
                    var redirectUrl = ReturnUrl.GetRedirectUrl(
                        $"/ui/admin/assessments/questions/analysis?bank={source.BankIdentifier}&question={source.QuestionIdentifier}", $"question={Question.Identifier}");

                    SourceCell.InnerHtml = $"<a href='{redirectUrl}'>{source.QuestionAssetNumber}</a>";
                }
            }

            base.OnPreRender(e);
        }

        protected string GetTaxonomy(object obj)
        {
            var rank = obj as int?;
            var taxonomy = rank.HasValue ? Taxonomies.Find(x => x.ItemNumber == rank) : null;

            return taxonomy != null ? $"{taxonomy.ItemSequence}. {taxonomy.ItemName}" : null;
        }

        protected string GetTaxonomy(object objTaxonomy, object objEnum)
        {
            var rank = objTaxonomy as int?;
            var taxonomy = rank.HasValue ? Taxonomies.Find(x => x.ItemSequence == rank) : null;

            var taxonomyRank = (taxonomy != null ? $"{taxonomy.ItemSequence}. {taxonomy.ItemName}" : null);

            if (objEnum == null)
                return taxonomyRank;

            if ((PublicationStatus)objEnum == PublicationStatus.Published)
                return taxonomyRank;

            return String.Format("<a href='#' class='editable-input taxonomy'" +
               " data-name='Tax'" +
               " data-type='select'" +
               " data-pk='{0}:{1}'" +
               " data-value='{2}'" +
           ">{3}</a>", Question.Set.Bank.Identifier, Question.Identifier, rank, taxonomyRank);
        }

        protected string GetEnumDescription(Enum value) => value.GetDescription();
    }
}