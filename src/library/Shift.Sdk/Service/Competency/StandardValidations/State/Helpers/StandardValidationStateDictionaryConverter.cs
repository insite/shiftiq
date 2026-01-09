using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Standards
{
    public class StandardValidationStateDictionaryConverter : StateDictionaryConverter<StandardValidationField>
    {
        private static readonly IReadOnlyDictionary<StandardValidationField, Func<JsonReader, object>> _mapping;

        static StandardValidationStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(StandardValidationFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<StandardValidationField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
