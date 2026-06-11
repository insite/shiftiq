using System;
using System.Collections.Generic;

using Shift.Common.Timeline.Changes;

using Shift.Common;

namespace InSite.Domain.Standards
{
    public class StandardAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new StandardState();

        public StandardState Data => (StandardState)State;

        public void CreateStandard(string standardType, int assetNumber, int sequence, ContentContainer content)
        {
            if (AggregateIdentifier == Guid.Empty)
                throw new AggregateException("AggregateIdentifier is empty");

            if (standardType.IsEmpty())
                throw new AggregateException("StandardType is empty");

            if (assetNumber <= 0)
                throw new AggregateException("AssetNumber is invalid");

            if (sequence < 0)
                throw new AggregateException("Sequence is invalid");

            if (content?.IsEmpty != false)
                throw new AggregateException("Content is empty");

            if (content.Title.Text.Default.IsEmpty())
                throw new AggregateException("ContentTitle is empty");

            Apply(new StandardCreated(standardType, assetNumber, sequence, content));
        }

        public void RemoveStandard()
        {
            if (Data.IsExist)
                Apply(new StandardRemoved());
        }

        public void ModifyStandardTimestamps(DateTimeOffset created, Guid createdBy, DateTimeOffset modified, Guid modifiedBy)
        {
            if (created == default)
                throw new AggregateException("Created is empty");

            if (createdBy == default)
                throw new AggregateException("CreatedBy is empty");

            if (modified == default)
                throw new AggregateException("Modified is empty");

            if (modifiedBy == default)
                throw new AggregateException("ModifiedBy is empty");

            var isChanged = Data.GetDateOffsetValue(StandardField.Created) != created
                || Data.GetGuidValue(StandardField.CreatedBy) != createdBy
                || Data.GetDateOffsetValue(StandardField.Modified) != modified
                || Data.GetGuidValue(StandardField.ModifiedBy) != modifiedBy;

            if (!isChanged)
                return;

            Apply(new StandardTimestampsModified(created, createdBy, modified, modifiedBy));
        }

        public void AddStandardCategory(StandardCategory[] categories)
        {
            if (categories.IsEmpty())
                return;

            var changeData = new List<StandardCategory>(categories.Length);

            foreach (var category in categories)
            {
                if (category.CategoryId == default)
                    throw new AggregateException("CategoryIdentifier is empty");

                if (category.Sequence.HasValue && category.Sequence.Value < 0)
                    throw new AggregateException("Sequence is invalid");

                if (!Data.HasCategory(category.CategoryId))
                    changeData.Add(category.Clone());
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardCategoryAdded(changeData.ToArray()));
        }

