using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Admin.Assets.Contents.Controls.ContentEditor;
using InSite.Application.Achievements.Write;
using InSite.Application.Contents.Read;
using InSite.Application.Courses.Read;
using InSite.Application.Courses.Write;
using InSite.Application.Files.Read;
using InSite.Application.Gradebooks.Write;
using InSite.Application.Records.Read;
using InSite.Application.Sites.Read;
using InSite.Application.Sites.Write;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Domain.CourseObjects;
using InSite.Persistence;

using Shift.Common;
using Shift.Constant;
using Shift.Sdk.UI;

namespace InSite.Admin.Courses.Outlines.Controls
{
    public partial class CourseSetup : BaseUserControl
    {
        #region Classes

        protected class ImgObj
        {
            public string relativePath { get; set; }
            public string fileName { get; set; }
            public string absolutPath { get; set; }
        }

        #endregion

        public Guid CourseIdentifier
        {
            get => (Guid)ViewState[nameof(CourseIdentifier)];
            set => ViewState[nameof(CourseIdentifier)] = value;
        }

        public bool IsPublished
        {
            get => (bool)ViewState[nameof(IsPublished)];
            set => ViewState[nameof(IsPublished)] = value;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Language.AutoPostBack = true;
            Language.ValueChanged += Language_ValueChanged;

            ContentRepeater.DataBinding += ContentRepeater_DataBinding;
            ContentRepeater.ItemDataBound += ContentRepeater_ItemDataBound;

            WebSiteIdentifier.AutoPostBack = true;
            WebSiteIdentifier.ValueChanged += WebSiteIdentifier_ValueChanged;

            WebFolderIdentifier.AutoPostBack = true;
            WebFolderIdentifier.ValueChanged += WebFolderIdentifier_ValueChanged;

            WebPageIdentifier.AutoPostBack = true;
            WebPageIdentifier.ValueChanged += WebPageIdentifier_ValueChanged;
            WebPageIdentifierAdd.Click += WebPageIdentifierAdd_Click;

            DeleteImage.Click += DeleteImage_Click;

            EnrollmentPersonAdd.Click += EnrollmentPersonAdd_Click;
            EnrollmentClassAdd.Click += EnrollmentClassAdd_Click;
            EnrollmentGroupAdd.Click += EnrollmentGroupAdd_Click;

            MissingGradebookRadio.SelectedIndexChanged += MissingGradebookRadio_SelectedIndexChanged;

            GradebookIdentifier.AutoPostBack = true;
            GradebookIdentifier.ListFilter.OrganizationIdentifier = Organization.Identifier;
            GradebookIdentifier.ValueChanged += (x, y) => { BindModelToControlsForGradebook(GradebookIdentifier.ValueAsGuid, false, true); };

            GradebookCreateButton.Click += (x, y) => { BindModelToControlsForGradebook(null, true, false); };

            AchievementIdentifier.AutoPostBack = true;
            AchievementIdentifier.ValueChanged += (x, y) => { BindModelToControlsForAchievement(AchievementIdentifier.Value, false); };
            AchievementCreateButton.Click += (x, y) => { BindModelToControlsForAchievement(null, true); };

            FrameworkIdentifier.Filter.StandardTypes = new[] { Shift.Constant.StandardType.Framework };

            CourseSaveButton.Click += CourseSaveButton_Click;
            CourseCancelButton.NavigateUrl = Request.RawUrl;

            DeleteEnrollmentsButton.Click += DeleteEnrollmentsButton_Click;
        }

