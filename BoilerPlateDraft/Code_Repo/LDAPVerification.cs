//Defaults added during class generation
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//additional references added for class

//for LDAP connection and checking
using System.DirectoryServices.AccountManagement;

//for adding cookie value to web page
using System.Web.Security;
using System.Net;
using System.Web.UI;

namespace BoilerPlateDraft.Code_Repo
{
	public class LDAPVerification
	{
    //variable to store the users username
    public static string ldapUserName;


    //if valid user add cookie to system
    private void ValidateUserAddCookie() {

      //first validate user
      //if(loginVerified(un, pw)) {};
      //or
      //if(loginVerified(un, pw, grp)) {};


      FormsAuthenticationTicket tkt;
      string cookiestr;
      HttpCookie cookie;
      tkt = new FormsAuthenticationTicket(1, ldapUserName, DateTime.Now, DateTime.Now.AddMinutes(30), false, "");
      cookiestr = FormsAuthentication.Encrypt(tkt);
      cookie = new HttpCookie(FormsAuthentication.FormsCookieName, cookiestr);
      cookie.Path = FormsAuthentication.FormsCookiePath;
      //Code taken from WebForm that allows Response to be used.  
      //Provides Error in current boilerPlate app configuration
      //Response.Cookies.Add(cookie);
      
    }

    //Function verifies if credentials are accurate against LDAP
    private bool loginVerified(string un, string pw)
    {
      //uses current users windows credentials for domain
      PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
      //Find user
      UserPrincipal ldapUser = UserPrincipal.FindByIdentity(ctx, un);

      if (ldapUser != null)
      {
        //check user authentication to verify logged in
        if (ctx.ValidateCredentials(un, pw) != true)
        {
          //invalid credentials
          return false;
        }
        else if (ldapUser.IsAccountLockedOut() == true)
        {
          //user's account is locked
          return false;
        }
        else if (ldapUser.AccountExpirationDate != null)
        {
          //User's account is expired
          return false;
        }
        else
        {
          return true;
        }
      }
      //user is null
      else return false;
    }

    //check login credentials as well as group membership
    private bool loginVerified(string un, string pw, string group)
    {
      //uses current users windows credentials for domain
      PrincipalContext ctx = new PrincipalContext(ContextType.Domain);
      //Find user
      UserPrincipal ldapUser = UserPrincipal.FindByIdentity(ctx, un);

      if (ldapUser != null)
      {
        //check user authentication to verify logged in
        if (ctx.ValidateCredentials(un, pw) != true)
        {
          //invalid credentials
          return false;
        }
        else if (ldapUser.IsAccountLockedOut() == true)
        {
          //user's account is locked
          return false;
        }
        else if (ldapUser.AccountExpirationDate != null)
        {
          //User's account is expired
          return false;
        }
        else
        {
          //Find the group
          GroupPrincipal grp = GroupPrincipal.FindByIdentity(ctx, group);
          if (ldapUser.IsMemberOf(grp)) return true;
          else return false;
        }
      }
      //user is null
      else return false;
    }

  }
}