using AspApiSample.DI.Attributes;
using AspApiSample.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AspApiSample.Web.Controllers
{
    public class HelloController : ApiController
    {
        private readonly IGreeter _greeter;

        public HelloController([DependencyName("Hello")] IGreeter greeter)
        {
            _greeter = greeter ?? throw new ArgumentNullException(nameof(greeter));
        }

        public string Get()
        {
            return _greeter.SayHello();
        }
    }
}