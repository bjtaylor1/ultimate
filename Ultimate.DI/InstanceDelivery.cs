using System;

namespace Ultimate.DI
{
    internal class InstanceDelivery
    {
        public InstanceDelivery(LifetimeStatus lifetime, Type type)
        {
            Lifetime = lifetime;
            Type = type;
        }

        public LifetimeStatus Lifetime { get; }
        public Type Type { get; }
    }
}