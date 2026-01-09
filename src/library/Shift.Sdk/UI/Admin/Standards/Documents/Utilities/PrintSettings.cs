using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class PrintSettings : Shift.Common.ISearchReport
    {
        public Guid? Identifier { get; set; }
        public string Name { get; set; }

        public string Language { get; set; } = "en";
        public string CompetencyPosition { get; set; }
        public int? CompetencyDepthFrom { get; set; }
        public int? CompetencyDepthThru { get; set; }
        public string FooterText { get; set; }
        public string[] CompetencyFields { get; set; }
        public string[] CompetencySettings { get; set; }
        public bool IsShowFieldHeading { get; set; }
        public bool IsOrderedList { get; set; }
        public bool IsBulletedList { get; set; }
        public bool IsPrintAsChecklist { get; set; }
        public bool IsRenderPageNumbers { get; set; }
        public bool IsRenderToc { get; set; }
        public bool IsRenderPageBreaks { get; set; }
    }
}