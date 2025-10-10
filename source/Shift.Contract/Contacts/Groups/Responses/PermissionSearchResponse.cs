using System;

namespace Shift.Contract.Responses
{
    public class PermissionSearchResponse
    {
        public Guid Identifier { get; set; }

        public ObjectResponse Group { get; set; }
        public ObjectResponse Object { get; set; }
        public Timestamp Granted { get; set; }
        public Access Access { get; set; }

        public int Mask { get; set; }
    }

    public class PermissionResponse
    {
        public Guid Identifier { get; set; }
    }
}