        public void BindModelToControls(Course course)
        {
            Language.LoadItems(
                CurrentSessionState.Identity.Organization.Languages,
                "TwoLetterISOLanguageName", "EnglishName");

            CourseIdentifier = course.Identifier;
            CourseName.Text = course.Content.Title.GetText();
            CourseCode.Text = course.Code;
            CourseLabel.Text = course.Label;

            CourseAsset.Text = course.Asset.ToString();
            CourseThumbprint.Text = course.Identifier.ToString();

            CompletionActivityIdentifier.CourseIdentifier = course.Identifier;
            CompletionActivityIdentifier.RefreshData();
            CompletionActivityIdentifier.ValueAsGuid = course.CompletionActivityIdentifier;

            IsMultipleUnitsEnabled.Checked = course.AllowMultipleUnits;
            IsProgressReportEnabled.Checked = course.IsProgressReportEnabled;
            AllowDiscussion.Checked = course.AllowDiscussion;
            OutlineWidth3.Checked = course.OutlineWidth == 3;
            OutlineWidth4.Checked = course.OutlineWidth == 4;
            OutlineWidth5.Checked = course.OutlineWidth == 5;

            var requirements = GetCompletionRequirements(course);
            RequirementCount.Text = $"This course has {Shift.Common.Humanizer.ToQuantity(requirements.Length, "activity completion requirement")}.";
            if (requirements.Length == 0)
                RequirementCount.CssClass = "text-danger";
            RequirementRepeater.DataSource = requirements;
            RequirementRepeater.DataBind();

            CourseStyle.Text = course.Style;
            CourseFlagColor.Value = course.FlagColor;
            CourseFlagText.Text = course.FlagText;

            var page = GetCoursePages(course.Identifier).FirstOrDefault();
            WebPagePanel.Visible = page != null;
            WebPageIdentifier.ValueAsGuid = page?.PageIdentifier;
            OnWebPageSelected(page?.PageIdentifier);

            CourseSlug.Text = course.Slug ?? StringHelper.Sanitize(course.Content.Title.GetText(), '-');
            CourseIcon.Text = course.Icon;
            CourseIconPreview.Visible = !string.IsNullOrEmpty(course.Icon);
            CourseIconPreview.InnerHtml = $"<i class='{course.Icon}'></i>";

            FileList.LoadData(course.Identifier);

            CourseImage.ImageUrl = course.Image;
            CourseImageField.Visible = !string.IsNullOrEmpty(course.Image);
            CourseImageUrl.Text = course.Image;
            CatalogIdentifier.ValueAsGuid = course.Catalog;
            CatalogSequence.ValueAsInt = course.Sequence;
            CourseIsHidden.Checked = course.IsHidden;
            CourseFlagText.Text = course.FlagText;
            CourseFlagColor.Value = course.FlagColor;
            CourseClosed.Value = course.Closed;

            BindModelToControlsForEnrollment(course.Gradebook?.Identifier);
            BindModelToControlsForGradebook(course.Gradebook?.Identifier, false, course.Gradebook != null);

            CourseHook.Text = course.Hook;

            DeleteLink.NavigateUrl = $"/ui/admin/courses/delete?course={course.Identifier}";

            ContentRepeater.DataSource = GetContentSections();
            ContentRepeater.DataBind();

            CategoryList.SetCourse(course.Identifier);

            OnLanguageChanged();

            RecordsTab.IsSelected = Request.QueryString["tab"] == "records";
            PublicationsTab.IsSelected = Request.QueryString["tab"] == "publication";

            FrameworkIdentifier.Value = course.Framework;

            PrivacySettingsGroups.LoadData(CourseIdentifier, "Course");
        }

        private static QPage[] GetCoursePages(Guid courseId)
        {
            return ServiceLocator.PageSearch.Select(x => x.ObjectType == "Course" && x.ObjectIdentifier == courseId);
        }

        private class RequirementItem
        {
            public string Type { get; set; }
            public int Count { get; set; }
        }

        private RequirementItem[] GetCompletionRequirements(Course course)
        {
            var d = new Dictionary<RequirementType, int>();
            foreach (var unit in course.Units)
                foreach (var module in unit.Modules)
                    foreach (var activity in module.Activities)
                        if (activity.Requirement != RequirementType.None)
                        {
                            if (!d.ContainsKey(activity.Requirement))
                                d.Add(activity.Requirement, 0);

                            d[activity.Requirement]++;
                        }
            return d.Select(x => new RequirementItem { Type = Shift.Common.Humanizer.TitleCase(x.Key.ToString()), Count = x.Value }).ToArray();
        }

        public void BindControlsToModel(QCourse course, ContentContainer content)
        {
            course.CourseName = CourseName.Text;
            course.CourseCode = CourseCode.Text;
            course.CourseLabel = CourseLabel.Text;

            course.CompletionActivityIdentifier = CompletionActivityIdentifier.ValueAsGuid;

            course.IsMultipleUnitsEnabled = IsMultipleUnitsEnabled.Checked;
            course.IsProgressReportEnabled = IsProgressReportEnabled.Checked;
            course.AllowDiscussion = AllowDiscussion.Checked;

            if (OutlineWidth3.Checked)
                course.OutlineWidth = 3;
            else if (OutlineWidth4.Checked)
                course.OutlineWidth = 4;
            else if (OutlineWidth5.Checked)
                course.OutlineWidth = 5;

            course.FrameworkStandardIdentifier = FrameworkIdentifier.Value;
            course.GradebookIdentifier = GradebookIdentifier.ValueAsGuid;

            course.CourseHook = CourseHook.Text;
            course.CourseStyle = CourseStyle.Text;

            course.CourseSlug = CourseSlug.Text;
            course.CourseIcon = CourseIcon.Text;

            course.CourseFlagColor = CourseFlagColor.Value;
            course.CourseFlagText = CourseFlagText.Text;

            SaveV2Image(course);

            course.CatalogIdentifier = CatalogIdentifier.ValueAsGuid;
            course.CourseSequence = CatalogSequence.ValueAsInt ?? 0;
            course.CourseIsHidden = CourseIsHidden.Checked;

            course.Closed = CourseClosed.Value;

            BindControlsToModelForContent(content, CourseName.Text);

            if (WebPageIdentifier.ValueAsGuid.HasValue)
                SavePage(WebPageIdentifier.ValueAsGuid.Value, course, content);

            CategoryList.SaveData();
        }

