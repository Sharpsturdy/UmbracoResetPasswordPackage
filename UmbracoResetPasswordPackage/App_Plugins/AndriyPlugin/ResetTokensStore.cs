﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin.Models;

namespace UmbracoResetPasswordPackage.App_Plugins.AndriyPlugin
{
  public class ResetTokensStore : DisposableObject
  {
    private readonly UmbracoDatabase _db;
    public ResetTokensStore()
    {
      _db = new UmbracoDatabase("umbracoDbDSN");
      if (!_db.TableExist("UserResetTokens"))
      {
        _db.CreateTable<ResetToken>();
      }
    }
    protected override void DisposeResources()
    {
      _db.Dispose();
    }

    public ResetToken CreateToken(int aUserID)
    {
      if (aUserID == 0) return null;
      string     randomToken = Path.GetRandomFileName().Replace(".", "");
      ResetToken token       = new ResetToken { UserID = aUserID, HashedToken = randomToken };
      try
      {
        if (IsTokenExist(aUserID))
        {
          _db.Update("UserResetTokens", "UserID", token);
        }
        else
        {
          _db.Insert("UserResetTokens", "UserID", token);
        }
      }
      catch (Exception)
      {
        return null;
      }
      return token;
    }
    public bool IsTokenCorrect(int aUserID, string aToken)
    {
      if (aUserID == 0 || string.IsNullOrEmpty(aToken))
      {
        return false;
      }
      var sqlCommand = new Sql().Select("*").From<ResetToken>().Where<ResetToken>(dto => dto.UserID == aUserID);
      IEnumerable<ResetToken> result = null;
      try
      {
        result = _db.Fetch<ResetToken>(sqlCommand);
      }
      catch (Exception)
      {
        return false;
      }
      if (result.Count() != 1)
      {
        return false;
      }
      if (result.First().HashedToken.Equals(aToken))
      {
        return true;
      }
      return false;
    }
    public bool IsTokenExist(int aUserID)
    {
      var sqlCommand = new Sql().Select("*").From<ResetToken>().Where<ResetToken>(dto => dto.UserID == aUserID);
      IEnumerable<ResetToken> result = null;
      try
      {
        result = _db.Fetch<ResetToken>(sqlCommand);
      }
      catch (Exception)
      {
        return false;
      }
      if (result.Count() != 1)
      {
        return false;
      }
      return true;
    }

    public void DeleteToken(int aUserID)
    {
      try
      {
        _db.Delete<ResetToken>(aUserID);
      }
      catch (Exception)
      {

      }
    }
  }
}