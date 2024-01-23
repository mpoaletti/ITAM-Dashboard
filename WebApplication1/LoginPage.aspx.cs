using System;
using System.Web;
using System.Web.UI;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;

namespace WebApplication1
{
	public partial class LoginPage : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (Request.IsAuthenticated)
			{
				Session["Username"] = HttpContext.Current.GetOwinContext().Request.User.Identity.Name;
				Session["logInStatus"] = true;
				Response.Redirect("Default.aspx", false);
			}
			else Session["logInStatus"] = false;
		}

		protected void bttnLogin_Click(object sender, EventArgs e)
		{
			if (!Request.IsAuthenticated)
			{
				HttpContext.Current.GetOwinContext().Authentication.Challenge(
					new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectAuthenticationDefaults.AuthenticationType
					);
			}
		}
	}
}