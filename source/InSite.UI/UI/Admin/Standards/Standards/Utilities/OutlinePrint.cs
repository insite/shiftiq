using System;
using System.Collections.Generic;
using System.IO;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using Shift.Common;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Standards.Standards.Utilities
{
    public static class OutlinePrint
    {
        #region Classes

        public class NodeItem
        {
            public string Label { get; set; }
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public StandardNodeType ItemType { get; set; }

            public List<NodeItem> Children { get; set; }
        }

        private class StyleIDs
        {
            public const string CaptionStyle = "CaptionStyle";
            public const string LabelProfileStyle = "LabelProfileStyle";
            public const string LabelFrameworkStyle = "LabelFrameworkStyle";
            public const string LabelAreaStyle = "LabelAreaStyle";
            public const string LabelCompetencyStyle = "LabelCompetencyStyle";
            public const string TitleStyle = "TitleStyle";
            public const string SubtitleStyle = "SubtitleStyle";
        }

        #endregion

        #region Constants

        private const double SpacingBetweenLines = 0.333333;
        private const double Indent = 0.173611;
        private const string Caption = "Standard Outline";

        #endregion

        #region Public methods

        public static void Print(List<NodeItem> nodes, Stream stream)
        {
            using (var word = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
            {
                word.AddMainDocumentPart();

                OxmlTextStyleCollection.GetChain()
                    .BasedOn("Normal").FontName("Arial").FontSize("8pt").FontColor("000000")

                    .Add(new OxmlTextStyle(StyleIDs.CaptionStyle)
                    {
                        Bold = true,
                        FontSize = "18pt"
                    })
                    .Add(new OxmlTextStyle(StyleIDs.TitleStyle)
                    {
                        FontSize = "10pt"
                    })
                    .Add(new OxmlTextStyle(StyleIDs.SubtitleStyle)
                    {
                        FontColor = "666666"
                    })

                    .FontColor("ffffff").Bold(true)

                    .Add(new OxmlTextStyle(StyleIDs.LabelProfileStyle)
                    {
                        BackColor = "9da9b0"
                    })
                    .Add(new OxmlTextStyle(StyleIDs.LabelFrameworkStyle)
                    {
                        BackColor = "9da9b0"
                    })
                    .Add(new OxmlTextStyle(StyleIDs.LabelAreaStyle)
                    {
                        BackColor = "111111"
                    })
                    .Add(new OxmlTextStyle(StyleIDs.LabelCompetencyStyle)
                    {
                        BackColor = "666666"
                    })

                    .AddToPackage(word);

                var bodyElements = new List<OpenXmlElement>
                {
                    new SectionProperties(new PageMargin
                    {
                        Left = OxmlHelper.InchesToDxa<uint>(0.3125),
                        Right = OxmlHelper.InchesToDxa<uint>(0.3125),
                        Top = OxmlHelper.InchesToDxa<int>(0.625),
                        Bottom = OxmlHelper.InchesToDxa<int>(0.625)
                    }),
                    CreateCaption()
                };

                AddNodes(0, nodes, bodyElements);

                word.MainDocumentPart.Document = new Document(new Body(bodyElements));
                word.Save();
            }
        }

        #endregion

        #region Create elements

        static Paragraph CreateCaption()
        {
            return new Paragraph(
                new ParagraphProperties
                {
                    SpacingBetweenLines = new SpacingBetweenLines
                    {
                        Line = OxmlHelper.InchesToDxa<int>(SpacingBetweenLines).ToString()
                    }
                },
                new Run(
                    new RunProperties { RunStyle = new RunStyle { Val = StyleIDs.CaptionStyle } },
                    new Text(Caption)
                )
            );
        }

        static void AddNodes(int depth, List<NodeItem> nodes, List<OpenXmlElement> bodyElements)
        {
            foreach (var node in nodes)
            {
                bodyElements.Add(CreateNodeParagraph(depth, node));

                if (node.Children.IsNotEmpty())
                    AddNodes(depth + 1, node.Children, bodyElements);
            }
        }

        static Paragraph CreateNodeParagraph(int depth, NodeItem nodeItem)
        {
            var properties = new ParagraphProperties
            {
                SpacingBetweenLines = new SpacingBetweenLines
                {
                    Line = OxmlHelper.InchesToDxa<int>(SpacingBetweenLines).ToString()
                }
            };

            if (depth > 0)
                properties.Indentation = new Indentation
                {
                    Left = OxmlHelper.InchesToDxa<int>(Indent * depth).ToString()
                };

            var list = new List<OpenXmlElement>
            {
                properties
            };

            if (!string.IsNullOrEmpty(nodeItem.Label))
            {
                string labelStyleID;

                switch (nodeItem.ItemType)
                {
                    case StandardNodeType.Profile:
                        labelStyleID = StyleIDs.LabelProfileStyle;
                        break;
                    case StandardNodeType.Framework:
                        labelStyleID = StyleIDs.LabelFrameworkStyle;
                        break;
                    case StandardNodeType.Area:
                        labelStyleID = StyleIDs.LabelAreaStyle;
                        break;
                    case StandardNodeType.Competency:
                        labelStyleID = StyleIDs.LabelCompetencyStyle;
                        break;
                    default:
                        throw new ApplicationException("Unkown node type: " + nodeItem.ItemType);
                }

                list.Add(CreateRun(labelStyleID, "  " + nodeItem.Label + "  "));
                list.Add(CreateRun(StyleIDs.TitleStyle, "  "));
            }

            list.Add(CreateRun(StyleIDs.TitleStyle, nodeItem.Title));
            list.Add(CreateRun(StyleIDs.SubtitleStyle, " " + nodeItem.Subtitle));

            return new Paragraph(list);
        }

        static Run CreateRun(string styleID, string text)
        {
            return new Run(
                new RunProperties { RunStyle = new RunStyle { Val = styleID } },
                new Text { Text = text, Space = SpaceProcessingModeValues.Preserve }
            );
        }

        #endregion
    }
}