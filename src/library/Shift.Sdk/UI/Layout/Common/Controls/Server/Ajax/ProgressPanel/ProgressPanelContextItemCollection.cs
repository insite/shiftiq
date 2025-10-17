using System.Collections.Generic;
using System.IO;

namespace Shift.Sdk.UI
{
    public class ProgressPanelContextItemCollection : ProgressPanelContextBaseCollection
    {
        #region Properties

        public override int Count => _items.Count;

        public ProgressPanelContextItem this[string id]
        {
            get
            {
                for (var i = 0; i < _items.Count; i++)
                {
                    var item = _items[i];
                    if (item.Id == id)
                        return item;
                }

                return null;
            }
        }

        #endregion

        #region Fields

        private List<ProgressPanelContextItem> _items;

        #endregion

        #region Construction

        public ProgressPanelContextItemCollection(IEnumerable<ProgressPanelContextItem> items)
        {
            _items = new List<ProgressPanelContextItem>(items);
        }

        #endregion

        #region Methods 

        internal override void ToJson(TextWriter output)
        {
            output.Write("{");

            for (var i = 0; i < _items.Count; i++)
            {
                if (i > 0)
                    output.Write(",");

                _items[i].ToJson(output);
            }

            output.Write("}");
        }

        #endregion
    }
}