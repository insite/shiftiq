using System;
using System.Collections.Generic;
using System.Linq;

using InSite.Application.Files.Read;

using Shift.Constant;

namespace InSite.Persistence
{
    /// <summary>
    /// A credential keeps the files of the learner's last submission, which can carry several
    /// certificates when an achievement needs more than one, and the certificates of the cycles
    /// the learner has since renewed, which are its record for audits. The portal wizard that adds
    /// a certificate used to attach its one file at the first step, so a learner who cancelled and
    /// started again left the first file on the credential, and a resubmission kept the previous
    /// file beside the new one. The certificate grids then showed whichever row SQL Server returned
    /// first, and for a UUIDv7 key that is a coin toss.
    /// </summary>
    public static class CredentialFiles
    {
        private static readonly string CredentialObjectType = FileObjectType.Credential.ToString();

        /// <summary>
        /// True when a file belongs to a cycle before the credential's current grant. A
        /// certificate cannot be uploaded before it was issued, so a file uploaded earlier is the
        /// certificate of a cycle the learner has since renewed. A self-declared grant is stored at
        /// 06:00 on the issue date in the learner's time zone, so a learner can upload a
        /// certificate hours before its own grant, and the day of slack keeps it in its cycle.
        /// </summary>
        public static bool IsEarlierCycle(DateTimeOffset uploaded, DateTimeOffset? granted) =>
            granted.HasValue && uploaded < granted.Value.AddDays(-1);

        /// <summary>
        /// True when an administrator accepted the credential's last submission: it is Valid, or
        /// it was and has since expired. A Submitted credential is waiting for review and a Pending
        /// one was rejected, so only their files are the learner's to replace.
        /// </summary>
        public static bool IsAccepted(string status) =>
            status == CredentialStatus.Valid.ToString()
            || status == CredentialStatus.Expired.ToString();

        /// <summary>
        /// True when an administrator accepted the file. The credential's status says only whether
        /// its last submission was accepted, and the next submission overwrites it, so Attach
        /// stamps the files of an accepted submission before it adds to them. Without the stamp a
        /// certificate accepted days before its renewal was issued, or uploaded late for a cycle
        /// already over, reads as part of the renewal and is deleted when the renewal is replaced.
        /// </summary>
        public static bool IsAcceptedFile(FileStorageModel file) =>
            file.Properties?.ApprovedTime != null;

        /// <summary>
        /// True when every upload is still temporary, so none has expired or been attached yet.
        /// </summary>
        public static bool CanAttach(IReadOnlyCollection<Guid> uploads)
        {
            var storage = ServiceLocator.StorageService;

            return uploads.Count > 0
                && uploads.All(x => storage.GetFile(x)?.ObjectType == FileObjectType.Temporary);
        }

        /// <summary>
        /// Attaches a submission's temporary uploads to a credential and deletes the files of the
        /// submission they replace. Attaches nothing and returns null unless every upload is still
        /// temporary, so a credential never holds part of one submission and part of another.
        /// Call it after the commands that grant the credential, with the status the credential
        /// had before them: a file is replaced only when nobody accepted it and it is in the cycle
        /// of the new grant, so both an approval and an earlier cycle keep it. An approval is
        /// stamped on the files here, so it still keeps them after a later submission is rejected.
        /// </summary>
        public static List<FileStorageModel> Attach(
            Guid organization, Guid credential, IReadOnlyCollection<Guid> uploads,
            string previousStatus)
        {
            if (!CanAttach(uploads))
                return null;

            var storage = ServiceLocator.StorageService;

            // Stamp before attaching, so the uploads are never among the accepted files.
            var accepted = IsAccepted(previousStatus);
            if (accepted)
                MarkAccepted(organization, credential);

            foreach (var upload in uploads)
                storage.ChangeObject(upload, credential, FileObjectType.Credential);

            // Delete only after the new files are attached, so a failure leaves the old ones.
            if (!accepted)
                DeleteReplaced(organization, credential, uploads);

            return uploads.Select(storage.GetFile).ToList();
        }

        private static void MarkAccepted(Guid organization, Guid credential)
        {
            var files = ServiceLocator.FileSearch
                .GetModels(organization, credential, null, false)
                .Where(x => x.ObjectType == FileObjectType.Credential && !IsAcceptedFile(x))
                .ToList();

            foreach (var x in files)
            {
                // Read through the storage service, whose cached copy the change replaces.
                var properties = ServiceLocator.StorageService.GetFile(x.FileIdentifier).Properties
                    ?? new FileProperties { DocumentName = x.FileName };

                properties.ApprovedTime = DateTimeOffset.UtcNow;

                ServiceLocator.StorageService
                    .ChangeProperties(x.FileIdentifier, x.UserIdentifier, properties, false);
            }
        }

