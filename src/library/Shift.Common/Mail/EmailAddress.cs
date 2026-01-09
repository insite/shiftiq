using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Serialization;
using System.Text;

using Newtonsoft.Json;

using Shift.Common;

namespace Shift.Common
{
    [Serializable, JsonObject(MemberSerialization.OptIn)]
    public class EmailAddress
    {
        #region Fields

        [JsonProperty(PropertyName = "Address")]
        private string _address;

        [JsonProperty(PropertyName = "DisplayName", NullValueHandling = NullValueHandling.Ignore)]
        private string _displayName;

        [NonSerialized]
        private MailAddress _mailAddress;

        #endregion

        #region Properties

        public string Address
        {
            get => _address;
            private set
            {
                _address = StringHelper.Trim(value).NullIfEmpty();
                if (_address == null)
                    return;

                _mailAddress = GetMailAddress(_address);
                if (_mailAddress == null)
                    return;

                _address = _mailAddress.Address;

                if (DisplayName.IsEmpty() && _mailAddress.DisplayName.IsNotEmpty())
                    DisplayName = _mailAddress.DisplayName;
            }
        }

        public string Mailbox => _mailAddress?.User.ToLower();

        public string Domain => _mailAddress?.Host.ToLower();

        public string DisplayName
        {
            get => _displayName;
            set => _displayName = StringHelper.TrimAndClean(value).NullIfEmpty();
        }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Variables { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Guid> Cc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Guid> Bcc { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Identifier { get; private set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Code { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Language { get; set; }

        #endregion

        #region Constructors

        private EmailAddress()
        {
            Variables = new Dictionary<string, string>();
            Cc = new List<Guid>();
            Bcc = new List<Guid>();
        }

        public EmailAddress(string address)
            : this(address, null)
        {

        }

        public EmailAddress(string address, string name)
            : this()
        {
            Address = address;
            DisplayName = name;
        }

        public EmailAddress(EmailAddress address)
            : this(address._address, address._displayName)
        {

        }

        public EmailAddress(Guid id, string address, string name, string code, string language)
            : this(address, name)
        {
            if (id != Guid.Empty)
                Identifier = id;

            Code = code;
            Language = language;
        }

        public EmailAddress Clone()
        {
            var clone = new EmailAddress();

            this.ShallowCopyTo(clone);
            clone.Cc.AddRange(Cc);
            clone.Bcc.AddRange(Bcc);
            clone.Variables = Variables.ToDictionary(x => x.Key, x => x.Value);

            return clone;
        }

        #endregion

        #region Methods (equality)

        public override bool Equals(object compareTo)
        {
            return compareTo is EmailAddress p && p._address.Equals(_address, StringComparison.CurrentCultureIgnoreCase);
        }

        public bool Equals(EmailAddress compareTo)
        {
            return compareTo != null && compareTo._address.Equals(_address, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return StringComparer.CurrentCultureIgnoreCase.GetHashCode(_address);
        }

        #endregion

        #region Methods (validation)

        public bool IsValid => _mailAddress != null;

        public static bool IsValidAddress(string address, bool enabled = true)
        {
            return enabled && GetMailAddress(address) != null;
        }

        public static string GetEnabledEmail(string email, bool enabled)
        {
            return GetEnabledEmail(email, enabled, null, false);
        }

        public static string GetEnabledEmail(string email, bool enabled, string altEmail, bool altEnabled)
        {
            if (enabled && email.IsNotEmpty() && IsValidAddress(email))
                return email;

            if (altEnabled && altEmail.IsNotEmpty() && IsValidAddress(altEmail))
                return altEmail;

            return null;
        }

        #endregion

        #region Methods (conversion)

        public override string ToString()
        {
            var sb = new StringBuilder();

            if (_displayName.IsNotEmpty())
                sb.Append($"{_displayName} <{_address}>");
            else
                sb.AppendFormat(_address);

            return sb.ToString();
        }

        public string ToHtml()
        {
            var sb = new StringBuilder();

            if (_displayName.IsNotEmpty())
                sb.Append($"{_displayName} <span class='form-text'>&lt;{_address}&gt;</span>");
            else
                sb.AppendFormat(_address);

            return sb.ToString();
        }

        #endregion

        #region Methods (serialization)

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            _mailAddress = GetMailAddress(_address);

            if (Variables == null)
                Variables = new Dictionary<string, string>();

            if (Cc == null)
                Cc = new List<Guid>();

            if (Bcc == null)
                Bcc = new List<Guid>();
        }

        public bool ShouldSerializeVariables()
        {
            return Variables.IsNotEmpty();
        }

        public bool ShouldSerializeCc()
        {
            return Cc.IsNotEmpty();
        }

        public bool ShouldSerializeBcc()
        {
            return Bcc.IsNotEmpty();
        }

        #endregion

        #region Methods (helpers)

        private static MailAddress GetMailAddress(string address)
        {
            try
            {
                return new MailAddress(address);
            }
            catch (ArgumentNullException)
            {

            }
            catch (ArgumentException)
            {

            }
            catch (FormatException)
            {

            }

            return null;
        }

        public static List<string> Filter(string emails, string whitelistDomains, string whitelistTesters)
        {
            return Filter(StringHelper.Split(emails), StringHelper.Split(whitelistDomains), StringHelper.Split(whitelistTesters));
        }

        public static List<string> Filter(IEnumerable<string> emails, string[] whitelistDomains, string[] whitelistTesters)
        {
            return Filter(new EmailAddressList(emails), whitelistDomains, whitelistTesters);
        }

        public static List<string> Filter(EmailAddressList emails, string[] whitelistDomains, string[] whitelistTesters)
        {
            var list = emails.Select(x => x.Address).ToList();

            if (whitelistDomains.IsNotEmpty() && whitelistTesters.IsNotEmpty())
            {
                list = emails.Where(x => StringHelper.EqualsAny(x.Domain, whitelistDomains)
                        || StringHelper.EqualsAny(x.Address, whitelistTesters))
                    .Select(x => x.Address)
                    .ToList();
            }
            else if (whitelistDomains.IsNotEmpty())
            {
                list = emails.Where(x => StringHelper.EqualsAny(x.Domain, whitelistDomains))
                    .Select(x => x.Address)
                    .ToList();
            }
            else if (whitelistTesters.IsNotEmpty())
            {
                list = emails.Where(x => StringHelper.EqualsAny(x.Address, whitelistTesters))
                    .Select(x => x.Address)
                    .ToList();
            }

            return list;
        }

        public static Dictionary<Guid, string> Filter(Dictionary<Guid, string> emails, string[] whitelistDomains, string[] whitelistTesters)
        {
            if (whitelistDomains.IsNotEmpty() && whitelistTesters.IsNotEmpty())
            {
                return emails
                    .Where(x => StringHelper.EndsWithAny(x.Value, whitelistDomains)
                             || StringHelper.EqualsAny(x.Value, whitelistTesters))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            else if (whitelistDomains.IsNotEmpty())
            {
                return emails
                    .Where(x => StringHelper.EndsWithAny(x.Value, whitelistDomains))
                    .ToDictionary(x => x.Key, x => x.Value);
            }
            else if (whitelistTesters.IsNotEmpty())
            {
                return emails
                    .Where(x => StringHelper.EqualsAny(x.Value, whitelistTesters))
                    .ToDictionary(x => x.Key, x => x.Value);
            }

            return new Dictionary<Guid, string>();
        }

        #endregion
    }
}