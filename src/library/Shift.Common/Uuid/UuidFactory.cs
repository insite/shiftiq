using System;
using System.Security.Cryptography;
using System.Text;

namespace Shift.Common
{
    public class UuidFactory
    {
        /// <summary>
        /// Generates a Version 7 UUID value. This is the default version.
        /// </summary>
        public static Guid Create()
            => CreateV7();

        /// <summary>
        /// Generates a Version 3 UUID value.
        /// </summary>
        /// <remarks>
        /// Version-3 UUID values are generated based on a unique "namespace" and a unique "name". 
        /// Namespace and name are concatenated and hashed. There is no temporal or random 
        /// component, so the same input always produces the same output. In other words, this 
        /// allows the caller to generate a unique deterministic hash for an arbitrary string value.
        /// The UUID specification establishes these 4 pre-defined namespaces:
        ///   DNS (fully qualified domain names) = 6ba7b810-9dad-11d1-80b4-00c04fd430c8
        ///   URL (URLs)                         = 6ba7b811-9dad-11d1-80b4-00c04fd430c8
        ///   OID (object identifiers)           = 6ba7b812-9dad-11d1-80b4-00c04fd430c8
        ///   XDN (X.500 distinguished names)    = 6ba7b814-9dad-11d1-80b4-00c04fd430c8
        /// </remarks>
        public static Guid CreateV3(Guid space, string name)
        {
            if (space == Guid.Empty)
                throw new ArgumentException("Namespace UUID cannot be empty.", nameof(space));

            if (name.IsEmpty())
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            // Convert namespace UUID to a byte array
            var spaceBytes = space.ToByteArray();

            // Convert the namespace UUID to big-endian (network byte order)
            SwapByteOrder(spaceBytes);

            // Convert the name to a byte array
            var nameBytes = Encoding.UTF8.GetBytes(name);

            // Combine namespace and name into one byte array
            var combined = new byte[spaceBytes.Length + nameBytes.Length];
            Buffer.BlockCopy(spaceBytes, 0, combined, 0, spaceBytes.Length);
            Buffer.BlockCopy(nameBytes, 0, combined, spaceBytes.Length, nameBytes.Length);

            // Generate the MD5 hash of the combined bytes
            using (MD5 md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(combined);

                // Set the version to 3 (UUIDv3)
                hash[6] = (byte)((hash[6] & 0x0F) | (3 << 4));

                // Set the variant to RFC 4122
                hash[8] = (byte)((hash[8] & 0x3F) | 0x80);

                // Convert the hash to a GUID
                SwapByteOrder(hash);

                return new Guid(hash);
            }
        }

        public static Guid CreateV4()
            => Guid.NewGuid();

        /// <summary>
        /// Generates a Version 5 UUID value.
        /// </summary>
        /// <remarks>
        /// Version-5 UUID values are generated based on a unique "namespace" and a unique "name". 
        /// Namespace and name are concatenated and hashed. There is no temporal or random 
        /// component, so the same input always produces the same output. In other words, this 
        /// allows the caller to generate a unique deterministic hash for an arbitrary string value.
        /// The UUID specification establishes these 4 pre-defined namespaces:
        ///   DNS (fully qualified domain names) = 6ba7b810-9dad-11d1-80b4-00c04fd430c8
        ///   URL (URLs)                         = 6ba7b811-9dad-11d1-80b4-00c04fd430c8
        ///   OID (object identifiers)           = 6ba7b812-9dad-11d1-80b4-00c04fd430c8
        ///   XDN (X.500 distinguished names)    = 6ba7b814-9dad-11d1-80b4-00c04fd430c8
        /// </remarks>
        public static Guid CreateV5(Guid space, string name)
        {
            if (space == Guid.Empty)
                throw new ArgumentException("Namespace UUID cannot be empty.", nameof(space));

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));

            // Convert namespace UUID to a byte array
            var namespaceBytes = space.ToByteArray();

            // Convert namespace UUID to big-endian (network byte order)
            SwapByteOrder(namespaceBytes);

