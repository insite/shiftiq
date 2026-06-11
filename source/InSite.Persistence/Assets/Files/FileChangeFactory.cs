using System;
using System.Collections.Generic;

using InSite.Application.Files.Read;

using Shift.Common;

namespace InSite.Persistence
{
    public class FileChangeFactory : IFileChangeFactory
    {
        private delegate FileChange GetChangeHandler(FileProperties oldProps, FileProperties newProps);

        private readonly Func<Guid, string> _userNameResolver;
        private readonly GetChangeHandler[] _changeHandlers;

        public FileChangeFactory(Func<Guid, string> userNameResolver)
        {
            _userNameResolver = userNameResolver;

            _changeHandlers = new GetChangeHandler[]
            {
                (oldProps, newProps) => GetChange("Document Name", oldProps.DocumentName, newProps.DocumentName),
                (oldProps, newProps) => GetChange("Description", oldProps.Description, newProps.Description),
                (oldProps, newProps) => GetChange("Document Type", oldProps.Category, newProps.Category),
                (oldProps, newProps) => GetChange("Document Subtype", oldProps.Subcategory, newProps.Subcategory),
                (oldProps, newProps) => GetChange("Status", oldProps.Status, newProps.Status),
                (oldProps, newProps) => GetChange("Expiry Date", oldProps.Expiry, newProps.Expiry),
                (oldProps, newProps) => GetChange("Date Received", oldProps.Received, newProps.Received),
                (oldProps, newProps) => GetChange("Alternate Date", oldProps.Alternated, newProps.Alternated),
                (oldProps, newProps) => GetChange("Reviewed Date", oldProps.ReviewedTime, newProps.ReviewedTime),
                (oldProps, newProps) => GetChange("Reviewed By", oldProps.ReviewedUserIdentifier, newProps.ReviewedUserIdentifier),
                (oldProps, newProps) => GetChange("Approved Date", oldProps.ApprovedTime, newProps.ApprovedTime),
                (oldProps, newProps) => GetChange("Approved By", oldProps.ApprovedUserIdentifier, newProps.ApprovedUserIdentifier),
                (oldProps, newProps) => GetChange("Allow Learner to Open File", oldProps.AllowLearnerToView, newProps.AllowLearnerToView),
            };
        }

        public FileChange[] CreateChanges(FileStorageModel oldModel, FileStorageModel newModel)
        {
            var oldProps = oldModel.Properties;
            var newProps = newModel.Properties;

            var changes = new List<FileChange>();
            foreach (var changeHandler in _changeHandlers)
            {
                var change = changeHandler.Invoke(oldProps, newProps);
                if (change != null)
                    changes.Add(change);
            }

            return changes.ToArray();
        }

        private FileChange GetChange(string fieldName, Guid? oldUser, Guid? newUser)
        {
            if (oldUser == newUser)
                return null;

            var oldUserName = oldUser != null ? _userNameResolver.Invoke(oldUser.Value) : null;
            var newUserName = newUser != null ? _userNameResolver.Invoke(newUser.Value) : null;

            return new FileChange(fieldName, oldUserName, newUserName);
        }

        private static FileChange GetChange(string fieldName, string oldValue, string newValue)
        {
            oldValue = oldValue.NullIfWhiteSpace();
            newValue = newValue.NullIfWhiteSpace();

            return !string.Equals(oldValue, newValue)
                ? new FileChange(fieldName, oldValue, newValue)
                : null;
        }

        private static FileChange GetChange(string fieldName, DateTimeOffset? oldValue, DateTimeOffset? newValue)
        {
            return oldValue != newValue
                ? new FileChange(fieldName, oldValue, newValue)
                : null;
        }

        private static FileChange GetChange(string fieldName, bool oldValue, bool newValue)
        {
            return oldValue != newValue
                ? new FileChange(fieldName, oldValue, newValue)
                : null;
        }
    }
}
