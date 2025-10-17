using System;

namespace Common.Timeline.Changes
{
    [Serializable]
    public class ChangeNotFoundException : Exception
    {
        public ChangeNotFoundException(string @class) 
            : base($"This change class is not found ({@class}).")
        {
        }
    }
}