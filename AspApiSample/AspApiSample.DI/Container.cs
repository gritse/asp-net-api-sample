using AspApiSample.DI.Dependencies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI
{
    public class Container
    {
        internal ConcurrentDictionary<Dependency, Lazy<object>> ScopedInstances { get; private set; }
    }
}
