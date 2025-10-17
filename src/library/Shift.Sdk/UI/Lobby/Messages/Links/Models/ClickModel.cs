using System;

namespace Shift.Sdk.UI
{
    public class ClickModel
    {
        public ClickModel(string link, string user)
        {
            if (Guid.TryParse(link, out var linkId))
                Link = linkId;

            if (Guid.TryParse(user, out var userId))
                User = userId;
        }

        public Guid Link { get; set; }
        public Guid User { get; set; }

        public string LinkUrl { get; set; }

        public bool IsValid => Link != Guid.Empty && User != Guid.Empty;
    }
}