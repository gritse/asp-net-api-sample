using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI.Dependencies
{
    internal abstract class Dependency
    {
        protected Dependency(Type abstraction, Type implementation)
        {
            Interface = abstraction ?? throw new ArgumentNullException(nameof(abstraction));
            Service = implementation ?? throw new ArgumentNullException(nameof(implementation));
            Name = string.Empty;
        }

        public string Name { get; set; }
        public Type Interface { get; private set; }
        public Type Service { get; private set; }
        public abstract object Build(Container container);

        public abstract Dependency CreatePartialCopy();
    }
}
