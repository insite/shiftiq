using System.Web.UI;

using InSite.Common;
using InSite.Domain.Payments;

namespace InSite.UI.Portal.Events.Classes.Controls
{
    public partial class CardDetail : UserControl
    {
        public UnmaskedCreditCard GetInputValues()
        {
            var card = new UnmaskedCreditCard();
            CreditCardCreator.Create(card, CardholderName.Text, CardNumber.Text, Expiry.Text, SecurityCode.Text, LabelHelper.GetTranslation);
            return card;
        }

        public virtual void Clear()
        {
            CardholderName.Text = null;
            CardNumber.Text = null;
            Expiry.Text = null;
            SecurityCode.Text = null;
        }
    }
}