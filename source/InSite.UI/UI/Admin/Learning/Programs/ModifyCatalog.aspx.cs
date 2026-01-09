using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;

using Shift.Common;

namespace InSite.Admin.Records.Programs
{
    public partial class ModifyCatalog : AdminBasePage, IHasParentLinkParameters
    {
        public const string NavigateUrl = "/ui/admin/learning/programs/modify-catalog";

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

            CatalogIdentifier.ValueAsGuid = program.CatalogIdentifier;
            CatalogSequence.ValueAsInt = program.CatalogSequence;

            ProgramCategoryList.SetProgram(program.ProgramIdentifier);

            CancelButton.NavigateUrl = Outline.GetNavigateUrl(ProgramId.Value, tab: "catalog");
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

            if (program.CatalogIdentifier != CatalogIdentifier.ValueAsGuid)
                program.CatalogIdentifier = CatalogIdentifier.ValueAsGuid;

            if (program.CatalogSequence != CatalogSequence.ValueAsInt)
                program.CatalogSequence = CatalogSequence.ValueAsInt;

            ProgramStore.Update(program, User.Identifier);

            ProgramCategoryList.SaveData();

            Outline.Redirect(program.ProgramIdentifier, tab: "catalog");
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