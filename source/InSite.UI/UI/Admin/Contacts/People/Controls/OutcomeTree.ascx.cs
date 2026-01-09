using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Persistence;

namespace InSite.UI.Admin.Contacts.People.Controls
{
    public partial class OutcomeTree : UserControl
    {
        private class StandardItem
        {
            public Guid StandardIdentifier { get; set; }
            public string StandardName { get; set; }
            public int Sequence { get; set; }
            public decimal? Points { get; set; }

            public List<StandardItem> Children { get; set; }

            public bool Visible
            {
                get
                {
                    return Assigned || Children != null && Children.Any(x => x.Visible);
                }
            }

            public bool Assigned { get; set; }
        }

        private class GradebookItem
        {
            public Guid GradebookIdentifier { get; set; }
            public string GradebookName { get; set; }
            public Guid? AchievementIdentifier { get; set; }
            public string AchievementName { get; set; }

            public List<StandardItem> Standards { get; set; }
        }

        private class StandardPlainItem
        {
            public Guid GradebookIdentifier { get; }
            public Guid StandardIdentifier { get; }
            public string StandardName { get; }
            public decimal? Points { get; }
            public int Level { get; }
            public bool Assigned { get; }

            public StandardPlainItem(StandardItem original, Guid gradebookIdentifier, int level)
            {
                StandardIdentifier = original.StandardIdentifier;
                StandardName = original.StandardName;
                Points = original.Points;
                GradebookIdentifier = gradebookIdentifier;
                Level = level;
                Assigned = original.Assigned;
            }
        }

        private class GradebookPlainItem
        {
            public Guid GradebookIdentifier { get; }
            public string GradebookName { get; }
            public Guid? AchievementIdentifier { get; }
            public string AchievementName { get; }
            public List<StandardPlainItem> Standards { get; }

            public GradebookPlainItem(GradebookItem original)
            {
                GradebookIdentifier = original.GradebookIdentifier;
                GradebookName = original.GradebookName;
                AchievementIdentifier = original.AchievementIdentifier;
                AchievementName = original.AchievementName;
                Standards = new List<StandardPlainItem>();
            }
        }

        private Guid UserIdentifier { get; set; }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            GradebookRepeater.ItemDataBound += GradebookRepeater_ItemDataBound;
        }

        private void GradebookRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var gradebook = (GradebookPlainItem)e.Item.DataItem;

