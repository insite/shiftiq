using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Shift.Common;
namespace InSite.Domain.Banks
{
    public static class BankHelper
    {
        #region Fields

        private static readonly HashSet<Type> _excludeObjTypes = new HashSet<Type>
        {
            typeof(MultilingualDictionary),
            typeof(MultilingualString),
            typeof(PivotTable),

            typeof(AttachmentImage),
            typeof(FormClassification),
            typeof(FormInvigilation),
            typeof(FormPublication),
            typeof(ImageDimension),
            typeof(Level),
            typeof(MatchingList),
            typeof(MatchingPair),
            typeof(OptionColumn),
            typeof(OptionLayout),
            typeof(QuestionClassification),
            typeof(Randomization),
            typeof(ScoreCalculation)
        };

        #endregion

        #region Methods (asset number)

        public static List<IHasAssetNumber> AssignAssetNumbers(object root, Func<int, IReadOnlyList<int>> numberFactory)
        {
            var assets = FindAssets(root);
            var numbers = numberFactory(assets.Count);

            for (var i = 0; i < numbers.Count; i++)
                assets[i].Asset = numbers[i];

            return assets;
        }

        public static List<IHasAssetNumber> FindAssets(object root)
        {
            var visitedObj = new HashSet<object>();
            var foundList = new List<IHasAssetNumber>();

            FindAssetNumberObjects(root, foundList, visitedObj);

            return foundList;
        }

        private static void FindAssetNumberObjects(object obj, List<IHasAssetNumber> list, HashSet<object> visitedObj)
        {
            if (visitedObj.Contains(obj))
                return;

            var objType = obj.GetType();

            if (IsValidObjType(objType))
            {
                visitedObj.Add(obj);

                if (obj is IHasAssetNumber assNum)
                    list.Add(assNum);

                var props = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
                foreach (var p in props)
                {
                    if (p.GetIndexParameters().Length > 0)
                        continue;

                    var pType = p.PropertyType;
                    if (!IsValidPropType(pType))
                        continue;

                    var value = p.GetValue(obj);
                    if (value == null)
                        continue;

                    FindAssetNumberObjects(value, list, visitedObj);
                }
            }

            if (objType.IsGenericType && typeof(IEnumerable).IsAssignableFrom(objType.GetGenericTypeDefinition()))
            {
                var genArgs = objType.GetGenericArguments();
                if (genArgs.Length == 1 && IsValidPropType(genArgs[0]))
                {
                    var enumerable = (IEnumerable)obj;
                    foreach (var item in enumerable)
                        FindAssetNumberObjects(item, list, visitedObj);
                }
            }

            bool IsValidPropType(Type t) =>
                t.IsClass && t != typeof(string);

            bool IsValidObjType(Type t) =>
                objType.IsGenericType ? !typeof(List<>).IsAssignableFrom(objType.GetGenericTypeDefinition()) : !_excludeObjTypes.Contains(t) && !t.IsSubclassOf(typeof(MultilingualDictionary));
        }

        #endregion

        #region Methods (identifier)

        private class ResetGuidState
        {
            public HashSet<object> VisitedObjects { get; } = new HashSet<object>();
            public Dictionary<Guid, Guid> IdMapping { get; } = new Dictionary<Guid, Guid>();
            public List<Tuple<object, PropertyInfo>> Properties { get; } = new List<Tuple<object, PropertyInfo>>();
            public List<IEnumerable> Enumerables { get; } = new List<IEnumerable>();
        }

