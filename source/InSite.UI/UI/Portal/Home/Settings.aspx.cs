using System;
using System.Web.UI;

using InSite.Application.Contacts.Read;
using InSite.Application.Files.Read;
using InSite.Common.Web;
using InSite.Common.Web.UI;
using InSite.Persistence;
using InSite.UI.Layout.Admin;
using InSite.UI.Layout.Portal;
using InSite.Web.Helpers;

using Shift.Common;
using Shift.Constant;

namespace InSite.UI.Portal.Home
{
    public partial class Settings : PortalBasePage
    {
        #region Classes

        private class UserAndPerson
        {
            public QUser User { get; }
            public QPerson Person { get; }

            public UserAndPerson(QUser user, QPerson person)
            {
                User = user;
                Person = person;
            }
        }

        #endregion

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            UploadProfilePicture.Text = "Upload";
            UploadProfilePicture.Click += UploadButton_Click;

            SaveButton.Click += SaveButton_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            PageHelper.AutoBindHeader(Page);

            PortalMaster.ShowAvatar();
            PortalMaster.EnableSidebarToggle(true);

            if (!CurrentSessionState.Identity.IsAuthenticated)
                HttpResponseHelper.SendHttp403();

            if (!IsPostBack)
                Open();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid || !Save())
                return;

            var toast = ToastItem.UrlEncode(AlertType.Success, "fas fa-check-circle", "Success", "Your changes are saved.");
            HttpResponseHelper.Redirect($"/ui/portal/home/settings?toast={toast}");
        }

        private void UploadButton_Click(object sender, EventArgs e)
        {
            try
            {
                var isUploaded = UploadProfilePhotoV2();

                if (isUploaded)
                {
                    var toast = ToastItem.UrlEncode(AlertType.Success, "fas fa-check-circle", "Success", "Your changes are saved.");
                    HttpResponseHelper.Redirect($"/ui/portal/home/settings?toast={toast}");
                }
            }
            catch (Exception ex)
            {
                StatusAlert.AddMessage(AlertType.Error, "Error during profile photo update: " + ex.Message);
            }

            ProfileUploadPanel.Attributes["class"] += " show";
        }


        private bool UploadProfilePhotoV2()
        {
            if (!ProfilePictureToUploadV2.HasFile)
            {
                StatusAlert.AddMessage(AlertType.Error, "Please select a file");
                return false;
            }

            var userAndPerson = GetUserAndPerson();
            var user = userAndPerson.User;
            var oldUrl = user.ImageUrl;

            var size = ProfilePictureHelper.MaxProfileImageSize;

            var newUrl = ProfilePictureToUploadV2.AdjustImageSaveAndGetUrl(
                user.UserIdentifier,
                FileObjectType.User,
                size,
                size);

            if (string.IsNullOrWhiteSpace(newUrl))
            {
                StatusAlert.AddMessage(AlertType.Error, "Failed to process the image.");
                return false;
            }

            if (!string.IsNullOrWhiteSpace(oldUrl) && !string.Equals(oldUrl, newUrl, StringComparison.OrdinalIgnoreCase))
                FileUploadV2.DeleteFileByUrl(oldUrl);

            user.ImageUrl = newUrl;
            UserStore.Update(user, null);
            return true;
        }


        private void Open()
        {
            var userAndPerson = GetUserAndPerson();
            var user = userAndPerson.User;
            var person = userAndPerson.Person;

            var homeAddress = person?.GetAddress(ContactAddressType.Home);
            if (homeAddress.AddressIdentifier == Guid.Empty) homeAddress = null;

            FirstName.Text = user.FirstName;
            LastName.Text = user.LastName;
            ProfileImage.AlternateText = user.FirstName + " " + user.LastName;
            ProfileImage.ImageUrl = string.IsNullOrEmpty(user.ImageUrl) ? "/UI/Layout/Portal/Images/Default.png" : user.ImageUrl;
            Email.Text = string.IsNullOrEmpty(user.Email) ? string.Empty : user.Email.ToLower();
            Email.Enabled = false;
            PhoneMobile.Text = user.PhoneMobile;

            if (homeAddress != null)
            {
                Street1.Text = homeAddress.Street1 ?? string.Empty;
                City.Text = homeAddress.City ?? string.Empty;
                PostalCodeLabel.Text = homeAddress.Country == "USA" || homeAddress.Country == "United States"
                    ? "Zip Code"
                    : "Postal Code";
                PostalCode.Text = homeAddress.PostalCode;
                Country.Value = homeAddress.Country;
            }

            var toast = ToastItem.UrlDecode(Page.Request["toast"]); ;
            if (toast != null)
            {
                ProfileToast.Indicator = toast.Color;
                ProfileToast.Icon = toast.Icon;
                ProfileToast.Title = toast.Header;
                ProfileToast.Text = toast.Body;
                ProfileToast.Visible = true;
            }
        }

        private bool Save()
        {
            var userAndPerson = GetUserAndPerson();
            var user = userAndPerson.User;
            var person = userAndPerson.Person;

            user.FirstName = FirstName.Text;
            user.LastName = LastName.Text;
            user.PhoneMobile = Phone.Format(PhoneMobile.Text);

            UserStore.Update(user, OrganizationSearch.GetPersonFullNamePolicy(Organization.Identifier));

            if (person != null)
            {
                var homeAddress = person?.GetAddress(ContactAddressType.Home);
                if (homeAddress.AddressIdentifier == Guid.Empty) homeAddress = null;
                if (homeAddress != null)
                    GetInputValues(homeAddress);

                PersonStore.Update(person);
            }

            var i = CurrentSessionState.Identity;
            i.User.FirstName = user.FirstName;
            i.User.LastName = user.LastName;
            i.User.FullName = $"{user.FirstName} {user.LastName}";
            CurrentSessionState.Identity = i;

            return true;
        }

        public void GetInputValues(QPersonAddress address)
        {
            address.Street1 = Street1.Text;
            address.City = City.Text;
            address.PostalCode = PostalCode.Text;
            address.Country = Country.Value;
        }

        private UserAndPerson GetUserAndPerson()
        {
            var user = ServiceLocator.UserSearch.GetUser(User.UserIdentifier);

            var person = ServiceLocator.PersonSearch.GetPerson(user.UserIdentifier, Organization.OrganizationIdentifier,
                x => x.HomeAddress,
                x => x.WorkAddress,
                x => x.BillingAddress,
                x => x.ShippingAddress
            );

            return new UserAndPerson(user, person);
        }
    }
}