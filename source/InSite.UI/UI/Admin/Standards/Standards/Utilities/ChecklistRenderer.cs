using System;
using System.Collections.Generic;
using System.Linq;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Persistence;

using Shift.Constant;
using Shift.Toolbox;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Standards.Utilities
{
    static class ChecklistRenderer
    {
        class StandardInfo
        {
            public Guid StandardIdentifier { get; set; }
            public Guid? ParentStandardIdentifier { get; set; }
            public string Title { get; set; }
            public int Sequence { get; set; }
            public int? ParentSequence { get; set; }

            public List<StandardInfo> Children { get; set; }
        }

        const double textCellWidth = 4.875d;
        const double separatorCellWidth = 0.1d;
        const double boxCellWidth = 0.2d;
        const int boxCellCount = 5;
        const double totalWidth = textCellWidth + (separatorCellWidth + boxCellWidth) * boxCellCount;

        public static void Render(WordprocessingDocument word, Guid rootStandardIdentifier, string lang)
        {
            var competencies = LoadCompetencies(rootStandardIdentifier, lang);

            if (competencies.Count == 0)
                return;

            var table = new Table();

            PrepareTable(table);

            foreach (var parent in competencies)
            {
                AddTextRow(table, parent.Title);
                AddTextRow(table, null);

                if (parent.Children != null)
                {
                    var sequence = 1;

                    foreach (var child in parent.Children)
                    {
                        AddCompetencyRow(table, child, sequence);
                        AddTextRow(table, null);

                        sequence++;
                    }
                }
            }

            word.MainDocumentPart.Document.Body.Append(table);
        }

        private static void AddCompetencyRow(Table table, StandardInfo standard, int sequence)
        {
            { // Row 1
                var row = new TableRow(
                    new TableRowProperties(
                        new TableRowHeight { Val = OxmlHelper.InchesToDxa<uint>(boxCellWidth) }
                    )
                );

                { // Text Cell
                    var cell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Type = TableWidthUnitValues.Dxa,
                                Width = OxmlHelper.InchesToDxa<int>(textCellWidth).ToString()
                            },
                            VerticalMerge = new VerticalMerge
                            {
                                Val = MergedCellValues.Restart
                            },
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder { Val = BorderValues.Nil },
                                RightBorder = new RightBorder { Val = BorderValues.Nil },
                                BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                            }
                        }
                    };

                    var headerParagraph = new Paragraph(
                        new ParagraphProperties
                        {
                            SpacingBetweenLines = new SpacingBetweenLines { After = "0" },
                            Tabs = new Tabs(new TabStop { Val = TabStopValues.Left, Position = 360 })
                        },
                        new Run(new TabChar()),
                        new Run(new Text($"{sequence}.")),
                        new Run(new TabChar()),
                        new Run(new Text(standard.Title))
                    );

                    cell.Append(headerParagraph);

                    row.Append(cell);
                }

                for (var boxIndex = 0; boxIndex < boxCellCount; boxIndex++)
                {
                    { // Separator Cell
                        var cell = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellWidth = new TableCellWidth
                                {
                                    Type = TableWidthUnitValues.Dxa,
                                    Width = OxmlHelper.InchesToDxa<int>(separatorCellWidth).ToString()
                                },
                                TableCellBorders = new TableCellBorders
                                {
                                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                                    RightBorder = new RightBorder { Val = BorderValues.Nil },
                                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                                },
                                TableCellMargin = new TableCellMargin
                                {
                                    LeftMargin = new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = "0" },
                                    RightMargin = new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "0" }
                                }
                            }
                        };

                        cell.Append(new Paragraph(
                            new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                        ));

                        row.Append(cell);
                    }

                    { // Box Cell
                        var cell = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellWidth = new TableCellWidth
                                {
                                    Type = TableWidthUnitValues.Dxa,
                                    Width = OxmlHelper.InchesToDxa<int>(boxCellWidth).ToString()
                                },
                                TableCellBorders = new TableCellBorders
                                {
                                    TopBorder = new TopBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = "auto" },
                                    RightBorder = new RightBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = "auto" },
                                    BottomBorder = new BottomBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = "auto" },
                                    LeftBorder = new LeftBorder { Val = BorderValues.Single, Size = 4, Space = 0, Color = "auto" },
                                },
                                TableCellMargin = new TableCellMargin
                                {
                                    LeftMargin = new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = "0" },
                                    RightMargin = new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "0" }
                                }
                            }
                        };

                        cell.Append(new Paragraph(
                            new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                        ));

                        row.Append(cell);
                    }
                }

                table.Append(row);
            }
        }

        private static void AddTextRow(Table table, string text)
        {
            { // Row 2
                var row = new TableRow();

                {
                    var cell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Type = TableWidthUnitValues.Dxa,
                                Width = OxmlHelper.InchesToDxa<int>(textCellWidth).ToString()
                            },
                            VerticalMerge = !string.IsNullOrEmpty(text) ? new VerticalMerge { Val = MergedCellValues.Restart } : new VerticalMerge(),
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder { Val = BorderValues.Nil },
                                RightBorder = new RightBorder { Val = BorderValues.Nil },
                                BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                            }
                        }
                    };

                    if (!string.IsNullOrEmpty(text))
                    {
                        var headerParagraph = new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId { Val = OxmlHelper.MicrosoftWordStyles.Paragraph.Heading3.Id },
                                new ParagraphMarkRunProperties()
                            )
                            {
                                SpacingBetweenLines = new SpacingBetweenLines { After = "0" },
                            },
                            new Run(new Text(text))
                        );

                        cell.Append(headerParagraph);
                    }
                    else
                    {
                        cell.Append(new Paragraph(
                            new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                        ));
                    }

                    row.Append(cell);
                }

                {
                    var cell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Type = TableWidthUnitValues.Dxa,
                                Width = OxmlHelper.InchesToDxa<int>((separatorCellWidth + boxCellWidth) * boxCellCount).ToString()
                            },
                            GridSpan = new GridSpan { Val = boxCellCount * 2 },
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder { Val = BorderValues.Nil },
                                RightBorder = new RightBorder { Val = BorderValues.Nil },
                                BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                            },
                            TableCellMargin = new TableCellMargin
                            {
                                LeftMargin = new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = "0" },
                                RightMargin = new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "0" }
                            }
                        }
                    };

                    cell.Append(new Paragraph(
                        new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                    ));

                    row.Append(cell);
                }

                table.Append(row);
            }
        }

        private static void PrepareTable(Table table)
        {
            table.Append(
                new TableProperties
                {
                    TableWidth = new TableWidth
                    {
                        Type = TableWidthUnitValues.Dxa,
                        Width = OxmlHelper.InchesToDxa<int>(totalWidth).ToString()
                    },
                    TableLayout = new TableLayout
                    {
                        Type = TableLayoutValues.Fixed
                    },
                }
            );

            var tableGrid = new TableGrid(
                new GridColumn { Width = OxmlHelper.InchesToDxa<int>(textCellWidth).ToString() }
            );

            for (var i = 0; i < boxCellCount; i++)
            {
                tableGrid.Append(
                    new GridColumn { Width = OxmlHelper.InchesToDxa<int>(separatorCellWidth).ToString() },
                    new GridColumn { Width = OxmlHelper.InchesToDxa<int>(boxCellWidth).ToString() }
                );
            }

            table.Append(tableGrid);

            {
                var row = new TableRow(
                    new TableRowProperties(
                        new TableRowHeight { Val = OxmlHelper.InchesToDxa<uint>(boxCellWidth * 1.5) }
                    )
                );

                { // Text Cell
                    var cell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Type = TableWidthUnitValues.Dxa,
                                Width = OxmlHelper.InchesToDxa<int>(textCellWidth).ToString()
                            },
                            VerticalMerge = new VerticalMerge
                            {
                                Val = MergedCellValues.Restart
                            },
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder { Val = BorderValues.Nil },
                                RightBorder = new RightBorder { Val = BorderValues.Nil },
                                BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                            }
                        }
                    };

                    cell.Append(new Paragraph(
                        new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                    ));

                    row.Append(cell);
                }

                for (var boxIndex = 0; boxIndex < boxCellCount; boxIndex++)
                {
                    { // Separator Cell
                        var cell = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellWidth = new TableCellWidth
                                {
                                    Type = TableWidthUnitValues.Dxa,
                                    Width = OxmlHelper.InchesToDxa<int>(separatorCellWidth).ToString()
                                },
                                TableCellBorders = new TableCellBorders
                                {
                                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                                    RightBorder = new RightBorder { Val = BorderValues.Nil },
                                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                                },
                                TableCellMargin = new TableCellMargin
                                {
                                    LeftMargin = new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = "0" },
                                    RightMargin = new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "0" }
                                }
                            }
                        };

                        cell.Append(new Paragraph(
                            new ParagraphProperties { SpacingBetweenLines = new SpacingBetweenLines { After = "0" } }
                        ));

                        row.Append(cell);
                    }

                    { // Box Cell
                        var cell = new TableCell
                        {
                            TableCellProperties = new TableCellProperties
                            {
                                TableCellWidth = new TableCellWidth
                                {
                                    Type = TableWidthUnitValues.Dxa,
                                    Width = OxmlHelper.InchesToDxa<int>(boxCellWidth).ToString()
                                },
                                TableCellBorders = new TableCellBorders
                                {
                                    TopBorder = new TopBorder { Val = BorderValues.Nil },
                                    RightBorder = new RightBorder { Val = BorderValues.Nil },
                                    BottomBorder = new BottomBorder { Val = BorderValues.Nil },
                                    LeftBorder = new LeftBorder { Val = BorderValues.Nil },
                                },
                                TableCellMargin = new TableCellMargin
                                {
                                    LeftMargin = new LeftMargin { Type = TableWidthUnitValues.Dxa, Width = "0" },
                                    RightMargin = new RightMargin { Type = TableWidthUnitValues.Dxa, Width = "0" }
                                }
                            }
                        };

                        cell.Append(new Paragraph(
                            new ParagraphProperties
                            {
                                SpacingBetweenLines = new SpacingBetweenLines { After = "0" },
                                Justification = new Justification { Val = JustificationValues.Center }
                            },
                            new Run(new Text((boxIndex + 1).ToString()))
                        ));

                        row.Append(cell);
                    }
                }

                table.Append(row);
            }
        }

        private static List<StandardInfo> LoadCompetencies(Guid standardKey, string lang)
        {
            var data = StandardContainmentSearch.Bind(
                    x => new StandardInfo
                    {
                        StandardIdentifier = x.Child.StandardIdentifier,
                        ParentStandardIdentifier = x.Child.ParentStandardIdentifier,
                        Title = CoreFunctions.GetContentText(x.Child.StandardIdentifier, ContentLabel.Title, lang)
                                ?? CoreFunctions.GetContentTextEn(x.Child.StandardIdentifier, ContentLabel.Title),
                        Sequence = x.Child.Sequence,
                        ParentSequence = x.Child.Parent.Sequence,
                    },
                    x => x.ParentStandardIdentifier == standardKey
                      && x.Child.StandardType == StandardType.Competency,
                    "ParentSequence, Sequence, Title"
                )
                .ToList();

            var map = data.ToDictionary(x => x.StandardIdentifier);

            var result = new List<StandardInfo>();

            foreach (var standard in data)
            {
                if (standard.ParentStandardIdentifier.HasValue && map.TryGetValue(standard.ParentStandardIdentifier.Value, out var parent))
                {
                    if (parent.Children == null)
                        parent.Children = new List<StandardInfo>();

                    parent.Children.Add(standard);
                }
                else
                    result.Add(standard);
            }

            return result;
        }
    }
}