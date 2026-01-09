using System;

namespace Shift.Sdk.UI
{
    internal class JsonUploadSuccess : IJsonUploadResult
    {
        public JsonUploadSuccess(Guid id, string name)
        {
            Type = "OK";
            Id = id;
            Name = name;
        }

        public Guid Id { get; }
        public string Name { get; }
        public string Type { get; }
    }
}