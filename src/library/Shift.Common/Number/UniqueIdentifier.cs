using System;

namespace Shift.Common
{
    public static class UniqueIdentifier
    {
        public static readonly Guid Empty = Guid.Parse("00000000-0000-0000-0000-000000000000");

        public static Guid Create()
            => GuidGenerator.NewGuid();

        public static Guid NewSequentialGuid()
            => GuidGenerator.NewSequentialGuid();
    }
}
