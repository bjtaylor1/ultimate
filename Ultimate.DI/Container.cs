﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Ultimate.DI
{
    public class Container : IContainer
    {
        private readonly ConcurrentDictionary<Type, ImmutableList<InstanceDelivery>> types = new ConcurrentDictionary<Type, ImmutableList<InstanceDelivery>>();
        private readonly ConcurrentDictionary<InstanceDelivery, object> singletons = new ConcurrentDictionary<InstanceDelivery, object>();
        private readonly ConcurrentDictionary<Type, ConstructorInfo> constructors = new ConcurrentDictionary<Type, ConstructorInfo>();

        public bool AutoResolveConcreteTypes { get; set; }

        public void AddTransient<TImplementation>() => AddTransient<TImplementation, TImplementation>();
        public void AddSingleton<TImplementation>() => AddSingleton<TImplementation, TImplementation>();

        private void Add(Type type, InstanceDelivery instanceDelivery) =>
            types.AddOrUpdate(type,
                t => new[] {instanceDelivery}.ToImmutableList(),
                (t, l) =>
                {
                    var b = l.ToBuilder();
                    b.Add(instanceDelivery);
                    return b.ToImmutable();
                });

        public void AddTransient<TService, TImplementation>()
        {
            constructors.GetOrAdd(typeof(TImplementation), GetConstructor); //mainly to check it (fail fast) but cache it now we've created it
            Add(typeof(TService), new InstanceDelivery(LifetimeStatus.Transient, typeof(TImplementation)));
        }

        public void AddSingleton<TService, TImplementation>()
        {
            constructors.GetOrAdd(typeof(TImplementation), GetConstructor); //mainly to check it (fail fast) but cache it now we've created it
            Add(typeof(TService), new InstanceDelivery(LifetimeStatus.Singleton, typeof(TImplementation)));
        }

        public void AddInstance<TService>(TService instance)
        { 
            var instanceDelivery = new InstanceDelivery(LifetimeStatus.Singleton, instance.GetType()); // don't check the constructor
            singletons.AddOrUpdate(instanceDelivery, instance, (key, existing) => instance);
            Add(typeof(TService), instanceDelivery);
        }

        public IContainer GetNestedContainer() => throw new NotImplementedException();

        public T Resolve<T>() => (T) Resolve(new Stack<Type>(), typeof(T));

        private object Resolve(Stack<Type> creating, Type type)
        {
            var isEnumerable = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
            var typeToLookup = isEnumerable ? type.GetGenericArguments().Single() : type;

            if (!types.TryGetValue(typeToLookup, out var instanceDeliveries))
            {
                var allTypes = new[] {typeToLookup}.Concat(creating.Reverse()).ToArray();
                throw new ResolutionException("Resolution failed when creating the following types:" +
                                              $"{Environment.NewLine}{string.Join(Environment.NewLine, allTypes.Select(t => t.FullName))}");
            }

            var resolution = isEnumerable ? instanceDeliveries.Select(c => Resolve(creating, c)) : Resolve(creating, instanceDeliveries.Last());

            return resolution;
        }

        private object Resolve(Stack<Type> creating, InstanceDelivery instanceDelivery)
        {
            switch (instanceDelivery.Lifetime)
            {
                case LifetimeStatus.Singleton:
                    return singletons.GetOrAdd(instanceDelivery, d => Create(creating, d));
                case LifetimeStatus.Transient:
                    return Create(creating, instanceDelivery);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private object Create(Stack<Type> creating, InstanceDelivery instanceDelivery)
        {
            if (creating.Contains(instanceDelivery.Type))
            {
                var allTypes = new[] { instanceDelivery.Type }.Concat(creating.Reverse()).ToArray();

                throw new ResolutionException("Circular reference when creating the following types:" +
                                              $"{Environment.NewLine}{string.Join(Environment.NewLine, allTypes.Select(t => t.FullName))}");
            }

            creating.Push(instanceDelivery.Type);
            try
            {
                var constructor = constructors.GetOrAdd(instanceDelivery.Type, GetConstructor); //should always already be in.
                var parameterValues = new List<object>();
                foreach (var parameterInfo in constructor.GetParameters())
                {
                    var parameterValue = Resolve(creating, parameterInfo.ParameterType);
                    parameterValues.Add(parameterValue);
                }

                try
                {
                    var resolution = constructor.Invoke(parameterValues.ToArray());
                    return resolution;
                }
                catch (Exception e)
                {
                    var allTypes = creating.Reverse().ToArray(); //already got the type we're trying to create in the stack
                    throw new ResolutionException("Resolution failed when creating the following types:" +
                                                  $"{Environment.NewLine}{string.Join(Environment.NewLine, allTypes.Select(t => t.FullName))}",
                        e);
                }


            }
            finally
            {
                creating.Pop();
            }
        }

        private static ConstructorInfo GetConstructor(Type t)
        {
            var defaultConstructor = t.GetConstructor(Type.EmptyTypes);
            if (defaultConstructor != null) return defaultConstructor;

            var allConstructors = t.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if (allConstructors.Length != 1)
            {
                throw new RegistrationException($"Type must have exactly one public constructor: {t.FullName}");
            }
            
            return allConstructors.Single();
        }
    }
}