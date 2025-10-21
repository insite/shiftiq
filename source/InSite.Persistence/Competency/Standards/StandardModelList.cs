using System;
using System.Collections.Generic;

namespace InSite.Persistence
{
    [Serializable]
    public class StandardModelList : List<StandardModel>
    {
        public StandardModel FindByNumber(int number)
        {
            return Find(x => x.AssetNumber == number);
        }
    }
}