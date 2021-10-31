using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Threading;

namespace Average_Skin_Generator
{
    internal class Program
    {
        static void Main()
        {
            int page = 1;
            int skin = 1;
            int pageLimit = int.MaxValue;

            if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\skins"))
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\skins");

            Console.WriteLine("Enter the players namemc link. \nExample: \"https://nl.namemc.com/minecraft-skins/profile/IMakeSkins.1\" or \n\"https://nl.namemc.com/minecraft-skins/trending/monthly\" \nMake sure everything is in there but not ?page=, that will be added by the program.");
            string player = Console.ReadLine();
            Console.WriteLine("Enter max amount of pages you can download. Enter -1 for no limit.");
            pageLimit = int.Parse(Console.ReadLine());
            if (pageLimit == -1) pageLimit = int.MaxValue;

            while (true)
            {
                HttpWebRequest client = (HttpWebRequest)WebRequest.Create(player + "?page=" + page);
                client.UseDefaultCredentials = true;
                client.Accept = "text/html";
                client.UserAgent = Guid.NewGuid().ToString();
                client.Method = "GET";

                HttpWebResponse response = (HttpWebResponse) client.GetResponse();

                string htmlString;
                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    htmlString = reader.ReadToEnd();
                    if (!htmlString.Contains("<a href=\"/skin/")) return;
                }

                Console.WriteLine(htmlString);

                WebClient webClient = new WebClient();

                while (htmlString.Contains("<a href=\"/skin/"))
                {
                    //get all the skins on the current page
                    int pFrom = htmlString.IndexOf("<a href=\"/skin/") + "<a href=\"/skin/".Length;
                    String result = htmlString.Substring(pFrom, 16);

                    Console.WriteLine(result);

                    byte[] dataArr = webClient.DownloadData("https://i.n-mc.co/" + result + ".png");
                    File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\skins\\skin" + skin + ".png", dataArr);

                    htmlString = htmlString.Substring(pFrom);
                    skin++;
                }

                Console.WriteLine("Going to next page!");
                page++;

                webClient.Dispose();
                response.Close();

                if (page > pageLimit) return; 
            }
        }
    }
}
