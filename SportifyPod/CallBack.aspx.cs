using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SportifyPod
{
    public partial class CallBack : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Token token;
            string error = Request.QueryString["error"];
            if (error == null)
            {
                string accessToken = Request.QueryString["access_token"];
                string tokenType = Request.QueryString["token_type"];
                string expiresIn = Request.QueryString["expires_in"];
                if (tokenType != null)
                {

                    token = new Token
                    {
                        AccessToken = accessToken,
                        ExpiresIn = double.Parse(expiresIn),
                        TokenType = tokenType
                    };

                    SaveTokenFile(token);

                    Response.Redirect("~/Music/Index");

                }
            }
            else
            {
                token = new Token
                {
                    Error = error
                };
            }
            
            
        }

        private void SaveTokenFile(Token token)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(token);
            System.IO.File.WriteAllText(Server.MapPath("/SportifyToken.json"), jsonString);
            System.Threading.Thread.Sleep(1000);
        }
    }
}