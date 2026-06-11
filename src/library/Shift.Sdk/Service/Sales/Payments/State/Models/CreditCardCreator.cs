using System;

namespace InSite.Domain.Payments
{
    public static class CreditCardCreator
    {
        public static void Create(
            UnmaskedCreditCard card,
            string cardholderName,
            string cardNumber,
            string expiry,
            string securityCode,
            Func<string, string> getTranslation)
        {
            card.CardholderName = cardholderName;
            card.CardNumber = cardNumber.Replace(" ", string.Empty);

            var expiryDate = CreditCardExpiry.Parse(expiry);
            if (expiryDate != null)
            {
                card.ExpiryMonth = expiryDate.Month;
                card.ExpiryYear = expiryDate.Year;
            }

            card.SecurityCode = securityCode;

            if (card.CardNumber.Length != 15 && card.CardNumber.Length != 16)
            {
                card.IsValid = false;
                card.ErrorMessage = $"{getTranslation("Validation Error: Card Number - Valid format")} 0000 0000 0000 0000";
            }
            else if (expiryDate == null)
            {
                card.IsValid = false;
                card.ErrorMessage = $"{getTranslation("Validation Error: Expiry Date - Valid format")} mm/yy";
            }
            else if (expiryDate.Month < 1 || expiryDate.Month > 12)
            {
                card.IsValid = false;
                card.ErrorMessage = $"{getTranslation("Validation Error: Expiry Date - Invalid month (format ")} mm/yy)";
            }
            else if (DateTime.Now > new DateTime(expiryDate.Year + 2000, expiryDate.Month, 1).AddMonths(1))
            {
                card.IsValid = false;
                card.ErrorMessage = $"{getTranslation("Validation Error: Expiry Date - Your card is expired (format")} mm/yy)";
            }
            else if (card.SecurityCode.Length != 3 && card.SecurityCode.Length != 4)
            {
                card.IsValid = false;
                card.ErrorMessage = getTranslation("Validation Error: CVC - Valid format 000 or 0000");
            }
        }
    }
}
