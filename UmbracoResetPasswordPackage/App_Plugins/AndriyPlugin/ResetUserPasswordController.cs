using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin
{

  public class ResetUserPasswordController : PluginController
  {
    public ResetUserPasswordController()
      : this(UmbracoContext.Current)
    {
    }

    public ResetUserPasswordController(UmbracoContext umbracoContext)
      : base(umbracoContext)
    {
    }
    public ActionResult Index()
    {

      string password = "Andriy123";
      string Hash = Umbraco.CreateMd5Hash(password);
      ViewBag.password=password;
      ViewBag.Hash=Hash;
      ViewBag.NewHash=Umbraco.CreateMd5Hash(password);
      return View();
    }
  }
}