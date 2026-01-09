using System.Runtime.CompilerServices;
using System.Web.UI;

namespace InSite.Common.Web.UI
{
    public abstract class StateBagProxy
    {
        #region Fields

        private string _prefix;
        private StateBag _viewState;

        #endregion

        #region Construction

        public StateBagProxy(string prefix, StateBag viewState)
        {
            _prefix = prefix + ".";
            _viewState = viewState;
        }

        #endregion

        #region Helper methods

        protected object GetValue([CallerMemberName] string name = null) => _viewState[_prefix + name];

        protected void SetValue(object value, [CallerMemberName] string name = null) => _viewState[_prefix + name] = value;

        #endregion
    }
}