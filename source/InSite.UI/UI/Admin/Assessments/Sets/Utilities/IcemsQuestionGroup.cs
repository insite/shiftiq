using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class IcemsQuestionGroup
    {
        #region Properties

        public string Label { get; private set; }
        public string Name { get; private set; }

        public IReadOnlyList<IcemsQuestion> Questions { get; private set; }

        #endregion

        #region Fields

        private List<IcemsQuestion> _questions = new List<IcemsQuestion>();

        #endregion

        #region Construction

        private IcemsQuestionGroup()
        {

        }

        #endregion

        #region Methods (reading)

        public static IcemsQuestionGroup[] ReadV1(IEnumerable<XElement> blocks)
        {
            return blocks.Select(x => new IcemsQuestionGroup
            {
                Label = x.Element("BLCK_LABEL").Value,
                Name = x.Element("BLCK_NAME_ENG").Value,
                Questions = IcemsQuestion.ReadV1(x.Elements("tblItem"))
            }).ToArray();
        }

        public static IcemsQuestionGroup[] ReadV2(IEnumerable<XElement> sections)
        {
            return sections.Select((x, i) => new IcemsQuestionGroup
            {
                Label = Calculator.ToBase26(i + 1),
                Name = x.Attribute("Title").Value,
                Questions = IcemsQuestion.ReadV2(x.Elements("Question"))
            }).ToArray();
        }

        #endregion
    }
}