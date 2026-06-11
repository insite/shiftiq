using System;
using System.Collections.Generic;

namespace InSite.UI.Admin.Assessments.Attempts.Utilities.TakerReport
{
    [Serializable]
    public class PersonRow
    {
        public string PersonCode { get; set; }
        public string CaseNumber { get; set; }
        public string CaseId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Birthdate { get; set; }
        public string ExamDate { get; set; }
        public string ExamLanguage { get; set; }
        public List<PersonFramework> Frameworks { get; set; }
    }
}