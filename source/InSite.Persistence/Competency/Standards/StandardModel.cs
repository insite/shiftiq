using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardModel
    {
        #region Properties (serialized)

        public Guid StandardIdentifier { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), MaxLength(512)]
        public string Code { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), MaxLength(9)]
        public string Hook { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<StandardContentModel> Content { get; set; } = new List<StandardContentModel>();

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), MaxLength(32)]
        public string Icon { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsHidden { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsPractical { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsPublished { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsShared { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public bool IsTheory { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), MaxLength(256)]
        public string Name { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string NavigateUrl { get; set; }

        public int AssetNumber { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Color { get; set; }

        public int Sequence { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore), MaxLength(256)]
        public string Source { get; set; }

        [MaxLength(64)]
        public string StandardType { get; set; }

        public Guid OrganizationIdentifier { get; set; }

        public Guid Thumbprint { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? UtcPublished { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid ModifiedBy { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Modified { get; set; }

        // related objects

        public StandardModelList Children { get; set; }

        #endregion

        #region Properties (not serialized)

        [JsonIgnore]
        public string CodePath
        {
            get
            {
                if (Parent == null) return Code ?? string.Empty;
                var x = Parent.CodePath;
                return x + (x != string.Empty && Code != string.Empty ? "." : string.Empty) + Code;
            }
        }

        [JsonIgnore]
        public int Depth => Parent == null ? 0 : 1 + Parent.Depth;

        [JsonIgnore]
        public StandardModel Parent { get; set; }

        [JsonIgnore]
        public StandardModelList Parents { get; set; }

        [JsonIgnore]
        public string Path => (Parent != null ? Parent.Path + "/" : string.Empty) + (Name ?? AssetNumber.ToString());

        [JsonIgnore]
        public string Title
        {
            get
            {
                return Content.Where(x => x.Label == ContentLabel.Title && x.Language == Language.Default).FirstOrDefault()?.Text;
            }
            set
            {
                var title = Content.Where(x => x.Label == ContentLabel.Title && x.Language == Language.Default).FirstOrDefault();
                if (title == null)
                {
                    Content.Add(title = new StandardContentModel
                    {
                        Label = ContentLabel.Title,
                        Language = Language.Default
                    });
                }

                title.Text = value;
            }
        }

        [JsonIgnore]
        public string Summary => Content?.Where(x => x.Label == ContentLabel.Summary && x.Language == Language.Default).FirstOrDefault()?.Text;

        [JsonIgnore]
        public string QualifiedName
        {
            get
            {
                var name = $"{Sequence}. {StandardType} {AssetNumber} {Title}";
                var regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidPathChars());
                var r = new Regex($"[{Regex.Escape(regexSearch)}]");
                name = r.Replace(name, "");
                if (name.Length > 250)
                    name = name.Substring(0, 250);
                return name;
            }
        }

        [JsonIgnore]
        public string QualifiedPath { get; set; }

        [JsonIgnore]
        public string SequencePath { get; set; }

        #endregion

        #region Methods (construction)

        public StandardModel()
        {
            Children = new StandardModelList();
            Parents = new StandardModelList();
        }

        #endregion

        #region Methods

        public StandardModel FindByNumber(int number)
        {
            if (AssetNumber == number)
                return this;

            foreach (var child in Children)
            {
                var x = child.FindByNumber(number);
                if (x != null)
                    return x;
            }

            return null;
        }

        public StandardModel FindByThumbprint(Guid thumbprint)
        {
            if (Thumbprint == thumbprint)
                return this;

            foreach (var child in Children)
            {
                var x = child.FindByThumbprint(thumbprint);
                if (x != null)
                    return x;
            }

            return null;
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        private void ResolveParents(StreamingContext context)
        {
            ResolveParents(this);
        }

        private static void ResolveParents(StandardModel asset)
        {
            foreach (var child in asset.Children)
            {
                child.Parent = asset;
                ResolveParents(child);
            }
        }

        public bool ShouldSerializeChildren()
        {
            return Children.IsNotEmpty();
        }

        #endregion
    }
}
