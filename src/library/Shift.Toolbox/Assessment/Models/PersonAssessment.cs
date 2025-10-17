using System;
using System.Collections.Generic;
using System.Text;

namespace Shift.Toolbox.Assessments.Models
{
    public class PersonAssessment
    {
        public List<QuestionItem> QuestionItems { get; set; }
        public string OrganizationLogoPath { get; set; }
        public string AssessmentTitle { get; set; }
        public string PersonFullName { get; set; }
        public string PersonEmail { get; set; }
    }
}
