using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace Shift.Sdk.UI.Help
{
    public class Instructions
    {
        public List<MarkdownSection> Sections { get; set; }

        public Instructions()
        {
            Sections = new List<MarkdownSection>();
        }

        internal string GetHtml(string locator)
        {
            var section = Sections.FirstOrDefault(x => StringHelper.Equals(locator, x.Path));

            if (section != null)
                return Markdown.ToHtml(section.Body);

            return null;
        }
    }
}