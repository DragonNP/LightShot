using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace LightShot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("1-Open image in browser; 2-Download image to folder: ");
            string type = Console.ReadLine();
            Console.WriteLine();

            if (type == "1")
                OpenBrowser();
            if (type == "2")
                DownloadImage();
        }

        public static void DownloadImage()
        {
            Console.Write("Please enter path to save images: ");
            string path_to_save = Console.ReadLine();

            Console.Write("\nEnter delay for save image (in seconds): ");
            int delay = Convert.ToInt32(Convert.ToDouble(Console.ReadLine()) * 1000);
            Console.WriteLine();

            while (true)
            {
                string random_symbols = "1";

                for (int i = 0; i < 5; i++)
                    random_symbols += Convert.ToString(GetLetter());

                string img_url = doSearchImage(random_symbols);
                DownloadImage(img_url, random_symbols, path_to_save);

                Console.WriteLine();
                Thread.Sleep(delay);
            }
        }

        public static void OpenBrowser()
        {
            Console.Write("Enter delay for open the browser (in seconds): ");
            int delay = Convert.ToInt32(Convert.ToDouble(Console.ReadLine()) * 1000);
            Console.WriteLine();

            while (true)
            {
                string random_url = "https://prnt.sc/1";

                for (int i = 0; i < 5; i++)
                    random_url += Convert.ToString(GetLetter());

                Console.WriteLine(random_url);
                OpenUrl(random_url);
                Thread.Sleep(delay);
            }
        }

        public static char GetLetter()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            Random rand = new Random();
            int num = rand.Next(0, chars.Length);
            return chars[num];
        }

        public static string doSearchImage(string symbols)
        {
            string url = "https://prnt.sc/" + symbols;
            Console.WriteLine("Random url: " + url);

            WebClient web_client = new WebClient();
            web_client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/95.0.4638.54 Safari/537.36 Edg/95.0.1020.30";

            string html = web_client.DownloadString(url);

            Regex r = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase);
            Match m = r.Match(html);

            string img_src = "";
            if (m.Success) img_src = (m.Groups[1].Value);

            return img_src;
        }

        public static void DownloadImage(string image_url, string symbols, string path_to_save)
        {
            if (image_url[0] == '/' && image_url[0] == '/') return;

            using (WebClient wc = new WebClient())
            {
                Console.WriteLine("Downloading image from url: " + image_url);
                wc.DownloadFile(
                    // Param1 = Link of file
                    new Uri(image_url),
                    // Param2 = Path to save
                    path_to_save + "\\"+ symbols + ".png"
                );
            }
        }

        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
