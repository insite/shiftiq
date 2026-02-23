using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class OrderingSolution
    {
        [JsonProperty]
        public Guid Identifier { get; private set; }

        [JsonProperty]
        public decimal Points { get; set; }

        [JsonProperty]
        public decimal? CutScore { get; set; }

        public IReadOnlyList<Guid> Options => _optionsReadOnly;

        [JsonProperty(PropertyName = "Options")]
        private List<Guid> _options;

        [NonSerialized]
        private Ordering _container;

        [NonSerialized]
        private IReadOnlyList<Guid> _optionsReadOnly;

        [JsonConstructor]
        private OrderingSolution()
        {
            _options = new List<Guid>();
            _optionsReadOnly = _options.AsReadOnly();
        }

        public OrderingSolution(Guid id)
            : this()
        {
            Identifier = id;
        }

        internal void SetContainer(Ordering container)
        {
            if (_container != null)
                throw ApplicationError.Create(typeof(OrderingSolution).FullName + " is already assigned to this option");

            _container = container;
            _container.OptionAdded += Container_OptionAdded;
            _container.OptionRemoved += Container_OptionRemoved;

            var existOptions = _options
                .Select((x, i) => (Id: x, Index: i))
                .ToDictionary(x => x.Id, x => x.Index);
            var orderedOptions = _container.Options
                .Select((x, i) => (Id: x.Identifier, Index: i))
                .OrderBy(x => existOptions.GetOrDefault(x.Id, int.MaxValue))
                .ThenBy(x => x.Index)
                .Select(x => x.Id);

            _options.Clear();
            _options.AddRange(orderedOptions);
        }

        private void Container_OptionAdded(object sender, GuidValueArgs e)
        {
            _options.Add(e.Value);
        }

        private void Container_OptionRemoved(object sender, GuidValueArgs e)
        {
            _options.Remove(e.Value);
        }

        public void ReorderOptions(IDictionary<Guid, int> order)
        {
            var orderedOptions = _options
                .Select((x, i) => (Id: x, Index: i))
                .OrderBy(x => order.GetOrDefault(x.Id, x.Index))
                .Select(x => x.Id)
                .ToArray();

            _options.Clear();
            _options.AddRange(orderedOptions);
        }

        public void CopyTo(OrderingSolution other)
        {
            other.Points = this.Points;
            other.CutScore = this.CutScore;
        }

        public bool IsEqual(OrderingSolution other, bool compareIdentifiers = true)
        {
            return (!compareIdentifiers || this.Identifier == other.Identifier)
                && this.Points == other.Points
                && this.CutScore == other.CutScore
                && this._options.Count == other._options.Count
                && this._options.Zip(other._options, (a, b) => a == b).All(x => x);
        }
    }
}
