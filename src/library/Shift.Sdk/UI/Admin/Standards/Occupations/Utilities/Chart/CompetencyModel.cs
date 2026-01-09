using System;

namespace Shift.Sdk.UI
{
    public class CompetencyModel : AssetModel
    {
        public CompetencyModel(AssetModel model) : base(model)
        {
        }

        public CompetencyModel(Guid id, string code, int number, Guid thumbprint, string title) : base(id, code, number, thumbprint, title)
        {
        }

        public AreaModel Area { get; set; }
    }
}