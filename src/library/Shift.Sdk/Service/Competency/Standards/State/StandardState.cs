using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Newtonsoft.Json;

using Shift.Common;

namespace InSite.Domain.Standards
{
    [Serializable]
    public class StandardState : AggregateState
    {
        #region Properties

        [JsonProperty]
        public bool IsExist { get; private set; }

        [JsonProperty]
        public ContentContainer Content { get; private set; }

        #endregion

        #region Fields 

        [JsonProperty(PropertyName = "Values"), JsonConverter(typeof(StandardStateDictionaryConverter))]
        private Dictionary<StandardField, object> _values = new Dictionary<StandardField, object>();

        [JsonProperty(PropertyName = "Categories")]
        private HashSet<Guid> _categories = new HashSet<Guid>();

        [JsonProperty(PropertyName = "Connections")]
        private HashSet<Guid> _connections = new HashSet<Guid>();

        [JsonProperty(PropertyName = "Containments")]
        private Dictionary<Guid, StandardContainment> _containments = new Dictionary<Guid, StandardContainment>();

        [JsonProperty(PropertyName = "Organizations")]
        private HashSet<Guid> _organizations = new HashSet<Guid>();

        [JsonProperty(PropertyName = "Achievements")]
        private HashSet<Guid> _achievements = new HashSet<Guid>();

        [JsonProperty(PropertyName = "Groups")]
        private HashSet<Guid> _groups = new HashSet<Guid>();

        #endregion

        #region Methods (Values)

        public object GetFieldValue(StandardField field) => _values.GetOrDefault(field);

        public bool? GetBoolValue(StandardField field) => (bool?)GetFieldValue(field);

        public DateTimeOffset? GetDateOffsetValue(StandardField field) => (DateTimeOffset?)GetFieldValue(field);

        public decimal? GetDecimalValue(StandardField field) => (decimal?)GetFieldValue(field);

        public Guid? GetGuidValue(StandardField field) => (Guid?)GetFieldValue(field);

        public int? GetIntValue(StandardField field) => (int?)GetFieldValue(field);

        public string GetTextValue(StandardField field) => (string)GetFieldValue(field);

        private void SetValue<T>(StandardField field, T value, bool directlyModified)
        {
            StandardFieldList.Validate(field, value, directlyModified);

            if (value == null)
                _values.Remove(field);
            else
                _values[field] = value;
        }

        private void SetValue(StandardField field, object value, bool directlyModified)
        {
            StandardFieldList.Validate(field, value, directlyModified);

            if (value == null)
                _values.Remove(field);
            else
                _values[field] = value;
        }

        #endregion

        #region Methods (Links)

        public bool HasCategory(Guid categoryId) => _categories.Contains(categoryId);

        public bool HasConnection(Guid toStandardId) => _connections.Contains(toStandardId);

        public bool HasContainment(Guid childStandardId) => _containments.ContainsKey(childStandardId);

        public bool HasOrganization(Guid organizationId) => _organizations.Contains(organizationId);

        public bool HasAchievement(Guid achievementId) => _achievements.Contains(achievementId);

        public bool HasGroup(Guid groupId) => _groups.Contains(groupId);

        public StandardContainment GetContainment(Guid childStandardId) => _containments.GetOrDefault(childStandardId);

        #endregion

        #region Methods (When)

        public void When(StandardCreated e)
        {
            IsExist = true;
            SetValue(StandardField.OrganizationIdentifier, e.OriginOrganization, false);
            SetValue(StandardField.StandardType, e.StandardType, false);
            SetValue(StandardField.AssetNumber, e.AssetNumber, false);
            SetValue(StandardField.Sequence, e.Sequence, false);
            SetValue(StandardField.Created, e.ChangeTime, false);
            SetValue(StandardField.CreatedBy, e.OriginUser, false);

            Content = e.Content.Clone();
            Content.CreateSnips();

            SetModified(e);
        }

        public void When(StandardRemoved e)
        {
            IsExist = false;
        }

        public void When(StandardTimestampsModified e)
        {
            SetValue(StandardField.Created, e.Created, false);
            SetValue(StandardField.CreatedBy, e.CreatedBy, false);
            SetValue(StandardField.Modified, e.Modified, false);
            SetValue(StandardField.ModifiedBy, e.ModifiedBy, false);
        }

        public void When(StandardCategoryAdded e)
        {
            foreach (var c in e.Categories)
                _categories.Add(c.CategoryId);
        }

        public void When(StandardCategoryRemoved e)
        {
            foreach (var id in e.CategoryIds)
                _categories.Remove(id);
        }

        public void When(StandardConnectionAdded e)
        {
            foreach (var c in e.Connections)
                _connections.Add(c.ToStandardId);
        }

        public void When(StandardConnectionRemoved e)
        {
            foreach (var id in e.ToStandardIds)
                _connections.Remove(id);
        }

        public void When(StandardContainmentAdded e)
        {
            foreach (var c in e.Containments)
                _containments.Add(c.ChildStandardId, c);
        }

        public void When(StandardContainmentModified e)
        {
            foreach (var c in e.Containments)
                _containments[c.ChildStandardId] = c;
        }

        public void When(StandardContainmentRemoved e)
        {
            foreach (var id in e.ChildStandardIds)
                _containments.Remove(id);
        }

        public void When(StandardContentModified e)
        {
            Content.Set(e.Content, ContentContainer.SetNullAction.Remove);
            Content.CreateSnips();

            SetModified(e);
        }

        public void When(StandardOrganizationAdded e)
        {
            foreach (var id in e.OrganizationIds)
                _organizations.Add(id);
        }

        public void When(StandardOrganizationRemoved e)
        {
            foreach (var id in e.OrganizationIds)
                _organizations.Remove(id);
        }

        public void When(StandardAchievementAdded e)
        {
            foreach (var id in e.AchievementIds)
                _achievements.Add(id);
        }

        public void When(StandardAchievementRemoved e)
        {
            foreach (var id in e.AchievementIds)
                _achievements.Remove(id);
        }

        public void When(StandardGroupAdded e)
        {
            foreach (var g in e.Groups)
                _groups.Add(g.GroupId);
        }

        public void When(StandardGroupRemoved e)
        {
            foreach (var id in e.GroupIds)
                _groups.Remove(id);
        }

        public void When(StandardFieldTextModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldDateOffsetModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldBoolModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldIntModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldDecimalModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldGuidModified e)
        {
            SetValue(e.Field, e.Value, true);
            SetModified(e);
        }

        public void When(StandardFieldsModified e)
        {
            foreach (var kv in e.Fields)
                SetValue(kv.Key, kv.Value, true);

            SetModified(e);
        }

        #endregion

        #region Methods (helpers)

        private void SetModified(Change e)
        {
            SetValue(StandardField.ModifiedBy, e.OriginUser, false);
            SetValue(StandardField.Modified, e.ChangeTime, false);
        }

        #endregion
    }
}
