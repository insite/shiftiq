using System;
using System.Collections.Generic;
using System.Web.UI;

using InSite.Application.Records.Read;
using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifySettings : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/modify-settings";

        public static string GetNavigateUrl(Guid programId) => NavigateUrl + "?id=" + programId;

        public static void Redirect(Guid programId) => HttpResponseHelper.Redirect(GetNavigateUrl(programId));

        private Guid? ProgramId => Guid.TryParse(Request["id"], out var result) ? result : (Guid?)null;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            SaveButton.Click += (s, a) => Save();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                Open();
        }

        #region Methods (open)

        private void Open()
        {
            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                Search.Redirect();

            PageHelper.AutoBindHeader(this, null, program.ProgramName);

            TaskGrid.BindModelToControls(program.ProgramIdentifier);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value, tab: "settings");
        }

        #endregion

        #region Methods (save)

        private void Save()
        {
            if (!Page.IsValid)
                return;

            var program = ProgramId.HasValue ? ProgramSearch.GetProgram(ProgramId.Value) : null;
            if (program == null)
                return;

            var achievements = TaskGrid.GetAchievements();
            var items = new List<TTask>();

            foreach (var achievement in achievements)
            {
                var item = new TTask
                {
                    ProgramIdentifier = program.ProgramIdentifier,
                    ObjectIdentifier = achievement.AchievementIdentifier,
                    TaskLifetimeMonths = achievement.LifetimeMonths,
                    TaskIsRequired = achievement.IsRequired,
                    TaskIsPlanned = achievement.IsPlanned
                };

                items.Add(item);
            }

            TaskStore.Update(items);

            Outline.Redirect(program.ProgramIdentifier, tab: "settings");
        }

        #endregion

        #region IHasParentLinkParameters

        public string GetParentLinkParameters(IWebRoute parent)
        {
            return parent.Name.EndsWith("/outline")
                ? $"id={ProgramId}"
                : null;
        }

        #endregion
    }
}