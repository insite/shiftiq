using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Attempts.Read;

using Shift.Common;

namespace InSite.Persistence
{
    internal class AttemptSummary
    {
        #region Classes

        private class AttemptState
        {
            public Guid Identifier { get; private set; }
            public bool IsStarted { get; private set; }
            public bool IsImported { get; private set; }
            public bool IsSubmitted { get; private set; }
            public bool IsGraded { get; private set; }
            public bool? IsPassing { get; private set; }

            public AttemptState(QAttempt attempt)
            {
                Identifier = attempt.AttemptIdentifier;

                IsStarted = attempt.AttemptStarted.HasValue;
                IsSubmitted = attempt.AttemptSubmitted.HasValue;
                IsGraded = attempt.AttemptGraded.HasValue;
                IsImported = attempt.AttemptImported.HasValue;
                IsPassing = IsSubmitted ? attempt.AttemptIsPassing : (bool?)null;
            }
        }

        #endregion

        #region Properties

        public TLearnerAttemptSummary Entity => _summary;

        #endregion

        #region Fields

        private InternalDbContext _db;
        private TLearnerAttemptSummary _summary;
        private List<AttemptState> _states;
        private QAttempt[] _allEntities = null;

        #endregion

        public AttemptSummary(InternalDbContext db, Guid form, Guid learner)
        {
            _db = db;
            _summary = _db.TLearnerAttemptSummaries
                .FirstOrDefault(x => x.FormIdentifier == form && x.LearnerUserIdentifier == learner);

            if (_summary == null)
                _db.TLearnerAttemptSummaries.Add(_summary = new TLearnerAttemptSummary
                {
                    FormIdentifier = form,
                    LearnerUserIdentifier = learner
                });
        }

        public void OnInsert(QAttempt attempt)
        {
            _summary.AttemptTotalCount += 1;

            var state = new AttemptState(attempt);

            if (state.IsImported)
                _summary.AttemptImportedCount += 1;

            Update(null, state, attempt);
        }

        public void OnRemove(QAttempt attempt)
        {
            _summary.AttemptVoidedCount += 1;

            var state = new AttemptState(attempt);

            if (state.IsImported)
                _summary.AttemptImportedCount -= 1;

            Update(state, null, attempt);
        }

        public void OnBeforeUpdate(QAttempt attempt)
        {
            var state = new AttemptState(attempt);

            if (_states == null)
                _states = new List<AttemptState>();

            var index = _states.FindIndex(x => x.Identifier == state.Identifier);
            if (index >= 0)
                _states[index] = state;
            else
                _states.Add(state);
        }

        public void OnAfterUpdate(QAttempt attempt)
        {
            var beforeState = _states.Find(x => x.Identifier == attempt.AttemptIdentifier);
            if (beforeState == null)
                throw ApplicationError.Create("Previous state not found");

            var afterState = new AttemptState(attempt);

            Update(beforeState, afterState, attempt);
        }

