using System;

using InSite.Application.Registrations.Read;

namespace InSite.Persistence.Plugin.SkilledTradesBC
{
    public class RegistrationPacket
    {
        public Guid Identifier { get; set; }

        public RegistrationPacket(Guid id) { Identifier = id; }

        public QRegistration Registration { get; set; }
    }
}
