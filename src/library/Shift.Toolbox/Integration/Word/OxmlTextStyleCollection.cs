using System.Collections;
using System.Collections.Generic;
using System.Linq;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Shift.Toolbox
{
    public class OxmlTextStyleCollection : IEnumerable<OxmlTextStyle>
    {
        #region Classes

        public interface IStyleChain
        {
            IStyleChain Add(string id);
            IStyleChain Add(OxmlTextStyle style);
            Styles ToStyles();
            OxmlTextStyleCollection AddToPackage(WordprocessingDocument word);

            IStyleChain BackColor(string color);
            IStyleChain BasedOn(string basedOn);
            IStyleChain Bold(bool isBold);
            IStyleChain FontName(string name);
            IStyleChain FontSize(string size);
            IStyleChain FontColor(string color);
        }

        private class StyleChain : IStyleChain
        {
            #region Fields

            private OxmlTextStyleCollection _collection;
            private readonly Dictionary<string, object> _modifiers = new Dictionary<string, object>();

            #endregion

            #region Construction

            public StyleChain(OxmlTextStyleCollection collection)
            {
                _collection = collection;
            }

            #endregion

            #region IStyleChain

            public IStyleChain Add(string id)
            {
                var style = new OxmlTextStyle(id);

                Add(style);

                return this;
            }

            public IStyleChain Add(OxmlTextStyle style)
            {
                foreach (var kv in _modifiers)
                    style.SetIfEmpty(kv.Key, kv.Value);

                _collection.Add(style);

                return this;
            }

            public Styles ToStyles()
            {
                return _collection.ToStyles();
            }

            public OxmlTextStyleCollection AddToPackage(WordprocessingDocument word)
            {
                _collection.AddToPackage(word);

                return _collection;
            }

            public IStyleChain BackColor(string color)
            {
                SetModifier(nameof(OxmlTextStyle.BackColor), color);

                return this;
            }

            public IStyleChain BasedOn(string basedOn)
            {
                SetModifier(nameof(OxmlTextStyle.BasedOn), "Normal");

                return this;
            }

            public IStyleChain Bold(bool isBold)
            {
                SetModifier(nameof(OxmlTextStyle.Bold), isBold);

                return this;
            }

            public IStyleChain FontName(string name)
            {
                SetModifier(nameof(OxmlTextStyle.FontName), name);

                return this;
            }

            public IStyleChain FontSize(string size)
            {
                SetModifier(nameof(OxmlTextStyle.FontSize), size);

                return this;
            }

            public IStyleChain FontColor(string color)
            {
                SetModifier(nameof(OxmlTextStyle.FontColor), color);

                return this;
            }

            #endregion

            #region Methods (helpers)

            private void SetModifier(string key, object value)
            {
                if (!_modifiers.ContainsKey(key))
                    _modifiers.Add(key, value);
                else
                    _modifiers[key] = value;
            }

            #endregion
        }

        #endregion

        #region Fields

        private List<OxmlTextStyle> _items = new List<OxmlTextStyle>();

        #endregion

        #region Methods

        public void Add(OxmlTextStyle style)
        {
            _items.Add(style);
        }

        public IStyleChain Chain()
        {
            return new StyleChain(this);
        }

        public Styles ToStyles()
        {
            return new Styles(_items.Select(x => x.ToStyle()));
        }

        public void AddToPackage(WordprocessingDocument word)
        {
            if (word.MainDocumentPart.StyleDefinitionsPart == null)
                word.MainDocumentPart.AddNewPart<StyleDefinitionsPart>().Styles = new Styles();

            foreach (var item in _items)
                word.MainDocumentPart.StyleDefinitionsPart.Styles.Append(item.ToStyle());
        }

        public static IStyleChain GetChain()
        {
            return new OxmlTextStyleCollection().Chain();
        }

        #endregion

        #region IEnumerable

        public IEnumerator<OxmlTextStyle> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion
    }
}
