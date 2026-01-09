using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

using InSite.Common.Web;
using InSite.Persistence;

using Shift.Common;

namespace InSite
{
    public class ReturnUrl
    {
        #region Classes (value container)

        private interface IValue
        {
            int Number { get; set; }
            string Name { get; }
        }

        private class ValueContainer<T> where T : IValue
        {
            #region Fields

            private readonly object _syncRoot = new object();

            private readonly List<T> _items = new List<T>();
            private readonly Dictionary<string, T> _itemsByName = new Dictionary<string, T>(StringComparer.OrdinalIgnoreCase);
            private readonly Dictionary<int, T> _itemsByNumber = new Dictionary<int, T>();

            private int _positiveIndex = 0;
            private int _negativeIndex = -1;

            #endregion

            public bool Contains(int number) => _itemsByNumber.ContainsKey(number);

            public bool Contains(string name) => _itemsByName.ContainsKey(name);

            public T Get(int number) => _itemsByNumber[number];

            public T Get(string name) => _itemsByName[name];

            public bool TryGet(int number, out T value) => _itemsByNumber.TryGetValue(number, out value);

            public bool TryGet(string name, out T value) => _itemsByName.TryGetValue(name, out value);

            public T GetOrAdd(int number, Func<int, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByNumber.TryGetValue(number, out var value))
                        Add(value = getValue(number));

                    return value;
                }
            }

            public T GetOrAdd(string name, Func<string, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByName.TryGetValue(name, out var value))
                        Add(value = getValue(name));

