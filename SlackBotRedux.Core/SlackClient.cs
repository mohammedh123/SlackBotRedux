using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using RestSharp;
using RestSharp.Deserializers;
using SlackBotRedux.Core.Models;
using SuperSocket.ClientEngine;
using WebSocket4Net;

namespace SlackBotRedux.Core
{
    public interface ISlackClient
    {
        /// <summary>
        /// Initializes a real-time messaging session and idles, eventually returning when the connection closes.
        /// </summary>
        /// <returns><b>true</b> if the session was started successfully; <b>false</b> otherwise.</returns>
        bool Start();
    }

    public class SlackClient : ISlackClient
    {
        private enum Status
        {
            Initialized,
            Connected,
            HelloReceived,
            ReceivingMessages
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ManualResetEvent _waitEvent;
        private readonly string _slackBotApiToken;
        private readonly RestClient _restClient;
        
        private Status _state = Status.Initialized;

        public SlackClient(string slackBotApiToken)
        {
            _waitEvent = new ManualResetEvent(false);
            _slackBotApiToken = slackBotApiToken;
            _restClient = new RestClient(SlackConstants.SlackBaseApiUrl);
        }

        private void SetState(Status newStatus)
        {
            
        }

        private void OnOpened(object sender, EventArgs args)
        {
            Console.WriteLine("Connected.");

            _state = Status.Connected;
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            Console.WriteLine("Error: {0}", args.Exception);
        }

        private void OnClosed(object sender, EventArgs args)
        {
            Console.WriteLine("Closed.");

            _waitEvent.Set();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Console.WriteLine("Received message: {0}", args.Message);
        }

        public bool Start()
        {
            var request = new RestRequest(SlackConstants.RtmStartAbsolutePath, Method.POST);
            request.AddParameter("token", _slackBotApiToken);

            var response = _restClient.Execute(request);
            var deserializer = new JsonDeserializer();
            var jsonResponse = deserializer.Deserialize<RtmStartResponse>(response);

            if (!jsonResponse.Ok) {
                Logger.Error("Error received when attempting to initialize an RTM session: {0}.",
                    jsonResponse.Error.ToString());

                return false;
            }

            var websocket = new WebSocket(jsonResponse.Url);
            websocket.Opened += OnOpened;
            websocket.Error += OnError;
            websocket.Closed += OnClosed;
            websocket.MessageReceived += OnMessageReceived;
            websocket.Open();

            _waitEvent.WaitOne();
            return true;
        }
    }
}
