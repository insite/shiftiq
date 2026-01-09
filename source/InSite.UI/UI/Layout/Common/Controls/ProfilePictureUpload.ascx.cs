using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Application.Files.Read;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.Web.Helpers;

using Shift.Common.File;
using Shift.Common.Integration.ImageMagick;
using Shift.Constant;
using Shift.Toolbox;

namespace InSite.UI.Layout.Common.Controls
{
    public partial class ProfilePictureUpload : UserControl
    {
        public event EventHandler<ProfileUploadEventArgs> ProfileUploadCompleted;

        private string ImageUrl
        {
            get => ViewState[nameof(ImageUrl)] as string;
            set => ViewState[nameof(ImageUrl)] = value;
        }

        private string AlternateText
        {
            get => ViewState[nameof(AlternateText)] as string;
            set => ViewState[nameof(AlternateText)] = value;
        }

        private Guid? UserID
        {
            get => ViewState[nameof(UserID)] as Guid?;
            set => ViewState[nameof(UserID)] = value;
        }


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadProfilePicture.Text = "Upload";
            UploadProfilePicture.Click += UploadButton_Click;

            DeleteProfilePicture.Click += DeleteButton_Click;
        }

        public void LoadProfilePicture(QUser user)
        {
            UserID = user.UserIdentifier;
            AlternateText = user.FirstName + " " + user.LastName;
            ImageUrl = user.ImageUrl;

            ProfileImage.ImageUrl = string.IsNullOrEmpty(ImageUrl) ? "/UI/Layout/Portal/Images/Default.png" : ImageUrl;
            ProfileImage.AlternateText = AlternateText;
        }

        protected void UploadButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (!UserID.HasValue)
                {
                    NotifyParent(false, "User information is missing.");
                    return;
                }

                var isUploaded = UploadProfilePhotoV2();

                if (isUploaded)
                    NotifyParent(true, "Profile picture updated successfully.");
                else
                    NotifyParent(false, "Failed to upload profile picture.");
            }
            catch (Exception ex)
            {
                NotifyParent(false, ex.ToString());
            }

            ProfileUploadPanel.Attributes["class"] += " show";
        }

        protected void DeleteButton_Click(object sender, EventArgs e)
        {
            if (!UserID.HasValue)
            {
                NotifyParent(false, "User information is missing.");
                return;
            }

            UpdateUserProfileImage(string.Empty);
            NotifyParent(true, "Profile picture deleted.");
        }

        private void NotifyParent(bool success, string message)
        {
            ProfileUploadCompleted?.Invoke(this, new ProfileUploadEventArgs(success, message));
        }

        private bool UploadProfilePhotoV2()
        {
            if (!ProfilePictureToUploadV2.HasFile)
            {
                NotifyParent(false, "Please select a file.");
                return false;
            }

            var user = GetUser();
            var oldUrl = user.ImageUrl;

            var size = ProfilePictureHelper.MaxProfileImageSize;

            var newUrl = ProfilePictureToUploadV2.AdjustImageSaveAndGetUrl(
                UserID.Value,
                FileObjectType.User,
                size,
                size);

            if (string.IsNullOrWhiteSpace(newUrl))
            {
                NotifyParent(false, "Failed to process the image.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(oldUrl) && !string.Equals(oldUrl, newUrl, StringComparison.OrdinalIgnoreCase))
                FileUploadV2.DeleteFileByUrl(oldUrl);

            user.ImageUrl = newUrl;
            UserStore.Update(user, null);

            return true;
        }


        private void UpdateUserProfileImage(string imgUrl)
        {
            var user = GetUser();
            user.ImageUrl = imgUrl;
            UserStore.Update(user, null);
            LoadProfilePicture(user);
        }

        private QUser GetUser()
           => ServiceLocator.UserSearch.GetUser(UserID.Value);
    }

    public class ProfileUploadEventArgs : EventArgs
    {
        public bool Success { get; }
        public string Message { get; }

        public ProfileUploadEventArgs(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}