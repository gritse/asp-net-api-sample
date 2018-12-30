using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI.Dependencies
{
    internal class TransientDependency : Dependency
    {
        public TransientDependency(Type abstraction, Type implementation) : base(abstraction, implementation)
        {
        }

        public ConstructorInfo Constructor { get; set; }
        public List<Dependency> Dependencies { get; set; }
        
        protected object Create(Container container)
        {
            object[] arguments = Dependencies.Select(x => x.Build(container)).ToArray();
            return Constructor.Invoke(arguments);
        }

        public override object Build(Container container)
        {
            return Create(container);
        }

        public override Dependency CreatePartialCopy()
        {
            return new TransientDependency(Interface, Service) { Name = Name };
        }
    }
}
