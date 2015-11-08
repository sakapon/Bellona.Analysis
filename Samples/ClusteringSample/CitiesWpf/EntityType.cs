using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CitiesWpf
{
    public static class EntityType
    {
        public static EntityType<TEntity> Create<TEntity>(TEntity baseObj)
        {
            var constructors = typeof(TEntity).GetConstructors();
            if (constructors.Length != 1) throw new InvalidOperationException("The number of the constructors must be 1.");

            return new EntityType<TEntity>(constructors[0]);
        }

        public static EntityType<TEntity> Create<TEntity>(Expression<Func<TEntity>> initializer)
        {
            if (initializer == null) throw new ArgumentNullException("initializer");

            var @new = initializer.Body as NewExpression;
            if (@new == null) throw new InvalidOperationException("The constructor must be specified.");

            return new EntityType<TEntity>(@new.Constructor);
        }
    }

    [DebuggerDisplay(@"{typeof(TEntity)}")]
    public class EntityType<TEntity>
    {
        public ConstructorInfo ConstructorInfo { get; private set; }

        internal EntityType(ConstructorInfo constructorInfo)
        {
            ConstructorInfo = constructorInfo;
        }

        public TEntity CreateEntity(params object[] parameters)
        {
            return (TEntity)ConstructorInfo.Invoke(parameters);
        }
    }
}
