namespace Shift.Contract
{
    public class WorkshopQuestionOption
    {
        public class OptionColumn
        {
            public string TextMarkdown { get; set; }
            public string TextHtml { get; set; }            
        }

        public int Number { get; set; }
        public string Letter { get; set; }
        public string TitleMarkdown { get; set; }
        public string TitleHtml { get; set; }
        public int Points { get; set; }
        public bool? IsTrue { get; set; }
        public OptionColumn[] Columns { get; set; }
    }
}
