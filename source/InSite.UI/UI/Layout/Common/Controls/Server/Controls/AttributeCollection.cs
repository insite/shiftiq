using System.Collections;
using System.Web.UI;

using Shift.Common.Events;

namespace InSite.Common.Web.UI
{
    public class AttributeCollection
    {
        #region Events

        internal event AttributeEventHandler AttributeAdd;
        private bool OnAttributeAdd(string key, string value)
        {
            var result = true;

            if (AttributeAdd != null)
            {
                var args = new AttributeEventArgs(key, value);

                AttributeAdd(this, args);

                result = !args.Cancel;
            }

            return result;
        }

        internal event AttributeEventHandler AttributeSet;
        private bool OnAttributeSet(string key, string value)
        {
            var result = true;

            if (AttributeSet != null)
            {
                var args = new AttributeEventArgs(key, value);

                AttributeSet(this, args);

                result = !args.Cancel;
            }

            return result;
        }

        internal event AttributeEventHandler AttributeRemove;
        private bool OnAttributeRemove(string key, string value)
        {
            var result = true;

            if (AttributeRemove != null)
            {
                var args = new AttributeEventArgs(key, value);

                AttributeRemove(this, args);

                result = !args.Cancel;
            }

            return result;
        }

        #endregion

        #region Properties

        public string this[string key]
        {
            get { return (string)_bag[key]; }
            set { Set(key, value); }
        }

        public ICollection Keys => _bag.Keys;

        public ICollection Values => _bag.Values;

        public int Count => _bag.Count;

        #endregion

        #region Fields

        private StateBag _bag;

        #endregion

        #region Construction

        public AttributeCollection(StateBag bag)
        {
            _bag = bag;
        }

        #endregion

        #region Methods

        public void Add(string key, string value)
        {
            if (!OnAttributeAdd(key, value))
                return;

            _bag.Add(key, value);
        }

        public void Set(string key, string value)
        {
            if (!OnAttributeSet(key, value))
                return;

            _bag[key] = value;
        }

        public bool Remove(string key)
        {
            var value = (string)_bag[key];
            if (value == null)
                return false;

            if (!OnAttributeRemove(key, value))
                return false;

            _bag.Remove(key);

            return true;
        }

        #endregion
    }
}