                    return value;
                }
            }

            public T GetOrAddDefined(int number, Func<int, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByNumber.TryGetValue(number, out var value))
                        AddDefined(value = getValue(number));

                    return value;
                }
            }

            public T GetOrAddDefined(string name, Func<string, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByName.TryGetValue(name, out var value))
                        AddDefined(value = getValue(name));

                    return value;
                }
            }

            public T GetOrAddNew(int number, Func<int, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByNumber.TryGetValue(number, out var value))
                        AddNew(value = getValue(number));

                    return value;
                }
            }

            public T GetOrAddNew(string name, Func<string, T> getValue)
            {
                lock (_syncRoot)
                {
                    if (!_itemsByName.TryGetValue(name, out var value))
                        AddNew(value = getValue(name));

                    return value;
                }
            }

            public void Add(T value)
            {
                if (value == null)
                    return;

                lock (_syncRoot)
                {
                    if (_itemsByName.ContainsKey(value.Name) || _itemsByNumber.ContainsKey(value.Number))
                        return;

                    _items.Add(value);
                    _itemsByName.Add(value.Name, value);
                    _itemsByNumber.Add(value.Number, value);

                    if (_positiveIndex <= value.Number)
                        _positiveIndex = value.Number + 1;

                    if (_negativeIndex >= value.Number)
                        _negativeIndex = value.Number - 1;
                }
            }

            public void AddDefined(T value)
            {
                if (value == null)
                    return;

                lock (_syncRoot)
                {
                    value.Number = _positiveIndex++;

                    Add(value);
                }
            }

            public void AddNew(T value)
            {
                if (value == null)
                    return;

                lock (_syncRoot)
                {
                    value.Number = _negativeIndex--;

                    Add(value);
                }
            }
        }

        private class DefaultValue : IValue
        {
            public int Number { get; set; }
            public string Name { get; private set; }

            public DefaultValue(string name)
            {
                Name = name;
            }

            public DefaultValue(int num, string name)
                : this(name)
            {
                Number = num;
            }
        }

        #endregion

        #region Classes (parameter info)

        private abstract class BaseParameter : IValue
        {
            public int Number { get; set; }
            public string Name { get; private set; }

            public BaseParameter(string name)
            {
                Name = name;
            }

            public abstract string Read(BinaryReader reader);
            public abstract void Write(BinaryWriter writer, string value);

            public override string ToString()
            {
                return Name;
            }
        }

        private class IntParameter : BaseParameter
        {
            public IntParameter(string name)
                : base(name)
            {

            }

            public override string Read(BinaryReader reader)
            {
                var num = reader.ReadInt32();

                return num.ToString();
            }

            public override void Write(BinaryWriter writer, string value)
            {
                var num = int.Parse(value);
                writer.Write(num);
            }
        }

        private class GuidParameter : BaseParameter
        {
            public GuidParameter(string name)
                : base(name)
            {

            }

            public override string Read(BinaryReader reader)
            {
                var value = reader.ReadGuid();

                return value.ToString();
            }

            public override void Write(BinaryWriter writer, string value)
            {
                var parsed = Guid.Parse(value);
                writer.Write(parsed);
            }
        }

        private class EnumParameter : BaseParameter
        {
            public ValueContainer<DefaultValue> Values { get; } = new ValueContainer<DefaultValue>();

            public EnumParameter(string name)
                : base(name)
            {

            }

            public EnumParameter(string name, string[] values)
                : this(name)
            {
                foreach (var val in values)
                    Values.AddDefined(new DefaultValue(val));
            }

            public override string Read(BinaryReader reader)
            {
                var number = (int)reader.ReadSByte();

                return Values.TryGet(number, out var value) ? value.Name : null;
            }

            public override void Write(BinaryWriter writer, string value)
            {
                var number = Values.GetOrAddNew(value, v => new DefaultValue(v)).Number;

                writer.Write((sbyte)number);
            }
        }

        private class StringParameter : BaseParameter
        {
            public StringParameter(string name)
                : base(name)
            {

            }

            public override string Read(BinaryReader reader)
            {
                return reader.ReadString();
            }

            public override void Write(BinaryWriter writer, string value)
            {
                writer.Write(value ?? string.Empty);
            }
        }

        private class Base64Parameter : BaseParameter
        {
            public Base64Parameter(string name)
                : base(name)
            {

            }

            public override string Read(BinaryReader reader)
            {
                var length = reader.ReadUInt16();
                var value = reader.ReadBytes(length);

                return HttpServerUtility.UrlTokenEncode(value);
            }

            public override void Write(BinaryWriter writer, string value)
            {
                var data = HttpServerUtility.UrlTokenDecode(value);

                writer.Write((ushort)data.Length);
                writer.Write(data);
            }
        }

        #endregion

        #region  Classes (other)

        private class ReturnState
        {
            private int _actionNumber = int.MinValue;
            private string _hash = string.Empty;
            private readonly List<BaseParameter> _requiredParams = new List<BaseParameter>();
            private readonly List<Tuple<BaseParameter, string>> _definedParams = new List<Tuple<BaseParameter, string>>();

            private ReturnState()
            {

            }

            public static ReturnState FromUrl(WebUrl returnUrl, WebUrl redirectUrl, Dictionary<string, string> queryData)
            {
                var result = new ReturnState();

                result._actionNumber = GetActionNumber(returnUrl.Path);
                result._hash = returnUrl.Hash;
                result._requiredParams.Clear();
                result._definedParams.Clear();

                foreach (string key in returnUrl.QueryString.Keys)
                {
                    var retValue = returnUrl.QueryString[key];
                    if (queryData != null && queryData.ContainsKey(key))
                    {
                        retValue = queryData[key];
                        queryData.Remove(key);
                    }

                    AddParameter(key, retValue);
                }

                if (queryData.IsNotEmpty())
                {
                    foreach (string key in queryData.Keys)
                        AddParameter(key, queryData[key]);
                }

                return result;

                void AddParameter(string key, string retValue)
                {
                    var redValue = redirectUrl.QueryString[key];
                    var parameter = _parameters.GetOrAddNew(key, name => new StringParameter(name));

                    if (!string.IsNullOrEmpty(retValue) && (string.IsNullOrEmpty(redValue) || !string.Equals(retValue, redValue, StringComparison.OrdinalIgnoreCase)))
                        result._definedParams.Add(new Tuple<BaseParameter, string>(parameter, retValue));
                    else
                        result._requiredParams.Add(parameter);
                }
            }

            public static ReturnState FromToken(string data)
            {
                ReturnState state = null;

                if (!string.IsNullOrEmpty(data))
                {
                    try
                    {
                        state = StringHelper.DecodeBase64Url(data, stream =>
                        {
                            using (var reader = new BinaryReader(stream))
                            {
                                var result = new ReturnState();

                                result._actionNumber = reader.ReadInt32();

                                for (int i = reader.ReadByte(); i > 0; i--)
                                {
                                    int num = reader.ReadSByte();
                                    if (_parameters.Contains(num))
                                    {
                                        var param = _parameters.Get(num);
                                        result._requiredParams.Add(param);
                                    }
                                }

                                for (int i = reader.ReadByte(); i > 0; i--)
                                {
                                    int num = reader.ReadSByte();
                                    if (_parameters.Contains(num))
                                    {
                                        var param = _parameters.Get(num);
                                        result._definedParams.Add(new Tuple<BaseParameter, string>(param, param.Read(reader)));
                                    }
                                }

                                result._hash = reader.ReadString();

                                return result;
                            }
                        });
                    }
                    catch
                    {
                        state = null;
                    }
                }

                return state;
            }

            public WebUrl ToUrl(WebUrl currentUrl, Dictionary<string, string> requiredParams, Dictionary<string, string> overrideParams, Dictionary<string, string> appendParams)
            {
                var actionName = GetActionName(_actionNumber);
                if (actionName.IsEmpty())
                    return null;

                var returnUrl = new WebUrl("/" + actionName);

                foreach (var p in _requiredParams)
                {
                    var value = overrideParams != null && overrideParams.ContainsKey(p.Name)
                        ? overrideParams[p.Name]
                        : requiredParams != null && requiredParams.ContainsKey(p.Name)
                            ? requiredParams[p.Name]
                            : currentUrl.QueryString[p.Name];

                    if (appendParams != null)
                        appendParams.Remove(p.Name);

                    if (value == null)
                        return null;

                    if (value.IsEmpty())
                        returnUrl.QueryString.Remove(p.Name);
                    else
                        returnUrl.QueryString[p.Name] = value;
                }

                foreach (var p in _definedParams)
                {
                    var pKey = p.Item1.Name;

                    returnUrl.QueryString[pKey] = overrideParams != null && overrideParams.ContainsKey(pKey)
                        ? overrideParams[pKey]
                        : p.Item2;

                    if (appendParams != null)
                        appendParams.Remove(pKey);
                }

                if (appendParams != null)
                    foreach (var p in appendParams)
                        returnUrl.QueryString[p.Key] = p.Value;

                returnUrl.Hash = _hash;

                return returnUrl;
            }

            public string ToToken()
            {
                return StringHelper.EncodeBase64Url(stream =>
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        writer.Write(_actionNumber);

                        writer.Write((byte)_requiredParams.Count);
                        foreach (var p in _requiredParams)
                            writer.Write((sbyte)p.Number);

                        writer.Write((byte)_definedParams.Count);
                        foreach (var p in _definedParams)
                        {
                            writer.Write((sbyte)p.Item1.Number);
                            p.Item1.Write(writer, p.Item2);
                        }

                        writer.Write(_hash.EmptyIfNull());
                    }
                });
            }

            public ReturnState Copy()
            {
                var result = new ReturnState { _actionNumber = _actionNumber, _hash = _hash };
                result._requiredParams.AddRange(_requiredParams);
                result._definedParams.AddRange(_definedParams);
                return result;
            }
        }

        #endregion

        #region Construction (static)

        private static readonly ValueContainer<DefaultValue> _actions = new ValueContainer<DefaultValue>();
        private static readonly ValueContainer<BaseParameter> _parameters = new ValueContainer<BaseParameter>();

        static ReturnUrl()
        {
            _parameters.AddDefined(new GuidParameter("bank"));
            _parameters.AddDefined(new GuidParameter("set"));
            _parameters.AddDefined(new GuidParameter("question"));
            _parameters.AddDefined(new GuidParameter("spec"));
            _parameters.AddDefined(new GuidParameter("criterion"));
            _parameters.AddDefined(new GuidParameter("form"));
            _parameters.AddDefined(new GuidParameter("section"));
            _parameters.AddDefined(new GuidParameter("attachment"));
            _parameters.AddDefined(new GuidParameter("comment"));
            _parameters.AddDefined(new EnumParameter("panel", new[] { "attachments", "comments", "questions", "content", "candidates", "permission", "referral" }));
            _parameters.AddDefined(new EnumParameter("tab", new[] { "fields", "title", "summary", "materials", "section", "body" }));
            _parameters.AddDefined(new GuidParameter("course"));
            _parameters.AddDefined(new GuidParameter("task"));
            _parameters.AddDefined(new GuidParameter("resource"));
            _parameters.AddDefined(new IntParameter("grid-page"));
            _parameters.AddDefined(new GuidParameter("asset"));
            _parameters.AddDefined(new GuidParameter("contact"));
            _parameters.AddDefined(new GuidParameter("user"));
            _parameters.AddDefined(new GuidParameter("group"));
            _parameters.AddDefined(new Base64Parameter("filter"));
            _parameters.AddDefined(new Base64Parameter("return"));
            _parameters.AddDefined(new GuidParameter("journal"));
            _parameters.AddDefined(new GuidParameter("journalsetup"));
            _parameters.AddDefined(new GuidParameter("experience"));
            _parameters.AddDefined(new GuidParameter("event"));
            _parameters.AddDefined(new GuidParameter("id"));
        }

        #endregion

        #region Construction

        private WebUrl _currentUrl;
        private WebUrl _returnUrl;
        private ReturnState _returnState;

        public ReturnUrl()
        {
            _currentUrl = HttpRequestHelper.GetCurrentWebUrl();
            _returnUrl = _currentUrl.Copy();
            _returnState = ReturnState.FromToken(_currentUrl.QueryString["return"]);
        }

        private ReturnUrl(ReturnUrl url)
        {
            _currentUrl = url._currentUrl.Copy();
            _returnUrl = url._returnUrl.Copy();
            _returnState = url._returnState?.Copy();
        }

        public ReturnUrl(string data)
            : this()
        {
            Dictionary<string, string> query;

            if (data.IsNotEmpty() && data.StartsWith("/"))
            {
                var url = new WebUrl(data);
                _returnUrl.Path = url.Path;
                _returnUrl.Hash = url.Hash;
                query = url.QueryString.ToDictionary();
            }
            else
            {
                query = ParseDataQuery(data);
            }

            Modify(query);
        }

        #endregion

        #region Methods

        public ReturnUrl Copy() => new ReturnUrl(this);

        public void Modify(string data) => Modify(ParseDataQuery(data));

        private void Modify(Dictionary<string, string> query)
        {
            if (query == null)
                return;

            foreach (var key in _returnUrl.QueryString.AllKeys)
            {
                if (key == null)
                {
                    _returnUrl.QueryString.Remove(key);
                }
                else if (query.ContainsKey(key))
                {
                    var value = query[key];
                    if (value == string.Empty)
                        _returnUrl.QueryString.Remove(key);
                    else if (value != null)
                        _returnUrl.QueryString[key] = value;

                    query.Remove(key);
                }
                else
                {
                    _returnUrl.QueryString.Remove(key);
                }
            }

            Append(query);
        }

        public void Append(string data) => Append(ParseDataQuery(data));

        private void Append(Dictionary<string, string> query)
        {
            if (query == null)
                return;

            foreach (var qKey in query.Keys)
                _returnUrl.QueryString[qKey] = query[qKey];
        }

        public void Remove(params string[] removeParams)
        {
            foreach (var removeParam in removeParams)
                _returnUrl.QueryString.Remove(removeParam);
        }

        public string GetRedirectUrl(string redirectUrl, string returnQuery = null)
        {
            var result = new WebUrl(redirectUrl);
            var queryData = ParseDataQuery(returnQuery);
            var state = ReturnState.FromUrl(_returnUrl, result, queryData);

            result.QueryString["return"] = state.ToToken();

            return result.ToString();
        }

        public string GetReturnUrl(string required = null, string @override = null, string append = null)
        {
            var requiredData = ParseDataQuery(required);
            var overrideData = ParseDataQuery(@override);
            var appendData = ParseDataQuery(append);

            return _returnState?.ToUrl(_currentUrl, requiredData, overrideData, appendData)?.ToString();
        }

        #endregion

        #region Helper methods

        private static int GetActionNumber(string path)
        {
            return _actions.GetOrAdd(path[0] == '/' ? path.Substring(1) : path, name =>
            {
                var action = TActionSearch.Get(name);
                if (action != null)
                    return new DefaultValue(action.ActionIdentifier.GetHashCode(), action.ActionUrl);

                int number;

                do
                {
                    number = RandomNumberGenerator.Instance.Next();
                } while (!TActionSearch.Exists(number) && !_actions.Contains(number));

                return new DefaultValue(number, name);
            }).Number;
        }

        private static string GetActionName(int number)
        {
            return _actions.GetOrAdd(number, n =>
            {
                var action = TActionSearch.Get(n);
                if (action != null)
                    return new DefaultValue(action.ActionIdentifier.GetHashCode(), action.ActionUrl);

                return null;
            })?.Name;
        }

        private static Dictionary<string, string> ParseDataQuery(string input)
        {
            var result = new Dictionary<string, string>();
            if (input.IsEmpty())
                return result;

            int iName, iValue;
            for (int i = 0; i < input.Length; i++)
            {
                iName = i;
                iValue = -1;

                for (; i < input.Length; i++)
                {
                    var ch = input[i];
                    if (ch == '=')
                    {
                        if (iValue >= 0)
                            throw ApplicationError.Create("Invalid input string: " + input);

                        iValue = i;
                    }
                    else if (ch == '&')
                        break;
                }

                string key, value;

                if (iValue == -1)
                {
                    key = input.Substring(iName, i - iName);
                    value = null;
                }
                else
                {
                    key = input.Substring(iName, iValue - iName);
                    value = input.Substring(iValue + 1, i - iValue - 1);
                }

                if (key.Length != 0)
                    result.Add(key, value);
            }

            return result;
        }

        #endregion
    }
}