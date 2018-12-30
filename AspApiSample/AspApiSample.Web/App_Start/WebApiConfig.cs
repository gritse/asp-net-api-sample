using AspApiSample.DI;
using AspApiSample.Web.Infrastructure;
using AspApiSample.Web.Interfaces;
using AspApiSample.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AspApiSample.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            ContainerBuilder builder = new ContainerBuilder();

            builder.AddTransient<IGreeter, HelloService>("Hello");
            builder.AddTransient<IGreeter, HiService>("Hi");

            Container container = builder.Build();
            config.DependencyResolver = new DependencyResolverAdapter(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}"
            );
        }
    }
}
