using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI.Dependencies
{
    internal class SingletonDependency : TransientDependency
    {
        private readonly object _sync = new object();
        private object _singleton;

        public SingletonDependency(Type abstraction, Type implementation) : base(abstraction, implementation)
        {
        }

        public override object Build(Container container)
        {
            if (_singleton == null)
            {
                lock (_sync)
                {
                    if (_singleton == null)
                    {
                        _singleton = Create(container);
                    }
                }
            }

            return _singleton;
        }

    }
}
