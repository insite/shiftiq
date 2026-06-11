using QuestPDF.Infrastructure;

namespace Shift.Toolbox
{
    public static class InitQuestPDF
    {
        public static void Run()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }
    }
}
