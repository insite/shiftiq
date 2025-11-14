using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Persistence
{
    public static class TCandidateLanguageProficiencyStore
    {
        public static void Delete(Guid userId, Guid organizationId)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TCandidateLanguageProficiencies
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .ToList();

                db.TCandidateLanguageProficiencies.RemoveRange(existing);
                db.SaveChanges();
            }
        }

        public static void UpdateFirst(Guid userId, Guid organizationId, (Guid Language, int Level) proficiency)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TCandidateLanguageProficiencies
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .OrderBy(x => x.Sequence)
                    .FirstOrDefault();

                if (existing != null)
                {
                    existing.LanguageItemIdentifier = proficiency.Language;
                    existing.ProficiencyLevel = proficiency.Level;
                }
                else
                {
                    var entity = new TCandidateLanguageProficiency
                    {
                        LanguageProficiencyIdentifier = UniqueIdentifier.Create(),
                        UserIdentifier = userId,
                        OrganizationIdentifier = organizationId,
                        LanguageItemIdentifier = proficiency.Language,
                        ProficiencyLevel = proficiency.Level,
                        Sequence = 1
                    };

                    db.TCandidateLanguageProficiencies.Add(entity);
                }

                db.SaveChanges();
            }
        }

        public static void Update(Guid userId, Guid organizationId, List<(Guid Language, int Level)> proficiencies)
        {
            using (var db = new InternalDbContext())
            {
                var existing = db.TCandidateLanguageProficiencies
                    .Where(x => x.UserIdentifier == userId && x.OrganizationIdentifier == organizationId)
                    .OrderBy(x => x.Sequence)
                    .ToList();

                var sharedCount = Math.Min(existing.Count, proficiencies.Count);

                for (int i = 0; i < sharedCount; i++)
                {
                    var entity = existing[i];
                    var proficiency = proficiencies[i];

                    entity.Sequence = i + 1;
                    entity.LanguageItemIdentifier = proficiency.Language;
                    entity.ProficiencyLevel = proficiency.Level;
                }

                for (int i = sharedCount; i < proficiencies.Count; i++)
                {
                    var proficiency = proficiencies[i];

                    var entity = new TCandidateLanguageProficiency
                    {
                        LanguageProficiencyIdentifier = UniqueIdentifier.Create(),
                        UserIdentifier = userId,
                        OrganizationIdentifier = organizationId,
                        LanguageItemIdentifier = proficiency.Language,
                        ProficiencyLevel = proficiency.Level,
                        Sequence = i + 1
                    };

                    db.TCandidateLanguageProficiencies.Add(entity);
                }

                for (int i = sharedCount; i < existing.Count; i++)
                    db.TCandidateLanguageProficiencies.Remove(existing[i]);

                db.SaveChanges();
            }
        }
    }
}
