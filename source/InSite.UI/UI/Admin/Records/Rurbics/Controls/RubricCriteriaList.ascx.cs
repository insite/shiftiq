using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using Shift.Common.Timeline.Commands;

using InSite.Application.Records.Read;
using InSite.Application.Rubrics.Write;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Admin.Records.Rurbics.Controls
{
    public partial class RubricCriteriaList : BaseUserControl
    {
        #region Classes

        public class RatingItem
        {
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public decimal? Points { get; set; }
            public string RangeCriteria { get; set; }
        }

        public class CriterionItem
        {
            public Guid Identifier { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public bool Range { get; set; }

            public decimal Points => Ratings.Max(x => x.Points ?? 0);

            public List<RatingItem> Ratings { get; set; }
        }

        #endregion

        #region Properties

        protected bool AllowCriterionDelete { get; private set; }

        protected int RatingCount { get; private set; }

        private CriterionItem CurrentCriterion { get; set; }

        #endregion

        #region Initialization & Even Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            CriteriaRepeater.ItemCreated += CriteriaRepeater_ItemCreated;
            CriteriaRepeater.ItemDataBound += CriteriaRepeater_ItemDataBound;
            CriteriaRepeater.ItemCommand += CriteriaRepeater_ItemCommand;

            AddNewCriterionButton.Click += AddNewCriterionButton_Click;
        }

        private void CriteriaRepeater_ItemCreated(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");
            ratingRepeater.ItemCommand += RatingRepeater_ItemCommand;

            var rangeCheckBox = (ICheckBoxControl)e.Item.FindControl("Range");
            rangeCheckBox.CheckedChanged += RangeCheckBox_CheckedChanged;
        }

        private void RangeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            var criteria = GetCriteria();

            BindCriteria(criteria);
        }

        private void CriteriaRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var criterion = (CriterionItem)e.Item.DataItem;
            var pointsNumericBox = (NumericBox)e.Item.FindControl("Points");

            pointsNumericBox.NumericMode = criterion.Range
                ? NumericBoxMode.Integer
                : NumericBoxMode.Float;

            var ratingRepeater = (Repeater)e.Item.FindControl("RatingRepeater");

            RatingCount = criterion.Ratings.Count;
            CurrentCriterion = criterion;

            ratingRepeater.ItemDataBound += RatingRepeater_ItemDataBound;
            ratingRepeater.DataSource = criterion.Ratings;
            ratingRepeater.DataBind();
        }

        private void CriteriaRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var criterionIndex = e.Item.ItemIndex;
            var criteria = GetCriteria();

            if (e.CommandName == "DeleteCriterion")
                criteria.RemoveAt(criterionIndex);
            else if (e.CommandName == "DuplicateCriterion")
            {
                var clone = DuplicateCriterion(criteria[criterionIndex]);
                clone.Title = $"Criterion {criteria.Count + 1}";

                criteria.Insert(0, clone);
            }

            BindCriteria(criteria);
        }

        private void RatingRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var notRangeDiv = e.Item.FindControl("NotRangeDiv");
            var rangeDiv = e.Item.FindControl("RangeDiv");

            notRangeDiv.Visible = !CurrentCriterion.Range;
            rangeDiv.Visible = CurrentCriterion.Range;
        }

        private void RatingRepeater_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            var criterionIndex = ((RepeaterItem)((Repeater)e.Item.Parent).NamingContainer).ItemIndex;
            var ratingIndex = e.Item.ItemIndex;
            var criteria = GetCriteria();
            var criterion = criteria[criterionIndex];

            if (e.CommandName == "AddRating")
            {
                var first = criterion.Ratings[ratingIndex].Points ?? 0;
                var second = criterion.Ratings[ratingIndex + 1].Points ?? 0;

                var rating = new RatingItem
                {
                    Points = Math.Round(first + (second - first) / 2, 0)
                };

                criterion.Ratings.Insert(ratingIndex + 1, rating);
            }
            else if (e.CommandName == "DeleteRating")
            {
                criterion.Ratings.RemoveAt(ratingIndex);
            }

            BindCriteria(criteria);
        }

        private void AddNewCriterionButton_Click(object sender, EventArgs e)
        {
            var criteria = GetCriteria();
            var newCriterion = CreateTemplate(criteria.Count + 1);

            criteria.Insert(0, newCriterion);

            BindCriteria(criteria);
        }

        #endregion

        #region Load & duplicate

        public void LoadData()
        {
            BindCriteria(new List<CriterionItem> { CreateTemplate(1) });
        }

        public void LoadData(QRubric rubric)
        {
            var criteria = ServiceLocator.RubricSearch.GetCriteria(rubric.RubricIdentifier, x => x.RubricRatings);
            var criteriaItems = new List<CriterionItem>();

            foreach (var criterion in criteria.OrderBy(x => x.CriterionSequence))
            {
                var criterionItem = new CriterionItem
                {
                    Identifier = criterion.RubricCriterionIdentifier,
                    Title = criterion.CriterionTitle,
                    Description = criterion.CriterionDescription,
                    Range = criterion.IsRange,
                    Ratings = criterion.RubricRatings
                        .OrderBy(x => x.RatingSequence)
                        .Select(x => new RatingItem
                        {
                            Identifier = x.RubricRatingIdentifier,
                            Title = x.RatingTitle,
                            Description = x.RatingDescription,
                            Points = x.RatingPoints
                        })
                        .ToList()
                };

                criteriaItems.Add(criterionItem);
            }

            if (criteriaItems.Count == 0)
                criteriaItems.Add(CreateTemplate(1));

            BindCriteria(criteriaItems);
        }

        private void BindCriteria(List<CriterionItem> criteria)
        {
            foreach (var criterion in criteria)
                SetRangeCriteria(criterion);

            AllowCriterionDelete = criteria.Count > 1;

            CriteriaRepeater.DataSource = criteria;
            CriteriaRepeater.DataBind();
        }

        private void SetRangeCriteria(CriterionItem criterion)
        {
            if (!criterion.Range)
                return;

            for (int i = 0; i < criterion.Ratings.Count - 1; i++)
                criterion.Ratings[i].RangeCriteria = $"to >{criterion.Ratings[i + 1].Points:n0}";
        }

        private static CriterionItem CreateTemplate(int criterionNumber)
        {
            var criterion = new CriterionItem
            {
                Title = $"Criterion {criterionNumber}",
                Ratings = new List<RatingItem>
                {
                    new RatingItem { Title = "Full Marks", Points = 5 },
                    new RatingItem { Title = "No Marks", Points = 0 },
                }
            };

            return criterion;
        }

        private CriterionItem DuplicateCriterion(CriterionItem original)
        {
            var clone = new CriterionItem();
            clone.Title = original.Title;
            clone.Description = original.Description;
            clone.Range = original.Range;

            clone.Ratings = new List<RatingItem>();

            foreach (var rating in original.Ratings)
            {
                var cloneRating = new RatingItem();
                cloneRating.Title = rating.Title;
                cloneRating.Description = rating.Description;
                cloneRating.Points = rating.Points;

                clone.Ratings.Add(cloneRating);
            }

            return clone;
        }

        #endregion

        #region Get Criteria

        public List<CriterionItem> GetCriteria()
        {
            var criteria = new List<CriterionItem>();

            foreach (RepeaterItem criterionItem in CriteriaRepeater.Items)
            {
                var criterion = GetCriterionItem(criterionItem);
                criterion.Ratings = new List<RatingItem>();

                criteria.Add(criterion);

                criterion.Ratings = new List<RatingItem>();

                var ratingRepeater = (Repeater)criterionItem.FindControl("RatingRepeater");

                foreach (RepeaterItem ratingItem in ratingRepeater.Items)
                {
                    var rating = GetRatingItem(criterion.Range, ratingItem);
                    criterion.Ratings.Add(rating);
                }
            }

            return criteria;
        }

        private static CriterionItem GetCriterionItem(RepeaterItem criterionItem)
        {
            var identifierLiteral = (ITextControl)criterionItem.FindControl("Identifier");
            var titleTextBox = (ITextBox)criterionItem.FindControl("Title");
            var descriptionTextBox = (ITextBox)criterionItem.FindControl("Description");
            var rangeCheckBox = (ICheckBoxControl)criterionItem.FindControl("Range");

            return new CriterionItem
            {
                Identifier = Guid.Parse(identifierLiteral.Text),
                Title = titleTextBox.Text,
                Description = descriptionTextBox.Text,
                Range = rangeCheckBox.Checked
            };
        }

        private static RatingItem GetRatingItem(bool range, RepeaterItem ratingItem)
        {
            var identifierLiteral = (ITextControl)ratingItem.FindControl("Identifier");
            var titleTextBox = (ITextBox)ratingItem.FindControl("Title");

            var pointsNumericBox = range
                ? (NumericBox)ratingItem.FindControl("RangePoints")
                : (NumericBox)ratingItem.FindControl("NotRangePoints");

            var descriptionTextBox = (ITextBox)ratingItem.FindControl("Description");

            return new RatingItem
            {
                Identifier = Guid.Parse(identifierLiteral.Text),
                Title = titleTextBox.Text,
                Points = pointsNumericBox.ValueAsDecimal,
                Description = descriptionTextBox.Text
            };
        }

        #endregion

        #region Save

        public void Save(Guid rubricIdentifier, List<ICommand> commands) => Save(rubricIdentifier, GetCriteria(), commands);

        public static void Save(Guid rubricId, List<CriterionItem> items, List<ICommand> commands)
        {
            SortRatings(items);

            var entities = ServiceLocator.RubricSearch.GetCriteria(rubricId, x => x.RubricRatings);

            AppendCriteriaCommands(rubricId, entities, items, commands);
        }

        private static void AppendCriteriaCommands(Guid rubricId, IEnumerable<QRubricCriterion> entities, IList<CriterionItem> items, List<ICommand> commands)
        {
            foreach (var entity in entities)
            {
                if (!items.Any(x => x.Identifier == entity.RubricCriterionIdentifier))
                    commands.Add(new RemoveRubricCriterion(entity.RubricIdentifier, entity.RubricCriterionIdentifier));
            }

            for (var i = items.Count - 1; i >= 0; i--)
            {
                var item = items[i];
                var entity = entities.FirstOrDefault(x => x.RubricCriterionIdentifier == item.Identifier);
                var criterionTitle = item.Title.IfNullOrEmpty("Criterion");

                if (entity == null)
                {
                    entity = new QRubricCriterion
                    {
                        RubricIdentifier = rubricId,
                        RubricCriterionIdentifier = UniqueIdentifier.Create()
                    };
                    commands.Add(new AddRubricCriterion(rubricId, entity.RubricCriterionIdentifier, criterionTitle, item.Range, 1));
                }
                else
                {
                    commands.Add(new RenameRubricCriterion(rubricId, entity.RubricCriterionIdentifier, criterionTitle));
                    commands.Add(new ModifyRubricCriterionIsRange(rubricId, entity.RubricCriterionIdentifier, item.Range));
                }

                commands.Add(new DescribeRubricCriterion(rubricId, entity.RubricCriterionIdentifier, item.Description));

                AppendRatingsCommands(entity, item, commands);
            }
        }

        private static void AppendRatingsCommands(QRubricCriterion entity, CriterionItem item, List<ICommand> commands)
        {
            foreach (var ratingEntity in entity.RubricRatings)
            {
                if (!item.Ratings.Any(x => x.Identifier == ratingEntity.RubricRatingIdentifier))
                    commands.Add(new RemoveRubricCriterionRating(entity.RubricIdentifier, ratingEntity.RubricRatingIdentifier));
            }

            for (var i = 0; i < item.Ratings.Count; i++)
            {
                var ratingItem = item.Ratings[i];
                var ratingEntity = entity.RubricRatings.FirstOrDefault(x => x.RubricRatingIdentifier == ratingItem.Identifier);
                var title = ratingItem.Title.IfNullOrEmpty("Rating");

                if (ratingEntity == null)
                {
                    ratingEntity = new QRubricRating { RubricRatingIdentifier = UniqueIdentifier.Create() };
                    commands.Add(new AddRubricCriterionRating(entity.RubricIdentifier, entity.RubricCriterionIdentifier, ratingEntity.RubricRatingIdentifier, title, ratingItem.Points ?? 0, i + 1));
                }
                else
                {
                    commands.Add(new RenameRubricCriterionRating(entity.RubricIdentifier, ratingEntity.RubricRatingIdentifier, title));
                    commands.Add(new ModifyRubricCriterionRatingPoints(entity.RubricIdentifier, ratingEntity.RubricRatingIdentifier, ratingItem.Points ?? 0));
                }

                commands.Add(new DescribeRubricCriterionRating(entity.RubricIdentifier, ratingEntity.RubricRatingIdentifier, ratingItem.Description));
            }
        }

        private static void SortRatings(List<CriterionItem> criteria)
        {
            foreach (var criterion in criteria)
            {
                var ratings = new List<(int Sequence, RatingItem Rating)>();

                foreach (var rating in criterion.Ratings)
                    ratings.Add((ratings.Count, rating));

                criterion.Ratings = ratings
                    .OrderByDescending(x => x.Rating.Points ?? 0)
                    .ThenBy(x => x.Sequence)
                    .Select(x => x.Rating)
                    .ToList();
            }
        }

        public void ResetData() => BindCriteria(new List<CriterionItem> { CreateTemplate(1) });

        #endregion
    }
}