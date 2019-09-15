using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SportifyPod.Controllers
{
    public class MusicController : Controller
    {
        public string ClientId
        {
            get {
                string _clientId = string.Empty;
                if (ConfigurationManager.AppSettings["SpotifyClientId"] != null)
                    _clientId = ConfigurationManager.AppSettings["SpotifyClientId"].ToString();
                return _clientId;
            }   
        }

        public string SecretId
        {
            get {
                string _secretId = string.Empty;
                if (ConfigurationManager.AppSettings["SpotifySecretId"] != null)
                    _secretId = ConfigurationManager.AppSettings["SpotifySecretId"].ToString();
                return _secretId;
            }
        }

        public string SpotifyCallbackUrl
        {
            get {
                string _callbakUrl = string.Empty;
                if (ConfigurationManager.AppSettings["SpotifyCallbackUrl"] != null)
                    _callbakUrl = ConfigurationManager.AppSettings["SpotifyCallbackUrl"].ToString();
                return _callbakUrl;
            }
        }


        SpotifyWebAPI api = null;
        private SpotifyWebAPI ClientApi
        {
            get {
                string tokenType = "Bearer";

                string authUrl = GetAuthUri(ClientId, "token", SpotifyCallbackUrl, "34fFs29kd09", false);
                Token token = GetLocalToken();

                if (token == null || token.IsExpired())
                {
                    Response.Redirect(authUrl);
                    //Response.Write("<script type='text/javascript'>window.open('" + authUrl + "');</script>");
                }
                if (api == null)
                api = new SpotifyWebAPI
                        {
                            AccessToken = token.AccessToken,
                                TokenType = tokenType
                        };

                return api;
            }
        }

        // GET: Music
        public async Task<ActionResult> Index()
        {                               

            string authUrl = GetAuthUri(ClientId, "token", SpotifyCallbackUrl, "34fFs29kd09", false);
            Token token = GetLocalToken();
            if (token == null || token.IsExpired())
            {
                Response.Redirect(authUrl);
                return View();
            }

            PrivateProfile userProfile = await ClientApi.GetPrivateProfileAsync();
            return View(userProfile);
        }

        [HttpGet]
        public async Task<ActionResult> MusicRecom()
        {
            PrepareNewReleaseAlbumList();

            PrepareTopArtistList();

            List<SimpleTrack> _list = new List<SimpleTrack>();
            var model = _list;
            return View(model);
        }

        private void PrepareNewReleaseAlbumList()
        {
            NewAlbumReleases data = ClientApi.GetNewAlbumReleases("SE");
            List<SelectListItem> _List = new List<SelectListItem>();
            SelectListItem item = null;
            foreach (var album in data.Albums.Items)
            {
                item = new SelectListItem();
                item.Text = album.Name;
                item.Value = album.Id;
                _List.Add(item);
            }
            _List.Insert(0, new SelectListItem() { Text = "Please select...", Value = "-1" });

            ViewData["NewRelease"] = _List;
        }

        private void PrepareTopArtistList()
        { 

            Paging<FullArtist> topArtist = ClientApi.GetUsersTopArtists();
            SelectListItem item = null;
            List<SelectListItem> _List = new List<SelectListItem>();
            foreach (var artist in topArtist.Items)
            {
                item = new SelectListItem();
                item.Text = artist.Name;
                item.Value = artist.Id;
                _List.Add(item);
            }

            ViewData["TopArtist"] = _List;
        }

        [HttpGet]
        [OutputCache(Duration = 0)]
        public async Task<ActionResult> GetRecommendationList(string trackSeedId, string artistSeedId)
        {
          
            List<string> artistSeed = new List<string>() { artistSeedId };
            List<string> trackSeed = new List<string>() { trackSeedId };  

            Recommendations recomList = await ClientApi.GetRecommendationsAsync(artistSeed, null, trackSeed, null, null, null);
            var model = recomList.Tracks;
            
            return PartialView("RecommendationList", model);

        }

        [HttpPost]
        public JsonResult GetTracks(string albumId)
        {          
            Paging<SimpleTrack> list = ClientApi.GetAlbumTracks(albumId);            
            return Json(list.Items);
        }

        public string GetArtistName(SimpleTrack track)
        {
            string result = string.Empty;
            foreach (var artist in track.Artists)
            {
                result += artist.Name + ", ";
            }
            return result;
        }



        public ActionResult CallBack()
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
            return View();
        }

        private void SaveTokenFile(Token token)
        {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(token);
            System.IO.File.WriteAllText(Server.MapPath("/SportifyToken.json"), jsonString);
            System.Threading.Thread.Sleep(1000);
        }

        public string GetAuthUri(string clientId, string type, string redirectUri, string state, bool showDialog)
        {
            StringBuilder builder = new StringBuilder("https://accounts.spotify.com/authorize?");
            builder.Append("client_id=" + clientId);
            builder.Append($"&response_type={type}");
            builder.Append("&redirect_uri=" + HttpUtility.UrlEncode(redirectUri));
            builder.Append("&state=" + state);
            builder.Append("&scope=" + "user-read-private%20user-read-email%20user-library-read%20user-top-read");
            builder.Append("&show_dialog=" + showDialog.ToString());            
            return builder.ToString();
        }

        public Token GetLocalToken()
        {
            Token token = null;
            string tokenFile = Server.MapPath("/SportifyToken.json");
            string tokenContent = string.Empty;

            if (System.IO.File.Exists(tokenFile))
            {
                tokenContent = System.IO.File.ReadAllText(tokenFile);
                token = Newtonsoft.Json.JsonConvert.DeserializeObject<Token>(tokenContent);
            }

            return token;


        }
    }
}