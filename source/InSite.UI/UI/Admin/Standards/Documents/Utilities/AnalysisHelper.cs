using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using InSite.Persistence;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.Admin.Standards.Documents.Utilities
{
    public static class AnalysisHelper
    {
        #region Classes (events)

        public class UpdateEventArgs<T> : EventArgs
        {
            public T Data { get; }

            public UpdateEventArgs(T data)
            {
                Data = data;
            }
        }

        public delegate void UpdateEventHandler<T>(object sender, UpdateEventArgs<T> e);

        #endregion

        #region Classes (data)

        [Serializable]
        public class StandardInfo
        {
            public Guid StandardIdentifier { get; set; }
            public Guid? ParentStandardIdentifier { get; set; }

            public int Sequence { get; set; }
            public string Language { get; set; } = "en";

            public Dictionary<string, string> Titles => _titles;
            private Dictionary<string, string> _titles { get; set; }

            public string Title
            {
                get
                {
                    GetTitles();
                    if (_titles.TryGetValue(Language, out var result))
                        return result;
                    if (_titles.TryGetValue("en", out result))
                        return result;
                    return "";
                }
            }

            public StandardInfo Root => Parent == null ? this : Parent.Root;
            public StandardInfo Parent { get; set; }
            public List<StandardInfo> Children { get; } = new List<StandardInfo>();

            public static readonly Expression<Func<Standard, StandardInfo>> Binder = LinqExtensions1.Expr((Standard standard) => new StandardInfo
            {
                StandardIdentifier = standard.StandardIdentifier,
                ParentStandardIdentifier = standard.ParentStandardIdentifier,
                Sequence = standard.Sequence
            });

            public StandardInfo()
            {

            }

            public StandardInfo(Standard entity)
                : this()
            {
                StandardIdentifier = entity.StandardIdentifier;
                Sequence = entity.Sequence;
                GetTitles();
            }

            private void GetTitles()
            {
                if (_titles == null)
                    _titles = ServiceLocator.ContentSearch.GetTitles(StandardIdentifier);
            }
        }

        [Serializable]
        public class ReportDataJobFit1
        {
            public string Title { get; set; }
            public double OverlapValue { get; set; }
            public ReportDataGac[] GacsObtain { get; set; }
            public ReportDataGac[] GacsOverlap { get; set; }
        }

        [Serializable]
        public class ReportDataGac
        {
            public string Title { get; internal set; }
            public StandardInfo[] Competencies { get; internal set; }
        }

        [Serializable]
        public class ReportDataJobFit2
        {
            public string Title { get; set; }
            public ReportDataJobFit1[] NosReports { get; set; }
        }

        [Serializable]
        public class ReportDataCareerMap
        {
            public string Title { get; set; }
            public double? Overlap { get; set; }
            public ReportDataGac[] SharedCompetencies { get; set; }
            public ReportDataGac[] MissingCompetencies { get; set; }
        }

        public interface IReportDataStandardAnalysis
        {
            string Title { get; }
        }

        [Serializable]
        public class ReportDataStandardAnalysis1 : IReportDataStandardAnalysis
        {
            public string Title { get; set; }
            public double? Overlap { get; set; }
            public ReportDataGac[] Shared { get; set; }
            public ReportDataGac[] Missing { get; set; }
        }

        [Serializable]
        public class ReportDataStandardAnalysis2 : IReportDataStandardAnalysis
        {
            public string Title { get; set; }
            public ReportDataStandardAnalysis1[] Reports { get; set; }
        }

        [Serializable]
        public class ReportDataStandardAnalysis3 : IReportDataStandardAnalysis
        {
            public string Title { get; set; }
            public StandardInfo[] Standards { get; set; }
        }

        #endregion

        #region Methods (select data)

        public static StandardInfo[] GetDocumentGacs(Guid nosKey, string lang)
        {
            var data = StandardContainmentSearch.Bind(
                LinqExtensions1.Expr((StandardContainment x) => new
                {
                    Framework = StandardInfo.Binder.Invoke(x.Child.Parent.Parent),
                    Gac = StandardInfo.Binder.Invoke(x.Child.Parent),
                    Competency = StandardInfo.Binder.Invoke(x.Child)
                }).Expand(),
                x => x.ParentStandardIdentifier == nosKey
                  && x.Child.StandardType == StandardType.Competency
                  && x.Child.ParentStandardIdentifier.HasValue
                  && x.Child.Parent.ParentStandardIdentifier.HasValue);

            var gacs = new List<StandardInfo>();

            foreach (var frameworkGroup in data.GroupBy(x => x.Framework.StandardIdentifier))
            {
                var frameworkInfo = frameworkGroup.First().Framework;
                frameworkInfo.Language = lang;

                foreach (var gacGroup in frameworkGroup.GroupBy(x => x.Gac.StandardIdentifier))
                {
                    var gacInfo = gacGroup.First().Gac;
                    gacInfo.Parent = frameworkInfo;
                    gacInfo.Language = lang;
                    frameworkInfo.Children.Add(gacInfo);

                    foreach (var competencyItem in gacGroup.OrderBy(x => x.Competency.Sequence))
                    {
                        var competencyInfo = competencyItem.Competency;
                        competencyInfo.Parent = gacInfo;
                        competencyInfo.Language = lang;
                        gacInfo.Children.Add(competencyInfo);
                    }

                    gacs.Add(gacInfo);
                }
            }

            return gacs.OrderBy(x => x.Parent.Title).ThenBy(x => x.Sequence).ToArray();
        }

        public static StandardInfo[] GetProfileCompetencies(Guid profileKey)
        {
            var containments = StandardContainmentSearch.SelectTree(new[] { profileKey });
            var allKeys = containments.Select(x => x.ChildStandardIdentifier).Distinct().ToArray();

            return StandardSearch.Bind(
                StandardInfo.Binder,
                x => allKeys.Contains(x.StandardIdentifier) && x.StandardType == StandardType.Competency);
        }

        public static StandardInfo[] GetStandardAnalysisGacs(Guid key, string lang)
        {
            var containments = StandardContainmentSearch.SelectTree(new[] { key });
            var allKeys = containments.Select(x => x.ChildStandardIdentifier).Append(key).Distinct().ToArray();
            var data = StandardSearch.Bind(
                LinqExtensions1.Expr((Standard x) => new
                {
                    Framework = StandardInfo.Binder.Invoke(x.Parent.Parent),
                    Gac = StandardInfo.Binder.Invoke(x.Parent),
                    Competency = StandardInfo.Binder.Invoke(x)
                }).Expand(),
                x => allKeys.Contains(x.StandardIdentifier)
                  && x.StandardType == StandardType.Competency
                  && x.ParentStandardIdentifier.HasValue
                  && x.Parent.ParentStandardIdentifier.HasValue);

            var gacs = new List<StandardInfo>();

            foreach (var frameworkGroup in data.GroupBy(x => x.Framework.StandardIdentifier))
            {
                var frameworkInfo = frameworkGroup.First().Framework;
                frameworkInfo.Language = lang;

                foreach (var gacGroup in frameworkGroup.GroupBy(x => x.Gac.StandardIdentifier))
                {
                    var gacInfo = gacGroup.First().Gac;
                    gacInfo.Parent = frameworkInfo;
                    gacInfo.Language = lang;
                    frameworkInfo.Children.Add(gacInfo);

                    foreach (var competencyItem in gacGroup.OrderBy(x => x.Competency.Sequence))
                    {
                        var competencyInfo = competencyItem.Competency;
                        competencyInfo.Parent = gacInfo;
                        competencyInfo.Language = lang;
                        gacInfo.Children.Add(competencyInfo);
                    }

                    gacs.Add(gacInfo);
                }
            }

            return gacs.OrderBy(x => x.Parent.Title).ThenBy(x => x.Sequence).ToArray();
        }

        #endregion

        #region Methods (report)

        public static ReportDataJobFit1 GetReportDataJobFit1(
            Standard nos,
            IEnumerable<StandardInfo> nosGacs,
            HashSet<Guid> competencies,
            string language,
            Func<string, string> translate
            )
        {
            var nosCompetencies = nosGacs.SelectMany(x => x.Children).ToArray();

            var overlapCount = nosCompetencies
                .Where(x => competencies.Contains(x.StandardIdentifier))
                .Count();

            var obtainGacs = new List<ReportDataGac>();
            var overlapGacs = new List<ReportDataGac>();

            foreach (var gac in nosGacs)
            {
                var obtainCompetencies = new List<StandardInfo>();
                var overlapCompetencies = new List<StandardInfo>();

                foreach (var competency in gac.Children)
                {
                    if (competencies.Contains(competency.StandardIdentifier))
                        overlapCompetencies.Add(competency);
                    else
                        obtainCompetencies.Add(competency);
                }

                if (obtainCompetencies.Count > 0)
                    obtainGacs.Add(new AnalysisHelper.ReportDataGac
                    {
                        Title = gac.Title,
                        Competencies = obtainCompetencies.ToArray()
                    });

                if (overlapCompetencies.Count > 0)
                    overlapGacs.Add(new AnalysisHelper.ReportDataGac
                    {
                        Title = gac.Title,
                        Competencies = overlapCompetencies.ToArray()
                    });
            }

            var nosTitle = ServiceLocator.ContentSearch.GetTitleText(nos.StandardIdentifier, language);
            var titleTemplate = translate("Job Fit Analysis for {0}");
            var title = string.Format(titleTemplate, nosTitle);

            return new ReportDataJobFit1
            {
                Title = title,
                OverlapValue = overlapCount == 0 ? 0d : ((double)overlapCount / nosCompetencies.Length),
                GacsObtain = obtainGacs.ToArray(),
                GacsOverlap = overlapGacs.ToArray()
            };
        }

        #endregion

        #region Methods (word)

        public static byte[] CreateWordDocument(Action<WordprocessingDocument, int> render)
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

                    var abstractNumId = -1;

                    {
                        OxmlHelper.MicrosoftWordStyles.SetupDefault(word.MainDocumentPart);

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

                        abstractNumId = newAbstractNum.AbstractNumberId;
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

                    render(word, abstractNumId);

                    word.PackageProperties.Created = DateTimeOffset.Now.DateTime;
                    word.PackageProperties.Creator = CurrentSessionState.Identity.User.FullName;

                    word.Save();
                }

                return stream.ToArray();
            }
        }

        public static void RenderGacs(WordprocessingDocument word, int abstractNumId, IEnumerable<ReportDataGac> gacs)
        {
            var newNumInstance = OxmlHelper.InsertNumInstance(
                word.MainDocumentPart,
                new NumberingInstance(new AbstractNumId { Val = abstractNumId }));

            foreach (var gac in gacs)
            {
                word.MainDocumentPart.Document.Body.AppendChild(
                    new Paragraph(
                        new ParagraphProperties(
                            new ParagraphStyleId { Val = "ListParagraph" },
                            new NumberingProperties(
                                new NumberingLevelReference { Val = 0 },
                                new NumberingId { Val = newNumInstance.NumberID })),
                        new Run(
                            new RunProperties(
                                new Bold(),
                                new BoldComplexScript()),
                            new Text { Text = gac.Title })));

                foreach (var competency in gac.Competencies)
                {
                    word.MainDocumentPart.Document.Body.AppendChild(
                        new Paragraph(
                            new ParagraphProperties(
                                new ParagraphStyleId { Val = "ListParagraph" },
                                new NumberingProperties(
                                    new NumberingLevelReference { Val = 1 },
                                    new NumberingId { Val = newNumInstance.NumberID })),
                            new Run(new Text { Text = competency.Title })));
                }
            }
        }

        #endregion
    }
}