using System;

using InSite.Domain.Records;

namespace InSite.Api.Models.Records
{
    public class Template
    {
        public Guid Identifier { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public string Description { get; set; }
        public Expiration Expiration { get; set; }
    }
}