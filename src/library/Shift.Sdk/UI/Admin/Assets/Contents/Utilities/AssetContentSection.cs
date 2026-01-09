using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

using Shift.Constant;

namespace Shift.Sdk.UI
{
    public abstract class AssetContentSection
    {
        #region Properties

        public string Id { get; }
        public string Title { get; set; }
        public abstract string ControlPath { get; }

        #endregion

        #region Construction

        public AssetContentSection(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            Id = id;
        }

        #endregion

        #region Definitions

        public abstract class TextSection : AssetContentSection
        {
            public string Label { get; set; }
            public string Description { get; set; }
            public bool IsRequired { get; set; }
            public MultilingualString Value { get; set; }

            public TextSection(string id)
                : base(id)
            {

            }
        }

        public abstract class ContentSectionViewer : AssetContentSection
        {
            public string EditUrl { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public bool IsRequired { get; set; }
            public MultilingualString Value { get; set; }

            public ContentSectionViewer(string id)
                : base(id)
            {

            }
        }

        public sealed class SingleLineList : AssetContentSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionSingleList.ascx";

            public string Label { get; set; }
            public string Description { get; set; }
            public string EntityName { get; set; }
            public bool IsRequired { get; set; }

            public IEnumerable<MultilingualString> Values { get; set; }

            public SingleLineList(string id)
                : base(id)
            {

            }
        }

        public sealed class TabList : AssetContentSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionTabs.ascx";

            public IEnumerable<TextSection> Tabs { get; set; }

            public TabList(string id)
                : base(id)
            {

            }
        }

        public sealed class SingleLine : TextSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionDefault.ascx";

