using Microsoft.Web.Http;
using Microsoft.Web.Http.Versioning;
using Microsoft.Web.Http.Versioning.Conventions;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using TheCodeCamp.Controllers;

namespace TheCodeCamp
{
  public static class WebApiConfig
  {
    public static void Register(HttpConfiguration config)
    {
      // Web API configuration and services
      AutofacConfig.Register();

            config.AddApiVersioning(cfg =>
            {
        // default version is 1,1
            cfg.DefaultApiVersion = new ApiVersion(1, 1);
        // assumes the we are using version 1,1
            cfg.AssumeDefaultVersionWhenUnspecified = true;
        // displays a new header named api-supported-versions (in postman headers when you call the api)
        // reports to people what versions are supported for the call they just made.
            cfg.ReportApiVersions = true;

        // chagne how we read the version
        //cfg.ApiVersionReader = new HeaderApiVersionReader("X-Version");

        //multiple versioning methods
            cfg.ApiVersionReader = ApiVersionReader.Combine(
            new HeaderApiVersionReader("X-Version"),
            new QueryStringApiVersionReader("ver"));

                // Versioning Conventions
                cfg.Conventions.Controller<TalksController>()
                        .HasApiVersion(1, 0)
                        .HasApiVersion(1, 1)
                        .Action(m => m.Get(default(string), default(int), default(bool)))
                        .MapToApiVersion(2,0);
        });

    // Change Case of JSON (eventDate instead of EventDate)
    config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

      // Web API routes
      config.MapHttpAttributeRoutes();

      //config.Routes.MapHttpRoute(
      //    name: "DefaultApi",
      //    routeTemplate: "api/{controller}/{id}",
      //    defaults: new { id = RouteParameter.Optional }
      //);
    }
  }
}
