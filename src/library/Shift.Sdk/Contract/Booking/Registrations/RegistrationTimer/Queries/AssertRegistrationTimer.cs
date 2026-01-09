using System;

using Shift.Common;

namespace Shift.Contract
{
    public class AssertRegistrationTimer : Query<bool>
    {
        public Guid TriggerCommand { get; set; }
    }
}