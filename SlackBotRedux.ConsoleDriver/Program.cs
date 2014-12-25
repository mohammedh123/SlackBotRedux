using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Deserializers;
using SlackBotRedux.Core;
using SlackBotRedux.Core.Models;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace SlackBotRedux.ConsoleDriver
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var client = new RestClient(SlackConstants.SlackBaseApiUrl);
            var request = new RestRequest("/rtm.start", Method.POST);
            request.AddParameter("token", ConfigurationManager.AppSettings["BotApiToken"]);

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var jsonResponse = deserializer.Deserialize<RtmStartResponse>(response);

            if (!jsonResponse.Ok) {
                Console.WriteLine("Error: " + jsonResponse.Error.ToString());

                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }

            var websocket = new WebSocket(jsonResponse.Url);
            websocket.Opened += OnOpened;
            websocket.Error += OnError;
            websocket.Closed += OnClosed;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Open();

            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }

        private static void OnOpened(object sender, EventArgs args)
        {
            Console.WriteLine("Connected.");
        }

        private static void OnError(object sender, ErrorEventArgs args)
        {
            Console.WriteLine("Error: {0}", args.Exception);
        }

        private static void OnClosed(object sender, EventArgs args)
        {
            Console.WriteLine("Closed.");
        }

        private static void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine("Received message: {0}", args.Message);
        }
    }
}
