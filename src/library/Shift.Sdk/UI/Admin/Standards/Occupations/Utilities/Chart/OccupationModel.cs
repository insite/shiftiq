using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class OccupationModel : AssetModel
    {
        public OccupationModel(AssetModel model) : base(model)
        {
            Frameworks = new List<FrameworkModel>();
        }

        public OccupationModel(Guid id, string code, int number, Guid thumbprint, string title) : base(id, code, number, thumbprint, title)
        {
            Frameworks = new List<FrameworkModel>();
        }

        public List<FrameworkModel> Frameworks { get; set; }

        public Chart CreateChart()
        {
            var chart = new Chart();

            foreach (var framework in Frameworks)
            foreach (var area in framework.Areas)
            {
                if (string.IsNullOrEmpty(area.AssetCode))
                    continue;

                chart.AddLine(area.AssetCode, area.AssetTitle);
            }

            foreach (var framework in Frameworks)
            foreach (var area in framework.Areas)
            {
                if (string.IsNullOrEmpty(area.AssetCode))
                    continue;

                foreach (var competency in area.Competencies)
                {
                    if (!int.TryParse(competency.AssetCode, out var n))
                        continue;

                    var item = chart.AddCompetency(area.AssetCode, n, competency.AssetTitle);

                    if (framework.AssetTitle.Contains("Level 1"))
                    {
                        item.HasLevel1 = true;
                        item.Level1Url = "/ui/admin/standards/edit?id=" + competency.AssetThumbprint;
                    }

                    if (framework.AssetTitle.Contains("Level 2"))
                    {
                        item.HasLevel2 = true;
                        item.Level2Url = "/ui/admin/standards/edit?id=" + competency.AssetThumbprint;
                    }

                    if (framework.AssetTitle.Contains("Level 3"))
                    {
                        item.HasLevel3 = true;
                        item.Level3Url = "/ui/admin/standards/edit?id=" + competency.AssetThumbprint;
                    }

                    if (framework.AssetTitle.Contains("Level 4"))
                    {
                        item.HasLevel4 = true;
                        item.Level4Url = "/ui/admin/standards/edit?id=" + competency.AssetThumbprint;
                    }
                }
            }

            return chart;
        }
    }
}