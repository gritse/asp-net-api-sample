using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspApiSample.DI.Dependencies
{
    internal class ConstantDependency : Dependency
    {
        private readonly object _constant;

        public ConstantDependency(Type abstraction, Type implementation, object constant) : base(abstraction, implementation)
        {
            _constant = constant ?? throw new ArgumentNullException(nameof(constant));
        }


        public override object Build(Container container)
        {
            return _constant;
        }
    }
}
