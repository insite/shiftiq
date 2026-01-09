using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    internal class ActivityStateDictionaryConverter : StateDictionaryConverter<ActivityField>
    {
        private static readonly IReadOnlyDictionary<ActivityField, Func<JsonReader, object>> _mapping;

        static ActivityStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(ActivityFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<ActivityField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
