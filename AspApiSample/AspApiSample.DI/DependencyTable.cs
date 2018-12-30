using AspApiSample.DI.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI
{
    internal class DependencyTable : Dictionary<Type, Dictionary<string, Dependency>>
    {
    }
}
