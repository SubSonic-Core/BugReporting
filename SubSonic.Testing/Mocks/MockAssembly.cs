using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SubSonic.Testing.Mocks
{
    public class MockAssembly : Assembly
    {
        private readonly string informationalVersion;

        public MockAssembly(string informationalVersion)
        {
            this.informationalVersion = informationalVersion;
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return new Attribute[] { new AssemblyInformationalVersionAttribute(informationalVersion) };
        }
    }
}
