using System;
using System.Collections.Generic;
using System.Linq;
using SlackBotRedux.Core.Data.Interfaces;
using SlackBotRedux.Core.Models.Slack;

namespace SlackBotRedux.Core.Data
{
    public class InMemoryRecentMessageRepository : IRecentMessageRepository
    {
        private readonly RecentMessageStore _msgStore;

        public InMemoryRecentMessageRepository(int maxMessagesStoredPerUser)
        {
            _msgStore = new RecentMessageStore(maxMessagesStoredPerUser);
        }

        public void AddNewMessage(InputMessageSlim msg)
        {
            _msgStore.AddMessageFromUser(msg.UserId, msg);
        }

        public IEnumerable<InputMessageSlim> GetRecentMessagesByUserId(string userId)
        {
            return _msgStore.GetRecentMessagesByUserId(userId);
        }
    }

    internal class RecentMessageStore
    {
        public int MaxMessagesStoredPerUser
        {
            get { return _maxMessagesStoredPerUser; }
        }

        private readonly IDictionary<string, Queue<InputMessageSlim>> _userToMessageQueueMap;
        private readonly int _maxMessagesStoredPerUser;

        public RecentMessageStore(int maxMessagesStoredPerUser)
        {
            _userToMessageQueueMap = new Dictionary<string, Queue<InputMessageSlim>>(StringComparer.InvariantCultureIgnoreCase);
            _maxMessagesStoredPerUser = maxMessagesStoredPerUser;
        }

        public void AddMessageFromUser(string userId, InputMessageSlim message)
        {
            if (!_userToMessageQueueMap.ContainsKey(userId))
            {
                _userToMessageQueueMap.Add(userId, new Queue<InputMessageSlim>(MaxMessagesStoredPerUser));
            }

            var usersMsgQueue = _userToMessageQueueMap[userId];
            while (usersMsgQueue.Count >= MaxMessagesStoredPerUser)
            {
                usersMsgQueue.Dequeue();
            }

            usersMsgQueue.Enqueue(message);
        }

        public IEnumerable<InputMessageSlim> GetRecentMessagesByUserId(string userId)
        {
            return !_userToMessageQueueMap.ContainsKey(userId)
                ? Enumerable.Empty<InputMessageSlim>()
                : _userToMessageQueueMap[userId].OrderBy(ims => ims.Timestamp);
        }
    }
}
