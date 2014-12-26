using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NLog;
using SlackBotRedux.Core.JsonHelpers;
using SlackBotRedux.Core.Models;
using WebSocket4Net;

namespace SlackBotRedux.Core
{
    /// <summary>
    /// An in-memory queue of messages received from Slack waiting to be processed.
    /// </summary>
    public interface IMessagePipeline
    {
        void EnqueueInputMessage(InputMessage msg);
        void EnqueueOutputMessage(OutputMessage msg);
        void BeginProcessing();
    }

    public class MessagePipeline : IMessagePipeline
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly WebSocket _websocket;
        private readonly BlockingCollection<InputMessage> _queueOfInputMessages;
        private readonly BlockingCollection<OutputMessage> _queueOfOutputMessages;

        private readonly JsonSerializerSettings _serializerSettings = new JsonSerializerSettings()
        {
            ContractResolver = new LowerCaseDelimitedPropertyNamesContractResolver('_')
        };

        public MessagePipeline(WebSocket websocket)
        {
            _websocket = websocket;
            _queueOfInputMessages = new BlockingCollection<InputMessage>();
            _queueOfOutputMessages = new BlockingCollection<OutputMessage>();
        }

        public void EnqueueInputMessage(InputMessage msg)
        {
            _queueOfInputMessages.Add(msg);
        }

        public void EnqueueOutputMessage(OutputMessage msg)
        {
            _queueOfOutputMessages.Add(msg);
        }

        public void BeginProcessing()
        {
            Task.Run(() => ProcessInputMessages());
            Task.Run(() => ProcessOutputMessages());
        }

        private void ProcessInputMessages()
        {
            foreach (var message in _queueOfInputMessages.GetConsumingEnumerable()) {
                Logger.Trace("Processing input message; Id: {0}; Text: {1}; Channel: {2}.", message.Text,
                    message.Channel);
            }
        }

        private void ProcessOutputMessages()
        {
            foreach (var message in _queueOfOutputMessages.GetConsumingEnumerable()) {
                Logger.Trace("Processing output message; Id: {0}; Text: {1}; Channel: {2}.", message.Id, message.Text,
                    message.Channel);

                _websocket.Send(JsonConvert.SerializeObject(message, Formatting.None, _serializerSettings));
            }
        }
    }
}
