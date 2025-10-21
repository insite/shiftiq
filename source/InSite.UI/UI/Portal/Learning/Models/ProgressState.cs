using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Routing;

using InSite.Application.Records.Read;
using InSite.Application.Sites.Read;
using InSite.Domain.CourseObjects;
using InSite.Persistence.Content;
using InSite.Web.Routing;

using Shift.Constant;

using static InSite.Domain.CourseObjects.ProgressModel;

namespace InSite.UI.Portal.Learning.Models
{
    public class ProgressState
    {
        public ProgressModel Model { get; set; }
        public PortalPageModel PortalPage { get; set; }

        private int _pageNumber;
        private Guid? _activityIdentifier;
        private Guid? _courseIdentifier;

        public ProgressState(RouteData routes, NameValueCollection parameters)
        {
            _courseIdentifier = GetIdentifier("course", routes, parameters);

            _pageNumber = int.TryParse(parameters["activity"], out int n)
                ? n : 0;

            _activityIdentifier = Guid.TryParse(parameters["activity"], out Guid j)
                ? j : (Guid?)null;
        }

        private Guid? GetIdentifier(string name, RouteData routes, NameValueCollection parameters)
        {
            var id = routes.Values[name] as string;

            if (id == null)
                id = parameters[name];

            if (id != null && Guid.TryParse(id, out Guid i))
                return i;

            return null;
        }

        public void LoadModel(PortalPageModel model)
        {
            PortalPage = model;
            Load();
        }

        public bool LoadModel()
        {
            var identity = CurrentSessionState.Identity;

            var hasWriteAccess = identity.IsGranted(PermissionIdentifiers.Admin_Sites, PermissionOperation.Write);

            var appUrl = ServiceLocator.Urls.GetApplicationUrl(identity.Organization.Code);

            PortalPage = new PortalPageModel()
            {
                Page = new QPage()
            };

            if (!_courseIdentifier.HasValue)
                return false;

            var page = ServiceLocator.PageSearch.BindFirst(p=>p, x => x.ObjectIdentifier == _courseIdentifier.Value && x.ObjectType == "Course");

            PortalPage.Page.ObjectType = "Course";
            PortalPage.Page.ObjectIdentifier = _courseIdentifier.Value;
            if(page != null )
                PortalPage.Page.IsHidden = page.IsHidden;
            PortalPage.Path = GetPreviewUrl(_courseIdentifier.Value);

            if (string.IsNullOrEmpty(PortalPage.Path))
                throw new ArgumentNullException("PortalPage.Path");

            Load();

            PortalPage.SetEditLinkUrl(identity.IsAuthenticated, identity.IsOperator, hasWriteAccess, appUrl);

            return true;
        }

        public static string GetPreviewUrl(Guid course)
            => RoutingConfiguration.PortalCourseUrl(course);

        private void Load()
        {
            var identity = CurrentSessionState.Identity;
            var user = identity.User.UserIdentifier;

            var course = PortalPage.Page.ObjectType == "Course" && PortalPage.Page.ObjectIdentifier.HasValue
                ? Persistence.CourseSearch.SelectCourse(ServiceLocator.RecordSearch, PortalPage.Page.ObjectIdentifier.Value)
                : null;

            Model = _activityIdentifier.HasValue
                ? new ProgressModel(course, user, PortalPage.Path, _activityIdentifier.Value)
                : new ProgressModel(course, user, PortalPage.Path, _pageNumber);
        }

        public void Reload()
        {
            Load();
            LoadModelProgress();
        }

        public void LoadModelProgress()
        {
            var identity = CurrentSessionState.Identity;
            var groups = identity.Groups.Select(x => x.Identifier).ToArray();

            var gradebook = Model?.Course?.Gradebook;

            if (gradebook != null)
            {
                var progresses = ServiceLocator.RecordSearch
                    .GetGradebookScores(new QProgressFilter { GradebookIdentifier = gradebook.Identifier, StudentUserIdentifier = Model.User })
                    .Select(x => new ProgressInfo
                    {
                        Identifier = x.ProgressIdentifier,
                        Record = x.GradebookIdentifier,
                        User = x.UserIdentifier,
                        Item = x.GradeItemIdentifier,
                        Percent = x.ProgressPercent,
                        Text = x.ProgressText,
                        Number = x.ProgressNumber,
                        Points = x.ProgressPoints,
                        MaxPoints = x.ProgressMaxPoints,
                        Comment = x.ProgressComment,
                        Graded = x.ProgressGraded,
                        IsPublished = x.ProgressIsPublished,
                        Status = x.ProgressStatus,
                        Grade = x.ProgressPassOrFail
                    })
                    .ToArray();

                var forms = Model.GetAssessmentForms();
                var answers = ServiceLocator.AttemptSearch
                    .GetAttemptQuestionsByLearner(Model.User, forms);

                Model.ApplyProgression(progresses, answers, groups);
            }
            else
            {
                if (Model != null)
                    Model.ApplyProgression(new ProgressInfo[] { }, Enumerable.Empty<Domain.Attempts.AnswerState>(), groups);
            }
        }

        public void LoadModelProgress(Guid activity, Guid gradeitem)
        {
            var identity = CurrentSessionState.Identity;

            var progress = ServiceLocator.RecordSearch
                .GetGradebookScores(new QProgressFilter { GradeItemIdentifier = gradeitem, StudentUserIdentifier = Model.User })
                .Select(x => new ProgressInfo
                {
                    Identifier = x.ProgressIdentifier,
                    Record = x.GradebookIdentifier,
                    User = x.UserIdentifier,
                    Item = x.GradeItemIdentifier,
                    Percent = x.ProgressPercent,
                    Text = x.ProgressText,
                    Number = x.ProgressNumber,
                    Points = x.ProgressPoints,
                    MaxPoints = x.ProgressMaxPoints,
                    Comment = x.ProgressComment,
                    Graded = x.ProgressGraded,
                    IsPublished = x.ProgressIsPublished,
                    Status = x.ProgressStatus,
                    Grade = x.ProgressPassOrFail
                })
                .FirstOrDefault();

            Model.ApplyProgression(activity, progress);
        }
    }
}