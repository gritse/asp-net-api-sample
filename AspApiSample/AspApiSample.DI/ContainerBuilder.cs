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

        public Container Build()
        {
            IEnumerable<Dependency> dependencies = BuildTree(_dependencies);
            Dictionary<Type, Dependency> table = ToDependencyTable(dependencies);

            return new Container(table);
        }

        private static IEnumerable<Dependency> BuildTree(IEnumerable<Dependency> items)
        {
            Dependency[] dependencies = items.Select(x => x.CreatePartialCopy()).ToArray();

            foreach (Dependency item in dependencies)
            {
                if (item is TransientDependency dependency)
                {
                    ConstructorInfo ctor = GetConstructor(dependency);

                    Type[] arguments = ctor.GetParameters().Select(x => x.ParameterType).ToArray();

                    dependency.Dependencies = new List<Dependency>(arguments.Length);
                    dependency.Constructor = ctor;

                    foreach (Type arg in arguments)
                    {
                        Dependency[] deps = dependencies.Where(d => d.Interface == arg).ToArray();

                        if (deps.Length > 1) throw new InvalidOperationException($"Ambiguous dependency {arg} for service {dependency.Service}");
                        if (deps.Length == 0) throw new InvalidOperationException($"Can't find dependency {arg} for service {dependency.Service}");

                        dependency.Dependencies.Add(deps.Single());
                    }
                }
            }

            return dependencies;
        }

        private static Dictionary<Type, Dependency> ToDependencyTable(IEnumerable<Dependency> dependencies)
        {
            var dependencyTable = new Dictionary<Type, Dependency>();

            foreach (Dependency dependency in dependencies)
            {
                if (dependencyTable.ContainsKey(dependency.Interface))
                    throw new InvalidOperationException($"Ambiguous interface {dependency.Interface}");

                dependencyTable.Add(dependency.Interface, dependency);
            }

            return dependencyTable;
        }

        private static ConstructorInfo GetConstructor(TransientDependency dependency)
        {
            ConstructorInfo[] constructors = dependency.Service.GetConstructors(BindingFlags.Public | BindingFlags.Instance);

            if (constructors.Length == 0)
                throw new InvalidOperationException($"Service {dependency.Service} hasn't any public constructors");

            // Service should has only one costructor or parameterless
            if (constructors.Length > 1 && constructors.Any(c => c.GetParameters().Length == 0) == false)
                throw new InvalidOperationException($"Service {dependency.Service} has many constructors and hasn't parameterless constructor");

            ConstructorInfo ctor = constructors.Length > 1
                ? constructors.First(c => c.GetParameters().Length == 0)
                : constructors.First();

            return ctor;
        }
    }
}
