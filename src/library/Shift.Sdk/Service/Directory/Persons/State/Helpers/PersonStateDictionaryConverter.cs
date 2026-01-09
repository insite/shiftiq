using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Contacts
{
    internal class PersonStateDictionaryConverter : StateDictionaryConverter<PersonField>
    {
        private static readonly IReadOnlyDictionary<PersonField, Func<JsonReader, object>> _mapping;

        static PersonStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(PersonFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<PersonField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
