using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Wordprocessing;

using HtmlToOpenXml.IO;

using Shift.Common;
using Shift.Sdk.UI;
using Shift.Toolbox;

using Document = DocumentFormat.OpenXml.Wordprocessing.Document;
using DocxHtmlConverter = HtmlToOpenXml.HtmlConverter;
using DrawingPictures = DocumentFormat.OpenXml.Drawing.Pictures;
using DrawingWordprocessing = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DrawingFont = System.Drawing.Font;
using DrawingFontStyle = System.Drawing.FontStyle;
using DrawingGraphics = System.Drawing.Graphics;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    public class QuestionDocxPrinter : IWebRequest
    {
        private const double DefaultFontSize = 12;
        private const int ListHangingIndent = 240;
        private const int ListLevelIndent = 720;
        private const int ListTextInset = 300;

        private readonly string _authorName;
        private readonly string _defaultImagePath;
        private readonly string _websiteUrl;

        public QuestionDocxPrinter(string authorName, string defaultImagePath, string websiteUrl)
        {
            _authorName = authorName;
            _defaultImagePath = defaultImagePath;
            _websiteUrl = websiteUrl;
        }

        public PrintOutputFile RenderDocx(string fileName, string html)
        {
            using (var stream = new MemoryStream())
            {
                using (var word = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document))
                {
                    SetupDocx(word);

                    RenderDocxContent(word, html);

                    word.Save();
                }

                return new PrintOutputFile(fileName, "docx", stream.ToArray());
            }
        }

        private void RenderDocxContent(WordprocessingDocument word, string html)
        {
            var htmlConverter = new DocxHtmlConverter(word.MainDocumentPart, this);

            try
            {
                using (var document = PrepareHtml(html))
                {
                    var images = MarkImages(document);
                    Shift.Common.TaskRunner.RunSync(() => htmlConverter.ParseBody(document.DocumentElement.OuterHtml));
                    ApplyTableBorderLayout(word.MainDocumentPart.Document.Body);
                    ApplyQuestionLayout(word.MainDocumentPart.Document.Body, document);
                    ApplyListLayout(word.MainDocumentPart.Document.Body);
                    ApplyImageLayout(word.MainDocumentPart, images);
                }
            }
            catch (Exception)
            {
                // Discard any partially converted content while retaining the page settings.
                foreach (var element in word.MainDocumentPart.Document.Body.ChildElements.ToArray())
                    if (!(element is SectionProperties))
                        element.Remove();

                Shift.Common.TaskRunner.RunSync(() => htmlConverter.ParseBody("<p style='font-size:30pt; font-weight:bold; color:red;'>UNABLE TO PARSE HTML CODE</p>"));
            }
        }

        private static IHtmlDocument PrepareHtml(string html)
        {
            var parser = new HtmlParser();
            string inlineHtml;
            using (var document = parser.ParseDocument(html))
            {
                foreach (var element in document.QuerySelectorAll("link[rel~='stylesheet'], script").ToArray())
                    element.Remove();

                // Supply the browser's paragraph gaps before the template's more specific overrides.
                var defaults = document.CreateElement("style");
                defaults.TextContent = "p { margin: 1em 0; }";
                document.Head.InsertBefore(defaults, document.Head.FirstChild);

                // Expand authored shorthand before PreMailer discards its !important priority.
                foreach (var paragraph in document.QuerySelectorAll("p"))
                    NormalizeParagraphMargins(paragraph);

                inlineHtml = Shift.Toolbox.HtmlBuilder.MoveCssInline(document.DocumentElement.OuterHtml);
            }

            var result = parser.ParseDocument(inlineHtml);
            try
            {
                result.Body.SetAttribute("style", result.Body.GetAttribute("style") + ";font-size:"
                    + DefaultFontSize.ToString(CultureInfo.InvariantCulture) + "pt");

                NormalizeStyles(result.Body, DefaultFontSize);

                foreach (var table in result.QuerySelectorAll("table[data-docx-option-layout]"))
                {
                    foreach (var cell in table.QuerySelectorAll("th.oicon, td.oicon").Where(x => x.Closest("table") == table).ToArray())
                        cell.Remove();
                }

                return result;
            }
            catch
            {
                result.Dispose();
                throw;
            }
        }

        private static void NormalizeStyles(IElement element, double inheritedFontSize)
        {
            if (string.Equals(GetStyleValue(element, "display"), "none", StringComparison.OrdinalIgnoreCase))
            {
                element.Remove();
                return;
            }

            var fontSize = inheritedFontSize;
            var size = GetStyleValue(element, "font-size");
            if (size.Length > 0)
            {
                fontSize = GetPoints(size, inheritedFontSize);
                if (fontSize <= 0)
                    fontSize = inheritedFontSize;

                // HtmlToOpenXml resolves em sizes against its own default rather than the parent.
                element.SetAttribute("style", element.GetAttribute("style") + ";font-size:"
                    + fontSize.ToString("0.###", CultureInfo.InvariantCulture) + "pt");
            }

            if (element.LocalName == "p")
                NormalizeParagraphMargins(element, fontSize);

            foreach (var child in element.Children.ToArray())
                NormalizeStyles(child, fontSize);
        }

        private static void NormalizeParagraphMargins(IElement paragraph, double? fontSize = null)
        {
            var sides = new[] { "top", "right", "bottom", "left" };
            var values = new string[4];
            var important = new bool[4];
            var declarations = Regex.Matches(paragraph.GetAttribute("style") ?? string.Empty,
                @"(?:^|;)\s*margin(?:-(top|right|bottom|left))?\s*:\s*([^;]*)", RegexOptions.IgnoreCase);

            foreach (Match declaration in declarations)
            {
                var value = declaration.Groups[2].Value.Trim();
                var isImportant = Regex.IsMatch(value, @"\s*!important\s*$", RegexOptions.IgnoreCase);
                value = Regex.Replace(value, @"\s*!important\s*$", string.Empty, RegexOptions.IgnoreCase).Trim();
                var side = declaration.Groups[1].Value.ToLowerInvariant();
                var parts = Regex.Split(value, @"\s+");
                if (side.Length == 0 && (parts.Length < 1 || parts.Length > 4))
                    continue;

                var margins = new[] { parts[0], parts.Length > 1 ? parts[1] : parts[0],
                    parts.Length > 2 ? parts[2] : parts[0], parts.Length > 3 ? parts[3] : parts.Length > 1 ? parts[1] : parts[0] };
                for (var i = 0; i < sides.Length; i++)
                {
                    if ((side.Length > 0 && side != sides[i]) || (important[i] && !isImportant))
                        continue;

                    values[i] = side.Length > 0 ? value : margins[i];
                    important[i] = isImportant;
                }
            }

            var style = paragraph.GetAttribute("style") ?? string.Empty;
            for (var i = 0; i < sides.Length; i++)
            {
                if (values[i] == null || (fontSize.HasValue && !Regex.IsMatch(values[i], @"^-?(?:\d+(?:\.\d*)?|\.\d+)(px|pt|in|cm|mm|pc|rem|em|%)?$", RegexOptions.IgnoreCase)))
                    continue;

                var value = fontSize.HasValue
                    ? GetPoints(values[i], fontSize.Value).ToString("0.###", CultureInfo.InvariantCulture) + "pt"
                    : values[i] + (important[i] ? " !important" : string.Empty);
                style += ";margin-" + sides[i] + ":" + value;
            }
            paragraph.SetAttribute("style", style);
        }

        private static string GetStyleValue(IElement element, string property)
        {
            var matches = Regex.Matches(element.GetAttribute("style") ?? string.Empty,
                @"(?:^|;)\s*" + Regex.Escape(property) + @"\s*:\s*([^;]*)", RegexOptions.IgnoreCase);
            return matches.Count == 0
                ? string.Empty
                : Regex.Replace(matches[matches.Count - 1].Groups[1].Value, @"\s*!important\s*$", string.Empty,
                    RegexOptions.IgnoreCase).Trim();
        }

        private static double GetPoints(string value, double fontSize)
        {
            var match = Regex.Match(value, @"^(-?(?:\d+(?:\.\d*)?|\.\d+))(px|pt|in|cm|mm|pc|rem|em|%)?$", RegexOptions.IgnoreCase);
            if (!match.Success)
                return 0;

            var number = double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            switch (match.Groups[2].Value.ToLowerInvariant())
            {
                case "px": return number * 0.75;
                case "in": return number * 72;
                case "cm": return number * 72 / 2.54;
                case "mm": return number * 72 / 25.4;
                case "pc": return number * 12;
                case "em": return number * fontSize;
                case "rem": return number * DefaultFontSize;
                case "%": return number * fontSize / 100;
                default: return number;
            }
        }

        private sealed class PreparedImage
        {
            public IElement Element { get; set; }
            public string Description { get; set; }
        }

        private static Dictionary<string, PreparedImage> MarkImages(IHtmlDocument document)
        {
            var images = new Dictionary<string, PreparedImage>();
            var prefix = "docx-image-" + Guid.NewGuid().ToString("N") + "-";
            foreach (var element in document.Body.QuerySelectorAll("img"))
            {
                var id = prefix + images.Count.ToString(CultureInfo.InvariantCulture);
                images.Add(id, new PreparedImage
                {
                    Element = element,
                    Description = element.GetAttribute("title") ?? element.GetAttribute("alt") ?? string.Empty
                });
                // The converter copies title into the picture description, even for repeated/data URI sources.
                element.SetAttribute("title", id);
            }
            return images;
        }

        private static void ApplyImageLayout(MainDocumentPart part, Dictionary<string, PreparedImage> images)
        {
            var sizes = new Dictionary<string, HtmlToOpenXml.Size>();
            foreach (var drawing in part.Document.Body.Descendants<Drawing>())
            {
                var inline = drawing.GetFirstChild<DrawingWordprocessing.Inline>();
                var picture = inline?.Descendants<DrawingPictures.Picture>().FirstOrDefault();
                var properties = picture?.NonVisualPictureProperties?.NonVisualDrawingProperties;
                if (properties?.Description == null || !images.TryGetValue(properties.Description.Value, out var image))
                    continue; // SVG drawings use their own metadata and retain the existing converter layout.

                properties.Description = image.Description;
                var relationship = picture.BlipFill?.Blip?.Embed?.Value;
                var imagePart = relationship == null ? null : part.GetPartById(relationship) as ImagePart;
                var transform = picture.ShapeProperties?.Transform2D?.Extents;
                if (imagePart == null || inline.Extent == null || transform == null)
                    continue;

                if (!sizes.TryGetValue(relationship, out var actualSize))
                {
                    using (var stream = imagePart.GetStream(FileMode.Open, FileAccess.Read))
                        actualSize = ImageHeader.GetDimensions(stream);
                    sizes.Add(relationship, actualSize);
                }
                if (actualSize.Width <= 0 || actualSize.Height <= 0)
                    continue;

                var availableWidth = GetImageAvailableWidth(part, drawing);
                var fontSize = GetPoints(GetInheritedStyleValue(image.Element, "font-size"), DefaultFontSize);
                if (fontSize <= 0)
                    fontSize = DefaultFontSize;

                var width = GetImageDimension(image.Element, "width", fontSize, availableWidth);
                var height = GetImageDimension(image.Element, "height", fontSize, null);
                var preferredWidth = width ?? actualSize.Width * 0.75;
                var preferredHeight = height ?? actualSize.Height * 0.75;

                if (width.HasValue && !height.HasValue)
                    preferredHeight = preferredWidth * actualSize.Height / actualSize.Width;
                else if (height.HasValue && !width.HasValue)
                    preferredWidth = preferredHeight * actualSize.Width / actualSize.Height;

                var maxWidth = GetImageLength(GetStyleValue(image.Element, "max-width"), fontSize, availableWidth);
                var maxHeight = GetImageLength(GetStyleValue(image.Element, "max-height"), fontSize, null);
                var scale = preferredWidth > 0 ? Math.Min(1, Math.Min(availableWidth, maxWidth ?? availableWidth) / preferredWidth) : 1;
                if (preferredHeight > 0 && maxHeight.HasValue)
                    scale = Math.Min(scale, maxHeight.Value / preferredHeight);

                // Both extents must agree. Keep the embedded pixels untouched and size at browser (96) DPI.
                var cx = Math.Max(1, (long)Math.Round(preferredWidth * scale * 12700));
                var cy = Math.Max(1, (long)Math.Round(preferredHeight * scale * 12700));
                inline.Extent.Cx = transform.Cx = cx;
                inline.Extent.Cy = transform.Cy = cy;
            }
        }

        private static double? GetImageDimension(IElement element, string property, double fontSize, double? percentageReference)
        {
            var value = GetStyleValue(element, property);
            if (string.Equals(value, "auto", StringComparison.OrdinalIgnoreCase))
                return null;

            return GetImageLength(value, fontSize, percentageReference)
                ?? GetImageLength(element.GetAttribute(property) ?? string.Empty, fontSize, percentageReference, true);
        }

        private static double? GetImageLength(string value, double fontSize, double? percentageReference, bool pixels = false)
        {
            value = value.Trim();
            var match = Regex.Match(value, @"^(\d+(?:\.\d*)?|\.\d+)(px|pt|in|cm|mm|pc|rem|em|%)?$", RegexOptions.IgnoreCase);
            if (!match.Success
                || !double.TryParse(match.Groups[1].Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number)
                || double.IsInfinity(number)
            )
            {
                return null;
            }

            var unit = match.Groups[2].Value;
            if (unit == "%")
                return percentageReference * number / 100;

            if (unit.Length == 0)
                return pixels ? number * 0.75 : number == 0 ? (double?)0 : null;

            return GetPoints(value, fontSize);
        }

        private static double GetImageAvailableWidth(MainDocumentPart part, Drawing drawing)
        {
            var section = part.Document.Body.Elements<SectionProperties>().Last();
            double width = section.GetFirstChild<PageSize>().Width.Value;
            var pageMargins = section.GetFirstChild<PageMargin>();
            width -= pageMargins.Left.Value + pageMargins.Right.Value + pageMargins.Gutter.Value;

            var cell = drawing.Ancestors<TableCell>().FirstOrDefault();
            if (cell != null)
            {
                var properties = cell.TableCellProperties;
                if (properties?.TableCellWidth?.Type?.Value == TableWidthUnitValues.Dxa
                    && double.TryParse(properties.TableCellWidth.Width?.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var cellWidth))
                {
                    width = cellWidth;
                }

                var defaults = cell.Ancestors<Table>().First().GetFirstChild<TableProperties>()?.TableCellMarginDefault;
                var left = properties?.TableCellMargin?.GetFirstChild<LeftMargin>()?.Width?.Value
                    ?? defaults?.GetFirstChild<TableCellLeftMargin>()?.Width?.Value.ToString(CultureInfo.InvariantCulture);
                var right = properties?.TableCellMargin?.GetFirstChild<RightMargin>()?.Width?.Value
                    ?? defaults?.GetFirstChild<TableCellRightMargin>()?.Width?.Value.ToString(CultureInfo.InvariantCulture);
                width -= double.TryParse(left, NumberStyles.Integer, CultureInfo.InvariantCulture, out var leftMargin) ? leftMargin : 108;
                width -= double.TryParse(right, NumberStyles.Integer, CultureInfo.InvariantCulture, out var rightMargin) ? rightMargin : 108;
            }

            var indentation = drawing.Ancestors<Paragraph>().FirstOrDefault()?.ParagraphProperties?.Indentation;
            if (indentation != null)
            {
                var values = new[]
                {
                    indentation.Start?.Value ?? indentation.Left?.Value,
                    indentation.End?.Value ?? indentation.Right?.Value,
                    indentation.FirstLine?.Value
                };

                foreach (var value in values)
                {
                    if (double.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var indent))
                        width -= Math.Max(0, indent);
                }
            }

            return Math.Max(1, width) / 20;
        }

        private static void ApplyTableBorderLayout(Body body)
        {
            // Missing and "none" borders can lose to the converter's TableGrid style during
            // Word's conflict resolution. "Nil" definitively suppresses absent borders.
            foreach (var table in body.Descendants<Table>())
            {
                var properties = table.GetFirstChild<TableProperties>() ?? table.PrependChild(new TableProperties());
                var borders = properties.TableBorders ?? (properties.TableBorders = new TableBorders());

                borders.TopBorder = GetHiddenBorder(borders.TopBorder);
                borders.LeftBorder = GetHiddenBorder(borders.LeftBorder);
                borders.BottomBorder = GetHiddenBorder(borders.BottomBorder);
                borders.RightBorder = GetHiddenBorder(borders.RightBorder);
                borders.InsideHorizontalBorder = GetHiddenBorder(borders.InsideHorizontalBorder);
                borders.InsideVerticalBorder = GetHiddenBorder(borders.InsideVerticalBorder);
            }

            var borderGroups = body.Descendants<TableBorders>().Cast<OpenXmlCompositeElement>()
                .Concat(body.Descendants<TableCellBorders>());

            foreach (var border in borderGroups.SelectMany(x => x.ChildElements.OfType<BorderType>()))
            {
                if (border.Val?.Value == BorderValues.None)
                    border.Val = BorderValues.Nil;
            }
        }

        private static T GetHiddenBorder<T>(T border) where T : BorderType, new()
        {
            border = border ?? new T();
            if (border.Val == null || border.Val.Value == BorderValues.None)
                border.Val = BorderValues.Nil;

            return border;
        }

        private static void ApplyListLayout(Body body)
        {
            // HtmlToOpenXml can copy the body's zero margin onto list paragraphs. Retain the
            // compact text position while leaving enough room inside the cell for the marker.
            foreach (var paragraph in body.Descendants<Paragraph>())
            {
                var properties = paragraph.ParagraphProperties;
                var indentation = properties?.Indentation;
                if (properties?.NumberingProperties == null || indentation == null)
                    continue;

                var attributes = indentation.GetAttributes();
                var hasLeft = attributes.Any(x => x.LocalName == "left" || x.LocalName == "start");
                var hasRight = attributes.Any(x => x.LocalName == "right" || x.LocalName == "end");
                if (hasLeft && hasRight && attributes.All(IsZeroHorizontalIndent))
                {
                    var level = properties.NumberingProperties.NumberingLevelReference?.Val?.Value ?? 0;
                    var cellMargin = GetListContainerLeftMargin(paragraph);
                    properties.Indentation = new Indentation
                    {
                        Left = (Math.Max(0, ListTextInset - cellMargin) + level * ListLevelIndent)
                            .ToString(CultureInfo.InvariantCulture),
                        Right = "0",
                        Hanging = ListHangingIndent.ToString(CultureInfo.InvariantCulture)
                    };
                }
            }
        }

        private static int GetListContainerLeftMargin(Paragraph paragraph)
        {
            var cell = paragraph.Ancestors<TableCell>().FirstOrDefault();
            if (cell == null)
                return 0;

            var properties = cell.TableCellProperties;
            var defaults = cell.Ancestors<Table>().First().GetFirstChild<TableProperties>()?.TableCellMarginDefault;
            var value = properties?.TableCellMargin?.GetFirstChild<LeftMargin>()?.Width?.Value
                ?? defaults?.GetFirstChild<TableCellLeftMargin>()?.Width?.Value.ToString(CultureInfo.InvariantCulture);

            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var margin)
                ? Math.Max(0, margin)
                : 0;
        }

        private static bool IsZeroHorizontalIndent(OpenXmlAttribute attribute)
        {
            if (attribute.LocalName != "left" && attribute.LocalName != "start"
                && attribute.LocalName != "right" && attribute.LocalName != "end")
            {
                return false;
            }

            return int.TryParse(attribute.Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
                && value == 0;
        }

        private static void ApplyQuestionLayout(Body body, IHtmlDocument document)
        {
            // Word requires a paragraph after a nested table at the end of a cell.
            foreach (var cell in body.Descendants<TableCell>().ToArray())
            {
                if (cell.LastChild is Table)
                {
                    cell.AppendChild(new Paragraph(new ParagraphProperties(new SpacingBetweenLines
                    {
                        Before = "0",
                        After = "0",
                        Line = "20",
                        LineRule = LineSpacingRuleValues.Exact
                    })));
                }
            }

            var htmlTables = document.Body.Children.Where(x => x.LocalName == "table").ToArray();
            var wordTables = body.Elements<Table>().ToArray();
            if (htmlTables.Length != wordTables.Length)
                throw new InvalidOperationException("The converted question tables do not match the HTML tables.");

            var questionIndexes = Enumerable.Range(0, htmlTables.Length)
                .Where(i => htmlTables[i].ClassList.Contains("question")).ToArray();

            foreach (var index in questionIndexes)
            {
                ApplyQuestionTableLayout(wordTables[index], htmlTables[index]);

                if (index != questionIndexes.Last())
                {
                    // Word tables have no bottom margin. A paragraph also keeps adjacent tables separate.
                    wordTables[index].InsertAfterSelf(new Paragraph(
                        new ParagraphProperties(new SpacingBetweenLines
                        {
                            Before = "0", After = "0", Line = "750", LineRule = LineSpacingRuleValues.Exact
                        })));
                }
            }
        }

        private static void ApplyQuestionTableLayout(Table table, IElement htmlTable)
        {
            var widths = new[] { 675, 6345 + 675, 3780 };
            var properties = table.GetFirstChild<TableProperties>() ?? table.PrependChild(new TableProperties());

            properties.TableWidth = new TableWidth
            {
                Width = widths.Sum().ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa
            };
            properties.TableLayout = new TableLayout { Type = TableLayoutValues.Fixed };

            table.GetFirstChild<TableGrid>()?.Remove();
            table.InsertAfter(new TableGrid(widths.Select(width => new GridColumn
            {
                Width = width.ToString(CultureInfo.InvariantCulture)
            })), properties);

            var htmlRows = htmlTable.QuerySelectorAll("tr").Where(x => x.Closest("table") == htmlTable).ToArray();
            var wordRows = table.Elements<TableRow>().ToArray();
            if (htmlRows.Length != wordRows.Length)
                throw new InvalidOperationException("The converted question rows do not match the HTML rows.");

            var mergedCells = new IElement[widths.Length];
            var optionTables = new Dictionary<Table, IElement>();
            for (var rowIndex = 0; rowIndex < wordRows.Length; rowIndex++)
            {
                var htmlCells = htmlRows[rowIndex].Children.Where(x => x.LocalName == "td" || x.LocalName == "th").ToArray();
                var htmlCellIndex = 0;
                var columnIndex = 0;
                foreach (var cell in wordRows[rowIndex].Elements<TableCell>())
                {
                    var cellProperties = cell.TableCellProperties ?? (cell.TableCellProperties = new TableCellProperties());
                    var span = cellProperties.GridSpan?.Val?.Value ?? 1;
                    if (columnIndex + span > widths.Length)
                        throw new InvalidOperationException("A question row exceeds the expected three columns.");

                    var merge = cellProperties.VerticalMerge;
                    var isContinuation = merge != null && (merge.Val == null || merge.Val.Value == MergedCellValues.Continue);
                    var htmlCell = isContinuation ? mergedCells[columnIndex] : htmlCells[htmlCellIndex++];

                    cellProperties.TableCellWidth = new TableCellWidth
                    {
                        Width = widths.Skip(columnIndex).Take(span).Sum().ToString(CultureInfo.InvariantCulture),
                        Type = TableWidthUnitValues.Dxa
                    };
                    if (htmlCell != null)
                        cellProperties.TableCellMargin = GetCellMargins(htmlCell);

                    for (var i = 0; i < span; i++)
                        mergedCells[columnIndex++] = htmlCell;
                }

                if (columnIndex != widths.Length || htmlCellIndex != htmlCells.Length)
                    throw new InvalidOperationException("A question row does not match the expected three columns.");

                var htmlOptionTable = htmlCells.SelectMany(cell => cell.Children)
                    .FirstOrDefault(x => x.LocalName == "table" && x.HasAttribute("data-docx-option-layout"));
                if (htmlOptionTable != null)
                {
                    var wordOptionTable = wordRows[rowIndex].Elements<TableCell>().SelectMany(cell => cell.Elements<Table>()).Single();
                    optionTables.Add(wordOptionTable, htmlOptionTable);
                }
            }

            FitNestedTables(table, optionTables);
        }

        private static void FitNestedTables(Table questionTable, Dictionary<Table, IElement> optionTables)
        {
            // Empty grids, especially with a colspan in the first row, can also break the outer layout.
            foreach (var table in questionTable.Descendants<Table>())
            {
                var parent = table.Ancestors<TableCell>().First().TableCellProperties;
                var availableWidth = int.Parse(parent.TableCellWidth.Width.Value, CultureInfo.InvariantCulture);
                var margins = parent.TableCellMargin;

                var values = new[]
                {
                    margins?.GetFirstChild<LeftMargin>()?.Width?.Value,
                    margins?.GetFirstChild<RightMargin>()?.Width?.Value
                };

                foreach (var margin in values)
                {
                    if (int.TryParse(margin, NumberStyles.Integer, CultureInfo.InvariantCulture, out var padding))
                        availableWidth -= padding;
                }

                if (optionTables.TryGetValue(table, out var htmlOptionTable))
                {
                    ApplyOptionTableLayout(table, htmlOptionTable, availableWidth);
                    continue;
                }

                var grid = table.GetFirstChild<TableGrid>().Elements<GridColumn>().ToArray();
                var columnWidth = Math.Max(1, availableWidth / grid.Length);
                foreach (var column in grid)
                    column.Width = columnWidth.ToString(CultureInfo.InvariantCulture);

                table.GetFirstChild<TableProperties>().TableWidth = new TableWidth
                {
                    Width = (columnWidth * grid.Length).ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa
                };
                foreach (var cell in table.Elements<TableRow>().SelectMany(row => row.Elements<TableCell>()))
                {
                    var properties = cell.TableCellProperties ?? (cell.TableCellProperties = new TableCellProperties());
                    properties.TableCellWidth = new TableCellWidth
                    {
                        Width = (columnWidth * (properties.GridSpan?.Val?.Value ?? 1)).ToString(CultureInfo.InvariantCulture),
                        Type = TableWidthUnitValues.Dxa
                    };
                }
            }
        }

        private static void ApplyOptionTableLayout(Table table, IElement htmlTable, int availableWidth)
        {
            var htmlRows = htmlTable.QuerySelectorAll("tr").Where(x => x.Closest("table") == htmlTable).ToArray();
            var wordRows = table.Elements<TableRow>().ToArray();
            var htmlCells = htmlRows.Select(row => row.Children.Where(x => x.LocalName == "td" || x.LocalName == "th").ToArray()).ToArray();
            var columnCount = table.GetFirstChild<TableGrid>().Elements<GridColumn>().Count();
            var textColumnCount = columnCount - 2;
            if (textColumnCount < 1 || htmlRows.Length != wordRows.Length || htmlCells.Any(cells => cells.Length != columnCount))
                throw new InvalidOperationException("The converted option table does not match the HTML columns.");

            int numberWidth;
            int requestedPointsWidth;
            using (var bitmap = new System.Drawing.Bitmap(1, 1))
            {
                using (var graphics = DrawingGraphics.FromImage(bitmap))
                {
                    graphics.PageUnit = System.Drawing.GraphicsUnit.Point;
                    numberWidth = Math.Max(375, htmlCells.Max(cells => MeasureCellWidth(graphics, cells[0])));
                    requestedPointsWidth = htmlCells.Max(cells => MeasureCellWidth(graphics, cells[columnCount - 1]));
                }
            }

            var pointsWidth = Math.Max(1, Math.Min(requestedPointsWidth, availableWidth - numberWidth - 1440));
            var remainingWidth = availableWidth - numberWidth - pointsWidth;
            if (remainingWidth < textColumnCount)
                throw new InvalidOperationException("The option table is too narrow for its answer columns.");

            var widths = new int[columnCount];
            widths[0] = numberWidth;
            widths[columnCount - 1] = pointsWidth;
            for (var i = 1; i <= textColumnCount; i++)
                widths[i] = remainingWidth / textColumnCount + (i <= remainingWidth % textColumnCount ? 1 : 0);

            var properties = table.GetFirstChild<TableProperties>();
            properties.TableWidth = new TableWidth { Width = availableWidth.ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa };
            properties.TableLayout = new TableLayout { Type = TableLayoutValues.Fixed };

            var columns = table.GetFirstChild<TableGrid>().Elements<GridColumn>().ToArray();
            for (var i = 0; i < widths.Length; i++)
                columns[i].Width = widths[i].ToString(CultureInfo.InvariantCulture);

            for (var rowIndex = 0; rowIndex < wordRows.Length; rowIndex++)
            {
                var cells = wordRows[rowIndex].Elements<TableCell>().ToArray();
                if (cells.Length != columnCount)
                    throw new InvalidOperationException("The converted option row does not match the HTML columns.");

                for (var columnIndex = 0; columnIndex < cells.Length; columnIndex++)
                {
                    var cellProperties = cells[columnIndex].TableCellProperties ?? (cells[columnIndex].TableCellProperties = new TableCellProperties());
                    cellProperties.TableCellWidth = new TableCellWidth
                    {
                        Width = widths[columnIndex].ToString(CultureInfo.InvariantCulture), Type = TableWidthUnitValues.Dxa
                    };
                    cellProperties.TableCellMargin = GetCellMargins(htmlCells[rowIndex][columnIndex]);
                    cellProperties.NoWrap = new NoWrap
                    {
                        Val = columnIndex == 0 || (columnIndex == columnCount - 1 && pointsWidth >= requestedPointsWidth)
                            ? OnOffOnlyValues.On : OnOffOnlyValues.Off
                    };
                }
            }
        }

        private static int MeasureCellWidth(DrawingGraphics graphics, IElement cell)
        {
            var text = Regex.Replace(cell.TextContent, @"\s+", " ").Trim();
            var size = GetPoints(GetInheritedStyleValue(cell, "font-size"), DefaultFontSize);
            var family = GetInheritedStyleValue(cell, "font-family").Split(',')[0].Trim(' ', '\'', '"');
            var weight = GetInheritedStyleValue(cell, "font-weight");
            var fontStyle = string.Equals(weight, "bold", StringComparison.OrdinalIgnoreCase)
                || (int.TryParse(weight, out var numericWeight) && numericWeight >= 600)
                ? DrawingFontStyle.Bold : DrawingFontStyle.Regular;

            if (string.Equals(GetInheritedStyleValue(cell, "font-style"), "italic", StringComparison.OrdinalIgnoreCase))
                fontStyle |= DrawingFontStyle.Italic;

            double textWidth;
            using (var font = new DrawingFont(family.Length > 0 ? family : "Arial", (float)(size > 0 ? size : DefaultFontSize), fontStyle, System.Drawing.GraphicsUnit.Point))
            {
                using (var format = System.Drawing.StringFormat.GenericTypographic)
                    textWidth = text.Length == 0 ? 0 : graphics.MeasureString(text, font, int.MaxValue, format).Width;
            }

            var margins = GetCellMargins(cell);
            var padding = int.Parse(margins.GetFirstChild<LeftMargin>().Width.Value, CultureInfo.InvariantCulture)
                + int.Parse(margins.GetFirstChild<RightMargin>().Width.Value, CultureInfo.InvariantCulture);

            // Leave a little room for font substitution and differences in Word's text metrics.
            return (int)Math.Ceiling((textWidth * 1.1 + 6) * 20) + padding;
        }

        private static string GetInheritedStyleValue(IElement element, string property)
        {
            for (var current = element; current != null; current = current.ParentElement)
            {
                var value = GetStyleValue(current, property);
                if (value.Length > 0)
                    return value;
            }

            return string.Empty;
        }

        private static TableCellMargin GetCellMargins(IElement cell)
        {
            var padding = GetStyleValue(cell, "padding");
            var parts = padding.Length == 0 ? new string[0] : Regex.Split(padding, @"\s+");
            var values = new[] { "0", "0", "0", "0" };
            if (parts.Length > 0 && parts.Length <= 4)
            {
                values[0] = parts[0];
                values[1] = parts.Length > 1 ? parts[1] : parts[0];
                values[2] = parts.Length > 2 ? parts[2] : parts[0];
                values[3] = parts.Length > 3 ? parts[3] : values[1];
            }

            var fontSize = GetPoints(GetStyleValue(cell, "font-size"), DefaultFontSize);
            if (fontSize <= 0)
                fontSize = DefaultFontSize;

            var sides = new[] { "top", "right", "bottom", "left" };
            var widths = new string[4];
            for (var i = 0; i < sides.Length; i++)
            {
                var value = GetStyleValue(cell, "padding-" + sides[i]);
                widths[i] = Math.Max(0, (int)Math.Round(GetPoints(value.Length > 0 ? value : values[i], fontSize) * 20))
                    .ToString(CultureInfo.InvariantCulture);
            }

            return new TableCellMargin(
                new TopMargin { Width = widths[0], Type = TableWidthUnitValues.Dxa },
                new LeftMargin { Width = widths[3], Type = TableWidthUnitValues.Dxa },
                new BottomMargin { Width = widths[2], Type = TableWidthUnitValues.Dxa },
                new RightMargin { Width = widths[1], Type = TableWidthUnitValues.Dxa });
        }

        private void SetupDocx(WordprocessingDocument word)
        {
            word.AddMainDocumentPart();
            var documentBody = new Body();
            var wordDocument = new Document(documentBody);
            wordDocument.Save(word.MainDocumentPart);

            OxmlHelper.MicrosoftWordStyles.SetupDefault(word.MainDocumentPart);

            var sectionProps = new SectionProperties(
                new PageSize
                {
                    Width = OxmlHelper.InchesToDxa<uint>(8.5),
                    Height = OxmlHelper.InchesToDxa<uint>(11),
                    Orient = PageOrientationValues.Portrait
                },
                new PageMargin
                {
                    Left = OxmlHelper.InchesToDxa<uint>(0.25),
                    Right = OxmlHelper.InchesToDxa<uint>(0.25),
                    Top = OxmlHelper.InchesToDxa<int>(0.5),
                    Bottom = OxmlHelper.InchesToDxa<int>(0.5),
                    Header = OxmlHelper.InchesToDxa<uint>(0.25),
                    Footer = OxmlHelper.InchesToDxa<uint>(0.25),
                    Gutter = OxmlHelper.InchesToDxa<uint>(0)
                });

            word.MainDocumentPart.Document.Body.AppendChild(sectionProps);

            word.PackageProperties.Created = DateTimeOffset.Now.DateTime;
            word.PackageProperties.Creator = _authorName;
        }

        async Task<HtmlToOpenXml.IO.Resource> IWebRequest.FetchAsync(Uri requestUri, CancellationToken cancellationToken)
        {
            if (!requestUri.IsAbsoluteUri && requestUri.OriginalString.StartsWith("/"))
                requestUri = new Uri($"{_websiteUrl}/{requestUri.OriginalString}", UriKind.Absolute);

            Stream stream;

            try
            {
                stream = await StaticHttpClient.Client.GetStreamAsync(requestUri);
            }
            catch
            {
                stream = new FileStream(_defaultImagePath, FileMode.Open);
            }

            return new HtmlToOpenXml.IO.Resource
            {
                StatusCode = HttpStatusCode.OK,
                Content = stream
            };
        }

        bool IWebRequest.SupportsProtocol(string protocol) => true;
    }
}
