using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// An attachment is an uploaded file with metadata.
    /// </summary>
    [Serializable]
    public class Attachment : IHasAssetNumber, IHasVersionControl<Attachment>
    {
        /// <summary>
        /// Uniquely identifies the attacment.
        /// </summary>
        public Guid Identifier { get; set; }

        /// <summary>
        /// Attachments are organized into types. Certain functions might apply to one type and not to another type.
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        public AttachmentType Type { get; set; }

        /// <summary>
        /// Who uploaded the file?
        /// </summary>
        public Guid Author { get; set; }

        /// <summary>
        /// What file was uploaded?
        /// </summary>
        public Guid Upload { get; set; }

        /// <summary>
        /// Every attachment is assigned a unique asset number in the organization's inventory.
        /// </summary>
        public int Asset { get; set; }

        /// <summary>
        /// Every asset number has a unique version number.
        /// </summary>
        public int AssetVersion => this.GetVersionNumber();

        /// <summary>
        /// Attachments need support for multilingual titles and descriptions.
        /// </summary>
        public ContentTitle Content { get; set; }

        /// <summary>
        /// Image attachments have additional extended properties that apply to images only.
        /// </summary>
        public AttachmentImage Image { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Condition { get; set; }

        [DefaultValue(PublicationStatus.Drafted)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.IgnoreAndPopulate)]
        public PublicationStatus PublicationStatus { get; set; }

        public DateTimeOffset Uploaded { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTimeOffset? FirstPublished { get; set; }

        [JsonIgnore]
        public HashSet<Guid> QuestionIdentifiers { get; set; }

        [JsonIgnore]
        public Attachment NextVersion { get; set; }

        public Attachment PreviousVersion { get; set; }

        /// <summary>
        /// Constructs an empty attachment.
        /// </summary>
        public Attachment()
        {
            Content = new ContentTitle();
            QuestionIdentifiers = new HashSet<Guid>();
        }

        public Attachment Clone()
        {
            var clone = new Attachment();

            this.ShallowCopyTo(clone);

            clone.Content = Content.Clone();
            clone.Image = Image?.Clone();
            clone.PreviousVersion = PreviousVersion?.Clone();
            clone.RestoreNextVersionReferences();

            return clone;
        }

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            this.RestoreNextVersionReferences();
        }

        #endregion

        #region Methods (helpers)

        public static AttachmentType GetAttachmentType(string extension)
        {
            if (FileExtension.IsImage(extension))
                return AttachmentType.Image;

            if (FileExtension.IsDocument(extension))
                return AttachmentType.Document;

            if (FileExtension.IsArchive(extension))
                return AttachmentType.Archive;

            return AttachmentType.Unknown;
        }

        #endregion
    }
}
