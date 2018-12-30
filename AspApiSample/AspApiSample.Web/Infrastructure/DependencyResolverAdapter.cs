using AspApiSample.DI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace AspApiSample.Web.Infrastructure
{
    public class DependencyResolverAdapter : IDependencyResolver
    {
        private readonly ConcurrentDictionary<Type, ParameterInfo[]> _cache = new ConcurrentDictionary<Type, ParameterInfo[]>();
        private readonly Container _container;

        public DependencyResolverAdapter(Container container)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public IDependencyScope BeginScope()
        {
            return new DependencyResolverAdapter(_container.CreateScope());
        }

        public object GetService(Type serviceType)
        {
            if (typeof(ApiController).IsAssignableFrom(serviceType))
            {
                return _container.Resolve(serviceType);
            }

            return null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return new object[] { };
        }

        public void Dispose()
        {
        }
    }
}