using System;
using System.Collections.Generic;

namespace Shift.Common
{
    public class EmailVariables
    {
        private Dictionary<string, string> _items = new Dictionary<string, string>();

        public EmailVariables() { }

        public EmailVariables(Guid user, string address, Guid organization, Dictionary<string, string> variables)
        {
            RecipientIdentifier = user;
            RecipientEmail = address;
            OrganizationIdentifier = organization;

            foreach (var item in variables)
                SetValue(item.Key, item.Value);
        }

        public Dictionary<string, string> GetItems() => _items;

        public string RecipientCode { get => GetValue(nameof(RecipientCode)); set => SetValue(nameof(RecipientCode), value); }

        public string RecipientEmail { get => GetValue(nameof(RecipientEmail)); set => SetValue(nameof(RecipientEmail), value); }

        public Guid? RecipientIdentifier { get => GetValueAsGuid(nameof(RecipientIdentifier)); set => SetValueAsGuid(nameof(RecipientIdentifier), value); }

        public string RecipientName { get => GetValue(nameof(RecipientName)); set => SetValue(nameof(RecipientName), value); }

        public string RecipientNameFirst { get => GetValue(nameof(RecipientNameFirst)); set => SetValue(nameof(RecipientNameFirst), value); }

        public string RecipientNameLast { get => GetValue(nameof(RecipientNameLast)); set => SetValue(nameof(RecipientNameLast), value); }

        public Guid? OrganizationIdentifier { get => GetValueAsGuid(nameof(OrganizationIdentifier)); set => SetValueAsGuid(nameof(OrganizationIdentifier), value); }

        public string GetValue(string key)
        {
            var lowercaseKey = CreateLowercaseKey(key);

            if (_items.ContainsKey(lowercaseKey)) 
                return _items[lowercaseKey];

            return null;
        }

        public Guid? GetValueAsGuid(string key)
        {
            var lowercaseKey = CreateLowercaseKey(key);

            if (_items.ContainsKey(lowercaseKey))
                return Guid.Parse(_items[lowercaseKey]);

            return null;
        }

        public void SetValue(string key, string value)
        {
            var lowercaseKey = CreateLowercaseKey(key);

            if (_items.ContainsKey(lowercaseKey))
                _items[lowercaseKey] = value;
            else
                _items.Add(lowercaseKey, value);
        }

        public void SetValueAsGuid(string key, Guid? value)
        {
            var lowercaseKey = CreateLowercaseKey(key);

            if (_items.ContainsKey(lowercaseKey))
                _items[lowercaseKey] = value?.ToString();
            else
                _items.Add(lowercaseKey, value?.ToString());
        }

        private string CreateLowercaseKey(string key)
        {
            return key.ToLower();
        }
    }
}
