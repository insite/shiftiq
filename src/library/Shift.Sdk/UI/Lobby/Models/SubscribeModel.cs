using System;

namespace Shift.Sdk.UI
{
    [Serializable]
    public class SubscribeModel
    {
        public GroupListItem[] Groups { get; set; }

        [Serializable]
        public class GroupListItem
        {
            public Guid Identifier { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
        }
    }
}