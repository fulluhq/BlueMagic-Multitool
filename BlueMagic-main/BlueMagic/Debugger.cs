using System;
using System.Diagnostics;

namespace CsStealer
{
    class Debugger
    {
        public static long StartRawTime;

        public static void Debug(string _Type, string Message)
        {
            string DebugLogsPath = Path.Combine(Config.OutputDir, "Debug.log");

            if (!File.Exists(DebugLogsPath))
            {
                string? CurrentFilePath;

                try
                {
                    CurrentFilePath = Process.GetCurrentProcess().MainModule.FileName;
                }
                catch
                {
                    CurrentFilePath = "--";
                }

                string HeadersText = $"{Credit.Header}[ WORKING ON : {Environment.MachineName} ]\n[ USER : {Environment.UserName} ]\n\n[ PAYLOAD PATH : {CurrentFilePath} ]\n\n[ START TIME : {DateTimeOffset.FromUnixTimeSeconds(StartRawTime)} ]";

                File.WriteAllText(DebugLogsPath, $"{HeadersText}\n\n[ {DateTime.Now} ] INFO    | CS Stealer started processing\n");
            }

            if (_Type.Trim() == "INFO") _Type = "INFO   ";
            else if (_Type.Trim() == "ERROR") _Type = "ERROR  ";

            File.AppendAllText(DebugLogsPath, $"[ {DateTime.Now} ] {_Type} | {Message}\n");
        }
    }
}