using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using RestSharp;
using RestSharp.Deserializers;
using SlackBotRedux.Configuration;
using SlackBotRedux.Core.JsonHelpers;
using SlackBotRedux.Core.Models.Slack;
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
            ReceivingMessages
        }

        private class EventMessage
        {
            public EventType Type { get; set; }

            [JsonExtensionData]
            public JObject Data { get; set; } 
        }

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ManualResetEvent _waitEvent;
        private readonly string _slackBotApiToken;
        private readonly RestClient _restClient;
        private readonly JsonDeserializer _deserializer;
        private readonly JsonSerializerSettings _deserializerSettings = new JsonSerializerSettings() { Converters = new List<JsonConverter> { new EventTypeEnumConverter() } };
        
        private Status _state = Status.Initialized;
        
        private MessagePipeline _pipeline;
        private WebSocket _websocket;

        private readonly Bot _bot;

        public SlackClient(IBotConfiguration botConfig)
        {
            _waitEvent = new ManualResetEvent(false);
            _slackBotApiToken = botConfig.ApiToken;
            _restClient = new RestClient(SlackConstants.SlackBaseApiUrl);
            _deserializer = new JsonDeserializer();

            _bot = new Bot(botConfig.Name, _pipeline);
        }

        private void SetState(Status newStatus)
        {
            Logger.Trace("Transitioning from {0} to {1}.", _state, newStatus);
            _state = newStatus;
        }

        private void OnOpened(object sender, EventArgs args)
        {
            Logger.Trace("Connected.");

            SetState(Status.Connected);
        }

        private void OnError(object sender, ErrorEventArgs args)
        {
            Logger.Trace("Error received.", args.Exception);
        }

        private void OnClosed(object sender, EventArgs args)
        {
            Logger.Trace("Closed.");

            _waitEvent.Set();
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs args)
        {
            Logger.Trace("Received message: {0}", args.Message);

            var eventMsg = JsonConvert.DeserializeObject<EventMessage>(args.Message, _deserializerSettings);

            if (_state == Status.Connected) {
                if (eventMsg.Type == EventType.Hello) {
                    SetState(Status.ReceivingMessages);

                    _pipeline.BeginProcessing(_bot);
                }
                else if (eventMsg.Type == EventType.Error) {
                    // todo: add retry capability
                    var errorMsg = eventMsg.Data["error"]["msg"];
                    var errorCode = eventMsg.Data["error"]["code"];

                    Logger.Error("Error received when attempting to connect to RTM websocket server. Code = [{0}]; error message = [{1}].", errorCode, errorMsg);
                }
            }
            else if (_state == Status.ReceivingMessages) {
                if (eventMsg.Type == EventType.Message) {
                    var msg = JsonConvert.DeserializeObject<InputMessage>(args.Message, _deserializerSettings);

                    // ignore non-plain messages
                    if (msg.SubType != MessageSubType.PlainMessage) return;

                    _pipeline.EnqueueInputMessage(msg);
                }
                else if (eventMsg.Type == EventType.UserChange) {
                    var userChangeMsg = JsonConvert.DeserializeObject<UserChangeMessage>(args.Message,
                        _deserializerSettings);
                    var user = userChangeMsg.User;

                    _bot.ReceiveMessage(new UpdateUserBotMessage(user));
                }
            }
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

            _bot.ReceiveMessage(new UpdateTeamBotMessage(new TeamState() { UsersById = jsonResponse.Users.ToDictionary(u => u.Id) }));

            _websocket = new WebSocket(jsonResponse.Url);
            _pipeline = new MessagePipeline(_websocket);
            _websocket.Opened += OnOpened;
            _websocket.Error += OnError;
            _websocket.Closed += OnClosed;
            _websocket.MessageReceived += OnMessageReceived;
            _websocket.Open();

            _waitEvent.WaitOne();
            return true;
        }
    }
}
