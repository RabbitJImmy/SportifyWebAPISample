﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace SpotifyAPI.Web.Models
{
    public class SimpleTrack : BasicModel
    {
        [JsonProperty("artists")]
        public List<SimpleArtist> Artists { get; set; }
        public string ArtistsName {
            get {
                string result = string.Empty;
                foreach (SimpleArtist artist in Artists)
                {                    
                    result += artist.Name + ",";
                }
                if (result.EndsWith(","))
                    result = result.Substring(0, result.Length - 1);

                return result;
            }
        }

        [JsonProperty("available_markets")]
        public List<string> AvailableMarkets { get; set; }

        [JsonProperty("disc_number")]
        public int DiscNumber { get; set; }

        [JsonProperty("duration_ms")]
        public int DurationMs { get; set; }

        public string DurationText
        {
            get {
                TimeSpan span = TimeSpan.FromMilliseconds(DurationMs);
                return span.ToString(@"mm\:ss");

            }
        }

        [JsonProperty("explicit")]
        public bool Explicit { get; set; }

        [JsonProperty("external_urls")]
        public Dictionary<string, string> ExternUrls { get; set; }

        [JsonProperty("href")]
        public string Href { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("preview_url")]
        public string PreviewUrl { get; set; }

        [JsonProperty("track_number")]
        public int TrackNumber { get; set; }

        [JsonProperty("restrictions")]
        public Dictionary<string, string> Restrictions { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("uri")]
        public string Uri { get; set; }
    }
}