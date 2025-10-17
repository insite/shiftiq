using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    internal class UserStateDictionaryConverter : StateDictionaryConverter<UserField>
    {
        private static readonly IReadOnlyDictionary<UserField, Func<JsonReader, object>> _mapping;

        static UserStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(UserFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<UserField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
