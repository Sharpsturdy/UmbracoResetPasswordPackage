using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin.Models
{
  public class ResetPasswordModel
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    [Compare("PasswordConfirm")]
    [MinLength(4)]
    public string Password { get; set; }
    [Required]
    public string PasswordConfirm { get; set; }
    public string ResetToken { get;set;}
    public int    UserID { get;set;}
  }
}