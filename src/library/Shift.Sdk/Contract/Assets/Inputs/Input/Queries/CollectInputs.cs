using System;
using System.Collections.Generic;

using Shift.Common;

namespace Shift.Contract
{
    public class CollectInputs : Query<IEnumerable<InputModel>>, IInputCriteria
    {
        public Guid? OrganizationId { get; set; }

        public Guid? ContainerId { get; set; }
        public Guid[] ContainerIds { get; set; }

        public string ContainerType { get; set; }
        public string ContentLabel { get; set; }
        public string ContentLanguage { get; set; }
    }
}
