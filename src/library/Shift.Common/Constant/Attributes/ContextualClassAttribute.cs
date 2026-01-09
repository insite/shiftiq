using System;

namespace Shift.Constant
{
    [AttributeUsage(AttributeTargets.All)]
    public class ContextualClassAttribute : Attribute
    {
        #region Constants

        public static readonly ContextualClassAttribute Default = new ContextualClassAttribute();

        #endregion

        #region Properties

        public string Name
        {
            get;
            private set;
        }

        #endregion

        #region Construction

        public ContextualClassAttribute()
            : this(string.Empty)
        {
        }

        public ContextualClassAttribute(string name)
        {
            Name = name;
        }

        #endregion

        #region Methods (comparings)

        public override bool Equals(object obj)
        {
            return obj == this || obj is ContextualClassAttribute attr && attr.Name == Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        #endregion

        #region Methods (overrides)

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }

        #endregion
    }
}
