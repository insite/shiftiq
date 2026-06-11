using System;

namespace InSite.Persistence.Plugin.NCSHA
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class NcshaHistoryEventAttribute : Attribute
    {
        #region Properties

        public string Type { get; }

        public string Name { get; }

        public bool Visible { get; }

        #endregion

        #region Construction

        public NcshaHistoryEventAttribute(string type, string name, bool visible)
        {
            Type = type;
            Name = name;
            Visible = visible;
        }

        #endregion
    }
}
