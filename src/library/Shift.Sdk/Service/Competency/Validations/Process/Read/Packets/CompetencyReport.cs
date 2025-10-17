using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;

namespace InSite.Application.Standards.Read
{
    public class CompetencyReport
    {
        public interface IQuestion
        {
            Guid QuestionIdentifier { get; }
            int QuestionSequence { get; }
            string QuestionText { get; }
            decimal? QuestionPoints { get; }
            decimal? AnswerPoints { get; }

            Guid? CompetencyAreaIdentifier { get; }
            string CompetencyAreaLabel { get; }
            string CompetencyAreaCode { get; }
            string CompetencyAreaTitle { get; }

            Guid? CompetencyItemIdentifier { get; }
            string CompetencyItemLabel { get; }
            string CompetencyItemCode { get; }
            string CompetencyItemTitle { get; }
        }

        private class QAttemptQuestionMapper : IQuestion
        {
            public Guid QuestionIdentifier => _entity.QuestionIdentifier;
            public int QuestionSequence => _entity.QuestionSequence;
            public string QuestionText => _entity.QuestionText;
            public decimal? QuestionPoints => _entity.QuestionPoints;
            public decimal? AnswerPoints => _entity.AnswerPoints;

            public Guid? CompetencyAreaIdentifier => _entity.CompetencyAreaIdentifier;
            public string CompetencyAreaLabel => _entity.CompetencyAreaLabel;
            public string CompetencyAreaCode => _entity.CompetencyAreaCode;
            public string CompetencyAreaTitle => _entity.CompetencyAreaTitle;

            public Guid? CompetencyItemIdentifier => _entity.CompetencyItemIdentifier;
            public string CompetencyItemLabel => _entity.CompetencyItemLabel;
            public string CompetencyItemCode => _entity.CompetencyItemCode;
            public string CompetencyItemTitle => _entity.CompetencyItemTitle;

            private QAttemptQuestion _entity;

            public QAttemptQuestionMapper(QAttemptQuestion entity)
            {
                _entity = entity;
            }
        }

        public List<CompetencyArea> Folders { get; set; }

        public CompetencyReport(IEnumerable<IQuestion> questions)
        {
            Folders = GetFolders(questions);
        }

        public CompetencyReport(IEnumerable<QAttemptQuestion> questions)
        {
            Folders = GetFolders(questions.Select(x => new QAttemptQuestionMapper(x)));
        }

        private static List<CompetencyArea> GetFolders(IEnumerable<IQuestion> questions)
        {
            if (questions == null)
                return new List<CompetencyArea>();

            return questions
                .Where(x => x.CompetencyAreaIdentifier.HasValue)
                .GroupBy(x => new
                {
                    FolderIdentifier = x.CompetencyAreaIdentifier.Value,
                    FolderLabel = x.CompetencyAreaLabel,
                    FolderCode = x.CompetencyAreaCode,
                    FolderTitle = x.CompetencyAreaTitle
                })
                .Select(folder => new CompetencyArea
                {
                    Identifier = folder.Key.FolderIdentifier,
                    Label = folder.Key.FolderLabel,
                    Code = folder.Key.FolderCode,
                    Title = folder.Key.FolderTitle,
                    Items = folder
                        .Where(y => y.CompetencyItemIdentifier.HasValue)
                        .GroupBy(y => new
                        {
                            ItemIdentifier = y.CompetencyItemIdentifier.Value,
                            ItemLabel = y.CompetencyItemLabel,
                            ItemCode = y.CompetencyItemCode,
                            ItemTitle = y.CompetencyItemTitle
                        })
                        .Select(item => new CompetencyItem
                        {
                            Identifier = item.Key.ItemIdentifier,
                            Label = item.Key.ItemLabel,
                            Code = item.Key.ItemCode,
                            Title = item.Key.ItemTitle,
                            Questions = item.Select(question => new CompetencyQuestion
                            {
                                QuestionIdentifier = question.QuestionIdentifier,
                                QuestionNumber = question.QuestionSequence,
                                QuestionText = question.QuestionText,
                                QuestionPoints = question.QuestionPoints ?? 0,
                                AnswerPoints = question.AnswerPoints ?? 0
                            })
                            .OrderBy(x => x.QuestionNumber)
                            .ToList()
                        })
                        .OrderBy(x => x.Code)
                        .ToList()
                })
                .OrderBy(x => x.Code)
                .ToList();
        }
    }
}
