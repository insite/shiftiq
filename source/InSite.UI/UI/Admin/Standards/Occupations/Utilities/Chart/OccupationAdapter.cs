using System;
using System.Linq;

using InSite.Persistence;

using Shift.Sdk.UI;

using ContentLabel = Shift.Constant.ContentLabel;

namespace InSite.Admin.Standards.Occupations.Utilities.Chart
{
    public class OccupationAdapter
    {
        public static OccupationModel Load(string organizationCode, Guid thumbprint)
        {
            var asset = StandardSearch
                .BindFirst(
                    x => new { x.StandardIdentifier, x.Code, x.AssetNumber, Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title) },
                    x => x.Organization.OrganizationCode == organizationCode && x.StandardIdentifier == thumbprint
                );

            var model = new OccupationModel(new AssetModel(asset.StandardIdentifier, asset.Code, asset.AssetNumber, asset.StandardIdentifier, asset.Title));

            LoadFrameworks(model);

            return model;
        }

        private static void LoadFrameworks(OccupationModel model)
        {
            var assets = StandardSearch.Bind(
                x => new { x.StandardIdentifier, x.Code, x.AssetNumber, Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title), x.Sequence },
                x => x.ParentStandardIdentifier == model.AssetId || x.ChildContainments.Any(relationship => relationship.ParentStandardIdentifier == model.AssetId && relationship.ChildStandardIdentifier == x.StandardIdentifier),
                "Code"
            );

            foreach (var asset in assets)
            {
                var framework = new FrameworkModel(asset.StandardIdentifier, asset.Code, asset.AssetNumber, asset.StandardIdentifier, asset.Title);
                model.Frameworks.Add(framework);

                var children = StandardSearch.Bind(
                    x => new { x.StandardIdentifier, x.Code, x.AssetNumber, Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title), x.Sequence },
                    x => x.ParentStandardIdentifier == framework.AssetId,
                    "Code");

                foreach (var child in children)
                {
                    var area = new AreaModel(child.StandardIdentifier, child.Code, child.AssetNumber, child.StandardIdentifier, child.Title);
                    framework.Areas.Add(area);

                    var grandchildren = StandardSearch
                        .Bind(
                            x => new { x.StandardIdentifier, x.Code, x.AssetNumber, Title = CoreFunctions.GetContentTextEn(x.StandardIdentifier, ContentLabel.Title), x.Sequence },
                            x => x.ParentStandardIdentifier == child.StandardIdentifier,
                            "Code");

                    foreach (var grandchild in grandchildren)
                    {
                        var competency = new CompetencyModel(grandchild.StandardIdentifier, grandchild.Code, grandchild.AssetNumber, grandchild.StandardIdentifier, grandchild.Title);
                        area.Competencies.Add(competency);
                    }
                }
            }
        }
    }
}