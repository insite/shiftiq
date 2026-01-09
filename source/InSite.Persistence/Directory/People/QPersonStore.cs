using System;
using System.Linq;

using Shift.Common.Timeline.Changes;

using InSite.Application.Contacts.Read;
using InSite.Application.Contents.Read;
using InSite.Domain.Contacts;

using Shift.Constant;

namespace InSite.Persistence
{
    public class QPersonStore : IPersonStore
    {
        public void Insert(PersonCreated e)
        {
            var entity = new QPerson
            {
                PersonIdentifier = e.AggregateIdentifier,
                UserIdentifier = e.UserId,
                OrganizationIdentifier = e.OrganizationId,
                IsAdministrator = PersonState.Defaults.IsAdministrator,
                IsDeveloper = PersonState.Defaults.IsDeveloper,
                IsLearner = PersonState.Defaults.IsLearner,
                IsOperator = PersonState.Defaults.IsOperator,
                EmailEnabled = PersonState.Defaults.EmailEnabled,
                EmailAlternateEnabled = PersonState.Defaults.EmailAlternateEnabled,
                MarketingEmailEnabled = PersonState.Defaults.MarketingEmailEnabled,
                FullName = e.FullName,
                Created = e.ChangeTime,
                CreatedBy = e.OriginUser
            };

            entity.Modified = entity.Created;
            entity.ModifiedBy = entity.CreatedBy;

            using (var db = new InternalDbContext())
            {
                db.QPersons.Add(entity);
                db.SaveChanges();
            }
        }

        public void Delete(PersonDeleted e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QPersons
                    .Where(x => x.PersonIdentifier == e.AggregateIdentifier)
                    .FirstOrDefault();

                if (entity == null)
                    return;

                if (entity.BillingAddress != null)
                    db.QPersonAddresses.Remove(entity.BillingAddress);

                if (entity.ShippingAddress != null)
                    db.QPersonAddresses.Remove(entity.ShippingAddress);

                if (entity.WorkAddress != null)
                    db.QPersonAddresses.Remove(entity.WorkAddress);

                if (entity.HomeAddress != null)
                    db.QPersonAddresses.Remove(entity.HomeAddress);

                db.QPersons.Remove(entity);
                db.SaveChanges();
            }
        }

