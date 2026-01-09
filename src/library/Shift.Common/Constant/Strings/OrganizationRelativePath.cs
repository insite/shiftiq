namespace Shift.Constant
{
    public static class OrganizationRelativePath
    {
        public const string AssetPathTemplate = "/Assets/{0}/"; // 0 = Entity.Number
        public const string ContactArchivePath = "/Archives/Contacts/";
        public const string ExamPathTemplate = "/exams/{0}/{1}/"; // 0 = Exam.Subtype (pluralized), 1 = Entity.ID
        public const string IecbcContactPathTemplate = "/Custom/IECBC/Contacts/{0}/"; // 0 = Entity.ContactKey
        public const string IecbcContactUploadsPathTemplate = "/Tenants/IECBC/Contacts/People/{0}/Attachments/"; // 0 = Entity.ContactIdentifier (Thumbprint)
        public const string JobsUploadsPathTemplate = "/Tenants/{0}/Jobs/People/{1}/Attachments/"; // 0 = OrganizationIdentifier, 1 = Entity.ContactIdentifier (Thumbprint)
        public const string MessageArchivePath = "/Archives/Messages/";
        public const string SurveyPathTemplate = "/surveys/{0}/{1}/"; // 0 = Survey.Subtype (pluralized), 1 = Entity.ID
        public const string UserPathTemplate = "/Users/{0}/"; // 0 = User.Email
        public const string GlossaryPathTemplate = "/glossaries/{0}/{1}/{2}"; // 0 = glossary ID, 1 = item type (pluralized), 2 = item ID
        public const string JournalSetupPathTemplate = "/journalsetups/{0}"; // 0 = journalSetup ID
        public const string JournalCommentCreatePathTemplate = "/journalsetups/{0}/{1}/comments/temp/{2}"; // 0 = journalSetup ID, 1 = user ID, 2 = unique ID
        public const string JournalCommentChangePathTemplate = "/journalsetups/{0}/{1}/comments/{2}"; // 0 = journalSetup ID, 1 = user ID, 2 = comment ID
        public const string JournalExperienceChangePathTemplate = "/experiences/{0}/{1}"; // 0 = experience ID, 1 = field type
        public const string JournalExperienceCreatePathTemplate = "/experiences/{0}/temp/{1}/{2}"; // 0 = journal ID, 1 = unique ID, 2 = field type
        public const string JournalExperienceCommentCreatePathTemplate = "/experiences/{0}/comments/temp/{1}"; // 0 = experience ID, 1 = unique ID
        public const string JournalExperienceCommentChangePathTemplate = "/experiences/{0}/comments/{1}"; // 0 = experience ID, 1 = comment ID
        public const string EventRegistrationsBuildMessagePathTemplate = "/events/registrations/emails/{0}/{1}"; // 0 = user ID, 1 = session ID
    }
}