        private void SaveV2Image(QCourse course)
        {
            var oldUrl = course.CourseImage;

            var newUrl = CourseImageUploadV2.AdjustImageSaveAndGetUrl(
                course.CourseIdentifier,
                FileObjectType.Course,
                300,
                200);

            if (string.IsNullOrWhiteSpace(newUrl))
                return;

            if (!string.IsNullOrWhiteSpace(oldUrl) && !string.Equals(oldUrl, newUrl, StringComparison.OrdinalIgnoreCase))
                FileUploadV2.DeleteFileByUrl(oldUrl);

            course.CourseImage = newUrl;
        }

        private void SavePage(Guid pageId, QCourse course, ContentContainer courseContent)
        {
            var oldPagesOriginal = GetCoursePages(course.CourseIdentifier);
            var newPageOriginal = ServiceLocator.PageSearch.Select(pageId);

            var newPage = newPageOriginal.Clone();
            newPage.ObjectType = "Course";
            newPage.ObjectIdentifier = course.CourseIdentifier;
            newPage.ContentControl = "Course";

            if (PublicationStatus.Checked && newPage.IsHidden)
            {
                newPage.AuthorDate = DateTimeOffset.UtcNow;
                newPage.AuthorName = User.FullName;
                CourseClosed.Value = null;
            }
            else if (!PublicationStatus.Checked && !newPage.IsHidden && !CourseClosed.Value.HasValue)
            {
                CourseClosed.Value = DateTimeOffset.UtcNow;
            }

            newPage.IsHidden = !PublicationStatus.Checked;
            newPage.PageIcon = course.CourseIcon;
            newPage.PageSlug = course.CourseSlug;

            if (course.CourseImage != null)
            {
                if (newPage.ContentLabels == null)
                    newPage.ContentLabels = "ImageURL";
                else if (!newPage.ContentLabels.Contains("ImageURL"))
                    newPage.ContentLabels += ", ImageURL";

                var pageContent = ServiceLocator.ContentSearch.SelectContainer(newPage.PageIdentifier, "ImageURL", "en")
                    ?? new TContent
                    {
                        ContentIdentifier = UniqueIdentifier.Create(),
                        ContainerType = "Web Page",
                        ContainerIdentifier = newPage.PageIdentifier,
                        ContentLabel = "ImageURL",
                        ContentLanguage = "en",
                        OrganizationIdentifier = Organization.OrganizationIdentifier
                    };

                pageContent.ContentText = course.CourseImage;
                ServiceLocator.ContentStore.Save(pageContent);
            }

            var newPageContent = ServiceLocator.ContentSearch.GetBlock(newPage.PageIdentifier);
            newPageContent.Title.Set(courseContent.Title);

            var newPageCommands = new PageCommandGenerator().GetDifferencePageSetupCommands(newPageOriginal, newPage);

            newPageCommands.Add(new InSite.Application.Pages.Write.ChangePageContent(newPage.PageIdentifier, newPageContent));

            ServiceLocator.SendCommands(newPageCommands);

            foreach (var oldPageOriginal in oldPagesOriginal)
            {
                if (newPageOriginal.PageIdentifier == oldPageOriginal.PageIdentifier)
                    continue;

                var oldPage = oldPageOriginal.Clone();
                oldPage.ObjectIdentifier = null;

                var oldPageCommands = new PageCommandGenerator().GetDifferencePageSetupCommands(oldPageOriginal, oldPage);

                ServiceLocator.SendCommands(oldPageCommands);
            }
        }

        public void BindControlsToModelForContent(ContentContainer content, string courseName)
        {
            content.Title.Text = DetailsTab.IsSelected
                ? new MultilingualString { Default = courseName }
                : GetContentValue(ContentSectionDefault.Title, null);

            content.Body.Text = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyText);
            content.Body.Html = GetContentValue(ContentSectionDefault.Body, ContentSectionDefault.BodyHtml);

            MultilingualString GetContentValue(ContentSectionDefault id1, ContentSectionDefault? id2)
            {
                var index = ContentID[id1.GetName()];
                var item = ContentRepeater.Items[index];
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                return id2.HasValue ? section.GetValue(id2.Value.GetName()) : section.GetValue();
            }
        }

        private void CourseSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                var course = ServiceLocator.CourseSearch.GetCourse(CourseIdentifier);
                var units = CourseSearch.SelectUnitsForCourse(CourseIdentifier);
                var content = ServiceLocator.ContentSearch.GetBlock(CourseIdentifier);

