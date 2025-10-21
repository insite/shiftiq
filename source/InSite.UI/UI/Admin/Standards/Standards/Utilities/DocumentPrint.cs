using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using HtmlToOpenXml;

using InSite.Common;
using InSite.Persistence;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Standards.Standards.Utilities
{
    public static class DocumentPrint
    {
        public class CompetencyModel
        {
            public Guid Key { get; private set; }
            public int Sequence { get; private set; }
            public Shift.Common.ContentContainer Content { get; private set; }

            public CompetencyModel(StandardContainment containment)
            {
                Key = containment.Child.StandardIdentifier;
                Sequence = containment.ChildSequence;
                Content = ServiceLocator.ContentSearch.GetBlock(containment.Child.StandardIdentifier);
            }
        }

        public class RenderContext
        {
            public const double DefaultIndent = 0.15d;

            public WordprocessingDocument Document { get; }
            public Body Body => Document.MainDocumentPart.Document.Body;
            public HtmlConverter HtmlConverter { get; }
            public double Indent { get; }

            public string Language { get; set; }
            public bool ShowFieldHeading { get; set; }
            public ICollection<string> Fields { get; set; }
            public ICollection<string> Settings { get; set; }

            public int Level { get; set; }
            public int CompetencyNumberingID { get; set; }
            public int TocReferenceId { get; set; }

            public RenderContext(WordprocessingDocument doc, double indent = DefaultIndent)
            {
                Document = doc;
                HtmlConverter = new HtmlConverter(doc.MainDocumentPart);
                Indent = indent;
            }
        }

        public class StandardModel
        {
            #region Properties

            public Guid StandardIdentifier { get; set; }
            public Guid? ParentStandardIdentifier { get; set; }
            public string StandardType { get; set; }
            public int Sequence { get; set; }
            public string Tags { get; internal set; }
            public int? Complexity { get; internal set; }
            public int? Criticality { get; internal set; }
            public string Frequency { get; internal set; }
            public string Recurrence { get; internal set; }
            public int? Difficulty { get; internal set; }
            public string Language { get; private set; }
            public Func<string, string> AutoTranslate { get; private set; }

            public string Title => GetContent().Title.Text[Language];

            public StandardModel Parent { get; set; }
            public List<StandardModel> Children { get; } = new List<StandardModel>();

            #endregion

            #region Fields

            private Shift.Common.ContentContainer _content = null;

            #endregion

            #region Static

            public static readonly Func<Standard, StandardModel> BinderFunc;
            public static readonly Expression<Func<Standard, StandardModel>> BinderExpr;

            static StandardModel()
            {
                BinderExpr = LinqExtensions1.Expr((Standard x) => new StandardModel
                {
                    StandardIdentifier = x.StandardIdentifier,
                    ParentStandardIdentifier = x.ParentStandardIdentifier,
                    StandardType = x.StandardType,
                    Sequence = x.Sequence,
                    Tags = x.Tags
                    /*
                    Complexity = x.Complexity,
                    Criticality = x.Criticality,
                    Frequency = x.Frequency,
                    Recurrence = x.Recurrence,
                    Difficulty = x.Difficulty
                    */
                });
                BinderFunc = BinderExpr.Compile();
            }

            #endregion

            #region Methods (data binding)

            public static StandardModel[] LoadTree(Guid standardKey, string lang, Func<string, string> autoTranslate)
            {
                var data = StandardContainmentSearch.Bind(
                    LinqExtensions1.Expr((StandardContainment x) => BinderExpr.Invoke(x.Child)).Expand(),
                    x => x.ParentStandardIdentifier == standardKey
                      && x.Child.StandardType == Shift.Constant.StandardType.Competency);

                var accumulator = new Dictionary<Guid, StandardModel>();
                foreach (var info in data)
                    accumulator.Add(info.StandardIdentifier, info);

                LoadCompetencies(accumulator, data);

                var mapping = accumulator.Values
                    .Where(x => x.ParentStandardIdentifier.HasValue)
                    .GroupBy(x => x.ParentStandardIdentifier.Value)
                    .ToDictionary(x => x.Key, x => x.AsQueryable().OrderBy(y => y.Sequence));

                IEnumerable<StandardModel> topLevel = accumulator.Values
                    .Where(x => x.StandardType == Shift.Constant.StandardType.Framework)
                    .ToArray();
                IEnumerable<StandardModel> prevLevel = topLevel;

                while (true)
                {
                    var level = new List<StandardModel>();

                    foreach (var pModel in prevLevel)
                    {
                        pModel.Language = lang;
                        pModel.AutoTranslate = autoTranslate;

                        if (!mapping.TryGetValue(pModel.StandardIdentifier, out var children))
                            continue;

                        foreach (var cModel in children)
                        {
                            var isCompetency = cModel.StandardType == Shift.Constant.StandardType.Competency;
                            var isArea = cModel.StandardType == Shift.Constant.StandardType.Area;
                            if (!isCompetency && !isArea || !isCompetency && pModel.StandardType == Shift.Constant.StandardType.Competency)
                                continue;

                            cModel.Parent = pModel;
                            pModel.Children.Add(cModel);
                            level.Add(cModel);
                        }
                    }

                    if (level.Count == 0)
                        break;

                    prevLevel = level;
                }

                return topLevel.OrderBy(x => x.Title).ToArray();
            }

            private static void LoadCompetencies(Dictionary<Guid, StandardModel> accumulator, IEnumerable<StandardModel> children)
            {
                var typeFilter = new[]
                {
                    Shift.Constant.StandardType.Framework,
                    Shift.Constant.StandardType.Area,
                    Shift.Constant.StandardType.Competency,
                };
                var keyFilter = children
                    .Where(x => x.ParentStandardIdentifier.HasValue && !accumulator.ContainsKey(x.ParentStandardIdentifier.Value))
                    .Select(x => x.ParentStandardIdentifier.Value)
                    .Distinct()
                    .ToArray();

                if (keyFilter.Length == 0)
                    return;

                var data = StandardSearch.Bind(
                    LinqExtensions1.Expr((Standard x) => BinderExpr.Invoke(x)).Expand(),
                    x => keyFilter.Contains(x.StandardIdentifier) && typeFilter.Contains(x.StandardType));

                if (data.Length == 0)
                    return;

                foreach (var info in data)
                    accumulator.Add(info.StandardIdentifier, info);

                LoadCompetencies(accumulator, data);
            }

            #endregion

            #region Methods (render)

            private Shift.Common.ContentContainer GetContent() =>
                _content ?? (_content = ServiceLocator.ContentSearch.GetBlock(StandardIdentifier));

            public void RenderFields(RenderContext context)
            {
                var content = GetContent();

                var fields = CurrentSessionState.Identity.Organization.GetStandardContentLabels();
                foreach (var field in fields)
                {
                    if (!context.Fields.Contains(field))
                        continue;

                    var html = GetContentHtml(field);
                    if (string.IsNullOrEmpty(html))
                        continue;

                    AddFieldHtml(
                        context,
                        LabelHelper.GetTranslation(field, Language),
                        html);
                }

                string GetContentHtml(string name)
                {
                    var item = content[name];

                    var html = item.Html[Language];
                    if (string.IsNullOrEmpty(html))
                        html = Markdown.ToHtml(item.Text[Language]);

                    return html;
                }
            }

            public void RenderSettings(RenderContext context)
            {
                var heading = context.ShowFieldHeading
                    ? RenderSubheading(context, LabelHelper.GetTranslation("Standards.Documents.Headings.Settings.Title", Language))
                    : null;
                var itemsBefore = context.Body.ChildElements.Count;

                if (context.Settings.Contains("Tags"))
                {
                    var tags = !string.IsNullOrEmpty(Tags)
                        ? JsonConvert.DeserializeObject<Tuple<string, string[]>[]>(Tags)
                        : null;

                    if (tags.IsNotEmpty())
                    {
                        var html = new StringBuilder();

                        for (var x = 0; x < tags.Length; x++)
                        {
                            var category = tags[x];
                            var collectionName = AutoTranslate != null ? AutoTranslate(category.Item1) : LabelHelper.GetTranslation(category.Item1, Language);

                            html.Append("<i>").Append(HttpUtility.HtmlEncode(collectionName)).Append("</i>: ");

                            for (var y = 0; y < category.Item2.Length; y++)
                            {
                                var tagName = AutoTranslate != null ? AutoTranslate(category.Item2[y]) : LabelHelper.GetTranslation(category.Item2[y], Language);

                                html.Append("<span>").Append(HttpUtility.HtmlEncode(tagName)).Append("</span>");

                                if (y < category.Item2.Length - 1)
                                    html.Append(", ");
                            }

                            if (x < tags.Length - 1)
                                html.Append("; ");
                        }

                        AddFieldHtml(
                            context,
                            LabelHelper.GetTranslation("Standards.Documents.Headings.Tags.Title", Language),
                            html.ToString());
                    }
                }

                if (context.Settings.Contains("Complexity") && Complexity.HasValue)
                {
                    var value = (AssetComplexity)Complexity.Value;
                    AddFieldText(
                        context,
                        LabelHelper.GetTranslation("Standards.Documents.Headings.Complexity.Title", Language),
                        value.GetDescription());
                }

                if (context.Settings.Contains("Criticality") && Criticality.HasValue)
                {
                    var value = (AssetCriticality)Criticality.Value;
                    AddFieldText(
                        context,
                        LabelHelper.GetTranslation("Standards.Documents.Headings.Criticality.Title", Language),
                        value.GetDescription());
                }

                if (context.Settings.Contains("Frequency") && !string.IsNullOrEmpty(Frequency))
                    AddFieldText(
                        context,
                        LabelHelper.GetTranslation("Standards.Documents.Headings.Frequency.Title", Language),
                        Frequency);

                if (context.Settings.Contains("Recurrence") && !string.IsNullOrEmpty(Recurrence))
                    AddFieldText(
                        context,
                        LabelHelper.GetTranslation("Standards.Documents.Headings.Recurrence.Title", Language),
                        Recurrence);

                if (context.Settings.Contains("Difficulty") && Difficulty.HasValue)
                {
                    var value = (AssetDifficulty)Difficulty.Value;
                    AddFieldText(
                        context,
                        LabelHelper.GetTranslation("Standards.Documents.Headings.Difficulty.Title", Language),
                        value.GetDescription());
                }

                if (heading != null && itemsBefore == context.Body.ChildElements.Count)
                    heading.Remove();
            }

            private static void RenderHeading(RenderContext context, string text)
            {
                var indent = GetOxmlIndent(context);

                context.Body.AppendChild(new Paragraph(
                    new ParagraphProperties
                    {
                        Indentation = new Indentation { Left = indent.ToString() },
                        SpacingBetweenLines = new SpacingBetweenLines
                        {
                            After = "0"
                        }
                    },
                    new Run(
                        new RunProperties { Bold = new Bold { Val = new OnOffValue(true) } },
                        new Text { Text = text }
                    ),
                    new Run(
                        new Text { Text = ": ", Space = SpaceProcessingModeValues.Preserve }
                    )
                ));
            }

            private static OpenXmlElement RenderSubheading(RenderContext context, string title)
            {
                var indent = OxmlHelper.InchesToDxa<int>(/*context.Indent * context.Level +*/ 0.1);
                var paragraph = new Paragraph(
                    new ParagraphProperties
                    {
                        Indentation = new Indentation { Left = indent.ToString() },
                    },
                    new Run(
                        new RunProperties
                        {
                            Bold = new Bold { Val = new OnOffValue(true) },
                            Caps = new Caps { Val = new OnOffValue(true) }
                        },
                        new Text { Text = title }
                    )
                );

                context.Body.AppendChild(paragraph);

                return paragraph;
            }

            private static void AddFieldHtml(RenderContext context, string label, string html)
            {
                if (context.ShowFieldHeading)
                    RenderHeading(context, label);

                var indent = GetOxmlIndent(context);

                foreach (var item in context.HtmlConverter.Parse(html))
                {
                    if (item is Paragraph p)
                    {
                        if (p.ParagraphProperties == null)
                            p.ParagraphProperties = new ParagraphProperties();

                        if (p.ParagraphProperties.Indentation == null)
                            p.ParagraphProperties.Indentation = new Indentation();

                        var pIndent = indent;

                        var indentLeft = p.ParagraphProperties.Indentation.Left;
                        if (indentLeft != null && indentLeft.HasValue && int.TryParse(indentLeft.Value, out int indentLeftValue))
                            pIndent += indentLeftValue;
                        else if (p.ParagraphProperties.ParagraphStyleId?.Val == "ListParagraph")
                            pIndent += 720;

                        p.ParagraphProperties.Indentation.Left = pIndent.ToString();
                    }

                    context.Body.AppendChild(item);
                }
            }

            private void AddFieldText(RenderContext context, string label, string text)
            {
                if (context.ShowFieldHeading)
                    RenderHeading(context, label);

                var indent = GetOxmlIndent(context);

                context.Body.AppendChild(new Paragraph(
                    new ParagraphProperties { Indentation = new Indentation { Left = indent.ToString() }, },
                    new Run(new Text { Text = text })
                ));
            }

            private static int GetOxmlIndent(RenderContext context) =>
                OxmlHelper.InchesToDxa<int>(/*context.Indent * context.Level +*/ 0.1);

            #endregion
        }

        public static class CompetencyStyle
        {
            public static readonly OxmlStyleInfo Label = new OxmlStyleInfo("CompetencyLabel", "Competency Tag");
            public static readonly OxmlStyleInfo Title = new OxmlStyleInfo("CompetencyTitle", "Competency Title");
            public static readonly OxmlStyleInfo Subtitle = new OxmlStyleInfo("CompetencySubtitle", "Asset Subtitle");
        }

        public static byte[] CreateDocument(Action<WordprocessingDocument> render)
        {
            using (var stream = new MemoryStream())
            {
                using (var word = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    {
                        word.AddMainDocumentPart();
                        var documentBody = new Body();
                        var wordDocument = new Document(documentBody);
                        wordDocument.Save(word.MainDocumentPart);
                    }

                    {
                        OxmlHelper.MicrosoftWordStyles.SetupDefault(word.MainDocumentPart);

                        OxmlTextStyleCollection.GetChain()
                            .BasedOn("Normal")

                            .Add(new OxmlTextStyle(CompetencyStyle.Title.Id, CompetencyStyle.Title.Name)
                            {
                                FontSize = "14pt"
                            })
                            .Add(new OxmlTextStyle(CompetencyStyle.Label.Id, CompetencyStyle.Label.Name)
                            {
                                Bold = true,
                                FontColor = "ffffff",
                                BackColor = "666666"
                            })

                            .AddToPackage(word);
                    }

                    {
                        var sectionProps = new SectionProperties(
                            new PageSize
                            {
                                Width = OxmlHelper.InchesToDxa<uint>(8.5),
                                Height = OxmlHelper.InchesToDxa<uint>(11),
                                Orient = PageOrientationValues.Portrait
                            },
                            new PageMargin
                            {
                                Left = OxmlHelper.InchesToDxa<uint>(1),
                                Right = OxmlHelper.InchesToDxa<uint>(1),
                                Top = OxmlHelper.InchesToDxa<int>(1),
                                Bottom = OxmlHelper.InchesToDxa<int>(1),
                                Header = OxmlHelper.InchesToDxa<uint>(0.25),
                                Footer = OxmlHelper.InchesToDxa<uint>(0.25),
                                Gutter = OxmlHelper.InchesToDxa<uint>(0)
                            });

                        word.MainDocumentPart.Document.Body.AppendChild(sectionProps);
                    }

                    render(word);

                    word.PackageProperties.Created = DateTimeOffset.Now.DateTime;
                    word.PackageProperties.Creator = CurrentSessionState.Identity.User.FullName;

                    word.Save();
                }

                return stream.ToArray();
            }
        }

        public static void RenderBulletedList(WordprocessingDocument word, CompetencyModel[] competencies, string lang)
        {
            int numberingId;

            {
                var newAbstractNum = OxmlHelper.InsertAbstractNum(
                    word.MainDocumentPart,
                    new AbstractNum($@"<w:abstractNum w15:restartNumberingAfterBreak=""0"" xmlns:w15=""http://schemas.microsoft.com/office/word/2012/wordml"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"">
    <w:nsid w:val=""61252301""/>
    <w:multiLevelType w:val=""hybridMultilevel""/>
    <w:tmpl w:val=""0C58118C""/>
    <w:lvl w:ilvl=""0"" w:tplc=""04090001"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""·""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""720"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Symbol"" w:hAnsi=""Symbol""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""1"" w:tplc=""04090003"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""o""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1440"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Courier New"" w:hAnsi=""Courier New"" w:cs=""Courier New""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""2"" w:tplc=""04090005"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""§""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""2160"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Wingdings"" w:hAnsi=""Wingdings""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""3"" w:tplc=""04090001"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""·""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""2880"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Symbol"" w:hAnsi=""Symbol""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""4"" w:tplc=""04090003"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""o""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""3600"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Courier New"" w:hAnsi=""Courier New"" w:cs=""Courier New""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""5"" w:tplc=""04090005"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""§""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""4320"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Wingdings"" w:hAnsi=""Wingdings""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""6"" w:tplc=""04090001"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""·""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""5040"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Symbol"" w:hAnsi=""Symbol""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""7"" w:tplc=""04090003"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""o""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""5760"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Courier New"" w:hAnsi=""Courier New"" w:cs=""Courier New""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""8"" w:tplc=""04090005"" w:tentative=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""bullet""/>
        <w:lvlText w:val=""§""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""6480"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default"" w:ascii=""Wingdings"" w:hAnsi=""Wingdings""/>
        </w:rPr>
    </w:lvl>
</w:abstractNum>"));

                var newNumInstance = OxmlHelper.InsertNumInstance(
                    word.MainDocumentPart,
                    new NumberingInstance(new AbstractNumId { Val = newAbstractNum.AbstractNumberId }));

                numberingId = newNumInstance.NumberID;
            }

            foreach (var competency in competencies)
            {
                word.MainDocumentPart.Document.Body.Append(
                    new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId { Val = "ListParagraph" },
                            new NumberingProperties(
                                new NumberingLevelReference { Val = 0 },
                                new NumberingId { Val = numberingId }),
                            new Justification { Val = JustificationValues.Start }
                        ),
                        new Run(
                            new RunProperties(),
                            new Text(competency.Content.Title.Text[lang]))
                    ));
            }
        }

        public static void CreateTocContainer(WordprocessingDocument word, string header)
        {
            var body = word.MainDocumentPart.Document.Body;
            var tocContentBlock = new SdtContentBlock(
                new Paragraph(
                    new ParagraphProperties(new ParagraphStyleId { Val = "TOCHeading" }),
                    new Run(new Text { Text = header })
                )
            );

            var firstParagraph = (OpenXmlElement)body.Elements<Paragraph>().FirstOrDefault();

            InsertElement(new SdtBlock(
                new SdtProperties(
                    new SdtContentDocPartObject(
                        new DocPartGallery { Val = header },
                        new DocPartUnique()
                    )
                ),
                new SdtEndCharProperties(
                    new RunProperties(
                        new RunFonts
                        {
                            AsciiTheme = ThemeFontValues.MinorHighAnsi,
                            HighAnsiTheme = ThemeFontValues.MinorHighAnsi,
                            EastAsiaTheme = ThemeFontValues.MinorHighAnsi,
                            ComplexScriptTheme = ThemeFontValues.MinorBidi
                        },
                        new Color { Val = "auto" },
                        new FontSize { Val = "22" },
                        new FontSizeComplexScript { Val = "22" }
                    )
                ),
                tocContentBlock
            ));

            InsertElement(new Paragraph(
                new Run(
                    new Break { Type = BreakValues.Page }
                )
            ));

            void InsertElement(OpenXmlElement el)
            {
                if (firstParagraph == null)
                    body.Append(el);
                else
                    body.InsertBefore(el, firstParagraph);
            }
        }

        public static void AddTocItem(RenderContext context, Paragraph paragraph, string text)
        {
            var tocRefId = context.TocReferenceId++;
            var anchorName = $"__TocRef{tocRefId:0000}";
            var bookmarkId = tocRefId.ToString();

            var bookmarkElements = new OpenXmlElement[]
            {
                new BookmarkStart { Id = bookmarkId, Name = anchorName },
                new BookmarkEnd { Id = bookmarkId }
            };

            var pProps = paragraph.GetFirstChild<ParagraphProperties>();
            if (pProps != null)
            {
                for (var i = bookmarkElements.Length - 1; i >= 0; i--)
                    paragraph.InsertAfter(bookmarkElements[i], pProps);
            }
            else
            {
                var run = paragraph.GetFirstChild<Run>();
                foreach (var el in bookmarkElements)
                    paragraph.InsertBefore(el, run);
            }

            var tocContentBlock = context.Body.GetFirstChild<SdtBlock>().GetFirstChild<SdtContentBlock>();

            tocContentBlock.Append(new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId { Val = $"TOC{context.Level + 1}" },
                    new Indentation() { Left = OxmlHelper.InchesToDxa<int>(0.15 * context.Level).ToString() }
                ),
                new Hyperlink(
                    new Run(
                        new RunProperties(
                            //new RunStyle { Val = "Hyperlink" },
                            new Bold { Val = context.Level == 0 },
                            new BoldComplexScript { Val = context.Level == 0 }
                        ),
                        new Text { Text = text }
                    )
                )
                { History = true, Anchor = anchorName }/*,
                new Run(
                    new PositionalTab
                    { 
                        Alignment = AbsolutePositionTabAlignmentValues.Right, 
                        RelativeTo = AbsolutePositionTabPositioningBaseValues.Margin, 
                        Leader = AbsolutePositionTabLeaderCharValues.Dot
                    }
                ),
                new Run(
                    new RunProperties(new Bold(), new BoldComplexScript()), 
                    new Text { Text = "?" }
                )*/
            ));
        }

        public static Paragraph AddHeading1(WordprocessingDocument word, string text)
        {
            var paragraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading1.Id },
                    new ParagraphMarkRunProperties()
                ),
                new Run(new Text { Text = text })
            );

            word.MainDocumentPart.Document.Body.Append(paragraph);

            return paragraph;
        }

        public static Paragraph AddFieldHeader(RenderContext context, string title)
        {
            var paragraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId
                    {
                        Val = $"Heading{context.Level + 1}"
                    }/*,
                    new Indentation
                    {
                        Left = OxmlHelper.InchesToDxa<int>(RenderContext.DefaultIndent * context.Level).ToString()
                    }*/)
                {
                    SpacingBetweenLines = new SpacingBetweenLines
                    {
                        After = OxmlHelper.InchesToDxa<int>(0.1).ToString()
                    },
                    ContextualSpacing = new ContextualSpacing { Val = false }
                },
                new Run(new Text(title))
            );

            context.Body.AppendChild(paragraph);

            return paragraph;
        }

        public static Paragraph AddOrderedListHeader(RenderContext context, string title)
        {
            var paragraph = new Paragraph(
                new ParagraphProperties(
                    new ParagraphStyleId { Val = "ListParagraph" },
                    new NumberingProperties(
                        new NumberingLevelReference { Val = context.Level },
                        new NumberingId { Val = context.CompetencyNumberingID }),
                    new RunProperties(
                        new RunStyle { Val = $"Heading{context.Level + 1}Char" }),
                    new Tabs(
                        new TabStop { Val = TabStopValues.Left, Leader = TabStopLeaderCharValues.None, Position = OxmlHelper.InchesToDxa<int>(0.2) }))
                {
                    SpacingBetweenLines = new SpacingBetweenLines
                    {
                        After = OxmlHelper.InchesToDxa<int>(0.1).ToString()
                    },
                    ContextualSpacing = new ContextualSpacing { Val = false }
                },
                new Run(
                    new RunProperties(
                        new RunStyle { Val = $"Heading{context.Level + 1}Char" }),
                    new Text(title))
            );

            context.Body.AppendChild(paragraph);

            return paragraph;
        }

        public static int DefineOrderedList(RenderContext context)
        {
            var newAbstractNum = OxmlHelper.InsertAbstractNum(
                context.Document.MainDocumentPart,
                new AbstractNum($@"<w:abstractNum w15:restartNumberingAfterBreak=""0"" xmlns:w15=""http://schemas.microsoft.com/office/word/2012/wordml"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"">
    <w:multiLevelType w:val=""multilevel""/>
    <w:lvl w:ilvl=""0"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""upperLetter""/>
        <w:lvlText w:val=""%1.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""216"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlRestart w:val=""0"" />
        <w:lvlText w:val=""%2.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""432"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""2"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""648"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""3"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""864"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""4"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1080"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""5"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1296"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""6"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1512"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""7"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.%8.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1728"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""8"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.%8.%9.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""1944"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
</w:abstractNum>"));
            var newNumInstance = OxmlHelper.InsertNumInstance(
                context.Document.MainDocumentPart,
                new NumberingInstance(new AbstractNumId { Val = newAbstractNum.AbstractNumberId }));

            return newNumInstance.NumberID;
        }

        public static int DefineStandardsDocumentOrderedList(RenderContext context)
        {
            var newAbstractNum = OxmlHelper.InsertAbstractNum(
                context.Document.MainDocumentPart,
                new AbstractNum($@"<w:abstractNum w15:restartNumberingAfterBreak=""0"" xmlns:w15=""http://schemas.microsoft.com/office/word/2012/wordml"" xmlns:w=""http://schemas.openxmlformats.org/wordprocessingml/2006/main"">
    <w:multiLevelType w:val=""multilevel""/>
    <w:lvl w:ilvl=""0"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""upperLetter""/>
        <w:lvlText w:val=""%1.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""1"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlRestart w:val=""0"" />
        <w:lvlText w:val=""%2.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""2"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""3"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""4"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""5"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""6"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""7"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.%8.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
    <w:lvl w:ilvl=""8"">
        <w:start w:val=""1""/>
        <w:numFmt w:val=""decimal""/>
        <w:lvlText w:val=""%2.%3.%4.%5.%6.%7.%8.%9.""/>
        <w:lvlJc w:val=""left""/>
        <w:pPr>
            <w:ind w:left=""0"" w:hanging=""360""/>
        </w:pPr>
        <w:rPr>
            <w:rFonts w:hint=""default""/>
        </w:rPr>
    </w:lvl>
</w:abstractNum>"));
            var newNumInstance = OxmlHelper.InsertNumInstance(
                context.Document.MainDocumentPart,
                new NumberingInstance(new AbstractNumId { Val = newAbstractNum.AbstractNumberId }));

            return newNumInstance.NumberID;
        }

        #region Methods (footer)

        private static Paragraph EnsureFooterExists(WordprocessingDocument word)
        {
            var footerPart = word.MainDocumentPart.FooterParts.FirstOrDefault();
            if (footerPart == null)
            {
                footerPart = word.MainDocumentPart.AddNewPart<FooterPart>();
                footerPart.Footer = new Footer();
                OxmlHelper.SetupHeaderFooter(footerPart.Footer);
                var sectionProps = word.MainDocumentPart.Document.Body.GetFirstChild<SectionProperties>();
                sectionProps.AppendChild(new FooterReference
                {
                    Id = word.MainDocumentPart.GetIdOfPart(footerPart),
                    Type = HeaderFooterValues.Default
                });
            }

            var paragraph = footerPart.Footer.GetFirstChild<Paragraph>();

            if (paragraph == null)
            {
                paragraph = new Paragraph(
                    new ParagraphProperties(
                        new ParagraphStyleId { Val = "Footer" },
                        new Justification { Val = JustificationValues.Left },
                        new ParagraphMarkRunProperties()
                    )
                );

                footerPart.Footer.Append(paragraph);
            }

            return paragraph;
        }

        public static void RenderFooterText(WordprocessingDocument word, string text)
        {
            if (string.IsNullOrEmpty(text))
                return;

            var paragraph = EnsureFooterExists(word);

            paragraph.Append(
                new Run(
                    new Text { Space = SpaceProcessingModeValues.Preserve, Text = text }
                )
            );
        }

        public static void RenderFooterNumber(WordprocessingDocument word)
        {
            var paragraph = EnsureFooterExists(word);

            paragraph.Append(
                new Run(
                    new TabChar(),
                    new TabChar()
                ),
                new Run(new FieldChar { FieldCharType = FieldCharValues.Begin }),
                new Run(new FieldCode { Text = " PAGE " }),
                new Run(new FieldChar { FieldCharType = FieldCharValues.Separate }),
                new Run(new Text { Text = "1" }),
                new Run(new FieldChar { FieldCharType = FieldCharValues.End })
            );
        }

        #endregion
    }
}