        public void Update(PersonAddressModified e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QPersons.Where(x => x.PersonIdentifier == e.AggregateIdentifier).FirstOrDefault();
                if (entity == null)
                    return;

                if (e.Address != null)
                    UpdateAddress(entity, e.OriginUser, e.AddressType, e.Address, GetAddressDb, AddAddressDb);
                else
                    DeleteAddress(entity, e.OriginUser, e.AddressType, GetAddressDb, RemoveAddressDb);

                db.SaveChanges();

                QPersonAddress GetAddressDb(Guid addressId) => db.QPersonAddresses.Where(x => x.AddressIdentifier == addressId).FirstOrDefault();
                void AddAddressDb(QPersonAddress address) => db.QPersonAddresses.Add(address);
                void RemoveAddressDb(QPersonAddress address) => db.QPersonAddresses.Remove(address);
            }
        }

        public void Update(PersonCommentModified e)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QPersons.Where(x => x.PersonIdentifier == e.AggregateIdentifier).FirstOrDefault();
                if (entity == null)
                    return;

                if (e.Comment == null)
                    return;

                if (e.CommentActionType != CommentActionType.Delete)
                    UpdateComment(entity, e.OriginUser, e.CommentActionType, e.Comment, GetCommentDb, AddCommentDb);
                else
                    DeleteComment(entity, e.OriginUser, e.Comment.Comment, GetCommentDb, RemoveCommentDb);

                db.SaveChanges();

                QComment GetCommentDb(Guid commentId) => db.QComments.Where(x => x.CommentIdentifier == commentId).FirstOrDefault();
                void AddCommentDb(QComment comment) => db.QComments.Add(comment);
                void RemoveCommentDb(QComment comment) => db.QComments.Remove(comment);
            }

        }

        public void Update(Change e, Action<QPerson> action)
        {
            using (var db = new InternalDbContext())
            {
                var entity = db.QPersons.Where(x => x.PersonIdentifier == e.AggregateIdentifier).FirstOrDefault();
                if (entity == null)
                    return;

                action(entity);

                entity.Modified = DateTimeOffset.UtcNow;
                entity.ModifiedBy = e.OriginUser;

                db.SaveChanges();
            }
        }

        private static void UpdateComment(
            QPerson person,
            Guid originUser,
            CommentActionType commentActionType,
            PersonComment comment,
            Func<Guid, QComment> getComments,
            Action<QComment> addComment
            )
        {
            bool isNew = commentActionType == CommentActionType.Author;

            var commentEntity = !isNew ? getComments(comment.Comment) : null;

            if (commentEntity == null)
                addComment(commentEntity = new QComment { CommentIdentifier = comment.Comment });

            commentEntity.AuthorUserIdentifier = comment.Author;
            commentEntity.TopicUserIdentifier = comment.Topic;
            commentEntity.ContainerIdentifier = comment.Container;
            commentEntity.OrganizationIdentifier = comment.Organization;
            commentEntity.TimestampModifiedBy = comment.ModifiedBy;
            commentEntity.AuthorUserName = comment.AuthorName;
            commentEntity.CommentText = comment.Text;
            commentEntity.ContainerType = comment.ContainerType;
            commentEntity.CommentFlag = comment.Flag;
            commentEntity.CommentPosted = comment.Posted;
            commentEntity.CommentRevised = comment.Revised;
            commentEntity.CommentResolved = comment.Resolved;
            commentEntity.CommentIsPrivate = comment.IsPrivate;

            person.Modified = DateTimeOffset.UtcNow;
            person.ModifiedBy = originUser;
        }

        private static void DeleteComment(
            QPerson person,
            Guid originUser,
            Guid commentId,
            Func<Guid, QComment> getComments,
            Action<QComment> removeComment)
        {
            var commentEntity = getComments(commentId);
            if (commentEntity == null)
                return;

            removeComment(commentEntity);

            person.Modified = DateTimeOffset.UtcNow;
            person.ModifiedBy = originUser;
        }

        private static void UpdateAddress(
            QPerson person,
            Guid originUser,
            AddressType addressType,
            PersonAddress address,
            Func<Guid, QPersonAddress> getAddress,
            Action<QPersonAddress> addAddress
            )
        {
            bool isNew;
            switch (addressType)
            {
                case AddressType.Billing:
                    isNew = person.BillingAddressIdentifier == null;
                    person.BillingAddressIdentifier = address.Identifier;
                    break;
                case AddressType.Shipping:
                    isNew = person.ShippingAddressIdentifier == null;
                    person.ShippingAddressIdentifier = address.Identifier;
                    break;
                case AddressType.Work:
                    isNew = person.WorkAddressIdentifier == null;
                    person.WorkAddressIdentifier = address.Identifier;
                    break;
                case AddressType.Home:
                    isNew = person.HomeAddressIdentifier == null;
                    person.HomeAddressIdentifier = address.Identifier;
                    break;
                default:
                    throw new ArgumentException($"Unsupported address type: {addressType}");
            }

            var addressEntity = !isNew ? getAddress(address.Identifier) : null;
            if (addressEntity == null)
                addAddress(addressEntity = new QPersonAddress { AddressIdentifier = address.Identifier });

            addressEntity.City = address.City;
            addressEntity.Country = address.Country;
            addressEntity.Description = address.Description;
            addressEntity.PostalCode = address.PostalCode;
            addressEntity.Province = address.Province;
            addressEntity.Street1 = address.Street1;
            addressEntity.Street2 = address.Street2;

            person.Modified = DateTimeOffset.UtcNow;
            person.ModifiedBy = originUser;
        }

        private static void DeleteAddress(QPerson person, Guid originUser, AddressType addressType, Func<Guid, QPersonAddress> getAddress, Action<QPersonAddress> removeAddress)
        {
            Guid? addressId;
            switch (addressType)
            {
                case AddressType.Billing:
                    addressId = person.BillingAddressIdentifier;
                    person.BillingAddressIdentifier = null;
                    break;
                case AddressType.Shipping:
                    addressId = person.ShippingAddressIdentifier;
                    person.ShippingAddressIdentifier = null;
                    break;
                case AddressType.Work:
                    addressId = person.WorkAddressIdentifier;
                    person.WorkAddressIdentifier = null;
                    break;
                case AddressType.Home:
                    addressId = person.HomeAddressIdentifier;
                    person.HomeAddressIdentifier = null;
                    break;
                default:
                    throw new ArgumentException($"Unsupported address type: {addressType}");
            }

            if (addressId == null)
                return;

            var addressEntity = getAddress(addressId.Value);
            if (addressEntity == null)
                return;

            removeAddress(addressEntity);

            person.Modified = DateTimeOffset.UtcNow;
            person.ModifiedBy = originUser;
        }
    }
}