        private static void DeleteReplaced(
            Guid organization, Guid credential, IReadOnlyCollection<Guid> uploads)
        {
            var granted = ServiceLocator.AchievementSearch
                .GetCredential(credential)?.CredentialGranted;

            var replaced = ServiceLocator.FileSearch
                .GetModels(organization, credential, null, false)
                .Where(x => x.ObjectType == FileObjectType.Credential
                    && !uploads.Contains(x.FileIdentifier))
                .Where(x => !IsAcceptedFile(x) && !IsEarlierCycle(x.Uploaded, granted))
                .ToList();

            foreach (var x in replaced)
                ServiceLocator.StorageService.Delete(x.FileIdentifier);
        }

        /// <summary>
        /// The files of the credential's current cycle, which are the ones a certificate grid
        /// offers. The files of earlier cycles stay on the credential as its record, and the Edit
        /// Education &amp; Training screen lists them. A file accepted before a later submission
        /// is part of that record too, so it gives way to the files submitted after it.
        /// </summary>
        public static List<FileStorageModel> InCurrentCycle(
            IEnumerable<FileStorageModel> files, DateTimeOffset? granted)
        {
            var current = files?
                .Where(x => !IsEarlierCycle(x.Uploaded, granted))
                .ToList();

            if (current == null || current.All(IsAcceptedFile))
                return current;

            return current.Where(x => !IsAcceptedFile(x)).ToList();
        }

        /// <summary>
        /// Groups files by the object they belong to, each group in the order it was uploaded, so
        /// a grid lists a credential's certificates the way the learner submitted them.
        /// </summary>
        public static Dictionary<Guid, List<FileStorageModel>> GroupByCredential(
            IEnumerable<FileStorageModel> files)
        {
            return files
                .GroupBy(x => x.ObjectIdentifier)
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .OrderBy(f => f.Uploaded)
                        .ThenBy(f => f.FileIdentifier)
                        .ToList());
        }

        /// <summary>
        /// The files the old wizard left behind. It took one file per submission, so every file
        /// but the newest on a credential came from an earlier submission and is superseded,
        /// except on an exempt achievement, where learners ran the wizard once for each of the
        /// certificates it needs, and except a certificate from an earlier renewal cycle.
        /// Orphaned files point at a credential that does not exist, because the learner
        /// cancelled before Save created it, or because it was deleted later.
        /// </summary>
        public static (Guid[] Superseded, Guid[] Orphaned) SelectStrayFiles(
            IReadOnlyCollection<Guid> exemptAchievements)
        {
            using (var db = new InternalDbContext())
            {
                var files = db.TFiles
                    .Where(x => x.ObjectType == CredentialObjectType)
                    .Select(x => new
                    {
                        x.FileIdentifier,
                        x.ObjectIdentifier,
                        x.FileUploaded,
                        AchievementIdentifier = db.QCredentials
                            .Where(c => c.CredentialIdentifier == x.ObjectIdentifier)
                            .Select(c => (Guid?)c.AchievementIdentifier)
                            .FirstOrDefault(),
                        CredentialGranted = db.QCredentials
                            .Where(c => c.CredentialIdentifier == x.ObjectIdentifier)
                            .Select(c => c.CredentialGranted)
                            .FirstOrDefault()
                    })
                    .ToList();

                // A file from an earlier cycle is the record of that cycle, not a stale copy.
                var superseded = files
                    .Where(x => x.AchievementIdentifier.HasValue
                        && !exemptAchievements.Contains(x.AchievementIdentifier.Value))
                    .Where(x => !IsEarlierCycle(x.FileUploaded, x.CredentialGranted))
                    .GroupBy(x => x.ObjectIdentifier)
                    .SelectMany(g => g
                        .OrderByDescending(x => x.FileUploaded)
                        .ThenByDescending(x => x.FileIdentifier)
                        .Skip(1))
                    .Select(x => x.FileIdentifier)
                    .ToArray();

                var orphaned = files
                    .Where(x => !x.AchievementIdentifier.HasValue)
                    .Select(x => x.FileIdentifier)
                    .ToArray();

                return (superseded, orphaned);
            }
        }
    }
}
