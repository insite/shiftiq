using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

using InSite.Persistence;

using Shift.Common;

namespace InSite.UI.Lobby
{
    public class ResetTokenFile
    {
        #region Constants

        public const int ResetTokenFileExpirationTime = 72; // hours

        #endregion

        #region Properties

        public static string StoragePath => ServiceLocator.FilePaths.ApplicationPasswordResetsPendingPath;

        public Guid TokenID { get; private set; }
        public Guid UserIdentifier { get; private set; }
        public string Email { get; private set; }

        #endregion

        #region Methods (I/O)

        public static Guid Create(User user)
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            return TryExecute(() =>
            {
                var id = UniqueIdentifier.Create();
                var path = GetFilePath(id, user.UserIdentifier);

                using (var fs = File.Open(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    using (var sw = new StreamWriter(fs))
                    {
                        sw.WriteLine(user.UserIdentifier);
                        sw.WriteLine(user.Email);
                        sw.WriteLine(DateTimeOffset.UtcNow);
                    }
                }

                return id;
            });
        }

        public static Guid GetOrCreateToken(User user)
        {
            if (!Directory.Exists(StoragePath))
                Directory.CreateDirectory(StoragePath);

            return TryExecute(() =>
            {
                var files = Directory.GetFiles(StoragePath, $"*[{user.UserIdentifier}].txt");
                var reuseThreshold = TimeSpan.FromHours(ResetTokenFileExpirationTime);

                foreach (var file in files)
                {
                    try
                    {
                        var lines = File.ReadAllLines(file);
                        if (lines.Length >= 3 &&
                            Guid.TryParse(lines[0], out var fileUserId) &&
                            DateTimeOffset.TryParse(lines[2], out var timestamp))
                        {
                            if (fileUserId == user.UserIdentifier &&
                                DateTimeOffset.UtcNow - timestamp <= reuseThreshold)
                            {
                                var fileName = Path.GetFileNameWithoutExtension(file);
                                var tokenPart = fileName.Split('[')[0];
                                if (Guid.TryParse(tokenPart, out var existingToken))
                                    return existingToken;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppSentry.SentryError(ex);
                    }
                }

                return Create(user);
            });
        }

        public void Delete()
        {
            Delete(TokenID);
        }

        public static void Delete(Guid id)
        {
            var path = GetFilePath(id);

            TryExecute(() =>
            {
                var isExist = File.Exists(path);

                if (isExist)
                    File.Delete(path);

                return isExist;
            });
        }

        public static void Clear(Func<ResetTokenFile, bool> condition = null)
        {
            if (!Directory.Exists(StoragePath))
                return;

            var files = Directory.GetFiles(StoragePath);
            foreach (var filename in files)
            {
                var tokenFile = ReadPath(filename);
                if (tokenFile != null && condition != null && condition(tokenFile))
                    tokenFile.Delete();
            }
        }

        public static ResetTokenFile Read(Guid token)
        {
            if (token == Guid.Empty)
                return null;

            var path = GetFilePath(token);

            return ReadPath(path);
        }

        private static ResetTokenFile ReadPath(string path)
        {
            return TryExecute(() =>
            {
                if (!File.Exists(path))
                    return null;

                var lines = File.ReadAllLines(path);
                if (lines.Length != 3)
                    return null;

                var contactId = ValueConverter.ToGuidNullable(lines[0]);
                var email = lines[1];
                var utcCreated = ParseDateTime(lines[2]);

                if (!contactId.HasValue || string.IsNullOrEmpty(email) || !utcCreated.HasValue)
                {
                    File.Delete(path);
                    return null;
                }

                var diff = DateTime.UtcNow - utcCreated.Value;
                if (diff.TotalHours > ResetTokenFileExpirationTime)
                {
                    File.Delete(path);
                    return null;
                }

                var tokenPart = path.Split('[')[0];

                return new ResetTokenFile
                {
                    TokenID = Guid.Parse(System.IO.Path.GetFileNameWithoutExtension(tokenPart)),
                    UserIdentifier = contactId.Value,
                    Email = email
                };
            });
        }

        public bool IsValid(out string validationError)
        {
            // A user account has one password only. Therefore, a user should be permitted to change their password if they are granted access to any
            // organization.

            var contact = UserSearch.Bind(UserIdentifier, x => new
            {
                x.UserIdentifier,
                x.Email,
                AccessGranted = x.Persons.Any(y => y.UserAccessGranted.HasValue)
            });

            validationError = null;

            if (contact == null)
                validationError = "user was removed from the system";
            else if (!string.Equals(contact.Email, Email, StringComparison.OrdinalIgnoreCase))
                validationError = "user email was changed";
            else if (!contact.AccessGranted)
                validationError = "user is not granted access to any organization";

            if (validationError == null)
                return true;

            var message = new StringBuilder();
            message
                .Append("The token file exists but ")
                .Append(validationError)
                .Append(": ");

            message.AppendFormat(
                "Token {{ ID={0}, ContactID={1}, Email={2} }}",
                TokenID,
                UserIdentifier,
                Email);

            if (contact != null)
                message.AppendFormat(
                    ", Contact {{ ID={0}, Email={1} }}",
                    contact.UserIdentifier,
                    contact.Email);

            validationError = message.ToString();

            return false;
        }

        #endregion

        #region Helpers

        private static string GetFilePath(Guid id)
        {
            if (!Directory.Exists(StoragePath))
                return null;

            return Directory
                .GetFiles(StoragePath, $"{id}[*].txt")
                .FirstOrDefault();
        }

        private static string GetFilePath(Guid id, Guid userId)
        {
            return System.IO.Path.Combine(StoragePath, $"{id}[{userId}].txt");
        }

        private static DateTime? ParseDateTime(string s)
        {
            return !string.IsNullOrEmpty(s) && DateTime.TryParse(s, out var parsed) ? parsed : (DateTime?)null;
        }

        private static T TryExecute<T>(Func<T> action)
        {
            Exception lastException = null;

            for (var i = 0; i < 5; i++)
                try
                {
                    return action();
                }
                catch (Exception ex)
                {
                    lastException = ex;

                    Thread.Sleep(500);
                }

            throw lastException ?? new Exception("Unable execute");
        }

        #endregion
    }
}