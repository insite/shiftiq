using System;
using System.Text;
using System.Web;

using InSite.Persistence;

namespace InSite.Admin.Assessments
{
    public class SnippetBuilder
    {
        public class StandardModel
        {
            public Guid Identifier { get; internal set; }
            public Guid Organization { get; internal set; }

            public string Code { get; internal set; }
            public string Title { get; internal set; }
            public string Label { get; internal set; }
            public string Name { get; internal set; }
            public string Type { get; internal set; }
            
            public int Number { get; internal set; }
            public int Sequence { get; internal set; }

            public StandardModel Parent { get; internal set; }
            
            public Guid? ParentIdentifier { get; set; }
            public string ParentCode { get; internal set; }
            public int? ParentSequence { get; internal set; }
        }

        public static string GetHtml(Guid standard, bool showParentCode = true)
        {
            StandardModel a = null;

            if (standard != Guid.Empty)
                a = StandardSearch.BindFirst(
                    x => new StandardModel { Code = x.Code, Title = x.ContentTitle, Label = x.StandardLabel, Name = x.ContentName, Number = x.AssetNumber, ParentCode = x.Parent.Code, Type = x.StandardType },
                    x => x.StandardIdentifier == standard);

            return GetHtml(a, showParentCode);
        }

        public static string GetHtml(StandardModel standard, bool showParentCode = true, bool showParent = false, bool showLabel = true, bool showLink = false, Guid? id = null)
        {
            if (standard == null)
                return "None";

            StringBuilder output = new StringBuilder();

            if (showParent && standard.Parent != null)
            {
                WriteParent(standard.Parent, output, showParentCode, showLabel, showLink);
                IndentStart(output);
            }

            WriteCode(standard, output, showParentCode);
            WriteTitle(standard, output);
            WriteLabel(standard, output, showLabel, showLink);

            if (showParent && standard.Parent != null)
            {
                IndentEnd(output);
            }

            return output.ToString();
        }

        private static void IndentStart(StringBuilder output)
        {
            output.Append($"<div class='mt-2 ms-3'>");
        }

        private static void IndentEnd(StringBuilder output)
        {
            output.Append($"</div>");
        }

        private static void WriteLabel(StandardModel standard, StringBuilder output, bool showLabel, bool showLink)
        {
            if (!showLabel)
                return;

            string label = HttpUtility.HtmlEncode(standard.Label ?? standard.Type);

            output.Append($"<div class='fs-xs text-body-secondary'><strong>{label}</strong> ");
            WriteAssetNumber(standard, output, showLink);
            output.Append("</div>");
        }

        private static void WriteCode(StandardModel standard, StringBuilder output, bool showParentCode)
        {
            string code = string.Empty;

            if (showParentCode)
            {
                if (!string.IsNullOrEmpty(standard.Parent?.Code))
                    code = HttpUtility.HtmlEncode(standard.Parent.Code + " ");
                else if (!string.IsNullOrEmpty(standard.ParentCode))
                    code = HttpUtility.HtmlEncode(standard.ParentCode + " ");
            }

            if (!string.IsNullOrEmpty(code) || !string.IsNullOrEmpty(standard.Code))
                output.Append($"{code}{HttpUtility.HtmlEncode(standard.Code)}. ");
        }

        private static void WriteTitle(StandardModel standard, StringBuilder output)
        {
            output.Append($"{standard.Title}");
        }

        private static void WriteParent(StandardModel standard, StringBuilder output, bool showParentCode, bool showLabel, bool showLink)
        {
            WriteCode(standard, output, showParentCode);
            WriteTitle(standard, output);
            WriteLabel(standard, output, showLabel, showLink);
        }

        private static void WriteAssetNumber(StandardModel standard, StringBuilder output, bool showLink)
        {
            if (showLink)
                output.Append($"<a class='ms-1 fs-sm' href=\"/ui/admin/standards/edit?id={standard.Identifier}\">Asset #{standard.Number}</a>");
            else
                output.Append($"Asset #{standard.Number}");
        }
    }
}