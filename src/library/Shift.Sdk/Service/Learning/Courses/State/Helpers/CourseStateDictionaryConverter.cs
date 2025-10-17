using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace InSite.Domain.Courses
{
    internal class CourseStateDictionaryConverter : StateDictionaryConverter<CourseField>
    {
        private static readonly IReadOnlyDictionary<CourseField, Func<JsonReader, object>> _mapping;

        static CourseStateDictionaryConverter()
        {
            _mapping = CreateMappingDictionary(CourseFieldList.GetAllFields());
        }

        protected override IReadOnlyDictionary<CourseField, Func<JsonReader, object>> GetMappingDictionary() => _mapping;
    }
}
