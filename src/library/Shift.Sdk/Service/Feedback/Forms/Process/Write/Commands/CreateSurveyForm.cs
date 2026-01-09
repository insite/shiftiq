using System;

using Shift.Common.Timeline.Commands;

using Shift.Constant;

namespace InSite.Application.Surveys.Write
{
    public class CreateSurveyForm : Command
    {
        public CreateSurveyForm(Guid form, string source, Guid tenant, int asset, string name, SurveyFormStatus status, string language)
        {
            AggregateIdentifier = form;

            Source = source;
            Tenant = tenant;

            Asset = asset;
            Name = name;
            Status = status;
            Language = language;
        }

        public string Source { get; set; }
        public Guid Tenant { get; set; }

        public int Asset { get; }
        public string Name { get; }
        public SurveyFormStatus Status { get; }
        public string Language { get; }
    }

    public class ChangeSurveyFormAsset : Command
    {
        public ChangeSurveyFormAsset(Guid form, int asset)
        {
            AggregateIdentifier = form;
            Asset = asset;
        }

        public int Asset { get; }
    }
}