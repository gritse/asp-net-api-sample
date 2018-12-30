using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI.Dependencies
{
    internal class ScopedDependency : TransientDependency
    {
        public ScopedDependency(Type abstraction, Type implementation) : base(abstraction, implementation)
        {
        }

        public override object Build(Container container)
        {
            // In ConcurrentDictionary it's not garanteed than valueFatory will be executed only once, so I use Lazy<object> to ensure than constructor
            // will call only once

            Lazy<object> lazyFactory = container.ScopedInstances.GetOrAdd(this, _ => new Lazy<object>(() => Create(container), true));
            return lazyFactory.Value;
        }

        public override Dependency CreatePartialCopy()
        {
            return new ScopedDependency(Interface, Service) { Name = Name };
        }
    }
}
