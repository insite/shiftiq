using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrievePermission : Query<PermissionModel>
    {
        public Guid PermissionIdentifier { get; set; }
    }
}