using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Records
{
    [Serializable]
    public class GradebookState : AggregateState
    {
        #region Fields and properties

        private List<GradeItem> _allItems;

        public Guid Identifier { get; set; }
        public Guid Tenant { get; set; }
        public Guid? PrimaryEvent { get; set; }
        public Guid? Achievement { get; set; }
        public Guid? Framework { get; set; }
        public Guid? Period { get; set; }
        public string Name { get; set; }
        public GradebookType Type { get; set; }
        public string Reference { get; set; }

        public List<GradeItem> RootItems { get; set; }
        public List<Enrollment> Enrollments { get; set; }
        public List<GradebookValidationScore> ValidationScores { get; set; }
        public HashSet<Guid> Events { get; set; } = new HashSet<Guid>();

        public bool IsOpen { get; set; }
        public bool IsLocked { get; set; }

        [JsonIgnore]
        public int AllItemCount => _allItems.Count;

        [JsonIgnore]
        public List<GradeItem> AllItems => _allItems;

        public bool ContainsLearner(Guid learner) => Enrollments.Any(x => x.Learner == learner);

        #endregion

        #region Methods

        public List<GradeItem> GetItemsWithAchievements()
        {
            return _allItems.Where(x => x.Achievement != null).ToList();
        }

        public HashSet<Guid> GetCompetencies()
        {
            var values = _allItems
                .Where(x => x.Competencies.IsNotEmpty())
                .SelectMany(x => x.Competencies)
                .Distinct();

            return new HashSet<Guid>(values);
        }

        public GradeItem GetItem(Func<GradeItem, bool> filter)
        {
            return _allItems.Find(x => filter(x));
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _allItems = new List<GradeItem>();

            RestoreReferences(RootItems, null);
        }

        private void RestoreReferences(List<GradeItem> items, GradeItem parent)
        {
            if (items == null)
                return;

            foreach (var item in items)
            {
                item.Parent = parent;

                _allItems.Add(item);

                RestoreReferences(item.Children, item);
            }
        }

        #endregion

        #region Managing Keys for Gradebook Items

        public bool ContainsCode(string code) => _allItems.Any(x => x.Code == code);

        public bool ContainsItem(Guid item) => _allItems.Any(x => x.Identifier == item);

        public GradeItem FindItem(Guid item) => _allItems.SingleOrDefault(x => x.Identifier == item);
        public GradeItem FindItem(string hook) => _allItems.SingleOrDefault(x => x.Hook == hook);

        public string GetNextCode()
        {
            var maxCode = _allItems.IsNotEmpty() ? _allItems.Max(x => int.TryParse(x.Code, out var value) ? value : -1) : -1;
            var nextCode = maxCode >= 0 ? maxCode + 1 : 1;

            return nextCode.ToString();
        }

        #endregion

        #region Changes

        public void When(GradebookAchievementChanged e)
        {
            Achievement = e.Achievement;
        }

        public void When(GradeItemNotificationsChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Notifications = e.Notifications;
        }

        public void When(GradebookCalculated _)
        {
        }

        public void When(GradebookCreated e)
        {
            _allItems = new List<GradeItem>();

            Identifier = e.AggregateIdentifier;
            Tenant = e.Tenant;
            Name = e.Name;
            Type = e.Type;
            PrimaryEvent = e.Event;
            Achievement = e.Achievement;
            Framework = e.Framework;
            RootItems = new List<GradeItem>();
            Enrollments = new List<Enrollment>();

            if (e.Event.HasValue)
                Events.Add(e.Event.Value);

            IsOpen = true;
        }

        public void When(GradebookEventChanged e)
        {
            if (PrimaryEvent == e.Event)
                return;

            PrimaryEvent = e.Event;

            Events.Clear();

            if (e.Event.HasValue)
                Events.Add(e.Event.Value);
        }

        public void When(GradebookEventAdded e)
        {
            Events.Add(e.Event);

            if (e.IsPrimary)
                PrimaryEvent = e.Event;
        }

        public void When(GradebookEventRemoved e)
        {
            Events.Remove(e.Event);

            PrimaryEvent = e.NewPrimaryEvent;
        }

        public void When(GradebookLocked _)
        {
            IsLocked = true;
        }

        public void When(GradebookReferenced e)
        {
            Reference = e.Reference;
        }

        public void When(GradebookRenamed e)
        {
            Name = e.Name;
        }

        public void When(GradebookTypeChanged e)
        {
            Type = e.Type;
            Framework = e.Framework;
        }

        public void When(GradebookPeriodChanged e)
        {
            Period = e.Period;
        }

        public void When(GradebookUnlocked _)
        {
            IsLocked = false;
        }

        public void When(GradebookDeleted _)
        {
            IsOpen = false;
        }

        public void When(GradebookWarningAdded e)
        {

        }

        public void When(GradeItemAchievementChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Achievement = e.Achievement;
        }

        public void When(GradeItemAdded e)
        {
            if (ContainsItem(e.Item))
                return;

            var item = new GradeItem
            {
                Identifier = e.Item,
                Sequence = e.Sequence,
                Code = e.Code,
                Name = e.Name,
                ShortName = e.ShortName,
                IsReported = e.IsReported,
                Format = e.Format,
                Type = e.Type,
                Weighting = e.Weighting,
                PassPercent = e.PassPercent
            };

            _allItems.Add(item);

            if (e.Parent.HasValue)
            {
                var parent = FindItem(e.Parent.Value);
                item.Parent = parent;
                parent.Children.Add(item);
            }
            else
            {
                RootItems.Add(item);
            }
        }

        public void When(GradeItemCalculationChanged e)
        {
            if (!ContainsItem(e.Item))
                return;

            var item = FindItem(e.Item);
            item.Parts = e.Parts;
        }

        public void When(GradeItemChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Code = e.Code;
            item.Name = e.Name;
            item.ShortName = e.ShortName;
            item.IsReported = e.IsReported;
            item.Format = e.Format;
            item.Type = e.Type;
            item.Weighting = e.Weighting;

            if (item.Parent?.Identifier != e.Parent)
            {
                if (item.Parent == null)
                    RootItems.Remove(item);
                else
                    item.Parent.Children.Remove(item);

                if (e.Parent.HasValue)
                {
                    var parent = FindItem(e.Parent.Value);
                    item.Parent = parent;
                    parent.Children.Add(item);
                }
                else
                {
                    item.Parent = null;
                    RootItems.Add(item);
                }
            }
        }

        public void When(GradeItemCompetenciesChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Competencies = e.Competencies;
        }

        public void When(GradeItemHookChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Hook = e.Hook;
        }

        public void When(GradeItemMaxPointsChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.MaxPoints = e.MaxPoints;
        }

        public void When(GradeItemPassPercentChanged e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.PassPercent = e.PassPercent;
        }

        public void When(GradeItemReferenced e)
        {
            var item = FindItem(e.Item);
            if (item == null)
                return;

            item.Reference = e.Reference;
        }

        public void When(GradeItemDeleted e)
        {
            var item = _allItems.Find(x => x.Identifier == e.Item);

            if (item.Children.IsNotEmpty())
                return;

            foreach (var calculation in _allItems)
                if (calculation.Parts != null && calculation.Parts.Any(x => x.Item == e.Item))
                    calculation.Parts = calculation.Parts.Where(x => x.Item != e.Item).ToArray();

            if (item.Parent == null)
                RootItems.Remove(item);
            else
                item.Parent.Children.Remove(item);

            _allItems.Remove(item);
        }

        public void When(GradeItemReordered e)
        {
            var children = e.Parent.HasValue ? _allItems.Find(x => x.Identifier == e.Parent)?.Children : RootItems;
            if (children == null
                || children.Count != e.Children.Length
                || children.Count(x => e.Children.Contains(x.Identifier)) != e.Children.Length
                )
            {
                return;
            }

            children.Clear();
            for (int i = 0; i < e.Children.Length; i++)
            {
                var key = e.Children[i];
                var child = _allItems.Find(x => x.Identifier == key);
                child.Sequence = i + 1;
                children.Add(child);
            }
        }

        public void When(EnrollmentStarted e)
        {
            if (ContainsLearner(e.Learner))
                return;

            Enrollments.Add(new Enrollment { Id = e.Enrollment, Learner = e.Learner, Period = e.Period, Comment = e.Comment });
        }

        public void When(GradebookUserNoted e)
        {
            var user = Enrollments.Find(x => x.Learner == e.User);
            if (user == null)
                return;

            user.Comment = e.Note;
        }

        public void When(GradebookUserPeriodChanged e)
        {
            var user = Enrollments.Find(x => x.Learner == e.User);
            if (user == null)
                return;

            user.Period = e.Period;
        }

        public void When(GradebookUserDeleted e)
        {
            var student = Enrollments.Find(x => x.Learner == e.User);
            if (student == null)
                return;

            if (ValidationScores != null)
            {
                var validationScores = ValidationScores.Where(x => x.User == e.User).ToList();
                foreach (var validationScore in validationScores)
                    ValidationScores.Remove(validationScore);
            }

            Enrollments.Remove(student);
        }

        public void When(GradebookValidationAdded e)
        {
            if (ValidationScores == null)
                ValidationScores = new List<GradebookValidationScore>();

            var validationScore = ValidationScores.SingleOrDefault(x => x.User == e.User && x.Competency == e.Competency);
            if (validationScore == null)
            {
                validationScore = new GradebookValidationScore
                {
                    User = e.User,
                    Competency = e.Competency
                };
                ValidationScores.Add(validationScore);
            }

            validationScore.Points = e.Points;
        }

        public void When(GradebookValidationChanged e)
        {
            if (ValidationScores == null)
                ValidationScores = new List<GradebookValidationScore>();

            var standardScore = ValidationScores.SingleOrDefault(x => x.User == e.User && x.Competency == e.Competency);
            if (standardScore == null)
            {
                standardScore = new GradebookValidationScore
                {
                    User = e.User,
                    Competency = e.Competency
                };
                ValidationScores.Add(standardScore);
            }

            standardScore.Points = e.Points;
        }

        public void When(EnrollmentRestarted e)
        {

        }

        #endregion
    }
}
