using System;
using System.Collections.Generic;
using System.Linq;

namespace Shift.Common
{
    public class ComponentHelper
    {
        public static string[] Recognized = new[] {
            "billing",
            "booking",
            "competency",
            "content",
            "directory",
            "evaluation",
            "integration",
            "internal",
            "learning",
            "messaging",
            "metadata",
            "orchestration",
            "progress",
            "reporting",
            "security",
            "setup",
            "timeline",
            "variant",
            "workflow",
            "workspace"
        };

        public static bool IsRecognized(string name)
        {
            return Recognized.Contains(name);
        }

        public static bool IsPrototype(string name)
        {
            return !IsRecognized(name);
        }

        public static string Resolve(string name)
        {
            if (IsRecognized(name))
                return name;

            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["achievements"] = "progress",
                ["assessments"] = "evaluation",
                ["assets"] = "content",
                ["assign-achievements"] = "progress",
                ["assign-certificates"] = "progress",
                ["assign-managers"] = "directory",
                ["assign-profiles"] = "competency",
                ["assign-supervisors"] = "directory",
                ["assign-validators"] = "directory",
                ["attachments"] = "content",
                ["attempts"] = "evaluation",
                ["banks"] = "evaluation",
                ["billing"] = "billing",
                ["certificate"] = "progress",
                ["classes"] = "booking",
                ["collections"] = "setup",
                ["colleges"] = "directory",
                ["comments"] = "content",
                ["contacts"] = "directory",
                ["contents"] = "content",
                ["courses"] = "learning",
                ["developers"] = "security",
                ["discounts"] = "billing",
                ["documents"] = "content",
                ["eula"] = "setup",
                ["events"] = "booking",
                ["exams"] = "evaluation",
                ["expire-achievements"] = "progress",
                ["expire-competencies"] = "competency",
                ["fields"] = "setup",
                ["forms"] = "workflow",
                ["foundations"] = "setup",
                ["gradebooks"] = "progress",
                ["identity"] = "security",
                ["integration"] = "integration",
                ["invoices"] = "billing",
                ["items"] = "setup",
                ["learning"] = "learning",
                ["logbooks"] = "progress",
                ["messages"] = "messaging",
                ["platform"] = "setup",
                ["products"] = "billing",
                ["questions"] = "evaluation",
                ["quizzes"] = "evaluation",
                ["records"] = "progress",
                ["reporting"] = "reporting",
                ["reports"] = "reporting",
                ["request-access"] = "security",
                ["reset-password"] = "security",
                ["sales"] = "billing",
                ["scores"] = "evaluation",
                ["sections"] = "evaluation",
                ["security"] = "security",
                ["sets"] = "evaluation",
                ["setup"] = "setup",
                ["signin"] = "security",
                ["signin-challenge"] = "security",
                ["signin-failure"] = "security",
                ["signin-mfa"] = "security",
                ["signin-social"] = "security",
                ["signin-success"] = "security",
                ["signout"] = "security",
                ["signout-completed"] = "security",
                ["sites"] = "workspace",
                ["specifications"] = "evaluation",
                ["standards"] = "competency",
                ["submissions"] = "workflow",
                ["subscribe"] = "messaging",
                ["support"] = "messaging",
                ["timeline"] = "timeline",
                ["validate-open-badge"] = "progress",
                ["verify-email"] = "directory",
                ["workflow"] = "workflow"
            };

            return map.TryGetValue(name, out var result) ? result : null;
        }
    }
}