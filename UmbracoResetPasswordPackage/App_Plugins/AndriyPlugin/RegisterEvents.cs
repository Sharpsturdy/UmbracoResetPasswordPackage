using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Umbraco.Core;

namespace UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin
{
  public class RegisterEvents : ApplicationEventHandler
  {
    protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
    {
       RouteTable.Routes.MapRoute(
            "AndriyCustomRoute",
            "ResetUserPassword/{action}/{id}",
            new
                {
                    controller = "ResetUserPassword", 
                    action = "Index", 
                    id = UrlParameter.Optional
                });           
      base.ApplicationStarted(umbracoApplication, applicationContext);
    }
  }
}