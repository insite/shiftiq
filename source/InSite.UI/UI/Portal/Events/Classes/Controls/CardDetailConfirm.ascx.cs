using System.Text;
using System.Web.UI;

using InSite.Domain.Payments;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class CardDetailConfirm : UserControl
    {
        public void SetInputValues(UnmaskedCreditCard card)
        {
            CardholderName.Text = card.CardholderName;
            CardNumber.Text = MaskCardNumber(card.CardNumber);
            Expiry.Text = $"{card.ExpiryMonth.ToString("00")}/{card.ExpiryYear.ToString("00")}";
        }

        private static string MaskCardNumber(string cardNumber)
        {
            var mask = new StringBuilder(cardNumber);

            for (int i = 4; i < mask.Length - 4; i++)
            {
                if (mask[i] != ' ')
                    mask[i] = '*';
            }

            return mask.ToString();
        }
    }
}