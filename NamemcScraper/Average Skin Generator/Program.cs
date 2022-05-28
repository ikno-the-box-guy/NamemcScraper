using System;
using System.Net;
using System.IO;
using System.Threading;
using OpenQA.Selenium.Chrome;

namespace Average_Skin_Generator
{
    internal class Program
    {
        static void Main()
        {
            // Setup chrome driver
            Console.WriteLine("Enter chromedriver.exe path");
            String webDriverPath = Console.ReadLine();

            int page = 1;
            int skin = 1;
            int pageLimit;

            if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\skins"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\skins");

            Console.WriteLine("Enter the players namemc link. \nExample: \"https://nl.namemc.com/minecraft-skins/profile/IMakeSkins.1\" or \n\"https://nl.namemc.com/minecraft-skins/trending/monthly\" \nMake sure everything is in there but not ?page=, that will be added by the program.");
            string player = Console.ReadLine();
            Console.WriteLine("Enter max amount of pages you can download. Enter -1 for no limit.");
            pageLimit = int.Parse(Console.ReadLine());
            if (pageLimit == -1) pageLimit = int.MaxValue;

            WebClient webClient = new WebClient(); // Image downloader
            do
            {
                var browser = new ChromeDriver(webDriverPath);
                browser.Manage().Window.Size = new System.Drawing.Size(0, 0);
                Thread.Sleep(100);
                browser.Navigate().GoToUrl(player + "?page=" + page);
                String htmlString = browser.PageSource;
                Console.WriteLine(htmlString);
                if (!htmlString.Contains("<a href=\"/skin/")) return; // If we reached the end of a page
                while (htmlString.Contains("<a href=\"/skin/"))
                {
                    //get all the skins on the current page
                    int pFrom = htmlString.IndexOf("<a href=\"/skin/") + "<a href=\"/skin/".Length;
                    String result = htmlString.Substring(pFrom, 16);

                    Console.WriteLine(result);

                    byte[] dataArr = webClient.DownloadData("https://s.namemc.com/i/" + result + ".png");
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\skins\\skin" + skin + ".png", dataArr);

                    htmlString = htmlString.Substring(pFrom);
                    skin++;
                }

                browser.Close();

                Console.WriteLine("Going to next page!");
                page++;
            } while (page <= pageLimit);
            Console.WriteLine("Finished downloading.");
        }
    }
}
