using System;
using System.Security.Cryptography;
using System.Text;

using Newtonsoft.Json;

namespace Shift.Toolbox.Integration.DirectAccess
{
    public class Individual
    {
        [JsonProperty("individualID")]
        public int IndividualKey { get; set; }

        [JsonProperty("firstName")]
        public string FirstName { get; set; }

        [JsonProperty("middleName")]
        public string MiddleName { get; set; }

        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("dateOfBirth")]
        public DateTime? Birthdate { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; }

        [JsonProperty("emailAddress")]
        public string Email { get; set; }

        [JsonProperty("aboriginalIndicator")]
        public string AboriginalIndicator { get; set; }

        [JsonProperty("aboriginalIdentity")]
        public string AboriginalIdentity { get; set; }

        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }

        [JsonProperty("addressLine2")]
        public string AddressLine2 { get; set; }

        [JsonProperty("addressCity")]
        public string AddressCity { get; set; }

        [JsonProperty("addressProvince")]
        public string AddressProvince { get; set; }

        [JsonProperty("addressPostalCode")]
        public string AddressPostalCode { get; set; }

        [JsonProperty("isActive")]
        public string IsActive { get; set; }

        [JsonProperty("isDeceased")]
        public string IsDeceased { get; set; }

        [JsonProperty("isMerged")]
        public string IsMerged { get; set; }

        [JsonProperty("programType")]
        public string ProgramType { get; set; }

        public Guid RefreshedBy { get; set; }

        public Guid? ContactIdentifier { get; set; }

        public Guid? CrmIdentifier { get; set; }

        public string HashCode { get; set; }

        public string Mobile { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public bool IsNew { get; set; }

        public int? PersonalEducationNumber { get; set; }

        public DateTimeOffset Refreshed { get; set; }

        #region Serialization

        public string Serialize()
        {
            PrepareForSerialization();
            return JsonConvert.SerializeObject(this);
        }

        public void PrepareForSerialization()
        {
            if (string.IsNullOrEmpty(Email))
                Email = IndividualKey.ToString() + "@itaportal.ca";

            if (!string.IsNullOrEmpty(Name) && Name.Length > 100)
                Name = Name.Substring(0, 100);

            if (!string.IsNullOrEmpty(LastName) && LastName.Length > 30)
                LastName = LastName.Substring(0, 30);

            if (!string.IsNullOrEmpty(MiddleName) && MiddleName.Length > 50)
                MiddleName = MiddleName.Substring(0, 50);

            if (!string.IsNullOrEmpty(FirstName) && FirstName.Length > 30)
                FirstName = FirstName.Substring(0, 30);

            // if (!string.IsNullOrEmpty(Phone))
            //     Phone = new Common.Numbers.Phone(Phone).ToString();

            // if (!string.IsNullOrEmpty(Mobile))
            //     Mobile = new Common.Numbers.Phone(Mobile).ToString();

            if (string.IsNullOrEmpty(Name))
                Name = $"{FirstName} {LastName}";

            if (!string.IsNullOrEmpty(AddressLine1) && AddressLine1.Length > 100)
                AddressLine1 = AddressLine1.Substring(0, 100);

            if (!string.IsNullOrEmpty(AddressLine2) && AddressLine2.Length > 100)
                AddressLine2 = AddressLine2.Substring(0, 100);

            if (!string.IsNullOrEmpty(AddressCity) && AddressCity.Length > 100)
                AddressCity = AddressCity.Substring(0, 100);

            if (!string.IsNullOrEmpty(AddressProvince) && AddressProvince.Length > 100)
                AddressProvince = AddressProvince.Substring(0, 100);

            if (!string.IsNullOrEmpty(AddressPostalCode) && AddressPostalCode.Length > 100)
                AddressPostalCode = AddressPostalCode.Substring(0, 100);

            if (MiddleName == string.Empty)
                MiddleName = null;

            if (AddressLine1 == string.Empty)
                AddressLine1 = null;

            if (AddressLine2 == string.Empty)
                AddressLine2 = null;

            if (AddressCity == string.Empty)
                AddressCity = null;

            if (ProgramType == string.Empty)
                ProgramType = null;

            HashCode = MD5(JsonConvert.SerializeObject(this));
        }

        public static string MD5(string str)
        {
            var encoder = Encoding.Unicode.GetEncoder();

            var unicodeText = new byte[str.Length * 2];

            encoder.GetBytes(str.ToCharArray(), 0, str.Length, unicodeText, 0, true);

            byte[] hash;
            using (MD5 md5 = new MD5CryptoServiceProvider())
                hash = md5.ComputeHash(unicodeText);

            var mappings = new uint[256];
            for (var i = 0; i < mappings.Length; i++)
            {
                var s = i.ToString("X2");
                mappings[i] = ((uint)s[0]) + ((uint)s[1] << 16);
            }

            return ByteArrayToHex(hash, mappings);
        }

        public static string ByteArrayToHex(byte[] value, uint[] mappings)
        {
            var result = new char[value.Length * 2];
            for (int i = 0, j = 0; i < value.Length; i++, j += 2)
                ByteToHex(result, j, value[i], mappings);
            return new string(result);
        }

        private static void ByteToHex(char[] array, int offset, byte value, uint[] mappings)
        {
            var mapping = mappings[value];
            array[offset] = (char)mapping;
            array[offset + 1] = (char)(mapping >> 16);
        }

        #endregion
    }
}