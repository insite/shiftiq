using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;

using Shift.Common.Timeline.Commands;

using InSite.Application.Journals.Write;
using InSite.Application.Records.Read;
using InSite.Common.Web.UI;

using Shift.Common;
using Shift.Constant;

using AspLiteral = System.Web.UI.WebControls.Literal;
using CommonMarkdown = Shift.Common.Markdown;

namespace InSite.UI.Portal.Records.Logbooks.Controls
{
    internal sealed class ExperienceFieldDescription
    {
        #region Fields

        private static class FieldInfo
        {
            public static class Text
            {
                public const string Name = "Text";

                public static void Set(IExperienceField field, string value, int maxLength)
                {
                    var input = (IExperienceTextInputField)field;
                    input.MaxLength = maxLength;
                    input.Value = value;
                }

                public static string Get(IExperienceField field)
                {
                    var input = (IExperienceTextInputField)field;
                    return input.Value;
                }

                public static string GetHtml(string value) =>
                    HttpUtility.HtmlEncode(value);
            }

            public static class Multiline
            {
                public const string Name = "Multiline";

                public static void Set(IExperienceField field, string value)
                {
                    var input = (IExperienceTextField)field;
                    input.Value = value;
                }

                public static string Get(IExperienceField field)
                {
                    var input = (IExperienceTextField)field;
                    return input.Value;
                }

                public static string GetHtml(string value) =>
                    value.IsNotEmpty()
                        ? "<div style='white-space:pre-wrap;'>" + HttpUtility.HtmlEncode(value) + "</div>"
                        : string.Empty;
            }

            public static class Number
            {
                public const string Name = "Number";

                public static void Set(IExperienceField field, decimal? value)
                {
                    var input = (IExperienceDecimalField)field;
                    input.Value = value;
                }

                public static decimal? Get(IExperienceField field)
                {
                    var input = (IExperienceDecimalField)field;
                    return input.Value;
                }

                public static string GetHtml(decimal? value) =>
                    value.HasValue ? $"{value.Value:n2}" : string.Empty;
            }

            public static class DropDown
            {
                public const string Name = "DropDown";

                public static void Set(IExperienceField field, string value)
                {
                    var input = (IExperienceDropDownField)field;
                    input.Value = value;
                }

                public static string Get(IExperienceField field)
                {
                    var input = (IExperienceDropDownField)field;
                    return input.Value;
                }

                public static string GetHtml(string value) =>
                    value.IsNotEmpty()
                        ? "<div style='white-space:pre-wrap;'>" + HttpUtility.HtmlEncode(value) + "</div>"
                        : string.Empty;
            }

            public static class Date
            {
                public const string Name = "Date";

                public static void Set(IExperienceField field, DateTime? value)
                {
                    var input = (IExperienceDateField)field;
                    input.Value = value;
                }

                public static DateTime? Get(IExperienceField field)
                {
                    var input = (IExperienceDateField)field;
                    return input.Value;
                }

                public static string GetHtml(DateTime? value) =>
                    value.HasValue ? $"{value.Value:MMM d, yyyy}" : string.Empty;
            }

            public static class TwoDates
            {
                public const string Name = "TwoDates";


                public static void Set(IExperienceField field, DateTime? value1, DateTime? value2)
                {
                    var input = (IExperienceTwoDatesField)field;
                    input.Value1 = value1;
                    input.Value2 = value2;
                }

                public static void Get(IExperienceField field, out DateTime? value1, out DateTime? value2)
                {
                    var input = (IExperienceTwoDatesField)field;
                    value1 = input.Value1;
                    value2 = input.Value2;
                }

                public static string GetHtml(DateTime? value1, DateTime? value2) =>
                    !value1.HasValue && !value2.HasValue
                        ? string.Empty
                        : !value1.HasValue
                            ? $"{value2:MMM d, yyyy}"
                            : !value2.HasValue
                                ? $"{value1:MMM d, yyyy}"
                                : $"{value1:MMM d, yyyy} - {value2:MMM d, yyyy}";
            }

            public static class Markdown
            {
                public const string Name = "Markdown";

                public static void Set(IExperienceField field, string value, QExperience entity, string subPath)
                {
                    var input = (IExperienceTextEditorField)field;
                    input.Value = value;
                    input.UploadPath = GetUploadPath(entity, subPath);
                }

                public static string Get(IExperienceField field)
                {
                    var input = (IExperienceTextEditorField)field;
                    return input.Value;
                }

                public static string GetHtml(string value) =>
                    CommonMarkdown.ToHtml(value);
            }

            public static class Media
            {
                public const string Name = "Media";

                public static void Set(IExperienceField field, QExperience entity)
                {
                    var input = (IExperienceMediaField)field;
                    input.SetData(entity);
                }

