using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Shift.Common
{
    public class GuidGenerator : IDisposable
    {
        public static readonly Guid Empty = Guid.Parse("00000000-0000-0000-0000-000000000000");

        private const int Capacity = 999;
        private readonly MD5 _md5 = MD5.Create();
        private readonly HashSet<Guid> _ids = new HashSet<Guid>();

        public Guid New(Type t)
        {
            for (var i = 0; i <= Capacity; i++)
            {
                var name = t.FullName;

                if (i > 0)
                    name += ":" + i;

                var bytes = Encoding.Default.GetBytes(name);
                var hash = _md5.ComputeHash(bytes);
                var id = new Guid(hash);

                if (!_ids.Contains(id))
                    return id;
            }

            throw new Exception($"The system cannot generate more than {Capacity} unique identifiers for this type: {t.FullName}");
        }

        public static Guid NewGuid(Type t)
        {
            using (var gen = new GuidGenerator())
                return gen.New(t);
        }

        /// <summary>
        /// Returns a new GUID that is optimized for indexing.
        /// </summary>
        public static Guid NewGuid()
        {
            var result = Guid.NewGuid().ToByteArray();

            var start = new DateTime(1900, 1, 1);
            var now = DateTime.UtcNow;
            var time = now.TimeOfDay;
            var span = new TimeSpan(now.Ticks - start.Ticks);

            var bytes = BitConverter.GetBytes(span.Days);
            Array.Reverse(bytes);

            var array = BitConverter.GetBytes((long)(time.TotalMilliseconds / 3.333333));
            Array.Reverse(array);

            Array.Copy(bytes, bytes.Length - 2, result, result.Length - 6, 2);
            Array.Copy(array, array.Length - 4, result, result.Length - 4, 4);

            return new Guid(result);
        }

        public static Guid NewSequentialGuid() => UuidFactory.CreateV7();

        public void Dispose()
        {
            if (_md5 != null)
                _md5.Dispose();
        }
    }
}