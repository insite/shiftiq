using System;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace InSite.Web.Change
{
    public class BaseChangeDescriptor
    {
        public class CustomFieldValue
        {
            public string Id { get; private set; }
            public string Name { get; private set; }
            public (string Name, string Value)[] CustomValues { get; private set; }

            public CustomFieldValue(string id, string name)
            {
                Id = id;
                Name = name;
            }

            public CustomFieldValue((string Name, string Value)[] customValues)
            {
                CustomValues = customValues;
            }
        }

        public class CustomField
        {
            public string ChangeType { get; private set; }
            public string FieldName { get; private set; }
            public Func<string, object, CustomFieldValue> Transform { get; private set; }

            public CustomField(string changeType, string fieldName, Func<string, object, CustomFieldValue> transform)
            {
                ChangeType = changeType;
                FieldName = fieldName;
                Transform = transform;
            }
        }

        private readonly string[] _excludedProperties;
        private readonly CustomField[] _customFields;

        public BaseChangeDescriptor(string[] excludedProperties, CustomField[] customFields)
        {
            _excludedProperties = excludedProperties;
            _customFields = customFields;
        }

        public string GetDescription(Guid aggregateId, int version)
        {
            var change = ServiceLocator.ChangeStore.GetChange(aggregateId, version);
            if (change == null)
                return null;

            var json = ServiceLocator.Serializer.Serialize(change, _excludedProperties, false);
            var changeType = change.GetType().Name;

            var activeCustomFields = _customFields != null
                ? _customFields.Where(x => x.ChangeType == null || x.ChangeType == changeType).ToList()
                : null;

            if (activeCustomFields == null || activeCustomFields.Count == 0)
                return json;

            var changeProps = JObject.Parse(json);

            foreach (var customField in activeCustomFields)
            {
                if (!changeProps.ContainsKey(customField.FieldName))
                    continue;

                var field = (JValue)changeProps[customField.FieldName];
                var newValue = customField.Transform(changeType, field.Value);

                SetValue(field, newValue);
            }

            return changeProps.ToString();
        }

        private static void SetValue(JValue field, CustomFieldValue newValue)
        {
            if (newValue.CustomValues != null)
            {
                var o = new JObject();
                foreach (var (name, value) in newValue.CustomValues)
                    o.Add(name, value);

                field.Replace(o);
            }
            else if (newValue.Name != null)
            {
                field.Replace(
                    new JObject
                    {
                            { "Id", newValue.Id },
                            { "Name", newValue.Name }
                    }
                );
            }
            else
                field.Value = newValue.Id;
        }
    }
}