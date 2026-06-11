using System;

namespace Shift.Contract
{
    public class WorkshopQuestionData
    {
        public int TotalQuestionCount { get; set; }
        public WorkshopItem[] Sections { get; set; }
        public WorkshopItem[] Taxonomies { get; set; }
        public Guid FirstSectionId { get; set; }
        public Guid? FirstSectionAreaId { get; set; }
        public WorkshopStandard[] FirstSectionStandards { get; set; }
        public WorkshopQuestion[] FirstSectionQuestions { get; set; }
    }
}
