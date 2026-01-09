using System;
using System.ComponentModel;

namespace InSite.Domain.Payments
{
    public class MaskedCreditCard
    {
        private string _number;
        private string _cvd;

        public string GetNumber() => _number;
        public string SetNumber(string number) => _number = number;

        public string GetCvd() => _cvd;
        public string SetCvd(string cvd) => _cvd = cvd;

        public string Number { get { return MaskNumber(_number); } set { _number = MaskNumber(value); } }
        public string Name { get; set; }

        public int ExpiryMonth { get; set; }
        public int ExpiryYear { get; set; }

        public static string MaskNumber(string number)
        {
            if (string.IsNullOrEmpty(number) || number.Length <= 4)
                return number;

            var lastDigits = number.Substring(number.Length - 4);
            var newNumber = $"**** {lastDigits}";

            return newNumber;
        }
    }

    [Serializable]
    public class UnmaskedCreditCard
    {
        public string CardholderName { get; set; }
        public string CardNumber { get; set; }
        public int ExpiryMonth { get; set; } = -1;
        public int ExpiryYear { get; set; } = -1;
        public string SecurityCode { get; set; }

        [DefaultValue(true)]
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = null;
    }
}