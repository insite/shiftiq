using System;
using System.Collections.Generic;
using System.Linq;

using Shift.Common;

namespace InSite.Domain.Banks
{
    /// <summary>
    /// Questions can be classified using open-ended tag, taxonomy rank, difficulty rank, and/or like-item-group.
    /// </summary>
    [Serializable]
    public class QuestionClassification
    {
        public string Code
        {
            get => _code;
            set => _code = value.NullIfWhiteSpace();
        }

        public int? Difficulty { get; set; }

        public string LikeItemGroup
        {
            get => _likeItemGroup;
            set => _likeItemGroup = value.NullIfWhiteSpace();
        }

        public string Tag
        {
            get => _tag;
            set => _tag = value.NullIfWhiteSpace();
        }

        public int? Taxonomy { get; set; }

        public string Reference
        {
            get => _reference;
            set => _reference = value.NullIfWhiteSpace();
        }

        public List<Tuple<string, List<string>>> Tags { get; set; }

        private string _code;
        private string _likeItemGroup;
        private string _tag;
        private string _reference;

        #region Methods (comparison)

        public bool IsEmpty
        {
            get
            {
                return this.Tag == null
                    && this.Difficulty == null
                    && this.LikeItemGroup == null
                    && this.Taxonomy == null
                    && this.Reference == null
                    && this.Code == null
                    && Tags.IsEmpty();
            }
        }

        public bool Equals(QuestionClassification other)
        {
            return this.Tag == other.Tag
                && this.Difficulty == other.Difficulty
                && this.LikeItemGroup == other.LikeItemGroup
                && this.Taxonomy == other.Taxonomy
                && this.Reference == other.Reference
                && this.Code == other.Code
                && AreTagsEqual(other);
        }

        private bool AreTagsEqual(QuestionClassification other)
        {
            if ((Tags?.Count ?? 0) != (other.Tags?.Count ?? 0))
                return false;

            if (Tags == null || other.Tags == null)
                return true;

            for (int i = 0; i < Tags.Count; i++)
            {
                var tuple = Tags[i];
                var otherTuple = other.Tags[i];

                if (!tuple.Item1.Equals(otherTuple.Item1, StringComparison.OrdinalIgnoreCase) || tuple.Item2.Count != otherTuple.Item2.Count)
                    return false;

                for (int j = 0; j < tuple.Item2.Count; j++)
                {
                    if (!tuple.Item2[j].Equals(otherTuple.Item2[j], StringComparison.OrdinalIgnoreCase))
                        return false;
                }
            }

            return true;
        }

        #endregion

        #region Methods (helpers)

        public void Copy(QuestionClassification source)
        {
            source.ShallowCopyTo(this);

            Tags = source.Tags.EmptyIfNull().Select(x => new Tuple<string, List<string>>(x.Item1, x.Item2.ToList())).ToList();
        }

        public QuestionClassification Clone()
        {
            var clone = new QuestionClassification();
            clone.Copy(this);
            return clone;
        }

        #endregion
    }
}
