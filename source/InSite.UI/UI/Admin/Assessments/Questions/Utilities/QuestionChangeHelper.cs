using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Domain.Banks;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public static class QuestionChangeHelper
    {
        #region Constants

        public static readonly string[] QuestionChangeTypeNameList = ChangeTypeCollection.Current
            .Select(x => x.GetType().GetGenericArguments()[0].Name)
            .ToArray();

        #endregion

        #region Classes

        public abstract class ChangeTypeInfo
        {
            public abstract bool IsMatch(IChange e, ICollection<Guid> ids);

            public abstract IEnumerable<Guid> GetIdentifiers(IChange e);
        }

        private sealed class ChangeTypeInfo<T> : ChangeTypeInfo where T : IChange
        {
            private readonly Func<T, IEnumerable<Guid>> _getId;

            public ChangeTypeInfo(Func<T, IEnumerable<Guid>> getId)
            {
                _getId = getId
                    ?? throw new ArgumentNullException(nameof(getId));
            }

            public bool IsMatch(T e, ICollection<Guid> ids)
            {
                foreach (var id in _getId(e))
                {
                    if (ids.Contains(id))
                        return true;
                }

                return false;
            }

            public override bool IsMatch(IChange e, ICollection<Guid> ids) =>
                IsMatch((T)e, ids);

            public IEnumerable<Guid> GetIdentifiers(T e) => _getId(e);

            public override IEnumerable<Guid> GetIdentifiers(IChange e) => GetIdentifiers((T)e);
        }

        public sealed class ChangeTypeCollection : IEnumerable<ChangeTypeInfo>
        {
            public static ChangeTypeCollection Current { get; } = new ChangeTypeCollection
            {
                new ChangeTypeInfo<QuestionAdded>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionClassificationChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionContentChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionDuplicated2>(x => new[] { x.SourceQuestion, x.DestinationQuestion }),
                new ChangeTypeInfo<QuestionFlagChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionLayoutChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionMatchesChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionMoved>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionMovedIn>(x => new[] { x.Question.Identifier }),
                new ChangeTypeInfo<QuestionMovedOut>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionPublicationStatusChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionRandomizationChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionDeleted>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionScoringChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionSetChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionsReordered>(x => new Guid[0]),
                new ChangeTypeInfo<QuestionStandardChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<QuestionUpgraded>(x => new[] { x.CurrentQuestion, x.UpgradedQuestion }),
                new ChangeTypeInfo<QuestionConditionChanged>(x => new[] { x.Question }),

                new ChangeTypeInfo<AttachmentAddedToQuestion>(x => new[] { x.Question }),
                new ChangeTypeInfo<AttachmentDeletedFromQuestion>(x => new[] { x.Question }),
                new ChangeTypeInfo<BankCommentPosted>(x => new[] { x.Subject }),
                new ChangeTypeInfo<FieldAdded>(x => new[] { x.Question }),
                new ChangeTypeInfo<FieldsDeleted>(x => new[] { x.Question }),
                new ChangeTypeInfo<FieldDeleted>(x => new[] { x.Question }),
                new ChangeTypeInfo<OptionAdded>(x => new[] { x.Question }),
                new ChangeTypeInfo<OptionChanged>(x => new[] { x.Question }),
                new ChangeTypeInfo<OptionsReordered>(x => new[] { x.Question }),
            };

            private readonly Dictionary<Type, ChangeTypeInfo> _dict;

            public ChangeTypeCollection()
            {
                _dict = new Dictionary<Type, ChangeTypeInfo>();
            }

            private void Add<T>(ChangeTypeInfo<T> info) where T : IChange =>
                _dict.Add(typeof(T), info);

            public bool IsMatch(IChange e, ICollection<Guid> questions) =>
                _dict.TryGetValue(e.GetType(), out var info) && info.IsMatch(e, questions);

            public IEnumerable<Guid> GetIdentifiers(IChange e) =>
                _dict.TryGetValue(e.GetType(), out var info) ? info.GetIdentifiers(e) : Enumerable.Empty<Guid>();

            public IEnumerator<ChangeTypeInfo> GetEnumerator() =>
                _dict.Values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator() =>
                GetEnumerator();
        }

        #endregion

        #region Methods

        public static IEnumerable<IChange> GetChanges(Guid bankId, IEnumerable<string> changeTypes = null)
        {
            var filter = changeTypes == null
                ? QuestionChangeTypeNameList
                : QuestionChangeTypeNameList.Where(x => changeTypes.Contains(x)).ToArray();

            return filter.Length == 0
                ? Enumerable.Empty<IChange>()
                : ServiceLocator.ChangeStore.GetChanges("Bank", bankId, filter);
        }

        public static IEnumerable<IChange> GetChanges(Guid bankId, ICollection<Guid> questions) =>
            GetChanges(bankId).Where(c => ChangeTypeCollection.Current.IsMatch(c, questions));

        #endregion
    }
}