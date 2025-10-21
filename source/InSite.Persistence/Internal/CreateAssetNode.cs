using System;
using System.Collections.Specialized;

namespace InSite.Persistence
{
    [Serializable]
    public class CreateAssetNode
    {
        public int? ParentId { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public int Id { get; set; }
        public int Sequence { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public int? Number { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public string Reference { get; set; }
        public string Summary { get; set; }
        public string BodyHtml { get; set; }
        public string BodyText { get; set; }
        public StringCollection Statements { get; set; }

        public CreateAssetNode()
        {
            Statements = new StringCollection();
        }

        public override string ToString()
        {
            return $"{Sequence}. {Title}";
        }
    }
}
