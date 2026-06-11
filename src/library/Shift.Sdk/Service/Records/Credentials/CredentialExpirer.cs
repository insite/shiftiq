using System;
using System.Collections.Generic;

using InSite.Application;
using InSite.Application.Credentials.Write;
using InSite.Application.Records.Read;
using InSite.Domain.Records;

using Shift.Constant;

namespace Shift.Sdk.Service.Progress.Credentials
{
    public class CredentialExpirer
    {
        private readonly ICommander _commander;
        private readonly IAchievementSearch _achievementSearch;

        public CredentialExpirer(ICommander commander, IAchievementSearch achievementSearch)
        {
            _commander = commander;
            _achievementSearch = achievementSearch;
        }

        public List<VCredential> GetExpiredCredentials()
        {
            var filter = new VCredentialFilter()
            {
                CredentialStatus = CredentialStatus.Valid.ToString(),
                IsPendingExpiry = true
            };

            return _achievementSearch.GetCredentials(filter);
        }

        public int Expire(List<VCredential> expiredCredentials)
        {
            var expiredCount = 0;

            foreach (var credential in expiredCredentials)
            {
                var expiration = new Expiration(credential.CredentialExpirationType, credential.CredentialExpirationFixedDate, credential.CredentialExpirationLifetimeQuantity, credential.CredentialExpirationLifetimeUnit);
                var actual = credential.CredentialStatus;
                var expected = CredentialState.ExpectedStatus(credential.CredentialGranted, credential.CredentialRevoked, expiration, DateTimeOffset.UtcNow);

                if (actual == CredentialStatus.Valid.ToString() && expected == CredentialStatus.Expired)
                {
                    var expectedExpiry = CredentialState.CalculateExpectedExpiry(expiration, credential.CredentialGranted);
                    if (expected == CredentialStatus.Expired && expectedExpiry.HasValue)
                    {
                        _commander.Send(new ExpireCredential(credential.CredentialIdentifier, expectedExpiry.Value));
                        expiredCount++;
                    }
                }
            }

            return expiredCount;
        }
    }
}
