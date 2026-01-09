using System;

using Shift.Common;

namespace Shift.Contract
{
    public class RetrieveRegistrationTimer : Query<RegistrationTimerModel>
    {
        public Guid TriggerCommand { get; set; }
    }
}