                public static IExperienceMediaFieldData Get(IExperienceField field, QExperience experience)
                {
                    var input = (IExperienceMediaField)field;
                    return input.GetData(experience);
                }

                public static string GetHtml(string name, Guid resourceId, Guid sessionId, bool audioOnly)
                {
                    return $"<strong>NOT IMPLEMENTED</strong>";
                }
            }
        }

        private static class ChangeActions
        {
            public static readonly ChangeAction Employer =
                x => new ChangeExperienceEmployer(x.JournalIdentifier, x.ExperienceIdentifier, x.Employer);

            public static readonly ChangeAction Supervisor =
                x => new ChangeExperienceSupervisor(x.JournalIdentifier, x.ExperienceIdentifier, x.Supervisor);

            public static readonly ChangeAction StartAndEndDate =
                x => new ChangeExperienceTime(x.JournalIdentifier, x.ExperienceIdentifier, x.ExperienceStarted, x.ExperienceStopped);

            public static readonly ChangeAction Completed =
                x => new ChangeExperienceCompleted(x.JournalIdentifier, x.ExperienceIdentifier, x.ExperienceCompleted);

            public static readonly ChangeAction Hours =
                x => new ChangeExperienceHours(x.JournalIdentifier, x.ExperienceIdentifier, x.ExperienceHours);

            public static readonly ChangeAction Training =
                x => new ChangeExperienceTraining(
                    x.JournalIdentifier,
                    x.ExperienceIdentifier,
                    x.TrainingLevel,
                    x.TrainingLocation,
                    x.TrainingProvider,
                    x.TrainingCourseTitle,
                    x.TrainingComment,
                    x.TrainingType);

            public static readonly ChangeAction Experience =
                x => new ChangeExperienceEvidence(x.JournalIdentifier, x.ExperienceIdentifier, x.ExperienceEvidence);

            public static readonly ChangeAction Media =
                x => new ChangeExperienceMediaEvidence(
                    x.JournalIdentifier,
                    x.ExperienceIdentifier,
                    x.MediaEvidenceName,
                    x.MediaEvidenceType,
                    x.MediaEvidenceFileIdentifier);
        }

        #endregion

        #region Instance

