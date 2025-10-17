using System;

using InSite.Domain.Records;

namespace InSite.Application.Records.Read
{
    public interface IRubricStore
    {
        Guid StartTransaction(Guid rubricId);
        void CancelTransaction(Guid transactionId);
        void CommitTransaction(Guid transactionId);

        void Insert(RubricCreated e);
        void Delete(RubricDeleted e);
        void Update(RubricRenamed e);
        void Update(RubricDescribed e);
        void Update(RubricTimestampModified e);
        void Update(RubricTranslated e);

        void Insert(RubricCriterionAdded e);
        void Delete(RubricCriterionRemoved e);
        void Update(RubricCriterionRenamed e);
        void Update(RubricCriterionDescribed e);
        void Update(RubricCriterionIsRangeModified e);
        void Update(RubricCriterionTranslated e);

        void Insert(RubricCriterionRatingAdded e);
        void Delete(RubricCriterionRatingRemoved e);
        void Update(RubricCriterionRatingRenamed e);
        void Update(RubricCriterionRatingDescribed e);
        void Update(RubricCriterionRatingPointsModified e);
        void Update(RubricCriterionRatingTranslated e);

        void DeleteAll();
        void Delete(Guid rubricId);
    }
}
