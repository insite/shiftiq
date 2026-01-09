using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using Shift.Common;
using Shift.Common.Events;

namespace InSite.Domain.Banks
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class Ordering
    {
        #region Events

        public event GuidValueHandler OptionAdded;

        private void OnOptionAdded(Guid id) => OptionAdded?.Invoke(this, new GuidValueArgs(id));

        public event GuidValueHandler OptionRemoved;

        private void OnOptionRemoved(Guid id) => OptionRemoved?.Invoke(this, new GuidValueArgs(id));

        #endregion

        #region Properties

        [JsonProperty]
        public OrderingLabel Label { get; }

        public IReadOnlyList<OrderingOption> Options => _optionsReadOnly;

        public IReadOnlyList<OrderingSolution> Solutions => _solutionsReadOnly;

        [JsonProperty(PropertyName = "Options")]
        private List<OrderingOption> _options;

        [JsonProperty(PropertyName = "Solutions")]
        private List<OrderingSolution> _solutions;

        public bool IsEmpty => Label.IsEmpty
            && _options.Count == 0
            && _solutions.Count == 0;

        #endregion

        #region Fields

        [NonSerialized]
        private IReadOnlyList<OrderingOption> _optionsReadOnly;

        [NonSerialized]
        private IReadOnlyList<OrderingSolution> _solutionsReadOnly;

        #endregion

        #region Construction

        public Ordering()
        {
            Label = new OrderingLabel();

            _options = new List<OrderingOption>();
            _optionsReadOnly = _options.AsReadOnly();

            _solutions = new List<OrderingSolution>();
            _solutionsReadOnly = _solutions.AsReadOnly();
        }

        #endregion

        #region Methods (options)

        public OrderingOption AddOption(Guid id)
        {
            var option = new OrderingOption(id);

            AddOption(option);

            return option;
        }

        public void AddOption(OrderingOption option)
        {
            if (option.Identifier == Guid.Empty)
                throw ApplicationError.Create("Option identifier can't be empty");

            if (_options.Any(x => x.Identifier == option.Identifier))
                throw ApplicationError.Create("Identifier duplicate: " + option.Identifier);

            _options.Add(option);

            OnOptionAdded(option.Identifier);
        }

        public OrderingOption GetOption(int number)
        {
            return _options.FirstOrDefault(x => x.Number == number);
        }

        public OrderingOption GetOption(Guid id)
        {
            return _options.FirstOrDefault(x => x.Identifier == id);
        }

        public int GetOptionIndex(Guid id)
        {
            return _options.FindIndex(x => x.Identifier == id);
        }

        public int GetOptionIndex(OrderingOption option)
        {
            return _options.IndexOf(option);
        }

        public void ReorderOptions(IDictionary<Guid, int> order)
        {
            var orderedOptions = _options
                .Select((x, i) => (Option: x, Index: i))
                .OrderBy(x => order.GetOrDefault(x.Option.Identifier, x.Index))
                .Select(x => x.Option)
                .ToArray();

            _options.Clear();
            _options.AddRange(orderedOptions);
        }

        public bool RemoveOption(Guid id)
        {
            var index = GetOptionIndex(id);
            if (index == -1)
                return false;

            _options.RemoveAt(index);

            OnOptionRemoved(id);

            return true;
        }

        #endregion

        #region Methods (solutions)

        public OrderingSolution AddSolution(Guid id)
        {
            var solution = new OrderingSolution(id);

            AddSolution(solution);

            return solution;
        }

        public void AddSolution(OrderingSolution solution)
        {
            if (solution.Identifier == Guid.Empty)
                throw ApplicationError.Create("Solution identifier can't be empty");

            if (_solutions.Any(x => x.Identifier == solution.Identifier))
                throw ApplicationError.Create("Identifier duplicate: " + solution.Identifier);

            solution.SetContainer(this);

            _solutions.Add(solution);
        }

        public OrderingSolution GetSolution(Guid id)
        {
            return _solutions.FirstOrDefault(x => x.Identifier == id);
        }

        public int GetSolutionIndex(Guid id)
        {
            return _solutions.FindIndex(x => x.Identifier == id);
        }

        public void ReorderSolutions(IDictionary<Guid, int> order)
        {
            var orderedSolutions = _solutions
                .Select((x, i) => (Solution: x, Index: i))
                .OrderBy(x => order.GetOrDefault(x.Solution.Identifier, x.Index))
                .Select(x => x.Solution)
                .ToArray();

            _solutions.Clear();
            _solutions.AddRange(orderedSolutions);
        }

        public bool RemoveSolution(Guid id)
        {
            var index = GetSolutionIndex(id);
            if (index == -1)
                return false;

            _solutions.RemoveAt(index);

            return true;
        }

        #endregion

        #region Methods (other)

        public Ordering Clone()
        {
            var cloneOrdering = new Ordering();

            this.Label.CopyTo(cloneOrdering.Label);

            cloneOrdering._options.AddRange(this._options.Select(x => x.Clone()));

            cloneOrdering._solutions.AddRange(this._solutions.Select(solution =>
            {
                var cloneSolution = new OrderingSolution(solution.Identifier);

                solution.CopyTo(cloneSolution);

                cloneSolution.SetContainer(cloneOrdering);
                cloneSolution.ReorderOptions(
                    solution.Options
                        .Select((x, i) => (Id: x, Index: i))
                        .ToDictionary(x => x.Id, x => x.Index));

                return cloneSolution;
            }));

            return cloneOrdering;
        }

        public bool IsEqual(Ordering other, bool compareIdentifiers = true)
        {
            return this.Label.IsEqual(other.Label)
                && this._options.Count == other._options.Count
                && this._solutions.Count == other._solutions.Count
                && this._options.Zip(other._options, (a, b) => a.IsEqual(b, compareIdentifiers)).All(x => x)
                && this._solutions.Zip(other._solutions, (a, b) => a.IsEqual(b, compareIdentifiers)).All(x => x);
        }

        private void RestoreReferences()
        {
            foreach (var solution in _solutions)
                solution.SetContainer(this);
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            _optionsReadOnly = _options.AsReadOnly();

            RestoreReferences();
        }

        public bool ShouldSerializeLabel() => !Label.IsEmpty;

        public bool ShouldSerialize_options() => _options.Count > 0;

        public bool ShouldSerialize_solutions() => _solutions.Count > 0;

        #endregion
    }
}
