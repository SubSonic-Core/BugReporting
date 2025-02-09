using System;

namespace SubSonic.Logging
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DisplayNameAttribute
        : Attribute
    {
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
