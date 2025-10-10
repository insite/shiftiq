using System;

namespace Shift.Contract.Requests
{
    public class PermissionSearchRequest : PagedRequest
    {
    
    }

    public class PermissionCreateRequest
    {
        public Guid Identifier { get; set; }
        public Guid Group { get; set; }
        public Guid Object { get; set; }

        public string ObjectType { get; set; }

        public Timestamp Granted { get; set; }
        public Access Access { get; set; }
    }

    public class PermissionUpdateRequest
    {
        public Timestamp Granted { get; set; }
        public Access Access { get; set; }
    }
}