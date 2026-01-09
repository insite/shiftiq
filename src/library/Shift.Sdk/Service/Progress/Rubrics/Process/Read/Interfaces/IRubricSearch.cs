using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace InSite.Application.Records.Read
{
    public interface IRubricSearch
    {
        QRubric GetRubric(Guid rubricId);
        QRubric[] GetRubrics(IEnumerable<Guid> rubricIds);
        RubricSearchItem[] GetRubrics(QRubricFilter filter);
        int CountRubrics(QRubricFilter filter);
        bool RubricHasAttempts(Guid rubricId);
        QRubric GetQuestionRubric(Guid questionId);
        Dictionary<Guid, QRubric> GetQuestionRubrics(IEnumerable<Guid> questionIds);

        QRubricCriterion GetCriterion(Guid criterionId);
        QRubricCriterion[] GetCriteria(Guid rubricId, params Expression<Func<QRubricCriterion, object>>[] includes);
        int CountCriteria(Guid rubricId);

        QRubricRating GetRating(Guid ratingId);
        QRubricRating[] GetRatings(Guid[] ratingsIds, params Expression<Func<QRubricRating, object>>[] includes);
    }
}
