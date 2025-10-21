using Shift.Common;

namespace InSite.Common.Web.UI
{
    public class HonorificComboBox : ComboBox
    {
        protected override BindingType ControlBinding => BindingType.Code;

        protected override ListItemArray CreateDataSource()
        {
            var list = new ListItemArray();

            list.Add("Dr.");
            list.Add("Hon.");
            list.Add("Miss");
            list.Add("Mme.");
            list.Add("Mr.");
            list.Add("Mrs.");
            list.Add("Ms.");
            list.Add("Prof.");

            return list;
        }
    }
}
