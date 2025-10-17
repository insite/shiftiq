using System;

namespace Shift.Common
{
    [Flags]
    public enum BasicAccess
    {
        Deny = 0,
        Allow = 1,
        All = Allow
    }

    [Flags]
    public enum DataAccess
    {
        None = 0,
        Read = 1 << 0,
        Write = 1 << 1,
        Create = 1 << 2,
        Delete = 1 << 3,
        Administrate = 1 << 4,
        Configure = 1 << 5,
        All = Read | Write | Create | Delete | Administrate | Configure
    }

    [Flags]
    public enum HttpAccess
    {
        None = 0,
        Head = 1 << 0,
        Get = 1 << 1,
        Put = 1 << 2,
        Post = 1 << 3,
        Delete = 1 << 4,
        All = Head | Get | Put | Post | Delete
    }

    [Flags]
    public enum AuthorityAccess
    {
        None = 0,
        Learner = 1 << 0,
        Administrator = 1 << 1,
        Developer = 1 << 2,
        Operator = 1 << 3,
        All = Learner | Administrator | Developer | Operator
    }
}