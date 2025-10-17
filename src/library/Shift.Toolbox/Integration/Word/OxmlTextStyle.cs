using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using DocumentFormat.OpenXml.Wordprocessing;

using Shift.Common;

namespace Shift.Toolbox
{
    public class OxmlTextStyle
    {
        public string Id { get; }

        public string Name { get; }

        public bool Bold
        {
            get => GetValue(false);
            set => SetValue(value);
        }

        public string FontColor
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string BackColor
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string FontName
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string FontSize
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        public string BasedOn
        {
            get => GetValue(string.Empty);
            set => SetValue(value);
        }

        private Dictionary<string, object> _values = new Dictionary<string, object>();

        public OxmlTextStyle(string id)
            : this(id, id)
        {

        }

        public OxmlTextStyle(string id, string name)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentNullException(nameof(id));

            Id = id;
            Name = name;
        }

        private T GetValue<T>(T defaultValue, [CallerMemberName] string key = null)
        {
            return _values.ContainsKey(key) ? (T)_values[key] : defaultValue;
        }

        private void SetValue<T>(T value, [CallerMemberName] string key = null)
        {
            if (_values.ContainsKey(key))
                _values[key] = value;
            else
                _values.Add(key, value);
        }

        internal void SetIfEmpty(string key, object value)
        {
            if (!_values.ContainsKey(key))
                _values.Add(key, value);
        }

        public Style ToStyle()
        {
            if (string.IsNullOrEmpty(Id))
                throw ApplicationError.Create("Style identifier is not defined");

            var style = new Style { Type = StyleValues.Character, StyleId = Id, CustomStyle = true };

            style.Append(new StyleName { Val = Name });

            if (!string.IsNullOrEmpty(BasedOn))
                style.Append(new BasedOn { Val = BasedOn });

            var styleRunPropertries = new StyleRunProperties();

            if (!string.IsNullOrEmpty(FontSize))
                styleRunPropertries.Append(new FontSize { Val = FontSize });

            if (!string.IsNullOrEmpty(FontColor))
                styleRunPropertries.Append(new DocumentFormat.OpenXml.Wordprocessing.Color { Val = FontColor });

            if (!string.IsNullOrEmpty(FontName))
                styleRunPropertries.Append(new RunFonts { Ascii = FontName });

            if (!string.IsNullOrEmpty(BackColor))
                styleRunPropertries.Append(new Shading { Val = ShadingPatternValues.Clear, Fill = BackColor });

            if (Bold)
                styleRunPropertries.Append(new Bold());

            style.Append(styleRunPropertries);

            return style;
        }
    }
}
