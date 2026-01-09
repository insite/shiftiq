using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Records
{
    public class CredentialState : AggregateState
    {
        public Guid Identifier { get; set; }
        public Guid User { get; set; }

        public DateTimeOffset? Assigned { get; set; }
        public DateTimeOffset? Granted { get; set; }
        public DateTimeOffset? Expired { get; set; }
        public DateTimeOffset? Revoked { get; set; }

        public Expiration Expiration { get; set; }
        public CredentialStatus Status { get; set; }

        public Guid? Authority { get; set; }
        public string AuthorityName { get; set; }
        public string AuthorityType { get; set; }

        public string Location { get; set; }
        public string Necessity { get; set; }
        public string Priority { get; set; }
        public string Reference { get; set; }
        public string Description { get; set; }

        public string GrantedReason { get; set; }
        public string RevokedReason { get; set; }

        public decimal? Hours { get; set; }
        public decimal? Score { get; set; }

        public Guid? EmployerGroup { get; set; }
        public string EmployerGroupStatus { get; set; }

        public static DateTimeOffset? CalculateExpectedExpiry(Expiration expiration, DateTimeOffset? granted)
        {
            if (expiration == null || expiration.Type == ExpirationType.None)
                return null;

            else if (expiration.Type == ExpirationType.Fixed)
            {
                if (granted == null || granted.Value <= expiration.Date)
                    return expiration.Date;
            }

            else if (expiration.Type == ExpirationType.Relative && expiration.Lifetime != null && granted.HasValue)
            {
                var quantity = expiration.Lifetime.Quantity;

                if (quantity == 0)
                    return null;

                switch (expiration.Lifetime.Unit)
                {
                    case "Year":
                        return granted.Value.AddYears(quantity);
                    case "Month":
                        return granted.Value.AddMonths(quantity);
                    default:
                        throw new UnsupportedUnitException(expiration.Lifetime.Unit);
                }
            }

            return null;
        }

        public static CredentialStatus ExpectedStatus(DateTimeOffset? granted, DateTimeOffset? revoked, Expiration expiration, DateTimeOffset at)
        {
            if (revoked.HasValue && revoked <= at)
                return CredentialStatus.Revoked;

            if (granted.HasValue)
            {
                if (granted > at)
                    return CredentialStatus.Pending;

                var expiry = CalculateExpectedExpiry(expiration, granted);
                if (expiry.HasValue && expiry <= at)
                    return CredentialStatus.Expired;

                return CredentialStatus.Valid;
            }

            return CredentialStatus.Undefined;
        }

        public void When(CredentialCreated e)
        {
            Identifier = e.AggregateIdentifier;
            User = e.User;
            Assigned = e.Assigned;
            Status = CredentialStatus.Pending;
        }

        public void When(CredentialGranted3 e)
        {
            Granted = e.Granted;
            GrantedReason = e.Description;
            Expired = null;
            Revoked = null;
            Status = CredentialStatus.Valid;
            Score = e.Score;
        }

        public void When(CredentialAuthorityChanged e)
        {
            Authority = e.AuthorityIdentifier;
            AuthorityName = e.AuthorityName;
            AuthorityType = e.AuthorityType;

            Location = e.Location;
            Reference = e.Reference;
            Hours = e.Hours;
            Status = CredentialStatus.Valid;
        }

        public void When(CredentialExpirationChanged e)
        {
            Expiration = e.Expiration;
        }

        public void When(CredentialTagged e)
        {
            Necessity = e.Necessity;
            Priority = e.Priority;
        }

        [Obsolete]
        public void When(CredentialExpired e)
        {
            Expired = e.Expired;
            Status = CredentialStatus.Expired;
        }

        public void When(CredentialExpired2 e)
        {
            Expired = e.Expired;
            Status = CredentialStatus.Expired;
        }

        public void When(CredentialRevoked2 e)
        {
            Revoked = e.Revoked;
            RevokedReason = e.Reason;
            Granted = null;
            Status = CredentialStatus.Pending;
            Score = e.Score;
        }

        public void When(CredentialDeleted2 e)
        {
            Status = CredentialStatus.Undefined;
        }

        public void When(CredentialDescribed2 e)
        {
            Description = e.Description;
        }

        public void When(CredentialEmployerChanged e)
        {
            EmployerGroup = e.EmployerGroup;
            EmployerGroupStatus = e.EmployerGroupStatus;
        }

        public void When(ExpirationReminderRequested2 _)
        {

        }

        public void When(ExpirationReminderDelivered2 _)
        {

        }
    }
}