        private void Update(AttemptState before, AttemptState after, QAttempt attempt)
        {
            if (!(before?.IsStarted ?? false) && after?.IsStarted == true)
            {
                _summary.AttemptStartedCount += 1;

                if (!after.IsImported)
                {
                    _summary.AttemptLastStartedIdentifier = attempt.AttemptIdentifier;
                    _summary.AttemptLastStarted = attempt.AttemptStarted.Value;
                }
            }
            else if (before?.IsStarted == true && !(after?.IsStarted ?? false))
            {
                _summary.AttemptStartedCount -= 1;

                if (!before.IsImported && _summary.AttemptLastStartedIdentifier == attempt.AttemptIdentifier)
                {
                    var lastStarted = GetLastAttempt(attempt.AttemptIdentifier, x => x.AttemptStarted.HasValue);
                    _summary.AttemptLastStartedIdentifier = lastStarted?.AttemptIdentifier;
                    _summary.AttemptLastStarted = lastStarted?.AttemptStarted.Value;
                }
            }

            if (!(before?.IsSubmitted ?? false) && after?.IsSubmitted == true)
            {
                _summary.AttemptSubmittedCount += 1;

                if (!after.IsImported)
                {
                    _summary.AttemptLastSubmittedIdentifier = attempt.AttemptIdentifier;
                    _summary.AttemptLastSubmitted = attempt.AttemptSubmitted.Value;
                }
            }
            else if (before?.IsSubmitted == true && !(after?.IsSubmitted ?? false))
            {
                _summary.AttemptSubmittedCount -= 1;

                if (!before.IsImported && _summary.AttemptLastSubmittedIdentifier == attempt.AttemptIdentifier)
                {
                    var lastSubmitted = GetLastAttempt(attempt.AttemptIdentifier, x => x.AttemptSubmitted.HasValue);
                    _summary.AttemptLastSubmittedIdentifier = lastSubmitted?.AttemptIdentifier;
                    _summary.AttemptLastSubmitted = lastSubmitted?.AttemptSubmitted.Value;
                }
            }

            if (!(before?.IsGraded ?? false) && after?.IsGraded == true)
            {
                _summary.AttemptGradedCount += 1;

                if (!after.IsImported)
                {
                    _summary.AttemptLastGradedIdentifier = attempt.AttemptIdentifier;
                    _summary.AttemptLastGraded = attempt.AttemptGraded.Value;
                }
            }
            else if (before?.IsGraded == true && !(after?.IsGraded ?? false))
            {
                _summary.AttemptGradedCount -= 1;

                if (!before.IsImported && _summary.AttemptLastGradedIdentifier == attempt.AttemptIdentifier)
                {
                    var lastGraded = GetLastAttempt(attempt.AttemptIdentifier, x => x.AttemptGraded.HasValue);
                    _summary.AttemptLastGradedIdentifier = lastGraded?.AttemptIdentifier;
                    _summary.AttemptLastGraded = lastGraded?.AttemptGraded.Value;
                }
            }

            if (!(before?.IsPassing ?? false) && after?.IsPassing == true)
            {
                _summary.AttemptPassedCount += 1;

                if (!after.IsImported)
                {
                    _summary.AttemptLastPassedIdentifier = attempt.AttemptIdentifier;
                    _summary.AttemptLastPassed = attempt.AttemptGraded ?? attempt.AttemptSubmitted.Value;
                }
            }
            else if (before?.IsPassing == true && !(after?.IsPassing ?? false))
            {
                _summary.AttemptPassedCount -= 1;

                if (!before.IsImported && _summary.AttemptLastPassedIdentifier == attempt.AttemptIdentifier)
                {
                    var lastPassed = GetLastAttempt(attempt.AttemptIdentifier, x => x.AttemptSubmitted.HasValue && x.AttemptIsPassing);
                    _summary.AttemptLastPassedIdentifier = lastPassed?.AttemptIdentifier;
                    _summary.AttemptLastPassed = lastPassed?.AttemptGraded ?? lastPassed?.AttemptSubmitted;
                }
            }

            if ((before?.IsPassing ?? true) && after?.IsPassing == false)
            {
                _summary.AttemptFailedCount += 1;

                if (!after.IsImported)
                {
                    _summary.AttemptLastFailedIdentifier = attempt.AttemptIdentifier;
                    _summary.AttemptLastFailed = attempt.AttemptGraded ?? attempt.AttemptSubmitted;
                }
            }
            else if (before?.IsPassing == false && (after?.IsPassing ?? true))
            {
                _summary.AttemptFailedCount -= 1;

                if (!before.IsImported && _summary.AttemptLastFailedIdentifier == attempt.AttemptIdentifier)
                {
                    var lastFailed = GetLastAttempt(attempt.AttemptIdentifier, x => x.AttemptGraded.HasValue && !x.AttemptIsPassing);
                    _summary.AttemptLastFailedIdentifier = lastFailed?.AttemptIdentifier;
                    _summary.AttemptLastFailed = lastFailed?.AttemptGraded ?? lastFailed?.AttemptSubmitted;
                }
            }
        }

        private QAttempt GetLastAttempt(Guid excludeId, Func<QAttempt, bool> condition)
        {
            if (_allEntities == null)
                _allEntities = _db.QAttempts.AsNoTracking()
                    .Where(x => x.FormIdentifier == _summary.FormIdentifier && x.LearnerUserIdentifier == _summary.LearnerUserIdentifier)
                    .OrderByDescending(x => x.AttemptNumber)
                    .ToArray();

            return _allEntities
                .Where(x => x.AttemptIdentifier != excludeId && (!(x is QAttempt y) || !y.AttemptImported.HasValue))
                .Where(condition)
                .FirstOrDefault();
        }
    }
}
