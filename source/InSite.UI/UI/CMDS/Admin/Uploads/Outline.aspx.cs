using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

using InSite.Cmds.Admin.Uploads.Controls;
using InSite.Cmds.Infrastructure;
using InSite.Persistence;
using InSite.Persistence.Plugin.CMDS;
using InSite.UI.Layout.Admin;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Cmds.Actions.Contact.Company.Achievement
{
    public partial class Outline : AdminBasePage, ICmdsUserControl
    {
        #region Constants

        private const int ListCount = 2;

        #endregion

        #region Properties

        private Guid OrganizationIdentifier => CurrentIdentityFactory.ActiveOrganizationIdentifier;

        private int SectionCount
        {
            get => (int?)ViewState[nameof(SectionCount)] ?? 0;
            set => ViewState[nameof(SectionCount)] = value;
        }

        #endregion

        #region Initialization & Loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SectionBodyRepeater.ItemDataBound += SectionBodyRepeater_ItemDataBound;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        #endregion

        #region Load data

        private void SectionBodyRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            var section = (UploadSection)e.Item.FindControl("Section");
            var category = (UploadSection.AchievementCategoryItem)e.Item.DataItem;

            section.LoadData(category);
        }

        private void LoadData()
        {
            var organization = OrganizationSearch.Select(OrganizationIdentifier);

            PageHelper.AutoBindHeader(this, null, $"View Organization-Specific Resources ({organization.CompanyName})");

            var excludeElm = true;
            var excludeTssc = true;

            var uploads =
                UploadRepository.SelectOrganizationAchievements(OrganizationIdentifier, Request["achievementType"], excludeElm, excludeTssc);

            NoDownloadsPanel.Visible = uploads.Rows.Count == 0;
            MainSection.Visible = uploads.Rows.Count > 0;

            if (uploads.Rows.Count == 0)
                return;

            var categories = GetAchievementCategories(uploads);

            SectionRepeater.DataSource = categories;
            SectionRepeater.DataBind();

            SectionBodyRepeater.DataSource = categories;
            SectionBodyRepeater.DataBind();

            SectionCount = categories.Count;
        }

        private List<UploadSection.AchievementCategoryItem> GetAchievementCategories(DataTable uploads)
        {
            var hash = new HashSet<Guid>();
            List<UploadSection.AchievementCategoryItem> categories = new List<UploadSection.AchievementCategoryItem>();

            string categoryTitle = null;
            List<UploadSection.AchievementTypeItem> types = null;

            var labels = ServiceLocator.AchievementSearch.GetAchievementLabels(Organization.Identifier);

            foreach (DataRow uploadRow in uploads.Rows)
            {
                var curCategoryTitle = uploadRow["CategoryTitle"] as string;

                if (curCategoryTitle != null)
                    hash.Add((Guid)uploadRow["AchievementIdentifier"]);

                if (types == null || !StringHelper.Equals(categoryTitle, curCategoryTitle))
                {
                    if (types != null)
                        AddAchievementCategory(categoryTitle, types, categories);

                    categoryTitle = curCategoryTitle;
                    types = new List<UploadSection.AchievementTypeItem>();
                }

                AddAchievementType(uploadRow, types, labels);
            }

            AddAchievementCategory(categoryTitle, types, categories);

            var list = new List<UploadSection.AchievementCategoryItem>();
            for (int i = 0; i < categories.Count; i++)
            {
                var category = categories[i];

                var itemCategory = new UploadSection.AchievementCategoryItem
                {
                    Title = category.Title
                };

                foreach (List<UploadSection.AchievementTypeItem> typeList in category.ListOfTypes)
                {
                    var itemTypeList = new List<UploadSection.AchievementTypeItem>();

                    foreach (UploadSection.AchievementTypeItem type in typeList)
                    {
                        var itemType = new UploadSection.AchievementTypeItem
                        {
                            TitleDisplay = type.TitleDisplay,
                            TitleValue = type.TitleValue
                        };

                        foreach (UploadSection.AchievementItem achievement in type.Achievements)
                        {
                            var itemAchievement = new UploadSection.AchievementItem
                            {
                                ID = achievement.ID,
                                Number = achievement.Number,
                                Title = achievement.Title
                            };

                            if (itemCategory.Title == null && hash.Contains(achievement.ID))
                                continue;

                            foreach (UploadSection.DownloadItem download in achievement.Downloads)
                            {
                                var itemDownload = new UploadSection.DownloadItem
                                {
                                    ID = download.ID,
                                    Title = download.Title,
                                    SizeInKilobytes = download.SizeInKilobytes,
                                    Url = download.Url,
                                    Competencies = download.Competencies
                                };

                                itemAchievement.Downloads.Add(itemDownload);
                            }

                            if (itemAchievement.Downloads.Count > 0)
                                itemType.Achievements.Add(itemAchievement);
                        }

                        if (itemType.Achievements.Count > 0)
                            itemTypeList.Add(itemType);
                    }

                    if (itemTypeList.Count > 0)
                        itemCategory.ListOfTypes.Add(itemTypeList);
                }

                if (itemCategory.ListOfTypes.Count > 0)
                    list.Add(itemCategory);
            }
            return list;
        }

        private void AddAchievementCategory(string categoryTitle, List<UploadSection.AchievementTypeItem> achievementTypes,
            List<UploadSection.AchievementCategoryItem> categories)
        {
            categories.Add(new UploadSection.AchievementCategoryItem
            {
                Title = categoryTitle,
                ListOfTypes = SplitAchievementTypes(achievementTypes, ListCount)
            });
        }

        private void AddAchievementType(DataRow uploadRow, List<UploadSection.AchievementTypeItem> achievementTypes, string[] achievementLabels)
        {
            var typeName = uploadRow["AchievementLabel"] as string;
            var achievementType = achievementTypes.Count != 0 ? achievementTypes[achievementTypes.Count - 1] : null;
            var achievementTypeMapping = Custom.CMDS.Common.Controls.Server.AchievementTypeSelector.CreateAchievementLabelMapping(Organization.Code, achievementLabels);

            if (achievementType == null || !StringHelper.Equals(achievementType.TitleValue, typeName))
                achievementTypes.Add(achievementType = new UploadSection.AchievementTypeItem
                {
                    TitleValue = typeName,
                    TitleDisplay = achievementTypeMapping.GetOrDefault(typeName, typeName),
                    Achievements = new List<UploadSection.AchievementItem>()
                });

            AddAchievementToType(uploadRow, achievementType);
        }

        private void AddAchievementToType(DataRow uploadRow, UploadSection.AchievementTypeItem achievementType)
        {
            var achievementIdentifier = (Guid)uploadRow["AchievementIdentifier"];

            var achievement = achievementType.Achievements.Count > 0
                ? achievementType.Achievements[achievementType.Achievements.Count - 1]
                : null;

            if (achievement == null || achievement.ID != achievementIdentifier)
                achievementType.Achievements.Add(achievement = new UploadSection.AchievementItem
                {
                    ID = achievementIdentifier,
                    Number = uploadRow["ResourceNumber"] as string,
                    Title = uploadRow["AchievementTitle"] as string,
                    Downloads = new List<UploadSection.DownloadItem>()
                });

            AddUploadToAchievement(uploadRow, achievement);
        }

        private void AddUploadToAchievement(DataRow uploadRow, UploadSection.AchievementItem achievement)
        {
            if (uploadRow["UploadIdentifier"] == DBNull.Value)
                return;

            var uploadId = (Guid)uploadRow["UploadIdentifier"];
            var containerId = (Guid)uploadRow["UploadContainerIdentifier"];

            var download = achievement.Downloads.Count != 0 ? achievement.Downloads[achievement.Downloads.Count - 1] : null;

            if (download == null || download.ID != uploadId)
            {
                download = new UploadSection.DownloadItem
                {
                    ID = uploadId,
                    Title = (string)uploadRow["UploadTitle"],
                    Competencies = new List<UploadSection.CompetencyItem>()
                };

                var uploadType = uploadRow["UploadType"] as string;
                var uploadName = (string)uploadRow["UploadName"];
                var dataLength = uploadRow["ContentSize"] as int?;

                if (StringHelper.Equals(uploadType, UploadType.Link))
                {
                    download.Url = uploadName;
                }
                else
                {
                    download.Url = CmdsUploadProvider.GetFileRelativeUrl(containerId, uploadName);
                    download.SizeInKilobytes = dataLength.Value / 1024;
                }

                achievement.Downloads.Add(download);
            }

            if (uploadRow["CompetencyStandardIdentifier"] != DBNull.Value)
                download.Competencies.Add(new UploadSection.CompetencyItem
                {
                    ID = (Guid)uploadRow["CompetencyStandardIdentifier"],
                    Number = (string)uploadRow["CompetencyNumber"]
                });
        }

        #endregion

        #region Split achievement types

        private List<List<UploadSection.AchievementTypeItem>> SplitAchievementTypes(List<UploadSection.AchievementTypeItem> achievementTypes, int listCount)
        {
            var downloadCount = CalculateDownloadCount(achievementTypes);
            var downloadCountForList = downloadCount <= listCount ? downloadCount : downloadCount / listCount;

            var listOfAchievementTypes = new List<UploadSection.AchievementTypeItem>[listCount];

            var index = 0;

            for (var listIndex = 0; listIndex < listCount - 1; listIndex++)
            {
                var addedDownloads = 0;

                listOfAchievementTypes[listIndex] = new List<UploadSection.AchievementTypeItem>();

                while (addedDownloads < downloadCountForList && index < achievementTypes.Count)
                {
                    var achievementType = achievementTypes[index];

                    listOfAchievementTypes[listIndex].Add(achievementType);

                    foreach (var achievement in achievementType.Achievements)
                        addedDownloads += achievement.Downloads.Count;

                    index++;
                }
            }

            listOfAchievementTypes[listCount - 1] = new List<UploadSection.AchievementTypeItem>();

            while (index < achievementTypes.Count)
            {
                listOfAchievementTypes[listCount - 1].Add(achievementTypes[index]);

                index++;
            }

            return listOfAchievementTypes.ToList();
        }

        private static int CalculateDownloadCount(IList<UploadSection.AchievementTypeItem> achievementTypes)
        {
            var count = 0;

            foreach (var achievementeType in achievementTypes)
                foreach (var achievement in achievementeType.Achievements)
                    count += achievement.Downloads.Count;

            return count;
        }

        #endregion
    }
}
