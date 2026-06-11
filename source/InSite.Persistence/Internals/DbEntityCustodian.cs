using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace InSite.Persistence
{
    public class DbEntityCustodian
    {
        public bool IsChanged(DbEntityEntry entry)
        {
            var isAdded = entry.State == EntityState.Added;
            var isModified = entry.State == EntityState.Modified;

            return isAdded || isModified;
        }

        public void PrepareToSaveChanges(DbContext context)
        {
            foreach (var entry in context.ChangeTracker.Entries())
                SetEmptyStringsToNull(entry);
        }

        private void SetEmptyStringsToNull(DbEntityEntry entry)
        {
            if (!IsChanged(entry))
                return;

            var entityType = entry.Entity.GetType();

            foreach (var propertyName in entry.CurrentValues.PropertyNames)
            {
                var entryProperty = entry.Property(propertyName);
                if (entry.State != EntityState.Added && !entryProperty.IsModified)
                {
                    continue;
                }

                var propertyInfo = entityType.GetProperty(propertyName);
                if (propertyInfo.PropertyType != typeof(string))
                {
                    continue;
                }

                var propertyValue = (string)entryProperty.CurrentValue;
                if (propertyValue != null)
                {
                    var trimmedValue = propertyValue.Trim();
                    if (trimmedValue.Length == 0)
                    {
                        entryProperty.CurrentValue = null;
                    }
                    else if (trimmedValue.Length != propertyValue.Length)
                    {
                        entryProperty.CurrentValue = trimmedValue;
                    }
                }
            }
        }
    }
}