            // Convert the name to a byte array (UTF-8 encoding)
            var nameBytes = Encoding.UTF8.GetBytes(name);

            // Combine the namespace bytes and the name bytes
            var data = new byte[namespaceBytes.Length + nameBytes.Length];
            Buffer.BlockCopy(namespaceBytes, 0, data, 0, namespaceBytes.Length);
            Buffer.BlockCopy(nameBytes, 0, data, namespaceBytes.Length, nameBytes.Length);

            // Hash the combined data using SHA-1
            byte[] hash;
            using (SHA1 sha1 = SHA1.Create())
            {
                hash = sha1.ComputeHash(data);
            }

            // Create a new UUID from the first 16 bytes of the hash
            var uuidBytes = new byte[16];
            Array.Copy(hash, 0, uuidBytes, 0, 16);

            // Set the version to 5 (UUIDv5)
            uuidBytes[6] = (byte)((uuidBytes[6] & 0x0F) | 0x50);

            // Set the variant to RFC 4122
            uuidBytes[8] = (byte)((uuidBytes[8] & 0x3F) | 0x80);

            // Convert back to little-endian before returning as Guid
            SwapByteOrder(uuidBytes);

            return new Guid(uuidBytes);
        }

        /// <remarks>
        /// The UUID DNS namespace is specifically designed for domain-name-based identifiers like 
        /// email addresses, which typically follow the user@domain.com pattern. This ensures 
        /// consistency and standardization when generating UUIDs for email addresses.
        /// </remarks>
        public static Guid CreateV5ForDns(string email)
            => CreateV5(PredefinedUuids.DNS, email);

        public static Guid CreateV5ForUrl(string resource)
            => CreateV5(PredefinedUuids.URL, resource);

        /// <summary>
        /// Generates a Version 7 (timestamp and random) UUID value.
        /// </summary>
        /// <remarks>
        /// The Guid.NewGuid() method generates a Version 4 UUID, which is suitable for scenarios 
        /// where randomness is sufficient and there are no strict requirements for sequential or 
        /// time-based ordering. Version 7 UUIDs (UUIDv7) are designed for keys in high-load 
        /// databases and distributed systems. For more information about UUIDv7 values, refer to
        /// https://en.wikipedia.org/wiki/Universally_unique_identifier#Version_7_(timestamp_and_random)
        /// </remarks>
        public static Guid CreateV7()
        {
            // Get current timestamp in milliseconds since Unix epoch
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Convert timestamp to byte array
            var timestampBytes = BitConverter.GetBytes(timestamp);

            // Ensure big-endian order for timestamp
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(timestampBytes);
            }

            // Create a 16-byte array for the UUID
            var uuidBytes = new byte[16];

            // Set the first 6 bytes to the timestamp (most significant bits)
            Array.Copy(timestampBytes, 2, uuidBytes, 0, 6);

            // Fill the remaining bytes with random data
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(uuidBytes, 6, 10);
            }

            // Set the version to 7 (in the 4 most significant bits of the 7th byte)
            uuidBytes[6] = (byte)((uuidBytes[6] & 0x0F) | 0x70);

            // Set the variant to RFC 4122 (most significant bits of the 9th byte)
            uuidBytes[8] = (byte)((uuidBytes[8] & 0x3F) | 0x80);

            // Return the UUID as a Guid
            return new Guid(uuidBytes);
        }

        public static int GetVersion(Guid guid)
        {
            // Convert the GUID to a byte array
            var guidBytes = guid.ToByteArray();

            // The version number is stored in the high nibble (4 bits) of the 7th byte
            var versionByte = guidBytes[7];

            // Extract the version (shift right 4 bits to get the high nibble)
            var version = (versionByte >> 4) & 0xF;

            return version;
        }

        private static void SwapByteOrder(byte[] bytes)
        {
            if (BitConverter.IsLittleEndian)
            {
                // Swap the first three groups of bytes
                Array.Reverse(bytes, 0, 4); // Time-low
                Array.Reverse(bytes, 4, 2); // Time-mid
                Array.Reverse(bytes, 6, 2); // Time-high-and-version
            }
        }
    }
}