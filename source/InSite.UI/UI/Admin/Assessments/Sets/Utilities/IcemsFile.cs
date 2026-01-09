using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Linq;

using Shift.Common;

namespace InSite.Admin.Assessments.Sets.Utilities
{
    public class IcemsFile
    {
        #region Properties

        public int Version { get; private set; }

        public IReadOnlyList<IcemsQuestionGroup> Groups { get; private set; }

        #endregion

        #region Construction

        private IcemsFile()
        {
        }

        #endregion

        #region Methods (parse file)

        public static IcemsFile Read(Stream input)
        {
            XElement root;

            try
            {
                using (var reader = new StreamReader(input))
                    root = XElement.Load(reader);
            }
            catch (XmlException)
            {
                throw new ApplicationError("File has unsupported format");
            }

            if (root?.Element("tblExam")?.Elements("tblBlock") != null)
                return new IcemsFile
                {
                    Version = 1,
                    Groups = IcemsQuestionGroup.ReadV1(root.Element("tblExam").Elements("tblBlock"))
                };

            if (root?.Element("Exam")?.Elements("Section") != null)
                return new IcemsFile
                {
                    Version = 2,
                    Groups = IcemsQuestionGroup.ReadV2(root.Element("Exam").Elements("Section"))
                };

            throw new ApplicationError("File has unsupported format");
        }

        #endregion
    }
}