            public SingleLine(string id)
                : base(id)
            {

            }
        }

        public sealed class Markdown : TextSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionDefault.ascx";

            public bool AllowUpload { get; set; }
            public string UploadFolderPath { get; set; }
            public UploadMode UploadStrategy { get; set; }

            public Markdown(string id)
                : base(id)
            {

            }
        }

        public sealed class Html : TextSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionDefault.ascx";

            public bool AllowUpload { get; set; }
            public string UploadFolderPath { get; set; }
            public UploadMode UploadStrategy { get; set; }

            public Html(string id)
                : base(id)
            {

            }
        }

        public sealed class TileList : AssetContentSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionTiles.ascx";

            public TileSize Size { get; set; }

            public IEnumerable<AssetContentSection> Options { get; set; }

            public TileList(string id)
                : base(id)
            {
                Size = TileSize.Half;
            }
        }

        public sealed class MarkdownAndHtml : AssetContentSection
        {
            public override string ControlPath => "~/UI/Admin/Assets/Contents/Controls/ContentEditor/SectionMdHtml.ascx";

            public string MarkdownLabel { get; set; }
            public string MarkdownDescription { get; set; }
            public MultilingualString MarkdownValue { get; set; }

            public string HtmlLabel { get; set; }
            public string HtmlDescription { get; set; }
            public MultilingualString HtmlValue { get; set; }

            public bool IsMultiValue { get; set; }

            public bool AllowUpload { get; set; }
            public string UploadFolderPath { get; set; }
            public UploadMode UploadStrategy { get; set; }
            public bool IsRequired { get; set; }
            public bool EnableHtmlBuilder { get; set; }

            public MarkdownAndHtml(string id)
                : base(id)
            {

            }
        }

        #endregion

        #region Factory

        public static AssetContentSection Create(ContentSectionDefault id, MultilingualDictionary content, bool? isRequired = null)
        {
            MultilingualString[] values;

            switch (id)
            {
                case ContentSectionDefault.Title:
                    values = new[] { content.Get("Title") };
                    break;
                case ContentSectionDefault.Summary:
                    values = new[] { content.Get("Summary") };
                    break;
                case ContentSectionDefault.Description:
                    values = new[] { content.Get("Description") };
                    break;
                case ContentSectionDefault.Rationale:
                    values = new[] { content.Get("Rationale") };
                    break;
                case ContentSectionDefault.Feature:
                    values = new[] { content.Get("Feature") };
                    break;
                case ContentSectionDefault.Instructions:
                    values = new[] { content.Get("InstructionsForOnline"), content.Get("InstructionsForPaper") };
                    break;
                case ContentSectionDefault.InstructionsForOnline:
                    values = new[] { content.Get("InstructionsForOnline") };
                    break;
                case ContentSectionDefault.InstructionsForPaper:
                    values = new[] { content.Get("InstructionsForPaper") };
                    break;
                case ContentSectionDefault.Introduction:
                    values = new[] { content.Get("Introduction") };
                    break;
                case ContentSectionDefault.Body:
                    values = new[] { content.Get("BodyText"), content.Get("BodyHtml") };
                    break;
                case ContentSectionDefault.BodyText:
                    values = new[] { content.Get("BodyText") };
                    break;
                case ContentSectionDefault.BodyHtml:
                    values = new[] { content.Get("BodyHtml") };
                    break;
                case ContentSectionDefault.Conclusion:
                    values = new[] { content.Get("Conclusion") };
                    break;
                case ContentSectionDefault.Statements:
                    values = new[] { content.Get("Statements") };
                    break;
                case ContentSectionDefault.Knowledge:
                    values = new[] { content.Get("Knowledge") };
                    break;
                case ContentSectionDefault.Skills:
                    values = new[] { content.Get("Skills") };
                    break;
                case ContentSectionDefault.Materials:
                    values = new[] { content.Get("MaterialsForDistribution"), content.Get("MaterialsForParticipation") };
                    break;
                case ContentSectionDefault.MaterialsForDistribution:
                    values = new[] { content.Get("MaterialsForDistribution") };
                    break;
                case ContentSectionDefault.MaterialsForParticipation:
                    values = new[] { content.Get("MaterialsForParticipation") };
                    break;
                case ContentSectionDefault.Unavailable:
                    values = new[] { content.Get("Unavailable") };
                    break;
                case ContentSectionDefault.ClassLink:
                    values = new[] { content.Get("ClassLink") };
                    break;
                default:
                    throw new NotImplementedException("Identifier: " + id.GetName());
            }

            var AssetContentSection = Create(id, values);

            if (isRequired.HasValue && AssetContentSection is SingleLine singleLine)
                singleLine.IsRequired = isRequired.Value;

            return AssetContentSection;
        }

        public static AssetContentSection Create(ContentSectionDefault id, params MultilingualString[] values)
        {
            switch (id)
            {
                case ContentSectionDefault.Title:
                    return new SingleLine(id.GetName())
                    {
                        Title = "Title",
                        Label = "Title",
                        // Description = "The user-friendly name or heading.",
                        IsRequired = true,
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Summary:
                    return new Markdown(id.GetName())
                    {
                        Title = "Summary",
                        Label = "Summary",
                        // Description = "A synopsis, abstract, or executive summary of the content.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Description:
                    return new Markdown(id.GetName())
                    {
                        Title = "Description",
                        Label = "Description",
                        // Description = "A detailed description of the content, normally for internal administration of the content.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Rationale:
                    return new Markdown(id.GetName())
                    {
                        Title = "Rationale",
                        Label = "Rationale",
                        // Description = "What is the purpose of this content?",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Feature:
                    return new Markdown(id.GetName())
                    {
                        Title = "Feature",
                        Label = "Feature",
                        // Description = "What are the unique, essential elements (alone or in combination) that form the critical steps in successfully answering the question asked, or solving the problem posed?",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Instructions:
                    return new TileList(id.GetName())
                    {
                        Title = "Instructions",
                        Options = new[]
                        {
                            (Markdown)Create(ContentSectionDefault.InstructionsForOnline, values[0]),
                            (Markdown)Create(ContentSectionDefault.InstructionsForPaper, values[1]),
                        }
                    };
                case ContentSectionDefault.InstructionsForOnline:
                    return new Markdown(id.GetName())
                    {
                        Title = "Instructions",
                        Label = "Instructions for Online",
                        // Description = "The instructions given to a learner who is writing the exam online.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.InstructionsForPaper:
                    return new Markdown(id.GetName())
                    {
                        Title = "Instructions",
                        Label = "Instructions for Paper",
                        // Description = "The instructions given to a learner who is writing the exam on paper.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Introduction:
                    return new Markdown(id.GetName())
                    {
                        Title = "Introduction",
                        Label = "Introduction",
                        // Description = "Opening instructions for a survey form or an exam form.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Body:
                    return new MarkdownAndHtml(id.GetName())
                    {
                        Title = "Body",

                        MarkdownLabel = "Body Text (Markdown)",
                        // MarkdownDescription = "The main body of the content for a web page or an email message in Markdown format.",
                        MarkdownValue = values != null && values.Length >= 1 ? values[0] : null,

                        HtmlLabel = "Body HTML",
                        // HtmlDescription = "The main body of the content for a web page or an email message in HTML format.",
                        HtmlValue = values != null && values.Length >= 2 ? values[1] : null,
                    };
                case ContentSectionDefault.BodyText:
                    return new Markdown(id.GetName())
                    {
                        Title = "Body",
                        Label = "Body Text",
                        // Description = "The main body of the content for a web page or an email message in Markdown format.",
                        Value = values[0]
                    };
                case ContentSectionDefault.BodyHtml:
                    return new Html(id.GetName())
                    {
                        Title = "Body",
                        Label = "Body HTML",
                        // Description = "The main body of the content for a web page or an email message in HTML format.",
                        Value = values[0]
                    };
                case ContentSectionDefault.Conclusion:
                    return new Markdown(id.GetName())
                    {
                        Title = "Conclusion",
                        Label = "Conclusion",
                        // Description = "Closing instructions for a survey form or an exam form.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Statements:
                    return new Markdown(id.GetName())
                    {
                        Title = "Statements",
                        Label = "Statements",
                        // Description = "This is an ordered list of statements that provide additional description or definition.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Knowledge:
                    return new Markdown(id.GetName())
                    {
                        Title = "Knowledge",
                        Label = "Knowledge",
                        // Description = "What knowledge is expected?",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Skills:
                    return new Markdown(id.GetName())
                    {
                        Title = "Skills",
                        Label = "Skill",
                        // Description = "What skills should be demonstrated?",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Materials:
                    return new TileList(id.GetName())
                    {
                        Title = "Materials",
                        Options = new[]
                        {
                            (Markdown)Create(ContentSectionDefault.MaterialsForDistribution, values[0]),
                            (Markdown)Create(ContentSectionDefault.MaterialsForParticipation, values[1]),
                        }
                    };
                case ContentSectionDefault.MaterialsForDistribution:
                    return new Markdown(id.GetName())
                    {
                        Title = "Materials",
                        Label = "Materials for Distribution",
                        // Description = "The physical materials required in a distribution package.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.MaterialsForParticipation:
                    return new Markdown(id.GetName())
                    {
                        Title = "Materials",
                        Label = "Materials for Participation",
                        // Description = "The materials permitted to those who participate in a scheduled event.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.Unavailable:
                    return new Markdown(id.GetName())
                    {
                        Title = "Unavailable",
                        Label = "Unavailable",
                        // Description = "If this content is not available (e.g. a survey form has closed) then this is the text displayed to users.",
                        Value = values.FirstOrDefault()
                    };
                case ContentSectionDefault.ClassLink:
                    return new SingleLine(id.GetName())
                    {
                        Title = "Class Link",
                        Label = "Class Link",
                        Value = values.FirstOrDefault()
                    };
                default:
                    throw new NotImplementedException("Identifier: " + id.GetName());
            }
        }

        #endregion
    }
}
