using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class FrameworkModel : AssetModel
    {
        public FrameworkModel(AssetModel model) : base(model)
        {
            Areas = new List<AreaModel>();
        }

        public FrameworkModel(Guid id, string code, int number, Guid thumbprint, string title) : base(id, code, number, thumbprint, title)
        {
            Areas = new List<AreaModel>();
        }

        public List<AreaModel> Areas { get; set; }
    }
}