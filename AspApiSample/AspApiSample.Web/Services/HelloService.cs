using AspApiSample.Web.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AspApiSample.Web.Services
{
    public class HelloService : IGreeter
    {
        public string SayHello()
        {
            return "Hi everyone!";
        }
    }
}