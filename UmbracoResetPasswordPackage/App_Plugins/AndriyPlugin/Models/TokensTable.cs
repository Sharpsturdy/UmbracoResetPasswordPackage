using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin.Models
{
  [TableName("UserResetTokens")]
  [ExplicitColumns]
  [PrimaryKey("UserID")]
  public class ResetToken
  {
    [Column("ID")]
    [PrimaryKeyColumn(Name = "PK_ID")]
    public int ID { get;set;}

    [Column("UserID")]
    public int UserID { get; set; }
    
    [Column("Token")]
    [NullSetting(NullSetting=NullSettings.NotNull)]
    public string HashedToken { get; set; }
  }
}