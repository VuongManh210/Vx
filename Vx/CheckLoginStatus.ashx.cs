using System;
using System.Web;
using System.Web.SessionState;

namespace Vx
{
    public class CheckLoginStatus : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            bool isLoggedIn = context.Session["UserId"] != null;
            context.Response.Write("{\"isLoggedIn\": " + isLoggedIn.ToString().ToLower() + "}");
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}