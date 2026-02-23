using System;

namespace Shift.Common
{
    public interface IPartitionModel
    {
        /// <summary>
        /// Partition number in the range 0..99
        /// </summary>
        int Number { get; set; }

        /// <summary>
        /// Product brand name for the partition (e.g., "Shift iQ")
        /// </summary>
        string Brand { get; set; }

        /// <summary>
        /// Cascading stylesheet name (e.g. "shift.css")
        /// </summary>
        string Style { get; }

        /// <summary>
        /// Top-level domain name for the partition (e.g. "insite.com", "shiftiq.com")
        /// </summary>
        string Domain { get; set; }

        /// <summary>
        /// Email address for support requests submitted by users working in the partition
        /// </summary>
        string Email { get; set; }

        /// <summary>
        /// Short, unique, descriptive name for the partition
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Tenant identifier for the partition in the CI/CD automation pipeline (Octopus)
        /// </summary>
        string Tenant { get; }

        /// <summary>
        /// Organization account identifier for the organization in which partition-wide data is managed
        /// </summary>
        Guid Identifier { get; set; }

        /// <summary>
        /// Organization account code for the organization in which partition-wide data is managed
        /// </summary>
        /// <remarks>
        /// This may or may not match the Octopus Tenant identifier.
        /// </remarks>
        string Slug { get; set; }

        /// <summary>
        /// Comma-separated list of top-level domain names whitelisted for bulk mailouts
        /// </summary>
        string WhitelistDomains { get; set; }

        /// <summary>
        /// Comma-separated list of individual email addresses whitelisted for bulk mailouts
        /// </summary>
        string WhitelistEmails { get; set; }

        bool IsE01();
        bool IsE02();
        bool IsE03();
        bool IsE04();
        bool IsE07();
    }
}