            var standardRepeater = (Repeater)e.Item.FindControl("StandardRepeater");
            standardRepeater.DataSource = gradebook.Standards;
            standardRepeater.DataBind();
        }

        public void LoadData(Guid userIdentifier)
        {
            UserIdentifier = userIdentifier;

            var gradebooks = ReadOutcomes(userIdentifier);

            GradebookRepeater.DataSource = gradebooks;
            GradebookRepeater.DataBind();

            GradebookRepeater.Visible = gradebooks.Count > 0;
            NoOutcomes.Visible = gradebooks.Count == 0;
        }

        private static List<GradebookPlainItem> ReadOutcomes(Guid userIdentifier)
        {
            var gradebooks = new List<GradebookItem>();
            var map = new Dictionary<Tuple<Guid, Guid>, StandardItem>();

            var filter = new QGradebookCompetencyValidationFilter
            {
                OrganizationIdentifier = CurrentSessionState.Identity.Organization.OrganizationIdentifier,
                UserIdentifier = userIdentifier
            };

            var validations = ServiceLocator.RecordSearch.GetValidations(filter);
            foreach (var validation in validations)
            {
                var standardItem = GetStandardItem(validation.GradebookIdentifier, validation.CompetencyIdentifier, gradebooks, map);
                standardItem.Points = validation.ValidationPoints;
                standardItem.Assigned = true;
            }

            var result = new List<GradebookPlainItem>();

            gradebooks.Sort((a, b) => string.Compare(a.GradebookName, b.GradebookName));
            foreach (var gradebook in gradebooks)
            {
                var gradebookPlain = new GradebookPlainItem(gradebook);
                result.Add(gradebookPlain);

                AddStandardPlainItems(gradebookPlain, gradebook.Standards, 1);
            }

            return result;
        }

        private static void AddStandardPlainItems(GradebookPlainItem gradebook, List<StandardItem> standards, int level)
        {
            if (standards == null)
                return;

            var sorted = standards
                .OrderBy(x => x.Sequence)
                .ThenBy(x => x.StandardName)
                .ToList();

            foreach (var standard in sorted)
            {
                if (standard.Visible)
                {
                    gradebook.Standards.Add(new StandardPlainItem(standard, gradebook.GradebookIdentifier, level));

                    AddStandardPlainItems(gradebook, standard.Children, level + 1);
                }
            }
        }

        private static StandardItem GetStandardItem(
            Guid gradebookIdentifier,
            Guid standardIdentifier,
            List<GradebookItem> gradebooks,
            Dictionary<Tuple<Guid, Guid>, StandardItem> map
            )
        {
            var key = new Tuple<Guid, Guid>(gradebookIdentifier, standardIdentifier);
            if (map.TryGetValue(key, out StandardItem standardItem))
                return standardItem;

            var standard = StandardSearch.SelectStandardHierarchy(standardIdentifier);
            var standards = standard.RootStandardIdentifier.HasValue
                ? StandardSearch.SelectStandardHierarchyList(standard.RootStandardIdentifier.Value).OrderBy(x => x.Depth).ToList()
                : new List<StandardHierarchy> { standard };

            var gradebook = gradebooks.Find(x => x.GradebookIdentifier == gradebookIdentifier);
            if (gradebook == null)
            {
                gradebook = CreateGradebookItem(gradebookIdentifier);
                gradebooks.Add(gradebook);
            }

            AddGradebookStandards(gradebook, standards, map);

            return map[key];
        }

        private static void AddGradebookStandards(GradebookItem gradebook, List<StandardHierarchy> standards, Dictionary<Tuple<Guid, Guid>, StandardItem> map)
        {
            foreach (var current in standards)
            {
                var currentKey = new Tuple<Guid, Guid>(gradebook.GradebookIdentifier, current.StandardIdentifier.Value);
                var currentItem = new StandardItem
                {
                    StandardIdentifier = current.StandardIdentifier.Value,
                    StandardName = current.Title,
                    Sequence = current.Sequence ?? 0
                };

                if (current.ParentStandardIdentifier.HasValue)
                {
                    var parentKey = new Tuple<Guid, Guid>(gradebook.GradebookIdentifier, current.ParentStandardIdentifier.Value);
                    var parentItem = map[parentKey];
                    if (parentItem.Children == null)
                        parentItem.Children = new List<StandardItem>();

                    parentItem.Children.Add(currentItem);
                }
                else
                {
                    gradebook.Standards.Add(currentItem);
                }

                map.Add(currentKey, currentItem);
            }
        }

        private static GradebookItem CreateGradebookItem(Guid gradebookIdentifier)
        {
            var query = ServiceLocator.RecordSearch.GetGradebook(gradebookIdentifier, x => x.Achievement);

            return new GradebookItem
            {
                GradebookIdentifier = gradebookIdentifier,
                GradebookName = query.GradebookTitle,
                AchievementIdentifier = query.AchievementIdentifier,
                AchievementName = query.Achievement?.AchievementTitle,
                Standards = new List<StandardItem>()
            };
        }

        protected string GetEditUrl(object dataItem)
        {
            var item = (StandardPlainItem)dataItem;

            return new ReturnUrl().GetRedirectUrl(
                $"/ui/admin/records/outcomes/change?gradebook={item.GradebookIdentifier}&competency={item.StandardIdentifier}&user={UserIdentifier}",
                "panel=outcomes"
            );
        }
    }
}