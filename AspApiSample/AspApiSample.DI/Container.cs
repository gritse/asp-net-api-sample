using AspApiSample.DI.Attributes;
using AspApiSample.DI.Dependencies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI
{
    public class Container
    {
        private readonly DependencyTable _dependencies;

        internal Container(DependencyTable dependencies)
        {
            _dependencies = dependencies ?? throw new ArgumentNullException(nameof(dependencies));
            ScopedInstances = new ConcurrentDictionary<Dependency, Lazy<object>>();
        }

        internal ConcurrentDictionary<Dependency, Lazy<object>> ScopedInstances { get; private set; }

        public object Get(Type abstraction)
        {
            if (_dependencies.TryGetValue(abstraction, out Dictionary<string, Dependency> table) == false)
                throw new InvalidOperationException($"Dependency {abstraction} not registered");

            if (table.Count > 1) throw new InvalidOperationException($"Ambiguous services for {abstraction}");
            return table.Single().Value.Build(this);
        }

        public object Get(Type abstraction, string name)
        {
            if (_dependencies.TryGetValue(abstraction, out Dictionary<string, Dependency> table) == false || table.TryGetValue(name, out Dependency dependency) == false)
                throw new InvalidOperationException($"Dependency {abstraction} with name {name} not registered");

            return dependency.Build(this);
        }

        public object Resolve(Type type)
        {
            ConstructorInfo ctor = ContainerBuilder.GetConstructor(type);
            ParameterInfo[] parameters = ctor.GetParameters();

            object[] arguments = parameters.Select(param =>
            {
                var attribute = param.GetCustomAttribute<DependencyNameAttribute>();
                Type argument = param.ParameterType;

                return attribute == null ? this.Get(argument) : this.Get(argument, attribute.Name);
            }).ToArray();

            return ctor.Invoke(arguments);
        }

        public Container CreateScope()
        {
            return new Container(_dependencies);
        }
    }
}
