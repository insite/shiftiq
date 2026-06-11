using System;

using Shift.Common.Timeline.Changes;

using Shift.Constant;

namespace InSite.Domain.Contacts
{
    public class PersonAggregate : AggregateRoot
    {
        public override AggregateState CreateState() => new PersonState();

        public PersonState Data => (PersonState)State;

        public void CreatePerson(Guid userId, Guid organizationId, string fullName)
        {
            Apply(new PersonCreated(userId, organizationId, fullName));
        }

        public void ArchivePerson(DateTimeOffset archived)
        {
            Apply(new PersonArchived(archived));
        }

        public void UnarchivePerson(DateTimeOffset unarchived)
        {
            Apply(new PersonUnarchived(unarchived));
        }

        public void DeletePerson()
        {
            Apply(new PersonDeleted());
        }

        public void ModifyPersonAddress(AddressType addressType, PersonAddress address)
        {
            Apply(new PersonAddressModified(addressType, address));
        }

        public void ModifyPersonComment(CommentActionType commentActionType, PersonComment comment)
        {
            Apply(new PersonCommentModified(commentActionType, comment));
        }

        public void ApprovePersonJob(DateTimeOffset? approved, string approvedBy)
        {
            Apply(new PersonJobApproved(approved, approvedBy));
        }

        public void GrantPersonAccess(DateTimeOffset granted, string grantedBy)
        {
            Apply(new PersonAccessGranted(granted, grantedBy));
        }

        public void RevokePersonAccess(DateTimeOffset revoked, string revokedBy)
        {
            Apply(new PersonAccessRevoked(revoked, revokedBy));
        }

        public void ModifyPersonFieldText(PersonField personField, string value)
        {
            Apply(new PersonFieldTextModified(personField, value));
        }

        public void FixPersonFieldDateOffset(PersonField personField, DateTimeOffset? value)
        {
            Apply(new PersonFieldDateOffsetFixed(personField, value));
        }

        public void ModifyPersonFieldDateOffset(PersonField personField, DateTimeOffset? value)
        {
            Apply(new PersonFieldDateOffsetModified(personField, value));
        }

        public void ModifyPersonFieldDate(PersonField personField, DateTime? value)
        {
            Apply(new PersonFieldDateModified(personField, value));
        }

        public void ModifyPersonFieldBool(PersonField personField, bool? value)
        {
            Apply(new PersonFieldBoolModified(personField, value));
        }

        public void ModifyPersonFieldInt(PersonField personField, int? value)
        {
            Apply(new PersonFieldIntModified(personField, value));
        }

        public void ModifyPersonFieldGuid(PersonField personField, Guid? value)
        {
            Apply(new PersonFieldGuidModified(personField, value));
        }
    }
}
