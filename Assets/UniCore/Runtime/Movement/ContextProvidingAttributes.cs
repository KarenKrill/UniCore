using System;

namespace KarenKrill.UniCore.Movement
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class RequiredContextAttribute : Attribute
    {
        public Type ContextType { get; }

        public RequiredContextAttribute(Type contextType)
        {
            ContextType = contextType;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ProvidesContextAttribute : Attribute
    {
        public Type ContextType { get; }

        public ProvidesContextAttribute(Type contextType)
        {
            ContextType = contextType;
        }
    }
}
