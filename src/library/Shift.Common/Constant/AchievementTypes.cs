namespace Shift.Common
{
    public static class AchievementTypes
    {
        public static bool RenameModuleToLearningModule { get; set; }

        public const string AdditionalComplianceRequirement = "Additional Compliance Requirement";
        public const string CodeOfPractice = "Code of Practice";
        public const string Module = "Module";
        public const string HumanResourcesDocument = "Human Resources Document";
        public const string HumanResourcesModule = "HR Learning Module";
        public const string OtherAchievement = "Other Achievement";
        public const string Orientation = "Orientation";
        public const string SafeOperatingPractice = "Safe Operating Practice";
        public const string SiteSpecificOperatingProcedure = "Site-Specific Operating Procedure";
        public const string TimeSensitiveSafetyCertificate = "Time-Sensitive Safety Certificate";
        public const string TrainingGuide = "Training Guide";

        private static string[] _types = new[]
        {
            AdditionalComplianceRequirement,
            CodeOfPractice,
            Module,
            HumanResourcesDocument,
            HumanResourcesModule,
            OtherAchievement,
            Orientation,
            SafeOperatingPractice,
            SiteSpecificOperatingProcedure,
            TimeSensitiveSafetyCertificate,
            TrainingGuide
        };

        public static string[] Collect()
            => _types;

        public static string Pluralize(string type, string organization)
        {
            return Humanizer.Pluralize(Display(type, organization));
        }

        public static bool IsAchievementTypeIssuedByCourseCompletion(string achievementType)
        {
            return StringHelper.EqualsAny(achievementType, new[] { "HR Learning Module", "Module" });
        }

        public static string Display(string type, string organization)
        {
            if (type == "Other Achievement")
                return "Other Achievement";

            var display = type;

            if (StringHelper.Equals(organization, "keyera"))
            {
                if (StringHelper.Equals(type, "Code of Practice"))
                    display = "Corporate Safe Operating Practice";

                else if (StringHelper.Equals(type, "Safe Operating Practice"))
                    display = "Site Safe Operating Practice";

                else if (StringHelper.Equals(type, "Site-Specific Operating Procedure"))
                    display = "Operating Procedure";

                else if (StringHelper.Equals(type, "Training Guide"))
                    display = "Training Document";
            }

            if (StringHelper.Equals(organization, "sinopec"))
            {
                if (StringHelper.Equals(type, "Human Resources Document"))
                    display = "HSE Policies";
            }

            if (RenameModuleToLearningModule && StringHelper.Equals(type, "Module"))
                display = "e-Learning Module";

            return Humanizer.Pluralize(display);
        }

        public static string RetrieveColor(int index)
        {
            const string Black = "#000000";

            var colors = new string[]
            {
                "#4169E1", // Time-Sensitive Safety Certificates
                "#2A9D8F", // Additional Compliance Requirements
                "#F4A261", // Critical Competencies
                "#264653", // Non-Critical Competencies
                "#E76F51", // Codes of Practice
                "#8338EC", // Safe Operating Practices
                "#06FFA5", // Human Achievement Documents
                "#FFD60A", // e-Learning Modules
                "#0077B6", // Training Guides
                "#FF006E", // Site-Specific Operating Procedures
                "#06D6A0", // Orientation
                "#FB5607", // HR Learning Modules
            };

            if (0 <= index && index < colors.Length)
                return colors[index];

            return Black;
        }

        public static string RetrieveMarker(int index)
        {
            const string Dot = "Dot";

            var markers = new string[]
            {
                "Circle",   // Time-Sensitive Safety Certificates
                "Star",     // Additional Compliance Requirements
                "Diamond",  // Critical Competencies
                "Dot",      // Non-Critical Competencies
                "X",        // Codes of Practice
                "Plus",     // Safe Operating Practices
                "Triangle", // Human Achievement Documents
                "Dash",     // e-Learning Modules
                "Square",   // Training Guides
                "Picture",  // Site-Specific Operating Procedures
                "Diamond",  // Orientation
                "Dash",     // HR Learning Modules
            };

            if (0 <= index && index < markers.Length)
                return markers[index];

            return Dot;
        }
    }
}