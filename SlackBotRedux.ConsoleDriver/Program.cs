using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
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
            var slackClient = new SlackClient(ConfigurationManager.AppSettings["BotApiToken"]);
            slackClient.Start();
        }
    }
}
