using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Sdk.UI
{
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    public abstract class BaseObject
    {
        public bool IsParsed { get; protected set; }

        private Dictionary<string, object> _values = new Dictionary<string, object>();

        protected bool HasValue([CallerMemberName] string name = null) =>
            _values.ContainsKey(name);

        protected T GetValue<T>(T @default = default, [CallerMemberName] string name = null) =>
            _values.ContainsKey(name) ? (T)_values[name] : @default;

        protected bool SetValue<T>(T value, [CallerMemberName] string name = null)
        {
            if (IsParsed)
                return false;

            if (HasValue(name))
            {
                var currentValue = GetValue<T>(name: name);
                if (EqualityComparer<T>.Default.Equals(currentValue, value))
                    return false;
            }

            if (value == null)
                _values.Remove(name);
            else
                _values[name] = value;

            return true;
        }

        public abstract void VerifyObject();

        protected void VerifyRequiredField(string name)
        {
            if (!HasValue(name))
                throw ApplicationError.Create("Missing required field: " + name);
        }

        protected void VerifyFieldValue<T>(T currentValue, T expectedValue, string fieldName, bool allowDefault)
        {
            if (!EqualityComparer<T>.Default.Equals(currentValue, expectedValue) && (!allowDefault || !EqualityComparer<T>.Default.Equals(currentValue, default)))
                throw ApplicationError.Create("The expected value does not match the current value: " + fieldName);
        }

        protected void VerifyFieldEnumerable<T>(IEnumerable<T> currentValue, IEnumerable<T> expectedValue, string fieldName, bool allowDefault)
        {
            if (currentValue == null)
                currentValue = Enumerable.Empty<T>();

            if (expectedValue == null)
                expectedValue = Enumerable.Empty<T>();

            if (!currentValue.SequenceEqual(expectedValue))
                throw ApplicationError.Create("The expected value does not match the current value: " + fieldName);
        }

        [OnDeserialized]
        private void OnDeserializedMethod(StreamingContext context)
        {
            IsParsed = true;
        }

        protected Exception CreateSerializationRequiredFieldError(string fieldName)
        {
            return ApplicationError.Create("Serialization error: {0}.{1} is required field", GetType().Name, fieldName);
        }
    }
}