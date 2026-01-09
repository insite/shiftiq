using System;
using System.Text;

using InSite.Application.Contents.Read;
using InSite.Domain.CourseObjects;

using Newtonsoft.Json;

namespace InSite.Persistence
{
    [Serializable]
    public class Course2PublicationModel
    {
        private IContentSearch _contentSearch;

        public DateTime Date { get; set; }
        public string Publisher { get; set; }
        public Guid OrganizationIdentifier { get; set; }

        public string AssetType { get; set; }
        public string AssetSubtype { get; set; }
        public int Number { get; set; }
        public string Title { get; set; }
        public int Version { get; set; }

        public string Command { get; set; }
        public int CountInserts { get; set; }
        public int CountUpdates { get; set; }
        public int CountDeletes { get; set; }
        public int ExecutionTime { get; set; }

        public Course Course { get; set; }

        public void Load(Course model, string publisher, string command, IContentSearch contentSearch)
        {
            Course = model;

            Date = DateTime.UtcNow;

            OrganizationIdentifier = Course.Organization;
            Publisher = publisher;
            Command = command;
            Number = Course.Asset;
            Title = Course.Content.Title.GetText();

            _contentSearch = contentSearch;
        }

        public string SerializeAsJson()
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            return JsonConvert.SerializeObject(this, Formatting.Indented, settings);
        }

        public string SerializeAsMarkdownOutline()
        {
            var sb = new StringBuilder();
            sb.Append("Tiers: Course, Unit, Module, Activity");

            sb.AppendLine().AppendLine();

            SerializeAsMarkdownOutline(sb);

            return sb.ToString();
        }

        private void SerializeAsMarkdownOutline(StringBuilder sb)
        {
            sb.Append('#');

            if (!string.IsNullOrEmpty(Course.Code))
                sb.Append(' ').Append(Course.Code).Append('.');

            sb.Append(' ').Append(Course.Content.Title.GetText());
            sb.AppendLine().AppendLine();

            foreach (var unit in Course.Units)
            {
                sb.Append("##");

                if (!string.IsNullOrEmpty(unit.Code))
                    sb.Append(' ').Append(unit.Code).Append('.');

                sb.Append(' ').Append(unit.Content.Title.GetText());
                sb.AppendLine().AppendLine();

                foreach (var module in unit.Modules)
                {
                    sb.Append("###");

                    if (!string.IsNullOrEmpty(module.Code))
                        sb.Append(' ').Append(module.Code).Append('.');

                    sb.Append(' ').Append(module.Content.Title.GetText());
                    sb.AppendLine().AppendLine();

                    foreach (var activity in module.Activities)
                    {
                        sb.Append("####");

                        if (!string.IsNullOrEmpty(activity.Code))
                            sb.Append(' ').Append(activity.Code).Append('.');

                        sb.Append(' ').Append(activity.Content.Title.GetText());
                        sb.Append(" [").Append(activity.Type).Append(']');

                        sb.AppendLine().AppendLine();

                        var content = _contentSearch.GetBlock(activity.Identifier);
                        if (content != null)
                        {
                            var summary = content.Summary?.Text.Default;
                            if (!string.IsNullOrEmpty(summary))
                                sb.Append("Summary:").AppendLine().Append(summary).AppendLine().AppendLine();

                            var bodyHtml = content.Body?.Html.Default;
                            if (!string.IsNullOrEmpty(bodyHtml))
                                sb.Append("BodyHtml:").AppendLine().Append(bodyHtml).AppendLine().AppendLine();

                            var bodyText = content.Body?.Text.Default;
                            if (!string.IsNullOrEmpty(bodyText))
                                sb.Append("BodyText:").AppendLine().Append(bodyText).AppendLine().AppendLine();
                        }
                    }
                }
            }
        }

        public string SerializeAsMarkdownShell()
        {
            var sb = new StringBuilder();
            sb.Append("Tiers: Course, Unit, Module, Activity");

            sb.AppendLine().AppendLine();

            SerializeAsMarkdownShell(sb);

            return sb.ToString();
        }

        private void SerializeAsMarkdownShell(StringBuilder sb)
        {
            sb.Append('#');
            sb.Append(' ').Append(Course.Content.Title.GetText() + " - Copy");
            sb.AppendLine().AppendLine();

            foreach (var unit in Course.Units)
            {
                sb.Append("##");
                sb.Append(' ').Append(unit.Content.Title.GetText());
                sb.AppendLine().AppendLine();

                foreach (var module in unit.Modules)
                {
                    sb.Append("###");
                    sb.Append(' ').Append(module.Content.Title.GetText());
                    sb.AppendLine().AppendLine();

                    foreach (var activity in module.Activities)
                    {
                        sb.Append("####");
                        sb.Append(' ').Append(activity.Content.Title.GetText());
                        sb.Append(" [").Append(activity.Type).Append(']');
                        sb.AppendLine().AppendLine();
                    }
                }
            }
        }
    }
}
