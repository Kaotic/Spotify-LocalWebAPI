/*-----------------------------------------
 * API Spotify v1.0.1.1060.gc75ebdfd
 * Created by Kaotic --- http://kaotic.fr/
 * File created date: 14-03-15 - 05:52
 * -----------------------------------------
 * Use:
 * SpotifyAPI API = new SpotifyAPI();
 * API.Pause();
 * API.Resume();
 * API.Play(URI);
 * 
 */﻿

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Forms;

namespace RemoteControlServer
{
    class SpotifyAPI
    {
        private const string ua = @"Mozilla/5.0 (Windows NT 6.3; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.111 Safari/537.36";
        private const string org = "https://open.spotify.com";
        private const string port = ":4380";

        private static string oauthToken;
        private static string csrfToken;

        public SpotifyAPI()
        {
            if (Process.GetProcessesByName("SpotifyWebHelper").Length < 1){
                try{
                    System.Diagnostics.Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Spotify\\Data\\SpotifyWebHelper.exe");
                }catch (Exception dd){
                    MessageBox.Show("Aucune installation de Spotify n'a était trouvé. Cette fonctionnalité ne sera pas supporté.");
                    throw new Exception("Could not launch SpotifyWebHelper. You not have Spotify installed", dd);
                }
            }else{
                if (oauthToken == null || oauthToken == "null")
                {
                    SetOAuth();
                }
                if (csrfToken == null || csrfToken == "null")
                {
                    SetCSRF();
                }
            }

        }

        public void Play(string uri)
        {
                string a = GetSecurePage(URL("remote/play.json?uri=" + uri), true);
        }

        public void Resume()
        {
                string a = GetSecurePage(URL("remote/pause.json?pause=false"), true);
        }

        public void Pause()
        {
                string a = GetSecurePage(URL("remote/pause.json?pause=true"), true);
        }

        /*public Responses.Status Status
        {
            get
            {
                string a = GetSecurePage(GetURL("remote/status.json"), false);
                List<Responses.Status> d = (List<Responses.Status>)JsonConvert.DeserializeObject(a, typeof(List<Responses.Status>));
                return d[0];
            }
        }*/

        private static string URL(string path)
        {
            return "http://kaotic.spotilocal.com:4380/" + path;
        }

        private static string GetSecurePage(string URL, bool parse)
        {
            try
            {
                WebClient w = new WebClient();
                w.Headers.Add("user-agent", ua);
                w.Headers.Add("Origin", org);
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                if (parse == true) { return w.DownloadString(URL + "&csrf=" + csrfToken + "&oauth=" + oauthToken); } else { return w.DownloadString(URL + "?csrf=" + csrfToken + "&oauth=" + oauthToken); }
            }
            catch (Exception e)
            {
                return "Error: " + e;
            }
            

        }

        private static string GetPage(string URL)
        {
            try { 
            WebClient p = new WebClient();
            p.Headers.Add("user-agent", ua);
            p.Headers.Add("Origin", org);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            byte[] bytes = Encoding.Default.GetBytes(p.DownloadString(URL));
            return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception e)
            {
                return "ERROR";
            }
        }

        private static void SetCSRF()
        {
            String url = URL("simplecsrf/token.json");
            String csrf = GetPage(url);
            JObject o = JObject.Parse(csrf);
            csrfToken = "" + o["token"];
        }

        private static void SetOAuth()
        {

            string raw = GetPage("https://open.spotify.com/token");
            JObject t = JObject.Parse(raw);
            oauthToken = "" + t["t"];

        }

    }
}

