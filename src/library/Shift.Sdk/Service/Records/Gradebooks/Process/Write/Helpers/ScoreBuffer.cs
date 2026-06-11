using System;
using System.Collections.Generic;

namespace InSite.Application.Records.Write
{
    public class ScoreBuffer
    {
        private Dictionary<string, decimal?> _scores = new Dictionary<string, decimal?>();

        private string CreateKey(Guid user, Guid gradeitem)
            => $"{user}.{gradeitem}";

        public bool Exists(Guid user, Guid gradeitem)
            => _scores.ContainsKey(CreateKey(user, gradeitem));

        public decimal? Get(Guid user, Guid gradeitem)
            => _scores[CreateKey(user, gradeitem)];

        public void Set(Guid user, Guid gradeitem, decimal? score)
            => _scores[CreateKey(user, gradeitem)] = score;
    }
}