                BindControlsToModel(course, content);

                SaveChangesToGradebook(course, units);
                SaveChangesToAchievement(course);

                Course2Store.UpdateCourse(course, content);

                HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "panel", "course", "tab", "publication"));
            }
            catch (Application.Records.AchievementException ex)
            {
                CourseSetupAlert.AddMessage(AlertType.Error, $"Modifications are not permitted while the achievement is locked. Please unlock it before making any changes. {ex.Message}");
            }
        }

        private void DeleteImage_Click(object sender, EventArgs e)
        {
            var course = ServiceLocator.CourseSearch.GetCourse(CourseIdentifier);
            if (course == null)
                return;

            if (course.CourseImage == null)
                return;

            var (fileIdentifier, _) = ServiceLocator.StorageService.ParseFileUrl(course.CourseImage);
            if (fileIdentifier != null)
                ServiceLocator.StorageService.Delete(fileIdentifier.Value);

            course.CourseImage = null;
            Course2Store.UpdateCourse(course, null);

            Response.StatusCode = (int)HttpStatusCode.OK;
            HttpResponseHelper.Redirect(UrlParser.BuildRelativeUrl(Request.RawUrl, "panel", "course", "tab", "publication"));
        }

        #region Publication

        private void WebSiteIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebSiteSelected();

        private void OnWebSiteSelected()
        {
            WebFolderIdentifier.ClearSelection();
            WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

            if (WebSiteIdentifier.ValueAsGuid.HasValue)
                WebFolderIdentifier.ListFilter.SiteIdentifier = WebSiteIdentifier.ValueAsGuid;

            WebFolderIdentifier.RefreshData();
            OnWebFolderSelected();
        }

        private void WebFolderIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebFolderSelected();

        private void OnWebFolderSelected()
        {
            WebPageIdentifier.ClearSelection();
            WebPageIdentifier.ListFilter.FolderIdentifier = Guid.Empty;

            WebPagePanel.Visible = WebFolderIdentifier.ValueAsGuid.HasValue;
            if (WebPagePanel.Visible)
                WebPageIdentifier.ListFilter.FolderIdentifier = WebFolderIdentifier.ValueAsGuid;

            WebPageIdentifier.RefreshData();
            OnWebPageSelected(WebPageIdentifier.ValueAsGuid);
        }

        private void WebPageIdentifier_ValueChanged(object sender, EventArgs e)
            => OnWebPageSelected(WebPageIdentifier.ValueAsGuid);

        private void OnWebPageSelected(Guid? id)
        {
            IsPublished = id.HasValue;
            if (IsPublished)
            {
                var page = SitemapSearch.Get(id.Value);

                if (page != null)
                {
                    var pageInfo = ServiceLocator.PageSearch.GetPage(id.Value);

                    PublishedInfoPanel.Visible = !page.PageIsHidden;
                    PublishedBy.Text = pageInfo.AuthorName;
                    PublishedDate.Text = pageInfo.AuthorDate.Format(User.TimeZone, isHtml: true, nullValue: string.Empty);

                    WebPageIdentifier.ListFilter.FolderIdentifier = page.FolderIdentifier;
                    WebFolderIdentifier.ListFilter.SiteIdentifier = page.SiteIdentifier;
                    WebFolderIdentifier.ValueAsGuid = page.FolderIdentifier;
                    WebSiteIdentifier.ValueAsGuid = page.SiteIdentifier;

                    PublicationStatus.Checked = !page.PageIsHidden;

                    WebPageIdentifierEdit.HRef = $"/ui/admin/sites/pages/outline?id={page.PageIdentifier}";
                    WebPageIdentifierView.HRef = ServiceLocator.PageSearch.GetPagePath(page.PageIdentifier, false);
                    WebPageHelp.InnerHtml = $"{WebPageIdentifierView.HRef}";

                    WebPageIdentifierAdd.Visible = false;
                    WebPageIdentifierLinks.Visible = true;
                }
            }
            else
            {
                WebPageIdentifier.ListFilter.FolderIdentifier = Guid.Empty;
                WebFolderIdentifier.ListFilter.SiteIdentifier = Guid.Empty;

                WebPageIdentifierAdd.Visible = true;
                WebPageIdentifierLinks.Visible = false;
            }
        }

        private void WebPageIdentifierAdd_Click(object sender, EventArgs e)
        {
            var course = CourseSearch.SelectCourse(CourseIdentifier);
            var page = new QPage
            {
                ContentControl = "Course",
                ObjectType = "Course",
                ObjectIdentifier = course.CourseIdentifier,
                Title = course.CourseName,
                OrganizationIdentifier = Organization.OrganizationIdentifier,
                ParentPageIdentifier = WebFolderIdentifier.ValueAsGuid.Value,
                PageSlug = course.CourseSlug.HasValue() ? course.CourseSlug : CourseSlug.Text,
                PageIdentifier = UniqueIdentifier.Create(),
                PageType = "Page",
                SiteIdentifier = WebSiteIdentifier.ValueAsGuid.Value,
                AuthorDate = DateTimeOffset.Now,
                AuthorName = User.FullName
            };

            var pageContent = new ContentContainer();
            var courseContent = ServiceLocator.ContentSearch.GetBlock(CourseIdentifier, labels: new[] { ContentLabel.Title });

            pageContent.Title.Set(courseContent.Title);

            if (pageContent.Title.Text.Default.IsEmpty())
                pageContent.Title.Text.Default = course.CourseName;

            var commands = new PageCommandGenerator().GetCommands(page);

            commands.Add(new InSite.Application.Pages.Write.ChangePageContent(page.PageIdentifier, pageContent));

            ServiceLocator.SendCommands(commands);

            WebPageIdentifier.ListFilter.FolderIdentifier = page.ParentPageIdentifier;
            WebPageIdentifier.RefreshData();
            WebPageIdentifier.Value = page.PageIdentifier.ToString();
            OnWebPageSelected(page.PageIdentifier);
        }

        #endregion

        #region Content

        private Dictionary<string, int> ContentID
        {
            get => (Dictionary<string, int>)ViewState[nameof(ContentID)];
            set => ViewState[nameof(ContentID)] = value;
        }

        private void ContentRepeater_DataBinding(object sender, EventArgs e)
        {
            ContentID = new Dictionary<string, int>();
        }

        private void ContentRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (!IsContentItem(e))
                return;

            var dataItem = (AssetContentSection)e.Item.DataItem;
            if (ContentID.ContainsKey(dataItem.Id))
                throw ApplicationError.Create("Invalid section ID: " + dataItem.Id);

            var container = (DynamicControl)e.Item.FindControl("Container");
            var section = (SectionBase)container.LoadControl(dataItem.ControlPath);
            section.SetOptions(dataItem);

            ContentID.Add(dataItem.Id, e.Item.ItemIndex);
        }

        private void Language_ValueChanged(object sender, EventArgs e) => OnLanguageChanged();

        private void OnLanguageChanged()
        {
            var rebind = false;
            var lang = Language.Value.IfNullOrEmpty("en");

            if (lang != "en")
            {
                var from = ServiceLocator.ContentSearch.SelectContainerByLanguage(CourseIdentifier, "en");
                var to = ServiceLocator.ContentSearch.SelectContainerByLanguage(CourseIdentifier, lang);
                foreach (var en in from)
                {
                    var tx = to?.FirstOrDefault(x => x.ContentLanguage == lang && x.ContentLabel == en.ContentLabel);
                    if (tx?.ContentText == null)
                    {
                        rebind = true;
                        var translation = Translate("en", lang, en.ContentText);
                        ServiceLocator.ContentStore.Save(en.ContainerType, en.ContainerIdentifier, en.ContentLabel, translation, lang, en.OrganizationIdentifier);
                    }
                }
                if (rebind)
                {
                    ContentRepeater.DataSource = GetContentSections();
                    ContentRepeater.DataBind();
                }
            }

            foreach (RepeaterItem item in ContentRepeater.Items)
            {
                var container = (DynamicControl)item.FindControl("Container");
                var section = (SectionBase)container.GetControl();

                section.SetLanguage(lang);
            }
        }

        private IEnumerable<AssetContentSection> GetContentSections()
        {
            var content = ServiceLocator.ContentSearch.GetBlock(CourseIdentifier);

            {
                var section = (AssetContentSection.SingleLine)AssetContentSection.Create(ContentSectionDefault.Title, content.Title.Text);
                section.Title = "Title";
                section.Label = "Title";
                yield return section;
            }

            {
                var section = (AssetContentSection.MarkdownAndHtml)AssetContentSection.Create(ContentSectionDefault.Body, content.Body.Text, content.Body.Html);
                section.Title = "Body";
                section.MarkdownLabel = "Body Text (Markdown)";
                section.HtmlLabel = "Body (Html)";
                section.AllowUpload = true;
                section.UploadFolderPath = $"/courses/{CourseIdentifier}";
                section.EnableHtmlBuilder = true;
                yield return section;
            }
        }

        #endregion

        #region Enrollments

        private class EnrollmentRepeaterItem
        {
            public Guid LearnerIdentifier { get; set; }
            public string LearnerName { get; set; }
        }

        public void BindModelToControlsForEnrollment(Guid? gradebookId)
        {
            var gradebook = gradebookId.HasValue ? ServiceLocator.RecordSearch.GetGradebookState(gradebookId.Value) : null;
            var hasGradebook = gradebook != null;

            EnrollmentsPanel.Visible = hasGradebook;
            EnrollmentsMissingGradebookPanel.Visible = !hasGradebook;

            if (hasGradebook)
            {
                var filter = new QEnrollmentFilter { GradebookIdentifier = gradebookId.Value };
                var enrollments = ServiceLocator.RecordSearch.GetEnrollments(filter, null, null, x => x.Learner)
                    .Select(x => new EnrollmentRepeaterItem
                    {
                        LearnerIdentifier = x.LearnerIdentifier,
                        LearnerName = x.Learner.UserFullName
                    })
                    .OrderBy(x => x.LearnerName)
                    .ToArray();

                EnrollmentList.DataSource = enrollments;
                EnrollmentList.DataBind();

                CommandButtons.Visible = enrollments.Length > 0;
            }
        }

        private void EnrollmentClassAdd_Click(object sender, EventArgs e)
        {
            if (!EnrollmentClassIdentifier.HasValue || !GradebookIdentifier.ValueAsGuid.HasValue)
                return;

            var gradebook = GradebookIdentifier.ValueAsGuid.Value;
            var @class = EnrollmentClassIdentifier.Value.Value;
            var registrations = ServiceLocator.RegistrationSearch.GetRegistrationsByEvent(@class);

            foreach (var registration in registrations)
            {
                var now = DateTimeOffset.Now;

                var learner = registration.CandidateIdentifier;
                if (ServiceLocator.RecordSearch.EnrollmentExists(gradebook, learner))
                    continue;

                var enrollment = UniqueIdentifier.Create();
                ServiceLocator.SendCommand(new AddEnrollment(gradebook, enrollment, learner, null, now, null));
            }

            BindModelToControlsForEnrollment(gradebook);

            EnrollmentClassIdentifier.Value = null;
        }

        private void EnrollmentPersonAdd_Click(object sender, EventArgs e)
        {
            if (!EnrollmentPersonIdentifier.HasValue || !GradebookIdentifier.ValueAsGuid.HasValue)
                return;

            var gradebook = GradebookIdentifier.ValueAsGuid.Value;
            var learner = EnrollmentPersonIdentifier.Value.Value;
            var enrollment = UniqueIdentifier.Create();

            if (ServiceLocator.RecordSearch.EnrollmentExists(gradebook, learner))
                return;

            ServiceLocator.SendCommand(new AddEnrollment(gradebook, enrollment, learner, null, DateTimeOffset.Now, null));
            BindModelToControlsForEnrollment(gradebook);
            EnrollmentPersonIdentifier.Value = null;
        }

        private void EnrollmentGroupAdd_Click(object sender, EventArgs e)
        {
            if (!EnrollmentGroupIdentifier.HasValue || !GradebookIdentifier.ValueAsGuid.HasValue)
                return;

            var gradebook = GradebookIdentifier.ValueAsGuid.Value;
            var @group = EnrollmentGroupIdentifier.Value.Value;
            var members = MembershipSearch.Select(x => x.GroupIdentifier == @group);

            foreach (var member in members)
            {
                var enrollment = UniqueIdentifier.Create();

                if (ServiceLocator.RecordSearch.EnrollmentExists(gradebook, member.UserIdentifier))
                    return;

                ServiceLocator.SendCommand(new AddEnrollment(gradebook, enrollment, member.UserIdentifier, null, DateTimeOffset.Now, null));
                BindModelToControlsForEnrollment(gradebook);
                EnrollmentPersonIdentifier.Value = null;
            }

            //foreach (var registration in registrations)
            //{
            //    var now = DateTimeOffset.Now;

            //    var learner = registration.CandidateIdentifier;
            //    if (ServiceLocator.RecordSearch.EnrollmentExists(gradebook, learner))
            //        continue;

            //    var enrollment = UniqueIdentifier.Create();
            //    ServiceLocator.SendCommand(new AddEnrollment(gradebook, enrollment, learner, null, now, null));
            //}

            //BindModelToControlsForEnrollment(gradebook);

            //EnrollmentClassIdentifier.Value = null;
        }

        private void DeleteEnrollmentsButton_Click(object sender, EventArgs e)
        {
            var users = GetSelectedEnrollments();

            if (users.Count == 0)
                return;

            var gradebook = GradebookIdentifier.ValueAsGuid.Value;

            foreach (var id in users)
                ServiceLocator.SendCommand(new DeleteEnrollment(gradebook, id));

            BindModelToControlsForEnrollment(gradebook);
        }

        private List<Guid> GetSelectedEnrollments()
        {
            var result = new List<Guid>();

            foreach (DataListItem item in EnrollmentList.Items)
            {
                var isSelected = (ICheckBoxControl)item.FindControl("IsSelected");
                if (!isSelected.Checked)
                    continue;

                var learnerIdentifier = Guid.Parse(((ITextControl)item.FindControl("LearnerIdentifier")).Text);
                result.Add(learnerIdentifier);
            }

            return result;
        }

        #endregion

        #region Records

        private void MissingGradebookRadio_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (MissingGradebookRadio.SelectedValue == "Create")
                BindModelToControlsForGradebook(null, true, false);
            else
                BindModelToControlsForGradebook(null, false, true);
        }

        public void BindModelToControlsForGradebook(Guid? gradebookId, bool forceCreate, bool forceSelect)
        {
            MissingGradebookPanel.Visible = false;
            GradebookHeading.Visible = !MissingGradebookPanel.Visible;
            GradebookIdentifierField.Visible = false;

            GradebookNameField.Visible = false;
            AchievementPanel.Visible = false;

            if (forceCreate)
            {
                GradebookIdentifierField.Visible = false;
                GradebookNameField.Visible = true;
                GradebookName.Text = CourseName.Text;
                FinalGradeItemPassPercentField.Visible = true;
                FinalGradeItemPassPercent.ValueAsInt = Organization.Toolkits?.Gradebooks?.DefaultPassPercent ?? 50;
                return;
            }

            var gradebook = gradebookId.HasValue ? ServiceLocator.RecordSearch.GetGradebook(gradebookId.Value) : null;
            var hasGradebook = gradebook != null;

            GradebookIdentifierField.Visible = forceSelect;
            MissingGradebookPanel.Visible = !forceSelect && !hasGradebook;
            GradebookHeading.Visible = !MissingGradebookPanel.Visible;

            GradebookIdentifier.ValueAsGuid = gradebookId;
            GradebookIdentifier.EnsureDataBound();

            FinalGradeItemIdentifierField.Visible = hasGradebook;

            FinalGradeItemIdentifier.GradebookIdentifier = gradebookId;
            FinalGradeItemIdentifier.RefreshData();

            GradebookOutlineLink.Visible = hasGradebook;
            if (hasGradebook)
                GradebookOutlineLink.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}";

            if (hasGradebook)
            {
                GradebookOutlineLink.NavigateUrl = $"/ui/admin/records/gradebooks/outline?id={gradebook.GradebookIdentifier}";
                GradebookName.Text = gradebook.GradebookTitle;
                AchievementIdentifier.Value = gradebook.AchievementIdentifier;

                GradebookNameField.Visible = true;
                AchievementPanel.Visible = true;

                BindModelToControlsForAchievement(gradebook.AchievementIdentifier, false);
            }
        }

        public void BindModelToControlsForAchievement(Guid? achievementId, bool forceNew)
        {
            AchievementFields.Visible = false;

            if (forceNew)
            {
                AchievementIdentifierField.Visible = false;
                AchievementFields.Visible = true;
                AchievementName.Text = "New Achievement";
                AchievementLabel.Text = string.Empty;
                return;
            }

            var achievement = achievementId.HasValue
                ? ServiceLocator.AchievementSearch.GetAchievement(achievementId.Value)
                : null;

            if (achievement == null)
                return;

            AchievementOutlineLink.NavigateUrl = $"/ui/admin/records/achievements/outline?id={achievementId}";
            AchievementName.Text = achievement.AchievementTitle;
            AchievementLabel.Text = achievement.AchievementLabel;
            AchievementExpiration.Text = achievement.ExpirationToString();
            AchievementLayout.Value = achievement.CertificateLayoutCode;
            AchievementFields.Visible = true;

            var hasCrenedtials = HasStudentCredentials(achievementId.Value);

            AchievementWarning.Visible = hasCrenedtials;
            AchievementIdentifier.Enabled = !hasCrenedtials;
            AchievementCreateButton.Visible = !hasCrenedtials;

            BindModelToControlsForFinalScore(GradebookIdentifier.ValueAsGuid, achievementId);

            CategoryList.SetAchievement(achievement.AchievementLabel);
        }

        private bool HasStudentCredentials(Guid achievementId)
        {
            var count = ServiceLocator.AchievementSearch.CountCredentials(new VCredentialFilter
            {
                AchievementIdentifier = achievementId
            });

            return count > 0;
        }

        private void BindModelToControlsForFinalScore(Guid? gradebookId, Guid? achievementId)
        {
            var hasGradebookAndAchievement = gradebookId != null && achievementId != null;
            FinalGradeItemIdentifierField.Visible = hasGradebookAndAchievement;
            FinalGradeItemPassPercentField.Visible = hasGradebookAndAchievement;

            if (!hasGradebookAndAchievement)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(gradebookId.Value);
            var item = gradebook.AllItems
                .FirstOrDefault(x => x.Achievement?.Achievement == achievementId.Value && x.Type == GradeItemType.Category);

            if (item == null)
                return;

            FinalGradeItemIdentifier.ValueAsGuid = item.Identifier;
            FinalGradeItemPassPercent.ValueAsDecimal = item.PassPercent * 100m;
        }

        private void SaveChangesToGradebook(QCourse course, QUnit[] units)
        {
            if (MissingGradebookRadio.SelectedIndex == 0)
                CreateGradebook(course, units, FinalGradeItemPassPercent.ValueAsDecimal / 100m);
            else
                ModifyGradebook(course);
        }

        private void CreateGradebook(QCourse course, QUnit[] units, decimal? passPercent)
        {
            course.GradebookIdentifier = UniqueIdentifier.Create();

            var achievementType = Organization.Toolkits?.Achievements?.DefaultAchievementType ?? "Course";
            var certificateLayout = Organization.Toolkits?.Achievements?.DefaultCertificateLayout;

            var generator = new CourseObjectCommandGenerator(ServiceLocator.BankSearch.GetForm, id => ServiceLocator.QuizSearch.Select(id));
            generator.CreateCourseGradebook(course, units, course.GradebookIdentifier.Value, GradebookName.Text, achievementType, certificateLayout, passPercent);

            foreach (var command in generator.Commands)
                ServiceLocator.SendCommand(command);

            foreach (var item in generator.MapActivityToGradeItem)
                Course2Store.ConnectCourseActivityGradeItem(CourseIdentifier, item.Key, item.Value);
        }

        private void ModifyGradebook(QCourse course)
        {
            course.GradebookIdentifier = GradebookIdentifier.ValueAsGuid;
            if (course.GradebookIdentifier.HasValue)
                ServiceLocator.SendCommand(new RenameGradebook(course.GradebookIdentifier.Value, GradebookName.Text));
        }

        private void SaveChangesToAchievement(QCourse course)
        {
            if (!AchievementPanel.Visible)
                return;

            if (AchievementIdentifierField.Visible)
                UpdateAchievement(course);
            else
                AddNewAchievement(course);
        }

        private void UpdateAchievement(QCourse course)
        {
            var achievementId = AchievementIdentifier.Value;
            var achievement = achievementId.HasValue ? ServiceLocator.AchievementSearch.GetAchievement(achievementId.Value) : null;
            if (achievement == null)
                return;

            if (!achievement.AchievementIsEnabled)
                throw new Application.Records.AchievementException(achievement.AchievementTitle);

            if (!course.GradebookIdentifier.HasValue)
                return;

            var gradebook = ServiceLocator.RecordSearch.GetGradebookState(course.GradebookIdentifier.Value);

            if (gradebook.Achievement != achievement.AchievementIdentifier)
                ServiceLocator.SendCommand(new ChangeGradebookAchievement(gradebook.Identifier, achievement.AchievementIdentifier));

            if (achievement.AchievementLabel != AchievementLabel.Text || achievement.AchievementTitle != AchievementName.Text)
                ServiceLocator.SendCommand(new DescribeAchievement(
                    achievement.AchievementIdentifier, AchievementLabel.Text, AchievementName.Text, achievement.AchievementDescription, false));

            if (achievement.CertificateLayoutCode != AchievementLayout.Value)
                ServiceLocator.SendCommand(new ChangeCertificateLayout(
                    achievement.AchievementIdentifier, AchievementLayout.Value));

            if (FinalGradeItemIdentifierField.Visible && FinalGradeItemPassPercentField.Visible)
            {
                var itemId = FinalGradeItemIdentifier.ValueAsGuid;
                var item = itemId.HasValue ? gradebook.FindItem(itemId.Value) : null;

                if (item != null)
                {
                    var score = FinalGradeItemPassPercent.ValueAsDecimal / 100m;

                    ServiceLocator.SendCommand(new ChangeGradeItemPassPercent(
                        gradebook.Identifier, item.Identifier, score));
                }
            }
        }

        private void AddNewAchievement(QCourse course)
        {
            var achievement = UniqueIdentifier.Create();

            var label = Organization.Toolkits?.Achievements?.DefaultAchievementType ?? AchievementLabel.Text;
            var title = AchievementName.Text;
            if (string.IsNullOrWhiteSpace(title))
                title = "(New Achievement)";

            ServiceLocator.SendCommand(new CreateAchievement(achievement, Organization.OrganizationIdentifier, label, title, null, null, null));

            var layout = Organization.Toolkits?.Achievements?.DefaultCertificateLayout;
            if (!string.IsNullOrEmpty(layout))
                ServiceLocator.SendCommand(new ChangeCertificateLayout(achievement, layout));

            var gradebook = course.GradebookIdentifier.Value;

            ServiceLocator.SendCommand(new ChangeGradebookAchievement(gradebook, achievement));
        }

        #endregion
    }
}