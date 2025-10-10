
using System;

namespace Shift.Contract.Requests
{
    public class UserSearchRequest : PagedRequest
    {
    
    }

    public class UserCreateRequest
    {
        public Guid Identifier { get; set; }
    }

    public class UserUpdateRequest
    {

    }
}