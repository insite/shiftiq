using System;

namespace Shift.Common
{
    public static class UniqueIdentifier
    {
        public static Guid Create()
            => UuidFactory.CreateV7();
    }
}
