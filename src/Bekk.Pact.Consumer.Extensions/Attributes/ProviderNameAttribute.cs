using System;

namespace Bekk.Pact.Consumer.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public class ProviderNameAttribute : Attribute
    {
        public string Name { get; }
        public ProviderNameAttribute(string name)
        {
            Name = name;
        }
        public override string ToString() => Name;
    }
}