using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace tgbot
{
    class Program
    {
        public static TelegramBotClient bot = new TelegramBotClient("1832183491:AAGYaAlV7Ufa5geFUZWWZUgW2QzROdNoZRQ");
        static void Main(string[] args)
        {
            var url = "https://developerslife.ru/random";

            var web = new HtmlWeb();
            var doc = web.Load(url);



            bot.StartReceiving();

            bot.OnMessage += Bot_OnMessage;

            Console.ReadLine();

            bot.StopReceiving();
        }


        public static List<HtmlNode> GetInstitutes()
            {
                var url = "http://www.surgu.ru/instituty";
                var handler = new HttpClientHandler { AllowAutoRedirect = true };
                var httpClient = new HttpClient(handler);
                var web = new HtmlWeb();
                var doc = web.Load(url);
                var elements = doc.DocumentNode.Descendants("h2")
                                  .Where(x => x.Attributes["class"] != null)
                                  .Where(x => x.Attributes["class"].Value == "institute_name")
                                  .ToList();
                return elements;   
            }

        public static string GetInstituteInfo(int k, List<HtmlNode> Institutes)
        {
            string URL = Institutes[k - 1].ChildNodes[0].Attributes["href"].Value;
            URL = URL + "/obschaya-informatsiya";
            URL = "http://www.surgu.ru/" + URL;
            var handler = new HttpClientHandler { AllowAutoRedirect = true };
            var httpClient = new HttpClient(handler);
            var web = new HtmlWeb();
            var doc = web.Load(URL);

            var elements = doc.DocumentNode.Descendants("div")
                              .Where(x => x.Attributes["class"] != null)
                              .Where(x => x.Attributes["class"].Value == "top_margin")
                              .ToList();
            string InstituteInfo = "";
            foreach (var Child in elements[1].ChildNodes)
            {
                InstituteInfo = InstituteInfo + Child.InnerText + "\n";
            }
            return InstituteInfo;
        }


        public static void SendTelegramMessage4096 (string test, long id)
        {
            int k=0;
            if (test.Length>4096)
            {
                while (k<test.Length)
                {
                    if ((test.Length-k)>4096)
                        bot.SendTextMessageAsync(id, test.Substring(k, 4096));
                    else
                    {
                        Thread.Sleep(800);
                        bot.SendTextMessageAsync(id, test.Substring(k, test.Length - k));
                    }    
                        

                    k += 4096;
                } 
            } 
            else
            {
                bot.SendTextMessageAsync(id, test);
            }
        }



        public static void CheckMessage(Telegram.Bot.Args.MessageEventArgs e)
        {
            List<HtmlNode> Institutes=new List<HtmlNode>();
            Institutes = GetInstitutes();
            int i=0;
            string test = "";
            string InstitutesList = "";
            switch (e.Message.Text)
            {
                case "/institutes":

                    foreach (var institute in Institutes)
                    {
                        i++;
                        InstitutesList = InstitutesList + i.ToString() + ") " + institute.InnerText + "\n";
                    }

                    bot.SendTextMessageAsync(e.Message.Chat.Id, InstitutesList);
                    break;
                
                case "/institutesinfo":
                    foreach (var institute in Institutes)
                    {
                        i++;
                        InstitutesList = InstitutesList + i.ToString() + ") " + institute.InnerText + "\n";
                    }

                    bot.SendTextMessageAsync(e.Message.Chat.Id, InstitutesList);
                    bot.SendTextMessageAsync(e.Message.Chat.Id, "Укажите номер института для получения информации о нём");
 
                    break;

                case "1":
                    test = GetInstituteInfo(1, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);             
                    break;

                case "2":
                    test= GetInstituteInfo(2, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);
                    break;

                case "3":
                    test = GetInstituteInfo(3, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);
                    break;

                case "4":
                    test = GetInstituteInfo(4, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);
                    break;

                case "5":
                    test = GetInstituteInfo(5, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);
                    break;

                case "6":
                    test = GetInstituteInfo(6, Institutes);
                    SendTelegramMessage4096(test, e.Message.Chat.Id);
                    break;

            }
             
                
           
                   



        }




        private static void Bot_OnMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
        {
            var bot = sender as TelegramBotClient;

            CheckMessage(e);

        }
    }
}
