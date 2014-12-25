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

namespace SlackBotRedux.ConsoleDriver
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new RestClient(SlackConstants.SlackBaseApiUrl);
            var request = new RestRequest("/rtm.start", Method.POST);
            request.AddParameter("token", ConfigurationManager.AppSettings["BotApiToken"]);

            var response = client.Execute(request);
            var deserializer = new JsonDeserializer();
            var jsonResponse = deserializer.Deserialize<RtmStartResponse>(response);
        }
    }
}
