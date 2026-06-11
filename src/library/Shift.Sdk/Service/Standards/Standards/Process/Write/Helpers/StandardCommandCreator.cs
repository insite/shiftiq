using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Shift.Common.Timeline.Commands;

using InSite.Application.Standards.Read;
using InSite.Domain.Standards;

using Shift.Common;

namespace InSite.Application.Standards.Write
{
    public class StandardCommandCreator
    {
        private readonly QStandard _old;
        private readonly QStandard _new;

        private List<Command> _result = new List<Command>();

        private StandardCommandCreator(QStandard old, QStandard @new)
        {
            _old = old;
            _new = @new;
        }

        public static List<Command> Create(QStandard old, QStandard @new)
        {
            return new StandardCommandCreator(old, @new).Create();
        }

        private List<Command> Create()
        {
            AddCreateCommand();
            AddRemoveCommand();

            if (_new != null)
            {
                var fieldsCommand = new ModifyStandardFields(_new.StandardIdentifier);

                FieldMapping.Guid.AddValues(this, fieldsCommand);
                FieldMapping.DateOffset.AddValues(this, fieldsCommand);
                FieldMapping.Text.AddValues(this, fieldsCommand);
                FieldMapping.Bool.AddValues(this, fieldsCommand);
                FieldMapping.Int.AddValues(this, fieldsCommand);
                FieldMapping.Decimal.AddValues(this, fieldsCommand);

                if (fieldsCommand.Fields.IsNotEmpty())
                    _result.Add(fieldsCommand);

                var contentCommand = new ModifyStandardContent(_new.StandardIdentifier);
                contentCommand.Content.Title.Text.Default = _new.ContentTitle;
                contentCommand.Content.Description.Text.Default = _new.ContentDescription;
                contentCommand.Content.Summary.Text.Default = _new.ContentSummary;
                _result.Add(contentCommand);
            }

            return _result;
        }

        private void AddCreateCommand()
        {
            if (_old != null)
                return;

            var content = new ContentContainer();

            if (_new.ContentTitle.IsNotEmpty())
                content.Title.Text.Default = _new.ContentTitle;

            if (_new.ContentDescription.IsNotEmpty())
                content.Description.Text.Default = _new.ContentDescription;

            if (_new.ContentSummary.IsNotEmpty())
                content.Summary.Text.Default = _new.ContentSummary;

            _result.Add(new CreateStandard(_new.StandardIdentifier, _new.StandardType, _new.AssetNumber, _new.Sequence, content));
        }

        private void AddRemoveCommand()
        {
            if (_new == null)
                _result.Add(new RemoveStandard(_old.StandardIdentifier));
        }

        private static class FieldMapping
        {
            #region Entity

            public sealed class Entity<T>
            {
                public readonly IReadOnlyDictionary<StandardField, Func<QStandard, T>> Mapping;
                public readonly Func<T, T, bool> IsSame;

                public Entity(Func<T, T, bool> comparer, IDictionary<StandardField, Func<QStandard, T>> mapping)
                {
                    IsSame = comparer;
                    Mapping = new ReadOnlyDictionary<StandardField, Func<QStandard, T>>(mapping);
                }

                public void AddValues(StandardCommandCreator creator, ModifyStandardFields command)
                {
                    if (creator._old == null)
                    {
                        foreach (var kv in Mapping)
                            TryAddValue(command, default, kv.Value(creator._new), kv.Key);
                    }
                    else
                    {
                        foreach (var kv in Mapping)
                            TryAddValue(command, kv.Value(creator._old), kv.Value(creator._new), kv.Key);
                    }
                }

                private void TryAddValue(ModifyStandardFields command, T oldValue, T newValue, StandardField field)
                {
                    if (!IsSame(oldValue, newValue))
                        command.Fields[field] = newValue;
                }
            }

            #endregion

            #region Fields

            public static readonly Entity<Guid?> Guid;
            public static readonly Entity<DateTimeOffset?> DateOffset;
            public static readonly Entity<string> Text;
            public static readonly Entity<int?> Int;
            public static readonly Entity<decimal?> Decimal;
            public static readonly Entity<bool?> Bool;

            #endregion

            #region Initialization

