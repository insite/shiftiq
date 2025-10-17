using System;

namespace Shift.Contract
{
    // TODO: Implement me using Gitbook integration
    public class HelpTopic
    {
        public Guid Identifier { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }

        public string GetBodyText()
        {
            return string.Empty;
        }

        public string GetSummaryText()
        {
            return string.Empty;
        }
    }

    public class HelpTopics
    {
        public static HelpTopic FindTopicByContentActionId(Guid actionIdentifier)
        {
            return null;
        }

        public static HelpTopic FindTopicByUrl(string helpUrl)
        {
            return null;
        }

        public static void Refresh()
        {

        }
    }
}