using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Core.Models.Membership;
using System.Web.Security;
using umbraco;
using Umbraco.Core.Configuration;
using Umbraco.Core.Configuration.UmbracoSettings;
using UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin.Models;
using System.Threading.Tasks;
using System.Net.Mail;
using Umbraco.Core.Services;

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
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Index(ResetPasswordModel model)
    {
      ModelState["Password"].Errors.Clear();
      ModelState["PasswordConfirm"].Errors.Clear();
      if (ModelState.IsValid == false) return View();

      IUser user = ApplicationContext.Services.UserService.GetByEmail(model.Email);
      if (user == null)
      {
        ModelState.AddModelError("", "Email is not found!");
        return View(model);
      }
      ResetTokensStore store = new ResetTokensStore();
      ResetToken token = store.CreateToken(user.Id);
      store.Dispose();
      string url = Url.Action("ResetPassword", new { id = token.UserID, token = token.HashedToken });
      await SendEmailAsync(url, user.Email);
      return View("EmailSent");
    }

    public ActionResult ResetPassword(int id, string token)
    {
      return View(new ResetPasswordModel { ResetToken = token, UserID = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ResetPassword(ResetPasswordModel model)
    {
      if (ModelState.IsValid == false) return View(model);
      IUserService service = ApplicationContext.Services.UserService;
      IUser user = service.GetUserById(model.UserID);
      if (user == null)
      {
        ModelState.AddModelError("", "Can't reset password. Wrong user data.");
        return View(model);
      }
      ResetTokensStore store = new ResetTokensStore();
      bool isTokenOK = store.IsTokenCorrect(model.UserID, model.ResetToken);
      store.Dispose();
      if (isTokenOK == false)
      {
        ModelState.AddModelError("", "Can't reset password. Wrong recovey link.");
        return View(model);
      }

      if (model.Email.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase) == false)
      {
        ModelState.AddModelError("", "Email is not correct.");
        return View(model);
      }
      try
      {
        service.SavePassword(user, model.Password);
      }
      catch (Exception)
      {
        ModelState.AddModelError("", "Error occuried during password update.");
        return View(model);
      }
      using (ResetTokensStore s = new ResetTokensStore())
      {
        s.DeleteToken(model.UserID);
      }
      return View("PasswordUpdated");
    }

    async Task SendEmailAsync(string aRecoverUrl, string email)
    {
      // Plug in your email service here to send an email.
      SmtpClient smtpClient = new SmtpClient();
      smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
      MailMessage mail = new MailMessage();
      mail.IsBodyHtml = true;
      mail.Subject = "Password recovery link";
      mail.Body = "To recover password <a href='" + aRecoverUrl + "'>click here</a>";
      mail.From = new MailAddress("no-replay@sag.no", "Umbraco");
      mail.To.Add(new MailAddress(email));
      await smtpClient.SendMailAsync(mail);
    }
  }
}


//string backOfficeSecurityProvider = UmbracoConfig.For.UmbracoSettings().Providers.DefaultBackOfficeUserProvider;
//      MembershipUser user = Membership.Providers[backOfficeSecurityProvider].GetUser(1, false);