using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Records.Programs
{
    public partial class Navigate : PortalBasePage
    {
        #region Properties

        protected TProgram Program => _state.Model.Program;
        protected List<TaskModel> Tasks => _state.Model.UserTasks;
        protected string Slug => _state.Model.Slug;
        protected ContentContainer ProgramContenet => _state.Model.ProgramContenet;

        #endregion

        #region Fields

        private ProgramState _state;

        #endregion

        #region Initialization

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            TaskRepeater.ItemDataBound += TaskRepeater_ItemDataBound;
        }

        #endregion

        #region Loading

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadState();

            if (IsPostBack)
                return;

            BindBodyPanel();

            PageHelper.HideBreadcrumbs(this);

            var model = _state.Model;

            if (Page.Master is PortalMaster m)
            {
                m.HideBreadcrumbsAndTitle();
                m.SidebarVisible(false);
            }

            AddBreadcrumb(Translate("Home"), GetHomeUrl());
            AddBreadcrumb(model.Program.ProgramName, null);
        }

        private void LoadState()
        {
            if (_state != null)
                return;

            _state = new ProgramState(RouteData, Request.QueryString);

            if (!_state.LoadModel())
                HttpResponseHelper.Redirect(GetHomeUrl(), true);

            if (_state.Model.Program == null)
                HttpResponseHelper.SendHttp404();
        }

        private void BindBodyPanel()
        {
            ProgramTitle.Text = ProgramContenet.Title.GetText(CurrentLanguage, true);
            OverviewText.Text = ProgramContenet.Summary.GetHtml(CurrentLanguage, true);

            TaskRepeater.DataSource = Tasks.OrderBy(x => x.TaskSequence).ToList();
            TaskRepeater.DataBind();
        }

        #endregion

        private void TaskRepeater_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var item = (TaskModel)e.Item.DataItem;
            var card = (HtmlAnchor)e.Item.FindControl("cardLink");

            var cardImage = (Literal)e.Item.FindControl("CardImage");
            var cardIcon = (Literal)e.Item.FindControl("CardIcon");

            string image = string.Empty;
            string icon = string.Empty;

            if (item != null && item.TaskImage.HasValue())
            {
                image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl(item.TaskImage)}' alt=''>";
            }
            else
            {
                if (item.ObjectType.HasValue())
                {
                    if (item.ObjectType.Equals("Achievement"))
                    {
                        icon = $"<i class='far fa-award fa-3x mb-3'></i>";
                        image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl("library/images/cards/task-achievement.jpg")}' alt=''>";
                    }
                    if (item.ObjectType.Equals("AssessmentForm"))
                    {
                        icon = $"<i class='far fa-award fa-3x mb-3'></i>";
                        image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl("library/images/cards/assessments.jpg")}' alt=''>";
                    }
                    else if (item.ObjectType.Equals("Survey"))
                    {
                        icon = $"<i class='far fa-check-square fa-3x mb-3'></i>";
                        image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl("library/images/cards/task-survey.jpg")}' alt=''>";
                    }
                    else if (item.ObjectType.Equals("Course"))
                    {
                        icon = $"<i class='far fa-users-class fa-3x mb-3'></i>";
                        image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl("library/images/cards/task-course.jpg")}' alt=''>";
                    }
                    else if (item.ObjectType.Equals("Logbook"))
                    {
                        icon = $"<i class='far fa-pencil-ruler fa-3x mb-3'></i>";
                        image = $"<img class='card-img-top' src='{PathHelper.ToAbsoluteUrl("library/images/cards/task-logbook.jpg")}' alt=''>";
                    }
                    else
                        icon = $"<i class='far fa-file-alt fa-3x mb-3'></i>";
                }
                else
                {
                    icon = $"<i class='far fa-file-alt fa-3x mb-3'></i>";
                }
            }

            if (item.IsLocked)
            {
                var badge = $"<span style='z-index:1; top:15px; right:-10px' class='position-absolute p-2 badge bg-danger'><i class=\"fa-solid fa-lock\"></i></span>";
                var cardBadge = (Literal)e.Item.FindControl("CardBadge");
                cardBadge.Text = badge;
                card.Attributes.Add("class", "card card-hover card-tile border-0 shadow disableLink");
            }
            else if (item.IsCompleted)
            {
                var badge = $"<span style='z-index:1; top:15px; right:-10px' class='position-absolute p-1 badge bg-success'>Completed</span>";
                var cardBadge = (Literal)e.Item.FindControl("CardBadge");
                cardBadge.Text = badge;
            }

            cardImage.Text = image;
            cardIcon.Text = icon;

            if (image.HasValue())
                cardIcon.Visible = false;
        }

        #region Methods (helpers)

        private string GetHomeUrl()
        {
            var url = RelativeUrl.PortalHomeUrl;

            return url;
        }

        protected string GetTaskUrl(object taskId)
            => (taskId == null) ? string.Empty : $"navigate/{Slug}/{taskId}";

        protected string GetTaskName(Guid taskId, string taskType)
        {
            if (taskType.Equals("AssessmentForm"))
                taskType = "Assessment";

            if (!_state.Model.TaskContenet.TryGetValue(taskId, out var taskContent))
                return taskType;

            return $"{taskContent.Title.GetText(CurrentLanguage, true)} - {taskType}";
        }

        #endregion

    }
}