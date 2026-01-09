using System;

namespace Shift.Constant
{
    public static class OrganizationIdentifiers
    {
        public static Guid Archive { get; private set; }
        public static Guid Global { get; private set; }
        public static Guid Demo { get; private set; }
        public static Guid Test { get; private set; }

        public static Guid CMDS = Guid.Parse("8258CB0A-D1E8-4BC1-94B3-E70652503437");
        public static Guid EHRC = Guid.Parse("2A2727CB-35F3-4AD7-B965-AB570012BD29");
        public static Guid Inspire = Guid.Parse("bcfe9d2e-228e-4a41-979f-aff80106215f");
        public static Guid NCSHA = Guid.Parse("CF500796-48F1-45C9-B6E3-5C26AAF127B0");
        public static Guid RCABC = Guid.Parse("9e1a1f8a-4652-490e-8577-ab5f0147b999");
        public static Guid SkilledTradesBC = Guid.Parse("EFB530A0-8B3C-448C-9FB4-DDF7602489A6");
        public static Guid SkillsCheck = Guid.Parse("947701F8-2AA7-4EBE-AEED-ACB40166736E");

        public static void Initialize(Common.Organizations organizations)
        {
            Archive = organizations.Archive;
            Global = organizations.Global;
            Demo = organizations.Demo;
            Test = organizations.Test;
        }
    }
}