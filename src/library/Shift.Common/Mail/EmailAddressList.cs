using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Common
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class EmailAddressList : IEnumerable<EmailAddress>
    {
        #region Fields

        [JsonProperty(PropertyName = "Items")]
        private readonly List<EmailAddress> _items;

        [NonSerialized]
        private Dictionary<string, EmailAddress> _indexByAddress;

        [NonSerialized]
        private Dictionary<Guid, EmailAddress> _indexById;

        #endregion

        #region Constructors

        public EmailAddressList()
        {
            _items = new List<EmailAddress>();
            _indexByAddress = new Dictionary<string, EmailAddress>(StringComparer.CurrentCultureIgnoreCase);
            _indexById = new Dictionary<Guid, EmailAddress>();
        }

        public EmailAddressList(IEnumerable<EmailAddress> list)
            : this()
        {
            foreach (var item in list)
                Add(item);
        }

        public EmailAddressList(IEnumerable<string> addresses)
            : this()
        {
            foreach (var address in addresses)
                Add(address, null);
        }

        public EmailAddressList(string csv)
            : this()
        {
            if (csv.IsEmpty())
                return;

            var values = StringHelper.Split(csv).Select(x => x.ToLower()).Distinct().OrderBy(x => x).ToArray();
            foreach (var value in values)
            {
                if (!EmailAddress.IsValidAddress(value))
                    throw new InvalidEmailAddressException(value);

                Add(new EmailAddress(value));
            }
        }

        #endregion

        #region Indexers

        public EmailAddress this[int index] =>
            _items[index];

        public EmailAddress this[string address] =>
            address.IsNotEmpty() && _indexByAddress.TryGetValue(address, out var value) ? value : null;

        public EmailAddress this[Guid id] =>
            _indexById.TryGetValue(id, out var value) ? value : null;

        #endregion

        #region Properties

        public int Count => _items.Count;

        #endregion

        #region Methods (conversion)

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (var i = 0; i < Count; i++)
            {
                if (i > 0)
                    sb.Append(", ");

                sb.AppendFormat("{0}", this[i]);
            }

            return sb.ToString();
        }

        #endregion

        #region Methods (list management)

        public void Add(EmailAddress address)
        {
            if (address.Address.IsEmpty() || this[address.Address] != null)
                return;

            if (address.Identifier.HasValue && this[address.Identifier.Value] != null)
                return;

            _items.Add(address);
            _indexByAddress.Add(address.Address, address);

            if (address.Identifier.HasValue)
                _indexById.Add(address.Identifier.Value, address);
        }

        public void Add(IEnumerable<EmailAddress> addresses)
        {
            foreach (var address in addresses.EmptyIfNull())
                Add(address);
        }

        public void Add(string address, string displayName)
        {
            Add(new EmailAddress(address, displayName));
        }

        public void Add(Guid id, string address, string name, string personCode, string language)
        {
            if (id != Guid.Empty && address.IsNotEmpty())
                Add(new EmailAddress(id, address, name, personCode, language));
        }

        public void Add(string address)
        {
            if (address.IsEmpty())
                return;

            var list = StringHelper.Split(address, new[] { ',', ';', '|', '\r', '\n' });

            foreach (var item in list)
                Add(new EmailAddress(item));
        }

        public void Clear()
        {
            _items.Clear();
            _indexByAddress.Clear();
            _indexById.Clear();
        }

        public void Remove(EmailAddress item)
        {
            if (!_items.Remove(item))
                return;

            _indexByAddress.Remove(item.Address);

            if (item.Identifier.HasValue)
                _indexById.Remove(item.Identifier.Value);
        }

        public void Remove(string address)
        {
            if (address.IsNotEmpty() && _indexByAddress.TryGetValue(address, out var item))
                Remove(item);
        }

        public void Remove(Guid id)
        {
            if (_indexById.TryGetValue(id, out var item))
                Remove(item);
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            _indexByAddress = _items.ToDictionary(x => x.Address, x => x);
            _indexById = _items.Where(x => x.Identifier.HasValue).ToDictionary(x => x.Identifier.Value, x => x);
        }

        #endregion

        #region Methods (enumeration)

        public IEnumerator<EmailAddress> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion

        #region Methods (helpers)

        public List<string> GetVariableNames() =>
            _items.SelectMany(x => x.Variables.Keys).Distinct().ToList();

        #endregion
    }
}