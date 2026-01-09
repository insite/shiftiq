using System;
using System.IO;
using System.Linq;

using InSite.Common.Web;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Accounts.Users
{
    public partial class SelectBackground : PortalBasePage
    {
        private const string WallpaperImagePath = "/Library/Images/Wallpapers/";

        private AdminHomeSettings _userSettings;

        private AdminHomeSettings UserSettings => _userSettings
            ?? (_userSettings = PersonalizationRepository.GetValue<AdminHomeSettings>(Guid.Empty, User.UserIdentifier, PersonalizationName.AdminHome, false) ?? new AdminHomeSettings());

        #region Initialization and loading

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            RadioNone.AutoPostBack = true;
            RadioNone.CheckedChanged += Radio_CheckedChanged;

            RadioColor.AutoPostBack = true;
            RadioColor.CheckedChanged += Radio_CheckedChanged;

            RadioImage.AutoPostBack = true;
            RadioImage.CheckedChanged += Radio_CheckedChanged;

            SaveNoneButton.Click += SaveNoneButton_Click;
            SaveColorButton.Click += SaveColorButton_Click;

            ClearWallpaperButton.Click += ClearWallpaperButton_Click;
            SaveWallpaperButton.Click += SaveWallpaperButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
                LoadData();
        }

        private void LoadData()
        {
            Layout.Admin.PageHelper.AutoBindHeader(this);

            RadioColor.Checked = UserSettings.BackgroundColor.HasValue();
            RadioImage.Checked = UserSettings.WallpaperUrl.HasValue();
            RadioNone.Checked = !RadioColor.Checked && !RadioImage.Checked;

            ColorSelector.Value = UserSettings.BackgroundColor.HasValue()
                ? UserSettings.BackgroundColor
                : "#35cfe3";

            SelectedWallpaper.Value = UserSettings.WallpaperUrl.HasValue()
                ? Path.GetFileName(UserSettings.WallpaperUrl)
                : string.Empty;

            WallpaperRepeater.DataSource = Directory.GetFiles(MapPath(WallpaperImagePath)).Select(x =>
            {
                var filename = Path.GetFileName(x);

                return new
                {
                    Title = Path.GetFileNameWithoutExtension(filename),
                    Filename = filename,
                    Url = WallpaperImagePath + filename
                };
            }).OrderBy(x => x.Title).ThenBy(x => x.Url);
            WallpaperRepeater.DataBind();

            OnRadioCheckedChanged();

            PageHelper.HideSideContent(Page);
        }

        #endregion

        #region Event handlers

        private void Radio_CheckedChanged(object sender, EventArgs e)
        {
            OnRadioCheckedChanged();
        }

        private void OnRadioCheckedChanged()
        {
            NonePanel.Visible = RadioNone.Checked;
            ColorPanel.Visible = RadioColor.Checked;
            ImagePanel.Visible = RadioImage.Checked;
        }

        private void SaveNoneButton_Click(object sender, EventArgs e)
        {
            UserSettings.WallpaperUrl = null;
            UserSettings.BackgroundColor = null;

            PersonalizationRepository.SetValue(Guid.Empty, User.UserIdentifier, PersonalizationName.AdminHome, UserSettings);
            HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);
        }

        private void SaveColorButton_Click(object sender, EventArgs e)
        {
            UserSettings.WallpaperUrl = null;
            UserSettings.BackgroundColor = ColorSelector.Value;

            PersonalizationRepository.SetValue(Guid.Empty, User.UserIdentifier, PersonalizationName.AdminHome, UserSettings);
            HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);
        }

        private void SaveWallpaperButton_Click(object sender, EventArgs e)
        {
            var relativePath = WallpaperImagePath + SelectedWallpaper.Value;
            var physicalPath = MapPath(relativePath);
            if (!File.Exists(physicalPath))
                return;

            UserSettings.BackgroundColor = null;
            UserSettings.WallpaperUrl = relativePath;

            PersonalizationRepository.SetValue(Guid.Empty, User.UserIdentifier, PersonalizationName.AdminHome, UserSettings);
            HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);
        }

        private void ClearWallpaperButton_Click(object sender, EventArgs e)
        {
            UserSettings.WallpaperUrl = null;
            PersonalizationRepository.SetValue(Guid.Empty, User.UserIdentifier, PersonalizationName.AdminHome, UserSettings);
            HttpResponseHelper.Redirect(RelativeUrl.PortalHomeUrl);
        }

        #endregion
    }
}