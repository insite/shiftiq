using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Constant;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class Hotspot
    {
        #region Constants

        public const int MinPinLimit = 1;

        #endregion

        #region Properties

        public int PinLimit
        {
            get
            {
                return _type == QuestionItemType.HotspotImageCaptcha || _type == QuestionItemType.HotspotMultipleAnswer
                    ? Number.CheckRange(_options.Count, MinPinLimit)
                    : _pinLimit;
            }
            set
            {
                if (_type != QuestionItemType.HotspotCustom)
                    throw new InvalidOperationException("Invalid question type: " + _type.GetName());

                _pinLimit = value >= MinPinLimit ? value : throw new ArgumentOutOfRangeException(nameof(PinLimit));
            }
        }

        public bool ShowShapes
        {
            get
            {
                return _type == QuestionItemType.HotspotMultipleChoice
                    || _type == QuestionItemType.HotspotMultipleAnswer
                    || _showShapes;
            }
            set
            {
                if (_type != QuestionItemType.HotspotCustom)
                    throw new InvalidOperationException("Invalid question type: " + _type.GetName());

                _showShapes = value;
            }
        }

        [JsonProperty]
        public HotspotImage Image { get; }

        public IReadOnlyList<HotspotOption> Options => _optionsReadOnly;

        public bool IsEmpty => _pinLimit == MinPinLimit
            && _showShapes == default
            && Image.IsEmpty
            && _options.Count == 0;

        #endregion

        #region Fields

        [DefaultValue(MinPinLimit)]
        [JsonProperty(PropertyName = nameof(PinLimit), DefaultValueHandling = DefaultValueHandling.Ignore)]
        private int _pinLimit = MinPinLimit;

        [JsonProperty(PropertyName = nameof(ShowShapes), DefaultValueHandling = DefaultValueHandling.Ignore)]
        private bool _showShapes = default;

        [JsonProperty(PropertyName = "Options")]
        private List<HotspotOption> _options;

        [NonSerialized]
        private IReadOnlyList<HotspotOption> _optionsReadOnly;

        [DefaultValue(QuestionItemType.HotspotCustom)]
        [JsonProperty(PropertyName = "QuestionType", DefaultValueHandling = DefaultValueHandling.Ignore)]
        private QuestionItemType _type = QuestionItemType.HotspotCustom;

        #endregion

        #region Construction

        public Hotspot()
        {
            Image = new HotspotImage();
            Image.Resized += Image_Resized;

            _options = new List<HotspotOption>();
            _optionsReadOnly = _options.AsReadOnly();
        }

        private void Image_Resized(object sender, HotspotImage.ResizeEventArgs args)
        {
            foreach (var o in _options)
                o.Shape.Resize(args);
        }

        #endregion

        #region Methods (options)

        public void SetQuestionType(QuestionItemType type)
        {
            if (!type.IsHotspot())
                throw new ArgumentException("Invalid question type: " + type.GetName());

            _type = type;

            if (_type != QuestionItemType.HotspotCustom)
            {
                _pinLimit = MinPinLimit;
                _showShapes = default;
            }
        }

        public HotspotOption AddOption(HotspotShape shape)
        {
            return AddOption(UuidFactory.Create(), shape);
        }

        public HotspotOption AddOption(Guid id, HotspotShape shape)
        {
            var option = new HotspotOption(id, shape);

            AddOption(option);

            return option;
        }

        public void AddOption(HotspotOption option)
        {
            if (option.Identifier == Guid.Empty)
                throw ApplicationError.Create("Option identifier can't be empty");

            if (_options.Any(x => x.Identifier == option.Identifier))
                throw ApplicationError.Create("Identifier duplicate: " + option.Identifier);

            option.SetContainer(this);

            _options.Add(option);
        }

        public HotspotOption GetOption(int number)
        {
            return _options.FirstOrDefault(x => x.Number == number);
        }

        public HotspotOption GetOption(Guid id)
        {
            return _options.FirstOrDefault(x => x.Identifier == id);
        }

        public int GetOptionIndex(Guid id)
        {
            return _options.FindIndex(x => x.Identifier == id);
        }

        public void ReorderOptions(IDictionary<int, int> order)
        {
            _options = _options.OrderBy(x => order.GetOrDefault(x.Number, x.Index)).ToList();
            _optionsReadOnly = _options.AsReadOnly();
        }

        public bool RemoveOption(Guid id)
        {
            var index = GetOptionIndex(id);

            if (index == -1)
                return false;

            _options.RemoveAt(index);

            return true;
        }

        #endregion

        #region Methods (other)

        public Hotspot Clone()
        {
            var cloneHotspot = new Hotspot();

            cloneHotspot._type = _type;
            cloneHotspot._pinLimit = _pinLimit;
            cloneHotspot._showShapes = _showShapes;

            Image.CopyTo(cloneHotspot.Image);

            cloneHotspot._options.AddRange(_options.Select(x =>
            {
                var cloneOption = x.Clone();

                cloneOption.SetContainer(cloneHotspot);

                return cloneOption;
            }));

            return cloneHotspot;
        }

        public bool IsEqual(Hotspot other, bool compareIdentifiers = true)
        {
            return this._type == other._type
                && this._pinLimit == other._pinLimit
                && this._showShapes == other._showShapes
                && this._options.Count == other._options.Count
                && this.Image.IsEqual(other.Image)
                && this._options.Zip(other._options, (a, b) => a.IsEqual(b, compareIdentifiers)).All(x => x);
        }

        private void RestoreReferences()
        {
            foreach (var option in _options)
                option.SetContainer(this);
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _optionsReadOnly = _options.AsReadOnly();

            RestoreReferences();
        }

        public bool ShouldSerializeImage() => !Image.IsEmpty;

        public bool ShouldSerialize_options() => _options.Count > 0;

        #endregion
    }
}
