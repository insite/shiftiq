using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using InSite.Domain.Banks;

using Shift.Common;
using Shift.Common.Linq;
using Shift.Constant;

namespace InSite.Admin.Assessments.Questions.Utilities
{
    [Serializable]
    public class QuestionFilter
    {
        public Guid? StandardIdentifier { get; set; }
        public HashSet<FlagType> Flag { get; set; }
        public HashSet<string> Condition { get; set; }
        public int? Taxonomy { get; set; }
        public bool? HasLig { get; set; }
        public bool? HasReference { get; set; }

        public bool IsEmpty => !StandardIdentifier.HasValue
            && Flag.IsEmpty()
            && Condition.IsEmpty()
            && !Taxonomy.HasValue
            && !HasLig.HasValue
            && !HasReference.HasValue;

        #region Initialization

        static QuestionFilter()
        {
            QuestionFilterSerializer.Register<QuestionFilter>(15, _Read, Write);
        }

        #endregion

        #region Methods (serialization)

        protected static void Write(QuestionFilter filter, BinaryWriter writer)
        {
            writer.WriteNullable(filter.StandardIdentifier);

            if (filter.Flag != null)
            {
                writer.Write((byte)filter.Flag.Count);
                foreach (var flag in filter.Flag)
                    writer.Write((byte)flag);
            }
            else
            {
                writer.Write((byte)0);
            }

            if (filter.Condition != null)
            {
                writer.Write((byte)filter.Condition.Count);
                foreach (var condition in filter.Condition)
                    writer.Write(condition);
            }
            else
            {
                writer.Write((byte)0);
            }

            writer.Write((byte?)filter.Taxonomy ?? byte.MaxValue);
            writer.WriteNullable(filter.HasLig);
            writer.WriteNullable(filter.HasReference);
        }

        protected static void Read(QuestionFilter filter, BinaryReader reader)
        {
            filter.StandardIdentifier = reader.ReadGuidNullable();

            {
                var count = (int)reader.ReadByte();
                if (count > 0)
                {
                    filter.Flag = new HashSet<FlagType>();

                    while (count > 0)
                    {
                        var flag = (FlagType)reader.ReadByte();
                        if (Enum.IsDefined(typeof(FlagType), flag))
                            filter.Flag.Add(flag);
                        count--;
                    }
                }
            }

            {
                var count = (int)reader.ReadByte();
                if (count > 0)
                {
                    filter.Condition = new HashSet<string>();

                    while (count > 0)
                    {
                        var condition = reader.ReadString();
                        if (!string.IsNullOrEmpty(condition))
                            filter.Condition.Add(condition);
                        count--;
                    }
                }
            }

            {
                var v = reader.ReadByte();
                if (v != byte.MaxValue)
                    filter.Taxonomy = v;
            }

            filter.HasLig = reader.ReadBooleanNullable();
            filter.HasReference = reader.ReadBooleanNullable();
        }

        private static QuestionFilter _Read(BinaryReader reader)
        {
            var filter = new QuestionFilter();

            Read(filter, reader);

            return filter;
        }

        #endregion
    }

    public static class QuestionFilterSerializer
    {
        #region Classes

        private class FilterTypeInfo
        {
            #region Properties

            public byte FilterID { get; }
            public Type Type { get; }
            public Func<BinaryReader, QuestionFilter> Read { get; }
            public Action<QuestionFilter, BinaryWriter> Write { get; }

            #endregion

            #region Construction

            public FilterTypeInfo(
                byte id,
                Type type,
                Func<BinaryReader, QuestionFilter> read,
                Action<QuestionFilter, BinaryWriter> write)
            {
                FilterID = id;
                Type = type;
                Read = read;
                Write = write;
            }

            #endregion
        }

        #endregion

        #region Fields

        private static readonly List<FilterTypeInfo> _infos = new List<FilterTypeInfo>();

        #endregion

        #region Methods (mappings)

        public static void Register<T>(
            byte id,
            Func<BinaryReader, QuestionFilter> read,
            Action<QuestionFilter, BinaryWriter> write) where T : QuestionFilter
        {
            if (_infos.Any(x => x.FilterID == id))
                throw ApplicationError.Create("Question filter ID is already registered: {0}", id);

            var type = typeof(T);
            if (_infos.Any(x => x.Type == type))
                throw ApplicationError.Create("Question filter type is already registered: {0}", type.FullName);

            _infos.Add(new FilterTypeInfo(id, type, read, write));
        }

        #endregion

        #region Methods (serialization)

        private static FilterTypeInfo FindInfo(byte id) =>
            _infos.Where(x => x.FilterID == id).FirstOrDefault();

        private static FilterTypeInfo FindInfo(Type type) =>
            _infos.Where(x => x.Type == type).FirstOrDefault();

        public static string Serialize<T>(T filter) where T : QuestionFilter
        {
            var info = FindInfo(filter.GetType());

            return StringHelper.EncodeBase64Url(stream =>
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(info.FilterID);

                    info.Write(filter, writer);
                }
            });
        }

        public static QuestionFilter Deserialize(string data)
        {
            if (data.IsEmpty())
                return null;

            try
            {
                return StringHelper.DecodeBase64Url(data, stream =>
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        var info = FindInfo(reader.ReadByte());

                        return info == null ? null : info.Read(reader);
                    }
                });
            }
            catch
            {
                return null;
            }
        }

        #endregion
    }

    public static class QuestionFilterExtensions
    {
        public static Expression<Func<Question, bool>> BuildExpression(this QuestionFilter filter)
        {
            var expr = PredicateBuilder.True<Question>();

            if (filter.StandardIdentifier.HasValue)
                expr = expr.And(x => x.Standard == filter.StandardIdentifier.Value);

            if (filter.Flag != null)
                expr = expr.And(x => filter.Flag.Contains(x.Flag));

            if (filter.Condition != null)
                expr = expr.And(x => filter.Condition.Contains(x.Condition));

            if (filter.Taxonomy.HasValue)
                expr = expr.And(x => x.Classification.Taxonomy == filter.Taxonomy.Value);

            if (filter.HasLig.HasValue)
            {
                if (filter.HasLig.Value)
                    expr = expr.And(x => x.Classification.LikeItemGroup != null);
                else
                    expr = expr.And(x => x.Classification.LikeItemGroup == null);
            }

            if (filter.HasReference.HasValue)
            {
                if (filter.HasReference.Value)
                    expr = expr.And(x => x.Classification.Reference != null);
                else
                    expr = expr.And(x => x.Classification.Reference == null);
            }

            return expr;
        }

        public static IEnumerable<Question> Filter(this IEnumerable<Question> data, QuestionFilter filter)
        {
            var expr = filter.BuildExpression();

            return data.Where(x => expr.Invoke(x));
        }
    }
}