        public static readonly IReadOnlyDictionary<JournalSetupFieldType, ExperienceFieldDescription> Items = new ReadOnlyDictionary<JournalSetupFieldType, ExperienceFieldDescription>(new Dictionary<JournalSetupFieldType, ExperienceFieldDescription>
        {
            {
                JournalSetupFieldType.Employer,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.Employer, 100);
                    },
                    Save = (field, entity) =>
                    {
                        entity.Employer = FieldInfo.Text.Get(field);
                        return ChangeActions.Employer;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.Employer).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.Employer).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.Supervisor,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.Supervisor, 100);
                    },
                    Save = (field, entity) =>
                    {
                        entity.Supervisor = FieldInfo.Text.Get(field);
                        return ChangeActions.Supervisor;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.Supervisor).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.Supervisor).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.StartAndEndDates,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.TwoDates.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.TwoDates.Set(field, entity.ExperienceStarted, entity.ExperienceStopped);
                    },
                    Save = (field, entity) =>
                    {
                        FieldInfo.TwoDates.Get(field, out var value1, out var value2);
                        entity.ExperienceStarted = value1;
                        entity.ExperienceStopped = value2;
                        return ChangeActions.StartAndEndDate;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.TwoDates.GetHtml(entity.ExperienceStarted, entity.ExperienceStopped).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.TwoDates.GetHtml(entity.ExperienceStarted, entity.ExperienceStopped).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.Completed,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Date.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Date.Set(field, entity.ExperienceCompleted);
                    },
                    Save = (field, entity) =>
                    {
                        entity.ExperienceCompleted = FieldInfo.Date.Get(field);
                        return ChangeActions.Completed;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Date.GetHtml(entity.ExperienceCompleted).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Date.GetHtml(entity.ExperienceCompleted).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.Hours,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Number.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Number.Set(field, entity.ExperienceHours);
                    },
                    Save = (field, entity) =>
                    {
                        entity.ExperienceHours = FieldInfo.Number.Get(field);
                        return ChangeActions.Hours;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Number.GetHtml(entity.ExperienceHours).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Number.GetHtml(entity.ExperienceHours).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingEvidence,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Markdown.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Markdown.Set(field, entity.ExperienceEvidence, entity, "tevidence");
                    },
                    Save = (field, entity) =>
                    {
                        entity.ExperienceEvidence = FieldInfo.Markdown.Get(field);
                        return ChangeActions.Experience;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Markdown.GetHtml(entity.ExperienceEvidence).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Markdown.GetHtml(entity.ExperienceEvidence).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingLevel,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.TrainingLevel, 200);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingLevel = FieldInfo.Text.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.TrainingLevel).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.TrainingLevel).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingLocation,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.TrainingLocation, 200);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingLocation = FieldInfo.Text.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.TrainingLocation).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.TrainingLocation).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingProvider,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.TrainingProvider, 100);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingProvider = FieldInfo.Text.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.TrainingProvider).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.TrainingProvider).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingCourseTitle,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Text.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Text.Set(field, entity.TrainingCourseTitle, 200);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingCourseTitle = FieldInfo.Text.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Text.GetHtml(entity.TrainingCourseTitle).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Text.GetHtml(entity.TrainingCourseTitle).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingComment,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Multiline.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Multiline.Set(field, entity.TrainingComment);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingComment = FieldInfo.Multiline.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.Multiline.GetHtml(entity.TrainingComment).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.Multiline.GetHtml(entity.TrainingComment).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.TrainingType,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.DropDown.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.DropDown.Set(field, entity.TrainingType);
                    },
                    Save = (field, entity) =>
                    {
                        entity.TrainingType = FieldInfo.DropDown.Get(field);
                        return ChangeActions.Training;
                    },
                    AddValue = (entity, container) =>
                    {
                        var literal = container.LoadControl<AspLiteral>();
                        literal.Text = FieldInfo.DropDown.GetHtml(entity.TrainingType).IfNullOrEmpty("<i>None</i>");
                    },
                    GetHtml = (entity) =>
                    {
                        return FieldInfo.DropDown.GetHtml(entity.TrainingType).IfNullOrEmpty("<i>None</i>");
                    }
                }
            },
            {
                JournalSetupFieldType.MediaEvidence,
                new ExperienceFieldDescription
                {
                    ControlName = FieldInfo.Media.Name,
                    Load = (field, entity) =>
                    {
                        FieldInfo.Media.Set(field, entity);
                    },
                    Save = (field, entity) =>
                    {
                        var values = FieldInfo.Media.Get(field, entity);
                        entity.MediaEvidenceName = values.Name;
                        entity.MediaEvidenceType = values.Type;
                        entity.MediaEvidenceFileIdentifier = values.FileIdentifier;
                        return ChangeActions.Media;
                    },
                    AddValue = (entity, container) =>
                    {
                        var control = container.LoadControl<ExperienceFields.Media.OutputControl>();
                        var file = entity.MediaEvidenceFileIdentifier.HasValue
                            ? ServiceLocator.StorageService.GetFile(entity.MediaEvidenceFileIdentifier.Value)
                            : null;

                        if (file != null)
                        {
                            control.Type = entity.MediaEvidenceType;
                            control.Title = entity.MediaEvidenceName;
                            control.MediaURL = ServiceLocator.StorageService.GetFileUrl(file, false);
                        }
                    },
                    GetHtml = (entity) =>
                    {
                        if (!entity.MediaEvidenceFileIdentifier.HasValue)
                            return "<i>None</i>";
                        else if (entity.MediaEvidenceType == "audio")
                            return "<i>Audio Data</i>";
                        else if (entity.MediaEvidenceType == "video")
                            return "<i>Video Data</i>";
                        else
                            return "<i>Unknown Data</i>";
                    }
                }
            },
        });

        #endregion

        #region Delegates

        public delegate Command ChangeAction(QExperience experience);

        #endregion

        #region Fields

        public string ControlName { get; private set; }
        public Action<IExperienceField, QExperience> Load { get; private set; }
        public Func<IExperienceField, QExperience, ChangeAction> Save { get; private set; }
        public Action<QExperience, DynamicControl> AddValue { get; private set; }
        public Func<QExperience, string> GetHtml { get; private set; }

        #endregion

        #region Methods (helpers)

        private static readonly string UniqueIdStorage = typeof(ExperienceFieldDescription).FullName + ".UniqueID";

        private static string GetUploadPath(QExperience entity, string type)
        {
            if (entity.ExperienceIdentifier == Guid.Empty)
            {
                var uniqueIds = (Dictionary<object, Guid>)(HttpContext.Current.Items[UniqueIdStorage]
                    ?? (HttpContext.Current.Items[UniqueIdStorage] = new Dictionary<object, Guid>()));

                var uid = uniqueIds.GetOrAdd(entity, () => Guid.NewGuid());

                return string.Format(OrganizationRelativePath.JournalExperienceCreatePathTemplate, entity.JournalIdentifier, uid, type);
            }
            else
            {
                return string.Format(OrganizationRelativePath.JournalExperienceChangePathTemplate, entity.ExperienceIdentifier, type);
            }
        }

        #endregion
    }
}