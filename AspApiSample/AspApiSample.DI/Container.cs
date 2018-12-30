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
        private readonly Dictionary<Type, Dependency> _dependencies;

        internal Container(Dictionary<Type, Dependency> dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            ScopedInstances = new ConcurrentDictionary<Dependency, Lazy<object>>();
        }

        internal ConcurrentDictionary<Dependency, Lazy<object>> ScopedInstances { get; private set; }

        public object Get(Type abstraction)
        {
            if (_dependencies.TryGetValue(abstraction, out Dependency dependency) == false)
                throw new InvalidOperationException($"Dependency {abstraction} not registered");

            return dependency.Build(this);
        }
    }
}
