using System;
using System.Collections.Generic;

namespace Shift.Sdk.UI
{
    public class AreaModel : AssetModel
    {
        public AreaModel(AssetModel model) : base(model)
        {
            Competencies = new List<CompetencyModel>();
        }

        public AreaModel(Guid id, string code, int number, Guid thumbprint, string title) : base(id, code, number, thumbprint, title)
        {
            Competencies = new List<CompetencyModel>();
        }

        public List<CompetencyModel> Competencies { get; set; }
    }
}