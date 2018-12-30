using AspApiSample.DI.Attributes;
using AspApiSample.DI.Dependencies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI
{
    public class ContainerBuilder
    {
        private readonly List<Dependency> _dependencies = new List<Dependency>();

        public void AddSingleton<TInterface, TService>() where TService : TInterface
        {
            _dependencies.Add(new SingletonDependency(typeof(TInterface), typeof(TService)));
        }

        public void AddSingleton<TInterface, TService>(TService constant) where TService : TInterface
        {
            _dependencies.Add(new ConstantDependency(typeof(TInterface), typeof(TService), constant));
        }

        public void AddScoped<TInterface, TService>() where TService : TInterface
        {
            _dependencies.Add(new ScopedDependency(typeof(TInterface), typeof(TService)));
        }

        public void AddTransient<TInterface, TService>() where TService : TInterface
        {
            _dependencies.Add(new TransientDependency(typeof(TInterface), typeof(TService)));
        }

        public void AddSingleton<TInterface, TService>(string name) where TService : TInterface
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _dependencies.Add(new SingletonDependency(typeof(TInterface), typeof(TService)) { Name = name });
        }

        public void AddSingleton<TInterface, TService>(TService constant, string name) where TService : TInterface
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _dependencies.Add(new ConstantDependency(typeof(TInterface), typeof(TService), constant) { Name = name });
        }

        public void AddScoped<TInterface, TService>(string name) where TService : TInterface
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _dependencies.Add(new ScopedDependency(typeof(TInterface), typeof(TService)) { Name = name });
        }

        public void AddTransient<TInterface, TService>(string name) where TService : TInterface
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            _dependencies.Add(new TransientDependency(typeof(TInterface), typeof(TService)) { Name = name });
        }

        public Container Build()
        {
            IEnumerable<Dependency> dependencies = BuildTree(_dependencies);
            DependencyTable table = ToDependencyTable(dependencies);

            ThrowOnСircularDependency(dependencies);

            return new Container(table);
        }

        private static void ThrowOnСircularDependency(IEnumerable<Dependency> dependencies)
        {
            // I use primitive algorithm O(n2), because in this case method usually called only once at startup
            // also, typically, the dependencies count about 100-200

            foreach (TransientDependency vertex in dependencies.OfType<TransientDependency>())
            {
                if (DepthFirstSearch(vertex, new List<Dependency>()))
                {
                    throw new InvalidOperationException($"Сircular dependency for {vertex.Interface}");
                }
            }
        }

        private static bool DepthFirstSearch(TransientDependency vertex, List<Dependency> visited)
        {
            if (visited.Contains(vertex)) return true;
            visited.Add(vertex);

            foreach (TransientDependency dependency in vertex.Dependencies.OfType<TransientDependency>())
            {
                if (DepthFirstSearch(dependency, visited) == true) return true;
            }

            return false;
        }

        private static IEnumerable<Dependency> BuildTree(IEnumerable<Dependency> items)
        {
            Dependency[] dependencies = items.Select(x => x.CreatePartialCopy()).ToArray();

            foreach (Dependency item in dependencies)
            {
                if (item is TransientDependency dependency)
                {
                    ConstructorInfo ctor = GetConstructor(dependency.Service);

                    ParameterInfo[] parameters = ctor.GetParameters();

                    dependency.Dependencies = new List<Dependency>(parameters.Length);
                    dependency.Constructor = ctor;

                    foreach (ParameterInfo param in parameters)
                    {
                        Type argument = param.ParameterType;
                        var attribute = param.GetCustomAttribute<DependencyNameAttribute>();

                        Dependency[] deps = dependencies.Where(d => d.Interface == argument && (attribute == null || d.Name == attribute.Name)).ToArray();

                        if (deps.Length > 1) throw new InvalidOperationException($"Ambiguous dependency {argument} for service {dependency.Service}");
                        if (deps.Length == 0) throw new InvalidOperationException($"Can't find dependency {argument} for service {dependency.Service}");

                        dependency.Dependencies.Add(deps.Single());
                    }
                }
            }

            return dependencies;
        }

        private static DependencyTable ToDependencyTable(IEnumerable<Dependency> dependencies)
        {
            var dependencyTable = new DependencyTable();

            foreach (var dependencyGroup in dependencies.GroupBy(x => x.Interface))
            {
                var bucket = new Dictionary<string, Dependency>();

                foreach (Dependency dependency in dependencyGroup)
                {
                    if (bucket.ContainsKey(dependency.Name))
                        throw new InvalidOperationException($"Ambiguous interface {dependency.Interface}");

                    bucket.Add(dependency.Name, dependency);
                }

                dependencyTable[dependencyGroup.Key] = bucket;
            }

            return dependencyTable;
        }

        internal static ConstructorInfo GetConstructor(Type service)
        {
            ConstructorInfo[] constructors = service.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
                throw new InvalidOperationException($"Service {service} hasn't any public constructors");

            // Service should has only one costructor or parameterless
            if (constructors.Length > 1 && constructors.Any(c => c.GetParameters().Length == 0) == false)
                throw new InvalidOperationException($"Service {service} has many constructors and hasn't parameterless constructor");

            ConstructorInfo ctor = constructors.Length > 1
                ? constructors.First(c => c.GetParameters().Length == 0)
                : constructors.First();

            return ctor;
        }
    }
}
