using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common.Timeline.Commands;

using InSite.Application.Gradebooks;
using InSite.Application.Progresses.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace InSite.Application.Records.Write
{
    public class GradebookCalculator
    {
        private Guid _gradebook;
        private readonly IRecordSearch _records;

        private ScoreBuffer _buffer = new ScoreBuffer();
        private GradebookState _data;
        private List<QProgress> _progresses;
        private List<Command> _commands;
        private HashSet<Guid> _newProgresses;

        private GradebookCalculator(Guid gradebook, IRecordSearch records)
        {
            _gradebook = gradebook;
            _records = records;
        }

        public static Command[] Calculate(Guid gradebook, Guid? learner, bool publish, IRecordSearch records)
        {
            return new GradebookCalculator(gradebook, records).Calculate(learner, publish);
        }

        private Command[] Calculate(Guid? learner, bool publish)
        {
            _data = _records.GetGradebookState(_gradebook);
            _progresses = _records.GetGradebookScores(new QProgressFilter { GradebookIdentifier = _gradebook });
            _commands = new List<Command>();
            _newProgresses = new HashSet<Guid>();

            var affectedItems = new HashSet<GradeItem>();

            Enrollment[] learners = learner.HasValue
                ? _data.Enrollments.Where(x => x.Learner == learner.Value).ToArray()
                : _data.Enrollments.ToArray();

            foreach (var item in _data.AllItems)
                Calculate(item, affectedItems, learners);

            _commands.Add(new CalculateGradebook(_gradebook, learners.Select(x => x.Learner).ToArray()));

            if (publish)
            {
                foreach (var progress in _progresses)
                {
                    if (progress.ProgressStatus == ProgressStarted.Status)
                        _commands.Add(new IncompleteProgress(progress.ProgressIdentifier));

                    _commands.Add(new PublishProgress(progress.ProgressIdentifier));
                }

                foreach (var progress in _newProgresses)
                    _commands.Add(new PublishProgress(progress));
            }

            return _commands.ToArray();
        }

        private void Calculate(GradeItem item, HashSet<GradeItem> affectedItems, Enrollment[] learners)
        {
            if (item.Type == GradeItemType.Score)
            {
                CheckIgnored(item, learners);
                return;
            }

            if (affectedItems.Contains(item))
                return;

            affectedItems.Add(item);

            if (item.Type == GradeItemType.Category)
            {
                foreach (var child in item.Children)
                    Calculate(child, affectedItems, learners);

                switch (item.Weighting)
                {
                    case GradeItemWeighting.Equally:
                        CalculateEqually(item, false, learners);
                        break;
                    case GradeItemWeighting.EquallyWithNulls:
                        CalculateEqually(item, true, learners);
                        break;
                    case GradeItemWeighting.ByPoints:
                        CalculateByPoint(item, learners);
                        break;
                    case GradeItemWeighting.Sum:
                        CalculateSum(item, learners);
                        break;
                }
            }
            else if (item.Type == GradeItemType.Calculation)
            {
                foreach (var part in item.Parts)
                {
                    var partItem = _data.FindItem(part.Item);
                    Calculate(partItem, affectedItems, learners);
                }

                CalculateByParts(item, learners);
            }
        }

        private void CheckIgnored(GradeItem item, Enrollment[] learners)
        {
            var progresses = _progresses
                .Where(x =>
                    learners.Any(y => y.Learner == x.UserIdentifier)
                    && x.GradeItemIdentifier == item.Identifier
                    && x.ProgressIsIgnored
                    && (x.ProgressGraded.HasValue || !x.NoScore)
                )
                .ToList();

            foreach (var progress in progresses)
            {
                if (progress.ProgressNumber.HasValue)
                    _commands.Add(new ChangeProgressNumber(progress.ProgressIdentifier, null, null));

                if (progress.ProgressPercent.HasValue)
                    _commands.Add(new ChangeProgressPercent(progress.ProgressIdentifier, null, null));

                if (progress.ProgressPoints.HasValue)
                    _commands.Add(new ChangeProgressPoints(progress.ProgressIdentifier, null, null, null));

                if (!string.IsNullOrEmpty(progress.ProgressText))
                    _commands.Add(new ChangeProgressText(progress.ProgressIdentifier, null, null));
            }
        }

        private void CalculateEqually(GradeItem item, bool includeNullValues, Enrollment[] learners)
        {
            var children = item.Children.Where(x =>
                   x.Format == GradeItemFormat.Percent
                || x.Format == GradeItemFormat.Point
                || x.Type != GradeItemType.Score
            ).ToList();

            if (children.Count == 0)
                return;

            foreach (var user in learners)
            {
                var totalScore = 0m;
                var totalCount = 0;

                foreach (var child in children)
                {
                    var (childScoreValue, isIgnored) = GetScoreValue(user.Learner, child);
                    if (!isIgnored && (childScoreValue.HasValue || includeNullValues))
                    {
                        totalScore += childScoreValue ?? 0m;
                        totalCount++;
                    }
                }

                var percent = totalCount > 0
                    ? RoundPercent(totalScore / totalCount)
                    : (decimal?)null;

                ChangeProgressPercent(user.Learner, item.Identifier, percent);
            }
        }

        private (decimal? Value, bool isIgnored) GetScoreValue(Guid user, GradeItem item)
        {
            var score = _progresses.Find(x => x.UserIdentifier == user && x.GradeItemIdentifier == item.Identifier);
            if (score != null && score.ProgressIsIgnored)
                return (null, true);

            if (_buffer.Exists(user, item.Identifier))
                return (_buffer.Get(user, item.Identifier), false);

            if (score == null)
                return (null, false);

            if (item.Type != GradeItemType.Score || item.Format != GradeItemFormat.Point)
                return (score.ProgressPercent, false);

            if (score.ProgressPoints == null)
                return (null, false);

            if (item.MaxPoints == null || item.MaxPoints == 0)
                throw new CalculateScoreException("Item does not have Max Points therefore percentage cannot be calculated");

            return (score.ProgressPoints.Value / item.MaxPoints.Value, false);
        }

        private void CalculateByPoint(GradeItem item, Enrollment[] learners)
        {
            var children = item.Children.Where(x =>
                x.Format == GradeItemFormat.Percent
                || x.Format == GradeItemFormat.Point
                || x.Type != GradeItemType.Score
            ).ToList();

            if (children.Count == 0)
                return;

            var formats = children.Select(x => x.Format).Distinct().ToList();
            if (formats.Count > 1)
                throw new CalculateScoreException("Calculation by points cannot be done for mixed format");

            var format = formats[0];

            if (format == GradeItemFormat.Point)
                CalculateByPointForPoint(item, children, learners);
            else
                CalculateByPointForPercent(item, children, learners);
        }

        private void CalculateByPointForPoint(GradeItem parent, List<GradeItem> children, Enrollment[] learners)
        {
            foreach (var student in learners)
            {
                var totalMaxPoints = 0m;
                var totalPoints = 0m;

                foreach (var child in children)
                {
                    var childScore = _progresses.Find(x => x.UserIdentifier == student.Learner && x.GradeItemIdentifier == child.Identifier);

                    if (childScore?.ProgressPoints != null && !childScore.ProgressIsIgnored)
                    {
                        if (child.MaxPoints.HasValue)
                            totalMaxPoints += child.MaxPoints.Value;

                        totalPoints += childScore.ProgressPoints.Value;
                    }
                }

                var percent = totalMaxPoints > 0
                    ? RoundPercent(totalPoints / totalMaxPoints)
                    : (decimal?)null;

                ChangeProgressPoints(student.Learner, parent.Identifier, totalPoints, totalMaxPoints, percent);
            }
        }

        private void CalculateByPointForPercent(GradeItem parent, List<GradeItem> children, Enrollment[] learners)
        {
            foreach (var user in learners)
            {
                if (children.Any(x => _progresses.Any(y => y.UserIdentifier == user.Learner && y.GradeItemIdentifier == x.Identifier && y.ProgressMaxPoints.HasValue)))
                {
                    var totalMaxPoints = 0m;
                    var totalPoints = 0m;

                    foreach (var child in children)
                    {
                        var childScore = _progresses.Find(x => x.UserIdentifier == user.Learner && x.GradeItemIdentifier == child.Identifier);

                        if (childScore != null && !childScore.ProgressIsIgnored)
                        {
                            if (childScore.ProgressPercent.HasValue && (childScore.ProgressPoints == null || childScore.ProgressMaxPoints == null))
                                throw new CalculateScoreException("Points and percents cannot be mixed");

                            totalMaxPoints += childScore.ProgressMaxPoints ?? 0;
                            totalPoints += childScore.ProgressPoints ?? 0;
                        }
                    }

                    var percent = totalMaxPoints > 0
                        ? RoundPercent(totalPoints / totalMaxPoints)
                        : (decimal?)null;

                    ChangeProgressPoints(user.Learner, parent.Identifier, totalPoints, totalMaxPoints, percent);
                }
                else
                {
                    var totalPercent = 0m;
                    var count = 0;

                    foreach (var child in children)
                    {
                        var (childScoreValue, isIgnored) = GetScoreValue(user.Learner, child);

                        if (isIgnored)
                            continue;

                        count++;

                        if (childScoreValue.HasValue)
                            totalPercent += childScoreValue ?? 0;
                    }

                    var percent = RoundPercent(totalPercent / count);

                    ChangeProgressPercent(user.Learner, parent.Identifier, percent);
                }
            }
        }

        private void CalculateByParts(GradeItem item, Enrollment[] learners)
        {
            foreach (var user in learners)
            {
                var totalScore = 0m;
                var totalCount = 0;

                foreach (var part in item.Parts)
                {
                    var partItem = _data.FindItem(part.Item);
                    if (partItem.Type == GradeItemType.Score && partItem.Format == GradeItemFormat.Point)
                        throw new CalculateScoreException("Points score cannot be part of calculation");

                    var (childScoreValue, isIgnored) = GetScoreValue(user.Learner, partItem);
                    if (isIgnored)
                        continue;

                    if (childScoreValue.HasValue)
                    {
                        var partScore = item.Weighting == GradeItemWeighting.ByPercent ? part.Score : 1m;
                        totalScore += childScoreValue.Value * partScore;
                    }

                    if (childScoreValue.HasValue || item.Weighting == GradeItemWeighting.EquallyWithNulls)
                        totalCount++;
                }

                var percent = totalCount > 0
                    ? RoundPercent(item.Weighting == GradeItemWeighting.ByPercent ? totalScore : totalScore / totalCount)
                    : (decimal?)null;

                ChangeProgressPercent(user.Learner, item.Identifier, percent);
            }
        }

        private void CalculateSum(GradeItem item, Enrollment[] learners)
        {
            var children = item.Children.Where(x => x.Format == GradeItemFormat.Number || x.Type != GradeItemType.Score).ToList();

            if (children.Count == 0)
                return;

            foreach (var user in learners)
            {
                var total = 0.0m;

                foreach (var child in children)
                {
                    var progress = _progresses.Find(x => x.UserIdentifier == user.Learner && x.GradeItemIdentifier == child.Identifier);

                    if (progress?.ProgressNumber != null && !progress.ProgressIsIgnored)
                        total += progress.ProgressNumber.Value;
                }

                ChangeProgressNumber(user.Learner, item.Identifier, total);
            }
        }

        private void ChangeProgressPoints(Guid user, Guid item, decimal points, decimal maxPoints, decimal? percent)
        {
            RunProgress(user, item, percent, (progress, progressIdentifier) =>
            {
                if (progress == null
                    || progress.ProgressPoints != points
                    || progress.ProgressMaxPoints != maxPoints
                    )
                {
                    _commands.Add(new ChangeProgressPoints(progressIdentifier, points, maxPoints, DateTimeOffset.UtcNow));
                }

                if (progress?.ProgressPercent != percent)
                    _commands.Add(new ChangeProgressPercent(progressIdentifier, percent, DateTimeOffset.UtcNow));
            });
        }

        private void ChangeProgressPercent(Guid user, Guid item, decimal? percent)
        {
            RunProgress(user, item, percent, (progress, progressIdentifier) =>
            {
                if (progress?.ProgressNumber != null)
                    _commands.Add(new ChangeProgressNumber(progressIdentifier, null, DateTimeOffset.UtcNow));

                if (progress?.ProgressPercent != percent)
                    _commands.Add(new ChangeProgressPercent(progressIdentifier, percent, DateTimeOffset.UtcNow));
            });
        }

        private void ChangeProgressNumber(Guid user, Guid item, decimal number)
        {
            RunProgress(user, item, null, (progress, progressIdentifier) =>
            {
                if (progress?.ProgressNumber != number)
                    _commands.Add(new ChangeProgressNumber(progressIdentifier, number, DateTimeOffset.UtcNow));
            });
        }

        private void RunProgress(Guid user, Guid item, decimal? percent, Action<QProgress, Guid> action)
        {
            Guid progressIdentifier;
            var progress = _progresses.Find(x => x.UserIdentifier == user && x.GradeItemIdentifier == item);

            if (progress == null)
            {
                if ((percent == null || percent == 0) && !_progresses.Any(x => x.UserIdentifier == user))
                    return;

                var command = _records.CreateCommandToAddProgress(null, _gradebook, item, user);

                _commands.Add(command);

                progressIdentifier = command.AggregateIdentifier;

                _newProgresses.Add(progressIdentifier);
            }
            else
            {
                progressIdentifier = progress.ProgressIdentifier;
            }

            if (progress != null && progress.ProgressIsIgnored)
                _commands.Add(new ChangeProgressPercent(progressIdentifier, null, null));
            else
            {
                _buffer.Set(user, item, percent);

                action(progress, progressIdentifier);
            }
        }

        private static decimal RoundPercent(decimal percent)
            => Math.Round(percent, 4, MidpointRounding.AwayFromZero);
    }
}