using System;

using Shift.Common;

using UserModel = InSite.Domain.Foundations.User;

namespace InSite.Persistence
{
    public static class UserAdapter
    {
        public static UserModel ToModel(User user, 
            string personCode, string phone, string jobTitle, 
            OtpModes? otpMode)
        {
            return new UserModel
            {
                PasswordExpiry = user.UserPasswordExpired,
                TimeZone = TimeZoneInfo.FindSystemTimeZoneById(user.TimeZone),
                Identifier = user.UserIdentifier,
                Email = user.Email,
                PersonCode = personCode,
                EmailVerified = user.EmailVerified,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.FullName,
                PasswordHash = user.UserPasswordHash,
                Phone = phone,
                JobTitle = jobTitle,
                AccessGrantedToCmds = user.AccessGrantedToCmds,
                IsCloaked = user.IsCloaked,
                MultiFactorAuthentication = user.MultiFactorAuthentication,
                ActiveOtpMode = otpMode ?? OtpModes.None,
                UserLicenseAccepted = user.UserLicenseAccepted
            };
        }
    }
}