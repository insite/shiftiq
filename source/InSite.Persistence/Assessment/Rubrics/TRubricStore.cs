using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TRubricStore
    {
        public static void ConnectRubric(Guid questionIdentifier, Guid rubricIdentifier)
        {
            // ASSUMPTION: Aleksey: For now, allow only one connection per question
            // If nothing will change we need to change the table structure

            using (var context = CreateContext())
            {
                var existing = context.TRubricConnections
                    .Where(x => x.QuestionIdentifier == questionIdentifier)
                    .ToList();

                if (existing.Count == 1 && existing[0].RubricIdentifier == rubricIdentifier)
                    return;

                if (existing.Count > 0)
                    context.TRubricConnections.RemoveRange(existing);

                var connection = new TRubricConnection
                {
                    ConnectionIdentifier = UniqueIdentifier.Create(),
                    RubricIdentifier = rubricIdentifier,
                    QuestionIdentifier = questionIdentifier
                };

                context.TRubricConnections.Add(connection);

                context.SaveChanges();
            }
        }

        public static void DisconnectRubrics(Guid questionIdentifier)
        {
            using (var context = CreateContext())
            {
                var existing = context.TRubricConnections
                    .Where(x => x.QuestionIdentifier == questionIdentifier)
                    .ToList();

                if (existing.Count == 0)
                    return;

                if (context.QAttemptQuestions.Any(x => x.QuestionIdentifier == questionIdentifier))
                    throw ApplicationError.Create("The rubric cannot be disconnected if the question has attempts");

                context.TRubricConnections.RemoveRange(existing);
                context.SaveChanges();
            }
        }

        public static void InsertRubric(TRubric rubric)
        {
            using (var context = CreateContext())
            {
                context.TRubrics.Add(rubric);
                context.SaveChanges();
            }
        }

        public static void UpdateRubric(TRubric rubric)
        {
            using (var context = CreateContext())
            {
                context.Entry(rubric).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public static void DeleteRubric(Guid rubricIdentifier)
        {
            using (var context = CreateContext())
            {
                var rubric = context.TRubrics
                    .Where(x => x.RubricIdentifier == rubricIdentifier)
                    .Include(x => x.Criteria.Select(y => y.Ratings))
                    .Include(x => x.Connections)
                    .FirstOrDefault();

                if (rubric == null)
                    return;

                if (rubric.Criteria.Count > 0)
                {
                    foreach (var criterion in rubric.Criteria)
                    {
                        if (criterion.Ratings.Count > 0)
                            context.TRubricRatings.RemoveRange(criterion.Ratings);
                    }

                    context.TRubricCriteria.RemoveRange(rubric.Criteria);
                }

                if (rubric.Connections.Count > 0)
                    context.TRubricConnections.RemoveRange(rubric.Connections);

                context.TRubrics.Remove(rubric);

                context.SaveChanges();
            }
        }

        public static void UpdateCriteria(
            List<TRubricCriterion> addedCriteria,
            List<TRubricCriterion> updatedCriteria,
            List<Guid> deletedCriteria,
            List<TRubricRating> addedRatings,
            List<TRubricRating> updatedRatings,
            List<Guid> deletedRatings
            )
        {
            using (var context = CreateContext())
            {
                var deletedRatingEntities = context.TRubricRatings.Where(x => deletedRatings.Contains(x.RatingIdentifier)).ToList();
                var deletedCriterionEntities = context.TRubricCriteria.Where(x => deletedCriteria.Contains(x.CriterionIdentifier)).ToList();

                context.TRubricRatings.RemoveRange(deletedRatingEntities);
                context.TRubricCriteria.RemoveRange(deletedCriterionEntities);

                context.TRubricCriteria.AddRange(addedCriteria);
                context.TRubricRatings.AddRange(addedRatings);

                foreach (var criteria in updatedCriteria)
                    context.Entry(criteria).State = EntityState.Modified;

                foreach (var rating in updatedRatings)
                    context.Entry(rating).State = EntityState.Modified;

                context.SaveChanges();
            }
        }

        static InternalDbContext CreateContext() => new InternalDbContext(false);
    }
}