            static FieldMapping()
            {
                Guid = new Entity<Guid?>(
                    (old, @new) => old == @new,
                    new Dictionary<StandardField, Func<QStandard, Guid?>>
                    {
                        { StandardField.BankIdentifier, e => e.BankIdentifier },
                        { StandardField.BankSetIdentifier, e => e.BankSetIdentifier },
                        { StandardField.CategoryItemIdentifier, e => e.CategoryItemIdentifier },
                        { StandardField.DepartmentGroupIdentifier, e => e.DepartmentGroupIdentifier },
                        { StandardField.IndustryItemIdentifier, e => e.IndustryItemIdentifier },
                        { StandardField.ParentStandardIdentifier, e => e.ParentStandardIdentifier },
                        { StandardField.StandardStatusLastUpdateUser, e => e.StandardStatusLastUpdateUser }
                    });

                DateOffset = new Entity<DateTimeOffset?>(
                    (old, @new) => old == @new,
                    new Dictionary<StandardField, Func<QStandard, DateTimeOffset?>>
                    {
                        { StandardField.AuthorDate, e => e.AuthorDate },
                        { StandardField.DatePosted, e => e.DatePosted },
                        { StandardField.StandardStatusLastUpdateTime, e => e.StandardStatusLastUpdateTime },
                        { StandardField.UtcPublished, e => e.UtcPublished },
                    });

                Text = new Entity<string>(
                    (old, @new) => StringHelper.EqualsCaseSensitive(old, @new, true),
                    new Dictionary<StandardField, Func<QStandard, string>>
                    {
                        { StandardField.AuthorName, e => e.AuthorName },
                        { StandardField.CanvasIdentifier, e => e.CanvasIdentifier },
                        { StandardField.Code, e => e.Code },
                        { StandardField.CompetencyScoreCalculationMethod, e => e.CompetencyScoreCalculationMethod },
                        { StandardField.CompetencyScoreSummarizationMethod, e => e.CompetencyScoreSummarizationMethod },
                        { StandardField.ContentName, e => e.ContentName },
                        { StandardField.ContentSettings, e => e.ContentSettings },
                        { StandardField.CreditIdentifier, e => e.CreditIdentifier },
                        { StandardField.DocumentType, e => e.DocumentType },
                        { StandardField.Icon, e => e.Icon },
                        { StandardField.Language, e => e.Language },
                        { StandardField.LevelCode, e => e.LevelCode },
                        { StandardField.LevelType, e => e.LevelType },
                        { StandardField.MajorVersion, e => e.MajorVersion },
                        { StandardField.MinorVersion, e => e.MinorVersion },
                        { StandardField.SourceDescriptor, e => e.SourceDescriptor },
                        { StandardField.StandardAlias, e => e.StandardAlias },
                        { StandardField.StandardLabel, e => e.StandardLabel },
                        { StandardField.StandardPrivacyScope, e => e.StandardPrivacyScope },
                        { StandardField.StandardStatus, e => e.StandardStatus },
                        { StandardField.StandardTier, e => e.StandardTier },
                        { StandardField.StandardType, e => e.StandardType },
                        { StandardField.Tags, e => e.Tags },
                        { StandardField.StandardHook, e => e.StandardHook },
                    });

                Int = new Entity<int?>(
                    (old, @new) => old == @new,
                    new Dictionary<StandardField, Func<QStandard, int?>>
                    {
                        { StandardField.CalculationArgument, e => e.CalculationArgument },
                        { StandardField.Sequence, e => e.Sequence },
                    });

                Decimal = new Entity<decimal?>(
                    (old, @new) => old == @new,
                    new Dictionary<StandardField, Func<QStandard, decimal?>>
                    {
                        { StandardField.CertificationHoursPercentCore, e => e.CertificationHoursPercentCore },
                        { StandardField.CertificationHoursPercentNonCore, e => e.CertificationHoursPercentNonCore },
                        { StandardField.CreditHours, e => e.CreditHours },
                        { StandardField.MasteryPoints, e => e.MasteryPoints },
                        { StandardField.PassingScore, e => e.PassingScore },
                        { StandardField.PointsPossible, e => e.PointsPossible },
                    });

                Bool = new Entity<bool?>(
                    (old, @new) => old == @new,
                    new Dictionary<StandardField, Func<QStandard, bool?>>
                    {
                        { StandardField.IsCertificateEnabled, e => e.IsCertificateEnabled },
                        { StandardField.IsFeedbackEnabled, e => e.IsFeedbackEnabled },
                        { StandardField.IsHidden, e => e.IsHidden },
                        { StandardField.IsLocked, e => e.IsLocked },
                        { StandardField.IsPractical, e => e.IsPractical },
                        { StandardField.IsPublished, e => e.IsPublished },
                        { StandardField.IsTemplate, e => e.IsTemplate },
                        { StandardField.IsTheory, e => e.IsTheory },
                    });
            }

            #endregion
        }
    }
}