        public static void ResetGuidIdentifiers(object obj, Dictionary<Guid, Guid> standardHooks)
        {
            var state = new ResetGuidState();

            ResetGuidIdentifiers(obj, state);

            if (standardHooks != null)
                foreach (var hook in standardHooks.Keys)
                    if (!state.IdMapping.ContainsKey(hook))
                        state.IdMapping.Add(hook, standardHooks[hook]);

            foreach (var prop in state.Properties)
            {
                var id = (Guid?)prop.Item2.GetValue(prop.Item1);
                if (id.HasValue && state.IdMapping.ContainsKey(id.Value))
                    prop.Item2.SetValue(prop.Item1, state.IdMapping[id.Value]);
            }

            foreach (var @enum in state.Enumerables)
            {
                if (@enum is IList list)
                {
                    for (var i = 0; i < list.Count; i++)
                    {
                        var oldId = (Guid?)list[i];
                        if (oldId.HasValue && state.IdMapping.TryGetValue(oldId.Value, out var newId))
                            list[i] = newId;
                    }
                }
                else if (@enum is ICollection<Guid> coll)
                {
                    foreach (var oldId in coll.ToArray())
                    {
                        if (state.IdMapping.TryGetValue(oldId, out var newId))
                        {
                            coll.Remove(oldId);
                            coll.Add(newId);
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException($"Unexpected enumerable type: " + @enum.GetType());
                }
            }

            ResetLikertRowIdentifiers(obj);
        }

        private static void ResetLikertRowIdentifiers(object obj)
        {
            if (!(obj is BankState bank))
                return;

            var questions = bank.Sets.SelectMany(s => s.Questions);

            foreach (var q in questions)
            {
                if (q.Likert == null || q.Likert.IsEmpty)
                    continue;

                q.Likert = q.Likert.Clone(false);
            }

        }

        private static void ResetGuidIdentifiers(object obj, ResetGuidState state)
        {
            if (state.VisitedObjects.Contains(obj))
                return;

            var objType = obj.GetType();

            if (IsValidObjType(objType))
            {
                state.VisitedObjects.Add(obj);

                var props = objType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty);
                foreach (var p in props)
                {
                    if (p.GetIndexParameters().Length > 0)
                        continue;

                    var pType = p.PropertyType;

                    if (pType == typeof(Guid) && p.Name == "Identifier")
                    {
                        var oldId = (Guid)p.GetValue(obj);
                        var newId = UuidFactory.Create();

                        p.SetValue(obj, newId);

                        if (oldId != Guid.Empty)
                        {
                            if (state.IdMapping.ContainsKey(oldId))
                                throw ApplicationError.Create($"Duplicate identifier found: {oldId}");

                            state.IdMapping.Add(oldId, newId);
                        }
                    }
                    else if (IsGuidType(pType))
                    {
                        state.Properties.Add(new Tuple<object, PropertyInfo>(obj, p));
                    }

                    if (!IsValidPropType(pType))
                        continue;

                    var value = p.GetValue(obj);
                    if (value == null)
                        continue;

                    ResetGuidIdentifiers(value, state);
                }
            }

            if (objType.IsGenericType)
            {
                var isHandled = false;
                var genTypeDef = objType.GetGenericTypeDefinition();

                if (typeof(IEnumerable).IsAssignableFrom(genTypeDef))
                {
                    var genArgs = objType.GetGenericArguments();
                    if (genArgs.Length == 1 && IsValidPropType(genArgs[0]))
                    {
                        var enumerable = (IEnumerable)obj;
                        foreach (var item in enumerable)
                            ResetGuidIdentifiers(item, state);

                        isHandled = true;
                    }
                }

                if (!isHandled && typeof(IEnumerable).IsAssignableFrom(genTypeDef))
                {
                    var genArgs = objType.GetGenericArguments();
                    if (genArgs.Length == 1 && IsGuidType(genArgs[0]))
                        state.Enumerables.Add((IEnumerable)obj);
                }
            }

            bool IsValidPropType(Type t) =>
                t.IsClass && t != typeof(string);

            bool IsValidObjType(Type t) =>
                objType.IsGenericType ? !typeof(List<>).IsAssignableFrom(objType.GetGenericTypeDefinition()) : !_excludeObjTypes.Contains(t);

            bool IsGuidType(Type t) =>
                t == typeof(Guid) || t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>) && Nullable.GetUnderlyingType(t) == typeof(Guid);
        }


        #endregion
    }
}
