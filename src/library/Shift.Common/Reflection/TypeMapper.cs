using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Shift.Common
{
    public interface ITypeMapper
    {
        int Get(Type t);
        ITypeMapperInfo Get(int id);
    }

    public interface ITypeMapperInfo
    {
        Type Type { get; }
        object Create();
    }

    public class TypeMapper<T> : ITypeMapper
    {
        #region Classes

        public class Info : ITypeMapperInfo
        {
            public Type Type { get; }
            public Func<T> Create { get; }

            public Info(Type t)
            {
                Type = t;
                Create = Expression.Lambda<Func<T>>(Expression.New(t)).Compile();
            }

            object ITypeMapperInfo.Create() => Create();
        }

        #endregion

        #region Fields

        private readonly Info[] _data;

        #endregion

        #region Construction

        public TypeMapper()
            : this(Assembly.GetAssembly(typeof(T)).GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
        {

        }

        public TypeMapper(IEnumerable<Type> types)
        {
            _data = types.OrderBy(t => t.FullName)
                .Select(t => new Info(t))
                .ToArray();
        }

        #endregion

        #region Methods

        public int Get(Type t)
        {
            for (var i = 0; i < _data.Length; i++)
                if (_data[i].Type == t)
                    return i + 1;

            throw ApplicationError.Create("Type not found: {0}", t.FullName);
        }

        public Info Get(int id)
        {
            if (id > 0 && id <= _data.Length)
                return _data[id - 1];

            throw ApplicationError.Create("Type not found: {0}", id);
        }

        ITypeMapperInfo ITypeMapper.Get(int id) => Get(id);

        #endregion
    }
}
