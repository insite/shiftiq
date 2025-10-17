using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertPermission : Query<bool>
    {
        public Guid PermissionIdentifier { get; set; }
    }
}