        public void RemoveStandardCategory(Guid[] categoryIds)
        {
            if (categoryIds.IsEmpty())
                return;

            var changeData = new List<Guid>(categoryIds.Length);

            foreach (var categoryId in categoryIds)
            {
                if (categoryId == default)
                    throw new AggregateException("CategoryIdentifier is empty");

                if (Data.HasCategory(categoryId))
                    changeData.Add(categoryId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardCategoryRemoved(changeData.ToArray()));
        }

        public void AddStandardConnection(StandardConnection[] connections)
        {
            if (connections.IsEmpty())
                return;

            var changeData = new List<StandardConnection>(connections.Length);

            foreach (var connection in connections)
            {
                if (connection.ToStandardId == default)
                    throw new AggregateException("ToStandardIdentifier is empty");

                if (connection.ConnectionType.IsEmpty())
                    throw new AggregateException("ConnectionType is empty");

                if (!Data.HasConnection(connection.ToStandardId))
                    changeData.Add(connection.Clone());
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardConnectionAdded(changeData.ToArray()));
        }

        public void RemoveStandardConnection(Guid[] toStandardIds)
        {
            if (toStandardIds.IsEmpty())
                return;

            var changeData = new List<Guid>(toStandardIds.Length);

            foreach (var toStandardId in toStandardIds)
            {
                if (toStandardId == default)
                    throw new AggregateException("ToStandardIdentifier is empty");

                if (Data.HasConnection(toStandardId))
                    changeData.Add(toStandardId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardConnectionRemoved(changeData.ToArray()));
        }

        public void AddStandardContainment(StandardContainment[] containments)
        {
            if (containments.IsEmpty())
                return;

            var changeData = new List<StandardContainment>(containments.Length);

            foreach (var containment in containments)
            {
                if (containment.ChildStandardId == default)
                    throw new AggregateException("ChildStandardIdentifier is empty");

                if (containment.ChildSequence < 0)
                    throw new AggregateException("ChildSequence is invalid");

                if (!Data.HasContainment(containment.ChildStandardId))
                {
                    var item = containment.Clone();

                    if (string.IsNullOrWhiteSpace(item.CreditType))
                        item.CreditType = null;

                    changeData.Add(item);
                }
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardContainmentAdded(changeData.ToArray()));
        }

        public void RemoveStandardContainment(Guid[] childStandardIds)
        {
            if (childStandardIds.IsEmpty())
                return;

            var changeData = new List<Guid>(childStandardIds.Length);

            foreach (var childStandardId in childStandardIds)
            {
                if (childStandardId == default)
                    throw new AggregateException("ChildStandardIdentifier is empty");

                if (Data.HasContainment(childStandardId))
                    changeData.Add(childStandardId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardContainmentRemoved(changeData.ToArray()));
        }

        public void AddStandardOrganization(Guid[] organizationIds)
        {
            if (organizationIds.IsEmpty())
                return;

            var changeData = new List<Guid>(organizationIds.Length);

            foreach (var orgId in organizationIds)
            {
                if (orgId == default)
                    throw new AggregateException("OrganizationIdentifier is empty");

                if (!Data.HasOrganization(orgId))
                    changeData.Add(orgId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardOrganizationAdded(changeData.ToArray()));
        }

        public void RemoveStandardOrganization(Guid[] organizationIds)
        {
            if (organizationIds.IsEmpty())
                return;

            var changeData = new List<Guid>(organizationIds.Length);

            foreach (var organizationId in organizationIds)
            {
                if (organizationId == default)
                    throw new AggregateException("OrganizationIdentifier is empty");

                if (Data.HasOrganization(organizationId))
                    changeData.Add(organizationId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardOrganizationRemoved(changeData.ToArray()));
        }

        public void AddStandardAchievement(Guid[] achievementIds)
        {
            if (achievementIds.IsEmpty())
                return;

            var changeData = new List<Guid>(achievementIds.Length);

            foreach (var id in achievementIds)
            {
                if (id == default)
                    throw new AggregateException("AchievementIdentifier is empty");

                if (!Data.HasAchievement(id))
                    changeData.Add(id);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardAchievementAdded(changeData.ToArray()));
        }

        public void RemoveStandardAchievement(Guid[] achievementIds)
        {
            if (achievementIds.IsEmpty())
                return;

            var changeData = new List<Guid>(achievementIds.Length);

            foreach (var achievementId in achievementIds)
            {
                if (achievementId == default)
                    throw new AggregateException("AchievementIdentifier is empty");

                if (Data.HasAchievement(achievementId))
                    changeData.Add(achievementId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardAchievementRemoved(changeData.ToArray()));
        }

        public void AddStandardGroup(StandardGroup[] groups)
        {
            if (groups.IsEmpty())
                return;

            var changeData = new List<StandardGroup>(groups.Length);

            foreach (var group in groups)
            {
                if (group.GroupId == default)
                    throw new AggregateException("GroupIdentifier is empty");

                if (!Data.HasGroup(group.GroupId))
                    changeData.Add(group.Clone());
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardGroupAdded(changeData.ToArray()));
        }

        public void RemoveStandardGroup(Guid[] groupIds)
        {
            if (groupIds.IsEmpty())
                return;

            var changeData = new List<Guid>(groupIds.Length);

            foreach (var groupId in groupIds)
            {
                if (groupId == default)
                    throw new AggregateException("GroupIdentifier is empty");

                if (Data.HasGroup(groupId))
                    changeData.Add(groupId);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardGroupRemoved(changeData.ToArray()));
        }

        public void ModifyStandardContainment(StandardContainment[] containments)
        {
            if (containments.IsEmpty())
                return;

            var changeData = new List<StandardContainment>(containments.Length);

            foreach (var containment in containments)
            {
                if (containment.ChildStandardId == default)
                    throw new AggregateException("ChildStandardIdentifier is empty");

                var exist = Data.GetContainment(containment.ChildStandardId);
                if (exist == null)
                    continue;

                if (containment.ChildSequence < 0)
                    throw new AggregateException("ChildSequence is invalid");

                var item = containment.Clone();

                if (string.IsNullOrWhiteSpace(item.CreditType))
                    item.CreditType = null;
                else if (item.CreditType.Length > 10)
                    throw new AggregateException("CreditType exceeded the maximum length: " + item.CreditType);

                var isChanged = exist.ChildSequence != item.ChildSequence
                    || exist.CreditHours != item.CreditHours
                    || exist.CreditType != item.CreditType;

                if (isChanged)
                    changeData.Add(item);
            }

            if (changeData.IsEmpty())
                return;

            Apply(new StandardContainmentModified(changeData.ToArray()));
        }

        public void ModifyStandardContent(ContentContainer content)
        {
            if (!content.HasItems)
                return;

            var changeContent = content.Clone();
            foreach (var label in changeContent.GetLabels())
            {
                var stateItem = Data.Content[label];
                var changeItem = changeContent[label];
                changeItem.Text.RemoveExist(stateItem.Text);
                changeItem.Html.RemoveExist(stateItem.Html);
                changeItem.Snip.RemoveExist(stateItem.Snip);
            }

            if (!changeContent.HasItems)
                return;

            if (changeContent.Title.Text.Exists(MultilingualString.DefaultLanguage) && changeContent.Title.Text.Default.IsEmpty())
                throw new AggregateException("ContentTitle is empty");

            Apply(new StandardContentModified(changeContent));
        }

        public void ModifyStandardFieldText(StandardField field, string value)
        {
            if (value.IsEmpty())
                value = null;

            StandardFieldList.Validate(field, value, true);

            if (Data.GetTextValue(field) == value)
                return;

            Apply(new StandardFieldTextModified(field, value));
        }

        public void ModifyStandardFieldDateOffset(StandardField field, DateTimeOffset? value)
        {
            StandardFieldList.Validate(field, value, true);

            if (Data.GetDateOffsetValue(field) == value)
                return;

            Apply(new StandardFieldDateOffsetModified(field, value));
        }

        public void ModifyStandardFieldBool(StandardField field, bool? value)
        {
            StandardFieldList.Validate(field, value, true);

            if (Data.GetBoolValue(field) == value)
                return;

            Apply(new StandardFieldBoolModified(field, value));
        }

        public void ModifyStandardFieldInt(StandardField field, int? value)
        {
            StandardFieldList.Validate(field, value, true);

            if (Data.GetIntValue(field) == value)
                return;

            Apply(new StandardFieldIntModified(field, value));
        }

        public void ModifyStandardFieldDecimal(StandardField field, decimal? value)
        {
            StandardFieldList.Validate(field, value, true);

            if (Data.GetDecimalValue(field) == value)
                return;

            Apply(new StandardFieldDecimalModified(field, value));
        }

        public void ModifyStandardFieldGuid(StandardField field, Guid? value)
        {
            StandardFieldList.Validate(field, value, true);

            if (Data.GetGuidValue(field) == value)
                return;

            Apply(new StandardFieldGuidModified(field, value));
        }

        public void ModifyStandardFields(IDictionary<StandardField, object> values)
        {
            var changeValues = new Dictionary<StandardField, object>();

            foreach (var kv in values)
            {
                var field = kv.Key;
                var newValue = kv.Value;

                StandardFieldList.Validate(field, newValue, true);

                var nowValue = Data.GetFieldValue(field);
                var isNowNull = nowValue == null;
                var isNewNull = newValue == null;

                if (isNowNull != isNewNull || !isNowNull && !nowValue.Equals(newValue))
                    changeValues[field] = newValue;
            }

            if (changeValues.IsEmpty())
                return;

            Apply(new StandardFieldsModified(changeValues));
        }
    }
}
