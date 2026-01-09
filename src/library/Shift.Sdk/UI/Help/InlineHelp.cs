namespace Shift.Sdk.UI.Help
{
    public class InlineHelp
    {
        public string InstructionsUrl { get; }

        public string LabelsUrl { get; }

        public InlineHelp(string instructionsUrl, string labelsUrl)
        {
            InstructionsUrl = instructionsUrl;

            LabelsUrl = labelsUrl;
        }

        public string GetInstruction(string locator)
        {
            var parser = new InlineHelpDocumentParser();

            var instructions = parser.ParseInstructions(InstructionsUrl);

            return instructions.GetHtml(locator);
        }

        public string GetLabel(string key, string language)
        {
            var parser = new InlineHelpDocumentParser();

            var lexicon = parser.ParseLexicon(LabelsUrl);

            return lexicon.GetDisplay(key, language);
        }
    }
}