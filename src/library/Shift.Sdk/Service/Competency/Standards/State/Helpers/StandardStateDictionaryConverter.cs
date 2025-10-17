using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardStateDictionaryConverter : StateDictionaryConverter<StandardField>
    {
        private static readonly IReadOnlyDictionary<StandardField, Func<JsonReader, object>> _mapping;

        static StandardStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(StandardFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<StandardField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
